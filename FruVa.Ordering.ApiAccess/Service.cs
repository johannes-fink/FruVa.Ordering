using FruVa.Ordering.ApiAccess.Models;
using System.Numerics;
using System.Text.Json;

namespace FruVa.Ordering.ApiAccess
{
    public class Service : IService
    {
        private readonly Uri BASE_URL = new("https://localhost:61404/api/");
        private HttpClient HttpClient {  get; set; }

        private List<Article>? Articles { get; set; }
        private List<Recipient>? Recipients { get; set; }

        public Service()
        {
            HttpClient = new HttpClient() { BaseAddress = BASE_URL };
        }

        public async Task<List<Article>> GetArticlesAsync()
        {
            if (Articles is not null)
            {
                return Articles;
            }

            var response = await HttpClient.GetAsync("articles");

            if (response.IsSuccessStatusCode == false)
            {
                return [];
            }

            var json = await response.Content.ReadAsStringAsync();
            var articles = JsonSerializer.Deserialize<List<Article>>(json);
            Articles = articles ?? [];

            return Articles;
        }

        public async Task<List<Recipient>> GetRecipientsAsync()
        {
            if (Recipients is not null)
            {
                return Recipients;
            }

            var response = await HttpClient.GetAsync("recipients");

            if (response.IsSuccessStatusCode == false)
            {
                return [];
            }

            var json = await response.Content.ReadAsStringAsync();
            var recipients = JsonSerializer.Deserialize<List<Recipient>>(json);
            Recipients = recipients ?? [];

            return Recipients;
        }

        public async Task<Article?> GetArticleByIdAsync(Guid id)
        {
            if (Articles is not null)
            {
                return Articles.FirstOrDefault(x => x.Id == id);
            }

            var response = await HttpClient.GetAsync($"articles/{id}");

            if (response.IsSuccessStatusCode == false)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var article = JsonSerializer.Deserialize<Article>(json);

            return article;
        }

        public async Task<Recipient?> GetRecipientByIdAsync(Guid id)
        {
            if (Recipients is not null)
            {
                return Recipients.FirstOrDefault(x => x.Id == id);
            }

            var response = await HttpClient.GetAsync($"Recipients/{id}");

            if (response.IsSuccessStatusCode == false)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var recipient = JsonSerializer.Deserialize<Recipient>(json);

            return recipient;
        }
    }
}