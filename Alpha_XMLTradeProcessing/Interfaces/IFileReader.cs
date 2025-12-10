using Alpha_XMLTradeProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha_XMLTradeProcessing.Interfaces
{
    public interface IFileReader
    {
        List<Trade> ReadTradeFile(string filePath);
        Dictionary<string, Security> ReadSecuritiesFile(string filePath);
    }
}
