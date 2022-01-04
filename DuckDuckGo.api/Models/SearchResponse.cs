using System.Collections.Generic;

namespace duckduckgo.api.Models
{
    public class SearchResponse
    {
        public IEnumerable<DuckDuckSearchResponse> Value { get; set; }
        public int TotalItems { get; set; }
    }
}
