using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DeformationSystem
{
    public class Testing : MonoBehaviour
    {
        private const string serverURL = "http://127.0.0.1:5000/multiply-vertices"; // Change this to your server URL

        public Data x;
        
        private string returnData;
        public string ReturnData => returnData;

        [System.Serializable]
        public class Data
        {
            public float E = 2000000;
        }

        public IEnumerator SendDebug()
        {
            var startTime = Time.realtimeSinceStartup;

            using (UnityWebRequest request = UnityWebRequest.Put("http://127.0.0.1:5000/run-demo", JsonUtility.ToJson(x)))
            {
                request.SetRequestHeader("Content-Type", "application/json");

                // Send the request
                yield return request.SendWebRequest();

                // Check for errors
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + request.error);
                }
                else
                {
                    // Print the response
                    Debug.Log("Response: " + request.downloadHandler.text);
                }
                
                returnData = request.result != UnityWebRequest.Result.Success ? "" : request.downloadHandler.text;
            }
            
            Debug.Log($"Time taken: {Time.realtimeSinceStartup - startTime}");
            Debug.Log(returnData);
        }
        
        [ContextMenu("RUN JAX")]
        public void StartTest()
        {
            StartCoroutine(SendDebug());
        }

        public IEnumerator SendRequest(string data)
        {
            // Create the request
            using (UnityWebRequest request = UnityWebRequest.Put(serverURL, data))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                Debug.Log(data);

                // Send the request
                yield return request.SendWebRequest();

                // Check for errors
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + request.error);
                }
                else
                {
                    // Print the response
                    Debug.Log("Response: " + request.downloadHandler.text);
                }
                
                returnData = request.result != UnityWebRequest.Result.Success ? data : request.downloadHandler.text;
            }
        }
    }
}