using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using duckduckgo.api.Models;
using DuckDuckGo.api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace duckduckgo.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public SearchController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<SearchResponse> Get(string text, short pageNumber, short pageSize)
        {
            if (string.IsNullOrEmpty(text)) return new SearchResponse { Value = new List<DuckDuckSearchResponse>(), TotalItems = 0 };
            string response = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://api.duckduckgo.com/?q={text}&format=json";
                using (var result = await client.GetAsync(uri))
                {
                    response = await result.Content.ReadAsStringAsync();
                    var relatedTopicsIndex = response.IndexOf("RelatedTopics");
                    var resultsIndex = response.IndexOf("Results");
                    try
                    {
                        var relatedTopics = response.Substring(relatedTopicsIndex - 1, resultsIndex - relatedTopicsIndex);
                        //    string search = "FirstURL([\\s\\S]*?)Text";
                        string search = "FirstURL([\\s\\S]*?)Text([\\s\\S]*?)}";
                        MatchCollection matches = Regex.Matches(relatedTopics, search);
                        var results = matches.Select((m, index) =>
                        {
                            var Jsonconvert = JsonConvert.DeserializeObject($" {{\"{m.Value}");
                            return new DuckDuckSearchResponse { value = JsonConvert.SerializeObject(Jsonconvert), index = index };
                        }).Skip(pageNumber * pageSize).Take(pageSize);
                        return new SearchResponse
                        {
                            Value = results,
                            TotalItems = matches.Count
                        };
                        //var jsonResult = JsonConvert.DeserializeObject(results);
                        //  return jsonResult.ToString();
                    }
                    catch (Exception ex)
                    {
                        return new SearchResponse { Value = new List<DuckDuckSearchResponse>(), TotalItems = 0 };

                    }

                }
            }
        }
    }

}
