//*****************************************************************************
// File: Trade.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: A class that records trades initiated by the Zorro Engine and
// executed through the TD Ameritrade API.
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
using System.Web.Script.Serialization;
using TDAmeritradeZorro.Classes.DBLib;
using TDAmeritradeZorro.Utilities;

namespace TDAmeritradeZorro.Classes.TDA
{
    //*************************************************************************
    //  Class: Trades
    //
    /// <summary>
    /// This class implements the functionality required to same and retrieve
    /// trades that have been entered into TD Ameritrade.
    /// 
    /// NOTE: The reason this class is required is because TD Ameritrade
    /// returns a LONG trade number but Zorro only accepts an INT trade number.
    /// Therefore, we need to put in-place a means of assigning an INT trade
    /// number to any entered trade and keeping track of it. This is done thru
    /// a JSON data file that is synchronized with a list of entered trades.
    /// This class creates the file and the list and keeps the two in-sync.
    /// 
    /// Could possibly have used an embedded Sqlite database to accomplish this
    /// but wanted to reduce dependence on third-party DLL. Also, first tries
    /// with Sqlite showed it may not play well with DLLExport.
    /// </summary>
    //*************************************************************************
    public class Trade
    {
        #region CLASS MEMBERS
        //*********************************************************************
        //  Member: sqlBylZorroId
        //
        /// <summary>
        /// SQL statement for getting a trade record by its Zorro ID number.
        /// </summary>
        //*********************************************************************
        private static readonly string sqlBylZorroId =
            @"SELECT * FROM [Trade] WHERE ZorroTradeId = '{0}'";
        #endregion CLASS MEMBERS

        #region CLASS PROPERTIES
        //*********************************************************************
        //  Property: Id
        //
        /// <summary>
        /// Primary key, and auto-incremented id for database table record.
        /// </summary>
        //*********************************************************************
        [PrimaryKey]
        [AutoIncrement]
        [NotNull]
        public int Id { get; set; }

        //*********************************************************************
        //  Member: Asset
        //
        /// <summary>
        /// The Zorro symbol for this asset.
        /// </summary>
        //*********************************************************************
        [NotNull]
        public string Asset { get; set; }

        //*********************************************************************
        //  Member: AssetTYpe
        //
        /// <summary>
        /// The asset type (EQUITY, ETF, FOREX, etc)
        /// </summary>
        //*********************************************************************
        [NotNull]
        public string AssetType { get; set; }

        //*********************************************************************
        //  Member: OrderType
        //
        /// <summary>
        /// The order type (MARKET, STOP, STOP_LIMIT, etc.)
        /// </summary>
        //*********************************************************************
        [NotNull]
        public string OrderType { get; set; }

        //*********************************************************************
        //  Member: Instruction
        //
        /// <summary>
        /// Instruction (BUY or SELL)
        /// </summary>
        //*********************************************************************
        [NotNull]
        public string Instruction{ get; set; }

        //*********************************************************************
        //  Member: TDTradeId
        //
        /// <summary>
        /// The trade id returned from TD Ameritrade.
        /// </summary>
        //*********************************************************************
        [NotNull]
        public long TDTradeId { get; set; }

        //*********************************************************************
        //  Member: ZorroTradeId
        //
        /// <summary>
        /// The trade id assigned to Zorro.
        /// </summary>
        //*********************************************************************
        [NotNull]
        public int ZorroTradeId { get; set; }

        //*********************************************************************
        //  Member: Quantity
        //
        /// <summary>
        /// The amount traded.
        /// </summary>
        //*********************************************************************
        [NotNull]
        public int Quantity { get; set; }

        //*********************************************************************
        //  Member: Price
        //
        /// <summary>
        /// The price of the trade entered.
        /// </summary>
        //*********************************************************************
        public double Price { get; set; }

        //*********************************************************************
        //  Member: Open
        //
        /// <summary>
        /// The price of the asset, including spread, upon entering a trade. 
        /// Not available for NFA compliant accounts. Not availabe for NFA
        /// compliant accounts.
        /// </summary>
        //*********************************************************************
        public double Open { get; set; }

        //*********************************************************************
        //  Member: Close
        //
        /// <summary>
        /// The price of the asset at end of trading.
        /// </summary>
        //*********************************************************************
        public double Close { get; set; }

        //*********************************************************************
        //  Member: Cost
        //
        /// <summary>
        /// The total rollover (swap) fee for a trade. Not available for NFA
        /// compliant accounts.
        /// </summary>
        //*********************************************************************
        public double Cost { get; set; }

        //*********************************************************************
        //  Property: Profit
        //
        /// <summary>
        /// The profit or loss of the trade, so far. Not available for NFA
        /// complient accounts:
        /// </summary>
        //*********************************************************************
        public double Profit { get; set; }

        //*********************************************************************
        //  Member: Filled
        //
        /// <summary>
        /// The amount filled.
        /// </summary>
        //*********************************************************************
        public int Filled { get; set; }

        //*********************************************************************
        //  Member: Status
        //
        /// <summary>
        /// Status of the trade.
        /// </summary>
        //*********************************************************************
        public string Status { get; set; }

        //*********************************************************************
        //  Member: StatusCode
        //
        /// <summary>
        /// StatusCode of the trade.
        /// </summary>
        //*********************************************************************
        public int StatusCode { get; set; }

        //*********************************************************************
        //  Member: OrderJson
        //
        /// <summary>
        /// The json payload for the order
        /// </summary>
        //*********************************************************************
        [NotNull]
        public string OrderJson { get; set; }

        //*********************************************************************
        //  Member: Entered
        //
        /// <summary>
        /// The date the trade was entered.
        /// </summary>
        //*********************************************************************
        [NotNull]
        public DateTime Entered { get; set; }
        #endregion CLASS PROPERTIES

        public Trade()
        {
            // Initialize class properties (doubles)
            Price = Close = Cost = Profit = Open = 0.0;

            // Initialize class properties (ints)
            ZorroTradeId = Quantity = Filled = StatusCode = 0;

            // Initialize class property (long)
            TDTradeId = 0L;

            // Initialize class properties (strings)
            Asset = AssetType = OrderType = Instruction = Status = string.Empty;

            // Initializse the date the trade was entered
            Entered = DateTime.MinValue;
        }

        #region PUBLIC METHODS
        //*********************************************************************
        //  Method: Save
        //
        /// <summary>
        /// Save a current trade and return the Zorro trade id.
        /// </summary>
        /// 
        /// <returns>
        /// A Zorro trade id (integer)
        /// </returns>
        //*********************************************************************
        public int
            Save
            ()
        {
            try
            {
                // Get the next Zorro Trade Id to assign to this trade
                ZorroTradeId = GetZorroTradeId();

                // Does the record already exist?
                if (GetTradeByZorroId(ZorroTradeId) == null)
                {
                    // NO: Insert it.
                    DataAccess.Insert(this);
                }

                // Return success code (Zorro trade id)
                return ZorroTradeId;
            }
            catch(Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"{Resx.GetString("ERROR")}: " + e.Message);

                // Return error code
                return -1;
            }
        }
        #endregion PUBLIC METHODS

        #region PUBLIC STATIC METHODS
        //*********************************************************************
        //  Method: GetTradeByZorroId
        //
        /// <summary>
        /// Given a Zorro trade id, get the trade entered into TD Ameritrade.
        /// </summary>
        /// 
        /// <param name="ZorroTradeId">
        /// Zorro trade id for the trade.
        /// </param>
        /// 
        /// <returns>
        /// Trade object with the given Zorro trade id.
        /// </returns>
        //*********************************************************************
        public static Trade
            GetTradeByZorroId
            (
                int ZorroTradeId
            )
        {
            // Look-up the trade in the trades list from the Zorro Trade Id
            List<Trade> trades = DataAccess.GetRecordBySql<Trade>(
                string.Format(sqlBylZorroId, ZorroTradeId));

            // Are there more than one record
            if (trades.Count == 1)
            {
                // YES: Got the record. Return the first trade
                return trades[0];
            }
            else
            {
                // NO: Got no record. Return a dummy trade.
                return null;
            }
        }
        #endregion PUBLIC STATIC METHODS

        #region PRIVATE STATIC METHODS
        //*********************************************************************
        //  Method: GetZorroTradeId
        //
        /// <summary>
        /// Get the current autoincrment value.
        /// </summary>
        //*********************************************************************
        private static int
            GetZorroTradeId
            ()
        {
            int retId = -1;

            // Get the first entry, which is a dummy entry for holding the auto-
            // increment value
            TradeId tradeId = DataAccess.GetOrdinalRecord<TradeId>(1, "");

            // Was the next trade id record found?
            if (tradeId != null)
            {
                // YES: Bump the next trade Id number by one
                retId = tradeId.NextZorroId++;

                // Update the trade id record
                DataAccess.Update(tradeId);
            }
            else
            {
                // NO: Must be no records so, create a new record
                tradeId = new TradeId();

                // Give it a starting Zorro trade id
                retId = 1000;
                tradeId.NextZorroId = 1001;

                // Save it
                DataAccess.Insert(tradeId);
            }

            // Return a Zorro trade id.
            return retId;
        }

        //*********************************************************************
        //  Method: TDAm2ZorroSymbol
        //
        /// <summary>
        /// Convert a symbol from a TD Asset symbol to a Zorro (IB) symbol 
        /// </summary>
        /// 
        /// <param name="tdSymbol">
        /// TD Asset symbol
        /// </param>
        /// 
        /// <returns>
        /// Zorro (IB) asset symbol
        /// </returns>
        /// 
        /// <remarks>
        /// TD Ameritrade symbol is in form SSS_MMDDYYXNN.N, where:
        /// 
        /// SSS is the ticker symbol
        /// MMDDYY is the expiration date (if present)
        /// X is PUT or CALL (if present)
        /// NN.N is the strike price if present.
        /// 
        /// An equivalent Zorro (IB) symbol is:
        /// SSS-TTT-YYYYMMDD-NNN.N-X-EEEE, where:
        /// 
        /// SSS is the ticker symbol
        /// TTT is the asset type (STK, ETF, OPT)
        /// YYYYMMDD is the expiration date
        /// NN.N is the strike price
        /// X is PUT or CALL
        /// EEEEE is the exchange (SMART for TD Ameritrade)
        /// </remarks>
        //*********************************************************************
        public static string
            TDAm2ZorroSymbol
            (
            string tdSymbol
            )
        {
            // Method members
            DateTime date;
            double strikePrice;
            string zSymbol = string.Empty;

            // Is the just a ticker symbol, retur it
            if (!tdSymbol.Contains("_")) return tdSymbol;

            // Break apart the symbol
            string[] parts = tdSymbol.Split('_');

            // Has more than a ticker symbol with underscore it in. Only doing
            // STOCKS, ETFs, and OPTIONS currently. If it were a STOCK or ETF
            // it would have not gotten this far. Must be an OPT.
            zSymbol = parts[0] + "-" + "OPT-";

            // Get the date from the TD Ameritrade symbol
            if (DateTime.TryParse($"{parts[1].Substring(0, 2)}/{parts[1].Substring(2, 2)}/{parts[1].Substring(4, 2)}", out date))
            {
                // Successful parse, add date to symbol string
                zSymbol += date.ToString("yyyyMMdd") + "-";

                // Get strike price from TD Ameritrade symbol
                if (Double.TryParse(parts[1].Substring(7), out strikePrice))
                {
                    // Successful parse of strike price, add to Zorro symbol
                    zSymbol += strikePrice.ToString("N2").Trim('0') + "-";

                    // Add the PUT or CALL
                    zSymbol += parts[1].Substring(6, 1) + "-NYSE";
                }
                else
                {
                    zSymbol = string.Empty;
                }
            }
            else
            {
                zSymbol = string.Empty;
            }

            // Return the Zorro symbol
            return zSymbol;
        }

        //*********************************************************************
        //  Method: Zorro2TDAmSymbol
        //
        /// <summary>
        /// Convert a symbol from a Zorro (IB) to a TD Amenitrade symbol.
        /// </summary>
        /// 
        /// <param name="ZSymbol">
        /// The Zorro symbol
        /// </param>
        /// 
        /// <returns>
        /// TD Ameritrade symbol
        /// </returns>
        /// 
        /// <remarks>
        /// Zorro (IB) symbol is:
        /// SSS-TTT-YYYYMMDD-NNN.N-X-EEEE, where:
        /// 
        /// SSS is the ticker symbol
        /// TTT is the asset type (STK, ETF, OPT)
        /// YYYYMMDD is the expiration date
        /// NN.N is the strike price
        /// X is PUT or CALL
        /// EEEEE is the exchange (SMART for TD Ameritrade)
        ///
        /// Equivalent TD Ameritrade symbol is in form SSS_MMDDYYXNN.N, where:
        /// 
        /// SSS is the ticker symbol
        /// MMDDYY is the expiration date (if present)
        /// X is PUT or CALL (if present)
        /// NN.N is the strike price if present.
        /// 
        /// </remarks>
        //*********************************************************************
        public static string
            Zorro2TDAmSymbol
            (
            string zSymbol
            )
        {
            // Create a TD Asset object, which will automatically parse the
            // symbol
            TDAsset tdAsset = new TDAsset(zSymbol);
            OrderSubmission order = new OrderSubmission(tdAsset, 0, 0, 0, false);

            return order.GetOptionSymbol();
        }
        //*********************************************************************
        //  Method: JsonCopy
        //
        /// <summary>
        /// Make a non-referential clone of an trade
        /// </summary>
        /// 
        /// <param name="trade">
        /// Trade for which a clane is being made.
        /// </param>
        /// 
        /// <returns>
        /// An trade object which does not reference any other trade object,
        /// i.e. a clone.
        /// </returns>
        //*********************************************************************
        public static Trade
            JsonCopy
            (
            Trade trade
            )
        {
            // Create a new javascript serializer
            JavaScriptSerializer jss = new JavaScriptSerializer();

            // Serialize, then deserialize the original trade, to get the clone
            return jss.Deserialize<Trade>(jss.Serialize(trade));
        }
        #endregion PRIVATE STATIC METHODS
    }
}
