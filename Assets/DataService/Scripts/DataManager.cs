using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Nura.DataServiceBlog
{
    public class DataManager : MonoBehaviour
    {
        private const int MAX_RETRY_COUNT = 3;
        private static DataManager _instance;
        public static DataManager Instance { get => _instance; }
        public static bool IsAlive => _instance;


        [SerializeField] private DataController[] _controllers;

        private Dictionary<Type, DataController> _typeToControllerMap;


        private void Awake()
        {
            if (_instance)
            {
                Debug.LogError("Already has an instance");
                Destroy(gameObject);
                return;
            }

            _instance = this;

            _typeToControllerMap = new Dictionary<Type, DataController>(_controllers.Length);
            foreach (var controller in _controllers)
            {
                _typeToControllerMap[controller.GetType()] = controller;
            }
        }

        private void OnDestroy()
        {
            if (this == _instance)
            {
                _instance = null;
            }
        }

        public void InitData()
        {
            foreach (var controller in _controllers)
            {
                controller.InitData();
            }
        }

        public async UniTask LoadData()
        {
            List<UniTask> loadingTasks = new List<UniTask>(_controllers.Length);

            bool isSuccess = false;

            //retry in case the process is error
            for (int i = 0; i < MAX_RETRY_COUNT; i++)
            {
                CancellationTokenSource cancellation = new CancellationTokenSource();

                //start loading data
                try
                {
                    loadingTasks.Clear();
                    foreach (var controller in _controllers)
                    {
                        UniTask loadingTask = controller.StartLoadData(cancellation.Token);
                        loadingTasks.Add(loadingTask);
                    }

                    await UniTask.WhenAll(loadingTasks).AttachExternalCancellation(cancellation.Token);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    //there is an error. Cancel the loading process and notify other data controller if there are still loading
                    Debug.LogError($"Loading process is error: {ex.GetBaseException()}\n{ex.StackTrace}");
                    cancellation.Cancel();
                    cancellation.Dispose();
                    cancellation = null;

                    //TODO: handle exception here
                }

                if (isSuccess)
                {
                    break;
                }
                else
                {
                    await UniTask.Delay(2000);
                }
            }

            if (isSuccess)
            {
                Debug.Log("Loading data is success");
            }
            else
            {
                //TODO: handle the error here
            }

        }

        public TDataController GetDataController<TDataController>() where TDataController : DataController
        {
            try
            {
                return _typeToControllerMap[typeof(TDataController)] as TDataController;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Can't find data controller of type {typeof(TDataController)}: {ex.GetBaseException()}\n{ex.StackTrace}");
                return null;
            }
        }
    }
}
