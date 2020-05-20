//*****************************************************************************
// File: OrderSubmission.cs
//
// Author: Clyde W. Ford
//
// Date: May 8, 2020
//
// Description: Store order information and create order JSON.
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
using TDAmeritradeZorro.Classes.TDA;
using TDAmeritradeZorro.Utilities;

namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Class: OrderSubmission
    //
    /// <summary>
    /// A class that encapsulates the information needed to suubit an order to
    /// // the TD Ameritrade API.
    /// </summary>
    //*************************************************************************
    public class OrderSubmission
    {
        #region CLASS MEMBERS
        //*********************************************************************
        //  Member: comboOrders
        //
        /// <summary>
        /// A list of combo OPTION orders waiting to be executed as a single
        /// order.
        /// </summary>
        //*********************************************************************
        private static List<OrderSubmission> comboOrders;

        //*********************************************************************
        //  Member: stockOrderTpl
        //
        /// <summary>
        /// The template for a stock order (BUY or SELL) with no STOP
        /// </summary>
        //*********************************************************************
        private static string stockOrderTpl =
            "{{" +
                // MARKET or LIMIT
                @"""orderType"": ""{0}""," +

                // NORMAL, AM, PM, SEAMLESS
                @"""session"": ""{1}""," +

                // DAY, GOOD_TILL_CANCEL, FILL_OR_KILL
                @"""duration"": ""{2}""," +

                // ORDER STRATEGY
                @"""orderStrategyType"": ""{3}""," +

                // The ORDER LEG collection (only one for plugin)
                @"""orderLegCollection"": [" +
                    "{{" +
                        // BUY, SELL, BUY_TO_COVER, SELL_SHORT, BUY_TO_OPEN,
                        // BUY_TO_CLOSE, SELL_TO_OPEN, SELL_TO_CLOSE, EXCHANGE
                        @"""instruction"": ""{4}""," +

                        // Order type: EQUITY, QPTION, INDEX, MUTUAL_FUND,
                        // CASH_EQUIVALENT, FIXED_INCOME, CURRENCY
                        @"""orderLegType"": ""{7}""," +

                        // Order quantity,
                        @"""quantity"": {5}," +

                        // The Instrument
                        @"""instrument"": {{" +

                            // The asset symbol
                            @"""symbol"": ""{6}""," +

                            // Asset type
                            @"""assetType"": ""{7}""" +
                        "}}" +
                    "}}" +
                 "]" +
            "}}";

        //*********************************************************************
        //  Member: stockOrderTpl
        //
        /// <summary>
        /// The template for a stock order (BUY or SELL) with no STOP
        /// </summary>
        //*********************************************************************
        private static string optionsOrderTpl =
            "{{" +
                // ORDER STRATEGY: MARKET or LIMIT
                @"""orderStrategyType"": ""{3}""," +

                // Order Strategy Type: MARKET or LIMIT
                @"""orderType"": ""{0}""," +

                // The ORDER LEG collection (only one for plugin)
                @"""orderLegCollection"": [" +
                    "{{" +
                        // The Instrument
                        @"""instrument"": {{" +

                            // Asset type
                            @"""assetType"": ""{7}""," +

                            // The asset symbol
                            @"""symbol"": ""{6}""" +

                        "}}," +

                        // BUY, SELL, BUY_TO_COVER, SELL_SHORT, BUY_TO_OPEN,
                        // BUY_TO_CLOSE, SELL_TO_OPEN, SELL_TO_CLOSE, EXCHANGE
                        @"""instruction"": ""{4}""," +

                        // Order quantity,
                        @"""quantity"": {5}" +

                    "}}" +

                 "]," +

                // Complex order strategy type
                //@"""complexOrderStrategyType"": ""NONE""," +

                // DAY, GOOD_TILL_CANCEL, FILL_OR_KILL
                @"""duration"": ""{2}""," +

                // NORMAL, AM, PM, SEAMLESS
                @"""session"": ""{1}""" +

            "}}";

        //*********************************************************************
        //  Member: orderLegTpl
        //
        /// <summary>
        /// The template for a single order leg, used for combo OPTION orders.
        /// </summary>
        //*********************************************************************
        private static string orderLegTpl =
        "{{" +
            // The Instrument
            @"""instrument"": {{" +

                // Asset type
                @"""assetType"": ""{0}""," +

                // The asset symbol
                @"""symbol"": ""{1}""" +

            "}}," +

            // BUY, SELL, BUY_TO_COVER, SELL_SHORT, BUY_TO_OPEN,
            // BUY_TO_CLOSE, SELL_TO_OPEN, SELL_TO_CLOSE, EXCHANGE
            @"""instruction"": ""{2}""," +

            // Order quantity,
            @"""quantity"": {3}" +

        "}}";

        //*********************************************************************
        //  Member: comboOrderTpl
        //
        /// <summary>
        /// The template for a combination OPTION order
        /// </summary>
        //*********************************************************************
        private static string comboOrderTpl =
        "{{" +
            
            // ORDER STRATEGY
            @"""orderStrategyType"": ""{3}""," +

            // MARKET or LIMIT
            @"""orderType"": ""{0}""," +

            // The ORDER LEG collection (only one for plugin)
            @"""orderLegCollection"": [" +

                // The combo order leg Json
                "{4}" +

                "]," +

            // Complex strategy type
            //@"""complexOrderStrategyType"": ""CUSTOM""," +

            // DAY, GOOD_TILL_CANCEL, FILL_OR_KILL
            @"""duration"": ""{2}""," +

            // NORMAL, AM, PM, SEAMLESS
            @"""session"": ""{1}""" +

        "}}";

        //*********************************************************************
        //  Member: fundOrderTpl
        //
        /// <summary>
        /// The template for a stock order (BUY or SELL) with no STOP
        /// </summary>
        //*********************************************************************
        private static string fundOrderTpl =
        "{{" +
            // MARKET or LIMIT
            @"""orderType"": ""{0}""," +

            // NORMAL, AM, PM, SEAMLESS
            @"""session"": ""{1}""," +

            // DAY, GOOD_TILL_CANCEL, FILL_OR_KILL
            @"""duration"": ""{2}""," +

            // Complex strategy type
            @"""complexOrderStrategyType"": ""NONE""," +

            // Price
            @"""price"": {5}," +

            // ORDER STRATEGY
            @"""orderStrategyType"": ""{3}""," +

            // The ORDER LEG collection (only one for plugin)
            @"""orderLegCollection"": [" +
                "{{" +
                    // BUY, SELL, BUY_TO_COVER, SELL_SHORT, BUY_TO_OPEN,
                    // BUY_TO_CLOSE, SELL_TO_OPEN, SELL_TO_CLOSE, EXCHANGE
                    @"""instruction"": ""{4}""," +

                    // Order type: EQUITY, OPTION, INDEX, MUTUAL_FUND,
                    // CASH_EQUIVALENT, FIXED_INCOME, CURRENCY
                    @"""orderLegType"": ""{7}""," +

                    // Order quantity,
                    @"""quantity"": 150," +

                    // Order quantity type,
                    @"""quantityType"": ""DOLLARS""," +

                    // The Instrument
                    @"""instrument"": {{" +

                        // The asset symbol
                        @"""symbol"": ""{6}""," +

                        // Asset type
                        @"""assetType"": ""{7}""," +

                        // Fund type ('NOT_APPLICABLE' or 'OPEN_END_NON_TAXABLE' or 'OPEN_END_TAXABLE' or 'NO_LOAD_NON_TAXABLE' or 'NO_LOAD_TAXABLE')
                        @"""type"": ""NOT_APPLICABLE""" +
                    "}}" +
                "}}" +
                "]" +
        "}}";

        //*********************************************************************
        //  Member: LimitPropsTpl
        //
        /// <summary>
        /// The template for the additional JSON properties that need to be
        /// added in the case of a LIMIT order.
        /// </summary>
        //*********************************************************************
        private static string
            LimitPropsTpl =

            // Need to include complexOrderStrategyType
            @"""complexOrderStrategyType"": ""NONE""," +

            // Limit price
            @"""price"": ""{0}"",";

        //*********************************************************************
        //  Member: trailingStopTpl
        //
        /// <summary>
        /// Template for a TRAILING STOP order at LIMIT or MARKET
        /// </summary>
        //*********************************************************************
        private static string trailingStopTpl =
        "{{" +
          @"""orderType"": ""{0}""," + 
          @"""session"": ""NORMAL""," + 
          @"""duration"": ""DAY""," + 
          @"""orderStrategyType"": ""TRIGGER""," + 
          @"""orderLegCollection"": [" + 
            "{{" + 
              @"""instruction"": ""{1}""," + 
              @"""quantity"": {2}," + 
              @"""instrument"": {{" + 
                @"""symbol"": ""{3}""," + 
                @"""assetType"": ""{4}""" + 
              "}}" + 
            "}}" + 
          "]," + 
          @"""childOrderStrategies"": [" + 
            "{{" + 
              @"""orderType"": ""TRAILING_STOP""," + 
              @"""stopPriceOffset"": {5}," + 
              @"""stopPriceLinkType"": ""VALUE""," + 
              @"""stopPriceLinkBasis"": ""BID""," + 
              @"""stopType"": ""STANDARD""," + 
              @"""session"": ""NORMAL""," + 
              @"""duration"": ""DAY""," + 
              @"""orderStrategyType"": ""SINGLE""," + 
              @"""orderLegCollection"": [" + 
                "{{" + 
                  @"""instruction"": ""{6}""," + 
                  @"""quantity"": {2}," + 
                  @"""instrument"": {{" + 
                    @"""symbol"": ""{3}""," + 
                    @"""assetType"": ""{4}""" + 
                  "}}" + 
                "}}" + 
              "]" + 
            "}}" + 
          "]" + 
        "}}";

        private bool toClose = false;
        #endregion CLASS MEMBERS

        #region CLASS PROPERTIES
        //*********************************************************************
        //  Property: asset
        //
        /// <summary>
        /// The underlying TD asset for this order
        /// </summary>
        //*********************************************************************
        public TDAsset asset { get; set; }

        //*********************************************************************
        //  Property: Symbol
        //
        /// <summary>
        /// The asset symbol
        /// </summary>
        //*********************************************************************
        public string Symbol { get; set; }

        //*********************************************************************
        //  Property: OrderType
        //
        /// <summary>
        /// The type of order (MARKET, LIMIT, STOP). STOP currently not
        /// implemented by the plug-in.
        /// </summary>
        //*********************************************************************
        public string OrderType { get; set; }

        //*********************************************************************
        //  Property: Instruction
        //
        /// <summary>
        /// The order instruction (BUY or SELL)
        /// </summary>
        //*********************************************************************
        public string Instruction { get; set; }

        //*********************************************************************
        //  Property: Price
        //
        /// <summary>
        /// The price for 1 lot of the asset
        /// </summary>
        //*********************************************************************
        public double Price { get; set; }

        //*********************************************************************
        //  Property: Amount
        //
        /// <summary>
        /// The asset symbol (i.e. AAPL or MSFT)
        /// </summary>
        //*********************************************************************
        public int Amount { get; set; }

        //*********************************************************************
        //  Property: Limit
        //
        /// <summary>
        /// The limit price for this order
        /// </summary>
        //*********************************************************************
        public double Limit { get; set; }

        //*********************************************************************
        //  Property: StopDist
        //
        /// <summary>
        /// The stop price offset (not currently implemented)
        /// </summary>
        //*********************************************************************
        public double StopDist { get; set; }

        //*********************************************************************
        //  Property: Session
        //
        /// <summary>
        /// Session type (NORMAL, AM, PM)
        /// </summary>
        //*********************************************************************
        public string Session { get; set; }

        #endregion CLASS PROPERTIES

        #region CLASS CONSTRUCTOR
        //*********************************************************************
        //  Constructor: OrderSubmission
        //
        /// <summary>
        /// Create a class object and set default values for its properties
        /// </summary>
        //*********************************************************************
        public OrderSubmission
            (
            TDAsset asset,
            int Amount,
            double Limit,
            double StopDist,
            bool close = false
            )
        {
            // Save the paramaters passed to us
            this.asset = asset;
            this.Amount = Amount;
            this.StopDist = StopDist;
            this.Limit = Limit;
            toClose = close;

        }
        #endregion CLASS CONSTRUCTOR

        #region PUBLIC METHODS FOR COMBO ORDERS
        //*********************************************************************
        //  Method: SaveInComboList
        //
        /// <summary>
        /// Save the current order in the combo order list.
        /// </summary>
        /// 
        /// <returns>
        /// True if successful, false if not.
        /// </returns>        
        //*********************************************************************
        public bool
            SaveInComboList
            ()

        {
            try
            {
                // Does the combo order list need to be initialized
                if (comboOrders == null) comboOrders = new List<OrderSubmission>();

                // Add this order to the combo order list
                comboOrders.Add(this);

                // Return with success
                return true;
            }
            catch(Exception e)
            {
                // Log error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("SAVING_COMBO_OPTION_ORDER")}. " + e.Message);

                // Return with failure
                return false;
            }
        }

        //*********************************************************************
        //  Method: GetCombinedOrderJson
        //
        /// <summary>
        /// Get the JSON payload for a combined OPTION order
        /// </summary>
        /// 
        /// <returns>
        /// A Json string representing the combo options order.
        /// </returns>
        //*********************************************************************
        public string
            GetCombinedOrderJson
            ()
        {
            // Method members
            string comboLegJson = string.Empty;
            string json = string.Empty;
            string marketType = null;
            double limit = -1;
            bool failure = false;

            // Create the order leg collection by interating through the save
            // orders
            foreach (OrderSubmission order in comboOrders)
            {
                // Test for limit? No LIMIT orders processed as combo leg
                // orders by TD Ameritrade
                if (order.Limit > 0)
                {
                    // Log the error
                    json = $"{Resx.GetString("ERROR").ToUpper()}: {Resx.GetString("NO_LIMIT_IN_COMBE")}";

                    // Combo order has failde
                    failure = true;

                    // Exit loop
                    break;
                }

                // Test for assets other than OPITONS
                if (order.asset.AssetType != "OPT")
                {
                    // Log the error
                    json = $"{Resx.GetString("ERROR").ToUpper()}: {order.asset.AssetType} {Resx.GetString("NOT_IN_COMBO")}";

                    // Combo order has failde
                    failure = true;

                    // Exit loop
                    break;
                }

                // NO: Get the TD Ameritrade asset type
                string AssetType = Helper.ZorroToTDAssetType(order.asset.AssetType);

                // Get the Instruction
                string instruction = order.Amount > 0 ? "BUY" : "SELL";
                if (AssetType == "OPTION" && !toClose) instruction += "_TO_OPEN";
                if (AssetType == "OPTION" && toClose) instruction += "_TO_CLOSE";

                // Add the order leg to the combo leg Json
                comboLegJson += string.Format(orderLegTpl,

                    // Asset type (0)                   
                    AssetType,

                    // Option symbol (1)
                    AssetType == "OPTION" ? order.GetOptionSymbol() : order.asset.TickerSymbol,

                    // Instruction: Buy or sell to open (2)
                    instruction,

                    // Amount of order (3)
                    Math.Abs(order.Amount)
                    ) + ",";
            }

            // Has order failed validation?
            if (!failure)
            {
                // NO: Remove the last comma from the combo Json
                comboLegJson = comboLegJson.Trim(',');

                // Clear out the combo order list
                comboOrders.Clear();

                // Create the main combo order
                json = string.Format(
                    // Template
                    comboOrderTpl,

                    // Order Type (LIMIT or MARKET) (0)
                    "MARKET",

                    // Session (1)
                    "NORMAL",

                    // Duration (2)
                    "DAY",

                    // Order strategy type (3)
                    "SINGLE",

                    // Combo leg Json (4)
                    comboLegJson
                    );


                // If this is as LIMIT combo OPTION order, add the LIMIT price
                if (Limit > 0)
                    json = json.Replace("\"orderStrategy", $"\"price\": {Limit}," + "\"orderStrategy");
            }

            // Return the combo Json for order
            return json;
        }
        #endregion PUBLIC METHODS FOR COMBO OPTIONS

        #region PUBLIC METHODS
        //*********************************************************************
        //  Method: ConvertToJson
        //
        /// <summary>
        /// Convert the class object to a JSON string based on the type of
        /// order.
        /// </summary>
        //*********************************************************************
        public string 
            ConvertToJson
            ()
        {
            // Method members
            string json = Broker.jsonNull;

            // Switch based on the order type
            switch(asset.AssetType)
            {
                case "ETF":
                case "EQUITY":
                case "STK":
                    json = GetStockOrderJson();
                    break;

                case "OPTION":
                case "OPT":
                    json = GetOptionsOrderJson();
                    break;

                case "MUTUAL_FUND":
                    json = GetFundOrderJson();
                    break;

                default:
                    break;
            }

            // Return the JSON string
            return json;
        }

        //*********************************************************************
        //  Method: GetStockOrderJson
        //
        /// <summary>
        /// Get the JSON string for a stock order at MARKET or LIMIT.
        /// </summary>
        /// 
        /// <returns>
        /// JSON order payload for STK or ETF asset.
        /// </returns>
        //*********************************************************************
        public string
            GetStockOrderJson
            ()
        {
            //*****************************************************************
            // NOTE: The TD Ameritrade REST API appears to allow STOP MARKET or
            // STOP LIMIT orders. But as of 05/15/20 no STOP MARKET/STOP LIMIT 
            // orders were accepted. TD Ameritrade has been notified and may 
            // resolve this matter. But as of this release of the plug-in STOP 
            // MARKET and STOP LIMIT orders will not be placed. Zorro notes on 
            // the BrokerBuy2 command state that the STOP DISTANCE included as a 
            // parameter for this command, "is not the real stop loss, which is 
            // handled by the trade engine. Placing the stop is not mandatory." 
            // Consequently, STOP MARKET and STOP LIMIT orders will not be placed 
            // by this version of the plug-in.
            //
            // Uncomment the following six lines, if STOP MARKET OR STOP LIMIT
            // orders can be placed with the API at some future time.
            //*****************************************************************
            // Is this a STOP order?
            //if (StopDist != 0)
            //{
                // YES: Process the STOP order and return the json string
                //return GetStopOrderJson();
            //}

            // Get the TD Ameritrade asset type
            string AssetType = Helper.ZorroToTDAssetType(asset.AssetType);

            // Get the basic JSON string
            string json = string.Format(stockOrderTpl,

                // Order Type (0)
                Limit > 0 ? "LIMIT" : "MARKET",

                // Session (1) (TD Ameritrade API only supports NORMAL now)
                "NORMAL",

                // Duration of order (2) (TD Ameritrade only suports DAY now)
                "DAY",

                // Order strategy (3) (always SINGLE, TRIGGER orders
                // not supported by the plug-in)
                "SINGLE",

                // Instruction (4) (BUY or SELL)
                Amount > 0 ? "BUY" : "SELL",

                // Order amount (5). ABS to cover long and short trades
                Math.Abs(Amount),

                // Asset ticker symbol symbol (6)
                asset.TickerSymbol,

                // The asset type (7). Convert from Zorro to TD Ameritrade.
                Helper.ZorroToTDAssetType(asset.AssetType)
                );

            // If this is a LIMIT order, add LIMIT infomation to the JSON string
            if (Limit > 0)
                json = json.Replace("\"orderStrategy", string.Format(LimitPropsTpl, Limit) + "\"orderStrategy");

            // Return the JSON string
            return json;
        }

        //*********************************************************************
        //  Method: GetOptionsOrderJson
        //
        /// <summary>
        /// Get the JSON string for a Options order at MARKET or LIMIT.
        /// </summary>
        //*********************************************************************
        public string
            GetOptionsOrderJson
            ()
        {
            // Get the TD Ameritrade asset type
            string AssetType = Helper.ZorroToTDAssetType(asset.AssetType);

            // Get the basic JSON string
            string json = string.Format(optionsOrderTpl,

                // Order Type (0)
                Limit > 0 ? "LIMIT" : "MARKET",

                // Session (1) (TD Ameritrade API only supports NORMAL now)
                "NORMAL",

                // Duration of order (2) (TD Ameritrade only suports DAY now)
                "DAY",

                // Order strategy (3) (always SINGLE, TRIGGER orders
                // not supported by the plug-in)
                "SINGLE",

                // Instruction (4) (BUY or SELL)
                Amount > 0 ? "BUY_TO_OPEN" : "SELL_TO_OPEN",

                // Order amount (5). ABS to cover long and short trades
                Math.Abs(Amount),

                // Asset ticker symbol symbol (6)
                GetOptionSymbol(),

                // The asset type (7). Convert from Zorro to TD Ameritrade.
                Helper.ZorroToTDAssetType(asset.AssetType)
                );

            // If this is a LIMIT order, add LIMIT infomation to the JSON string
            if (Limit > 0)
                json = json.Replace("\"orderStrategy", string.Format(LimitPropsTpl, Limit) + "\"orderStrategy");

            // Return the JSON string
            return json;
        }

        //*********************************************************************
        //  Method: GetStopOrderJson
        //
        /// <summary>
        /// Return an order with a TRAILING STOP at either a LIMIT or MARKET
        /// price.
        /// </summary>
        /// 
        /// <returns>
        /// String with json for the order.
        /// </returns>
        //*********************************************************************
        private string
            GetStopOrderJson
            ()
        {
            // Json for the orrder:
            string json = string.Format(
                // Trail STOP LIMIT emplate
                trailingStopTpl,

                // Order type (LIMIT or MARKET) (0)
                (Limit > 0) ? "LIMIT" : "MARKET",

                // Primary instruction (1)
                (Amount > 0) ? "BUY" : "SELL",

                // Quantity (2)
                Math.Abs(Amount),

                // Symbol (3)
                asset.TickerSymbol,

                // Asset type (4)
                Helper.ZorroToTDAssetType(asset.AssetType),

                // Stop price distance (5)
                Math.Abs(StopDist),

                // Counter instruction (6)
                (Amount > 0) ? "SELL" : "BUY"
                );

            // If this is a LIMIT order, add LIMIT price to the JSON string
            if (Limit > 0)
                json = json.ReplaceFirst("\"duration", $"\"price\": {Limit}," + "\"duration");

            // Return the json string
            return json;
        }

        //*********************************************************************
        //  Method: GetFundOrderJson
        //
        /// <summary>
        /// Get the JSON string for a mutual fund order.
        /// </summary>
        //*********************************************************************
        public string
            GetFundOrderJson
            ()
        {
            // Get the basic JSON string
            string json = string.Format(fundOrderTpl,

                // Order Type (0)
                OrderType,

                // Session (1)
                Session,

                // Duration of order (2)
                "DAY",

                // Order strategy (3) (always SINGLE, TRIGGER orders
                // not supported by the plug-in)
                "SINGLE",

                // Instruction (4)
                Instruction,

                // Order amount (5). ABS to cover long and short trades
                Amount * Price,

                // Asset symbol (6)
                Symbol,

                // The asset type (7)
                asset.AssetType
                );

            // If this is a LIMIT order, add LIMIT infomation to the JSON string
            if (OrderType == "LIMIT")
                json = json.Replace("\"orderStrategy", string.Format(LimitPropsTpl, Limit) + "\"orderStrategy");

            // Return the JSON string
            return json;
        }

        //*********************************************************************
        //  Method: GetOptionSymbol
        //
        /// <summary>
        /// Get an option symbol.
        /// </summary>
        /// 
        /// <returns>
        /// The option symbol as a string.
        /// </returns>
        /// 
        /// <remarks>
        /// The option symbol is a string in the form of SSS_MMDDYYXNN, where:
        /// SSS = Asset ticker symbol
        /// MM = Month of expiration
        /// DD = Day of expiration
        /// YY = Year of expiration
        /// X = "C" for CALL, "P" for PUT
        /// NN = The strike price
        /// 
        /// NOTE: The original symbol will all of this information, and it 
        /// should have been encoded in the asset.
        /// </remarks>
        //*********************************************************************
        public string
            GetOptionSymbol
            ()
        {
            // Method member
            string optSymbol = string.Empty;

            // Place the ticker symbol on the string
            optSymbol = asset.TickerSymbol + "_";

            // Format the date as a six-character string
            string strDate = asset.option.ExpirationDate.ToString("MMddyy");

            // Create the symbol Stock Ticker + Exp. Date + PUT/CALL + Strike Price
            optSymbol += strDate + asset.option.PutCallType + asset.option.StrikePrice.ToString("N2");

            // Return the option symbol
            return optSymbol.Trim('0').Trim('.');
        }
        #endregion PUBLIC METHODS
    }
}
