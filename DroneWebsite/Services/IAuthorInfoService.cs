using System.Threading.Tasks;

namespace DroneWebsite.Services
{
    public interface IAuthorInfoService
    {
        Task<string> SearchAuthor(string name);
    }
}