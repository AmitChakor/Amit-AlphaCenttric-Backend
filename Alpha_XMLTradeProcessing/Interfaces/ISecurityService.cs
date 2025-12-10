using Alpha_XMLTradeProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha_XMLTradeProcessing.Interfaces
{
    public interface ISecurityService
    {
        Dictionary<string, Security> LoadSecurities(string filePath);
        bool IsValidSecurity(string bloombergId);
    }
}
