using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace DroneWebsite.Services
{
    public class AuthorInfoService : IAuthorInfoService
    {
        public Task<string> SearchAuthor(string name)
        {
            return RunWikiSearch(name);
        }

        private async Task<string> RunWikiSearch(string query)
        {
            var httpClient = new RestClient(new Uri("https://en.wikipedia.org/w/api.php", UriKind.Absolute));
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("action", "query");
            request.AddQueryParameter("format", "json");
            request.AddQueryParameter("list", "search");
            request.AddQueryParameter("srsearch", $"intitle:{query}");

            var response = await httpClient.ExecuteGetAsync(request);

            if (response.IsSuccessful)
            {
                var result = ProcessWikiResponse(response.Content);
                if (string.IsNullOrEmpty(result))
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        private string ProcessWikiResponse(string reponse)
        {
            JObject jsonObj = JObject.Parse(reponse);
            var queryObj = jsonObj["query"];
            var searchItems = (JArray)queryObj["search"];
            if (searchItems.HasValues)
            {
                foreach (var item in searchItems)
                {
                    // take first successful wiki result
                    var url = GetUrlFromWiki(item);
                    if (!string.IsNullOrEmpty(url))
                    {
                        return url;
                    }
                }
            }
            return null;
        }

        private string GetUrlFromWiki(JToken wiki)
        {
            var pageId = wiki["pageid"].ToString();
            if (string.IsNullOrEmpty(pageId))
            {
                return null;
            }
            return  $"https://en.wikipedia.org/?curid={pageId}";
        }

        private async Task<bool> CheckWikiPage(string url)
        {
            try
            {
                RestRequest request = new(url);
                RestClient restClient = new();
                var response = await restClient.ExecuteGetAsync(request);
                return response.IsSuccessful;
            }
            catch
            {
                return false;
            }
        }
    }
}
