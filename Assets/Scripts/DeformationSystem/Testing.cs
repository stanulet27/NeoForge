using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DeformationSystem
{
    public class Testing : MonoBehaviour
    {
        private const string serverURL = "http://127.0.0.1:5000/multiply-vertices"; // Change this to your server URL

        [ContextMenu("Test")]
        public void CallServer()
        {
            StartCoroutine(SendRequest("name"));
        }

        public IEnumerator SendRequest(string data)
        {
            // Create the request
            using (UnityWebRequest request = UnityWebRequest.PostWwwForm(serverURL, data))
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
            }
        }

        [System.Serializable]
        public class RequestData
        {
            
        }
    }
}