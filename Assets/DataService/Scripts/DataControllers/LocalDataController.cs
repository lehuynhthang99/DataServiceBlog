using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using UnityEngine;
using System.IO;
using System.Threading;


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

        protected override UniTask LoadData(CancellationToken cancelToken)
        {
            try
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

                        _data = result;
                    }
                    catch (Exception ex)
                    {
                       //don't throw exception here. Most of the time, when data can't be loaded, it means the data is corrupted.
                       //so loading more time won't change the result here
                       //in that case, just keep the default data
                        Debug.LogError($"Load data -- {filePath} -- is error: {ex.GetBaseException()}\n{ex.StackTrace}");
                    }
                }

                return UniTask.CompletedTask;
            }
            catch (Exception ex)
            {
               //just try catch for sure
               //if the code goes into this, it means there is an critical error in the logic codes
                throw;
            }
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
