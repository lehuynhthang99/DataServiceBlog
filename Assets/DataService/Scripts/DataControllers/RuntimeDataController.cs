using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
namespace Nura.DataServiceBlog
{
    /// <summary>
    /// use this for data that can only be crawled when running game
    /// for example: native device information
    /// </summary>
    /// <typeparam name="TDataModel"></typeparam>
    public abstract class RuntimeDataController<TDataModel> : DataController<TDataModel> where TDataModel : class, new()
    {
    }
}

