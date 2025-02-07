using Cysharp.Threading.Tasks;
using Nura.DataServiceBlog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Nura.Client.ShopFeature
{
    public class ShopDataController : DataController
    {
        [SerializeField] private LocalShopDataController _localShopDataController;
        [SerializeField] private FakeServerShopDataController _serverShopDataController;
        public override void InitData()
        {
            _localShopDataController.InitData();
            _serverShopDataController.InitData();
        }

        protected override UniTask LoadData(CancellationToken cancelToken)
        {
            try
            {
                UniTask localDataLoadingTask = _localShopDataController.StartLoadData(cancelToken);
                UniTask serverDataLoadingTask = _serverShopDataController.StartLoadData(cancelToken);

                return UniTask.WhenAll(localDataLoadingTask, serverDataLoadingTask).AttachExternalCancellation(cancelToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string FakeGetName()
        {
            return _localShopDataController.GetName() + _serverShopDataController.GetName();
        }

    }
}