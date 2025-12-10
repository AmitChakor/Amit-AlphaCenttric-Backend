using System.Collections.Generic;
using System.Xml.Serialization;

namespace Alpha_XMLTradeProcessing.Models
{
    [XmlRoot("Securities")]
    public class Securities
    {
        [XmlElement("Security")]
        public List<Security> SecurityList { get; set; }
    }

    public class Security
    {
        public int Id { get; set; }
        public string BloombergId { get; set; }
        public string IssueCountry { get; set; }
        public HistoricalPrices HistoricalPrices { get; set; }
    }

    public class HistoricalPrices
    {
        [XmlElement("HistoricalPrice")]
        public List<HistoricalPrice> PriceList { get; set; }
    }

    public class HistoricalPrice
    {
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public string EffectiveDate { get; set; }
    }
}
