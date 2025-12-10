using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha_XMLTradeProcessing.Models
{
    public class FileResult
    {
        public string FileName { get; set; }
        public int InvalidSecurityCount { get; set; }
        public List<Trade> InvalidTrades { get; set; }
    }
}
