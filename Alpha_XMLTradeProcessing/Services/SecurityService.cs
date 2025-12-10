using Alpha_XMLTradeProcessing.Interfaces;
using Alpha_XMLTradeProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha_XMLTradeProcessing.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IFileReader _fileReader;
        private Dictionary<string, Security> _securities;

        public SecurityService(IFileReader fileReader)
        {
            _fileReader = fileReader;
        }

        public Dictionary<string, Security> LoadSecurities(string filePath)
        {
            _securities = _fileReader.ReadSecuritiesFile(filePath);
            return _securities;
        }

        public bool IsValidSecurity(string bloombergId)
        {
            // Defensive: do not pass null/empty keys to Dictionary.ContainsKey
            if (string.IsNullOrWhiteSpace(bloombergId))
                return false;

            bloombergId = bloombergId.Trim();

            return _securities != null && _securities.ContainsKey(bloombergId);
        }
    }
}
