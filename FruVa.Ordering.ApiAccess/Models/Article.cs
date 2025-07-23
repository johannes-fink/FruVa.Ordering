using System.Text.Json.Serialization;

namespace FruVa.Ordering.ApiAccess.Models
{
    public class Article
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("mainArticleNumber")]
        public int MainArticleNumber { get; set; }

        [JsonPropertyName("detailArticleNumber")]
        public int DetailArticleNumber { get; set; }

        [JsonPropertyName("articleName")]
        public required string ArticleName { get; set; }

        [JsonPropertyName("packageSize")]
        public required string PackageSize { get; set; }

        [JsonPropertyName("articleGroupNumber")]
        public required string ArticleGroupNumber { get; set; }

        [JsonPropertyName("articleGroupName")]
        public required string ArticleGroupName { get; set; }

        [JsonPropertyName("originCountry")]
        public required string OriginCountry { get; set; }

        [JsonPropertyName("tradeClass")]
        public required string TradeClass { get; set; }

        [JsonPropertyName("colli")]
        public float Colli { get; set; }

        [JsonPropertyName("caliber")]
        public required string Caliber { get; set; }

        [JsonPropertyName("variety")]
        public required string Variety { get; set; }

        [JsonPropertyName("ownBrand")]
        public required string OwnBrand { get; set; }

        [JsonPropertyName("searchQuery")]
        public required string SearchQuery { get; set; }

        [JsonPropertyName("rowVersion")]
        public required string RowVersion { get; set; }

        public override string ToString()
        {
            return $"{Id}|{MainArticleNumber}|{DetailArticleNumber}|{ArticleName}|{PackageSize}|{ArticleGroupNumber}|{ArticleGroupName}|{OriginCountry}|{TradeClass}|{Colli}|{Caliber}|{Variety}|{OwnBrand}|{SearchQuery}|{RowVersion}";
        }
    }

}
