using FruVa.Ordering.ApiAccess;

namespace FruVa.Ordering.Tests
{
    [TestClass]
    public sealed class ApiAccessTests
    {
        private IService? _service;
        private readonly Guid ARTICLE_ID = Guid.Parse("a70d7800-2099-4085-88cd-000039c882c5");
        private readonly Guid RECIPIENT_ID = Guid.Parse("fa0460f7-7e76-4ad4-bcc3-064eed3e500f");

        [TestInitialize]
        public void Setup()
        {
            _service = new Service();
        }

        [TestMethod]
        public async Task CanGetArticles()
        {
            var articles = await _service!.GetArticlesAsync();

            Assert.IsNotNull(articles);
            Assert.IsTrue(articles.Count > 0);
        }

        [TestMethod]
        public async Task CanGetRecipients()
        {
            var recipients = await _service!.GetRecipientsAsync();

            Assert.IsNotNull(recipients);
            Assert.IsTrue(recipients.Count > 0);
        }

        [TestMethod]
        public async Task CanGetArticleById()
        {
            var article = await _service!.GetArticleByIdAsync(ARTICLE_ID);

            Assert.IsNotNull(article);
            Assert.AreEqual(ARTICLE_ID, article.Id);
        }

        [TestMethod]
        public async Task CanGetRecipientById()
        {
            var recipient = await _service!.GetRecipientByIdAsync(RECIPIENT_ID);

            Assert.IsNotNull(recipient);
            Assert.AreEqual(RECIPIENT_ID, recipient.Id);
        }
    }
}
