using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using UnityEngine;
using System.IO;


namespace Nura.DataServiceBlog
{
    /// <summary>
    /// use this to implement data that is crawled from local storage
    /// </summary>
    /// <typeparam name="TDataModel"></typeparam>
    public abstract class LocalDataController<TDataModel> : DataController<TDataModel> where TDataModel : class, new()
    {
        [SerializeField] protected string _filePath;

        protected string _fullPath;

        public string GetFullPath()
        {
            if (string.IsNullOrEmpty(_fullPath))
            {
                _fullPath = $"{Application.persistentDataPath}/{_filePath}.dat";
            }

            return _fullPath;
        }

        public override UniTask LoadData()
        {
            var filePath = GetFullPath();

            TDataModel result = null;

            if (File.Exists(filePath))
            {
                try
                {
                    var savedData = File.ReadAllText(filePath);
                    result = JsonConvert.DeserializeObject<TDataModel>(savedData);
                    Debug.Log($"LoadData complete {filePath}\n{savedData}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Load data -- {filePath} -- is error: {ex.GetBaseException()}\n{ex.StackTrace}");
                }
            }

            if (result == null)
            {
                result = new TDataModel();
            }

            return UniTask.CompletedTask;
        }
        public void SaveData()
        {
            var filePath = GetFullPath();

            try
            {
                var saveData = JsonConvert.SerializeObject(_data);
                File.WriteAllText(filePath, saveData);

                Debug.Log($"SaveData complete {filePath}\n{saveData}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Save data -- {this.GetType()} -- is error: {ex.GetBaseException()}\n{ex.StackTrace}");
            }
        }

    }

}
