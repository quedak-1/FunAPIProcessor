using System;
using System.Text.Json.Serialization;

namespace ScaleThreadProcess
{
    public class BitcoinPriceResponse
    {
        [JsonPropertyName("time")]
        public TimeInfo TimeReported { get; set; }

        [JsonPropertyName("disclaimer")]
        public string Disclaimer { get; set; }

        [JsonPropertyName("chartName")]
        public string ChartName { get; set; }

        [JsonPropertyName("bpi")]
        public Bpi Bpi { get; set; }

        public override string ToString()
        {
            return $@"{TimeReported}<hr>{ChartName} Bpi: {Bpi}";
        }
    }

    public class TimeInfo
    {
        [JsonPropertyName("updated")]
        public string Updated { get; set; }

        [JsonPropertyName("updatedISO")]
        public DateTime UpdatedISO { get; set; }

        [JsonPropertyName("updateduk")]
        public string UpdatedUk { get; set; }

        public override string ToString()
        {
            return $@"{Updated}";
        }
    }

    public class Bpi
    {
        [JsonPropertyName("USD")]
        public CurrencyInfo USD { get; set; }

        [JsonPropertyName("GBP")]
        public CurrencyInfo GBP { get; set; }

        [JsonPropertyName("EUR")]
        public CurrencyInfo EUR { get; set; }

        public override string ToString()
        {
            return $@"{USD}";
        }

    }

    public class CurrencyInfo
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("rate")]
        public string Rate { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("rate_float")]
        public float RateFloat { get; set; }

        public override string ToString()
        {
            return $@"{Symbol} Rate: <strong>{Rate}</strong> in {Description}";
        }

    }

}
