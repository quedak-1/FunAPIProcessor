using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinraEDSProcessor.Classes
{
    public class ProfileConfig
    {
        public string ApiBaseUri { get; set; }
        public string TokenEndpointUri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiEndpointSearchIndv { get; set; }
        public string ApiEndpointValidateIndv { get; set; }
        public string ApiEndpointValidateFirm { get; set; }
    }
}
