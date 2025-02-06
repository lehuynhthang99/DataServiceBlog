using Nura.DataServiceBlog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nura.Client.ExampleFeature
{
    [CreateAssetMenu(menuName = "DataController/ExampleDataController", fileName = "ExampleDataController")]
    public class ExampleDataController : LocalDataController<ExampleDataModel>
    {
        public int GetVersion()
        {
            return _data.Version;
        }

        public void SetVersion(int version)
        {
            _data.Version = version;
            SaveData();
        }

        public string GetName()
        {
            return _data.Name;
        }

        public void SetName(string newName)
        {
            _data.Name = newName;
            SaveData();
        }
    }
}