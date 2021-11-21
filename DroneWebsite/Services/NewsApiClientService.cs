using DroneWebsite.DataAccess;
using DroneWebsite.Properties;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DroneWebsite.Services
{
    public class NewsApiClientService : BackgroundService
    {
        private readonly ILogger<NewsApiClientService> _logger;
        private readonly IArticleCollection _articleCollection;
        private DateTime _lastUpdated;

        public NewsApiClientService(ILogger<NewsApiClientService> logger, IArticleCollection articleCollection)
        {
            _logger = logger;
            _articleCollection = articleCollection;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                _lastUpdated = await GetLatestArticleAsync();
                while (!cancellationToken.IsCancellationRequested)
                {
                    await RequestNewsFeed();
                    await Task.Delay(int.Parse(Resources.ArticleScanIntervalSeconds)*1000, cancellationToken);
                }
            });
        }

        private async Task RequestNewsFeed()
        {
            try
            {
                var newsApiClient = new NewsApiClient(Resources.NewsApiKey);
                var articlesResponse = await newsApiClient.GetEverythingAsync(new EverythingRequest
                {
                    Q = "Drone",
                    SortBy = SortBys.PublishedAt,
                    Language = Languages.EN,
                    From = _lastUpdated
                });
                if (articlesResponse.Status == Statuses.Ok)
                {
                    _lastUpdated = DateTime.Now;
                    if (articlesResponse.TotalResults > 0)
                    {
                        await HandleNewArticles(articlesResponse.Articles);
                    }
                }
                else
                {
                    _logger.LogError("Error requesting news from newsapi.org", articlesResponse.Error.Code, articlesResponse.Error.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting news from newsapi.org", ex);
            }
        }

        private async Task<DateTime> GetLatestArticleAsync()
        {
            var articles = await _articleCollection.GetAllAsync();
            if (articles == null || articles.Any())
            {
                return DateTime.Now.Subtract(TimeSpan.FromHours(24));
            }
            var latest = articles.OrderBy(a => a.PublishedAt).LastOrDefault();
            if (latest != null)
            {
                return latest.PublishedAt.HasValue ? latest.PublishedAt.Value : DateTime.Now;
            }

            return DateTime.Now.Subtract(TimeSpan.FromHours(24));
        }

        private async Task HandleNewArticles(IEnumerable<Article> articles)
        {
            await _articleCollection.AddAsync(articles.ToArray());
        }
    }
}