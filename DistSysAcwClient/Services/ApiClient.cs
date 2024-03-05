using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DistSysAcwClient.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> TalkBackAsync()
        {
            // Send a GET request to the TalkBack endpoint.
            var response = await _httpClient.GetAsync("api/talkback/hello");

            response.EnsureSuccessStatusCode(); // This will throw an exception

            return await response.Content.ReadAsStringAsync();
        }


        public async Task<string> SortIntegersAsync(int[] integers)
        {
            // Construct the query string from the array of integers
            var queryString = string.Join("&", integers.Select(i => $"integers={i}"));

            // Send a GET request to the TalkBack Sort endpoint with the query string
            var response = await _httpClient.GetAsync($"Api/TalkBack/sort?{queryString}");

            response.EnsureSuccessStatusCode(); // This will throw an exception 

            // Read the response as a string 
            return await response.Content.ReadAsStringAsync();
        }
      
        
        public async Task<string> GetUserAsync(string username)
        {
            // Construct the query string with the username parameter
            var queryString = $"username={Uri.EscapeDataString(username)}";

            // Send a GET request to the user endpoint with the query string
            var response = await _httpClient.GetAsync($"api/user/new?{queryString}");

            response.EnsureSuccessStatusCode(); // This will throw an exception 

            // Read the response as a string 
            return await response.Content.ReadAsStringAsync();
        }

        


        public async Task<string> CreateUserAsync(string username)
        {
            var json = JsonConvert.SerializeObject(username); // This will add the necessary quotes
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send a POST request to the user endpoint with the username in the request body
            var response = await _httpClient.PostAsync("api/user/new", content);

            response.EnsureSuccessStatusCode(); // This will throw an exception 
            var responseContent = await response.Content.ReadAsStringAsync();

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Environment.SetEnvironmentVariable("USERNAME", username);
                Environment.SetEnvironmentVariable("API_KEY", responseContent);
               
                Console.WriteLine("Got API Key");
            }

            return responseString;
        }



        public void SetUserCredentials(string username, string apiKey)
        {
            // Set environment variables
            Environment.SetEnvironmentVariable("API_KEY", apiKey);
            Environment.SetEnvironmentVariable("USERNAME", username);
            Console.WriteLine("Stored");
        }


        public async Task<bool> DeleteUserAsync()
        {
            string username = Environment.GetEnvironmentVariable("USERNAME");
            string apiKey = Environment.GetEnvironmentVariable("API_KEY");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("You need to do a User Post or User Set first.");
                return false;
            }

            // Set up the request to include the API key in the header
            _httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);


            // Construct the URL with the username
            string url = $"api/user/removeuser?username={Uri.EscapeDataString(username)}";

            // Send the DELETE request
            var response = await _httpClient.DeleteAsync(url);

            // Check the response status code and return true if the delete was successful
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("User deleted successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("Failed to delete user.");
                return false;
            }
        }
        public async Task<HttpResponseMessage> ChangeUserRoleAsync(string jsonPayload, string apiKey)
        {
            // Set the API key in the header.
            _httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

            // Create the HTTP content with the JSON payload.
            HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Send the POST request.
            return await _httpClient.PostAsync("api/User/ChangeRole", content);
        }

        public async Task<HttpResponseMessage> GetProtectedHelloAsync(string apiKey)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);
            return await _httpClient.GetAsync("api/protected/hello");
        }

        public async Task<HttpResponseMessage> GetSha1HashAsync(string message, string apiKey)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);
            return await _httpClient.GetAsync($"api/protected/sha1?message={Uri.EscapeDataString(message)}");
        }
        public async Task<HttpResponseMessage> GetSha256HashAsync(string message, string apiKey)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);
            return await _httpClient.GetAsync($"api/protected/sha256?message={Uri.EscapeDataString(message)}");
        }


        public async Task<string> GetPublicKeyAsync()
        {
            string apiKey = Environment.GetEnvironmentVariable("API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("You need to do a User Post or User Set first.");
                return null;
            }

            // Clear any existing headers to avoid conflicts
            _httpClient.DefaultRequestHeaders.Clear();

            // Add the API key to the headers
            _httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

            try
            {
                var response = await _httpClient.GetAsync("api/protected/getpublickey");

                if (response.IsSuccessStatusCode)
                {
                    string publicKeyXml = await response.Content.ReadAsStringAsync();
                    Environment.SetEnvironmentVariable("PUBLIC_KEY", publicKeyXml, EnvironmentVariableTarget.User);
                    Console.WriteLine("Got Public Key");
                    return publicKeyXml;
                }
                else
                {
                    Console.WriteLine("Couldn't Get the Public Key");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving public key: {ex.Message}");
                return null;
            }
        }




    }


}
