using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinView.Models
{
    class Currency
    {
        public string Id { get; set; } = string.Empty;
		public int Rank { get; set; }
        public string Symbol { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public decimal Supply { get; set; }
        public decimal MaxSuplly { get; set; }
        public decimal MarketCapUsd { get; set; }
        public decimal VolumeUsd24Hr { get; set; }
        public decimal PriceUsd { get; set; }
        public decimal ChangePercent24Hr { get; set; }
        public decimal Vwap24Hr { get; set; }
        public string Explorer { get; set; } = string.Empty;
    }
}
