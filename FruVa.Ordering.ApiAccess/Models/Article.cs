using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        public string ArticleName { get; set; }

        [JsonPropertyName("packageSize")]
        public string PackageSize { get; set; }

        [JsonPropertyName("articleGroupNumber")]
        public string ArticleGroupNumber { get; set; }

        [JsonPropertyName("articleGroupName")]
        public string ArticleGroupName { get; set; }

        [JsonPropertyName("originCountry")]
        public string OriginCountry { get; set; }

        [JsonPropertyName("tradeClass")]
        public string TradeClass { get; set; }

        [JsonPropertyName("colli")]
        public float Colli { get; set; }

        [JsonPropertyName("caliber")]
        public string Caliber { get; set; }

        [JsonPropertyName("variety")]
        public string Variety { get; set; }

        [JsonPropertyName("ownBrand")]
        public string OwnBrand { get; set; }

        [JsonPropertyName("searchQuery")]
        public string SearchQuery { get; set; }

        [JsonPropertyName("rowVersion")]
        public string RowVersion { get; set; }
    }

}
