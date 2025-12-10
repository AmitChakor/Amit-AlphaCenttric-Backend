using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha_XMLTradeProcessing.Models
{
    public class TradeSummary
    {
        public string BloombergId { get; set; }
        public string TransactionCode { get; set; }
        public string TradeDate { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal AveragePrice { get; set; }
        public int TradeCount { get; set; }
    }
}
