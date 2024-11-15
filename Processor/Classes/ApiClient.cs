using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinraEDSProcessor.Classes
{
    public class ApiClient
    {
        private readonly ProfileConfig _profile;

        // Cached token and expiration
        private string _cachedAccessToken;
        private DateTime _tokenExpiration;

        public ApiClient(ProfileConfig config)
        {
            _profile = config;
        }

        /// <summary>
        /// Get basic auth header
        /// </summary>
        /// <returns></returns>
        private string GetBasicAuthHeader()
        {
            string credentials = $"{_profile.ClientId}:{_profile.ClientSecret}";
            byte[] byteCredentials = Encoding.UTF8.GetBytes(credentials);
            return Convert.ToBase64String(byteCredentials);
        }

        /// <summary>
        /// Get access token
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<string> GetAccessTokenAsync()
        {
            try
            {
                // Check if cached token is still valid
                if (!string.IsNullOrEmpty(_cachedAccessToken) && DateTime.UtcNow < _tokenExpiration)
                {
                    return _cachedAccessToken;
                }

                // Build the token request URL with `grant_type=client_credentials`
                string tokenRequestUri = $"{_profile.TokenEndpointUri}";

                // Create a HttpWebRequest for the POST request
                var request = (HttpWebRequest)WebRequest.Create(tokenRequestUri);
                request.Method = "POST";

                // Set the Basic Authorization header
                request.Headers["Authorization"] = $"Basic {GetBasicAuthHeader()}";

                // Specify the Content-Type as application/x-www-form-urlencoded
                request.ContentType = "application/x-www-form-urlencoded";

                // Use Task.Run to make this call asynchronously
                var response = await Task.Run(() => (HttpWebResponse)request.GetResponse());

                // Read the response
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseBody = await reader.ReadToEndAsync();
                    var AccessTokenResponse = JsonSerializer.Deserialize<AccessTokenResponse>(responseBody);

                    // Parse the expires_in field and cache the token
                    if (AccessTokenResponse != null && int.TryParse(AccessTokenResponse.expires_in, out int expiresInSeconds))
                    {
                        _cachedAccessToken = AccessTokenResponse.access_token;
                        _tokenExpiration = DateTime.UtcNow.AddSeconds(expiresInSeconds);
                    }

                    return _cachedAccessToken;
                }
            }
            catch (WebException ex)
            {
                throw new ApplicationException("Error obtaining access token: " + GetWebExceptionMessage(ex), ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error obtaining access token: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Search API (POST with Payload)
        /// </summary>
        /// <param name="apiBaseURI"></param>
        /// <param name="apiEndPoint"></param>
        /// <param name="payLoad"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<ApiResponseIndividualSearch> GetDataAsyncIndividualSearchd(string apiBaseURI, string apiEndPoint, Object payLoad)
        {
            try
            {
                // Get the access token (from cache if valid)
                string token = await GetAccessTokenAsync();


                // Serialize the payload to JSON
                string jsonPayload = JsonSerializer.Serialize(payLoad);


                // Create a HttpWebRequest for the API GET request
                string apiRequestUri = apiBaseURI + apiEndPoint;
                var request = (HttpWebRequest)WebRequest.Create(apiRequestUri);
                request.ContentType = "application/json";
                request.Method = "POST";


                // Write the JSON payload to the request stream
                using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
                {
                    streamWriter.Write(jsonPayload);
                }

                // Set the Bearer token in the Authorization header
                request.Headers["Authorization"] = $"Bearer {token}";

                // Use Task.Run to make this call asynchronously
                var response = await Task.Run(() => (HttpWebResponse)request.GetResponse());

                // Read the response
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseBody = await reader.ReadToEndAsync();
                    var ApiResponseIndividualSearch = JsonSerializer.Deserialize<ApiResponseIndividualSearch>(responseBody);

                    if (ApiResponseIndividualSearch == null)
                    {
                        throw new Exception("Failed to deserialize API response.");
                    }

                    return ApiResponseIndividualSearch;
                }
            }
            catch (WebException ex)
            {
                throw new ApplicationException("Error during API data retrieval: " + GetWebExceptionMessage(ex), ex);
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("Error deserializing API response: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error during data retrieval: " + ex.Message, ex);
            }
        }

        public async Task<ApiResponseIndvValidation> GetDataAsyncGetIndv(string apiBaseURI, string apiEndPoint, string indvCrdId)
        {
            try
            {
                // Get the access token (from cache if valid)
                string token = await GetAccessTokenAsync();

                // Create a HttpWebRequest for the API GET request
                string apiRequestUri = apiBaseURI + apiEndPoint;
                var request = (HttpWebRequest)WebRequest.Create($"{apiRequestUri}{indvCrdId}");
                request.ContentType = "application/json";
                request.Method = "GET";

                // Set the Bearer token in the Authorization header
                request.Headers["Authorization"] = $"Bearer {token}";

                // Use Task.Run to make this call asynchronously
                var response = await Task.Run(() => (HttpWebResponse)request.GetResponse());

                // Read the response
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseBody = await reader.ReadToEndAsync();
                    var ApiResponseIndvValidation = JsonSerializer.Deserialize<ApiResponseIndvValidation>(responseBody);

                    if (ApiResponseIndvValidation == null)
                    {
                        throw new Exception("Failed to deserialize API response.");
                    }

                    return ApiResponseIndvValidation;
                }
            }
            catch (WebException ex)
            {
                throw new ApplicationException("Error during API data retrieval: " + GetWebExceptionMessage(ex), ex);
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("Error deserializing API response: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error during data retrieval: " + ex.Message, ex);
            }
        }

        public async Task<List<FirmInfo>> GetDataAsyncGetFirm(string apiBaseURI, string apiEndPoint, string firmCrdId)
        {
            try
            {
                // Get the access token (from cache if valid)
                string token = await GetAccessTokenAsync();

                // Create a HttpWebRequest for the API GET request
                string apiRequestUri = apiBaseURI + apiEndPoint;
                var request = (HttpWebRequest)WebRequest.Create($"{apiRequestUri}{firmCrdId}");
                request.ContentType = "application/json";
                request.Method = "GET";

                // Set the Bearer token in the Authorization header
                request.Headers["Authorization"] = $"Bearer {token}";

                // Use Task.Run to make this call asynchronously
                var response = await Task.Run(() => (HttpWebResponse)request.GetResponse());

                // Read the response
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseBody = await reader.ReadToEndAsync();

                    List<FirmInfo> ApiResponseFirmValidation = JsonSerializer.Deserialize<List<FirmInfo>>(responseBody);

                    if (ApiResponseFirmValidation == null)
                    {
                        throw new Exception("Failed to deserialize API response.");
                    }

                    return ApiResponseFirmValidation;
                }
            }
            catch (WebException ex)
            {
                throw new ApplicationException("Error during API data retrieval: " + GetWebExceptionMessage(ex), ex);
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("Error deserializing API response: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error during data retrieval: " + ex.Message, ex);
            }
        }

        private string GetWebExceptionMessage(WebException ex)
        {
            if (ex.Response != null)
            {
                using (var errorResponse = (HttpWebResponse)ex.Response)
                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                {
                    string errorText = reader.ReadToEnd();
                    return $"HTTP {errorResponse.StatusCode}: {errorText}";
                }
            }
            return ex.Message;
        }
    }
}
