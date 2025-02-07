using Cysharp.Threading.Tasks;
using Nura.Client.ExampleFeature;
using Nura.DataServiceBlog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nura.Client
{
    public class Client : MonoBehaviour
    {
        private void Start()
        {
            DataManager.Instance.InitData();
            DataManager.Instance.LoadData().Forget();
        }

        [ContextMenu("TryGetData")]
        private void TryGetData()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            ExampleDataController exampleDataController = DataManager.Instance.GetDataController<ExampleDataController>();
            Debug.Log(exampleDataController.GetVersion());
        }        
        
        [ContextMenu("SetData")]
        private void SetData()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            ExampleDataController exampleDataController = DataManager.Instance.GetDataController<ExampleDataController>();
            exampleDataController.SetVersion(Random.Range(0, 100));
            Debug.Log(exampleDataController.GetVersion());
        }
    }
}