using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinraEDSProcessor.Classes
{

    public class IndvInfo
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

    public class ApiResponseIndividualSearch
    {
        public int individualCrdNumber { get; set; }  // Individual's CRD ID number.
        public string lastName { get; set; }  // Individual's primary last name.
        public string firstName { get; set; }  // Individual's primary first name.

        public override string ToString()
        {
            if (individualCrdNumber > 0)
            {
                return $"CRDId: {individualCrdNumber} LastName: {lastName} FirtName: {firstName}";
            }
            else
            {
                return "Individual not found";
            }

        }
    }

    public class ApiResponseIndvValidation
    {
        public int individualCrdNumber { get; set; }  // The CRD Number of an individual.
        public string firstName { get; set; }  // .
        public string middleName { get; set; }  // 
        public string lastName { get; set; }  // The type of form to which the event applies.
        public string suffixName { get; set; }  // 

        [JsonPropertyName("employments.firmCrdNumber")]
        public string employments_firmCrdNumber { get; set; }  // 

        [JsonPropertyName("employments.doingBusinessAsName")]
        public string employments_doingBusinessAsName { get; set; }  // 

        [JsonPropertyName("employments.isActive")]
        public string employments_isActive { get; set; }  // 

        [JsonPropertyName("registrations.regulatorName")]
        public string registrations_regulatorName { get; set; }  // Description of the event.

        [JsonPropertyName("registrations.isInactiveOrSuspended")]
        public string registrations_isInactiveOrSuspended { get; set; }  // Description of the event.

        [JsonPropertyName("registrations.regScope")]
        public string registrations_regScope { get; set; }  // Indicates whether the registration held by the individual is as BrokerDealer or Investment Advisor 
    }
}
