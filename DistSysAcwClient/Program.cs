using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DistSysAcwClient.Services;

#region Task 10 and beyond
class Program
{
    static async Task Main(string[] args)
    {
        HttpClient httpClient = new HttpClient
        {
            // Use the appropriate base address depending on your environment
           // BaseAddress = new Uri("https://localhost:44394/")
             BaseAddress = new Uri("http://150.237.94.9/9689818/")
        };

        ApiClient apiClient = new ApiClient(httpClient);
        Console.WriteLine("Hello. What would you like to do?");

        string nextInput = Console.ReadLine(); // Get the initial input

        while (true)
        {
            string input = nextInput;


            if (string.IsNullOrWhiteSpace(input) || input.Equals("Exit", StringComparison.OrdinalIgnoreCase))
            {
                break; // Exit the loop and terminate the program
            }
            Console.WriteLine("...please wait...");
            try
            {
                // Determine the command type
                string command = input.Split(' ')[0]; // Get the first word as the command

                switch (command)
                {
                    case "TalkBack":
                        await HandleTalkBackCommand(input, apiClient);
                        break;
                    case "User":
                        await HandleUserCommand(input, apiClient);
                        break;
                    case "Protected":
                        await HandleProtectedCommand(input, apiClient);
                        break;
                    default:
                        Console.WriteLine("Command not recognized.");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input: please check your command format and try again.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            Console.WriteLine("What would you like to do next?");
            nextInput = Console.ReadLine(); // Store the next command for processing in the next iteration
            Console.Clear();

        }
    }

    static async Task HandleTalkBackCommand(string input, ApiClient apiClient)
    {
        string subCommand = input.Split(' ')[1]; // Get the second word as the sub-command
        switch (subCommand)
        {
            case "Hello":
                string helloResponse = await apiClient.TalkBackAsync();
                Console.WriteLine(helloResponse);
                break;
            case "Sort":
                string integersString = input.Substring("TalkBack Sort".Length).Trim();
                integersString = integersString.Trim('[', ']');
                var integers = integersString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                             .Select(int.Parse)
                                             .ToArray();
                string sortResponse = await apiClient.SortIntegersAsync(integers);
                Console.WriteLine($"Sorted Integers: {sortResponse}");
                break;
            default:
                Console.WriteLine("TalkBack command not recognized.");
                break;
        }
    }

    static async Task HandleUserCommand(string input, ApiClient apiClient)
    {
        string subCommand = input.Split(' ')[1]; // Get the second word as the sub-command
        string parameter = input.Substring(input.IndexOf(subCommand) + subCommand.Length).Trim();
        switch (subCommand)
        {
            case "Get":
                string getResponse = await apiClient.GetUserAsync(parameter);
                Console.WriteLine(getResponse);
                break;

            case "Post":
                await apiClient.CreateUserAsync(parameter);
               
                break;

                case "Set":
                string[] parts = input.Split(' ');
                if (parts.Length == 4)
                {
                    string username = parts[2];
                    string apiKey = parts[3];
                    apiClient.SetUserCredentials(username, apiKey);
                   
                }
                else
                {
                    Console.WriteLine("Invalid input for User Set command.");
                }
                break;

            case "Delete":
                bool isDeleted = await apiClient.DeleteUserAsync();
                Console.WriteLine(isDeleted ? "True" : "False");
                break;
            case "Role":
                await HandleChangeRoleCommand(input, apiClient);
                break;
            default:
                Console.WriteLine("User command not recognized.");
                break;
        }
    }
    static async Task HandleProtectedCommand(string input, ApiClient apiClient)
    {
        string[] parts = input.Split(' ');
        if (parts.Length < 2)
        {
            Console.WriteLine("Invalid protected command.");
            return;
        }
        string protectedCommand = parts[1];

        switch (protectedCommand)
        {
            case "Hello":
                await HandleProtectedHello(apiClient);
                break;

            case "SHA1":
                // The SHA1 command requires additional parameters, so pass the full input
                await HandleSha1Command(input, apiClient);
                break;
            case "SHA256":
                await HandleSha256Command(input, apiClient);
                break;
            case "Get": 
                string publicKey = await apiClient.GetPublicKeyAsync();
               
               
                break;
            default:
                Console.WriteLine("Protected command not recognized.");
                break;
        }
    }
    static async Task HandleChangeRoleCommand(string input, ApiClient apiClient)
    {
        string[] parts = input.Split(' ');
        if (parts.Length < 3)
        {
            Console.WriteLine("Invalid input for User ChangeRole command.");
            return;
        }

        string usernameToChange = parts[2]; // Assuming the username to change role is the third part of the input
        string newRole = parts[3]; // Assuming the new role is the fourth part of the input

        // Retrieve the API key from the environment.
        string apiKey = Environment.GetEnvironmentVariable("API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("You need to do a User Post or User Set first.");
            return;
        }

        // Construct the JSON payload.
        var payload = new
        {
            username = usernameToChange,
            role = newRole
        };

        // Convert the payload to a JSON string.
        string jsonPayload = JsonConvert.SerializeObject(payload);

        // Send the ChangeRole request.
        HttpResponseMessage response = await apiClient.ChangeUserRoleAsync(jsonPayload, apiKey);

        // Check the response.
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
        }
        else
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Failed to change user role. Status code: {response.StatusCode}, Content: {errorContent}");
        }
    }
    static async Task HandleProtectedHello(ApiClient apiClient)
    {
        string apiKey = Environment.GetEnvironmentVariable("API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("You need to do a User Post or User Set first.");
            return;
        }

        HttpResponseMessage response = await apiClient.GetProtectedHelloAsync(apiKey);
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
        }
        else
        {
            Console.WriteLine($"Failed to get protected hello. Status code: {response.StatusCode}");
        }
    }

   static async Task HandleSha1Command(string input, ApiClient apiClient)
    {
        string[] parts = input.Split(' ');
        if (parts.Length < 3)
        {
            Console.WriteLine("Invalid input for Sha1 command.");
            return;
        }

        string message = parts[2];
        string apiKey = Environment.GetEnvironmentVariable("API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("You need to do a User Post or User Set first.");
            return;
        }

        HttpResponseMessage response = await apiClient.GetSha1HashAsync(message, apiKey);
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
        }
        else
        {
            Console.WriteLine($"Failed to get SHA1 hash. Status code: {response.StatusCode}");
        }
    }
    static async Task HandleSha256Command(string input, ApiClient apiClient)
    {
        string[] parts = input.Split(' ');
        if (parts.Length < 3)
        {
            Console.WriteLine("Invalid input for Sha256 command.");
            return;
        }

        string message = parts[2];
        string apiKey = Environment.GetEnvironmentVariable("API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("You need to do a User Post or User Set first.");
            return;
        }

        HttpResponseMessage response = await apiClient.GetSha256HashAsync(message, apiKey);
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
        }
        else
        {
            Console.WriteLine($"Failed to get SHA256 hash. Status code: {response.StatusCode}");
        }
    }
}

#endregion