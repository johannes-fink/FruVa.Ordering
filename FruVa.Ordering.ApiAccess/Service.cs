using FruVa.Ordering.ApiAccess.Models;
using System.Text.Json;

namespace FruVa.Ordering.ApiAccess
{
    public class Service : IService
    {
        private readonly Uri BASE_URL = new Uri("https://localhost:61404/api/");
        private HttpClient HttpClient {  get; set; }

        public Service()
        {
            HttpClient = new HttpClient() { BaseAddress = BASE_URL };
        }

        public async Task<List<Article>> GetArticlesAsync()
        {
            var response = await HttpClient.GetAsync("articles");

            if (response.IsSuccessStatusCode == false)
            {
                // TODO: Error => Log and inform user
                return [];
            }

            var json = await response.Content.ReadAsStringAsync();
            var articles = JsonSerializer.Deserialize<List<Article>>(json);

            return articles ?? [];
        }

        public async Task<List<Recipient>> GetRecipientsAsync()
        {
            var response = await HttpClient.GetAsync("recipients");

            if (response.IsSuccessStatusCode == false)
            {
                // TODO: Error => Log and inform user
                return [];
            }

            var json = await response.Content.ReadAsStringAsync();
            var recipients = JsonSerializer.Deserialize<List<Recipient>>(json);

            return recipients ?? [];
        }

        public async Task<Article?> GetArticleByIdAsync(Guid id)
        {
            var response = await HttpClient.GetAsync($"articles/{id}");

            if (response.IsSuccessStatusCode == false)
            {
                // TODO: Error => Log and inform user
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var article = JsonSerializer.Deserialize<Article>(json);

            return article;
        }
        public async Task<Recipient?> GetRecipientByIdAsync(Guid id)
        {
            var response = await HttpClient.GetAsync($"Recipients/{id}");

            if (response.IsSuccessStatusCode == false)
            {
                // TODO: Error => Log and inform user
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var recipient = JsonSerializer.Deserialize<Recipient>(json);

            return recipient;
        }
    }
}