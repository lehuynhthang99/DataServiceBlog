using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Nura.DataServiceBlog
{
    public enum DataLoadState
    {
        None = 0,
        Loading = 1,
        Error = 2,
        Loaded = 3,
    }

    public abstract class DataController : ScriptableObject
    {
        [NonSerialized] private DataLoadState lastDataLoadState;

        protected DataLoadState LastDataLoadState => lastDataLoadState;

        public async UniTask StartLoadData(CancellationToken cancelToken)
        {
            try
            {
               //data is loaded, don't need to reload
                if (lastDataLoadState == DataLoadState.Loaded)
                {
                    return;
                }

               //wait for the previous process of this data to complete
                if (lastDataLoadState == DataLoadState.Loading)
                {
                    await UniTask.WaitUntil(() => lastDataLoadState != DataLoadState.Loading, cancellationToken: cancelToken);
                    cancelToken.ThrowIfCancellationRequested();
                }

               //start loading
                lastDataLoadState = DataLoadState.Loading;
                
                await LoadData(cancelToken);
                cancelToken.ThrowIfCancellationRequested();
                
                lastDataLoadState = DataLoadState.Loaded;
            }
            catch (Exception ex)
            {
               //log the error and throw to the upper handler
                Debug.LogError($"Loading data is error: {ex.Message}\n{ex.GetBaseException()}\n{ex.StackTrace}");
                lastDataLoadState = DataLoadState.Error;
                throw;
            }
        }

        protected abstract UniTask LoadData(CancellationToken cancelToken);
        public abstract void InitData();
        public void ClearData()
        {
            InitData();
            lastDataLoadState = DataLoadState.None;
        }
    }


    public abstract class DataController<TDataModel> : DataController where TDataModel : class, new()
    {
        [SerializeField] protected TDataModel _data;

        public override void InitData()
        {
            _data = new TDataModel();
        }
    }
}