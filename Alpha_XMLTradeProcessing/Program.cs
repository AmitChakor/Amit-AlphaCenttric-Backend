using Alpha_XMLTradeProcessing.Interfaces;
using Alpha_XMLTradeProcessing.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha_XMLTradeProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Configuration paths
                string securitiesFilePath = @"C:\Engineer Code Test\Securities.xml";
                string tradesRootFolder = @"C:\Engineer Code Test\Test";

                // Resolve dependencies
                var fileReader = UnityConfig.Resolve<IFileReader>();
                var securityService = UnityConfig.Resolve<ISecurityService>();
                var tradeService = UnityConfig.Resolve<ITradeService>();

                // Load securities
                Console.WriteLine("Loading securities...");
                securityService.LoadSecurities(securitiesFilePath);
                Console.WriteLine($"Securities loaded successfully.");

                // Process trade files
                Console.WriteLine("\nProcessing trade files...");
                var allTrades = tradeService.ProcessTradeFiles(tradesRootFolder);
                Console.WriteLine($"Found {allTrades.Count} trades from all files.");

                // Aggregate trades
                Console.WriteLine("\nAggregating trades...");
                var aggregatedTrades = tradeService.AggregateTrades(allTrades);

                // Get invalid security trades
                Console.WriteLine("\nChecking for invalid securities...");
                var invalidSecurityTrades = tradeService.GetInvalidSecurityTrades(allTrades, securityService);

                // Display results
                DisplayResults(aggregatedTrades, invalidSecurityTrades);

                // Optionally write to file
                WriteResultsToFile(aggregatedTrades, invalidSecurityTrades);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void DisplayResults(List<TradeSummary> aggregatedTrades, List<FileResult> invalidSecurityTrades)
        {
            Console.WriteLine("\n=== AGGREGATED TRADES ===");
            Console.WriteLine("BloombergId | TransCode | TradeDate   | TotalQty | AvgPrice | Count");
            Console.WriteLine("--------------------------------------------------------------------");

            foreach (var trade in aggregatedTrades)
            {
                Console.WriteLine($"{trade.BloombergId,-11} | {trade.TransactionCode,-9} | {trade.TradeDate} | " +
                                $"{trade.TotalQuantity,8:F2} | {trade.AveragePrice,8:F2} | {trade.TradeCount,5}");
            }

            Console.WriteLine("\n=== INVALID SECURITY TRADES ===");
            Console.WriteLine("File Name                                      | Invalid Count");
            Console.WriteLine("--------------------------------------------------------------");

            foreach (var fileResult in invalidSecurityTrades)
            {
                var fileName = Path.GetFileName(fileResult.FileName);
                Console.WriteLine($"{fileName,-45} | {fileResult.InvalidSecurityCount,13}");

                // Display individual invalid trades if needed
                foreach (var trade in fileResult.InvalidTrades)
                {
                    Console.WriteLine($"  - Account: {trade.TradingAccount}, Security: {trade.Security.Code}");
                }
            }
        }

        static void WriteResultsToFile(List<TradeSummary> aggregatedTrades, List<FileResult> invalidSecurityTrades)
        {
            string outputPath = @"C:\Engineer Code Test\Output.txt";

            using (var writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("AGGREGATED TRADES");
                writer.WriteLine("BloombergId,TransactionCode,TradeDate,TotalQuantity,AveragePrice,TradeCount");

                foreach (var trade in aggregatedTrades)
                {
                    writer.WriteLine($"{trade.BloombergId},{trade.TransactionCode},{trade.TradeDate}," +
                                   $"{trade.TotalQuantity},{trade.AveragePrice},{trade.TradeCount}");
                }

                writer.WriteLine("\nINVALID SECURITY TRADES");
                writer.WriteLine("FileName,InvalidSecurityCount");

                foreach (var fileResult in invalidSecurityTrades)
                {
                    var fileName = Path.GetFileName(fileResult.FileName);
                    writer.WriteLine($"{fileName},{fileResult.InvalidSecurityCount}");
                }
            }

            Console.WriteLine($"\nResults written to: {outputPath}");
        }
    }
}
