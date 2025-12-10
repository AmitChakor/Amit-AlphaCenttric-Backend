using System.Collections.Generic;
using System.Xml.Serialization;

namespace Alpha_XMLTradeProcessing.Models
{
    [XmlRoot("trades")]
    public class Trades
    {
        [XmlElement("trade")]
        public List<Trade> TradeList { get; set; }
    }

    public class Trade
    {
        [XmlElement("tradingAccount")]
        public string TradingAccount { get; set; }

        [XmlElement("tradeDate")]
        public string TradeDate { get; set; }

        [XmlElement("settleDate")]
        public string SettleDate { get; set; }

        [XmlElement("actualDate")]
        public string ActualDate { get; set; }

        [XmlElement("transactionCode")]
        public string TransactionCode { get; set; }

        [XmlElement("quantity")]
        public decimal Quantity { get; set; }

        [XmlElement("tradeCurrency")]
        public string TradeCurrency { get; set; }

        [XmlElement("settlementCurrency")]
        public string SettlementCurrency { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("execBroker")]
        public string ExecBroker { get; set; }

        // SourceFile is set by the reader, not from XML
        public string SourceFile { get; set; }

        [XmlElement("security")]
        public TradeSecurity Security { get; set; }
    }

    public class TradeSecurity
    {
        [XmlElement("id")]
        public int Id { get; set; }

        // trade XML uses <code> for Bloomberg id
        [XmlElement("code")]
        public string Code { get; set; }
    }
}
