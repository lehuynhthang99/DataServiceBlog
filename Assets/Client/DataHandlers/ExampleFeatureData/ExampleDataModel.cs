using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nura.Client.ExampleFeature
{
    [Serializable]
    public class ExampleDataModel
    {
        [JsonProperty][SerializeField] private int version;
        [JsonProperty][SerializeField] private string name;

        [JsonIgnore] public int Version { get => version; set => version = value; }
        [JsonIgnore]public string Name { get => name; set => name = value; }
    }
}