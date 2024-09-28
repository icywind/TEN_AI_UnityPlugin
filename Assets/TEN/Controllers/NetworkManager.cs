using Agora.TEN.Server.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace Agora.TEN.Client
{
    public static class NetworkManager
    {
        /// <summary>
        /// Request the server to generate a token based on channel and uid.
        /// The caller should handle the exception from this networking action.
        /// </summary>
        /// <param name="uid">The user ID.</param>
        /// <returns>RTC token string</returns>
        public static async Task<string> RequestTokenAsync(uint uid)
        {
            /// AppConfig.Shared.Channel = "agora_astratest";
            // Get the shared AppConfig instance
            var config = AppConfig.Shared;

            // Create an AgoraRTCTokenRequest object with a unique request ID, channel name, and user ID
            var data = new AgoraRTCTokenRequest
            {
                RequestId = GetUUID(),
                ChannelName = config.Channel,
                Uid = uid
            };

            // Construct the API endpoint URL
            var endpoint = $"{config.ServerBaseURL}/token/generate";

            // Make a server API request and decode the response
            using (var httpClient = new HttpClient())
            {
                var responseString = await ServerApiRequest(endpoint, data);

                var decoded = JsonConvert.DeserializeObject<AgoraRTCTokenResponse>(responseString);

                // Return the token from the decoded response
                return decoded.Data.Token;
            }
        }


        public static async Task<string> ApiRequestStartService(uint uid)
        {
            // Get the shared AppConfig instance
            var config = AppConfig.Shared;

            // Create a ServiceStartRequest object with request ID, channel name, OpenAI proxy URL, remote stream ID, graph name, voice type, and start properties
            var data = new ServiceStartRequest
            {
                RequestId = GetUUID(),
                ChannelName = config.Channel,
                UserUid = uid,
                GraphName = config.GraphName, // e.g.  "camera.va.openai.azure",
                Properties = config.AgentProperties
            };

            // Construct the API endpoint URL
            var endpoint = $"{config.ServerBaseURL}/start";

            // Make a server API request and return the response data
            using (var httpClient = new HttpClient())
            {
                var responseString = await ServerApiRequest(endpoint, data);

                // Return the token from the decoded response
                return responseString;
            }
        }

        /// <summary>
        /// Stops the service by sending a request to the server.
        /// </summary>
        /// <returns>The data returned by the server.</returns>
        /// <exception cref="Exception">Thrown if the request fails.</exception>
        public static async Task<string> ApiRequestStopService()
        {
            // Get the shared AppConfig instance
            var config = AppConfig.Shared;

            // Create a ServiceStopRequest object with request ID and channel name
            var data = new ServiceStopRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                ChannelName = config.Channel
            };

            // Construct the API endpoint URL
            var endpoint = $"{config.ServerBaseURL}/stop";

            // Make a server API request and return the response data
            using (var httpClient = new HttpClient())
            {
                var responseString = await ServerApiRequest(endpoint, data);

                // Return the token from the decoded response
                return responseString;
            }
        }


        /// <summary>
        /// Sends a ping request to the server to check service status.
        /// </summary>
        /// <returns>The data returned by the server.</returns>
        /// <exception cref="Exception">Thrown if the request fails.</exception>
        public static async Task<string> ApiRequestPingService()
        {
            // Get the shared AppConfig instance
            var config = AppConfig.Shared;

            // Create a ServicePingRequest object with request ID and channel name
            var data = new ServicePingRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                ChannelName = config.Channel
            };

            // Construct the API endpoint URL
            var endpoint = $"{config.ServerBaseURL}/ping";

            // Make a server API request and return the response data
            using (var httpClient = new HttpClient())
            {
                var responseString = await ServerApiRequest(endpoint, data);

                // Return the token from the decoded response
                return responseString;
            }
        }

        public static async Task<string> ServerApiRequest(string url, object data)
        {
            var json = JsonConvert.SerializeObject(data);

            Debug.Log("API Sending data:" + json);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            request.Timeout = 90000;

            using (var postStream = new StreamWriter(request.GetRequestStream()))
            {
                postStream.Write(json);
            }

            using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonResponse = reader.ReadToEnd();
                Debug.Log(url + " -------> " + jsonResponse);
                return jsonResponse;
            }
        }

        static string testStartJSON = @"{
    ""request_id"": ""a9634d7c-74ad-41ff-a1b1-2b3370a70e0e"",
    ""channel_name"": ""agora_astratest"",
    ""user_uid"": 149597,
    ""graph_name"": ""camera.va.openai.azure"",
    ""language"": ""en-US"",
    ""properties"": {
        ""agora_rtc"": {
            ""agora_asr_language"": ""en-US""
        },
        ""openai_chatgpt"": {
            ""model"": ""gpt-4o"",
            ""greeting"": ""ASTRA agent connected. How can i help you today?"",
            ""checking_vision_text_items"": ""[\""Let me take a look...\"",\""Let me check your camera...\"",\""Please wait for a second...\""]""
        },
        ""azure_tts"": {
            ""azure_synthesis_voice_name"": ""en-US-JennyNeural""
        }
    }
    }";

        public static async Task<string> SampleApiRequest()
        {
            AppConfig.Shared.Channel = "agora_astratest";
            string url = $"{AppConfig.Shared.ServerBaseURL}/start";
            var json = testStartJSON;

            Debug.Log("API Sending data:" + json);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            request.Timeout = 90000;

            using (var postStream = new StreamWriter(request.GetRequestStream()))
            {
                postStream.Write(json);
            }

            using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonResponse = reader.ReadToEnd();
                Debug.Log(url + " -------> " + jsonResponse);
                return jsonResponse;
            }
        }


        internal static string GetUUID()
        {
            return Guid.NewGuid().ToString();
        }
    }

}
