using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Service.SMSHelperService
{
    public class SMSHelperServices : ISMSHelperRepository
    {
        private const string LoginUrl = "https://bsms.hutch.lk/api/login";
        private const string Username = "chandika@x365.io";
        private const string Password = "Savi@2023";
        private static readonly HttpClient _httpClient = new HttpClient();
        private string _accessToken;
        private string _refreshToken;
        private DateTime _accessTokenExpiry;
        private static readonly object _tokenLock = new();
        public SMSHelperServices()
        {

            // Set default headers for HttpClient
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-API-VERSION", "v1");
        }

        public async Task<(bool IsSuccess, string ServerRef)> SendSMSAsync(string campaignName, int customerId, string message, string phoneNumber)
        {
            //var customerDetails = await _apiService.GetSmsGatewayDetailsByCustomerIdAsync(customerId);
            //if (customerDetails == null)
            //{
            //    Console.WriteLine("❌ Customer gateway details not found.");
            //    return (false, null);
            //}

            try
            {
                // Obtain access token by logging in
                string accessToken = await LoginAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    Console.WriteLine("❌ Failed to retrieve a valid access token.");
                    return (false, null);
                }

                string formattedPhoneNumber = FormatPhoneNumber(phoneNumber);

                var payload = new
                {
                    campaignName,
                    mask = "Christell",
                    numbers = formattedPhoneNumber.Trim(),
                    deliveryReportRequest = true,
                    content = message
                };

                string jsonPayload = JsonConvert.SerializeObject(payload);
                string smsBaseURL = "https://bsms.hutch.lk/api/sendsms";

                using (var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json"))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response = await _httpClient.PostAsync(smsBaseURL, content).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        dynamic responseData = JsonConvert.DeserializeObject(responseContent);
                        string serverRef = responseData.serverRef;
                        Console.WriteLine($"✅ SMS sent successfully! ServerRef: {serverRef}");
                        return (true, serverRef);
                    }
                    else
                    {
                        Console.WriteLine($"❌ SMS send failed. Status Code: {response.StatusCode}, Response: {await response.Content.ReadAsStringAsync()}");
                        return (false, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error sending SMS: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"🔍 Inner Exception: {ex.InnerException.Message}");

                return (false, null);
            }
        }

        private async Task<string> LoginAsync()
        {
            try
            {
                var loginPayload = new
                {
                    username = Username,
                    password = Password
                };

                string json = JsonConvert.SerializeObject(loginPayload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await _httpClient.PostAsync(LoginUrl, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"❌ Login failed. Status: {response.StatusCode}, Response: {errorContent}");
                        return null;
                    }

                    string responseJson = await response.Content.ReadAsStringAsync();
                    var tokenData = JsonConvert.DeserializeObject<TokenResponse>(responseJson);

                    if (tokenData == null || string.IsNullOrEmpty(tokenData.AccessToken))
                    {
                        Console.WriteLine("❌ Login succeeded but token data was missing or invalid.");
                        return null;
                    }

                    // Store token details
                    _accessToken = tokenData.AccessToken;
                    _refreshToken = tokenData.RefreshToken;
                    _accessTokenExpiry = DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn);

                    Console.WriteLine("✅ Login successful. Access token acquired.");
                    return tokenData.AccessToken;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Exception during login: {ex.Message}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during login: {ex}");
            }

            return null;
        }

        public async Task<bool> SendOTPAsync(int customerId, string phoneNumber, string otp)
        {
            string message = $"🔒 Secure Access Code: {otp} 🚀 Use this OTP to proceed, but keep it secret—it's just for you!";
            var (isSent, _) = await SendSMSAsync("OTP Service", customerId, message, phoneNumber);
            return isSent;
        }

        private static string FormatPhoneNumber(string phoneNumber)
        {
            if (!phoneNumber.StartsWith("94") && phoneNumber.StartsWith("0"))
            {
                return "94" + phoneNumber.TrimStart('0');
            }
            return phoneNumber.Trim();
        }

        private async Task<string> GetValidAccessTokenAsync()
        {
            // Check if current token is missing or expired
            if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _accessTokenExpiry)
            {
                Console.WriteLine("🔁 Access token expired or missing. Attempting to refresh...");

                if (!string.IsNullOrEmpty(_refreshToken))
                {
                    var newToken = await RefreshAccessTokenAsync();
                    if (newToken != null)
                    {
                        _accessToken = newToken.AccessToken;
                        _refreshToken = newToken.RefreshToken;
                        _accessTokenExpiry = DateTime.UtcNow.AddSeconds(newToken.ExpiresIn);
                        Console.WriteLine("✅ Token refreshed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Refresh token failed. Trying full login...");
                        var loginToken = await LoginAsync();
                        if (loginToken == null)
                        {
                            Console.WriteLine("❌ Failed to obtain new token from login.");
                            return null;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("🔑 No refresh token. Logging in...");
                    var loginToken = await LoginAsync();
                    if (loginToken == null)
                    {
                        Console.WriteLine("❌ Login failed.");
                        return null;
                    }
                }
            }

            return _accessToken;
        }

        private async Task<TokenResponse> RefreshAccessTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://bsms.hutch.lk/api/login/api/token/accessToken");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _refreshToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokenResponse>(content);
        }

        public class TokenResponse
        {
            [JsonProperty("accessToken")]
            public string AccessToken { get; set; }

            [JsonProperty("refreshToken")]
            public string RefreshToken { get; set; }

            [JsonProperty("expiresIn")]
            public int ExpiresIn { get; set; } // typically in seconds
        }
    }
}
