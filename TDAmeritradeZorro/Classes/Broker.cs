//*****************************************************************************
// File: Broker.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: Implementation of the Zorro Broker methods in C# using the
// DLLExport library for exposiing entry points in the DLL.
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
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using TDAmeritradeZorro.Classes.TDA;
using TDAmeritradeZorro.Classes.TDA.Assets;
using TDAmeritradeZorro.Interface;
using TDAmeritradeZorro.Authentication.Client;
using TDAmeritradeZorro.Authentication.Configuration;
using TDAmeritradeZorro.Authentication;
using TDAmeritradeZorro.Structs;
using TDAmeritradeZorro.Utilities;
using TDAmeritradeZorro.WebApi.Classes;
using System.Windows.Forms;
using DBLib.Classes;
using System.Reflection;

namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Class: Broker
    //
    /// <summary>
    /// A static class to implement the various Broker methods called by Zorro.
    /// </summary>
    //*************************************************************************
    public static class Broker
    {
        #region PRIVATE MEMBERS
        //*********************************************************************
        //  Member: rest
        //
        /// <summary>
        /// The TD Ameritrade REST API object that allows for communication 
        /// with the TD Ameritrade system.
        /// </summary>
        //*********************************************************************
        private static TDAmeritradeREST rest = null;

        //*********************************************************************
        //  Member: CurrentSymbol
        //
        /// <summary>
        /// The symbol used for subsequent commands set by the SET_SYMBOL
        /// command.
        /// </summary>
        //*********************************************************************
        private static string CurrentSymbol;

        //*********************************************************************
        //  Member: UnderlyingPrice
        //
        /// <summary>
        /// The underlying price for the previous BrokerAsset call when the
        /// asset was an option
        /// command.
        /// </summary>
        //*********************************************************************
        private static double UnderlyingPrice;

        //*********************************************************************
        //  Member: diagnostics
        //
        /// <summary>
        /// Whether messages between the trading engine and plug-in are logged.
        /// </summary>
        /// 
        /// <remarks>
        /// True = messages logged; False = messages not logged.
        /// </remarks>
        //*********************************************************************
        private static bool diagnostics = false;

        //*********************************************************************
        //  Member: jsonByOrderNum
        //
        /// <summary>
        /// Dictionary used to store JSON payload by order number
        /// </summary>
        /// </remarks>
        //*********************************************************************
        private static Dictionary<long, string> jsonByOrderNum;

        //*********************************************************************
        //  Members: Registry sub-key names 
        //
        /// <summary>
        /// Various registry ub-key names for the plug-in main registery entry.
        /// </summary>
        //*********************************************************************
        private static readonly string LICENSE_ACCEPTANCE = "LicenseAcceptance";
        public static readonly string VERSION_NUMBER = "VersionNumber";
        private static readonly string VERSION_BUILD_DATE = "VersionDate";

        //*********************************************************************
        //  Members: DATE_FORMAT_FULL
        //
        /// <summary>
        /// A date format string
        /// </summary>
        /// 
        /// <remarks>
        /// Format date as Thu, 21 May 2020 14:23:39
        /// </remarks>
        //*********************************************************************
        private static readonly string DATE_FORMAT_FULL = "ddd, dd MMM yyy HH:mm:ss";
        #endregion PRIVATE MEMBERS

        #region PUBLIC MEMBERS
        //*********************************************************************
        //  Member: jsonNull
        //
        /// <summary>
        /// A NULL JSON object.
        /// </summary>
        //*********************************************************************
        public static readonly string jsonNull = "{}";

        //*********************************************************************
        //  Member: subscriptionList
        //
        /// <summary>
        /// A list containing which assets have been subscribed to by Zorro.
        /// </summary>
        //*********************************************************************
        public static List<TDAsset> subscriptionList;

        //*********************************************************************
        //  Member: SETTINGS_FILE
        //
        /// <summary>
        /// The JSON file located in the Zorro's data folder that contains:
        ///     (1) Account currency;
        ///     (2) TD Ameritrade account id; and,
        ///     (3) TD Ameritrade client id
        /// </summary>
        //*********************************************************************
        public static readonly string SETTINGS_FILE = "/Data/tda.json";

        //*********************************************************************
        //  Member: ComboLegs
        //
        /// <summary>
        /// Whether to treat the following N orders as a combo order
        /// </summary>
        /// 
        /// <remarks>
        /// The number of following order to treat as a combo order, OPTION
        /// trading only.
        /// </remarks>
        //*********************************************************************
        public static int ComboLegs = 0;

        //********************************************************************
        //  Member: WORKING_DIR
        //
        /// <summary>
        /// The working directory where Zorro is running
        /// </summary>
        //********************************************************************
        public static string WORKING_DIR;

        //********************************************************************
        //  Member: sessionType
        //
        /// <summary>
        /// The type of session ("Real" or "Demo").
        /// </summary>
        //********************************************************************
        public static string sessionType;

        //********************************************************************
        //  Member: Settings
        //
        /// <summary>
        /// Settings for this user. See SETTINGS_FILE.
        /// </summary>
        //********************************************************************
        public static Settings settings;

        //*********************************************************************
        //  Member: oAuthConfiguration
        //
        /// <summary>
        /// Configuration data used for authentication.
        /// </summary>
        //*********************************************************************
        public static IClientConfiguration oAuthConfiguration;

        //*********************************************************************
        //  Member: oAuthClient
        //
        /// <summary>
        /// Client data used for authentication.
        /// </summary>
        //*********************************************************************
        public static IClient oAuthClient;

        //*********************************************************************
        //  Member: dataFile
        //
        /// <summary>
        /// The directory and name of the file used to save tokens.
        /// </summary>
        //*********************************************************************
        public static string dataFile = "/Data/auth.dat";

        //*********************************************************************
        //  Member: tokenDataFile
        //
        /// <summary>
        /// The full path name of the file used to save tokens.
        /// </summary>
        //*********************************************************************
        public static string tokenDataFile;

        //*********************************************************************
        //  Member: sellSellShort
        //
        /// <summary>
        /// What to do when SELLING more lots than owned.
        /// </summary>
        /// 
        /// <remarks>
        /// It's possible to isuue a SELL order to the TD Ameritrade REST API
        /// for an amount of shares greater than the amount owned. The normal
        /// response of TD Ameritrade is to then create two orders: one SELL
        /// order for the amount owned, and another SELL SHORT for the balance.
        /// This enumeration governs what to do in that case, with the follow-
        /// ing possibilities:
        /// 
        /// 0 = Cancel the SELL ordor;
        /// 1 = Fill the SELL order for amount of shares owned (default);
        /// 2 = SELL shares owned, SELL SHORT shares not owned (without doing
        ///     anything this is TD Ameritrade default)
        /// </remarks>
        //*********************************************************************
        public static SellSellShort sellSellShort = SellSellShort.Adjust;
        #endregion PUBLIC MEMBERS

        #region PUBLIC METHODS TO IMPLEMENT THE ZORRO BROKER METHODS
        //*********************************************************************
        //  Method: Open
        //
        /// <summary>
        /// Called by Zorro's BrokerOpen method for further initialization
        /// </summary>
        //*********************************************************************
        public static void
            Open
            ()
        {
            // Set the working directory
            WORKING_DIR = Directory.GetCurrentDirectory();
        }

        //*********************************************************************
        //  Method: Login
        //
        /// <summary>
        /// Called by Zorro's BrokerLogin method to authenticate the user on 
        /// the TD Ameritrade server.
        /// </summary>
        /// 
        /// <returns>
        /// TRUE if the login is a success an an aothentication token has been
        /// retrieved, FALSE othenwise.
        /// </returns>
        //*********************************************************************
        public static bool
            Login
            ()
        {
            // Method members
            bool retCode;
            AuthToken token = null;

            try
            {
                // Has the license been accepted?
                if (!LicenseAccepted())
                {
                    // NO: Give user a chance to accept the license; accepted?
                    if (!ShowLicenseForm())
                    {
                        // NO: Log the error
                        LogHelper.Log(LogLevel.Error, $"{Resx.GetString("LICENSE_NOT_ACCEPTED")}");

                        return false;
                    }
                }

                // Is plug-in in test mode?
                if (TDAmerAPI.opMode == OpMode.Demo)
                {
                    // YES: Test mode is Demo mode. Run through the tests of the
                    // broker plug-in.
                    retCode = Tests.DoTests(settings.ClientId);

                    // Tests successful?
                    if (retCode)
                    {
                        // YES:
                        LogHelper.Log(LogLevel.Info, $"\r\n{Resx.GetString("SUCCESS").ToUpper()}: {Resx.GetString("SUCCESS_IN_TESTS")}.");
                    }
                    else
                    {
                        // NO:
                        LogHelper.Log(LogLevel.Error, $"\r\n{Resx.GetString("FAILURE").ToUpper()}: {Resx.GetString("ERRORS_IN_TESTS")}.");
                    }

                    // Return a failure code, so processing stops with this method.
                    return false;
                }

                // NO: Live trading mode
                LogHelper.Log($"    {Resx.GetString("READING")} {Resx.GetString("SETTINGS")} {Resx.GetString("FILE")}...");

                // Initialize the currency interest rate dictionary. Use USD to
                // force a read of all rates except HKD. Then, do HKB separately.
                LogHelper.Log($"    {Resx.GetString("INTEREST_RATE_INIT")}...");

                //*************************************************************
                // 
                // NOTE: The TD Ameritrade REST API does not currently support 
                // FOREX trading. Uncomment the following code to initialize
                // currency interest rates if that changes in the future.
                // 
                //*************************************************************
                // Do the initialization of the currency interest rates
                /*
                retCode = CurrencyInterestRates.InitCurrencyInterestRates();

                // Log the result of currency interest rate initialization
                if (retCode)
                    LogHelper.Log($"    {Resx.GetString("SUCCESS")}");
                else
                    LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("FAILURE")}");
                */

                // Save the full path of the token data file
                tokenDataFile = WORKING_DIR + dataFile;

                // Initialize the connection configuration using the refresh token
                // (usn) and the client id obtained from the settings
                oAuthConfiguration = new TDAmeritradeConnectConfiguration(settings.ClientId);

                // Initialize the connection client with the configuration
                oAuthClient = new TDAmeritradeConnectClient(oAuthConfiguration);

                // Does the token file exist?
                if (File.Exists(tokenDataFile))
                {
                    // YES: Log authentication with refresh token
                    LogHelper.Log($"    {Resx.GetString("RETRIEVING")} {Resx.GetString("REFRESH_TOKEN")}.");

                    // Get an authentication token using a refresh token
                    token = AuthToken.GetAuthToken();
                }
                else
                {
                    // NO: This must be the initial use of the plug-in, in that
                    // case get an access token by fully authenticating user
                    // with the client id.
                    LogHelper.Log($"    {Resx.GetString("AUTHENTICATING_USER")}...");

                    // Execute the longer, one time only, user authentication.
                    token = AuthToken.AuthenticateUser(settings.ClientId);
                }

                // Has access token been obtained?
                if (token != null)
                {
                    // YES: Has the user accepted the license terms?
                    return LicenseAccepted();
                }
                else
                {
                    // NO: Return failure
                    return false;
                }
            }
            catch (Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("UNSPECIFIED_ERROR")} {Resx.GetString("ERROR")}: {Resx.GetString("BROKER_LOGIN")} " + e.Message);
            }

            // Past here return failure 
            return false;
        }

        //*********************************************************************
        //  Method: CreateDbTables
        //
        /// <summary>
        /// Create DB tables required by this plug-in, if they do not exist.
        /// </summary>
        /// 
        /// <returns>
        /// True if tables created succcessfully, otherwise false.
        /// </returns>
        //*********************************************************************
        public static bool
            CreateDbTables
            ()
        {
            // The database success object
            DBSuccess dbSuccess = new DBSuccess();

            // Create the Trade database table
            dbSuccess = DataAccess.CreateTable<Trade>();

            // Was creation successful?
            if (dbSuccess.Success)
            {
                // YES: Create the TradeXref table
                dbSuccess = DataAccess.CreateTable<TradeXref>();

                // Was creation successful?
                if (dbSuccess.Success)
                {
                    // YES: Create the TradeId table
                    dbSuccess = DataAccess.CreateTable<TradeId>();

                    // Was creation successful?
                    if (dbSuccess.Success)
                    {
                        // YES: Return the success code
                        return true;
                    }
                    else
                    {
                        // NO: Log the error message
                        LogHelper.Log(LogLevel.Error, dbSuccess.ErrorMsg);
                    }
                }
                else
                {
                    // NO: Log the error message
                    LogHelper.Log(LogLevel.Error, dbSuccess.ErrorMsg);
                }
            }
            else
            {
                // NO: Log the error message
                LogHelper.Log(LogLevel.Error, dbSuccess.ErrorMsg);
            }

            // Return failure code
            return false;
        }

        //*********************************************************************
        //  Method: Time
        //
        /// <summary>
        /// This is an implementation of the BrokerTime call from Zorro to
        /// determine is the connection exists, to get the server time, and to
        /// return whether a trading market is open.
        /// 
        /// It is possible that the user is making both EQUITY and FOREK trades
        /// at the same time. These are two different markets with different
        /// hours. This method will return whether the EQUITY market (NYSE) is
        /// open. But before each trade a test for market open conditions will
        /// be made again.
        /// </summary>
        /// 
        /// <param name="serverTime">
        /// OUTPUT: Server time as double.
        /// </param>
        /// 
        /// <returns>
        /// 0 = Something wrong with connection to the TD Ameritrade server.
        /// 1 = Server connection OK, NYSE is CLOSED
        /// 2 = Server connection OK, NYSE OPEN
        /// </returns>
        //*********************************************************************
        public static int
            Time
            (
               out double? serverTime
            )
        {
            // Set the server time (out variable)
            serverTime = null;

            try
            {
                // Get EQUITY (NYSE) market hours
                MarketHours hours = GetMarketHours("EQUITY");

                // If the return is null, then something is wrong with connection.
                if (hours == null) return 0;

                // Set the server time
                serverTime = hours.ServerTime;

                // If the market is closed, return "1"
                if (!IsOpen(hours)) return 1;

                // Otherwise, connection is good, market is open
                return 2;
            }
            catch(Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: Broker Time method. " + e.Message);
            }

            // Past here, return failure code
            return 0;
        }

        //*********************************************************************
        //  Method: Asset
        //
        /// <summary>
        /// Called by Zorro's BrokerAsset method to retrieve information on an
        /// asset traded with TD Ameriitrade. Currently the plugin is only
        /// supporting trades of FOREX pairs, Equites, and Indices (via ETFs).
        /// </summary>
        /// 
        /// <param name="symbol">
        /// The name (symbol) of the asset for which information is being 
        /// retrieved from TD Ameritrade.
        /// </param>
        /// 
        /// <returns>
        /// A TDAsset class object with information filled in, or null if the
        /// asset cannot be found and therefore the asset cannot be traded.
        /// </returns>
        //*********************************************************************
        public static TDAsset
            Asset
            (
            string symbol
            )
        {
            // Method members
            Dictionary<string, object> assetDict = new Dictionary<string, object>();
            TDAsset asset = null;

            // Initialize the underlying price as if this was not an option 
            UnderlyingPrice = -9999.99;

            try
            {
                // Parse this asset's symbol representation
                asset = new TDAsset(symbol);

                // Get a quote for this symbol
                string quoteStr = GetQuote(asset.TickerSymbol);

                // If quote data not found, return a null TDAsset object
                if (string.IsNullOrEmpty(quoteStr) || quoteStr == jsonNull) return null;

                // Past here, quote data has been returned, convert the string data
                // into a JSON object. Create a first level <string, object> dict.

                // Create a non-data contract JSON serializer
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                // Deserialize the result returned from TD Ameritrade into
                // the 1st level dictionary, i.e. <string, object>
                assetDict = (Dictionary<string, object>)serializer.Deserialize<object>(quoteStr);

                // Process the asset based on its type (STK, ETF, OPT, FUT, FUTX, etc)
                switch (asset.AssetType)
                {
                    case "STK":
                    case "ETF":
                        asset = ProcessEquityETF(asset, JsonDictConvert<EquityETF>((Dictionary<string, object>)assetDict[asset.TickerSymbol]));
                        break;

                    case "OPT":
                        asset = ProcessOption(asset, JsonDictConvert<Option>((Dictionary<string, object>)assetDict[asset.TickerSymbol]));
                        break;

                    case "FUND":
                        asset = ProcessMutualFund(asset, JsonDictConvert<MutualFund>((Dictionary<string, object>)assetDict[asset.TickerSymbol]));
                        break;

                    //*********************************************************
                    //
                    // NOTE: The TD Ameritrade REST API currently does not
                    // support FOREX trading. Uncomment the follow code if that
                    // changes in the future.
                    //
                    //*********************************************************
                    /*
                    case "currency":
                    case "forex":
                        asset = ProcessForex(JsonDictConvert<Forex>((Dictionary<string, object>)assetDict[symbol]));
                        break;
                    */

                    // For everything else, return a null asset object
                    default:
                        return null;
                }

                // Return the asset
                return asset;
            }
            catch (Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("BROKER_ASSET")}. " + e.Message);
            }

            // Past here, return failure
            return null;
        }

        //*********************************************************************
        //  Method: History
        //
        /// <summary>
        /// Implements the Zorro "BrokerHistory" method to return price history
        /// for an asset over a given period.
        /// </summary>
        /// 
        /// <param name="asset">
        /// INPUT: THe name of the asset, e.g. "EUR/USD" or "MSFT." 
        /// </param>
        /// 
        /// <param name="tStart">
        /// INPUT: UTC start date/time of the price history in OLE format. This
        /// has only the meaning of a seek-no-further date; the relevant date for 
        /// the begin of the history is tEnd.
        /// </param>
        /// 
        /// <param name="tEnd">
        /// INPUT: UTC END date/time of the price history. If the price history
        /// is not available in UTC time, but in the brokers's local time, the 
        /// plugin must convert it to UTC.
        /// </param>
        /// 
        /// <param name="nTickMinutes">
        /// INPUT: The time period of a tick in minutes. Usual values are 0 for 
        /// single price ticks (T1 data; optional), 1 for one-minute (M1) 
        /// historical data, or a larger value for more quickly filling the 
        /// LookBack period before starting a strategy. I.E. if nTickMinutes
        /// is 60, then the time period of a tick is one-hour (60 minutes
        /// per tick).
        /// </param>
        /// 
        /// <param name="nTicks">
        /// INPUT: The maximum number of ticks to be filled; guaranteed to be 
        /// 300 or less.
        /// </param>
        /// 
        /// <returns>
        /// An array of Tick strutures for which there must be no more than
        /// 300.
        /// </returns>
        //*********************************************************************
        public static Tick[]
            History
            (
            string asset,
            double tStart,
            double tEnd,
            int nTickMinutes,
            int nTicks
            )
        {
            // Method members
            int[] ticksPerMinute = new int[] { 30, 15, 10, 5, 1 };
            string queryString = string.Empty;
            int frequency = 0;
            int skip = 0;
            List<Tick> ticks = new List<Tick>();
            Dictionary<string, object> priceDict = new Dictionary<string, object>();
            List<OHLC> prices = new List<OHLC>();
            PriceHistory priceHistory;

            try
            {
                // Get a reference to REST API, if needed
                if (rest == null) rest = new TDAmeritradeREST();

                // Convert the START and END dates from OLE values to DateTime
                // values, then get the time in milliseconds since the UNIX
                // epoch.
                long Start = ToUnixEpoch(DateTime.FromOADate(tStart));
                long End = ToUnixEpoch(DateTime.FromOADate(tEnd));

                // Iterate through the ticks per minute and find out which one
                // nTickMinutes is a multiple of
                for (int i = 0; i < ticksPerMinute.Length - 1; ++i)
                {
                    if ((nTickMinutes % ticksPerMinute[i]) == 0)
                    {
                        frequency = ticksPerMinute[i];
                        break;
                    }
                }

                // Frequency should never be one, because any value of 
                // nTickMinutes should be divisible by (frequency = 1)
                if (frequency == 0)
                {
                    // Log an error
                    LogHelper.Log(LogLevel.Error, "ERROR: Getting correct tick count.");
                    return ticks.ToArray();
                }

                // Create the TD Ameritrade query string
                queryString = $"periodType=day&frequencyType=minute&frequency={frequency}&" +
                    $"endDate={End}&startDate={Start}&needExtendedHoursData=false";

                // Call the TD Ameritade API to get the price history
                string result = rest.QueryApi(

                    // The TD Ameritrade API method
                    ApiMethod.GetPriceHistory,

                    // The data object passed to the API method
                    ApiHelper.AccountDataWithQueryString(

                        // Data in URL before query string (symbol)
                        asset,

                        // The query string for the price history
                        queryString,

                        // Use authentication
                        true
                        )
                    );

                // Did the TD Ameritrade server return an error?
                if (!result.StartsWith("ERROR:"))
                {
                    // NO: Valid JSON result. Deserialize it to a PriceHistory POCO
                    priceHistory = DeserializeJson<PriceHistory>(result);

                    // Have any candles been returned?
                    if (priceHistory.Candles.Count > 0)
                    {
                        // YES: Has only one candle been returned?
                        if (priceHistory.Candles.Count == 1)
                        {
                            // YES: Get the last candle, which will be the only candle
                            // if there is only 1 tick returned
                            var pCandle = priceHistory.Candles[priceHistory.Candles.Count - 1];

                            // Give all ticks the values of this candle
                            for (int i = 0; i < nTicks; ++i)
                            {
                                // Add this candle to the tick list
                                ticks.Add(new Tick
                                {
                                    fOpen = (float)pCandle.Open,
                                    fClose = (float)pCandle.Close,
                                    fHigh = (float)pCandle.High,
                                    fLow = (float)pCandle.High,
                                    time = DateTimeOffset.FromUnixTimeMilliseconds(pCandle.Date).DateTime.ToUniversalTime().ToOADate()
                                });
                            }
                        }
                        else
                        {
                            // NO: Compute the skip period, in the case that
                            // nTickMinutes is actually greater than frequency
                            if (nTickMinutes > frequency) skip = nTickMinutes / frequency;

                            // Zorro expects the most recent ticks to be first, so 
                            // start from the last element of the "candles" property
                            for (int i = priceHistory.Candles.Count - 1; i >= 0; i = i - skip)
                            {
                                // If adding this candle create a list that has more 
                                // than nTick elements, or there would be more than 300
                                // elements in the list, then exit from this loop
                                if (ticks.Count >= nTicks || ticks.Count >= 300) break;

                                // Get the OHLC candle
                                var candle = priceHistory.Candles[i];

                                // Get the candle date
                                double candleDate = DateTimeOffset.FromUnixTimeMilliseconds(candle.Date).DateTime.ToUniversalTime().ToOADate();

                                // If the date of candle is prior to start date,
                                // then exit from this loop
                                if (candleDate < tStart) break;

                                // If the date of candle is greater than the
                                // end date continue processing ticks until 
                                // getting to one within the date range
                                if (candleDate > tEnd) continue;

                                // Add this candle to the tick list, after
                                // converting date to UTC time in OLE format
                                ticks.Add(new Tick
                                {
                                    fOpen = (float)candle.Open,
                                    fClose = (float)candle.Close,
                                    fHigh = (float)candle.High,
                                    fLow = (float)candle.High,
                                    time = DateTimeOffset.FromUnixTimeMilliseconds(candle.Date).DateTime.ToUniversalTime().ToOADate(),
                                    fVol = Convert.ToSingle(candle.Volume)
                                });
                            }
                        }
                    }
                }
                else
                {
                    // YES: An error was encountered getting the price history from
                    // the TD Ameritrade server. Log the error.
                    LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("PRICE_HISTORY")} {asset}.");
                }

                // Return the list of ticks as an array of ticks
                return ticks.ToArray();
            }
            catch (Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("BROKER_HISTORY2")}. " + e.Message);
            }

            // Past here, return an empty array
            return (new List<Tick>()).ToArray();
        }

        //*********************************************************************
        //  Method: Account
        //
        /// <summary>
        /// Implements the Zorro "BrokerAccount" method to return the current
        /// account status in terms of balances.
        /// </summary>
        /// 
        /// <param name="AccountId">
        /// The account id for the TD Ameritrade account.
        /// </param>
        /// 
        /// <returns>
        /// An AccountBalance object with balance information about the account
        /// or null if the account is not found, or the balance information is
        /// not available.
        /// </returns>
        //*********************************************************************
        public static AccountBalance
            Account
            (
            string AccountId,
            string acctType = "positions,orders"
            )
        {
            // Method members
            UserAccount account = null;
            AccountBalance balance = null;

            try
            {
                // Get a new TD Ameritrade REST API object, if needed
                if (rest == null) rest = new TDAmeritradeREST();

                // Get account information (balances, positions, orders)
                string result = rest.QueryApi(
                    ApiMethod.GetAccount,
                    ApiHelper.AccountDataWithQueryString(
                        AccountId,
                        string.Format("fields={0}", acctType),
                        true
                        )
                    );

                // Did the TD Ameritrade REST server return an error?
                if (result.StartsWith("ERROR:"))
                {
                    // YES: Log the error
                    LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("RETRIEVING")} {Resx.GetString("INFO")} {Resx.GetString("TDAMTRADE")} {Resx.GetString("ACCOUNT_NUM")}  {AccountId}");
                }
                else
                {
                    // NO: Convert from JSON to POCO
                    account = DeserializeJson<UserAccount>(result);

                    // Fill a new balance object with account balances
                    balance = new AccountBalance
                    {
                        AccountId = AccountId,
                        Balance = account.Account.CurrBalances.AvailableFundsNonMarginableTrade,
                        TradeValue = account.Account.CurrBalances.Equity - account.Account.CurrBalances.AvailableFundsNonMarginableTrade,
                        MarginValue = account.Account.CurrBalances.MarginBalance
                    };

                }

                // Return the account balance object
                return balance;
            }
            catch (Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("BROKER_ACCOUNT")}. " + e.Message);
            }

            // Past here, return failure
            return null;
        }

        //*********************************************************************
        //  Method: Buy
        //
        /// <summary>
        /// Implements the Zorro "BrokerBuy2" method returning both the status
        /// of the order fulfillment, and the purchase price of the asset.
        /// </summary>
        /// 
        /// <returns>
        /// A Trade object with trade information.
        /// </returns>
        /// 
        /// <remarks>
        /// See "BrokerBuy2" for all relevant information on this method's
        /// parameters.
        /// </remarks>
        //*********************************************************************
        public static Trade
            Buy
            (
            string asset,
            int amount,
            double stopDist,
            double limit,
            string session = "NORMAL"
            )
        {
            // Method members
            long orderId = 0L;
            TDAsset tdAsset;
            OrderSubmission order;
            bool toClose = false;
            try
            {
                // Is this a "TO_CLOSE" OPTION purchase?
                if (asset.EndsWith("-TO_CLOSE"))
                {
                    // YES: Strip off the "TO_CLOSE" ending
                    asset.Replace("-TO_CLOSE", "");

                    // Set the to close boolean
                    toClose = true;
                }

                // Look-up the asset in the subscription list
                tdAsset = subscriptionList.Where(s => s.Symbol == asset).SingleOrDefault();

                // Asset found in subscription list?
                if (tdAsset != null)
                {
                    // YES: Create an order submission object
                    order = new OrderSubmission
                    (
                        tdAsset,
                        amount,
                        limit,
                        stopDist,
                        toClose
                    );

                    // Submit the order
                    orderId = SubmitOrder(order);

                    // Was the trade entered?
                    if (orderId > 0L)
                    {                        
                        // YES: Get the order price by look-up, since TD Ameritrade
                        // does not return it with the PlaceOrder API method
                        Trade trade = GetTradeInfo(orderId, 0);

                        // Was the trade information returned?
                        if (trade != null)
                        {
                            // YES: Does the order JSON dictionary have an entry for this
                            // order id?
                            if (jsonByOrderNum.ContainsKey(orderId))
                            {
                                // YES: Get the order json
                                trade.OrderJson = jsonByOrderNum[orderId];

                                // Remove this entry from the dictionary
                                jsonByOrderNum.Remove(orderId);
                            }
                            else
                            {
                                // NO: No entry for this order id found
                                trade.OrderJson = jsonNull;
                            }

                            // Save this trade, which will automatically assign it
                            // a Zorro trade id
                            trade.Save();

                            // Return the trade 
                            return trade;
                        }
                        else
                        {
                            // NO: Trade information not returned, usually a result of
                            // a trade being REJECTED. Set order id to NOT FILLED
                            orderId = 0L;
                        }


                    }
                }
                else
                {
                    // NO: Asset not found in subscription list
                    LogHelper.Log(LogLevel.Error, $"    {tdAsset.TickerSymbol} {Resx.GetString("NOT_FOUND_IN_SUBSCRIPTIONS")}.");
                }
            }
            catch (Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("BROKER_BUY2")}. " + e.Message);
            }

            // Problem with trade, return dummy order with the correct TradeId,
            // which is return from trying to place the trade which failed.
            // This trade will never find its way to the trade list or to the
            // JSON db that backs that list. It is only used to convey the
            // Trade Id to Zorro in the case that the Trade Id is .lt. zero.
            return new Trade
            {
                Asset = asset,
                Quantity = amount,
                Price = -0.10,
                ZorroTradeId = -1,
                TDTradeId = orderId
            };
        }

        //*********************************************************************
        //  Method: Sell
        //
        /// <summary>
        /// Closes a trade either completely or partially, at MARKET; only for 
        /// non-NFA compliant accounts. Partial closing is seen primarily in
        /// FOREX trading, and currently it appears that TD Ameritrade does not
        /// suuport partial closing as it is NFA compliant.
        /// </summary>
        /// 
        /// <param name="ZorroTradeId">
        /// The Zorro trade ID as returned by BrokerBuy2.
        /// </param>
        /// 
        /// <param name="Amount">
        /// The number of contracts or lots to be closed, positive for a LONG
        /// trade and negative for a SHORT trade (see BrokerBuy2). If less
        /// than the original size of the trade, the trade is partially closed.
        /// </param>
        /// 
        /// <param name="Limit">
        /// The ask/bid price for a LIMIT order, or 0, if closing at MARKET.
        /// </param>
        /// 
        /// <returns>
        /// (1) Trade ID number of the remaining 'reduced' trade when it was 
        /// partially closed; OR,
        /// 
        /// (2) THe original trade ID number when it was completely closed; OR;
        /// 
        /// (3) 0 when the trade could not be closed. 
        /// </returns>
        /// 
        /// <remarks>
        /// See "BrokerSell2" of the Zorro Broker Plug-in API for further
        /// information about this method.
        /// </remarks>
        //*********************************************************************
        public static Trade
            Sell
            (
            int ZorroTradeId,
            int Amount,
            double Limit
            )
        {
            // Method members
            int success = 0;
            int failure = 0;
            Order tdaOrder;
            Trade tradeToClose;

            try
            {
                // Get the original trade information
                Trade trade = Trade.GetTradeByZorroId(ZorroTradeId);

                // Was the original trade found in the trades list?
                if (trade != null)
                {
                    // YES: Deserialize the order json
                    tdaOrder = DeserializeJson<Order>(trade.OrderJson);

                    // Iterate through each leg in the order leg collection
                    foreach (OrderLeg leg in tdaOrder.OrderLegCollection)
                    {
                        // Attempt to close the trade
                        tradeToClose = CloseTrade(ZorroTradeId, leg, Amount, Limit);

                        // Was the trade closed?
                        if (tradeToClose.TDTradeId != 0L)
                        {
                            // YES: Note trade to close was successful
                            ++success;
                        }
                        else
                        {
                            // NO: Not trade to close failde 
                            ++failure;
                        }
                    }

                    // Were all the trades to close successful?
                    if (failure == 0)
                    {
                        // YES: Place the original Zorro trade id in the status
                        // code of the trade
                        trade.StatusCode = ZorroTradeId;

                        if (success > 1)
                            LogHelper.Log($"{Resx.GetString("ALL_CLOSE_SUCCESS")} ");
                        else
                            LogHelper.Log(LogLevel.Error, $"{Resx.GetString("THIS_CLOSE_SUCCESS")} ");
                    }
                    else
                    {
                        // NO: Place a zero in the status code of the trade
                        trade.StatusCode = 0;

                        // Was this a multi-leg order?
                        if (tdaOrder.OrderLegCollection.Count > 1)
                        {
                            // YES: Multi-leg order
                            LogHelper.Log(LogLevel.Error, $"{Resx.GetString("SOME_COMBO_NOT_CLOSED")} ");
                        }
                        else
                        {
                            // NO: Single-leg order
                            LogHelper.Log(LogLevel.Error, $"{Resx.GetString("TRADE_NOT_CLOSED")} ");
                        }
                    }   

                    // Return the trade object
                    return trade;
                }
                else
                {
                    // NO: Log the error
                    LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("TRADE")} {ZorroTradeId} {Resx.GetString("NOT_FOUND")} {Resx.GetString("IN")}  {Resx.GetString("TRADE_LIST")}. " +
                        $"{ Resx.GetString("TRADE")} { ZorroTradeId} { Resx.GetString("TO_CLOSE")} { Resx.GetString("NOT_EXECUTED")}.");
                }
            }
            catch (Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: { Resx.GetString("TRADE")} { ZorroTradeId} { Resx.GetString("TO_CLOSE")} { Resx.GetString("NOT_EXECUTED")}. {e.Message}");
            }

            // PAST HERE: An error has been encountered, the error has been 
            // logged above, return with an error status code.
            return new Trade
            {
                StatusCode = 0
            };
        }

        //*********************************************************************
        //  Method: BrokerTrade
        //
        /// <summary>
        /// Gets the status of an open or recently closed trade. 
        /// </summary>
        /// 
        /// <param name="nTradeID">
        /// INPUT: The trade ID number as returned by BrokerBuy.
        /// </param>
        /// 
        /// <returns>
        /// (1) Number of contracts (amount) of the given trade ID number
        /// 
        /// (2) 0 when no trade with this ID could be found
        /// 
        /// (3) A negative number, if the trade was recently closed. 
        /// 
        /// NOTE: IF the returned value is nonzero, the output pointers must be
        /// filled.
        /// </returns>
        //*********************************************************************
        public static Trade
            BrokerTrade
            (
            int ZorroTradeId
            )
        {
            // Method member
            Trade trade = null;
            try
            {
                // Get the trade with the the Zorro Trade Id
                trade = Trade.GetTradeByZorroId(ZorroTradeId);

                // Valid trade returned from API?
                if (trade != null)
                {
                    // YES: What's the status of the trade?
                    switch (trade.Status)
                    {
                        // Trade already CLOSED 
                        case "CANCELED":
                        case "PENDING_CANCEL":
                        case "REPLACED":
                        case "PENDING_REPLACE":
                        case "EXPIRED":
                            trade.StatusCode = -1;
                            break;

                        // Trade is NOT FOUND
                        case "REJECTED":
                            trade.StatusCode = 0;
                            break;

                        // Trade is OPEN in some way
                        case "FILLED":
                        case "ACCEPTED":
                        case "QUEUED":
                        case "WORKING":
                        default:
                            trade.StatusCode = trade.Quantity;
                            break;
                    }
                }
                else
                {
                    // NO: Set the NOT FOUND status code
                    trade = new Trade { StatusCode = 0 };
                }
            }
            catch (Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("GETTING")} {Resx.GetString("STATUS")} {Resx.GetString("FROM")}  {Resx.GetString("BROKER_TRADE")}");

                // Return a null object
                return null;
            }

            // Return the trade
            return trade;
        }

        //*********************************************************************
        //  Method: Sync
        //
        /// <summary>
        /// Synchronize the trades saved to the client db with the trades
        /// on-file with TD Ameritraade.
        /// </summary>
        /// 
        /// <returns>
        /// TRUE if this method executed successfully, otherwise FALSE.
        /// </returns>
        /// 
        /// <remarks>
        /// If a trade is in the trades DB but not on the TD Ameritrade
        /// server, delete it from the trades DB. Cannot go the other way
        /// because user might be entering singleton trades on their on using
        /// TOS or the TD Ameritrade main interface.
        /// </remarks>
        //*********************************************************************
        public static bool

            Sync
            ()
        {
            // The deletion id list
            List<int> delList = new List<int>();

            try
            {
                // Method members
                Trade tdaTrade = null;
                List<Trade> newTradeList = new List<Trade>();

                // Get all the trades in the Trade DB table
                List<Trade> trades = DataAccess.GetAllRecords<Trade>();

                // Iterate through these trades
                foreach (Trade trade in trades)
                {
                    // Look-up this trade on TD Ameritrade
                    tdaTrade = GetTradeInfo(trade.TDTradeId, trade.ZorroTradeId);

                    // Was trade found on TD Ameritrade or is it CANCELED?
                    if (tdaTrade == null || tdaTrade.Status == "CANCELED")
                    {
                        // NO: Put on delete list
                        delList.Add(trade.Id);
                    }
                }

                // Are there items to delete?
                if (delList.Count > 0)
                {
                    // YES: Delete all in deletion id list
                    DataAccess.DeleteByIds<Trade>(delList);
                }

                // Return success code
                return true;
            }
            catch(Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("SYNCING")} {Resx.GetString("TRADE_LISTS")}.");
            }

            // Past here return non-success code.
            return false;
        }

        //*********************************************************************
        //  Method: Cancel
        //
        /// <summary>
        /// Cancel an order, given the Zorro Trade Id
        /// </summary>
        /// 
        /// <param name="ZorroTradeID">
        /// The Zorro order number to cancel.
        /// </param>
        /// 
        /// <returns>
        /// TRUE if the order was successfully canceled, FALSE otherwise.
        /// </returns>
        //*********************************************************************
        public static bool
            Cancel
            (
            int ZorroTradeId
            )
        {
            try
            {
                // Get the trade with the the Zorro Trade Id
                Trade trade = Trade.GetTradeByZorroId(ZorroTradeId);

                // Was the trade found?
                if (trade != null)
                {
                    // YES: Cancel the trade with the TDATradeId and return the
                    // value from the execution of the cancel method
                    return CancelOrder(trade.TDTradeId);
                }
            }
            catch(Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("CANCELING")} {Resx.GetString("TRADE")} {ZorroTradeId} ({Resx.GetString("ZORRO")}).");
            }

            // Past here return a failure code
            return false;
        }

        #endregion PUBLIC METHODS TO IMPLEMENT THE ZORRO BROKER METHODS

        #region PUBLIC METHODS TO IMPLEMENT THE ZORRO BROKER COMMANDS
        //*********************************************************************
        //  Method: SetLanguage
        //
        /// <summary>
        /// Set the language resource file to use.
        /// </summary>
        /// 
        /// <param name="nCommand">
        /// The SET_LANGUAGE command (4014)
        /// </param>
        /// 
        /// <param name="langResx">
        /// The language designator.
        /// </param>
        /// 
        /// <returns>
        /// Non-zero value (success), zero (failure).
        /// </returns>
        //*********************************************************************
        public static int
            SetLanguage
            (
            int nCommand,
            string langResx
            )
        {
            // Validate the language?
            if (ValidateLang(langResx))
            {
                // YES: Valid language. Set the language in SETTINGS
                settings.LangResx = langResx;

                // Return a non-zero value (success)
                return 1;
            }
            else
            {
                // NO: Return a zero value (failure)
                return 0;
            }
        }

        //*********************************************************************
        //  Method: GetUnderlying
        //
        /// <summary>
        /// Get the underlying price if the last asset, if it was an option.
        /// </summary>
        /// 
        /// <param name="nCommand">
        /// The GET_UNDERLYING command (67)
        /// </param>
        /// 
        /// <param name="parameter">
        /// 0 for this method
        /// </param>
        /// 
        /// <returns>
        /// The value of the underlying price, or 0 if the last asset was not
        /// an option.
        /// </returns>
        //*********************************************************************
        public static double
            GetUnderlying
            (
            int nCommand,
            int parameter
            )
        {
            // If the underlying price is not for an asset, return not found
            // else return the underlying price
            if (UnderlyingPrice < 9000.00) return 0.0;
            return UnderlyingPrice;
        }

        //*********************************************************************
        //  Method: SetDiagnostics
        //
        /// <summary>
        /// Set the logging of messages between Zorro and this plug-in.
        /// </summary>
        /// 
        /// <param name="nCommand">
        /// The SET_DIAGNOSTICS command (138)
        /// </param>
        /// 
        /// <param name="parameter">
        /// 0 = to disable; 1 = to enable
        /// </param>
        //*********************************************************************
        public static int
            SetDiagnostics
            (
            int nCommand,
            int parameter
            )
        {
            // Parameter must be zero or one
            if (parameter == 1 || parameter == 0)
            {
                // Set diagnostics variable for other use
                diagnostics = parameter == 1;

                // Return successful handling code
                return 1;
            }

            // Return failure handling code
            return 0;
        }

        //*********************************************************************
        //  Method: SetComboLegs
        //
        /// <summary>
        /// Set the next N OPTION trades as a combo order.
        /// </summary>
        /// 
        /// <param name="nCommand">
        /// The SET_DIAGNOSTICS command (138)
        /// </param>
        /// 
        /// <param name="numLegs">
        /// The number of legs to the order
        /// </param>
        //*********************************************************************
        public static int
            SetComboLegs
            (
            int nCommand,
            int numLegs
            )
        {
            // Max of 4 order legs
            if (numLegs < 0 || numLegs > 4) return 0;

            // Save the number of order legs
            ComboLegs = numLegs;

            // Rutern success code
            return 1;
        }

        //*********************************************************************
        //  Method: SetSymbol
        //
        /// <summary>
        /// Set the symbol used by subsequent commands
        /// </summary>
        /// 
        /// <param name="nCommand">
        /// The GET_POSITION command (132)
        /// </param>
        /// 
        /// <param name="ptr">
        /// An unmanaged pointer to the symbol (string) which is being set.
        /// </param>
        /// 
        /// <returns>
        /// Returns zero so Zorro can also implement this command.
        /// </returns>
        //*********************************************************************
        public static int
            SetSymbol
            (
            int nCommand,
            string symbol
            )
        {
            // Set the symbol in the plug-in
            CurrentSymbol = symbol;

            // Return 0, so Zorro can also set the symbol
            return 0;
        }

        //*********************************************************************
        //  Method: ShowResourceString
        //
        /// <summary>
        /// Show a resource string in the Zorro window
        /// </summary>
        /// 
        /// <param name="nCommand">
        /// The GET_POSITION command (4000)
        /// </param>
        /// 
        /// <param name="text">
        /// The key text used to retrieve a resource string.
        /// </param>
        /// 
        /// <returns>
        /// Non-zero for command execution.
        /// </returns>
        //*********************************************************************
        public static int
            ShowResourceString
            (
            int nCommand,
            string text
            )
        {
            // Show resource string in the Zorro windo
            TDAmerAPI.BrokerError($"{Resx.GetString(text)}");

            // Return a non-zero number
            return 1;
        }

        //*********************************************************************
        //  Method: GetPosition
        //
        /// <summary>
        /// Get the net open contracts for a given symbol.
        /// </summary>
        /// 
        /// <param name="nCommand">
        /// The GET_POSITION command (53)
        /// </param>
        /// 
        /// <param name="symbol">
        /// The ticker symbol of the position being sought.
        /// </param>
        /// 
        /// <returns>
        /// The net open contracts for the given symbol, negative for short
        /// positions, or zero when no positions held or symbol cannot be
        /// found.
        /// </returns>
        //*********************************************************************
        public static int
            GetPosition
            (
            int nCommand,
            string symbol
            )
        {
            // Method members
            UserAccount userAccount;
            int netPosition;

            // Log this command
            LogHelper.Log($"{Resx.GetString("GET_POSITION")}...");

            // Get the information
            try
            {
                // Get a new TD Ameritrade REST API object, if needed
                if (rest == null) rest = new TDAmeritradeREST();

                // Get account information (positions)
                string result = rest.QueryApi(
                    ApiMethod.GetAccount,
                    ApiHelper.AccountDataWithQueryString(
                        Broker.settings.TdaAccountNum,
                        "fields=positions",
                        true
                        )
                    );

                // Did the TD Ameritrade REST server return an error?
                if (result.StartsWith("ERROR:"))
                {
                    // YES: Log the error
                    LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("RETRIEVING")} {Resx.GetString("INFO")} {Resx.GetString("TDAMTRADE")} {Resx.GetString("ACCOUNT_NUM")}  {Broker.settings.TdaAccountNum}");
                }
                else
                {
                    // NO: Deserialize the result into a user account object
                    userAccount = DeserializeJson<UserAccount>(result);

                    // Was the deserialization successful?
                    if (userAccount != null)
                    {
                        // YES: Iterate through the positions
                        foreach (Position position in userAccount.Account.Positions)
                        {
                            // Is this position for the given symbol?
                            if (position.Instrument.Symbol == symbol)
                            {
                                // YES: Return the net positions on this asset
                                // Settled long quantity - Settled short quantity
                                // Log the find
                                netPosition = (int)(position.SettledLongQuantity - position.SettledShortQuantity);

                                // Log the net position
                                LogHelper.Log($"    {Resx.GetString("NET_POSITION_FOR")} {symbol} = {netPosition}");

                                // Return the net position
                                return netPosition;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("BROKER_ACCOUNT")}. " + e.Message);
            }

            // Return 0 for not found
            return 0;
        }

        //*********************************************************************
        //  Method: GetOptions
        //
        /// <summary>
        /// Get the option chain for an underlying asset (current symbol).
        /// </summary>
        /// 
        /// <param name="nCommand">
        /// The GET_OPTIONS command (64)
        /// </param>
        /// 
        /// <param name="ptrContracts">
        /// Pointer to an array of CONTRACT structures, maximum of 10,000.
        /// </param>
        /// 
        /// <returns>
        /// The number of contracts, or zero if no contracts obtained.
        /// </returns>
        //*********************************************************************
        public static int
            GetOptions
            (
            int nCommand,
            IntPtr ptrContracts
            )
        {
            // Method members
            List<CONTRACT> callContracts = new List<CONTRACT>();
            List<CONTRACT> putContracts = new List<CONTRACT>();
            Dictionary<string, object> dateDict = new Dictionary<string, object>();
            OptionChain oc;
            int putN;
            int callN;
            int nContracts = 0;
            int i;

            // Log this command
            LogHelper.Log($"{Resx.GetString("GET_OPTIONS")}...");

            // Gate-keeper, in case SET_SYMBOL has never been issued
            if (string.IsNullOrEmpty(CurrentSymbol)) return 0;

            try
            {
                // Get a new TD Ameritrade REST API object, if needed
                if (rest == null) rest = new TDAmeritradeREST();

                // Get the option chain for the underlying asset
                string result = rest.QueryApi(
                    ApiMethod.GetOptionChain,
                    ApiHelper.AccountDataWithQueryString(
                        Broker.settings.TdaAccountNum,
                        $"symbol={CurrentSymbol}",
                        true
                        )
                    );

                // Was result an error?
                if (result.StartsWith("ERROR:"))
                {
                    // YES: Log the error
                    LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("RETRIEVING_OPTION_CHAIN")}.");
                }
                else
                {
                    // NO: Create a non-data contract JSON serializer
                    JavaScriptSerializer serializer = new JavaScriptSerializer();

                    // Get the basic option chain information
                    oc = serializer.Deserialize<OptionChain>(result);

                    // Get the dictionary of dates and strike prices
                    dateDict = (Dictionary<string, object>)serializer.Deserialize<object>(result);

                    // Get the information for all the CALL contracts
                    callContracts.AddRange(ConvertOptionChainJson(oc, ContractType.CALL, (Dictionary<string, object>)dateDict["callExpDateMap"]));

                    // Get the information for all the PUT contracts
                    putContracts.AddRange(ConvertOptionChainJson(oc, ContractType.PUT, (Dictionary<string, object>)dateDict["putExpDateMap"]));

                    // Zorro should provide space for 10000 CONTRACT structures
                    // Are there more than 10000 contracts?
                    if (oc.NumberOfContracts > 10000)
                    {
                        // YES: Are there a lesser number of PUT contracts?
                        if (putContracts.Count < callContracts.Count)
                        {
                            // YES: Get the number of PUT contracts to include
                            putN = Math.Min(putContracts.Count, 5000);

                            // CALL contracts to include
                            callN = 10000 - putN;
                        }
                        else
                        {
                            // NO: Get the number CALL contracts to include
                            callN = Math.Min(callContracts.Count, 5000);

                            // PUT contracts to include
                            putN = 10000 - callN;
                        }
                    }
                    else
                    {
                        // NO: Include all of the contracts
                        putN = putContracts.Count;
                        callN = callContracts.Count;
                    }

                    //*****************************************************************
                    //
                    // NOTE: Do not return a double array of CONTRACT structures. Use
                    // an unmanaged (unsafe) pointer to get the original C++ CONTRACT
                    // array pointer, and load a CONTRACT array of structure at that
                    // address.
                    //
                    //*****************************************************************
                    unsafe
                    {
                        // Zorro passes a pointer to a an array of CONTRACT structs. We
                        // have to make that an IntPtr in C#, but create a new pointer
                        // and cast it as a CONTRACT pointer, so it will advance by the
                        // size of one CONTRACT structure through adding '1'.
                        CONTRACT* ptr = (CONTRACT*)ptrContracts;

                        // If Zorro's output pointer is NOT NULL store the CONTRACT
                        // data to it.
                        if ((int)ptr != 0)
                        {
                            // Iterate through the array of PUT contracts and layout a
                            // sequential CONTRACT structure array beginning at the 
                            // address pointed to by the IntPtr this method is called with.
                            for (i = 0; i < putN; ++i)
                            {
                                // Advance pointer by size of a CONTRACT structure
                                *(ptr + i) =

                                // Store a new PUT CONTRACT structure at this location
                                new CONTRACT(
                                    putContracts[i].time,
                                    putContracts[i].fAsk,
                                    putContracts[i].fBid,
                                    putContracts[i].fVal,
                                    putContracts[i].fVol,
                                    putContracts[i].fUnl,
                                    putContracts[i].fStrike,
                                    putContracts[i].Expiry,
                                    putContracts[i].Type
                                    );
                            }

                            // Now interate through the CALL contracts, advance
                            // pointer beyond where it left after PUT contracts
                            for (int n = 0; n < callN; ++n)
                            {
                                // Advance pointer by size of a CONTRACT structure
                                // and where it left after adding PUT contracts
                                *(ptr + i + n) =

                                // Store a new CALL CONTRACT structure at this location
                                new CONTRACT(
                                    callContracts[n].time,
                                    callContracts[n].fAsk,
                                    callContracts[n].fBid,
                                    callContracts[n].fVal,
                                    callContracts[n].fVol,
                                    callContracts[n].fUnl,
                                    callContracts[n].fStrike,
                                    callContracts[n].Expiry,
                                    callContracts[n].Type
                                    );
                            }

                            // Get the total number of contracts returned
                            nContracts = putN + callN;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                nContracts = 0;
            }

            // Return the total number of contracts in the CONTRACT array or 0
            // if an error was encountered
            return nContracts;
        }

        //*********************************************************************
        //  Method: GetAssetList
        //
        /// <summary>
        /// Get the current asset list.
        /// </summary>
        /// 
        /// <param name="nCommand">
        /// The GET_ASSE_TLIST command (4004)
        /// </param>
        /// 
        /// <param name="ptrChar">
        /// Pointer to an array of characters formatted as a 100 x 8 array of
        /// '\0' terminated strings.
        /// </param>
        /// 
        /// <returns>
        /// The number of assets
        /// </returns>
        //*********************************************************************
        public static int
            GetAssetList
            (
            int nCommand,
            IntPtr ptrChar
            )
        {
            // Method members
            int nAssets = 0;
            int i, j;

            // Log this command
            LogHelper.Log($"{Resx.GetString("GET_ASSET_LIST")}");
            try
            {
                unsafe
                {
                    byte* ptr = (byte*)ptrChar;

                    // Iterate through the subscription list
                    for (i = 0; i < subscriptionList.Count; ++i)
                    {
                        TDAsset tdAsset = subscriptionList[i];

                        // Iterate through the length of the ticker symbol
                        for (j = 0; j < tdAsset.TickerSymbol.Length; ++j)
                        {
                            *(ptr + (i * 8) + j) = (byte)tdAsset.TickerSymbol.ToCharArray()[j];
                        }
                    }
                }

                // Return the number of assets
                return subscriptionList.Count;

            }
            catch (Exception e)
            {
                nAssets = 0;
            }

            // Return the total number of contracts in the CONTRACT array or 0
            // if an error was encountered
            return nAssets;
        }

        //*********************************************************************
        //  Method: GetTestAssets
        //
        /// <summary>
        /// Get the test assets in the SETTINGS file.
        /// </summary>
        /// 
        /// <param name="nCommand">
        /// The GET_TEST_ASSETS command (4006)
        /// </param>
        /// 
        /// <param name="ptrChar">
        /// Pointer to an array of characters formatted as a N x 8 array of
        /// '\0' terminated strings.
        /// </param>
        /// 
        /// <returns>
        /// The number of assets
        /// </returns>
        //*********************************************************************
        public static int
            GetTestAssets
            (
            int nCommand,
            IntPtr ptrChar
            )
        {
            // Method members
            int nAssets = 0;
            int i, j;

            // Log this command
            LogHelper.Log($"{Resx.GetString("GET_TEST_ASSETS")}...");
            try
            {
                // Get the test assets and separate them by the comma
                string[] assets = settings.testAssets.Split(',');

                unsafe
                {
                    byte* ptr = (byte*)ptrChar;

                    // Iterate through the test assets
                    for (i = 0; i < assets.Length; ++i)
                    {
                        // Strip any spaces from it
                        string asset = assets[i].Trim();

                        // Iterate through the length of the ticker symbol
                        for (j = 0; j < asset.Length; ++j)
                        {
                            *(ptr + (i * 8) + j) = (byte)asset.ToCharArray()[j];
                        }
                    }
                }

                // Return the number of assets
                return assets.Length;
            }
            catch (Exception e)
            {
                nAssets = 0;
            }

            // Return the total number of contracts in the CONTRACT array or 0
            // if an error was encountered
            return nAssets;
        }

        //*********************************************************************
        //  Method: ValidateLang
        //
        /// <summary>
        /// Validate a language specification used for globalization.
        /// </summary>
        /// 
        /// <param name="lang">
        /// The language specification.
        /// </param>
        /// 
        /// <returns>
        /// True if valid language, false if not.
        /// </returns>
        //*********************************************************************
        public static bool
            ValidateLang
            (
            string lang
            )
        {
            // Validate the language
            int ret = Resx.ValidateLang(lang);

            switch (ret)
            {
                case 1:
                    // Language valid, set the language in the settings class
                    return true;

                case -1:
                    // Language spec invalid
                    LogHelper.Log(LogLevel.Error, $"    {Resx.GetStringFromLang("LANG_SPEC_INVALID", "en-US")}");
                    return false;

                case -2:
                    // Language not on-file
                    LogHelper.Log(LogLevel.Error, $"    {Resx.GetStringFromLang("LANG_NOT_FOUND", "en-US")}");
                    return false;

                case 0:
                default:
                    return false;
            }
        }

        //*********************************************************************
        //  Method: ReviewLicense
        //
        /// <summary>
        /// Review the plug-in license
        /// </summary>
        /// 
        /// <param name="nCommand">
        /// The REVIEW_LICENSE command (4002)
        /// </param>
        /// 
        /// <param name="parameter">
        /// Always zero.
        /// </param>
        /// 
        /// <returns>
        /// The nnumber .gt. zero.
        /// </returns>
        //*********************************************************************
        public static int
            ReviewLicense
            (
            int nCommand,
            int parameter
            )
        {
            // Do not accept license
            Helper.SetRegistryValue(LICENSE_ACCEPTANCE, "0");

            // Review the license
            LicenseAccepted();

            // Return a value .gt. zero
            return 1;
        }

        //*********************************************************************
        //  Method: SetSellSellShort
        //
        /// <summary>
        /// What to do when SELLING more lots than owned.
        /// </summary>
        /// 
        /// 
        /// <param name="nCommand">
        /// The SELL_SELL_SHORT command (4012)
        /// </param>
        /// 
        /// <param name="parameter">
        /// Integer value for the command
        /// </param>
        /// 
        /// <remarks>
        /// It's possible to isuue a SELL order to the TD Ameritrade REST API
        /// for an amount of shares greater than the amount owned. The normal
        /// response of TD Ameritrade is to then create two orders: one SELL
        /// order for the amount owned, and another SELL SHORT for the balance.
        /// This enumeration governs what to do in that case, with the follow-
        /// ing possibilities:
        /// 
        /// 0 = Cancel the SELL ordor;
        /// 1 = Fill the SELL order for amount of shares owned (default);
        /// 2 = SELL shares owned, SELL SHORT shares not owned (without doing
        ///     anything this is TD Ameritrade default)
        /// </remarks>
        //*********************************************************************
        public static int
            SetSellSellShort
            (
            int nCommand,
            int parameter
            )
        {
            // Limit out-of-bounds level, so it is set to default
            if (parameter > 2 || parameter < 0) parameter = 2;

            // Set the sellSellShort member
            sellSellShort = (SellSellShort)parameter;

            // Return non-negative value
            return 1;
        }
        #endregion PUBLIC METHODS TO IMPLEMENT THE ZORRO BROKER COMMANDS

        #region PRIVATE METHODS
        //*********************************************************************
        //  Method: CloseTrade
        //
        /// <summary>
        /// Close an order that has been placed.
        /// </summary>
        /// 
        /// <param name="ZorroTradeId">
        /// The Zorro trade id number.
        /// </param>
        /// 
        /// <param name="leg">
        /// The order leg with information about the trade.
        /// </param>
        /// 
        /// <param name="Amount">
        /// The amount of the trade to close. TD Ameritrade does not support
        /// partial closing of trades for Stocks, ETFs, and options, to this
        /// paramater is effectively always equal to the amount of the
        /// original trade.
        /// </param>
        /// 
        /// <param name="Limit">
        /// The limit price to close the trade at.
        /// </param>
        /// 
        /// <returns>
        /// A trade object with information about the trade.
        /// </returns>
        /// 
        /// <remarks>
        /// NOTE: There are several subtleties of CLOSING a trade on the TD
        /// Ameritrade platform:
        /// 
        /// (1) The trade needs to be FILLED. If it is still only QUEUED (as
        ///     some trades accepted after hours) then CANCEL the trade instead
        ///     of attempting to close it.
        ///     
        /// (2) Trades to close OPITONS are entered as "BUY_TO_CLOSE" or "SELL_
        ///     TO_CLOSE"
        ///     
        /// (3) If trades are open and closed too frequently, there's a danger
        ///     of triggering TD Ameritrade's "pattern trading" violation and
        ///     cause the account to be flagged for added charges and a 90-day
        ///     suspension of trading.
        /// </remarks>
        //*********************************************************************
        private static Trade
            CloseTrade
            (
            int ZorroTradeId,
            OrderLeg leg,
            int Amount,
            double Limit
            )
        {
            // Method member
            Trade trade = null;

            // Look-up the trade on TD Ameritrade
            trade = Trade.GetTradeByZorroId(ZorroTradeId);

            // Valid trade?
            if (trade != null)
            {
                // YES. What is the trade status?
                switch (trade.Status)
                {
                    // Statuses for which order should be CLOSED
                    case "FILLED":
                    case "WORKING":
                    case "ACCEPTED":
                        // Close this order leg
                        trade = CloseOrderLeg(leg, Amount, Limit);

                        // Set the TD trade Id for zero or one based on whether
                        // the trade has a Zorro trade Id (i.e. was a success)
                        trade.TDTradeId = trade.ZorroTradeId > 0 ? 1L : 0L;

                        // Out of switch
                        break;

                    // Statuses for which order has not been FILLED and it can
                    // be CANCELED
                    case "PENDING_ACTIVATION":
                    case "PENDING_CANCEL":
                    case "PENDING_REPLACE":
                    case "AWAITING_PARENT_ORDER":
                    case "AWAITING_CONDITION":
                    case "AWAITING_UR_OUT":
                    case "AWAITING_MANUAL_REVIEW":
                    case "QUEUED":
                        if (CancelOrder(trade.TDTradeId))
                        {
                            // YES: Log message that trade closed by CANCELED
                            LogHelper.Log($"{Resx.GetString("THIS_TRADE_WAS")} (TDA #{trade.TDTradeId}) {Resx.GetString(trade.Status)}. {Resx.GetString("IT_WAS_CANCELED")}.");

                            // Return success code
                            trade.TDTradeId = 1L;
                        }
                        else
                        {
                            // NO: Log message that trade could not be CANCELED
                            LogHelper.Log(LogLevel.Error, $"TDA #{trade.TDTradeId}: {Resx.GetString("THIS_TRADE_WAS")} {Resx.GetString(trade.Status)}. {Resx.GetString("BUT_NOT_CANCELED")} .");

                            // Return failure code
                            trade.TDTradeId = 0L;
                        }

                        // Out of switch
                        break;

                    // Status for which an informational message goes to user
                    // and nothing is done
                    case "REPLACED":
                    case "REJECTED":
                    case "CANCELED":
                    case "EXPIRED":
                    default:

                        // Send the message about the trade status and that it
                        // was not closed
                        LogHelper.Log(LogLevel.Error, $"{Resx.GetString("ORDER_LEG")}  #{leg.LegId + 1} (TDA #{trade.TDTradeId}): {Resx.GetString("TRADE_NOT_CLOSED_BECAUSE")}  {Resx.GetString(trade.Status)}.");

                        // Return code as though trade was closed, so we're
                        // not faced with closing it again
                        trade.TDTradeId = 1L;

                        // Out of switch
                        break;
                }
            }
            else
            {
                // Trade not found, create a dummy trade and set the TD trade
                // id to zero
                trade = new Trade();
                trade.TDTradeId = 0L;
            }

            // Return the trade
            return trade;
        }

        //*********************************************************************
        //  Method: CloseOrderLeg
        //
        /// <summary>
        /// Close the leg of a multi-leg order.
        /// </summary>
        /// 
        /// <param name="leg">
        /// The order leg to close.
        /// </param>
        /// 
        /// <param name="Amount">
        /// Amount to close (+ = close LONG position; - = close SHORT position)
        /// </param>
        /// 
        /// <param name="Limit">
        /// Limit price to execute close order at.
        /// </param>
        /// 
        /// <returns>
        /// A Trade class object representing the order to close the leg.
        /// </returns>
        //*********************************************************************
        private static Trade
            CloseOrderLeg
            (
            OrderLeg leg,
            int Amount,
            double Limit
            )
        {
            // Method member
            Trade trade;

            // Option leg?
            if (leg.Instrument.AssetType == "OPTION")
            {
                // YES: Execute a "TO_CLOSE" BUY or SELL order for this leg
                trade = Buy(leg.Instrument.Symbol + "-TO_CLOSE", -Amount, 0, Limit);
            }
            else
            {
                // NO: Execute a regular BUF or SELL order for this leg
                trade = Buy(leg.Instrument.Symbol, -Amount, 0, Limit);
            }

            // Does trade have a Zorro Id (i.e. successful order)
            if (trade.ZorroTradeId > 0)
            {
                // YES: Set the status code of the trade
                trade.StatusCode = 1;
            }
            else
            {
                // NO: Trade to close was not successful, set status code
                trade.StatusCode = 0;
            }

            return trade;
        }


        //*********************************************************************
        //  Method: CancelOrder
        //
        /// <summary>
        /// Cancel an order, given the TD Ameritrade order Id.
        /// </summary>
        /// 
        /// <param name="TDATradeID">
        /// The order number to cancel
        /// </param>
        /// 
        /// <returns>
        /// TRUE if the order was successfully canceled, FALSE otherwise.
        /// </returns>
        //*********************************************************************
        private static bool
            CancelOrder
            (
            long TDATradeID
            )
        {
            try
            {
                // Get a rest API objcet, if needed
                if (rest == null) rest = new TDAmeritradeREST();

                // Issue the ORDER CANCEL to the TD Ameritrade API
                string result = rest.QueryApi(
                    // The API method
                    ApiMethod.CancelOrder,

                    // THe API parameters
                    ApiHelper.AccountOrderData
                    (
                        // TD Ameritrade account number
                        settings.TdaAccountNum,

                        // TD Ameritrade order ID
                        TDATradeID.ToString(),

                        // YES, use an access token for authentication
                        true)
                    );

                // Did we get an error from the cancellation?
                if (!result.StartsWith("ERROR:"))
                {
                    // NO: Return a success code
                    return true;
                }
            }
            catch (Exception e)
            {
                // An error occurred, log it
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("CANCELING")} {Resx.GetString("TRADE")} #{TDATradeID.ToString()}");
            }

            // Return a failure code
            return false;
        }

        //*********************************************************************
        //  Method: SubmitOrder
        //
        /// <summary>
        /// Submit a market order to the TD Ameritrade API for FILLING.
        /// </summary>
        /// 
        /// <param name="order">
        /// The order to submit
        /// </param>
        /// 
        /// <returns>
        /// The Zorro trade number or a negative integer representing the
        /// status of a failed order.
        /// </returns>
        //*********************************************************************
        private static long
            SubmitOrder
            (
            OrderSubmission order
            )
        {
            // Method Members
            int nSharesOwned;
            bool validFund = false;
            long orderNum;
            string result = jsonNull;
            string json;

            try
            {
                // If this is an order for a mutual fund, validate that the order
                // is likely to be accepted
                if (order.asset.AssetType == "FUND")
                {
                    // Validate the fund
                    validFund = ValidateFund(order);

                    // If fund is not valid return 0 for no trade
                    if (!validFund)
                    {
                        // Log error message
                        LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("FUND_MINIMUM_NOT_MET")}.");
                        return 0L;
                    }
                }

                // IS this a SELL order?
                if (order.Amount < 0)
                {
                    // YES: Get how many shares the user already owns
                    nSharesOwned = GetPosition((int)ZorroCommand.GET_POSITION, order.asset.TickerSymbol);

                    // Number of shares to SELL greater than the number OWNED?
                    if (Math.Abs(order.Amount) > nSharesOwned)
                    {
                        // YES: What to do?
                        switch (sellSellShort)
                        {
                            // CANCEL the order
                            case SellSellShort.Cancel:
                                return 0;

                            // Adjust the order amount to the shares OWNED
                            case SellSellShort.Adjust:
                                order.Amount = -nSharesOwned;
                                break;

                            // Do nothing and let TD Ameritrade issue 2 orders:
                            // (a) on order to SELL the shares OWNED; ond
                            // (b) an order SELL SHORT balance of shares.
                            case SellSellShort.Short:
                            default:
                                break;
                        }
                    }
                }

                //*****************************************************************
                //
                // NOTE: TD Ameritrade does not accept OPTION trades outside of
                // market hours.
                //
                //*****************************************************************
                // Is the market open to trade? Note, the EQUITY market is the NYSE
                // as far as TD Ameritrade is concerned
                if (IsMarketOpen("EQUITY") ||

                    // OR, is the plug-in in test mode
                    TDAmerAPI.TestMode)
                {
                    // YES: Get a TD Ameritrade REST API, if needed
                    if (rest == null) rest = new TDAmeritradeREST();

                    // Are we combining trades?
                    if (ComboLegs > 0)
                    {
                        // YES: Save the order
                        order.SaveInComboList();

                        // Decrement the combo leg counter
                        --ComboLegs;

                        // Have we saved the last order?
                        if (ComboLegs == 0)
                        {
                            // YES: Get the combo json
                            json = order.GetCombinedOrderJson();

                            // Did an error occur?
                            if (json.StartsWith("ERROR:"))
                            {
                                // YES: Log the error
                                LogHelper.Log($"{json}.");

                                // Return failure code
                                return 0L;
                            }
                            else
                            {
                                // NO: Submit the order. Call the TD Ameritrade REST
                                // API to place the order and get the return
                                result = rest.QueryApi(

                                    // API method to place an order with TD Ameritrade
                                    ApiMethod.PlaceOrder,

                                    // JSON raw data for the order and use the access token
                                    ApiHelper.JsonRawData(json, settings.TdaAccountNum, true)
                                    );
                            }
                        }
                        else
                        {
                            // NO: Tis is not the last order for this combo order,
                            // return 0 for the order
                            return 0L;
                        }
                    }
                    else
                    {
                        // NO: Normal order submission process
                        // Get the Json payload
                        json = order.ConvertToJson();

                        // All the REST API method and get the result
                        result = rest.QueryApi(
                            // API method to place an order with TD Ameritrade
                            ApiMethod.PlaceOrder,

                            // JSON raw data for the order and use the access token
                            ApiHelper.JsonRawData(json, settings.TdaAccountNum, true)
                            );
                    }

                    // Log the Json
                    LogHelper.Log("\r\n" + Helper.FormatJson(json) + "\r\n");

                    // Was the result returned in error
                    if (!result.StartsWith("ERROR:"))
                    {
                        // NO: Good result. Can order number be obtained?
                        if (Int64.TryParse(result, out orderNum))
                        {
                            // YES: Save the json payload, linked to order number
                            // Initialise the dictionary if needed
                            if (jsonByOrderNum == null) jsonByOrderNum = new Dictionary<long, string>();

                            // If json for this order not already saved, save it.
                            // If key is already present rewrite json
                            if (!jsonByOrderNum.ContainsKey(orderNum))
                                jsonByOrderNum.Add(orderNum, json);
                            else
                                jsonByOrderNum[orderNum] = json;

                            // Return tho order number.
                            return orderNum;
                        }
                        else
                        {
                            // NO: Order entered. No ID returned yet. Log anomaly.
                            LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("NO_ID_RETURNED")} {order.Symbol}: {result}");

                            // Return code relative to above condition
                            return -3;
                        }
                    }
                    else
                    {
                        LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("PLACING_ORDER_FOR")} {order.Symbol}: {result}.");
                    }
                }
                else
                {
                    // Log market is not open
                    LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("MARKET_NOT_OPEN")}.");
                }

                // Asset not found, or market not open. In either case, trade not
                // entered
                return 0;
            }
            catch(Exception e)
            {
                TDAmerAPI.BrokerError($"ERROR in Submit Order: {e.Message}.");
                return 0;
            }
        }

        //*********************************************************************
        //  Method: ValidateFund
        //
        /// <summary>
        /// Validate the status of a mutual fund order.
        /// </summary>
        /// 
        /// <param name="order">
        /// The order being placed.
        /// </param>
        /// 
        /// <returns>
        /// True if fund is valid, and trade likely to succed; otherwise false.
        /// </returns>
        /// 
        /// <remarks>
        /// Mutual fund validation proceeds in several ways. First, the under-
        /// lying concern is that the mutual fund has an investment minimum,
        /// which if not met will prevent the order from being accepted. There
        /// are two ways to validate that this minimum has been met: (1) The
        /// user may already have this fund in his/her portfolio, in which 
        /// case they have presumably met the minimum; OR, (2) the amount of
        /// the purchase price meets the minimum required for this fund, in
        /// which case the trade is likely to succeed. It is possible that the
        /// fund minimum cannot be determined, in which case the trade will be
        /// attempted anyway.
        /// </remarks>
        //*********************************************************************
        private static bool
            ValidateFund
            (
            OrderSubmission order
            )
        {
            // Is the fund already in the user's portfolio
            if (IsAssetInPortfolio(order.Symbol)) return true;

            // NO: Fund is not in user's portfolio, get the fund minimum
            MutualFundInfo mfi = MutualFundInfo.GetFundInfo(order.Symbol);

            // Was valid mutual fund information returned?
            if (mfi != null)
            {
                // YES: Will order meet or exceed minimum required?
                return Math.Abs(order.Price) >= mfi.MinimumInvestment;
            }
            else
            {
                // NO: Mutual fund information not found and asset not already
                // in user's portfolio. Let the order be submitted even though
                // it might fail.
                return true;
            }

            // From here everything else points to a failed trade
            return false;
        }

        //*********************************************************************
        //  Method: Subscription
        //
        /// <summary>
        /// Subscribes Zorro to an asset if required
        /// </summary>
        /// 
        /// <param name="asset">
        /// The asset under consideration
        /// </param>
        /// 
        /// <returns>
        /// True if the asset needs to be subscribed to, false if not.
        /// </returns>
        //*********************************************************************
        public static bool
            Subscription
            (
            TDAsset asset
            )
        {
            // Initialize the subscription list, if necessary
            if (subscriptionList == null)
            {
                subscriptionList = new List<TDAsset>();
            }

            // Is this asset already in the subscription list? Use the full
            // symbol, in case, there are multiple assets of different types
            // related to the same ticker symbol (i.e. an stock and an option
            // for AAPL).
            if (subscriptionList.Where(a => a.Symbol == asset.Symbol).Count() > 0)
            {
                // YES: Asset already in the subscription list
                LogHelper.Log($"{asset.Symbol} {Resx.GetString("ALREADY_SUBSCRIBED")}.");

                // YES: Subscription not needed
                return false;
            }
            else
            {
                // NO: Asset is not in subscription list. Valid asset?
                if (asset.Valid)
                {
                    // YES: Add asset to subscription list
                    subscriptionList.Add(asset);

                    LogHelper.Log($"{asset.Symbol}. Valid asset added to subscription list.");

                    // Subscription needed
                    return true;
                }
                else
                {
                    // NO: Not a valid asset
                    LogHelper.Log(LogLevel.Error, $"{asset.Symbol}. Invalid asset not added to subscription list.");

                    // Subscription needed
                    return false;
                }
            }
        }

        //*********************************************************************
        //  Method: ProcessOption
        //
        /// <summary>
        /// Get information needed by Zorro from quote information returned by
        /// TD Ameritrade for an Equity or ETF asset.
        /// </summary>
        /// 
        /// <param name="asset">
        /// The asset
        /// </param>
        /// 
        /// <returns>
        /// A TDAsset class object with information filled in, or set invalid
        /// if asset cannot be found and therefore the asset cannot be traded.
        /// </returns>
        //*********************************************************************
        private static TDAsset
            ProcessOption
            (
            TDAsset tdAsset,
            Option asset
            )
        {
            // Get the ask price
            double askPrice = GetAskPrice(asset);

            // If no ask price can be determined, do not trade this security
            if (askPrice < -9000.0)
            {
                // Make the asset not available for trading
                tdAsset.Valid = false;

                // Underlying price is invalid
                UnderlyingPrice = -9999.99;

                // Return the asset
                return tdAsset;
            };

            // Get the bid price
            double bidPrice = GetBidPrice(asset);

            // Fill-in the TD Asset object
            tdAsset.Price = askPrice;
            tdAsset.Spread = askPrice - bidPrice;
            tdAsset.Volume = asset.TotalVolume;
            tdAsset.LotAmount = 1.0;
            tdAsset.MarginCost = 0;
            tdAsset.Pip = 0.01;
            tdAsset.PipCost = 0.01;

            // Set the underlying price
            UnderlyingPrice = askPrice;

            // Return the TDAsset
            return tdAsset;
        }

        //*********************************************************************
        //  Method: ProcessEquityETF
        //
        /// <summary>
        /// Get information needed by Zorro from quote information returned by
        /// TD Ameritrade for an Equity or ETF asset.
        /// </summary>
        /// 
        /// <param name="asset">
        /// The asset
        /// </param>
        /// 
        /// <returns>
        /// A TDAsset class object with information filled in, or set invalid
        /// if asset cannot be found and therefore the asset cannot be traded.
        /// </returns>
        //*********************************************************************
        private static TDAsset
            ProcessEquityETF
            (
            TDAsset tdAsset,
            EquityETF asset
            )
        {
            // Get the ask price
            double askPrice = GetAskPrice(asset);

            // If no ask price can be determined, do not trade this security
            if (askPrice < -9000.0)
            {
                // Make the asset not available for trading
                tdAsset.Valid = false;

                // Return the asset
                return tdAsset;
            };

            // Get the bid price
            double bidPrice = GetBidPrice(asset);

            // Fill-in the TD Asset object
            tdAsset.Price = askPrice;
            tdAsset.Spread = askPrice - bidPrice;
            tdAsset.Volume = asset.TotalVolume;
            tdAsset.LotAmount = 1.0;
            tdAsset.MarginCost = 0;
            tdAsset.Pip = 0.01;
            tdAsset.PipCost = 0.01;

            // Return the TDAsset
            return tdAsset;
        }

        //*********************************************************************
        //
        // NOTE: The TD Ameritrade REST API currently does not support FOREX
        // trading. Uncomment the following method if that changes in the 
        // future.
        //
        //*********************************************************************
        //  Method: ProcessForex
        //
        /// <summary>
        /// Get information needed by Zorro from quote information returned by
        /// TD Ameritrade for a FOREX currency pair.
        /// </summary>
        /// 
        /// <param name="asset">
        /// The asset
        /// </param>
        /// 
        /// <returns>
        /// A TDAsset class object with information filled in, or null if the
        /// asset cannot be found and therefore the asset cannot be traded.
        /// </returns>
        //*********************************************************************
        /*
        private static TDAsset
            ProcessForex
            (
            Forex asset
            )
        {
            // Get the ask price
            double askPrice = GetAskPrice(asset);

            // If no ask price can be determined, do not trade this security
            if (askPrice < -9000.0) return null;

            // Get the bid price
            double bidPrice = GetBidPrice(asset);

            // Get the rollover rates
            double ROR = CurrencyInterestRates.ComputeRollover(asset.Symbol, askPrice);

            // Round the rollover to five places
            ROR = Math.Round(ROR, 5);

            // Return a TD Asset object
            return new TDAsset
            {
                Symbol = asset.Symbol,
                Price = asset.AskPrice,
                Spread = askPrice - bidPrice,
                Volume = 0,
                LotAmount = 10000,
                MarginCost = 0.00,
                Pip = (asset.Symbol.Contains("/JPY") || asset.Symbol.Contains("JPY/")) ? 0.01 : 0.0001,
                PipCost = ComputePipCost(asset.Symbol),
                RolloverLong = ROR,
                RolloverShort = ROR
            };
        }
        */

        //*********************************************************************
        //  Method: ProcessMutualFund
        //
        /// <summary>
        /// Get information needed by Zorro from quote information returned by
        /// TD Ameritrade for a Mutual Fund asset.
        /// </summary>
        /// 
        /// <param name="asset">
        /// The asset
        /// </param>
        /// 
        /// <returns>
        /// A TDAsset class object with information filled in, or null if the
        /// asset cannot be found and therefore the asset cannot be traded.
        /// </returns>
        //*********************************************************************
        private static TDAsset
            ProcessMutualFund
            (
            TDAsset tdAsset,
            MutualFund asset
            )
        {
            // Price of the fund is the NAV. Only if it's not available, get
            // the price from somewhere else
            double askPrice = asset.NAV > 0 ? asset.NAV : GetAskPrice(asset);

            // If no ask price can be determined, do not trade this security
            if (askPrice < -9000.0)
            {
                // Make the asset not available for trading
                tdAsset.Valid = false;

                // Return the asset
                return tdAsset;
            };

            // Get the bid price
            double bidPrice = GetBidPrice(asset);

            // Fill-in the TD Asset object
            tdAsset.Price = askPrice;
            tdAsset.Spread = askPrice - bidPrice;
            tdAsset.Volume = asset.TotalVolume;
            tdAsset.LotAmount = 1.0;
            tdAsset.MarginCost = 0;
            tdAsset.Pip = 0.01;
            tdAsset.PipCost = 0.01;

            // Make the asset available for trading
            tdAsset.Valid = true;

            // Return the TDAsset
            return tdAsset;
        }

        //*********************************************************************
        //  Method: ComputePipCost
        //
        /// <summary>
        /// Compute the cost of a movement of 1 pip in account currency terms. 
        /// </summary>
        /// 
        /// <param name="forexPair">
        /// The currency pair that the pip cost is being computed for
        /// </param>
        /// 
        /// <returns>
        /// A value (to 2 places) in the account currency representing the cost
        /// of a pip.
        /// </returns>
        /// 
        /// <remarks>
        /// TD Ameritrade uses micro lots for forex trading, thus the minimum
        /// lot size is 10000. Pips are 0.0001 far all pairs except those that
        /// involve the Japanese Yen, in which case the pip is 0.01.
        /// 
        /// If the account currency equals the counter currency, the pip cost 
        /// is said to be 'fixed,' which for TD Ameritrade means that the pip
        /// cost is XXX1, where XXX is the counter currency.
        /// 
        /// If the account currency is the primary currency, the pip cost is
        /// 1 / ({ACCOUNT CURRENCY/XXX) rate, where XXX is counter currency. If
        /// the {ACCT CURR/XXX} pair cannot be found but the {XXX/ACCT CURR}
        /// pair can be found then compute pip cost as 1 * the {XXX/ACCT CURR}
        /// rate.
        /// </remarks>
        //*********************************************************************
        public static double
            ComputePipCost
            (
                string forexPair
            )
        {
            // Get TD Ameritrade REST API if needed
            if (rest == null) rest = new TDAmeritradeREST();

            // Method member
            double pipCost = 0.01;

            // Get the primary and counter currencies of the FOREX pair
            string[] currencies = forexPair.Split('/');
            string counterCurrency = currencies[1];

            // Is the account currency the same as counter currency?
            if (settings.Currency == counterCurrency)
            {
                // We have a 'fixed' pip cost based on the TDA lot size of
                // 1 currency unit in the account currency
                return 1.0;
            }

            // Past here, we will need the account currency/counter currency
            // pair. Form the pair
            string acctCntrCurrency = $"{settings.Currency}/{counterCurrency}";

            // Get the quote for this pair
            double? rate = GetForexRateQuote(rest, acctCntrCurrency);

            // Rate quote obtained?
            if (rate == null)
            {
                // NO: Get rate quote for pair reversal
                rate = GetForexRateQuote(rest, $"{counterCurrency}/{settings.Currency}");

                // Rate quote for pair reversal obtained?
                if (rate != null)
                {
                    // YES: Just need to round the result, which is equivalent
                    // to multiplying by 1
                    pipCost = Math.Round((double)rate, 4);
                }
            }
            else
            {
                // YES: Compute 1/rate rounded to four places
                pipCost = Math.Round(Math.Pow((double)rate, -1), 4);
            }

            // If dealing with the Yen, multiply pip cost by 100
            if (forexPair.Contains("JPY")) pipCost *= 100.0;

            // Return the pip cost
            return pipCost;
        }

        //*********************************************************************
        //  Method: GetForexRateQuote
        //
        /// <summary>
        /// Get a rate quote for a FOREX pair.
        /// </summary>
        /// 
        /// <param name="rest">
        /// Reference to the TD Ameritrade API.
        /// </param>
        /// 
        /// <param name="forexPair">
        /// The forex pair that a rate quote is being looked up for
        /// </param>
        /// 
        /// <returns>
        /// The rate quote for this pair, or null if the pair cannot be found.
        /// </returns>
        //*********************************************************************
        private static double?
            GetForexRateQuote
            (
                TDAmeritradeREST rest,
                string forexPair
            )
        {
            // Method member
            Dictionary<string, object> assetDict = new Dictionary<string, object>();

            // Query the TD Ameritrade API for the FOREX pair
            string result = rest.QueryApi(

                // The TD Ameritrade API method
                ApiMethod.GetQuotes,

                // The data object passed to the API method
                ApiHelper.AccountDataWithQueryString(

                    // No data in URL before query string
                    null,

                    // The query string used to get the market hours
                    string.Format($"symbol={forexPair}"),

                    // Use authentication
                    true
                    )
                );

            // Was the FOREX pair found?
            if (result != jsonNull)
            {
                // YES: Convert to a JSON dictionary object <string, object>
                // NO: Create a non-data contract JSON serializer
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                // Deserialize the result returned from TD Ameritrade into
                // the 1st level dictionary, i.e. <string, object>
                assetDict = (Dictionary<string, object>)serializer.Deserialize<object>(result);

                // Convert the FOREX JSON object to a Forex class object
                Forex forex = JsonDictConvert<Forex>((Dictionary<string, object>)assetDict[forexPair]);

                // Return the last price
                return forex.LastPrice;
            }
            return null;
        }

        //*********************************************************************
        //  Method: GetAskPrice
        //
        /// <summary>
        /// Get the ask price for a security.
        /// </summary>
        /// 
        /// <param name="price">
        /// An object the implements the IPrice interface.
        /// </param>
        /// 
        /// <returns>
        /// A double value representing the ask price.
        /// </returns>
        /// 
        /// <remarks>
        /// Sometimes the ask price is returned from the TDA API as 0, i.e. in
        /// the case of the USD/CNH currency pair. In this case, there actually
        /// is a last price, which is the price the currency pair last traded
        /// at. Zero will cause a rollover rate of +/- infinity computed, so
        /// use the last price as the asking price.
        /// </remarks>
        //*********************************************************************
        public static double
            GetAskPrice
            (
            IPrice price
            )
        {
            // Is the ask price zero?
            if (price.AskPrice == 0.0)
            {
                // YES: is the last price zero?
                if (price.LastPrice == 0.0)
                {
                    // YES: Is the close price zero
                    if (price.ClosePrice == 0.0)
                    {
                        // YES: Everything is zero. This security cannot be
                        // traded.
                        return -99999.99;
                    }
                    else
                    {
                        // NO: Use the close price as the ask price
                        return price.ClosePrice;
                    }
                }
                else
                {
                    // NO: Use the last price as the ask price
                    return price.LastPrice;
                }
            }
            else
            {
                // NO: Use the ask price as the ask price
                return price.AskPrice;
            }
        }

        //*********************************************************************
        //  Method: GetBidPrice
        //
        /// <summary>
        /// Get the bid price for a security.
        /// </summary>
        /// 
        /// <param name="price">
        /// An object the implements the IPrice interface.
        /// </param>
        /// 
        /// <returns>
        /// A double value representing the bid price.
        /// </returns>
        /// 
        /// <remarks>
        /// Sometimes the bid price is returned from the TDA API as 0, i.e. in
        /// the case of the USD/CNH currency pair. Use the close price as the
        /// ask price, if available. If no close price, the use the last price.
        /// </remarks>
        //*********************************************************************
        public static double
            GetBidPrice
            (
            IPrice price
            )
        {
            // Is the bid price zero?
            if (price.BidPrice == 0.0)
            {
                // YES: is the close price zero?
                if (price.ClosePrice == 0.0)
                {
                    // YES: Is the close price zero
                    if (price.LastPrice == 0.0)
                    {
                        // YES: Everything is zero. This security cannot be
                        // traded.
                        return -99999.99;
                    }
                    else
                    {
                        // NO: Use the close price as the bid price
                        return price.LastPrice;
                    }
                }
                else
                {
                    // NO: Use the last price as the bid price
                    return price.ClosePrice;
                }
            }
            else
            {
                // NO: Use the bid price as the bid price
                return price.BidPrice;
            }
        }

        //*********************************************************************
        //  Method: GetQuote
        //
        /// <summary>
        /// Get a quote for a given symbol (Equity, ETF, or FOREX) from the
        /// TD Ameritrade API.
        /// </summary>
        /// 
        /// <param name="symbol">
        /// The name (symbol) of the asset for which information is being 
        /// retrieved from TD Ameritrade.
        /// </param>
        /// 
        /// <returns>
        /// A JSON string with quote data for the symbol, or null.
        /// </returns>
        //*********************************************************************
        public static string
            GetQuote
            (
            string symbol
            )
        {
            // Get a TD Ameritrade API object, if needed
            if (rest == null) rest = new TDAmeritradeREST();

            // Call TD Ameritratde API to get current quotes for this symbol
            string result = rest.QueryApi(

                // The TD Ameritrade API method
                ApiMethod.GetQuotes,

                // The data object passed to the API method
                ApiHelper.AccountDataWithQueryString(

                    // No data in URL before query string
                    null,

                    // The query string used to get the quote for this symbol
                    string.Format("symbol={0}", symbol),

                    // Use authentication
                    true
                    )
                );

            // Was an error encountered?
            if (string.IsNullOrEmpty(result) || result.StartsWith("ERROR:"))
            {
                // YES: Does the result string begin with an error?
                if (result.StartsWith("ERROR:"))
                {
                    // YES: Log the specific error
                    LogHelper.Log(LogLevel.Error, result);
                }
                else
                {
                    // NO: Log a general error
                    LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("ERROR_FOR_QUOTE")} " + symbol);
                }

                // Make the result null because of error encountered
                result = null;
            }

            // Return result from query
            return result;
        }

        //*********************************************************************
        //  Method: IsMarketOpen
        //
        /// <summary>
        /// Determine if a given market is open.
        /// </summary>
        /// 
        /// <param name="market">
        /// The name of the market whose open hours is being determined:
        /// 'BOND', 'EQUITY', 'FUTURE', 'FOREX', OPTION'
        /// </param>
        /// 
        /// <param name="marketTime">
        /// Regular, Pre, or Post market.
        /// </param>
        /// 
        /// <param name="futureName">
        /// Name of the Futures market.
        /// </param>
        /// 
        /// <returns>
        /// TRUE is the market is currently open, FALSE if it is note
        /// </returns>
        //*********************************************************************
        public static bool
            IsMarketOpen(
            string market,
            MarketTime marketTime = MarketTime.RegularMarket,
            string futureName = ""
            )
        {
            try
            {
                // Get market hours
                MarketHours hours = GetMarketHours(market, futureName);

                // Return whether market is open
                return IsOpen(hours, marketTime);
            }
            catch (Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("MARKET_HOURS")}. " + e.Message);
            }

            // Return false for market not open
            return false;
        }

        //*********************************************************************
        // Method: IsOpen
        //
        /// <summary>
        /// Get whether a given market is open for trading.
        /// </summary>
        /// 
        /// <param name="hours">
        /// A market hours object.
        /// </param>
        /// 
        /// <param name="marketTime">
        /// The market hours (pre, post, normal for considering whether the
        /// market is open.
        /// </param>
        /// 
        /// <returns>
        /// True if the market is open, false if it is closed.
        /// </returns>
        //*********************************************************************
        private static bool
            IsOpen
            (
            MarketHours hours,
            MarketTime marketTime = MarketTime.RegularMarket
            )
        {
            // Method members
            DateTime marketOpen = DateTime.MinValue;
            DateTime marketClosed = DateTime.MaxValue;

            // Was a valid market hours object returned?
            if (hours != null)
            {
                // YES: Are open hours given?
                if (hours.HoursOpen != null)
                {
                    // YES: Get the start and end times of the market based on which market
                    // we're checking--REGULAR, PRE, or POST. REGULAR is default.
                    // Get the start and end hours for the selected market time
                    switch (marketTime)
                    {
                        case MarketTime.PreMarket:
                            marketOpen = hours.HoursOpen.PreMarket[0].Start;
                            marketClosed = hours.HoursOpen.PreMarket[0].End;
                            break;

                        case MarketTime.PostMarket:
                            marketOpen = hours.HoursOpen.PostMarket[0].Start;
                            marketClosed = hours.HoursOpen.PostMarket[0].End;
                            break;

                        case MarketTime.RegularMarket:
                            marketOpen = hours.HoursOpen.RegularMarket[0].Start;
                            marketClosed = hours.HoursOpen.RegularMarket[0].End;
                            break;

                        default:
                            break;
                    }

                    // Market open and closed hours are in local time, 
                    // convert them to UTC
                    marketOpen = marketOpen.ToUniversalTime();
                    marketClosed = marketClosed.ToUniversalTime();

                    // Current UTC time between the market's open and close hors?
                    return (DateTime.UtcNow >= marketOpen && DateTime.UtcNow < marketClosed);
                }
            }

            // If hours not retrieved, then return market is not open
            return false;
        }

        //*********************************************************************
        //  Method: GetMarketHours
        //
        /// <summary>
        /// Get the hours a particular market is open.
        /// </summary>
        /// 
        /// <param name="market">
        /// The name of the market whose open hours is being determined:
        /// 'BOND', 'EQUITY', 'ETF', 'FOREX', 'FUTURE', 'FUTURE_OPTION', 
        /// 'INDEX', 'INDICATOR', 'MUTUAL_FUND', 'OPTION', 'UNKNOWN'
        /// </param>
        /// 
        /// <param name="rest">
        /// The TD Ameritrade RESTful API class object
        /// </param>
        /// 
        /// <returns>
        /// A MarketHours object with information about whether the market is
        /// currently open, and what times it is open.
        /// </returns>
        //*********************************************************************
        public static MarketHours
            GetMarketHours
            (
            string market,
            string futureName = ""
            )
        {
            // Method member
            Dictionary<string, object> assetDict = new Dictionary<string, object>();
            MarketHours marketHours;

            // Get the TD Ameritrade REST API
            if (rest == null) rest = new TDAmeritradeREST();

            //*****************************************************************
            //
            // NOTE: Forcing MARKET to EQUITY (NYSE) for TD Ameritrade at
            // present. API suggests that system will return hours for other
            // markets but it does not.
            //
            //*****************************************************************
            market = "EQUITY";

            // Call the TD Ameritrade API to get the market hours
            string result = rest.QueryApi(

                // The TD Ameritrade API method
                ApiMethod.GetMarketHours,

                // The data object passed to the API method
                ApiHelper.AccountDataWithQueryString(

                    // The market whose hours is being determined
                    market.ToUpper(),

                    // The query string used to get the market hours
                    string.Format("date={1}", settings.ClientId, DateTime.UtcNow.ToString("s")),

                    // Use client Id in query string, no need for access token
                    true
                    )
                );

            // Was a valid return received?
            if (!result.StartsWith("ERROR:"))
            {
                // YES: Split apart the result string
                string[] resultParts = Helper.SeparateCreds(result);

                // Get the server time (in OLE format)
                double oleDateTime = Convert.ToDouble(resultParts[0]);

                // Get the actual query results
                result = resultParts[1];

                // Is this for a FUTURES market?
                if (market == "FUTURE")
                {
                    // YES: Create a non-data contract JSON serializer
                    JavaScriptSerializer serializer = new JavaScriptSerializer();

                    // Deserialize the result returned from TD Ameritrade into
                    // the 1st level dictionary, i.e. <string, object>
                    assetDict = (Dictionary<string, object>)serializer.Deserialize<object>(result);

                    // Look-up the hours for this specific future
                    result = ((Dictionary<string, object>)assetDict["future"])[futureName].ToString().Replace("\r\n", "").Replace("\"", "'");
                }
                else
                {
                    // NO: Pare down the result to get just the hours
                    result = result.Substring(result.IndexOf("{\"date"));
                    result = result.Substring(0, result.Length - 2);
                }

                // Desrialize the JSON result to a class object
                marketHours = Broker.DeserializeJson<MarketHours>(result);

                // Put the server time in the market hours object
                marketHours.ServerTime = oleDateTime;

                // Return the market object
                return marketHours;
            }
            else
            {
                // NO: Valid return NOT received
                return null;
            }
        }

        //*********************************************************************
        //  Method: GetTradeInfo
        //
        /// <summary>
        /// Given a Zorro trade id, get the TD Ameritrade information regarding
        /// this trade
        /// </summary>
        /// 
        /// <param name="ZorroTradeId">
        /// The Zorro trade id.
        /// </param>
        /// 
        /// <returns>
        /// Trade object with information about the trade
        /// </returns>
        //*********************************************************************
        public static Trade
        GetTradeInfo
        (
            long TDATradeId,
            int ZorroTradeId
        )
        {
            // Method members
            Trade trade = null;

            // Get the trade information from TD Ameritrade. First,
            // instantiate a new REST API, if needed
            if (rest == null) rest = new TDAmeritradeREST();

            // Get account order
            string orderStr = rest.QueryApi(
                ApiMethod.GetOrder,
                ApiHelper.AccountOrderData(
                    Broker.settings.TdaAccountNum,
                    TDATradeId.ToString(),
                    true
                    )
                );

            // Was on order obtained?
            if (!String.IsNullOrEmpty(orderStr) && !orderStr.StartsWith("ERROR:"))
            {
                // YES: Deserialize it to a JSON Order
                Order order = Broker.DeserializeJson<Order>(orderStr);

                // Was the order REJECTED
                if (order.Status != "REJECTED")
                {
                    // NO: Create a new Trade object
                    trade = new Trade
                    {
                        // This gives the trade object the TD Ameritrade symbol
                        Asset = Trade.TDAm2ZorroSymbol(order.OrderLegCollection[0].Instrument.Symbol),
                        AssetType = order.OrderLegCollection[0].Instrument.AssetType,
                        Quantity = order.Quantity,
                        OrderType = order.OrderType,
                        ZorroTradeId = ZorroTradeId,
                        TDTradeId = TDATradeId,
                        Instruction = order.OrderLegCollection[0].Instruction,
                        Price = order.Price,
                        Filled = order.RemainingQuantity,
                        Status = order.Status
                    };

                    // If the order activity collection is present, add some
                    // properties from there
                    if (order.OrderAttivityCollection != null)
                    {
                        trade.Quantity = order.OrderAttivityCollection[0].Quantity;
                        trade.Entered = order.OrderAttivityCollection[0].ExecutionLegs[0].Time;
                    }

                    // Verify that an entered date is present. If not give the
                    // entered date as today's date in UTC.
                    if (trade.Entered == DateTime.MinValue) 
                        trade.Entered = DateTime.UtcNow;

                    // Return the trade object
                    return trade;
                }
                else
                {
                    // This order was rejected. Log the error.
                    LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("ORDER")} {Resx.GetString("REJECTED")} {Resx.GetString("TDAMTRADE")}.");
                }
            }

            return null;
        }

        //**********************************************************************
        //  Method: IsAssetInPortfolio
        //
        /// <summary>
        /// Determine if an asset is in the user's TD Ameritrade portofoilo.
        /// </summary>
        /// 
        /// <param name="symbol">
        /// Asset's ticker symbol.
        /// </param>
        /// 
        /// <returns>
        /// True if the asset is in the user's portfolio, otherwise false.
        /// </returns>
        //**********************************************************************
        public static bool
            IsAssetInPortfolio
            (
            string symbol
            )
        {
            // Get the position the user is hold for this asset
            int position = GetPosition(53, symbol);

            // If the position is anything but 0, the user is holding this
            // asset
            return position != 0;
        }

        //*********************************************************************
        //  Method: InitSettings
        //
        /// <summary>
        /// Initialize the plug-in settings.
        /// </summary>
        /// 
        /// <param name="usn">
        /// The username, which is actually the clientId from TD Ameritraed.
        /// </param>
        /// 
        /// <returns>
        /// True if settings read successfully and initialized, false if not.
        /// </returns>
        //*********************************************************************
        public static bool
            InitSettings
            (
            string usn
            )
        {
            // Method member
            bool retCode = true;

            // Read and deserialize the settings file
            settings = Settings.Read();

            // Were settings obtained?
            if (settings == null)
            {
                // NO: Error has been logged, exit with failure code
                return false;
            }

            // Break apart the usn into client_id and language spec
            string[] creds = Helper.SeparateCreds(usn);

            // The client Id (consumer key) and save it in the settings
            settings.ClientId = creds[0];

            // If a language specification is included?
            if (creds.Length > 1)
            {
                // YES: Validate it
                retCode = ValidateLang(creds[1]);

                // Valid language spec?
                if (retCode)
                {
                    // YES: Set language property of Settings object
                    settings.LangResx = creds[1];
                }
                else
                {
                    // NO: Return from login with an error
                    return false;
                }
            }

            // Return from the initialization of the settings
            return retCode;
        }

        //*********************************************************************
        //  Method: DeserializeJson
        //
        /// <summary>
        /// Deserialize a JSON string.
        /// </summary>
        /// 
        /// <param name="json">
        /// The json string being deserialized.
        /// </param>
        /// 
        /// <typeparam name="T">
        /// The class type this JSON object is being converted into.
        /// </typeparam>
        /// 
        /// <returns>
        /// A class object of type T.
        /// </returns>
        /// 
        /// <remarks>
        /// For some reason JSON.NET and the normal DataContractJsonSerializer
        /// throw an error when run under Zorro. So, we are resorting to using
        /// a buffered JSON reader rather than a memory stream to create a POCO
        /// from the JSON string.
        /// </remarks>
        //*********************************************************************
        public static T
            DeserializeJson<T>
            (
                string json
            )
        {
            // Convert string to bytes
            byte[] result = Encoding.UTF8.GetBytes(json);

            // Get a JSON reader
            using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(result, XmlDictionaryReaderQuotas.Max))
            {
                var settings = new DataContractJsonSerializerSettings
                {
                    DateTimeFormat = new DateTimeFormat("yyyy-MM-ddTHH:mm:sszzz")
                };
                // Create a new JSON data contractserializer of the required
                // class type
                var outputSerialiser = new DataContractJsonSerializer(typeof(T), settings);

                // Read the buffered JSON data and return it as a POCO of the
                // required class stype
                return (T)outputSerialiser.ReadObject(jsonReader);
            }
        }

        //*********************************************************************
        //  Method: JsonDictConvert
        //
        /// <summary>
        /// Convert a JSON object within a dictionary to a class object.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// Class converting JSON object to.
        /// </typeparam>
        /// 
        /// <param name="dict">
        /// Dictionary containing the keys and values of the JSON object.
        /// </param>
        /// 
        /// <returns>
        /// Class of type T containing the JSON object
        /// </returns>
        /// 
        /// <remarks>
        /// Use this conversion process instead of the simpler methods of
        /// JSON.NET because some 3rd party libraries do not function with this
        /// DLL under Zorro.
        /// </remarks>
        //*********************************************************************
        public static T
            JsonDictConvert<T>
            (
            Dictionary<string, object> dict
            )
        {
            // Method members
            string json = "{";
            string value = string.Empty;

            // Iterate through each key of the dictionary
            foreach (string key in dict.Keys)
            {
                // Skip if the key value is null
                if (dict[key] == null) continue;

                // Get the key value as a string.
                value = dict[key].ToString();

                // Convert True or False strings to LC, so deserialization will
                // work correctly
                if (value.ToLower() == "true") value = "true";
                if (value.ToLower() == "false") value = "false";

                // Addd key and value to JSON string
                json += $"\"{key}\" : \"{value}\",";
            }

            // Trim the last comma from the JSON string and add closing brace
            json = json.Trim(',') + "}";

            // Convert the JSON string to the requested class and return the
            // class object
            return Broker.DeserializeJson<T>(json);
        }

        //*********************************************************************
        //  Method: ConvertOptionChainJson
        //
        /// <summary>
        /// Convent the JSON option chain data into a list of contract
        /// structures.
        /// </summary>
        /// 
        /// <param name="oc">
        /// The basic option chain information.
        /// </param>
        /// 
        /// <param name="contractType">
        /// The contract type (PUT or CALL).
        /// </param>
        /// 
        /// <param name="dateDict">
        /// The dictionary of expiration dates, and option chaing information.
        /// </param>
        /// 
        /// <returns>
        /// A list of CONTRACT structures.
        /// </returns>
        //*********************************************************************
        public static List<CONTRACT>
            ConvertOptionChainJson
            (
            OptionChain oc,
            ContractType contractType,
            Dictionary<string, object> dateDict
            )
        {
            // THe expiry date (YYYYMMDD long)
            long expiry;

            // The array of contract structures to return
            List<CONTRACT> contracts = new List<CONTRACT>();

            // The strike price dictionary with keys at each strike price
            // value
            Dictionary<string, object> spDict;

            // The values dictionary with values for each strike price
            Dictionary<string, object> valDict;

            // The strike price information object with information on each
            // value for a given strike price
            StrikePriceInfo spi;

            // The timestamp of this contract information
            double time = DateTime.UtcNow.ToOADate();

            // Iterate through each date the date dictionary
            foreach(string dateKey in dateDict.Keys)
            {
                // Convert the date key to a long date value of the form
                // YYYYMMDD
                expiry = GetContractExpiry(dateKey);

                // Get the underlying strike price dictionary
                spDict = (Dictionary<string, object>)dateDict[dateKey];

                // Iterate through each strike price in the strike price
                // dictionary
                foreach (string spKey in spDict.Keys)
                {
                    // Get the object of this strike price, which has the
                    // relevant values
                    object[] val = (object[])spDict[spKey];

                    // Get the dictionary of values for this strike price
                    valDict = (Dictionary<string, object>)val[0];

                    // Convert the strike price values to a strike price
                    // object
                    spi = JsonDictConvert<StrikePriceInfo>(valDict);

                    // Fill-in a contract structure
                    contracts.Add(new CONTRACT
                        (
                        time,
                        (float)spi.Ask,
                        (float)spi.Bid,
                        (float)spi.TimeValue,
                        (float)spi.TotalVolume,
                        (float)oc.UnderlyingPrice,
                        Convert.ToSingle(spKey),
                        expiry,
                        (long)contractType
                        ));
                }
            }

            // Return the list of contracts
            return contracts;
        }

        //*********************************************************************
        //  Method: LicenseAccepted
        //
        /// <summary>
        /// Get whether the plug-in license has been accepted.
        /// </summary>
        /// 
        /// <returns>
        /// True if accepted, false if not.
        /// </returns>
        //*********************************************************************
        private static bool
            LicenseAccepted
            ()
        {
            // Method member
            bool acceptance = false;

            // Are the app version numbers the same?
            if (AppVersionNumbersEqual())
            {
                // YES: Get the registry value for the license acceptance
                string acceptRegistry = Helper.GetRegistryValue(LICENSE_ACCEPTANCE);

                // Convert registery value to a boolean
                if (!string.IsNullOrEmpty(acceptRegistry))
                    acceptance = int.Parse(acceptRegistry) == 1;
                else
                    acceptance = false;
            }
            else
            {
                // NO: Does not matter whether previous acceptance is on-file.
                // New version number requires new acceptance.
                acceptance = false;
            }

            // Return the value of acceptance
            return acceptance;
        }

        //*********************************************************************
        //  Method: ShowLicenseForm
        //
        /// <summary>
        /// Show the Windows Form with the plug-in license
        /// </summary>
        /// 
        /// <returns>
        /// True if license accepted, false if not.
        /// </returns>
        //*********************************************************************
        private static bool
            ShowLicenseForm
            ()
        {
            // Create a license form object
            LicenseForm license = new LicenseForm();

            // Show the form and get the return result
            DialogResult result = license.ShowDialog();

            // Destroy the Windows Form Icon
            bool bDestroy = Helper.DestroyIcon(license.icon.Handle);
            if (!bDestroy) LogHelper.Log(LogLevel.Error, $"{Resx.GetString("WINDOWS_FORM_ICON_NOT_DESTROYED")}");

            // Has user accepted the license?
            if (result == DialogResult.Yes)
            {
                // Save the acceptance
                Helper.SetRegistryValue(LICENSE_ACCEPTANCE, "1");

                // Return acceptance
                return true;
            }
            else
            {
                // Save declination
                Helper.SetRegistryValue(LICENSE_ACCEPTANCE, "0");

                // Return non-acceptance
                return false;
            }
        }

        //*********************************************************************
        //  Method: GetContractExpiry
        //
        /// <summary>
        /// Get the contract expiration date.
        /// </summary>
        /// 
        /// <param name="dateStr">
        /// The string contract expiration date.
        /// </param>
        /// 
        /// <returns>
        /// An expiration date in the form YYYYMMDD as a long int.
        /// </returns>
        //*********************************************************************
        private static long 
            GetContractExpiry
            (
            string dateStr
            )
        {
            // Split the date YYYY-MM-DD:NN into its components
            string[] dateParts = dateStr.Split('-');

            // Reform the date YYYYMMDD (as a long)
            string retDate = dateParts[0] + dateParts[1] + dateParts[2].Substring(0, 2);

            // Convert to long and return
            return Convert.ToInt64(retDate);
        }

        //*********************************************************************
        //  Method ToUnixEpoch
        //
        /// <summary>
        /// Converts a DateTime walue to the number of milliseconds since the 
        /// UNIK epoch.
        /// </summary>
        /// 
        /// <param name="dateTime">
        /// A DateTime to convert to epoch time.
        /// </param>
        /// 
        /// <returns>
        /// The long number of milliseconds since the UNIX epoch.
        /// </returns>
        //*********************************************************************
        private static long ToUnixEpoch(DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;

        //*********************************************************************
        //  Method: AppVersionNumbersEqual
        //
        /// <summary>
        /// Get the current app version number and build date.
        /// </summary>
        /// 
        /// <returns>
        /// True if the version (major and minor) has not changed, false if it
        /// has changed.
        /// </returns>
        //*********************************************************************
        public static bool
            AppVersionNumbersEqual
            ()
        {
            // Method member
            bool retVal = false;

            // Get the app version from the executing assembly
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            // Get the build date from the app date
            var buildDate = new DateTime(2000, 1, 1)
                       .AddDays(version.Build).AddSeconds(version.Revision * 2);

            // Stringify the registry app version
            string versionString = version.ToString();

            // Get the last app date from the registry
            var regVersion = Helper.GetRegistryValue(VERSION_NUMBER);

            // Get whether the last version is the same as the current version
            retVal = CompareVersionNumbers(versionString, regVersion);

            // Are the versions equal?
            if (!retVal)
            {
                // NO: Save the current version number
                Helper.SetRegistryValue(VERSION_NUMBER, versionString);

                // Save the current build date
                Helper.SetRegistryValue(VERSION_BUILD_DATE, buildDate.ToString(DATE_FORMAT_FULL));
            }

            // Return whether the registry key has changed
            return retVal;
        }

        //*********************************************************************
        //  Method: CompareVersionNumbers
        //
        /// <summary>
        /// Compare values for two version numbers.
        /// </summary>
        /// 
        /// <param name="versionA">
        /// First version number.
        /// </param>
        /// 
        /// <param name="versionB">
        /// Second version number.
        /// </param>
        /// 
        /// <returns>
        /// True if the major and minor version numbers are the same, false if
        /// they are not.
        /// </returns>
        //*********************************************************************
        public static bool
            CompareVersionNumbers
            (
            string versionA,
            string versionB
            )
        {
            // Split apart the versions
            string[] A = versionA.Split('.');
            string[] B = versionB.Split('.');

            // Array of version number parts needs to be the same or versions
            // are not equal
            if (A.Length != B.Length) return false;

            // Return comparison of major and minor version numbers
            return (Convert.ToInt32(A[0])) == Convert.ToInt32(B[0]) &&
                (Convert.ToInt32(A[1])) == Convert.ToInt32(B[1]);
        }
        #endregion PRIVATE METHODS
    }
}