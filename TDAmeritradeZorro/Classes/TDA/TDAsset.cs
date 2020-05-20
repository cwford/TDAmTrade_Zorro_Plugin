//*****************************************************************************
// File: TDAsset.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: A class to hold information about a particular asset.
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
using System.Text.RegularExpressions;
using TDAmeritradeZorro.Utilities;

namespace TDAmeritradeZorro.Classes.TDA
{
    //*************************************************************************
    // Class: TDAsset
    //
    /// <summary>
    /// Class to encapsulates all of the properties required by Zorro's Broker
    /// Asset method.
    /// </summary>
    //*************************************************************************
    public class TDAsset
    {
        #region CLASS MEMBERS
        //*********************************************************************
        //  Member: CallCodes
        //
        /// <summary>
        /// Delivery month codes for OPTION CALLS
        /// </summary>
        //*********************************************************************
        private string CallCodes = "ABCDEFGHIJKL";

        //*********************************************************************
        //  Member: PutCodes
        //
        /// <summary>
        /// Delivery month codes for OPTION PUTS
        /// </summary>
        //*********************************************************************
        private string PutCodes = "MNOPQRSTUVWX";

        //*********************************************************************
        //  Member: FutureCodes
        //
        /// <summary>
        /// Delivery month codes for FUTURES
        /// </summary>
        //*********************************************************************
        private string FutureCodes = "FGHJKMNQUVXZ";

        //*********************************************************************
        //  Member: AllowableAssetTypes
        //
        /// <summary>
        /// List of allowable asset types.
        /// </summary>
        //*********************************************************************
        private List<string> AllowableAssetTypes = new List<string>
        {
            "STK", "OPT", "FUT", "FUTX", "IND", "FOP", "WAR", "CASH", "CFD",
            "STKCFD", "FUND", "EFP", "BAG", "BOND", "CMDTY"
        };

        //*********************************************************************
        //  Member: AllowableExchanges
        //
        /// <summary>
        /// List of allowable exchanges.
        /// </summary>
        //*********************************************************************
        private List<string> AllowableExchanges = new List<string>
        {
            "SMART", "AMEX", "ARCA", "BELFOX", "BOX", "BRUT", "BTRADE", "CBOE",
            "CBOT", "CFE", "CME", "DTB", "E-CBOT", "ECBOT", "EUREX US", "FOREX",
            "FTA", "GLOBEX", "HKFE", "IBIS", "ICE", "IDEM", "IDEALPRO", "ISE",
            "ISLAND", "LIFFE", "LSE", "MATIF", "ME", "MEFFRV", "MONEP", "NYBOT",
            "NYMEX", "NYSE", "ONE", "OSE.JPN PHLX", "PSE", "SNFE", "SOFFEX",
            "SUPERMONTAGE", "SWX", "TSE", "TSE.JPN", "TSX", "VIRTX", "XETRA"
        };
        #endregion CLASS MEMBERS

        //*********************************************************************
        //  Property: Symbol
        // 
        /// <summary>
        /// The original symbol string, i.e., 
        /// AAPL-OPT-20191218-1350.0-C-NYSE/ARCA/GLOBEX-EUR
        /// </summary>
        //*********************************************************************
        public string Symbol { get; set; }

        //*********************************************************************
        //  Property: TickerSymbol
        // 
        /// <summary>
        /// The ticker symbol only
        /// </summary>
        //*********************************************************************
        public string TickerSymbol { get; set; }

        //*********************************************************************
        //  Property: PrimaryCurrency
        //
        /// <summary>
        /// For FOREX currency pair.
        /// </summary>
        //*********************************************************************
        public string PrimaryCurrency { get; set; }

        //*********************************************************************
        //  Property: AssetType
        // 
        /// <summary>
        /// Asset type from the original symbol (STK, OPT, FUND, CMDTY, etc)
        /// </summary>
        //*********************************************************************
        public string AssetType { get; set; }

        //*********************************************************************
        //  Property: TDAssetType
        // 
        /// <summary>
        /// TD Ameritrade asset type, i.e. "EQUITY", "OPTION". "ETF", 
        /// "MUTUAL FUND", etc.
        /// </summary>
        //*********************************************************************
        public string TDAssetType { get; set; }

        //*********************************************************************
        //  Property: Exchange
        // 
        /// <summary>
        /// The exchanged (NYSE, NASDAQ, AMEX, etc.) the asset trades on.
        /// </summary>
        //*********************************************************************
        public string Exchange { get; set; }

        //*********************************************************************
        //  Property: Price
        // 
        /// <summary>
        /// (OPTIONAL): Current ask price of the asset, or NULL for 
        /// subscribing the asset. An asset must be subscribed before any 
        /// information about it can be retrieved.
        /// </summary>
        //*********************************************************************
        public double Price { get; set; }

        //*********************************************************************
        //  Property: Spread
        // 
        /// <summary>
        /// (OPTIONAL): Current difference of ask and bid price of the asset.
        /// </summary>
        //*********************************************************************
        public double Spread { get; set; }

        //*********************************************************************
        //  Property: Volume
        // 
        /// <summary>
        /// (OPTIONAL): Current trade volume of the asset, or 0 when the volume
        /// is unavailable, as for currencies, indexes, or CFDs.
        /// </summary>
        //*********************************************************************
        public double Volume { get; set; }

        //*********************************************************************
        //  Property: Pip
        // 
        /// <summary>
        /// (OPTIONAL): Size of 1 PIP, e.g. 0.0001 for EUR/USD, 0.0 for an JPY
        /// FOREX pair.
        /// </summary>
        //*********************************************************************
        public double Pip { get; set; }

        //*********************************************************************
        //  Property: PipCost
        // 
        /// <summary>
        /// (OPTIONAL): Calculated cost of 1 Pip.
        /// </summary>
        //*********************************************************************
        public double PipCost { get; set; }

        //*********************************************************************
        //  Property: LotAmount
        // 
        /// <summary>
        /// (OPTIONAL): Minimum order size, i.e. number of contracts for 1 lot
        /// of the asset. For currencies on TD Ameritrade it's 10000 for 1 lot.
        /// </summary>
        //*********************************************************************
        public double LotAmount { get; set; }

        //*********************************************************************
        //  Property: MarginCost
        // 
        /// <summary>
        /// (OPTIONAL): Required margin for buying 1 lot of the asset in  units
        /// of the account currency. Determines the leverage. Calculate, if
        /// needed.
        /// </summary>
        //*********************************************************************
        public double MarginCost { get; set; }

        //*********************************************************************
        //  Property: option
        //
        /// <summary>
        /// If this is an OPTION asset, then information about the OPTION
        /// </summary>
        //*********************************************************************
        public OptionAsset option { get; set; }

        //*********************************************************************
        //  Property: futures
        //
        /// <summary>
        /// If this is a FUTURES asset, then information about the FUTURES.
        /// </summary>
        //*********************************************************************
        public FutuersAsset futures { get; set; }

        //*********************************************************************
        //  Property: forex
        //
        /// <summary>
        /// If this is an FOREX asset, then information about the currency
        /// pair.
        /// </summary>
        //*********************************************************************
        public ForexAsset forex { get; set; }

        //*********************************************************************
        //  Property: Valid
        //
        /// <summary>
        /// Whether the symbol string is valid
        /// </summary>
        /// 
        /// <remarks>
        /// True if valid, false if not.
        /// </remarks>
        //*********************************************************************
        public bool Valid { get; set; }

        #region CLASS CONSTRUCTORS
        //*********************************************************************
        //  Constructor: TDAsset
        //
        /// <summary>
        /// The class constructor
        /// </summary>
        /// 
        /// <param name="assetStr">
        /// The asset string, this constructor is called with.
        /// </param>
        //*********************************************************************
        public TDAsset
            (
            string assetStr
            )
        {
            // Save the original asset string
            Symbol = assetStr;

            // Parse the asset string and fill out the class properties
            Parse();
        }

        // Parameterless constructor
        public TDAsset() { }
        #endregion CLASS CONSTRUCTOR

        //*********************************************************************
        //  Method: Parse
        //
        /// <summary>
        /// Parse the symbol, which is the original asset string.
        /// </summary>
        //*********************************************************************
        private void
            Parse
            ()
        {
            // Method members
            string[] currency;
            string[] parts;
            string type;
            string part;

            try
            {
                // Split the Symbol by hyphens
                parts = Symbol.Split('-');

                // Are there parts available?
                if (parts.Length > 0)
                {
                    //*********************************************************
                    //
                    // SECTION 1: TICKER SYMBOL
                    //
                    //*********************************************************
                    if (!parts[0].Contains("/"))
                    {
                        // NO: Get the stock ticker symbol
                        TickerSymbol = parts[0].Trim();
                    }
                    else
                    {
                        // YES: Split the currency
                        currency = parts[0].Split('/');

                        // Get the primary and counter currencies
                        PrimaryCurrency = currency[0];

                        // Does the secondary currency exist?
                        if (currency.Length > 1)
                        {
                            // YES: Initialize the Forex asset of this TDAsset
                            if (forex == null) forex = new ForexAsset();

                            // Save the counter currency
                            forex.CounterCurrency = currency[1];
                        }
                    }

                    // If there is only one part, then return now
                    if (parts.Length == 1)
                    {
                        // Assumed to be a stock
                        AssetType = "STK";

                        // Assumed to be NYSE
                        Exchange = "NYSE";

                        // Assumed to be default currency
                        PrimaryCurrency = Broker.settings.Currency;

                        // Symbol is valid
                        Valid = true;

                        // Return
                        return;
                    }

                    //*********************************************************
                    //
                    // SECTION 2: ASSET TYPE
                    //
                    //*********************************************************
                    type = parts[1].Trim();

                    // Validate against the allowable asset types. Does the
                    // allowable asset types list contain this type?
                    if (AllowableAssetTypes.Contains(type))
                    {
                        // YES: Sove it
                        AssetType = type;
                    }
                    else
                    {
                        // NO: Not an allowable asset type. Not valid string.
                        Valid = false;
                    }

                    // If only two parts, then return what we have so far
                    if (parts.Length == 2)
                    {
                        // Only two parts, an asset and an asset type, exchange
                        // and counter currency may be omitted. Currency is
                        // omitted, so use the default currency.
                        PrimaryCurrency = Broker.settings.Currency;

                        // Exchange is ommitted, so use NYSE
                        Exchange = "NYSE";

                        // Set asset symbol as valid
                        Valid = true;

                        // Return with what we have so sfar
                        return;
                    }

                    // AT THIS POINT, SHOULD HAVE WHETHER THE SYMBOL IS FOR:
                    // (1) an OPTION (OPT); (2) a FUTURES (FUT); or, (3) a 
                    // FUTURE OPTION (FOP)
                    //
                    // Switch based on asset type
                    switch (AssetType)
                    {
                        case "FOP":
                        case "OPT":
                            GetOptions(parts);
                            break;

                        case "FUT":
                        case "FUTX":
                            GetFutures(parts);
                            break;

                        case "STK":
                        default:
                            // Get the remaining parts of the symbol code
                            GetEquities(parts);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"{Resx.GetString("ERROR")}: {e.Message}");
            }
        }

        //*********************************************************************
        //  Method: GetEquities
        //
        /// <summary>
        /// Get the asset information for an equity (STOCK or ETF)
        /// </summary>
        /// 
        /// <param name="parts">
        /// An array containing the split apart original symbol string.
        /// </param>
        //*********************************************************************
        private void
            GetEquities
            (            
            string[] parts
            )
        {
            // Method members
            string part;
            List<string> exchanges = new List<string>();
            List<string> excList = new List<string>();

            //*********************************************************
            //
            // SECTION 3: Exchange
            //
            //*********************************************************                    
            GetExchange(parts[2].Trim());

            // If only three parts, then return the asset symbol
            if (parts.Length == 3)
            {
                // Currency is missing, use the default currency
                PrimaryCurrency = Broker.settings.Currency;

                // Assert asset is valid and return
                Valid = true;

                // Return with what we have
                return;
            }

            //*********************************************************
            //
            // SECTION 3: Currency
            //
            //*********************************************************                    

            // Get the currency
            part = parts[3];

            // Is this currency is no within the list of currencies supported,
            // use the USD.
            if (!CurrencyInterestRates.CurrencyDict.ContainsKey(part))
                part = "USD";

            // Save the currency
            PrimaryCurrency = part;

            // Asset is valid
            Valid = true;
        }

        //*********************************************************************
        //  Method: GetEquities
        //
        /// <summary>
        /// Get the asset information for an OPTION (OPT or FOP)
        /// </summary>
        /// 
        /// <param name="parts">
        /// An array containing the split apart original symbol string.
        /// </param>
        //*********************************************************************
        private void
            GetOptions
            (
            string[] parts
            )
        {
            // Method members
            string part;
            double price;
            DateTime date;

            // Initialize the options section
            if (option == null) option = new OptionAsset();

            //*********************************************************
            //
            // SECTION 3: EXPIRATION DATE (YYYYMMDD)
            //
            //*********************************************************                    
            part = parts[2].Trim();

            // Call expiration date function
            GetExpirationDate(part);

            // If expiration date resulted in invalid asset symbol,
            // just return
            if (!Valid) return;

            // If only three parts, then return what we have sofar
            if (parts.Length == 3)
            {
                // Option is not valid
                Valid = false;

                // Return the asset symbol
                return;
            }

            //*********************************************************
            //
            // SECTION 4: Strike price
            //
            //*********************************************************                    
            part = parts[3].Trim();

            // Parse a double value. Is it valid?
            if (Double.TryParse(part, out price))
            {
                // YES: Save the strike price
                option.StrikePrice = price;
            }

            // If only four parts, then return what we have sofar
            if (parts.Length == 4)
            {
                // Option is not valid
                Valid = false;

                // Return the asset symbol
                return;
            }

            //*********************************************************
            //
            // SECTION 5: PUT or CALL
            //
            //*********************************************************                    
            part = parts[4].Trim();

            // PUT or CALL?
            if (part == "C" || part == "P")
            {
                // YES: Save the PUT or CALL
                option.PutCallType = part;

                // Asset is valid
                Valid = true;
            }
            else
            {
                // Option is not valid
                Valid = false;

                // Return
                return;
            }

            // if only five parts return asset
            if (parts.Length == 5)
            {
                // Exchange not given, assume it is NYSE
                Exchange = "NYSE";

                // Asset is valid
                Valid = true;

                // Return
                return;
            }

            //*********************************************************
            //
            // SECTION 6: Exchange
            //
            //*********************************************************                    
            GetExchange(parts[5].Trim());

            // if only six parts return asset
            if (parts.Length == 6)
            {
                // Currency not given, assume it is default
                PrimaryCurrency = Broker.settings.Currency;

                // Asset is valid
                Valid = true;

                // Return
                return;
            }

            //*********************************************************
            //
            // SECTION 7: Currency
            //
            //*********************************************************                    
            part = parts[6].Trim();

            // Get the currency from the asset symbol string OR from
            // the default currency in the settings file
            if (CurrencyInterestRates.CurrencyDict.ContainsKey(part))
                PrimaryCurrency = part;
            else
                PrimaryCurrency = Broker.settings.Currency;

            // Have a valid asset now
            Valid = true;
        }

        //*********************************************************************
        //  Method: GetFutures
        //
        /// <summary>
        /// Get information for a FUT OR FUTX asset.
        /// </summary>
        /// 
        /// <param name="parts">
        /// An array containing the split apart original symbol string.
        /// </param>
        //*********************************************************************
        private void
            GetFutures
            (
            string[] parts
            )
        {
            // Method members
            DateTime date;
            string part;

            // Initialize the FUTURE section
            if (futures == null) futures = new FutuersAsset();

            //*********************************************************
            //
            // SECTION 3: EXPIRATION DATE (YYYYMMDD)
            //
            //*********************************************************                    
            part = parts[2].Trim();

            // Call expiration date function
            GetExpirationDate(part);

            // If expiration date resulted in invalid asset symbol,
            // just return
            if (!Valid) return;

            // If only three parts, then return what we have sofar
            if (parts.Length == 3)
            {
                // Option is not valid
                Valid = false;

                // Return the asset symbol
                return;
            }

            //*********************************************************
            //
            // SECTION 4: TRADING CLASS
            //
            //*********************************************************                    
            part = parts[3].Trim();

            // Get the trading class
            if (part != Symbol) futures.TradingClass = part;

            // If only four parts, then return what we have sofar
            if (parts.Length == 4)
            {
                // Option is valid
                Valid = true;

                // Exchange in NYSE
                Exchange = "NYSE";

                // Currency is defauld
                PrimaryCurrency = Broker.settings.Currency;

                // Return
                return;
            }

            //*********************************************************
            //
            // SECTION 5: Exchange
            //
            //*********************************************************                    
            GetExchange(parts[4].Trim());

            // if only five parts return asset
            if (parts.Length == 5)
            {
                // Currency not given, assume it is default
                PrimaryCurrency = Broker.settings.Currency;

                // Asset is valid
                Valid = true;

                // Return the asset symbol
                return;
            }

            //*********************************************************
            //
            // SECTION 6: Currency
            //
            //*********************************************************                    
            part = parts[5].Trim();

            // Get the currency from the asset symbol string OR from
            // the default currency in the settings file
            if (CurrencyInterestRates.CurrencyDict.ContainsKey(part))
                PrimaryCurrency = part;
            else
                PrimaryCurrency = Broker.settings.Currency;

            // Have a valid asset now
            Valid = true;
        }

        //*********************************************************************
        //  Method: GetExchange
        //
        /// <summary>
        /// Get the exchange(s) for a given asset
        /// </summary>
        /// 
        /// <param name="exc">
        /// The exchange string, which may have one or more exchanges separated
        /// by forward slashes.
        /// </param>
        //*********************************************************************
        private void
            GetExchange
            (
            string exc
            )
        {
            // Method members
            List<string> exchanges = new List<string>();
            List<string> excList = new List<string>();

            // If the exchange contains "/" separate out the exchanges
            if (exc.Contains("/")) exchanges.AddRange(exc.Split('/'));
            else exchanges.Add(exc);

            // Iterate through the exchanges
            foreach (string exchange in exchanges)
            {
                // Is this an allowable exchange?
                if (AllowableExchanges.Contains(exchange))
                {
                    // YES: Save it.
                    excList.Add(exchange);
                }
                else
                {
                    // NO: Flag as invalid asset symbol and return
                    Valid = false;
                    return;
                }
            }

            // Recombine the exchanges, separated by a comma this time
            // and save them
            Exchange = string.Join(",", excList);
        }

        //*********************************************************************
        //  Method: GetExpirationDate
        //
        /// <summary>
        /// Get the expiration date
        /// </summary>
        /// 
        /// <param name="strDate">
        /// The expiration date as a string.
        /// </param>
        /// 
        /// <remarks>
        /// The expiration date can be in one of two formats:
        /// (1) YYYYMMDD; OR
        /// (2) XN, where X is the month code and N is the last digit of the
        ///     year.
        /// </remarks>
        //*********************************************************************
        private void
            GetExpirationDate
            (
            string strDate
            )
        {
            // Method members
            string monthCode;
            int monthIndex;
            int yearCode;
            DateTime date;

            // Set the asset symbol to not valid
            Valid = false;

            try
            {
                // Are there only numbers in the date string?
                if (!Regex.IsMatch(strDate, @"^[A-Za-z]"))
                {
                    // YES: Parse of date time value good?
                    if (DateTime.TryParse($"{strDate.Substring(0, 4)}-{strDate.Substring(4, 2)}-{strDate.Substring(6, 2)}", out date))
                    {
                        // YES: Save the date
                        if (AssetType == "OPT") option.ExpirationDate = date;
                        else futures.ExpirationDate = date;

                        // Set validity of option based on exp. date?
                        Valid = date > DateTime.UtcNow;
                        
                        // Nothing more to process
                        return;
                    }
                }
                else
                {
                    // NO: An alphanumeric expiration date string should be in form
                    // of SSSSXN, where SSS is the symbol of the asset, X is the
                    // month character and N is the last digit of the year.
                    //
                    // Does expiration date string have the asset symbol in it?
                    if (strDate.StartsWith(Symbol))
                    {
                        // YES: Eliminate the asset symbol
                        strDate = strDate.Replace(Symbol, "");

                        // The date string should now be only two characters
                        if (strDate.Length == 2)
                        {
                            // YES: Correct length. Get the month code and the year
                            monthCode = strDate[0].ToString();
                            yearCode = Convert.ToInt32(strDate.Substring(1, 1));

                            // Is this a FUT or an FUTX?
                            if (AssetType.StartsWith("FUT"))
                            {
                                // YES: Expiration date for FUTURES. Third Fri.
                                // of every third month.
                                //
                                // Get the month index for the FUTURE expiration
                                monthIndex = FutureCodes.IndexOf(monthCode) + 1;

                                // Get the FUTURES expiration date
                                futures.ExpirationDate = GetDateFromCode(yearCode, monthIndex);
                            }
                            else
                            {
                                // NO: For OPTIONS. For CALL?
                                if (CallCodes.Contains(monthCode))
                                {
                                    // YES: For a CALL
                                    option.PutCallType = "C";

                                    // Get the month index for the CALL expiration
                                    monthIndex = CallCodes.IndexOf(monthCode) + 1;
                                }
                                else
                                {
                                    // NO: For a PUT
                                    option.PutCallType = "P";

                                    // Get the month index for the PUT expiration
                                    monthIndex = PutCodes.IndexOf(monthCode) + 1;
                                }

                                // Get and save the option expiration date
                                option.ExpirationDate = GetDateFromCode(yearCode, monthIndex);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            // Return the asset symbol
            return;
        }

        //*********************************************************************
        //  Method: GetDateFromCode
        //
        /// <summary>
        /// Return a date time from a year and month code.
        /// </summary>
        /// 
        /// <param name="yearCode">
        /// The year code of the date and time (the last digit of the year)
        /// </param>
        /// 
        /// <param name="monthIndex">
        /// The one character month code (different for PUTS, CALLS, and 
        /// FUTURES)
        /// </param>
        /// 
        /// <returns>
        /// A date time object.
        /// </returns>
        /// 
        /// <remarks>
        /// The expiration date, if not set explicitly, is set as the third
        /// Friday of the month.
        /// </remarks>
        //*********************************************************************
        private DateTime
            GetDateFromCode
            (
            int yearCode,
            int monthIndex
            )
        {
            // Method members
            DateTime date;
            int currentYear;
            int currentYearDiv10;
            int lastDigitYear;

            // Get the current year
            currentYear = DateTime.UtcNow.Year;

            // Divide the current year by 10
            currentYearDiv10 = currentYear / 10;

            // Isolate last digit of current year
            lastDigitYear = Math.Abs(currentYear - currentYearDiv10 * 10);

            // If the year code is less than the last digit
            // of the current year, add one to the year
            // divided by 10
            if (yearCode < lastDigitYear) ++currentYearDiv10;

            // Reformulate the expiration year
            date = new DateTime(currentYearDiv10 * 10 + lastDigitYear, monthIndex, 1);
            if (date.TryGetDayOfMonth(DayOfWeek.Friday, 3, out date))
            {
                // Is the expiration date .lte one day from today?
                if (date <= DateTime.UtcNow.AddDays(-1))
                {
                    // YES: Invalid expiration date
                    Valid = false;
                }
                else
                {
                    // NO: Valid expiration date
                    option.ExpirationDate = date;

                    // Set asset valid
                    Valid = true;
                }
            }

            // Return the computed date
            return date;
        }
    }
}
