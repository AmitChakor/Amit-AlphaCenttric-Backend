# Alpha_XMLTradeProcessing - Documentation

## Overview

This application reads trade and security data from XML files, aggregates trades, and reports trades referencing invalid securities. It is implemented using OOP, dependency injection (Unity), and interface-driven services to maximize testability and maintainability.

## Key Design Choices

- Dependency Injection (Unity)
  - Purpose: Decouple composition from implementation and enable easy swapping and mocking of services.
  - Implementation: Unity resolves IFileReader, ISecurityService, and ITradeService in Program.cs (composition root).
  - Rationale: Unity is compatible with .NET Framework 4.7.2 and fits legacy/enterprise requirements. Alternatives (Microsoft DI, Autofac) are viable but would require refactoring the existing composition.

- Object-Oriented Design
  - Purpose: Encapsulate data and behavior into clear responsibilities (models, services, interfaces).
  - Benefit: Improves readability, maintainability and enables unit testing.

- Interfaces
  - IFileReader, ISecurityService, ITradeService define contracts and allow implementations to be replaced/mocked without changing consumers.

- Class Structure and Responsibilities
  - Models: Represent XML-mapped data structures (Trade, TradeSecurity, Security, TradeSummary, FileResult). XML element mappings use [XmlElement] attributes where XML casing differs from C# property names.
  - XmlFileReader (IFileReader): Reads and deserializes trades and securities XML into model objects and attaches source file metadata to trades.
  - SecurityService (ISecurityService): Loads securities into a Dictionary keyed by BloombergId and validates security codes. Defensive checks prevent null keys from being used with Dictionary.ContainsKey.
  - TradeService (ITradeService): Discovers trade files, reads trades, aggregates by (Security.Code, TransactionCode, TradeDate), and identifies invalid-security trades grouped by source file.
  - Program.cs: Orchestrates workflow (load securities, process trades, aggregate, detect invalids, display/write results).

- File/Folder Reading
  - Directory.EnumerateFiles is used for efficient, memory-friendly discovery of matching trade files (streams results instead of allocating a full array).
  - XmlSerializer is used for straightforward object mapping when XML structure is stable.
  - Trade.SourceFile is set by the reader to preserve provenance for invalid-trade reporting.

## XML Mapping Notes

- Trade XML files used lowercase element names (e.g. <tradingAccount>, <transactionCode>, <security><code>), so the Trade model includes [XmlElement("...")] attributes with the exact element names to ensure proper deserialization.
- Security XML uses <Security><BloombergId> so the securities model maps BloombergId accordingly.
- Ensure the element names (including casing) in XML match the attributes on the models; otherwise properties will be null/empty and grouping/lookups will fail.

## Execution Flow (Step-by-step)

1. Program starts and resolves dependencies via UnityConfig.
2. securityService.LoadSecurities(filePath) -> XmlFileReader.ReadSecuritiesFile -> deserializes securities and returns Dictionary<string, Security> keyed by BloombergId.
   - Conditions: Missing file -> empty dictionary. Keys must match trade codes (consider normalization).
3. tradeService.ProcessTradeFiles(rootFolder) -> Directory.EnumerateFiles discovers files -> each file is deserialized via XmlFileReader.ReadTradeFile into Trades.TradeList. SourceFile is assigned.
   - Conditions: Missing/invalid files -> that file returns empty list.
4. tradeService.AggregateTrades(allTrades) -> GroupBy(Security.Code, TransactionCode, TradeDate) -> computes totals, averages, and counts.
   - Conditions: Null/empty Security.Code will result in an aggregated group with an empty BloombergId. Ensure mapping/normalization fixes this.
5. tradeService.GetInvalidSecurityTrades(allTrades, securityService) -> filters where !IsValidSecurity(t.Security?.Code) and groups by SourceFile.
   - SecurityService.IsValidSecurity now guards against null/empty keys and trims input before dictionary lookup to avoid ArgumentNullException.
6. Results are printed to console and written to Output.txt.

## Scenarios and Edge Cases

- Valid data: All trades map to securities; aggregated report shows grouped BloombergId rows.
- Invalid security codes: Trades with codes not found in securities are reported under INVALID SECURITY TRADES with file counts and per-trade listing.
- Null or empty security code: Treated as invalid; IsValidSecurity returns false. Previously caused ArgumentNullException when calling ContainsKey(null) — now fixed.
- Mismatched XML element names/casing: Deserialization yields null/empty properties — fix by adding correct [XmlElement("elementName")] attributes on model properties.
- Missing or malformed files: XmlFileReader returns empty collections for missing files; exceptions during deserialization should be handled or logged if they occur.
- Large file counts/volumes: EnumerateFiles avoids heavy memory use. For very large files or volumes, consider XmlReader streaming and controlled parallelism (ensure XmlSerializer thread-safety).

## Normalization and Robustness Recommendations

- Normalize security keys and trade codes (Trim and ToUpperInvariant) in both ReadSecuritiesFile and trade mappings before lookups to avoid mismatches due to whitespace or casing.
- Add logging for per-file deserialization errors and skip problematic files rather than failing the entire run.
- Consider adding unit tests for TradeService and SecurityService using mocked IFileReader to validate aggregation and invalid-trade behavior.
- For high throughput, implement a thread-safe XmlSerializer cache or instantiate XmlSerializer per thread and use Parallel.ForEach over file list with a bounded degree of parallelism.

## Run Instructions

1. Ensure App.config and UnityConfig are configured to register implementations for IFileReader, ISecurityService, and ITradeService.
2. Update the hard-coded paths in Program.cs or pass arguments for securities file and trades root folder.
3. Build the solution targeting .NET Framework 4.7.2.
4. Run the application; check console output and Output.txt for results.

## Troubleshooting Checklist

- If aggregated rows show empty BloombergId:
  - Verify Trade.Security.Code was populated by deserialization (check element name/casing in trade XML and model mapping).
  - Ensure securities were loaded and keys match normalized trade codes.
- If many trades reported as invalid:
  - Check normalization (whitespace/casing) and dictionary key format.
  - Verify securities file path and that LoadSecurities ran successfully before validating trades.
- If application throws ArgumentNullException at Dictionary.ContainsKey:
  - Confirm IsValidSecurity includes a null/empty guard (it does). Ensure LoadSecurities returns a non-null dictionary.

## Contact / Next Steps
- Add unit tests for the services.
- Add logging for file-processing steps and failures.
- Consider migrating to Microsoft.Extensions.DependencyInjection if moving to .NET Core/.NET 5+ in the future.

---

End of document.
