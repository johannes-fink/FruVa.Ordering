using FruVa.Ordering.ApiAccess.Models;

namespace FruVa.Ordering.ApiAccess
{
    public interface IService
    {
        Task<List<Article>> GetArticlesAsync();
        Task<List<Recipient>> GetRecipientsAsync();
        Task<Article?> GetArticleByIdAsync(Guid id);
        Task<Recipient?> GetRecipientByIdAsync(Guid id);
    }
}
