namespace ScaleThreadProcess
{
    public class ApiConfig
    {
        public string ApiBaseUri { get; set; }
        public string TokenEndpointUri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiEndpointDefault { get; set; }
        public string ApiEndpointSearchIndv { get; set; }
        public string ApiEndpointValidateIndv { get; set; }
        public string ApiEndpointValidateFirm { get; set; }
        public string ApiAuthMethod { get; set; }
        public string OutputFilePath { get; set; }
    }
}
