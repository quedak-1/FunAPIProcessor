using System;
using System.Diagnostics;

namespace ScaleThreadProcess
{
    public partial class ProcessWorker : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Payload procedures

        public void DailyJokePIGetTest(ProcessProfile profile)
        {

            ApiConfig apiConfig = ConfigLoader.LoadApiConfig(profile.ProcedureParameters);

            Console.WriteLine($"[+] Execute API");

            try
            {
                Console.WriteLine($"- API URI: {apiConfig.ApiBaseUri}");

                // Try to get data from the API
                Console.WriteLine($"- API end-point: {apiConfig.ApiEndpointDefault}");

                var ApiClient = new ApiClient<AccessTokenResponse>
                        (
                            apiBaseUri: apiConfig.ApiBaseUri
                        );

                DailyJockResponse DailyJockResponse = ApiClient.SendApiRequest<DailyJockResponse>($"{apiConfig.ApiEndpointDefault}");
                Console.WriteLine($"Data retrieved from API: [{DailyJockResponse}]");

                System.IO.File.AppendAllText($@"{apiConfig.OutputFilePath}", $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} > {DailyJockResponse}\n");
                EmailClass.SendEmail(profile.SupportContact, "", "", "ScaleThread API: Joke of the Day", $"{DailyJockResponse}", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Trace.WriteLine($"[{DateTime.Now}] > Error: {ex.Message}");
                Trace.Flush();
            }
        }

        public void CoinDeskAPIGetTest(ProcessProfile profile)
        {

            ApiConfig apiConfig = ConfigLoader.LoadApiConfig(profile.ProcedureParameters);

            Console.WriteLine($"[+] Execute API");

            try
            {
                Console.WriteLine($"- API URI: {apiConfig.ApiBaseUri}");

                // Try to get data from the API
                Console.WriteLine($"- API end-point: {apiConfig.ApiEndpointDefault}");

                var ApiClient = new ApiClient<AccessTokenResponse>
                        (
                            apiBaseUri: apiConfig.ApiBaseUri
                        );

                BitcoinPriceResponse BitcoinPriceResponse = ApiClient.SendApiRequest<BitcoinPriceResponse>($"{apiConfig.ApiEndpointDefault}");
                Console.WriteLine($"Data retrieved from API: [{BitcoinPriceResponse}]");

                System.IO.File.AppendAllText($@"{apiConfig.OutputFilePath}", $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} > {BitcoinPriceResponse}\n");
                EmailClass.SendEmail(profile.SupportContact, "", "", "ScaleThread API: BitCoin Price", $"{BitcoinPriceResponse}", true);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Trace.WriteLine($"[{DateTime.Now}] > Error: {ex.Message}");
                Trace.Flush();
            }
        }

        /// <summary>
        /// Test REST GET Public API
        /// </summary>
        /// <param name="profile"></param>
        public void CatFactsAPIGetTest(ProcessProfile profile)
        {

            ApiConfig apiConfig = ConfigLoader.LoadApiConfig(profile.ProcedureParameters);

            Console.WriteLine($"[+] Execute API call");

            try
            {
                Console.WriteLine($"- API URI: {apiConfig.ApiBaseUri}");

                // Try to get data from the API
                Console.WriteLine($"- API end-point: {apiConfig.ApiEndpointDefault}");

                var ApiClient = new ApiClient<AccessTokenResponse>
                        (
                            apiBaseUri: apiConfig.ApiBaseUri
                        );

                CatFactsResponse CatFactsResponse = ApiClient.SendApiRequest<CatFactsResponse>($"{apiConfig.ApiEndpointDefault}");
                Console.WriteLine($"Data retrieved from API: [{CatFactsResponse}]");

                System.IO.File.AppendAllText($"{apiConfig.OutputFilePath}", $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} > {CatFactsResponse}\n");
                EmailClass.SendEmail(profile.SupportContact, "", "", "ScaleThread API: Cat Fact Daily", $"{CatFactsResponse}", true);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Trace.WriteLine($"[{DateTime.Now}] > Error: {ex.Message}");
                Trace.Flush();
            }
        }
        #endregion
    }
}
