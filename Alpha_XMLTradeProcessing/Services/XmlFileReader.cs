using Alpha_XMLTradeProcessing.Interfaces;
using Alpha_XMLTradeProcessing.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Alpha_XMLTradeProcessing.Services
{
    public class XmlFileReader : IFileReader
    {
        public List<Trade> ReadTradeFile(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Trade>();

            var serializer = new XmlSerializer(typeof(Trades));
            using (var reader = new StreamReader(filePath))
            {
                var trades = (Trades)serializer.Deserialize(reader);

                // Add source file information
                foreach (var trade in trades.TradeList)
                {
                    trade.SourceFile = filePath;
                }

                return trades.TradeList;
            }
        }

        public Dictionary<string, Security> ReadSecuritiesFile(string filePath)
        {
            if (!File.Exists(filePath))
                return new Dictionary<string, Security>();

            var serializer = new XmlSerializer(typeof(Securities));
            using (var reader = new StreamReader(filePath))
            {
                var securities = (Securities)serializer.Deserialize(reader);
                return securities.SecurityList.ToDictionary(s => s.BloombergId);
            }
        }
    }
}
