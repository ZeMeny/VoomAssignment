using NewsAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DroneWebsite.DataAccess
{
    public class ArticleCollection : IArticleCollection
    {
        private readonly string _articleFilePath = AppContext.BaseDirectory + "articles.json";
        private List<Article> _loadedArticles = new();

        public ArticleCollection()
        {
            LoadArticles(_articleFilePath);
        }

        private void LoadArticles(string path)
        {
            lock (this)
            {
                if (File.Exists(path))
                {
                    using FileStream stream = new(path, FileMode.Open, FileAccess.Read);
                    using StreamReader reader = new(stream);
                    var json = reader.ReadToEnd();
                    _loadedArticles = JsonConvert.DeserializeObject<List<Article>>(json);
                    _loadedArticles = _loadedArticles.Where(a=>a.Title.Contains("Drone")).ToList();
                }
            }
        }

        private void SaveState(string path)
        {
            lock (this)
            {
                using StreamWriter writer = new(path, false);
                var json = JsonConvert.SerializeObject(_loadedArticles, Formatting.Indented);
                writer.Write(json);
            }
        }

        public Task<IEnumerable<Article>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Article>>(_loadedArticles.OrderByDescending(x=>x.PublishedAt));
        }

        public Task AddAsync(params Article[] articles)
        {
            _loadedArticles.AddRange(articles);
            SaveState(_articleFilePath);
            return Task.CompletedTask;
        }

        public Task EditAsync(Action<List<Article>> action)
        {
            return Task.Run(() => action(_loadedArticles));
        }

        public Task RemoveAsync(Article article)
        {
            _loadedArticles.Remove(article);
            SaveState(_articleFilePath);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Article>> SearchAsync(Func<Article, bool> query)
        {
            return Task.FromResult(_loadedArticles.OrderByDescending(x=>x.PublishedAt).Where(query));
        }
    }
}
