using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


public class ApiClient<TTokenResponse> where TTokenResponse : class, new()
{
    private readonly string _apiBaseUri;
    private readonly string _tokenEndpointUri;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly X509Certificate2 _clientCertificate;
    private readonly string _apiKey;
    private string _cachedAccessToken;
    private DateTime _tokenExpiration;
    private readonly bool _useApiKey;

    // Constructor for client secret-based authentication
    public ApiClient(string apiBaseUri, string tokenEndpointUri, string clientId, string clientSecret)
    {
        _apiBaseUri = apiBaseUri;
        _tokenEndpointUri = tokenEndpointUri;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _useApiKey = false;

        Console.WriteLine($"API: OAuth authenication mode");
    }

    // Constructor for certificate-based authentication
    public ApiClient(string apiBaseUri, string tokenEndpointUri, string clientId, X509Certificate2 clientCertificate)
    {
        _apiBaseUri = apiBaseUri;
        _tokenEndpointUri = tokenEndpointUri;
        _clientId = clientId;
        _clientCertificate = clientCertificate;
        _useApiKey = false;

        Console.WriteLine($"API: OAuth authenication mode");
    }

    // Constructor for API key-based authentication
    public ApiClient(string apiBaseUri, string apiKey = "")
    {
        _apiBaseUri = apiBaseUri;
        _apiKey = apiKey;
        _useApiKey = true;

        if (!String.IsNullOrEmpty(_apiKey))
            Console.WriteLine($"- API: API Keys authenication mode");
        else Console.WriteLine($"- API: No authenication mode");
    }

    // POST request with payload
    // POST request with payload, including API key if required
    public TResponse SendApiRequest<TResponse>(string endpoint, object payload) where TResponse : class
    {
        try
        {
            string token = _useApiKey ? null : GetAccessToken();
            string apiRequestUri = _apiBaseUri + endpoint;

            // Add API key to the payload if API key-based authentication is used
            var payloadWithApiKey = _useApiKey
                ? new { apiKey = _apiKey, data = payload }
                : payload;

            var request = (HttpWebRequest)WebRequest.Create(apiRequestUri);
            request.ContentType = "application/json";
            request.Method = "POST";

            // Set the Authorization header for non-API key based authentication
            if (!_useApiKey && token != null)
            {
                request.Headers["Authorization"] = $"Bearer {token}";
            }

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string jsonPayload = JsonSerializer.Serialize(payloadWithApiKey);
                streamWriter.Write(jsonPayload);
            }
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseBody = reader.ReadToEnd();
                        TResponse apiResponse = JsonSerializer.Deserialize<TResponse>(responseBody);

                        if (apiResponse == null)
                            throw new Exception("Failed to deserialize API response.");

                        return apiResponse;
                    }
                }
            }
            catch (Exception ex)

            {
                if (ex.Message.IndexOf("404") == -1)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
                else return null;
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

    // GET request without payload
    public TResponse SendApiRequest<TResponse>(string endpoint) where TResponse : class
    {
        try
        {
            string token = _useApiKey ? null : GetAccessToken();

            string apiRequestUri = _apiBaseUri + endpoint;
            var request = (HttpWebRequest)WebRequest.Create(apiRequestUri);
            request.ContentType = "application/json";
            request.Method = "GET";

            // Set the Authorization header for non-API key based authentication
            if (!_useApiKey && token != null)
            {
                request.Headers["Authorization"] = $"Bearer {token}";
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseBody = reader.ReadToEnd();

                        // Deserialize as a single object or a list based on TResponse type
                        TResponse apiResponse = JsonSerializer.Deserialize<TResponse>(responseBody) ?? throw new Exception("Failed to deserialize API response.");
                        return apiResponse;
                    }
                }
            }
            catch (Exception ex)

            {
                if (ex.Message.IndexOf("404") == -1)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
                else return null;
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

    private string GetAccessToken()
    {
        try
        {
            if (!string.IsNullOrEmpty(_cachedAccessToken) && DateTime.UtcNow < _tokenExpiration)
            {
                return _cachedAccessToken;
            }

            string tokenRequestUri = _tokenEndpointUri;
            var request = (HttpWebRequest)WebRequest.Create(tokenRequestUri);
            request.Method = "POST";
            request.Headers["Authorization"] = $"Basic {GetBasicAuthHeader()}";
            request.ContentType = "application/x-www-form-urlencoded";

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseBody = reader.ReadToEnd();
                    var tokenResponse = JsonSerializer.Deserialize<TTokenResponse>(responseBody) ?? new TTokenResponse();

                    _cachedAccessToken = ExtractAccessToken(tokenResponse);
                    _tokenExpiration = DateTime.UtcNow.AddSeconds(ExtractExpiresIn(tokenResponse));
                    Console.WriteLine($"API: bearer token received");
                    return _cachedAccessToken;
                }
            }
        }
        catch (WebException ex)
        {
            throw new ApplicationException("Error obtaining access token: " + GetWebExceptionMessage(ex), ex);
        }
    }

    private string GetBasicAuthHeader()
    {
        string credentials = $"{_clientId}:{_clientSecret}";
        byte[] byteCredentials = Encoding.UTF8.GetBytes(credentials);
        return Convert.ToBase64String(byteCredentials);
    }

    private string GetWebExceptionMessage(WebException ex)
    {
        using (var errorResponse = (HttpWebResponse)ex.Response)
        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
        {
            return $"HTTP {errorResponse.StatusCode}: {reader.ReadToEnd()}";
        }
    }

    // Methods to extract token and expiration values from the token response
    private string ExtractAccessToken(TTokenResponse tokenResponse)
    {
        return tokenResponse?.GetType().GetProperty("access_token")?.GetValue(tokenResponse)?.ToString() ?? "";
    }

    private int ExtractExpiresIn(TTokenResponse tokenResponse)
    {
        var expiresInValue = tokenResponse?.GetType().GetProperty("expires_in")?.GetValue(tokenResponse)?.ToString();
        return int.TryParse(expiresInValue, out int expiresIn) ? expiresIn : 3600;
    }
}

// Default AccessTokenResponse class if no custom token class is provided
public class AccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public string access_token { get; set; }

    [JsonPropertyName("expires_in")]
    public string expires_in { get; set; }

    [JsonPropertyName("scope")]
    public string scope { get; set; }

    [JsonPropertyName("token_type")]
    public string token_type { get; set; }

    public override string ToString()
    {
        return $"Type: {token_type} Scope: {scope} Expires: {expires_in}";
    }
}
