//*****************************************************************************
// File: Enums.cs
//
// Author: Clyde W. Ford
//
// Date: May 2, 2020
//
// Description: Various enumerations used by the TD Ameritrade broker plug-in.
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
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Enum: ApiMethod
    //
    /// <summary>
    /// HTTP-based Web API for TD Ameritrade REST API.
    /// </summary>
    //*************************************************************************
    public enum ApiMethod
    {
        //*********************************************************************
        //                                                                    *
        //                  A U T H E N T I C A T I O N                       *
        //                                                                    *
        //*********************************************************************
        [Display(Name = "https://auth.tdameritrade.com/auth?response_type=code&redirect_uri=http%3A%2F%2F127.0.0.1&client_id={account_id}%40AMER.OAUTHAP",
            GroupName = "application/x-www-form-urlencoded", Prompt = "POST")]
        GetAuthCode,

        //*********************************************************************
        //                                                                    *
        //                    A C C E S S    T O K E N                        *
        //                                                                    *
        //*********************************************************************
        [Display(Name = "/oauth2/token", GroupName = "application/x-www-form-urlencoded", Prompt = "POST")]
        PostAccessToken,

        //*********************************************************************
        //                                                                    *
        //                          Q U O T E S                               *
        //                                                                    *
        //*********************************************************************
        [Display(Name = "/marketdata/{account_id}/quotes", GroupName = "application/json", Prompt = "GET")]
        GetQuote,

        [Display(Name = "/marketdata/quotes", GroupName = "application/json", Prompt = "GET")]
        GetQuotes,

        //*********************************************************************
        //                                                                    *
        //                    O P T I O N  C H A I N S                        *
        //                                                                    *
        //*********************************************************************
        [Display(Name = "/marketdata/chains", GroupName = "application/json", Prompt = "GET")]
        GetOptionChain,

        //*********************************************************************
        //                                                                    *
        //                         A C C O U N T S                            *
        //                                                                    *
        //*********************************************************************
        [Display(Name = "/accounts/{account_id}", GroupName = "application/json", Prompt = "GET")]
        GetAccount,

        [Display(Name = "/accounts", GroupName = "application/json", Prompt = "GET")]
        GetAccounts,

        //*********************************************************************
        //                                                                    *
        //                            O R D E R S                             *
        //                                                                    *
        //*********************************************************************
        [Display(Name = "/accounts/{account_id}/orders/{order_id}", GroupName = "application/json", Prompt = "DELETE")]
        CancelOrder,

        [Display(Name = "/accounts/{account_id}/orders/{order_id}", GroupName = "application/json", Prompt = "GET")]
        GetOrder,

        [Display(Name = "/accounts/{account_id}/orders", GroupName = "application/json", Prompt = "GET")]
        GetOrdersByPath,

        [Display(Name = "/orders", GroupName = "application/json", Prompt = "GET")]
        GetOrdersByQuery,

        [Display(Name = "/accounts/{account_id}/orders", GroupName = "application/json", Prompt = "POST")]
        PlaceOrder,

        [Display(Name = "/accounts/{account_id}/orders/{order_id}", GroupName = "application/json", Prompt = "PUT")]
        ReplaceOrder,

        //*********************************************************************
        //                                                                    *
        //                     S A V E D  O R D E R S                         *
        //                                                                    *
        //*********************************************************************
        [Display(Name = "/accounts/{account_id}/savedorders", GroupName = "application/json", Prompt = "POST")]
        CreateSavedOrder,

        [Display(Name = "/accounts/{account_id}/savedorders/{order_id}", GroupName = "application/json", Prompt = "DELETE")]
        DeleteSavedOrder,

        [Display(Name = "/accounts/{account_id}/savedorders/{order_id}", GroupName = "application/json", Prompt = "GET")]
        GetSavedOrder,

        [Display(Name = "/accounts/{account_id}/savedorders", GroupName = "application/json", Prompt = "GETT")]
        GetSavedOrderByPath,

        [Display(Name = "/accounts/{account_id}/savedorders/{order_id}", GroupName = "application/json", Prompt = "PUT")]
        ReplaceSavedOrder,

        //*********************************************************************
        //                                                                    *
        //                     T R A D I N G  H O U R S                        *
        //                                                                    *
        //*********************************************************************
        [Display(Name = "/marketdata/{account_id}/hours", GroupName = "application/json", Prompt = "GET")]
        GetMarketHours,

        //*********************************************************************
        //                                                                    *
        //                     P R I C E  H I S T O R Y                       *
        //                                                                    *
        //*********************************************************************
        [Display(Name = "/marketdata/{account_id}/pricehistory", GroupName = "application/json", Prompt = "GET")]
        GetPriceHistory,

    }

    //*************************************************************************
    //  Enum: OpMode
    //
    /// <summary>
    /// The operating mode of the plug-in (Demo or Real)
    /// </summary>
    //*************************************************************************
    public enum OpMode
    {
        Demo,
        Real
    }

    //*************************************************************************
    //  Enum: MarketTime
    //
    /// <summary>
    /// An enumeration of the three times during which trading can take place
    /// on the TD Ameritrade platform.
    /// </summary>
    //*************************************************************************
    public enum MarketTime
    {
        PreMarket,
        PostMarket,
        RegularMarket
    }

    //*************************************************************************
    //  Enum: ContractType
    //
    /// <summary>
    /// An enumeration of the various contract types (from Zorro functions.h)
    /// </summary>
    //*************************************************************************
    public enum ContractType
    {
        CALL = 1 << 0,
        PUT = 1 << 1,
        EUROPEAN = 1 << 2,
        BINARY = 1 << 3,
        FUTURE = 1 << 4,
        ONLYW3 = 1 << 5
    }

    public enum FormType
    {
        Auth,
        License
    }

    //*************************************************************************
    //  Enum: ZorroCommands
    //
    /// <summary>
    /// An enumeration of the Zorro commands taken from trading.h
    /// </summary>
    //*************************************************************************
    public enum ZorroCommand
    {
        // brokerCommand=last incoming tick time
        GET_TIME = 5,

        // Count of digits after decimal point 
        GET_DIGITS = 12,

        // Stop level in points.
        GET_STOPLEVEL = 14,

        // Market starting date (usually used for futures).
        GET_STARTING = 20,

        // Market expiration date (usually used for futures).
        GET_EXPIRATION = 21,

        // Trade is allowed for the symbol.
        GET_TRADEALLOWED = 22,

        // Minimum permitted amount of a lot.
        GET_MINLOT = 23,

        // Step for changing lots.
        GET_LOTSTEP = 24,

        // Maximum permitted amount of a lot.
        GET_MAXLOT = 25,

        // Initial margin requirements for 1 lot.
        GET_MARGININIT = 29,

        // Margin to maintain open positions calculated for 1 lot.
        GET_MARGINMAINTAIN = 30,

        // Hedged margin calculated for 1 lot.
        GET_MARGINHEDGED = 31,

        // Free margin required to open 1 lot for buying.
        GET_MARGINREQUIRED = 32,

        // Time zone returned by broker plugin
        GET_BROKERZONE = 40,

        GET_DELAY = 41,

        GET_WAIT = 42,

        // Max history ticks
        GET_MAXTICKS = 43,

        // Trade id: 0=number; 1=GUID; string pointer
        GET_IDTYPE = 44,

        // For setting up MaxRequests
        GET_MAXREQUESTS = 45,

        // Locking required. 
        GET_LOCK = 46,

        // Asset type
        GET_TYPE = 50,

        // NFA compliance
        GET_COMPLIANCE = 51,

        // Number of open trades
        GET_NTRADES = 52,

        // Open net lots per asset 
        GET_POSITION = 53,

        // Account number (string)
        GET_ACCOUNT = 54,

        // Order book
        GET_BOOK = 62,

        // Option chain
        GET_OPTIONS = 64,

        GET_FUTURES = 65,

        GET_FOP = 66,

        GET_UNDERLYING = 67,

        GET_SERVERSTATE = 68,

        GET_GREEKS = 69,

        // Work around broker API bugs
        SET_PATCH = 128,

        // Max adverse slippage for orders
        SET_SLIPPAGE = 129,

        // Magic number for trades
        SET_MAGIC = 130,

        // Order comment for trades
        SET_ORDERTEXT = 131,

        // Set asset symbol for subsequent commands
        SET_SYMBOL = 132,

        // Set option/future multiplier filter
        SET_MULTIPLIER = 133,

        // Set trading class filter
        SET_CLASS = 134,

        // Set limit price for entry limit orders
        SET_LIMIT = 135,

        // Set file name for direct history download
        SET_HISTORY = 136,

        // Declare the next n trades as an option combo
        SET_COMBO_LEGS = 137,

        // Activate plugin diagnostics output
        SET_DIAGNOSTICS = 138,

        // Set order amount size
        SET_AMOUNT = 139,

        // Type of prices returned by the API
        GET_PRICETYPE = 150,

        SET_PRICETYPE = 151,

        // Type of volume returned by the API
        GET_VOLTYPE = 152,

        SET_VOLTYPE = 153,

        // UUID of last trade 
        GET_UUID = 154,

        // UUID for next trade
        SET_UUID = 155,

        // Order Type: 0=FOK 1=IOC 2=GTC
        SET_ORDERTYPE = 157,

        SET_DELAY = 169,

        SET_WAIT = 170,

        SET_LOCK = 171,

        SET_HWND = 172,

        // Max history ticks
        SET_MAXTICKS = 173,

        // Plugin supplied callback function 
        GET_CALLBACK = 174,

        // Comment on the chart
        SET_COMMENT = 180,

        // Set broker/exchange for aggregators
        SET_BROKER = 181,

        // Send a string to a plot object
        PLOT_STRING = 188,

        PLOT_REMOVE = 260,

        PLOT_REMOVEALL = 261,

        // plot to the MT4 chart window
        PLOT_HLINE = 280,

        PLOT_TEXT = 281,

        PLOT_MOVE = 282,

        // Exercise an option
        DO_EXERCISE = 300,

        // Cancel a GTC order
        DO_CANCEL = 301,

        //*********************************************************************
        //                                                                    *
        //       U S E R - S U P P L I E D  B R O K E R  C O M M A N D S      *
        //                                                                    *
        //*********************************************************************

        // Get a resource string
        SHOW_RESOURCE_STRING = 4000,

        // Review the plug-in license
        REVIEW_LICENSE = 4002,

        // Get the asset list
        GET_ASSET_LIST = 4004,

        // Get the test assets
        GET_TEST_ASSETS = 4006,

        // Set the verbosity level
        SET_VERBOSITY = 4008,

        // Set the Test Mode (1 = enable; 0 = disable)
        SET_TESTMODE = 4010,

        // Set the SELL or SELL SHORT mode
        SET_SELL_SELL_SHORT = 4012
    }

    //*************************************************************************
    //  Enum: LogLevel
    //
    /// <summary>
    /// The message level of a diagnostic message.
    /// </summary>
    //*************************************************************************
    public enum LogLevel
    {
        Info = 1,
        Caution = 2,
        Warning = 4,
        Error = 8,
        Critical = 16
    }

    //*************************************************************************
    //  Enum: SellSellSHort
    //
    /// <summary>
    /// What to do if sell order is for more lots than owned.
    /// </summary>
    //*************************************************************************
    public enum SellSellShort
    {
        // Cancel the order
        Cancel = 1,

        // Adjust the amount and only SELL what is owned (default)
        Adjust = 2,

        // SELL what is owned, SELL SHORT what is not
        Short = 4
    }

    //*************************************************************************
    //  Enum: Verbosity
    //
    /// <summary>
    /// The verbosity level of the diagnostic messages from the plug-in.
    /// </summary>
    //*************************************************************************
    [Flags]
    public enum Verbosity
    {
        // Zorro verbosity level = 0
        Basic = 0x01 << 0,

        // Zorro verbosity level = 1
        Intermediate = 0x01 << 1,

        // Zorro verbosity level = 2
        Advanced = 0x01 << 2,

        // Zorro verbosity level = 3
        Extensive = 0x01 << 3,

        // Zorro verbosity level = 7
        Extreme = 0x01 << 4,

        // Timestamp on messages that go to log file
        TimeStamp = 0x01 << 5,

        // Line number or messages that go to log file
        LineNumbers = 0x01 << 6
    }
}