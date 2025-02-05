using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nura.DataServiceBlog
{
    public class DataManager : MonoBehaviour
    {
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

        public UniTask LoadData()
        {
            List<UniTask> loadingTasks = new List<UniTask>(_controllers.Length);

            foreach (var controller in _controllers)
            {
                UniTask loadingTask = controller.LoadData();
                loadingTasks.Add(loadingTask);
            }

            return UniTask.WhenAll(loadingTasks);

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