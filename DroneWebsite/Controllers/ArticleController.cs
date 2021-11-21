using DroneWebsite.DataAccess;
using DroneWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DroneWebsite.Controllers
{
    [Route("api/articles")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleCollection _articleCollection;
        private readonly IAuthorInfoService _authorInfoService;

        public ArticleController(ILogger<ArticleController> logger, IArticleCollection articleCollection, 
            IAuthorInfoService authorInfoService)
        {
            _logger = logger;
            _articleCollection = articleCollection;
            _authorInfoService = authorInfoService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetLatestArticles()
        {
            try
            {
                var articles = await _articleCollection.GetAllAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting latest articles", ex);
                return StatusCode(500, "Sorry, something went wrong...");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetArticlesByKeyword([FromQuery] string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return Ok(await _articleCollection.GetAllAsync());
                }
                // get articles from the last 24 hours
                var articles = await _articleCollection.SearchAsync(x => x.Title.Contains(key) || x.Content.Contains(key) || x.Description.Contains(key));
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error finding articles by keyword", ex);
                return StatusCode(500, "Sorry, something went wrong...");
            }
        }

        [HttpGet("author")]
        public async Task<IActionResult> GetAuthorInfo([FromQuery]string name)
        {
            try
            {
                var data =  await _authorInfoService.SearchAuthor(name);
                if (!string.IsNullOrEmpty(data))
                {
                    return Ok(data);
                }
                else
                {
                    return NotFound($"Could not find any info on {name}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting author data", ex);
                return StatusCode(500, "Sorry, something went wrong...");
            }
        }
    }
}
