using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinView.Models
{
	public class CurrencyHistory
	{
		public decimal PriceUsd { get; set; }
		public long Time { get; set; }
		public decimal circulatingSupply { get; set; }
		public DateTime Date { get; set; }
	}
}
