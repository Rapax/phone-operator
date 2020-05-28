using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.WebUtilities;
using PhoneOperator.Console.Models;

namespace PhoneOperator.Console
{
    public class NumberServiceClient
    {
        private const string BaseUri = "http://api.pts.se/";
        private HttpClient _httpClient = new HttpClient();

        public async Task<IEnumerable<PhoneViewModel>> RequestInformation(IEnumerable<string> numbers)
        {
            var queryString = string.Join(",", numbers);
            var subQueryString = QueryHelpers.AddQueryString("PTSNumberService/Pts_Number_Service.svc/pox/SearchByNumberList", "numbers",
                queryString);
            var url = new Uri($"{BaseUri}/{subQueryString}");
            
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
                
                var stream = await response.Content.ReadAsStreamAsync();
                
                var document = XDocument.Load(stream);
                return document.Root?.Descendants("{http://psgi.pts.se/PTS_Number_Service}OperatorDataContract")
                    .Select(x => new PhoneViewModel
                    {
                        Name = x.Element("{http://psgi.pts.se/PTS_Number_Service}Name")?.Value,
                        Number = x.Element("{http://psgi.pts.se/PTS_Number_Service}Number")?.Value
                    });
            }
            catch (HttpRequestException e)
            {
                throw;
            }
        }
    }
}