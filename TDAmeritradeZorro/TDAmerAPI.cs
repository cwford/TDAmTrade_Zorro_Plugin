//*****************************************************************************
// File: TDAmerAPI.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: A broker plug-in for Zorro that allows the Zorro trading
// engine to use TD Ameritrade as a broker through the TD Ameritrade API. See
// the ReadMe file and other documentation for important information prior to
// deploying this plug-in.
//
// Copright (c) 2020 Clyde W. Ford. All rights reserved.
//
// License: LGPL-3.0 (Non-commercial use only)
//
// Notes: There a two related NuGet packages for DLLExport that have similar
// namespaces and methods: UnmanagedExports and DLLExpart. DO NOT attempt to
// compile this plug-in with UnmanagedExports. The namespaces and methods may
// resolve but you will receive unresolvable compilation and runtime errors.
// USE ONLY DLLExport (https://github.com/3F/DllExport) available as a NuGet
// package to compile this plug-in. 
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TDAmer;
using TDAmeritradeZorro.Classes;
using TDAmeritradeZorro.Classes.DBLib;
using TDAmeritradeZorro.Classes.TDA;
using TDAmeritradeZorro.Structs;
using TDAmeritradeZorro.Utilities;

namespace TDAmeritradeZorro
{
    //*************************************************************************
    //  Class: TDAmerAPI
    //
    /// <summary>
    /// The class implements the functionality of the Zorro calls to a plug-in.
    /// Although Zorro expects a 32-bit plug-in with typical C++ calling de-
    /// clarations, this plug-in uses the DllExport Library to achieve a 32-
    /// bit plug-in that exposes the broker functions which can be called by
    /// Zorro. The DLL Export Library can be found at:
    /// 
    ///  https://github.com/3F/DllExport/releases
    ///  
    /// NOTE: There is another DllExport library by Robert Giesecke, entitled
    /// UnmanagedExports which has the similar namespaces and similar methods.
    /// DO NOT USE this DLL only use the first one referenced above.
    /// 
    /// Some C# implementations of Broker plugins use double arrays as output
    /// arguments for the double pointers passed from Zorro. In general, this
    /// will work because the first element of an array is also the address of
    /// the pointer to that array. The problem is that Zorro sometimes passes
    /// a null pointer. Using a managed code array will not be able to detect
    /// a null pointer, and will result in an error when a double value is
    /// stored at that location. This plug-in accepts all Zorro output para-
    /// meters as IntPtrs which are then tested for being null and converted
    /// to double pointers in an UNSAFE code block. Thus, this plug-in should
    /// always be built with the 'unsafe' method checked.
    /// </summary>
    //*************************************************************************
    public class TDAmerAPI
    {
        #region DELEGATES
        //*********************************************************************
        //  Delegate: BrokerErrorDelegate
        //
        /// <summary>
        /// A delegate used to allow the BrokerError method to write to the
        /// Zorro window.
        /// </summary>
        //*********************************************************************
        public delegate int BrokerErrorDelegate(string txt);

        //*********************************************************************
        //  Delegate: BrokerProgressDelegate
        //
        /// <summary>
        /// A delegate used to allow the BrokerProgress method to show a
        /// progress indicator in the Zorro window.
        /// </summary>
        //*********************************************************************
        public delegate int BrokerProgressDelegate(int percent);
        #endregion DELEGATES

        #region PUBLIC MEMBERS
        //*********************************************************************
        //  Delegate: BrokerError
        //
        /// <summary>
        /// Reference to the method that writes to the Zorro window. This
        /// method made public so it can be called in methods outside of this
        /// class.
        /// </summary>
        //*********************************************************************
        public static BrokerErrorDelegate BrokerError;

        //*********************************************************************
        //  Delegate: opMode
        //
        /// <summary>
        /// The operating mode of the plug-in (Demo or Real).
        /// </summary>
        //*********************************************************************
        public static OpMode opMode;
        #endregion PUBLIC MEMBERS

        #region PRIVATE MEMBERS
        //*********************************************************************
        //  Delegate: BrokerProgress
        //
        /// <summary>
        /// Reference to the method that shows a progress indicator on the
        /// Zorro window.
        /// </summary>
        //*********************************************************************
        private static BrokerProgressDelegate BrokerProgress;

        //*********************************************************************
        //  Member: PLUGIN_VERSION
        //
        /// <summary>
        /// The broker interface version number, set by Zorro, currently at 2.
        /// </summary>
        //*********************************************************************
        private const int PLUGIN_VERSION = 2;

        //*********************************************************************
        //  Member: isConnected
        //
        /// <summary>
        /// Indicates whether the plug-in is connected to the TD Ameritrade
        /// server API.
        /// </summary>
        /// 
        /// <remarks>
        /// True = connected; False = not connected.
        /// </remarks>
        //*********************************************************************
        private static bool isConnected;

        //*********************************************************************
        //  Member: verbosityLevel
        //
        /// <summary>
        /// The verbosity level of the diagnostic messages from the plug-in
        /// </summary>
        /// 
        /// <remarks>
        /// Verbosity Levels are:
        /// 
        /// 0 =		Few messages
        /// 1 =		More importanat messages (default)
        /// 2 =		Even more messages, including command parameters
        /// 3 =		Still more messages, include skipped trades, possible 
        ///         outliers and
        ///			unction parameter errors
        /// 7 =		Extensive diagnostic messages, all messages
        /// +512    Print all messages to the Zorro log window as well as the
        ///         log file.
        /// </remarks>
        //*********************************************************************
        public static Verbosity verbosityLevel = Verbosity.Extreme | Verbosity.TimeStamp | Verbosity.LineNumbers;

        //*********************************************************************
        //  Member: TestMode
        //
        /// <summary>
        /// Indicates whether the plug-in is in test mode.
        /// </summary>
        /// 
        /// <remarks>
        /// True = in test mode; False = in live mode.
        /// </remarks>
        //*********************************************************************
        public static bool TestMode = false;
        #endregion PRIVATE MEMBERS

        #region CONSTRUCTOR
        //*********************************************************************
        //  Constructor: TDAmerAPI
        //
        /// <summary>
        /// The constructor for this class.
        /// </summary>
        //*********************************************************************
        static TDAmerAPI()
        {
            // Get the current domain
            AppDomain currentDomain = AppDomain.CurrentDomain;

            // Set an event handler for when the normal resolution of an 
            // assembly fails
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
        }
        #endregion CONSTRUCTOR

        #region PRIVATE METHODS
        //*********************************************************************
        //  Event Handler: LoadFromSameFolder
        //
        /// <summary>
        /// Handle firing of the 'AssemblyResolve' event.
        /// </summary>
        /// 
        /// <param name="sender">
        /// Object raising this event.
        /// </param>
        /// 
        /// <param name="args">
        /// Event arguments.
        /// </param>
        /// 
        /// <remarks>
        /// This should take place when Zorro is trying to determine which
        /// plug-ins to display in its window.
        /// </remarks>
        //*********************************************************************
        static Assembly 
            LoadFromSameFolder
            (
            object sender, 
            ResolveEventArgs args
            )
        {
            // Get the folder path that we are executing in. Should be plug-ins
            // directory
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Create a name for this assembly
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");

            // If we are already in the plug-ins folder exit
            if (File.Exists(assemblyPath) == false) return null;

            // Not in the plug-ins folder, load this assembly
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            // Return reference to the assembly
            return assembly;
        }
        #endregion PRIVATE METHODS

        #region ZORRO INTERFACE METHODS
        //*********************************************************************
        //  Method: DLLMain
        //
        /// <summary>
        /// Method used to define the entry point for this DLL.
        /// </summary>
        /// 
        /// <param name="hModule">
        /// Name of the module we're running under.
        /// </param>
        /// 
        /// <param name="ul_reason_for_call">
        /// May contain reason for call.
        /// </param>
        /// 
        /// <param name="lpReserved">
        /// Not used.
        /// </param>
        //*********************************************************************
        [DllExport("DLLMain", CallingConvention = CallingConvention.StdCall)]
        public static void 
            DLLMain
            (
            IntPtr hModule, 
            UInt32 ul_reason_for_call, 
            IntPtr lpReserved
            )
        {
            // Should be left blank. Nothing to do.
        }

        //*********************************************************************
        //  Method: BrokerOpen
        //
        /// <summary>
        /// Called at startup for all broker DLLs found in the Plugin folder. 
        /// Retrieves the name of the broker, and sets up two callback 
        /// functions. 
        /// </summary>
        /// 
        /// <param name="name">
        /// Output, char[32] array to be filled with the name of the broker
        /// </param>
        /// 
        /// <param name="fpError">
        /// Input, pointer to a int BrokerError(char* message) function, to be
        /// called for printing broker messages (usually error messages) in 
        /// Zorro's message window.
        /// </param>
        /// 
        /// <param name="fpProgress">
        /// Input, pointer to a int BrokerProgress(int progress=0) function, to
        /// be called repeatedly when broker operations take longer than a 
        /// second.
        /// </param>
        /// 
        /// <returns>
        /// Broker interface version number (PLUGIN_VERSION).
        /// </returns>
        /// 
        /// <remarks>
        /// NOTE: Using DLLExport library so Zorro will see the approriate
        /// functions in this bridge DLL.
        /// </remarks>
        //*********************************************************************
        [DllExport("BrokerOpen", CallingConvention = CallingConvention.Cdecl)]
        public static int 
            BrokerOpen
            (
            StringBuilder name, 
            BrokerErrorDelegate fpError, 
            BrokerProgressDelegate fpProgress
            )
        {
            // Give the name of this plugin back to Zorro
            name.Clear();
            name.Append("TD Ameritrade");

            // Provide a pointer to the broker message function
            BrokerError = fpError;

            // Provide a pointer to the progress function for operations that
            // take longer than 1.0 secs
            BrokerProgress = fpProgress;

            // Return the current plug-in version
            return PLUGIN_VERSION;
        }

        //*********************************************************************
        //  Method: BrokerLogin
        //
        /// <summary>
        /// Login or logout to the broker's API server; called in [Trade] mode
        /// or for downloading historical price data. If the connection to the 
        /// server was lost, i.e. due to to Internet problems or server weekend
        /// maintenance, Zorro calls this function repeatedly in regular inter-
        /// vals until it is logged in again. Make sure that the function inter-
        /// nally detects the login state and returns safely when the user was 
        /// still logged in. 
        /// </summary>
        /// 
        /// <param name="user">
        /// TDA initial refresh token..
        /// </param>
        /// 
        /// <param name="pwd">
        /// Left blank for TDAmeritrade
        /// </param>
        /// 
        /// <param name="type">
        /// Input, account type for logging in; either "Real" or "Demo".
        /// </param>
        /// 
        /// <param name="account">
        /// Optional output, char[1024] array to be filled with all user's 
        /// account numbers as subsequent zero-terminated strings, ending with 
        /// "" for the last string. Only the first account number is used by 
        /// Zorro.
        /// </param>
        /// 
        /// <returns>
        /// Login state: 1 when logged in, 0 otherwise.
        /// </returns>
        //*********************************************************************
        [DllExport("BrokerLogin", CallingConvention = CallingConvention.Cdecl)]
        public static int 
            BrokerLogin
            (
            string user, 
            string pwd, 
            string type, 
            StringBuilder accounts
            )
        {
            isConnected = false;

            // Information about this plug-in and disclaimer
            BrokerError("TD Ameritrade - Zorro Plug-In");
            BrokerError("Copyright © 2020 by Clyde W. Ford.");
            BrokerError("All Rights Reserved.");
            BrokerError($"Version {Broker.GetVersionNumber()}");
            BrokerError("Free for non-commercial use only.");
            BrokerError("USE AT YOU OWN RISK.\r\n\r\n");

            // Set the working directory first
            Broker.WORKING_DIR = Directory.GetCurrentDirectory();

            // Set the database connection string
            DataAccess.SetConnString(Broker.WORKING_DIR + "/Data/tda.db");

            // Was SETTINGS initialization successful?
            if (Broker.InitSettings(user))
            {
                // YES: Log the entrance to this method
                LogHelper.Log($"\r\n\r\n{Resx.GetString("BROKER_LOGIN")}...");

                // Get the operating mode of the plug-in
                opMode = type.ToLower() == "demo" ? OpMode.Demo : OpMode.Real;

                LogHelper.Log($"Plug-in is in {opMode} mode.");

                // Create the database tables. If creation not success return
                // a failure code to stop the Zorro trading engine
                if (!Broker.CreateDbTables())
                {
                    // Log the error
                    LogHelper.Log(LogLevel.Critical, $"{Resx.GetString("TABLES_NOT_CREATED")}");
                    return 0;
                }

                // Call the broker login method
                isConnected = Broker.Login();

                // Add the TD Ameritrade account to the accounts as the one and only
                // account
                accounts.Append(Broker.settings.TdaAccountNum + "\0" + "");

                // Log exit from method
                LogHelper.Log($"{Resx.GetString("EXITING")} {Resx.GetString("BROKER_LOGIN")}...");
                LogHelper.Log($"    {Resx.GetString("RETURN_CODE")} = {isConnected}");
            }
            else
            {
                // NO: Settings file could not be initialized.
                LogHelper.Log(LogLevel.Error, "Settings file (tda.json) could not be initialized.");
                LogHelper.Log(LogLevel.Error, "This file must be present for the plug-in to operate.");
                LogHelper.Log(LogLevel.Error, "Please verify the existence of this file, then try again.");
            }

            // Return connected code
            return isConnected ? 1 : 0;
        }

        //*********************************************************************
        //  Method: BrokerTime
        //
        /// <summary>
        /// Get status of connection and server time.
        /// </summary>
        /// 
        /// <param name="pTimeGMT">
        /// Output containg current server time as UTC (GMT+0) with no daylight
        /// savings time.
        /// 
        /// NOTE: pTimeGMT is in OLE date/time format, which is a double. In
        /// C# use .ToOADate() to convert from normal system date time format.
        /// 
        /// PTimeGMT is retrieved from the headers sent back with a response
        /// from the TD Ameritrade servers.
        /// </param>
        /// 
        /// <returns>
        /// 0, if connection to the server lost. Zorro will call for new login
        ///    through BrokerLogin.
        ///    
        /// 1, if connection to server is ok, but the market is closed or trade
        ///     orders are not being accepted at this time.
        ///     
        /// 2, if connection is ok and the market is open for trading at least 
        ///     one of the subscribed assets.
        /// </returns>
        //*********************************************************************
        [DllExport("BrokerTime", CallingConvention = CallingConvention.Cdecl)]
        public static int 
            BrokerTime
            (
            IntPtr pTimeGMT
            )
        {
            // Method member
            double? serverTime;

            // Log entering this method
            LogHelper.Log($"\r\n{Resx.GetString("BROKER_TIME")}...");

            // Call the implementation method and return the result
            int cnx = Broker.Time(out serverTime);

            // Log the time, if not null
            if (serverTime != null)
                LogHelper.Log("   Server Time: " + DateTime.FromOADate((double)serverTime).ToString("s"));

            // Is the server time present?
            if (serverTime != null)
            {
                // YES: Execute the following in an unsafe code block
                unsafe
                {
                    // Cast the IntPtr as a double pointer
                    Double* dPtr = (Double*)pTimeGMT;

                    // If double pointer is not NULL, store server time at
                    // the address pointed to
                    if ((int)dPtr > 0) *dPtr = (double)serverTime;
                }
            }

            // Log exit
            switch(cnx)
            {
                case 0:
                    // Log connection lost to Zorro window
                    LogHelper.Log(LogLevel.Warning, $"{Resx.GetString("CONN_LOST")}.");
                    break;

                case 1:
                    // Log only to file
                    LogHelper.Log(LogLevel.Caution, $"{Resx.GetString("CONN_OK_MARKET_CLOSED")}");
                    break;

                case 2:
                    // Log only to file
                    LogHelper.Log(LogLevel.Info, $"{Resx.GetString("CONN_OK_MARKET_OPEN")}");
                    break;

                default:
                    break;
            }

            // Return the connection status
            return cnx;
        }

        //*********************************************************************
        //  Method: BrokerAsset
        //
        /// <summary>
        /// Subscribes to an asset, or returns information about an asset. 
        /// </summary>
        /// 
        /// <param name="asset">
        /// INPUT: name of the asset, i.e. "EUR/USD" or "NAS100". TD Ameri-
        /// trade accepts "/" in a currency pair.
        /// </param>
        /// 
        /// <param name="pPrice">
        /// OUTPUT (OPTIONAL): Current ask price of the asset, or NULL for 
        /// subscribing the asset. An asset must be subscribed before any 
        /// information about it can be retrieved.
        /// </param>
        /// 
        /// <param name="pSpread">
        /// OUTPUT (OPTIONAL): Current difference of ask and bid price of the 
        /// asset.
        /// </param>
        /// 
        /// <param name="pVolume">
        /// OUTPUT (OPTIONAL): Current trade volume of the asset, or 0 when the 
        /// volume is unavailable, as for currencies, indexes, or CFDs.
        /// </param>
        /// 
        /// <param name="pPip">
        /// </param>
        /// 
        /// <param name="pPipCost">
        /// OUTPUT (OPTIONAL): Cost of 1 PIP profit or loss per lot, in units of 
        /// the account currency. If not directly supported, calculate it as 
        /// decribed under asset list.
        /// </param>
        /// 
        /// <param name="pLotAmount">
        /// OUTPUT (OPTIONAL): Minimum order size, i.e. number of contracts for 
        /// 1 lot of the asset. For currencies it's usually 10000 with mini 
        /// accounts and 1000 with micro accounts. For CFDs it's usually 1, but 
        /// can also be a fraction of a contract, e.g. 0.1.
        /// </param>
        /// 
        /// <param name="pMargin">
        /// OUTPUT (OPTIONAL): Required margin for buying 1 lot of the asset in 
        /// units of the account currency. Determines the leverage. If not 
        /// directly supported, calculate it as decribed under asset list.
        /// </param>
        /// 
        /// <param name="pRolloverLong">
        /// OUTPUT (OPTIONAL): Rollover fee for long trades, i.e. interest that 
        /// is added to or subtracted from the account for holding positions 
        /// overnight. The returned value is the daily fee per 10,000 contracts
        /// for currencies, and per contract for all other assets, in units of 
        /// the account currency.
        /// </param>
        /// 
        /// <param name="pRolloverShort">
        /// OUTPUT (OPTIONAL): Rollover fee for short trades.
        /// </param>
        /// 
        /// <returns> 
        /// 1, when the asset is available and the returned data is valid, 
        /// 0, otherwise. 
        /// 
        /// An asset that returns 0 after subscription will not be traded.
        /// </returns>
        /// 
        /// <remarks>
        /// Pointer and calling conventions to C++ have values defined as an 
        /// array of doubles. Place desired values into element [0] of the
        /// related array.
        /// </remarks>
        //*********************************************************************
        [DllExport("BrokerAsset", CallingConvention = CallingConvention.Cdecl)]
        public static int 
            BrokerAsset
            (
            string asset, 
            IntPtr pPrice, 
            IntPtr pSpread,
            IntPtr pVolume, 
            IntPtr pPip, 
            IntPtr pPipCost, 
            IntPtr pLotAmount,
            IntPtr pMarginCost, 
            IntPtr pRolloverLong, 
            IntPtr pRolloverShort
            )
        {
            // Method member
            bool subscribe = false;

            // Log entering this method
            LogHelper.Log($"\r\n{Resx.GetString("BROKER_ASSET")}...");

            // Test for connection to TD Ameritrade API
            if (!isConnected) return 0;

            // Call Broker.Asset implementation method to get the asset
            TDAsset tdAsset = Broker.Asset(asset);

            // Was asset found and data obtained?
            if (tdAsset != null)
            {
                // Process whether this asset needs to be subscribed to, or not.
                // True => asset needs to be subscribed to, False => asset already
                // subscribed to
                subscribe = Broker.Subscription(tdAsset);


                //*************************************************************
                //
                // NOTE: The pointers this method is called with from Zorro are
                // not always consistent. Sometimes a pointer is inexplicably 0.
                // Therefore, using managed double arrays is not reliable and
                // we're kickin' it old-school through unmanaged code to get
                // the C++ double pointers, test them for 0, and point them to
                // the correct value if they are non-zero.
                //
                //*************************************************************
                unsafe
                {
                    // Subscribing to the asset?
                    if (!subscribe)
                    {
                        // NO: Get a pointer to the Price
                        Double* dPtr = (Double*)pPrice;
                        if ((int)dPtr > 0) *dPtr = tdAsset.Price;

                        // Spread
                        dPtr = (Double*)pSpread;
                        if ((int)dPtr > 0) *dPtr = tdAsset.Spread;

                        // Volume
                        dPtr = (Double*)pVolume;
                        if ((int)dPtr > 0) *dPtr = tdAsset.Volume;

                        // Pip
                        dPtr = (Double*)pPip;
                        if ((int)dPtr > 0) *dPtr = tdAsset.Pip;

                        // Pip Cost
                        dPtr = (Double*)pPipCost;
                        if ((int)dPtr > 0) *dPtr = tdAsset.PipCost;

                        // Lot Amount
                        dPtr = (Double*)pLotAmount;
                        if ((int)dPtr > 0) *dPtr = tdAsset.LotAmount;

                        // Margin Cost
                        dPtr = (Double*)pMarginCost;
                        if ((int)dPtr > 0) *dPtr = tdAsset.MarginCost;

                        // For TD Ameritrade Rollover options only apply to
                        // Forex currency pairs, which are currently not
                        // supported,
                        if (tdAsset.AssetType == "CURRENCY")
                        {
                            // Rollover (Long)
                            dPtr = (Double*)pRolloverLong;
                            if ((int)dPtr > 0) *dPtr = tdAsset.forex.RolloverLong;

                            // Rollover (Short)
                            dPtr = (Double*)pRolloverShort;
                            if ((int)dPtr > 0) *dPtr = tdAsset.forex.RolloverShort;
                        }
                        else
                        {
                            // Rollover (Long)
                            dPtr = (Double*)pRolloverLong;
                            if ((int)dPtr > 0) *dPtr = 0.0;

                            // Rollover (Short)
                            dPtr = (Double*)pRolloverShort;
                            if ((int)dPtr > 0) *dPtr = 0.0;
                        }
                    }
                }

                // Log subscription or retrieval for this asset
                LogHelper.Log(subscribe ?
                    $"    {Resx.GetString("SUCCESS")}: {Resx.GetString("SUBSCRIBING_TO")} {tdAsset.Symbol}" :
                    $"    {Resx.GetString("SUCCESS")}: {Resx.GetString("RETRIEVING_INFO_FOR")} {tdAsset.Symbol}");

                // Return that this asset has valid data, and can be traded
                return 1;
            }

            // PAST HERE: Asset not found, and will not be traded.
            LogHelper.Log(LogLevel.Error, $"    {asset} {Resx.GetString("NOT_FOUND")}.");
            return 0;
        }


        //*********************************************************************
        //  Method: BrokerHistory2 (optional)
        //
        /// <summary>
        /// Returns the price history of an asset.
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
        /// LookBack period before starting a strategy.
        /// </param>
        /// 
        /// <param name="nTicks">
        /// INPUT: The maximum number of ticks to be filled; guaranteed to be 
        /// 300 or less.
        /// </param>
        /// 
        /// <param name="data">
        /// OUTPUT: A pointer to ann array of up to 300 TICK structs (defined in 
        /// include\trading.h) filled in with the ask price history. The ticks
        /// array is filled in reverse order from tEnd or until either the tick
        /// time reaches tStart or the num of ticks reaches nTicks, whichever 
        /// happens first. The most recent tick, closest to tEnd, is at the 
        /// start of the array. In the case of T1 data, or when only a single
        /// price is available, all prices in the TICK struct can be set to the
        /// same value.
        /// </param>
        /// 
        /// <returns> 
        /// (1) Number of ticks returned; OR,
        /// 
        /// (2) 0 when no ticks could be returned,
        /// 
        /// e.g. when the server was offline, the asset was not subscribed, or
        /// price history was not available for the given date/time.
        /// </returns>
        //*********************************************************************
        [DllExport("BrokerHistory2", CallingConvention = CallingConvention.Cdecl)]
        public static int
            BrokerHistory2
            (
            string asset,
            double tStart,
            double tEnd,
            int nTickMinutes,
            int nTicks,
            IntPtr data
            )
        {
            // Entry to method indicator
            LogHelper.Log($"\r\n{Resx.GetString("BROKER_HISTORY2")} ({asset})...");

            // Reasons for returning a zero code
            if (!isConnected || String.IsNullOrEmpty(asset) || nTicks == 0) return 0;

            // Call implementation function to get an array of Tick structures
            Tick[] outTicks = Broker.History(asset, tStart, tEnd, nTickMinutes, nTicks);

            // Do we have any ticks?
            if (outTicks.Length == 0)
            {
                // NO: Log no ticks returned
                LogHelper.Log(LogLevel.Warning, $"    {Resx.GetString("NO_TICKS_RETURNED")} {asset}.");

                // Return failure code
                return 0;
            }

            //*****************************************************************
            //
            // NOTE: The layout of managed and unmanaged arrays isn't the same.
            // Zorro passes a pointer to a T6 array, and we need to use an
            // unmanaged pointer to get the original C++ T6 array pointer. We
            // will loda a T6 array of structures at that address.
            //
            //*****************************************************************
            unsafe
            {
                // Zorro passes us a pointer to a an array of T6 structs. We
                // have to make that an IntPtr in C#, but create a new pointer
                // and cast it as a Tick pointer, so it will advance by the
                // size of one tick structure through adding '1'.
                Tick* ptr = (Tick*)data;

                // If output pointer is NOT NULL store historical data to it
                if ((int)ptr != 0)
                {
                    // Iterate through the array of ticks returned from the Browser
                    // History implementation and layout a sequential T6 structure
                    // array beginning at the address pointed to by the IntPtr this 
                    // method is called with.
                    for (int i = 0; i < outTicks.Length; ++i)
                    {
                        // Advance pointer by size of a Tick structure
                        *(ptr + i) =

                        // Store a new Tick structure at this location
                        new Tick(
                            outTicks[i].time,
                            outTicks[i].fOpen,
                            outTicks[i].fClose,
                            outTicks[i].fHigh,
                            outTicks[i].fLow,
                            outTicks[i].fVal,
                            outTicks[i].fVol
                        );
                    }
                }
            }

            // Log # ticks returned
            LogHelper.Log($"    {Resx.GetString("TICKS_RETURNED")} {asset} ({outTicks.Length}).");

            // Return the count of the number of ticks
            return outTicks.Length;
        }

        //*********************************************************************
        //  Method: BrokerAccount
        //
        /// <summary>
        /// Returns the current account status.
        /// </summary>
        /// 
        /// <param name="account">
        /// Input, new account number or NULL for using the current account.
        /// </param>
        /// 
        /// <param name="pdBalance">
        /// Optional output, current balance on the account.
        /// </param>
        /// 
        /// <param name="pdTradeVal">
        /// Optional output, current value of all open trades; the difference 
        /// between account equity and balance. If not available, it can be 
        /// replaced by a Zorro estimate with the SET_PATCH broker command.
        /// </param>
        /// 
        /// <param name="pdMarginVal">
        /// Optional output, current total margin bound by all open trades.
        /// </param>
        /// 
        /// <returns> 
        /// 1 when the account is available and the returned data is valid, 
        /// 0 when a wrong account was given or the account was not found.
        /// </returns>
        /// 
        /// <remarks>
        /// This version of the plug-in DOES NOT SUPPORT multiple TD Ameritrade
        /// accounts.
        /// </remarks>
        //*********************************************************************
        [DllExport("BrokerAccount", CallingConvention = CallingConvention.Cdecl)]
        public static int 
            BrokerAccount
            (
            string account, 
            IntPtr pdBalance, 
            IntPtr pdTradeVal, 
            IntPtr pdMarginVal
            )
        {
            // Gate-keeping test
            if (!isConnected) return 0;

            // Log entry to this method
            LogHelper.Log($"\r\n{Resx.GetString("BROKER_ACCOUNT")}...");

            // If account is NULL, use the current TD Ameritrade account
            if (string.IsNullOrEmpty(account)) account = Broker.settings.TdaAccountNum;

            // Call back-end implementation
            AccountBalance acctBalance = Broker.Account(account);

            // If account balance object is NULL, return invalid acct code
            if (acctBalance == null)
            {
                // Log account not found
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ACCOUNT_NUM")} {account} {Resx.GetString("NOT_FOUND")}.");

                // Return with failure
                return 0;
            }

            // Past here, we have a valid account balance object, give Zorro
            // the information desired
            unsafe
            {
                // NO: Get a pointer to the Account Balance
                Double* dPtr = (Double*)pdBalance;
                if ((int)dPtr > 0) *dPtr = acctBalance.Balance;

                // Trade Value
                dPtr = (Double*)pdTradeVal;
                if ((int)dPtr > 0) *dPtr = acctBalance.TradeValue;

                // Margin Value
                dPtr = (Double*)pdMarginVal;
                if ((int)dPtr > 0) *dPtr = acctBalance.MarginValue;
            }

            // Log account found
            LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ACCOUNT_NUM")} {account} {Resx.GetString("FOUND")}.");

            // Return valid data code
            return 1;
        }

        //*********************************************************************
        //  Method: BrokerBuy2
        //
        /// <summary>
        /// Enters a long or short trade at market or limit. Also used for NFA 
        /// compliant accounts to close a trade by opening a position in the
        /// opposite direction. 
        /// </summary>
        /// 
        /// <param name="asset">
        /// INPUT: name of the asset, i.e. "EUR/USD" or "MSFT". 
        /// </param>
        /// 
        /// <param name="nAmount">
        /// INPUT: Number of contracts, positive for a long trade and negative 
        /// for a short trade. The number of contracts is the number of lots 
        /// multiplied with the LotAmount. If LotAmount is < 1 (e.g. for a CFD 
        /// with 0.1 contracts lost size), the number of lots is given here 
        /// instead of the number of contracts.
        /// </param>
        /// 
        /// <param name="dStopDist">
        /// INPUT: 'Safety net' stop loss distance to the opening price, or 0 
        /// for no stop. This is not the real stop loss, which is handled by 
        /// the trade engine. Placing the stop is not mandatory. NFA compliant
        /// orders do not support a stop loss; in that case dStopDist is 0 for
        /// opening a trade and -1 for closing a trade by opening a position in
        /// the opposite direction.
        /// 
        /// NOTE: This plug-in does not support FOREX, FUTURE, or FUTURE OPTION
        /// trading, so it should not see a trade with a stop distance of -1.
        /// Also, as of 5/15/2020, STOP and STOP LIMIT orders could no be
        /// placed via the plug-in. TD Ameritrade has been notified and may
        /// have a resolution. 
        /// </param>
        /// 
        /// <param name="dLimit">
        /// INPUT (optional): Ask/bid price for limit orders, set up by the
        /// OrderLimit Zorro function, or 0 for market orders.
        /// 
        /// NOTE: This plug-in does support LIMIT orders but not STOP LIMIT
        /// orders, see above comment under dStopDist.
        /// </param>
        /// 
        /// <param name="pPrice">
        /// OUTPUT (optional): The current asset price at which the trade was 
        /// opened.
        /// </param>
        /// 
        /// <param name="pFill">
        /// OUTPUT (optional): The fill amount, needed for order types other
        /// than FOK (fill or kill) orders.
        /// </param>
        /// 
        /// <returns>
        /// Trade or order ID number = when either the order was filled, or a
        /// GTC order was successfully placed. If the broker does not support 
        /// individual trades or ID numbers, a unique 6-digit number, f.i. from
        /// a counter, can serve as a trade ID.
        /// 
        /// 0 = when a FOK or IOC order was not filled within 30 seconds 
        /// (adjustable with the SET_WAIT command). Unfilled FOK / IOC orders 
        /// must be cancelled.
        /// 
        /// -1 = when the trade identifier is a UUID that must be retrieved
        /// with the GET_UUID command.
        /// 
        /// -2 = when the broker API did not respond at all within 30 seconds. 
        /// The order must then be cancelled. Zorro displays a "possible orphan" 
        /// warning.
        /// 
        /// -3 = when the trade was entered, but got no ID yet. The ID is then
        /// taken over from the next BrokerBuy2 call that returns a ID > 0.
        /// </returns>
        /// 
        /// <remarks>
        /// NOTE: NFA compliance refers to the role that the National Futures
        /// Association (NFO) plays in regulating the futures market and is
        /// usually meant to refer to rule 2-43b, implemented in 2009 by NFA,
        /// which states that "forex deal members (FDM) and retail foreign 
        /// exchange dealers (RFED) cannot allow clients to hedage and must
        /// offset posititons on a first-in-first-out (FIFO) basis.
        /// 
        /// The effect of this rule is:
        /// 
        /// (1) Banning of price adjustments to executed customer orders, 
        /// except to resolve a complaint that is in the customer's favor.
        /// 
        /// (2) Increasesed transparency for customers and brings forex trading
        /// practices in line with those of the equities and futures markets.
        /// 
        /// NFA is a self-regulatory body of the FOREX industry, and their
        /// compliance rules apply ONLY TO trading of currency exchange trades.
        /// </remarks>
        //*********************************************************************
        [DllExport("BrokerBuy2", CallingConvention = CallingConvention.Cdecl)]
        public static int 
            BrokerBuy2
            (
            string asset,
            int nAmount,
            double dStopDist,
            double dLimit,
            IntPtr pPrice,
            IntPtr pFill
            )
        {
            // Method members
            string orderType = (nAmount > 0) ? Resx.GetString("BUYING") : Resx.GetString("SELLING");
            string marketType = (dLimit > 0) ? Resx.GetString("LIMIT") + " " + Resx.GetString("PRICE_OF") + " " + dLimit.ToString("N4") : Resx.GetString("MARKET");

            // Log entry to this method
            LogHelper.Log($"\r\n{Resx.GetString("BROKER_BUY2")}...");

            // Gate-keeping function for connection
            if (!isConnected) return 0;

            // If stop distance is -1, then trying to close an NFA trade but
            // this plug-in does not support NFA trades, so return a code as
            // if trade was canceled.
            if (dStopDist == -1) return 0;

            // Log the trade
            LogHelper.Log($"    {Resx.GetString("TRADE")}: {orderType} {Math.Abs(nAmount)} {Resx.GetString("SHARES_OF")} {asset} {Resx.GetString("AT")} {marketType}.");

            // Call back-end implementation and get a Trade object in return
            Trade trade = Broker.Buy(asset, nAmount, dStopDist, dLimit);

            // Is the Zorro Trade Id > 0?
            if (trade.ZorroTradeId > 0)
            {
                // YES: Good trade. Do we have a price also?
                if (trade.Price > 0.00)
                {
                    unsafe
                    {
                        // YES: We have a price. Define a double pointer to the
                        // output parameter passed to us by Zorro
                        Double* dPtr = (Double*)pPrice;

                        // If this passed parameter is NOT NULL, store price
                        // data to it
                        if ((int)dPtr != 0) *dPtr = trade.Price;
                    }                    
                }

                // Log a success
                LogHelper.Log($"    {Resx.GetString("SUCCESS")}");

                // Return the Zorro Trade Id, since the TD Trade Id will be
                // a long value .gt. what Zorro can handle
                return trade.ZorroTradeId;
            }
            else
            {
                // Is a combo OPTION trade in process?
                if (Broker.ComboLegs > 0)
                {
                    // YES: Waiting for more trade orders to make up a
                    // combination order
                    return 0;
                }
                else
                {
                    // Log a failure
                    LogHelper.Log(LogLevel.Warning, $"    {Resx.GetString("FAILURE")}");

                    // NO: Is TD Trade Id .lte. zero?
                    if (trade.TDTradeId <= 0)
                    {
                        // YES: Return it as an int
                        return (int)trade.TDTradeId;
                    }
                    else
                    {
                        // NO: Strange return. Should never get here but if
                        // we do, then return 0, for no trade
                        return 0;
                    }
                }
            }
        }

        //*********************************************************************
        //  Method: BrokerStop
        //
        /// <summary>
        /// Optional method to adjust the stop loss limit of an open trade if
        /// it had an original stop (dStopDist != 0 in BrokerBuy2) and if it's 
        /// not an NFA account. If this function is not provided, the original 
        /// stop loss is never updated. Only for non-NFA compliant accounts. 
        /// </summary>
        /// 
        /// <param name="nTradeID">
        /// INPUT: The trade ID number as returned by BrokerBuy2
        /// </param>
        /// 
        /// <param name="dStop">
        /// INPUT: The new stop loss price. Must be by a sufficient distance 
        /// (broker dependent) below the current price for a long trade, and 
        /// above the current price for a short trade.
        /// </param>
        /// 
        /// <returns>
        /// 0, if no open trade with this ID found, otherwise nonzero.
        /// </returns>
        /// 
        /// <remarks>
        /// NOTE: This method is not implemented by the plug-in becasue STOP
        /// and STOP LIMIT orders are not implented. See BrokerSell2 for more
        /// information on STOP LOSS order implementation
        /// </remarks>
        //*********************************************************************
        /*
        [DllExport("BrokerStop", CallingConvention = CallingConvention.Cdecl)]
        public static int 
            BrokerStop
            (
            int nTradeID, 
            double dStop
            )
        {
            // Log entry to this method
            BrokerError("BrokerStop...");

            // Gate-keeper to make sure a connection to TD Ameritrade exists
            if (!connected) return 0;
        }
        */

        //*********************************************************************
        //  Method: BrokerSell2
        //
        /// <summary>
        /// Closes a trade either completely or partially, at MARKET; only for 
        /// non-NFA compliant accounts. Partial closing is seen primarily in
        /// FOREX trading, and currently it appears that TD Ameritrade does not
        /// suuport partial closing as it is NFA compliant.
        /// </summary>
        /// 
        /// <param name="nTradeID">
        /// INPUT: The Zorro trade ID as returned by BrokerBuy2.
        /// </param>
        /// 
        /// <param name="nAmount">
        /// INPUT: The number of contracts or lots to be closed, positive for a 
        /// long trade and negative for a short trade (see BrokerBuy2). If less
        /// than the original size of the trade, the trade is partially closed.
        /// NOTE: This plug-in WILL NOT partially close a trade. It's all or
        /// nothing.
        /// </param>
        /// 
        /// <param name="dLimit">
        /// INPUT (optional): The ask/bid price for a LIMIT order, or 0, if
        /// closing at the market.
        /// </param>
        /// 
        /// <param name="pClose">
        /// OUTPUT (optional): The close price of the trade.
        /// </param>
        /// 
        /// <param name="pCost">
        /// OUTPUT (optional): The total rollover (swap) fee of the trade.
        /// </param>
        /// 
        /// <param name="pProfit">
        /// OUTPUT (optional): The total profit/loss of the trade in account 
        /// currency units.
        /// </param>
        /// 
        /// <param name="pFill">
        /// OUTPUT (optional): The fill amount of the trade.
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
        /// This Zorro method is being implemented by the plug-in to provide
        /// information about the closing, cost, and profit of a trade. Anytime
        /// a trade is closed with a call to this method, the trade will be
        /// completely closed because TD Ameritrade does not support partially
        /// closed trades.
        /// </remarks>
        //*********************************************************************
        [DllExport("BrokerSell2", CallingConvention = CallingConvention.Cdecl)]
        public static int 
            BrokerSell2
            (
            int nTradeID, 
            int nAmount,
            double dLimit,
            IntPtr pClose,
            IntPtr pCost,
            IntPtr pProfit,
            IntPtr pFilled
            )
        {
            // Log entry to this method
            LogHelper.Log($"\r\n{Resx.GetString("BROKER_SELL")}");

            // Gate-keeping for this method
            if (!isConnected) return 0;

            // Get a trade object from attempting to close the trade
            Trade trade = Broker.Sell(nTradeID, nAmount, dLimit);

            // If the trade to close was made, fill-in the OUTPUT variables
            if (trade.StatusCode > 0) {

                // SUCCESS: Manipulate pointers and store double values in an
                // unmanaged code block
                unsafe
                {
                    // Define a double pointer to the closing price
                    Double* dPtr = (Double*)pClose;
                    if ((int) dPtr != 0) *dPtr = trade.Close;

                    // Cost
                    dPtr = (Double*)pCost;
                    if ((int)dPtr != 0) *dPtr = trade.Cost;

                    // Profit
                    dPtr = (Double*)pProfit;
                    if ((int)dPtr != 0) *dPtr = trade.Profit;

                    // Filled
                    dPtr = (Double*)pFilled;
                    if ((int)dPtr != 0) *dPtr = trade.Filled;
                }

                LogHelper.Log($"{Resx.GetString("SUCCESS")}: {Math.Abs(nAmount)} shares of {trade.Asset}.");
            }
            else
            {
                // FAILURE
                LogHelper.Log(LogLevel.Error, $"{Resx.GetString("FAILURE")}: {Math.Abs(nAmount)} shares of {trade.Asset}.");
            }

            // Return the status code
            return trade.StatusCode;
        }

        //*********************************************************************
        //  Method: BrokerTrade
        //
        /// <summary>
        /// Returns the status of an open or recently closed trade. 
        /// </summary>
        /// 
        /// <param name="nTradeID">
        /// INPUT: The Zorro trade ID number as returned by BrokerBuy2.
        /// </param>
        /// 
        /// <param name="pOpen">
        /// OUTPUT (optional): The enter price of the asset including spread. Not
        /// available for NFA compliant accounts.
        /// </param>
        /// 
        /// <param name="pClose">
        /// OUTPUT (optional): The current price of the asset including spread.
        /// </param>
        /// 
        /// <param name="pCost">
        /// OUTPUT (optional): The total rollover fee (swap fee) of the trade so 
        /// far. Not available for NFA compliant accounts.
        /// </param>
        /// 
        /// <param name="pProfit">
        /// OUTPUT (optional): The profit or loss of the trade so far. Not 
        /// available for NFA compliant accounts. Possible wrong values due to 
        /// API bugs can be replaced by Zorro estimates with the SET_PATCH 
        /// broker command
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
        [DllExport("BrokerTrade", CallingConvention = CallingConvention.Cdecl)]
        public static int 
            BrokerTrade
            (
            int nTradeID, 
            IntPtr pOpen, 
            IntPtr pClose, 
            IntPtr pCost, 
            IntPtr pProfit
            )
        {
            // Log entry to this method
            LogHelper.Log($"\r\n{Resx.GetString("BROKER_TRADE")}...");

            // Gate-keeper for this method
            if (!isConnected) return 0;

            // Call the back-end implementation
            Trade trade = Broker.BrokerTrade(nTradeID);

            // Is the status code non-zero?
            if (trade.StatusCode != 0)
            {
                // YES: Fill-in the pointers
                unsafe
                {
                    // Define a double pointer to the closing price
                    Double* dPtr = (Double*)pClose;
                    if ((int)dPtr != 0) *dPtr = 0.0;

                    // Open
                    dPtr = (Double*)pOpen;
                    if ((int)dPtr != 0) *dPtr = trade.Price;

                    // Cost
                    dPtr = (Double*)pCost;
                    if ((int)dPtr != 0) *dPtr = 0.0;

                    // Profit
                    dPtr = (Double*)pProfit;
                    if ((int)dPtr != 0) *dPtr = 0.0;
                }
            }

            // Log status of this trade
            LogHelper.Log($"    {Resx.GetString("TRADE")}: {trade.Asset} {trade.Status.ToUpper()}");

            // Return to Zorro with the trade status code
            return trade.StatusCode;
        }

        //*********************************************************************
        //  Method: BrokerCommand
        //
        /// <summary>
        ///  Optional function. Sets various plugin parameters or returns asset 
        ///  specific extra data. This function is not mandatory, as it is not 
        ///  used by Zorro's trade engine; but it can be called in scripts
        ///  through the brokerCommand function for special purposes.
        /// </summary>
        /// 
        /// <param name="nCommand">
        /// INPUT: The command from the brokerCommand list.
        /// </param>
        /// 
        /// <param name="dwParameter">
        /// INPUT: The parameter or data to the command.
        /// </param>
        /// 
        /// <returns>
        /// 0 when the command is not supported by this broker plugin, otherwise 
        /// the data to be retrieved.
        /// </returns>
        /// 
        /// <remarks>
        /// </remarks>
        //*********************************************************************
        [DllExport("BrokerCommand", CallingConvention = CallingConvention.Cdecl)]
        public static double
            BrokerCommand
            (
            int nCommand, 
            IntPtr dwParameter
            )
        {
            // Log the command
            // Uncomment the following two lines to debug Zorro's brokerCommand
            string param = ((int)dwParameter) > 1 ? "\"" + Marshal.PtrToStringAnsi(dwParameter) + "\"" : dwParameter.ToString();
            LogHelper.Log($"brokerCommand({(ZorroCommand)nCommand}, {param})");

            // Gate-keeper for this method
            if (!isConnected) return 0;

            // Switch to see if this plug-in is handlinging any of the commands
            switch((ZorroCommand)nCommand)
            {
                case ZorroCommand.SET_SYMBOL:
                    // Call the SET_SYMBOL command implementation
                    return Broker.SetSymbol(nCommand, Marshal.PtrToStringAnsi(dwParameter));

                case ZorroCommand.GET_POSITION:
                    // Call the backend command to get the position on the asset,
                    // after converting from a pointer to a string
                    return Broker.GetPosition(nCommand, Marshal.PtrToStringAnsi(dwParameter));

                case ZorroCommand.GET_OPTIONS:
                    // Get the option chain of the underlying asset
                    return Broker.GetOptions(nCommand, dwParameter);

                case ZorroCommand.GET_UNDERLYING:
                    return Broker.GetUnderlying(nCommand, (int)dwParameter);

                case ZorroCommand.SET_COMBO_LEGS:
                    return Broker.SetComboLegs(nCommand, (int)dwParameter);

                case ZorroCommand.SHOW_RESOURCE_STRING:
                    return Broker.ShowResourceString(nCommand, Marshal.PtrToStringAnsi(dwParameter));

                case ZorroCommand.REVIEW_LICENSE:
                    return Broker.ReviewLicense(nCommand, 0);

                case ZorroCommand.GET_TEST_ASSETS:
                    // Get the asset list
                    return Broker.GetTestAssets(nCommand, dwParameter);

                case ZorroCommand.GET_ASSET_LIST:
                    // Get the asset list
                    return Broker.GetAssetList(nCommand, dwParameter);

                case ZorroCommand.SET_SELL_SELL_SHORT:
                    // Set what to do if selling more shares than owned
                    return Broker.SetSellSellShort(nCommand, (int)dwParameter);

                case ZorroCommand.SET_VERBOSITY:
                    // Set the verbosity level  
                    verbosityLevel = (Verbosity)(int)(dwParameter);
                    if (verbosityLevel > Verbosity.Intermediate)
                        BrokerError($"Plug-in verbosity level now set to {verbosityLevel.ToString().ToUpper()}");
                    return 1;

                case ZorroCommand.SET_TESTMODE:
                    TestMode = (int)dwParameter == 1;
                    return 1;
                /*
                case ZorroCommand.DO_EXERCISE:
                    // Exercise the give number of contracts of an option
                    return Broker.DoExercise(nCommand, Marshal.PtrToStringAnsi(dwParameter));
                */

                case ZorroCommand.GET_COMPLIANCE:
                    // Full NFA compliance
                    return 15;

                case ZorroCommand.SET_LANGUAGE:
                    // Set the language resource to use for globalization
                    return Broker.SetLanguage(nCommand, Marshal.PtrToStringAnsi(dwParameter));

                default:
                    break;
            }

            // Command is not handled by this plug-in
            return 0;
        }
        #endregion ZORRO INTERFACE METHODS
    }
}
