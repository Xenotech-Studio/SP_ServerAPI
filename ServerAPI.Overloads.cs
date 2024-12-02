using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSystem.Http
{
    public partial class ServerAPI
    {
        // Convert tool from callback style to async style
        public static void ToCallback<T>(Task<T> task, Action<T> callback)
        {
            Task.Run(async () =>
            {
                T result = await task;
                callback?.Invoke(result);
            });
        }
        
        
        // GET request in callback style
        public static void Get<T>(string url, Action<T> callback) where T : class => Get(url, null, callback);
        public static void Get<T>(string url, Dictionary<string, string> query, Action<T> callback) where T : class => ToCallback(Get<T>(url, query), callback);
        
        
        // POST request in callback style
        public static void Post<T1, T2>(string url, T1 data, Action<T2> callback) where T1 : class where T2 : class => Post(url, null, data, callback);
        public static void Post<T1, T2>(string url, Dictionary<string, string> query, T1 data, Action<T2> callback) where T1 : class where T2 : class => ToCallback(Post<T1, T2>(url, query, data), callback);
        
        
    }
}