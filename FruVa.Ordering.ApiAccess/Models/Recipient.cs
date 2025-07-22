using System.Text.Json.Serialization;

namespace FruVa.Ordering.ApiAccess.Models
{
    public class Recipient
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("street")]
        public required string Street { get; set; }

        [JsonPropertyName("streetNumber")]
        public required string StreetNumber { get; set; }

        [JsonPropertyName("postCode")]
        public required string PostCode { get; set; }

        [JsonPropertyName("city")]
        public required string City { get; set; }

        [JsonPropertyName("country")]
        public required string Country { get; set; }

        [JsonPropertyName("rowVersion")]
        public required string RowVersion { get; set; }

        public override string ToString()
        {
            return $"{Id}|{Name}|{Street}|{StreetNumber}|{PostCode}|{City}|{Country}|{RowVersion}";
        }
    }
}