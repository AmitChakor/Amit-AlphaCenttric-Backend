using Alpha_XMLTradeProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha_XMLTradeProcessing.Interfaces
{
    public interface ITradeService
    {
        List<Trade> ProcessTradeFiles(string rootFolder);
        List<TradeSummary> AggregateTrades(List<Trade> trades);
        List<FileResult> GetInvalidSecurityTrades(List<Trade> trades, ISecurityService securityService);
    }
}
