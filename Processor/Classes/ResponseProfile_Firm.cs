using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinraEDSProcessor.Classes
{

    public class FirmInfo
    {
        [JsonPropertyName("sec8FirmIdentifier")]
        public string Sec8FirmIdentifier { get; set; }

        [JsonPropertyName("firmApplicantName")]
        public string FirmApplicantName { get; set; }

        [JsonPropertyName("registrations")]
        public List<Registration> Registrations { get; set; }

        [JsonPropertyName("doingBusinessAsName")]
        public string DoingBusinessAsName { get; set; }

        [JsonPropertyName("registeredFirmTypeCode")]
        public string RegisteredFirmTypeCode { get; set; }

        [JsonPropertyName("firmFinraDistrict")]
        public string FirmFinraDistrict { get; set; }

        [JsonPropertyName("firmCrdNumber")]
        public int FirmCrdNumber { get; set; }

        [JsonPropertyName("sec802FirmIdentifier")]
        public string Sec802FirmIdentifier { get; set; }

        [JsonPropertyName("firmAddress")]
        public FirmAddress FirmAddress { get; set; }

        [JsonPropertyName("sec801FirmIdentifier")]
        public string Sec801FirmIdentifier { get; set; }
    }

    public class Registration
    {
        [JsonPropertyName("regulatorCode")]
        public string RegulatorCode { get; set; }
    }

    public class FirmAddress
    {
        [JsonPropertyName("cityName")]
        public string CityName { get; set; }

        [JsonPropertyName("stateName")]
        public string StateName { get; set; }

        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }

        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("addressLine1")]
        public string AddressLine1 { get; set; }

        [JsonPropertyName("addressLine2")]
        public string AddressLine2 { get; set; }

        [JsonPropertyName("stateCode")]
        public string StateCode { get; set; }

        [JsonPropertyName("countryName")]
        public string CountryName { get; set; }
    }
}
