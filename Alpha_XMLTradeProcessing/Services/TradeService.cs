using Alpha_XMLTradeProcessing.Interfaces;
using Alpha_XMLTradeProcessing.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alpha_XMLTradeProcessing.Services
{
    public class TradeService : ITradeService
    {
        private readonly IFileReader _fileReader;

        public TradeService(IFileReader fileReader)
        {
            _fileReader = fileReader;
        }

        public List<Trade> ProcessTradeFiles(string rootFolder)
        {
            var allTrades = new List<Trade>();

            // Efficiently find all trade files using EnumerateFiles
            var tradeFiles = Directory.EnumerateFiles(
                rootFolder,
                "Trades*.xml",
                SearchOption.AllDirectories
            );

            foreach (var file in tradeFiles)
            {
                var trades = _fileReader.ReadTradeFile(file);
                allTrades.AddRange(trades);
            }

            return allTrades;
        }

        public List<TradeSummary> AggregateTrades(List<Trade> trades)
        {
            return trades
                .GroupBy(t => new
                {
                    t.Security.Code,
                    t.TransactionCode,
                    t.TradeDate
                })
                .Select(g => new TradeSummary
                {
                    BloombergId = g.Key.Code,
                    TransactionCode = g.Key.TransactionCode,
                    TradeDate = g.Key.TradeDate,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    AveragePrice = g.Average(x => x.Price),
                    TradeCount = g.Count()
                })
                .OrderBy(t => t.BloombergId)
                .ThenBy(t => t.TradeDate)
                .ThenBy(t => t.TransactionCode)
                .ToList();
        }

        public List<FileResult> GetInvalidSecurityTrades(List<Trade> trades, ISecurityService securityService)
        {
            return trades
                // Use null-conditional operator so null Security doesn't throw; IsValidSecurity defends against null key
                .Where(t => !securityService.IsValidSecurity(t.Security?.Code))
                .GroupBy(t => t.SourceFile)
                .Select(g => new FileResult
                {
                    FileName = g.Key,
                    InvalidSecurityCount = g.Count(),
                    InvalidTrades = g.ToList()
                })
                .OrderByDescending(f => f.InvalidSecurityCount)
                .ThenBy(f => f.FileName)
                .ToList();
        }
    }
}
