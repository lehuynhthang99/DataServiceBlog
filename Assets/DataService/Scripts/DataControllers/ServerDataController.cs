using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Nura.DataServiceBlog
{
    /// <summary>
    /// use this to implement data that is crawled from HTTP/HTTPS
    /// </summary>
    /// <typeparam name="TDataModel"></typeparam>
    public abstract class ServerDataController<TDataModel> : DataController<TDataModel> where TDataModel : class, new()
    {
        protected const string BASE_URL = "https://example.com";
        protected const int MAX_RETRY_COUNT = 3;

        [SerializeField] protected string _path;

        protected string _fullPath;

        public string GetFullPath()
        {
            if (string.IsNullOrEmpty(_fullPath))
            {
                _fullPath = $"{BASE_URL}{_path}";
            }

            return _fullPath;
        }
        protected abstract UnityWebRequest GetRequest();

        protected override async UniTask LoadData(CancellationToken cancelToken)
        {
            try
            {
                bool isSuccess = false;
                string lastError = null;
                string txtResult = null;

               //retry if the loading process is error
                for (int i = 0; i < MAX_RETRY_COUNT; i++)
                {
                    UnityWebRequest webRequest = GetRequest();
                    var result = await webRequest.SendWebRequest();
                    cancelToken.ThrowIfCancellationRequested();

                   //check the result status
                    switch (result.result)
                    {
                        case UnityWebRequest.Result.ConnectionError:
                        case UnityWebRequest.Result.DataProcessingError:
                            lastError = result.error;
                            isSuccess = false;
                            break;
                        case UnityWebRequest.Result.ProtocolError:
                            lastError = result.error;
                            isSuccess = false;
                            break;
                        case UnityWebRequest.Result.Success:
                            isSuccess = true;
                            txtResult = result.downloadHandler.text;
                            Debug.Log($"Loading data received: {result.downloadHandler.text}");
                            break;
                    }

                   //break the loop is success
                    if (isSuccess)
                    {
                        break;
                    }
                }

                if (isSuccess)
                {
                    TDataModel result = JsonConvert.DeserializeObject<TDataModel>(txtResult);
                    _data = result;
                }
                else
                {
                    throw new Exception($"{this.GetType()} is error: {lastError}");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
