using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nura.DataServiceBlog
{
    public abstract class DataController : ScriptableObject
    {
        public abstract UniTask LoadData();
        public abstract void InitData();
    }


    public abstract class DataController<TDataModel> : DataController where TDataModel : class, new()
    {
        protected TDataModel _data;

        public override void InitData()
        {
            _data = new TDataModel();
        }
    }
}