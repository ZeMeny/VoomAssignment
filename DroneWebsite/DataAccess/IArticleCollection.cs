using NewsAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DroneWebsite.DataAccess
{
    public interface IArticleCollection
    {
        Task<IEnumerable<Article>> GetAllAsync();

        Task AddAsync(params Article[] articles);

        Task EditAsync(Action<List<Article>> action);

        Task RemoveAsync(Article article);

        Task<IEnumerable<Article>> SearchAsync(Func<Article, bool> query);
    }
}