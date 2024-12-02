using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace DataSystem.Http
{

    public partial class ServerAPI
    {
        public static string Hostname = "114.132.240.173:9200/api";
        
        
        // GET request in async style
        public static async Task<T> Get<T>(string api) where T : class => await Get<T>(api, new Dictionary<string, string>());
        public static async Task<T> Get<T>(string api, Dictionary<string, string> query) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 构建查询参数
                    string queryString = string.Empty;
                    if (query != null && query.Count > 0)
                    {
                        List<string> queryParams = new List<string>();
                        foreach (var param in query)
                        {
                            queryParams.Add($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}");
                        }

                        queryString = "?" + string.Join("&", queryParams);
                    }

                    // 构建完整URL
                    string url = $"http://{Hostname}/{api}/{queryString}";

                    // 发送GET请求
                    HttpResponseMessage response = await client.GetAsync(url);

                    // 检查请求是否成功
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // 反序列化响应数据
                        T result = JsonConvert.DeserializeObject<T>(responseBody);
                        return result;
                    }
                    else
                    {
                        Debug.LogError($"Error: {response.StatusCode}, {response.ReasonPhrase}");
                        return null;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request error: {e.Message}");
                return null;
            }
        }



        public static async Task<T2> Post<T1, T2>(string api, T1 data) where T1 : class where T2 : class => await Post<T1, T2>(api, null, data);
        public static async Task<T2> Post<T1, T2>(string api, Dictionary<string, string> query, T1 data)
            where T1 : class where T2 : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 构建查询参数
                    string queryString = string.Empty;
                    if (query != null && query.Count > 0)
                    {
                        List<string> queryParams = new List<string>();
                        foreach (var param in query)
                        {
                            queryParams.Add($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}");
                        }

                        queryString = "?" + string.Join("&", queryParams);
                    }

                    // 构建完整URL
                    string url = $"http://{Hostname}/{api}/{queryString}";

                    // 序列化请求体数据
                    string jsonData = JsonConvert.SerializeObject(data);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    // 发送POST请求
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    // 检查请求是否成功
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // 反序列化响应数据
                        T2 result = JsonConvert.DeserializeObject<T2>(responseBody);
                        return result;
                    }
                    else
                    {
                        Debug.LogError($"Error: {response.StatusCode}, {response.ReasonPhrase}");
                        return null;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request error: {e.Message}\n{e.StackTrace}");
                return null;
            }
        }
    }
    
    // When passing this class as http's result, you are assuming nothing is useful in the request result
    // Json: {}
    public class DefaultJson { }
    
    // When passing this class as http's result, you are assuming the request is a list of T
    // Json: { "message": "something", "results": [ T, T, T ... ] }
    public class ListResult<T>
    {
        [JsonProperty("message")] public string Message;
        [JsonProperty("results")] public List<T> Results = new List<T>();
    }
}