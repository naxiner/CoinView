using Newtonsoft.Json;

namespace CoinView.Models
{
    public class CurrencyData
    {
        public string Id { get; set; }
		public int Rank { get; set; }
        public string Symbol { get; set; }
		public string Name { get; set; }
		public decimal Supply { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public decimal MaxSupply { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public decimal MarketCapUsd { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public decimal VolumeUsd24Hr { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public decimal PriceUsd { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public decimal ChangePercent24Hr { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public decimal Vwap24Hr { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Explorer { get; set; }
    }
}
