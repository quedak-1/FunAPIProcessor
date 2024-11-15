using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinraEDSProcessor.Classes
{
    public class ApiResponseAccessTokenError
    {
        public string error_message { get; set; }
        public string error { get; set; }
    }


    // Updated AccessTokenResponse class to match the expected JSON fields
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

    public class RequestPayload
    {
        [JsonPropertyName("fields")]
        public List<string> Fields { get; set; }

        [JsonPropertyName("compareFilters")]
        public List<CompareFilter> CompareFilters { get; set; }
    }

    public class CompareFilter
    {
        [JsonPropertyName("compareType")]
        public string CompareType { get; set; }

        [JsonPropertyName("fieldName")]
        public string FieldName { get; set; }

        [JsonPropertyName("fieldValue")]
        public string FieldValue { get; set; }
    }

    /// <summary>
    /// ApiResponseDefaultError
    /// - use to capture failed requests
    /// Components
    ///   - DataRequest Class
    ///   - CompareFilter Class
    /// </summary>
    public class ApiResponseDefaultError
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("statusDescription")]
        public string StatusDescription { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("dataRequest")]
        public DataRequest DataRequest { get; set; }
    }

    public class DataRequest
    {
        [JsonPropertyName("async")]
        public bool Async { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("compareFilters")]
        public List<CompareFilter> CompareFilters { get; set; }

        [JsonPropertyName("datasetName")]
        public string DatasetName { get; set; }

        [JsonPropertyName("format")]
        public string Format { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("datasetGroup")]
        public string DatasetGroup { get; set; }

        [JsonPropertyName("fields")]
        public List<string> Fields { get; set; }
    }
}
