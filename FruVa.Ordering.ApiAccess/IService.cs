using FruVa.Ordering.ApiAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
