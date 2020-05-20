//*****************************************************************************
// File: AssetsCSV.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: A class to interact with the AssetsFix.CSV file.
//
// Copright (c) 2020 Clyde W. Ford. All rights reserved.
//
// License: LGPL-3.0 (Non-commercial use only)
//
// DISCLAIMER:
//
// This Zorro plug-in is offered on an AS IS basis with no claims or warranties
// that it is fit or complete for any given purpose. YOU USE THIS PLUG-IN AT
// YOUR OWN RISK.
//
// Since the plug-in may be used as part of a system to trade financial instru-
// ments, the user of this plug-in accepts complete and total responsibility 
// for any damages, monetary or otherwise, that arize from the use of the plug-
// in, and holds harmless the author of the plug-in for any damages, financial
// or otherwise, incurred.
//
// For further information, see the Disclaimer included with this plug-in.
//*****************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TDAmeritradeZorro.Classes.TDA;
using TDAmeritradeZorro.Classes.TDA.Assets;
using TDAmeritradeZorro.Utilities;
using TDAmeritradeZorro.WebApi.Classes;

namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Class: AssetsCSV
    //
    /// <summary>
    /// A class to read and update the AssetsFix.CSV file.
    /// </summary>
    //*************************************************************************
    public class AssetsCSV
    {
        //********************************************************************
        //  Member: ASSETS_CSV_FILE
        //
        /// <summary>
        /// The file name and folder for the assets CSV file.
        /// </summary>
        //********************************************************************
        private static readonly string ASSETS_CSV_FILE = "/History/AssetsFix.csv";

        private static string[] Lines;

        private static Dictionary<string, string> SymbolNameDict = new Dictionary<string, string>();

        //********************************************************************
        //  Method: UpdateAssetsCSV
        //
        /// <summary>
        /// Update the data for the assets CSV file.
        /// </summary>
        //********************************************************************
        public static bool
            UpdateAssetsCSV
            ()
        {
            // Method members
            string symbols;

            try
            {
                // Read all lines of the CSV file and get the symbols
                symbols = GetSymbols();

                // Access the TD Ameritrade API and get quotes for all the 
                // symbols
                List<TDAsset> assetList = GetQuotes(symbols).GetAwaiter().GetResult();

                // Write out a new CSV file
                WriteCSVFile(assetList);

                // Return success code
                return true;
            }
            catch(Exception e)
            {
                // Log the error and return
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: " + e.Message);
            }

            // Return failure code
            return false; 
        }

        //********************************************************************
        //  Method: WriteCSVFile
        //
        /// <summary>
        /// Write a new CSV file to Zorro's History folder
        /// </summary>
        /// 
        /// <param name="Assets">
        /// A list of assets, one asset per line
        /// </param>
        //********************************************************************
        private static void
            WriteCSVFile
            (
            List<TDAsset> Assets
            )
        {
            // Initialize the lines being written out
            List<string> lines = new List<string>();

            // Add the header line
            lines.Add("Name,Price,Spread,RollLong,RollShort,PIP,PIPCost,MarginCost,Leverage,LotAmount,Commission,Symbol");

            // Iterate through the assets
            foreach (TDAsset asset in Assets)

                // Create a line in the file from this assset
                if (asset.TDAssetType == "CURRENCY")
                {
                    lines.Add($"{SymbolNameDict[asset.Symbol]},{asset.Price},{asset.Spread},{asset.forex.RolloverLong}," +
                        $"{asset.forex.RolloverShort},{asset.Pip},{asset.PipCost},{asset.MarginCost},100,{asset.LotAmount}," +
                        $"0,{asset.Symbol}");
                }
            else
                {
                    lines.Add($"{SymbolNameDict[asset.Symbol]},{asset.Price},{asset.Spread},{0.00}," +
                        $"{0.00},{asset.Pip},{asset.PipCost},{asset.MarginCost},100,{asset.LotAmount}," +
                        $"0,{asset.Symbol}");
                }

            // Write all the lines out to the CSV file
            File.WriteAllLines(Broker.WORKING_DIR + "/History/AssetsFix.csv", lines);
        }

        //********************************************************************
        //  Method: GetSymbols
        //
        /// <summary>
        /// Read the Assets CSV file and get the symbols. 
        /// </summary>
        /// 
        /// <returns>
        /// A JSON string with the quote information.
        /// </returns>
        //********************************************************************
        private static string
            GetSymbols
            ()
        {
            // Method members
            string line;
            string symbol = null;
            string[] cells;
            string symbols = "symbol=";
            int lastIndex;
            string name = null;
            string sym = null;

            // Read all lines of the CSV file and get the symbols
            Lines = File.ReadAllLines(Broker.WORKING_DIR + ASSETS_CSV_FILE);

            // Iterate through the lines of the CSV file, skipping the header
            for (int i = 1; i < Lines.Length; ++i)
            {
                // Get the line
                line = Lines[i];

                // Break apart the line
                cells = line.Split(',');

                // Get the last index
                lastIndex = cells.Length - 1;

                // Get the name (0) and the symbol (last)
                name = cells[0];
                sym = cells[lastIndex];

                // Get the symbol. If the last cell is not empty use it as
                // the symbol. If it is empty use the first cell as symbol
                symbol = string.IsNullOrEmpty(sym) ? name : sym;

                // Was a symbol found?
                if (!string.IsNullOrEmpty(symbol))
                    symbols += symbol + ",";

                // Add an entry to the name/asset dictionary
                if (!SymbolNameDict.ContainsKey(sym))
                    SymbolNameDict.Add(sym, name);
            }

            // Remove the last comma from the symbols list
            return symbols.Trim(new char[] { ',' });
        }

        //********************************************************************
        //  Method: GetQuotes
        //
        /// <summary>
        /// Call the TD Ameritrade REST API to get quotes for all symbols in
        /// the AssetsFix CSV file.
        /// </summary>
        /// 
        /// <param name="symbols">
        /// Symbols in the assets file.
        /// </param>
        /// 
        /// <returns>
        /// A list of TDAssets with one entry per line of the CSV file
        /// </returns>
        //********************************************************************
        private static async Task<List<TDAsset>>
            GetQuotes
            (
                string symbols
            )
        {
            // Method members
            List<EquityETF> equityList = new List<EquityETF>();
            List<MutualFund> mutualFundList = new List<MutualFund>();
            List<Forex> forexList = new List<Forex>();
            List<TDAsset> assetList = new List<TDAsset>();
            Dictionary<string, object> assetDict = new Dictionary<string, object>();

            // Were symbols obtained?
            if (!string.IsNullOrEmpty(symbols))
            {
                // YES: Get the TD Ameritrade REST API object
                TDAmeritradeREST rest = new TDAmeritradeREST();

                // Call TD Ameritratde API to get current quotes for these symbols
                string result = rest.QueryApi(

                    // The TD Ameritrade API method
                    ApiMethod.GetQuotes,

                    // The data object passed to the API method
                    ApiHelper.AccountDataWithQueryString(

                        // No data in URL before query string
                        null,

                        // The query string used to get the quotes
                        symbols,

                        // Use authentication
                        true
                        )
                    );

                // Is the result not in error?
                if (!string.IsNullOrEmpty(result) && !result.StartsWith("ERROR:"))
                {
                    // NO: Create a non-data contract JSON serializer
                    JavaScriptSerializer serializer = new JavaScriptSerializer();

                    // Deserialize the result returned from TD Ameritrade into
                    // the 1st level dictionary, i.e. <string, object>
                    assetDict = (Dictionary<string, object>)serializer.Deserialize<object>(result);

                    // Iterate through each asset in the asset dictionary
                    foreach (string key in assetDict.Keys)
                    {
                        // The 2nd level object for each asset is also a 
                        // <string, object> dictionary. What type of asset?
                        switch (((Dictionary<string, object>)assetDict[key])["assetType"].ToString())
                        {
                            case "EQUITY":
                            case "ETF":
                                // Convert object to an Equity/ETF class object
                                equityList.Add(Broker.JsonDictConvert<EquityETF>((Dictionary<string, object>)assetDict[key]));
                                break;

                            case "MUTUAL_FUND":
                                // Convert object to a mutual fund class object
                                mutualFundList.Add(Broker.JsonDictConvert<MutualFund>((Dictionary<string, object>)assetDict[key]));
                                break;

                            case "FOREX":
                                //*********************************************
                                // NOTE: The TD Ameritrade REST API currently
                                // does not support FOREX trading.
                                //*********************************************
                                // Convert object to a Forex class object
                                //forexList.Add(Broker.JsonDictConvert<Forex>((Dictionary<string, object>)assetDict[key]));
                                break;

                            default:
                                break;
                        }
                    }

                    // Iterate through the equities and ETFs
                    foreach (EquityETF asset in equityList)
                    {
                        // Get the ask price
                        double askPrice = Broker.GetAskPrice(asset);

                        // If no ask price can be determined, do not trade this security
                        if (askPrice < -9000.0) continue;

                        // Get the bid price
                        double bidPrice = Broker.GetBidPrice(asset);

                        assetList.Add(new TDAsset
                        {
                            Symbol = asset.Symbol,
                            Price = asset.AskPrice,
                            Spread = askPrice - bidPrice,
                            Volume = 0,
                            LotAmount = 1,
                            MarginCost = 0.00,
                            Pip = 0.01,
                            PipCost = 0.01
                        });
                    }

                    // Iterate through the mutual funds
                    foreach (MutualFund asset in mutualFundList)
                    {
                        // Get the ask price
                        double askPrice = Broker.GetAskPrice(asset);

                        // If no ask price can be determined, do not trade this security
                        if (askPrice < -9000.0) continue;

                        // Get the bid price
                        double bidPrice = Broker.GetBidPrice(asset);

                        assetList.Add(new TDAsset
                        {
                            Symbol = asset.Symbol,
                            Price = asset.AskPrice,
                            Spread = askPrice - bidPrice,
                            Volume = 0,
                            LotAmount = 1,
                            MarginCost = 0.00,
                            Pip = 0.01,
                            PipCost = 0.01
                        });
                    }

                    //*********************************************************
                    // NOTE: FOREX trading is not currently supported by the
                    // TD Ameritrade REST API.
                    //
                    // Iterate through the Forex assets. Main issue is to
                    // compute the pipCost and the Rollover interest fees:
                    //
                    // PipCost = LotAmount * Pip / ( {Counter Currency} / {Account Currency} )
                    //*********************************************************
                    /*
                    foreach (Forex asset in forexList)
                    {
                        // Get the ask price
                        double askPrice = Broker.GetAskPrice(asset);

                        // If no ask price can be determined, do not trade this security
                        if (askPrice < -9000.0) continue;

                        // Get the bid price
                        double bidPrice = Broker.GetBidPrice(asset);

                        // Get the rollover rates
                        double ROR = CurrencyInterestRates.ComputeRollover(asset.Symbol, askPrice);

                        // Round the rollover to five places
                        ROR = Math.Round(ROR, 5);

                        assetList.Add(new TDAsset
                        {
                            Symbol = asset.Symbol,
                            Price = asset.AskPrice,
                            Spread = askPrice - bidPrice,
                            Volume = 0,
                            LotAmount = 10000,
                            MarginCost = 0.00,
                            Pip = (asset.Symbol.Contains("/JPY") || asset.Symbol.Contains("JPY/")) ? 0.01 : 0.0001,
                            PipCost = Broker.ComputePipCost(asset.Symbol),
                            RolloverLong = ROR,
                            RolloverShort = ROR
                        });
                    }
                    */
                }
            }

            // Return the asset list
            return assetList;
        }
    }
}
