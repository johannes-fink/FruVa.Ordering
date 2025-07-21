using System.Text.Json.Serialization;

namespace FruVa.Ordering.ApiAccess.Models
{
    public class Recipient
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("street")]
        public string Street { get; set; }

        [JsonPropertyName("streetNumber")]
        public string StreetNumber { get; set; }

        [JsonPropertyName("postCode")]
        public string PostCode { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("rowVersion")]
        public string RowVersion { get; set; }

        public override string ToString()
        {
            return $"{Id}|{Name}|{Street}|{StreetNumber}|{PostCode}|{City}|{Country}|{RowVersion}";
        }
    }
}