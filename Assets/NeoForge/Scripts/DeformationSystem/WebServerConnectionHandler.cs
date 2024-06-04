using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace DeformationSystem
{
    public static class WebServerConnectionHandler
    {
        /// TODO: Move off localhost
        private const string SERVER_URL = "http://127.0.0.1:5000";

        /// <summary>
        /// The data returned from the server after the last request finished.
        /// </summary>
        public static string ReturnData { get; private set; }

        /// <summary>
        /// Will send a request to a predefined server URL with the given data.
        /// </summary>
        /// <param name="data">The json file to be sent</param>
        public static IEnumerator SendPutRequest(string data, string endpoint = "/")
        {
            string url = SERVER_URL + endpoint;
            using var request = UnityWebRequest.Put(url, data);
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            
            Debug.Assert(request.result == UnityWebRequest.Result.Success, "Error: " + request.error);
            ReturnData = request.result != UnityWebRequest.Result.Success ? data : request.downloadHandler.text;
        }
        public static IEnumerator SendGetRequest(string endpoint = "/")
        {
            string url = SERVER_URL + endpoint;
            using var request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            Debug.Assert(request.result == UnityWebRequest.Result.Success, "Error: " + request.error);
            ReturnData = request.result != UnityWebRequest.Result.Success ? "" : request.downloadHandler.text;
        }
    }
}