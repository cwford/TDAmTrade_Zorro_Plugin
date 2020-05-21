//*****************************************************************************
// File: Tests.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: A variety of self-diagnostic tests.
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
using System.IO;
using System.Linq;
using TDAmeritradeZorro.Authentication;
using TDAmeritradeZorro.Authentication.Client;
using TDAmeritradeZorro.Authentication.Configuration;
using TDAmeritradeZorro.Classes.TDA;
using TDAmeritradeZorro.Structs;
using TDAmeritradeZorro.Utilities;

namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Class: Tests
    //
    /// <summary>
    /// This class supports testing of the methods and other components of this
    /// plug-in.
    /// </summary>
    //*************************************************************************
    public static class Tests
    {
        //*********************************************************************
        //  Method: DoTests
        //
        /// <summary>
        /// Main method for performing self-diagnostic tests.
        /// </summary>
        /// 
        /// <param name="usn">
        /// The text entered into the user name textbox of Zorro (client id).
        /// </param>
        /// 
        /// <returns>
        /// True if all tests successful, false if any test fails.
        /// </returns>
        /// 
        /// <remarks>
        /// NOTE: These tests evaluate only the internal operation of the plug-in.
        /// To evaluate communication between the Zorro trading engine and the
        /// plug-in run Zorro in 'Real' mode and select the TDAmeritradeTest
        /// script.
        /// </remarks>
        //*********************************************************************
        public static bool
            DoTests
            (
            string usn
            )
        {
            // Method members
            bool retCode;
            AuthToken token;

            // Demo mode information for the user
            LogHelper.Log("\r\nDEMO MODE");
            LogHelper.Log("---- ----");
            LogHelper.Log($"{Resx.GetString("TEST_HEADING")}");

            // Set the test mode
            TDAmerAPI.TestMode = true;

            // Initialize the database tables
            Broker.CreateDbTables();

            // Set the VERBOSITY LEVEL
            TDAmerAPI.verbosityLevel = Verbosity.Extensive; 

            //*****************************************************************
            //
            // T E S T  # 1:  S E T T I N G S  F I L E
            //
            //*****************************************************************
            LogHelper.Log($"\r\n{Resx.GetString("TEST")} #1:");
            LogHelper.Log($"{Resx.GetString("READING_SETTINGS_FILE")}....");
            Broker.settings = Settings.Read();

            // Were settings obtained?
            if (Broker.settings == null ||
                string.IsNullOrEmpty(Broker.settings.Currency) ||
                string.IsNullOrEmpty(Broker.settings.TdaAccountNum))
            {
                // NO: Error has been logged, exit with failure code
                LogHelper.Log($"{Resx.GetString("SETTINGS_FILE_ERROR")}....");
                return false;
            }
            else
            {
                LogHelper.Log($"{Resx.GetString("SUCCESS").ToUpper()}: {Resx.GetString("READING_SETTINGS_FILE")}.");
            }

            //*****************************************************************
            //
            // T E S T  # 2:  U S E R  A U T H E N T I C A T I O N
            //
            //*****************************************************************
            LogHelper.Log($"\r\n{Resx.GetString("TEST")} #2:");
            LogHelper.Log($"{Resx.GetString("AUTHENTICATING_USER")}...");

            // The username is actually the client id, save it in settings
            Broker.settings.ClientId = usn;

            // Save the full path of the token data file
            Broker.tokenDataFile = Broker.WORKING_DIR + Broker.dataFile;

            // Initialize the connection configuration using the refresh token
            // (usn) and the client id obtained from the settings
            Broker.oAuthConfiguration = new TDAmeritradeConnectConfiguration(Broker.settings.ClientId);

            // Initialize the connection client with the configuration
            Broker.oAuthClient = new TDAmeritradeConnectClient(Broker.oAuthConfiguration);

            // Delete the token file
            if (File.Exists(Broker.tokenDataFile)) File.Delete(Broker.tokenDataFile);

            // Get an authentication token
            token = GetAuthToken();

            // Auth token received?
            if (token != null)
            {
                LogHelper.Log($"{Resx.GetString("SUCCESS").ToUpper()}: {Resx.GetString("TEST_AUTHENTICATION_AUTH_TOKEN")}");
            }
            else
            {
                 LogHelper.Log(LogLevel.Error, $"{Resx.GetString("FAILURE").ToUpper()}: {Resx.GetString("TEST_AUTHENTICATION_FAILURE")}.");
                return false;
            }

            //*****************************************************************
            //
            // T E S T  # 3:  R E F R E S H   A U T H E N T I C A T I O N
            //
            //*****************************************************************
            LogHelper.Log($"\r\n{Resx.GetString("TEST")} #3:");
            LogHelper.Log($"{Resx.GetString("TEST_REFRESH_TOKEN_AUTH")}...");

            // Get another auth token, should be using refresh token now
            token = GetAuthToken();

            // Auth token received?
            if (token != null)
            {
                LogHelper.Log($"{Resx.GetString("SUCCESS").ToUpper()}: {Resx.GetString("TEST_ACCESS_TOKEN_FROM_REFRESH")}.");
            }
            else
            {
                LogHelper.Log(LogLevel.Error, $"{Resx.GetString("SUCCESS").ToUpper()}: {Resx.GetString("TEST_NO_ACCESS_TOKEN_FROM_REFRESH")}.");
                return false;
            }

            //*****************************************************************
            //
            // T E S T  # 4:  M A R K E T  H O U R S
            //
            //*****************************************************************
            LogHelper.Log($"\r\n{Resx.GetString("TEST")} #4:");
            LogHelper.Log($"{Resx.GetString("GET_NYSE_HOURS")}....");

            bool isNYSEOpen = Broker.IsMarketOpen("EQUITY");

            // Print out results
            string open = isNYSEOpen ? Resx.GetString("OPEN").ToUpper() : Resx.GetString("NOT_OPEN").ToUpper();
            LogHelper.Log($"{Resx.GetString("CURRENT_UTC_TIME")} {DateTime.UtcNow}.");
            LogHelper.Log($"{Resx.GetString("THE_NYSE_IS")} {open}.");

            //*****************************************************************
            //
            // T E S T  # 5:  B R O K E R  T I M E
            //
            //*****************************************************************
            LogHelper.Log($"\r\n{Resx.GetString("TEST")} #5:");
            LogHelper.Log($"{Resx.GetString("BROKER_TIME")}...");
            double? dServerTime;

            // Call the Broker.Time function and get the server time out
            int code = Broker.Time(out dServerTime);

            // Print the current time
            LogHelper.Log($"{Resx.GetString("CURRENT_UTC_TIME")} {DateTime.UtcNow}.");

            // Evaluate return code from Broker.Time
            if (code == 0)
            {
                LogHelper.Log($"{Resx.GetString("ERROR")}: {Resx.GetString("CONN_LOST")}.");
                return false;
            }
            else if (code == 1)
            {
                LogHelper.Log($"{Resx.GetString("CONN_OK_MARKET_CLOSED")}.");
            }
            else
            {
                LogHelper.Log($"{Resx.GetString("CONN_OK_MARKET_OPEN")}.");
            }

            LogHelper.Log($"{Resx.GetString("SERVER_UTC_OLE")}: {dServerTime}.");
            LogHelper.Log($"{Resx.GetString("SERVER_UTC_CONVERTED")}: {DateTime.FromOADate((double)dServerTime)}.");

            // Get the server time and the current time
            DateTime currentTime = DateTime.UtcNow;
            DateTime serverTime = DateTime.FromOADate((double)dServerTime);

            // Get the difference in the two times
            TimeSpan diffTime = serverTime - currentTime;
            int secondsDiff = Math.Abs(diffTime.Seconds);
            if (secondsDiff < 30)
            {
                LogHelper.Log($"{Resx.GetString("TEST_SERVER_TIME_SUCCESS")}.");
            }
            else
            {
                LogHelper.Log($"{Resx.GetString("TEST_SERVER_TIME_FAILURE")}.");
                return false;
            }

            //*****************************************************************
            //
            // T E S T  # 6:  A S S E T  S U B S C R I P T I O N
            //
            //*****************************************************************
            LogHelper.Log($"\r\n{Resx.GetString("TEST")} #6:");
            LogHelper.Log($"{Resx.GetString("BROKER_ASSET")}...");

            // Use Microsoft
            string symbol = "MSFT";
            TDAsset asset = Broker.Asset(symbol);
            if (asset == null)
            {
                LogHelper.Log($"{Resx.GetString("TEST_ASSET_INFO_FAILURE")} {symbol}.");
                return false;
            }

            // Subscribe to this asset
            Broker.Subscription(asset);

            // Is asset on subscription list
            if (Broker.subscriptionList.Count > 0 && Broker.subscriptionList[0].Symbol == symbol)
            {
                LogHelper.Log($"{Resx.GetString("TEST_ASSET_SUCCESS")} {symbol}.");
            }
            else
            {
                LogHelper.Log($"{Resx.GetString("TEST_ASSET_FAILURE")} {symbol}.");
                return false;
            }

            //*****************************************************************
            //
            // T E S T  # 7:  U P D A T E  A S S E T S  C S V  F I L E
            //
            //*****************************************************************
            LogHelper.Log($"\r\n{Resx.GetString("TEST")} #7:");
            LogHelper.Log($"{Resx.GetString("UPDATING_ASSETS_CSV")}...");

            // Update the assets CSV fileU
            retCode = AssetsCSV.UpdateAssetsCSV();
            if (retCode)
            {
                LogHelper.Log($"{Resx.GetString("SUCCESS").ToUpper()}: {Resx.GetString("UPDATING_ASSETS_CSV")}.");
            }
            else
            {
                LogHelper.Log($"{Resx.GetString("FAILURE").ToUpper()}: {Resx.GetString("UPDATING_ASSETS_CSV")}.");
                return false;
            }

            //*****************************************************************
            //
            // T E S T  # 8:  T R A D E  F I L E  S Y N C H R O N I Z A T I O N
            //
            //*****************************************************************
            LogHelper.Log($"\r\n{Resx.GetString("TEST")} #8:");
            LogHelper.Log($"{Resx.GetString("TRADE_FILE_SYNCHRONIZATION")}...");

            // Update the assets CSV file
            retCode = Broker.Sync();
            if (retCode)
            {
                LogHelper.Log($"{Resx.GetString("SUCCESS").ToUpper()}: {Resx.GetString("TRADE_FILE_SYNCHRONIZATION")}.");
            }
            else
            {
                LogHelper.Log($"{Resx.GetString("FAILURE").ToUpper()}: {Resx.GetString("TRADE_FILE_SYNCHRONIZATION")}.");
                return false;
            }


            //*****************************************************************
            //
            // T E S T  # 9:  M A R K E T  B U Y / S E L L

            //*****************************************************************
            LogHelper.Log($"\r\n{Resx.GetString("TEST")} #9:");
            LogHelper.Log($"{Resx.GetString("TEST_BUY_SELL_INFO")}");

            // The test symbol
            symbol = "GRPN";

            // Get information for
            TDAsset tdAsset = Broker.Asset(symbol);
            Trade order = null;

            // Was the information obtained for this asset?
            if (tdAsset != null)
            {

                // YES: Subscribe to this asset
                Broker.Subscription(tdAsset);

                // Is the NYSE Open?
                if (!Broker.IsMarketOpen("EQUITY"))
                {
                    // NO: BUY 1 share of tdAsset AT MARKET
                    order = PlaceOrder(tdAsset.Symbol, 2, 0, 0);
                    if (!GetOrderInfo(order)) return false;

                    // SELL 1 share of tdAsset AT MARKET
                    PlaceOrder(tdAsset.Symbol, -12, 0, 0);
                    if (!GetOrderInfo(order)) return false;

                }

                PlaceOrder(tdAsset.Symbol, 2, 1.50, 150);

                // Can always place LIMIT orders with outrageous limits so they can
                // be canceled immediately.

                // BUY 1 share of tdAsset AT LIMIT price (100% above current price)
                // Asset price should never get there, so order should pend and
                // easily be canceled.
                order = PlaceOrder(tdAsset.Symbol, 1, 0, tdAsset.Price * 2);
                if (!GetOrderInfo(order)) return false;

                // SELL 1 share of tdAsset AT LIMIT price (100% above current
                // price) Asset price should never get there, so order should pend 
                // and easily be canceled.
                order = PlaceOrder(tdAsset.Symbol, -12, 0, tdAsset.Price * 2);
                if (!GetOrderInfo(order)) return false;
            }
            else
            {
                // Log the error
                LogHelper.Log($"{Resx.GetString("ERROR")}: {Resx.GetString("SUBSCRIBING_TO_ASSET")}.");

                // Return error status
                return false;
            }

            //*****************************************************************
            //
            // T E S T  # 10:  B R O K E R  H I S T O R Y
            //
            //*****************************************************************
            LogHelper.Log($"\r\n{Resx.GetString("TEST")} #10:");
            LogHelper.Log($"{Resx.GetString("TEST_HISTORICAL_INFO")}");

            // Get the historical ticks for this asset
            Tick[] ticks = Broker.History(
                // Symbol
                "MSFT",

                // Start date
                DateTime.Parse("16 Feb 2019 14:00:00 GMT").ToOADate(),

                // End date
                DateTime.Parse("05 May 2020 18:00:00 GMT").ToOADate(),

                // Number of ticks per minute
                60,

                // Maximum number of ticks
                300
                );

            if (ticks.Count() == 300)
            {
                LogHelper.Log(LogLevel.Info, $"{Resx.GetString("SUCCESS").ToUpper()}: {Resx.GetString("MSFT_PRICE_HISTORY")}.") ;
            }
            else
            {
                LogHelper.Log(LogLevel.Error, $"{Resx.GetString("FAILURE").ToUpper()}: {Resx.GetString("MSFT_PRICE_HISTORY")}.");
                return false;
            }

            //*****************************************************************
            //
            // T E S T  # 11:  U S E R  A C C O U N T
            //
            //*****************************************************************
            LogHelper.Log($"\r\n{Resx.GetString("TEST")} #11:");
            LogHelper.Log($"{Resx.GetString("GET_USER_ACCT_INFO")}...");

            AccountBalance bal = Broker.Account(Broker.settings.TdaAccountNum);
            if (bal != null)
            {
                LogHelper.Log(LogLevel.Info, $"{Resx.GetString("SUCCESS").ToUpper()}: {Resx.GetString("RETRIEVE_ACCT_INFO")}{bal.AccountId}.");
            }
            else
            {
                LogHelper.Log(LogLevel.Error, $"{Resx.GetString("FAILURE").ToUpper()}: {Resx.GetString("RETRIEVE_ACCT_INFO")}{bal.AccountId}.");
                return false;
            }

            // If testing made it here, all tests were successful
            return true;
        }

        //*********************************************************************
        //  Method: GetOrderInfo
        //
        /// <summary>
        /// Get order information for an order.
        /// </summary>
        /// 
        /// <param name="order">
        /// Order to get information on.
        /// </param>
        /// 
        /// <returns>
        /// True if order information obtained successfully, false if not.
        /// </returns>
        //*********************************************************************
        private static bool
            GetOrderInfo
            (
            Trade order
            )
        {
            // Was the order successful?
            if (order.ZorroTradeId > 0)
            {
                order = Broker.BrokerTrade(order.ZorroTradeId);
                if (order != null)
                {
                    LogHelper.Log($"{Resx.GetString("TEST_ORDER_INFO_SUCCESS")} {order.Status}.");
                    return true;
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, $"{Resx.GetString("TEST_ORDER_INFO_FAILURE")}.");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //*********************************************************************
        //  Method: PlaceOrder
        //
        /// <summary>
        /// Place a trade order with TD Ameritrade using the REST API, then
        /// immediately cancel that order.
        /// </summary>
        /// 
        /// <param name="symbol">
        /// Ticker symbol to place order for.
        /// </param>
        /// 
        /// <param name="amount">
        /// Number of shares.
        /// </param>
        /// 
        /// <param name="dStopDist">
        /// Stop distance.
        /// </param>
        /// 
        /// <param name="dLimit">
        /// Limit price for LIMIT orders.
        /// </param>
        /// 
        /// <param name="session">
        /// Market session to trade at, TD Ameritrade only uses NORMAS for thu
        /// REST API.
        /// </param>
        /// 
        /// <returns>
        /// True if order placed was a success AND the order was then canceled,
        /// otherwise false.
        /// </returns>
        //*********************************************************************
        private static Trade
            PlaceOrder
            (
            string symbol,
            int amount,
            double dStopDist,
            double dLimit,
            string session = "NORMAL"
            )
        {
            // Method members
            bool retCode = false;
            string saleType = (amount > 0) ? Resx.GetString("BUYING") : Resx.GetString("SELLING");
            string orderType = (dLimit > 0) ? Resx.GetString("LIMIT") + " " + Resx.GetString("PRICE_OF") + " " + dLimit.ToString("N4") : Resx.GetString("MARKET");


            // Log the attempted trade
            LogHelper.Log($"\r\n{Resx.GetString("TRADE")}: {saleType} {Math.Abs(amount)} {Resx.GetString("SHARES_OF")} {symbol} {Resx.GetString("AT")} {orderType}.");

            // Place the order (force order at session)
            Trade trade = Broker.Buy(symbol, amount, dStopDist, dLimit, session);

            // Log success or failure?
            if (trade.TDTradeId > 0)
            {
                // Trade SUCCESS:
                LogHelper.Log($"{Resx.GetString("SUCCESS").ToUpper()}: {Resx.GetString("PLACING_ORDER")} {symbol}.");
            }
            else
            {
                if (Broker.ComboLegs > 0)
                {
                    // Trade WAITING
                    LogHelper.Log($"{Resx.GetString("WAITNG")}: {Resx.GetString("FOR_MORE_ORDER_LEGS")} {Resx.GetString("PLACING_ORDER")} {symbol}.");
                }
                else
                {
                    // Trade FAILURE:
                    LogHelper.Log(LogLevel.Error, $"{Resx.GetString("FAILURE").ToUpper()}: {Resx.GetString("PLACING_ORDER")} {symbol}.");
                }
            }

            // Cancel the trade if it was made
            if (trade.TDTradeId > 0)
            {
                retCode = Broker.Cancel(trade.ZorroTradeId);
                if (retCode)
                {
                    LogHelper.Log($"{Resx.GetString("SUCCESS").ToUpper()}: {Resx.GetString("CANCELING_ORDER")} {symbol}.");
                }
                else
                {
                    LogHelper.Log($"{Resx.GetString("FAILURE").ToUpper()}: {Resx.GetString("CANCELING_ORDER")} {symbol}.");
                }
            }

            // Return the trade
            return trade;
        }

        //*********************************************************************
        //  Method: GetAuthToken
        //
        /// <summary>
        /// Get an authorization token, necessary to use the REST API.
        /// </summary>
        /// 
        /// <returns>
        /// An outhorization tokn, or null if that token can not be obtained.
        /// </returns>
        //*********************************************************************
        private static AuthToken
            GetAuthToken
            ()
        {
            // Method member
            AuthToken token = null;

            // Does the token file exist?
            if (File.Exists(Broker.tokenDataFile))
            {
                // YES: Get an authentication token using a refresh token
                token = AuthToken.GetAuthToken();
            }
            else
            {
                // NO: This must be the initial use of the plug-in, in that
                // case get an access token by fully authenticating user
                // with the client id.

                // Execute the longer, one time only, user authentication.
                token = AuthToken.AuthenticateUser(Broker.settings.ClientId);
            }

            // Return the authentication token
            return token;
        }
    }
}
