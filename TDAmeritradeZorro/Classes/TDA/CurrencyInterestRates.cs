//*****************************************************************************
// File: CurrencyInterestRates.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The CurrencyInterestRates class.
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
    //
    // NOT SUPPORTED: FOREX trading (currency pairs) is not currently supported
    // in the TD Ameritrade API. This class is included for FUTURE USE ONLY.
    //
    //*************************************************************************

    //*************************************************************************
    //  Class: CurrencyIntrestRates
    //
    /// <summary>
    /// A class for computing and maintain current currency interest rates used
    /// for computing, among other things, Forex rollover rates charged for
    /// currency trades held overnight.
    /// </summary>
    /// 
    /// <remarks>
    /// NOTE: These interest rates are being obtained from the TradingEconomics
    /// website, esentially through "page scraping." Since the format of that
    /// website can change without warning, the current process of obtaining
    /// these rates may be invalid without warning as well.
    /// </remarks>
    //*************************************************************************
    public class CurrencyInterestRates
    {
        #region CLASS MEMBERS (PRIVATE)
        //*********************************************************************
        //  Member: BaseUrl
        //
        /// <summary>
        /// The Trading Economics website page with a table of current currency
        /// interest rates.
        /// </summary>
        //*********************************************************************
        private static readonly string BaseUrl = "https://tradingeconomics.com/";

        //*********************************************************************
        //  Member: SingleUrlTpl
        //
        /// <summary>
        /// The Trading Economics website page for a single country that is
        /// not found in the main page table.
        /// </summary>
        //*********************************************************************
        private static readonly string SingleUrlTpl = 
            "https://tradingeconomics.com/{0}/interest-rate";

        //*********************************************************************
        //  Member: TimeRead
        //
        /// <summary>
        /// The date and time the current table on the Trading Economics site
        /// was last read.
        /// </summary>
        //*********************************************************************
        private static DateTime TimeRead = DateTime.MinValue;

        //*********************************************************************
        //  Member: ReadTimeExpiration
        //
        /// <summary>
        /// The maximum number of hours before re-reading currency interest
        /// rates from the Trading Economics website.
        /// </summary>
        //*********************************************************************
        private static readonly double ReadTimeExpiration = 24;

        //*********************************************************************
        //  Member: InterestRateDictionary
        //
        /// <summary>
        /// A dictionary of currency symobls/interests rate for major FOREX 
        /// pairs.
        /// </summary>
        //*********************************************************************
        public static Dictionary<string, double> InterestRateDict;

        //*********************************************************************
        //  Member: CurrencyDict
        //
        /// <summary>
        /// A dictionary of currency symobls/country names for major FOREX 
        /// pairs.
        /// </summary>
        //*********************************************************************
        public static Dictionary<string, string> CurrencyDict = new Dictionary<string, string>()
    {
            {"JPY", "Japan" },
            {"CAD", "Canada" },
            {"CNH", "China" },
            {"CNY", "China" },
            {"AUD", "Australia" },
            {"NZD", "New Zealand" },
            {"GBP", "United Kingdom" },
            {"EUR", "Euro Area" },
            {"CHF", "Switzerland" },
            {"HKD", "Hong Kong" },
            {"USD", "United States" },
            {"MXN", "Mexico" },
    };
        #endregion CLASS MEMBERS (PRIVATE)

        #region CLASS METHODS (PRIVATE)
        //*********************************************************************
        //  Method: GetCurrencyInterestRates
        //
        /// <summary>
        /// Get the current central bank interest rates for a given FOREX pair.
        /// </summary>
        /// 
        /// <param name="forexPair">
        /// The currency pair for mhich interest rates are sought.
        /// </param>
        /// 
        /// <returns>
        /// A double array with values representing the interest rate for the 
        /// currency pair, [0] element of the array contains the base currency
        /// interest rade, [1] element of the array contains the quote currency
        /// interest rade.
        /// </returns>
        //*********************************************************************
        private static double[]
            GetCurrencyInterestRates
            (
            string forexPair
            )
        {
            // If not a valid currency pair, return 0.0
            if (forexPair.Length != 7 || !forexPair.Contains("/")) 
                return new double[] {0.0, 0.0 };

            // Break apart the currencies in the pair
            string[] currencies = forexPair.Split('/');

            // Get the IRB
            double IRB = GetInterestRate(currencies[0]);

            // Get the IRQ
            double IRQ = GetInterestRate(currencies[1]);

            // Return a double array
            return new double[] { IRB, IRQ };
        }
        #endregion CLASS METHODS (PRIVATE)

        #region CLASS METHODS (PUBLIC)
        //*********************************************************************
        //  Method: InitCurrencyInterestRates
        //
        /// <summary>
        /// Initialize the currency interest rates.
        /// </summary>
        /// 
        /// <remarks>
        /// Currency interest rates are determined by the central banks of the
        /// country from which the currency derives. There are a few sources of
        /// these rates online but they each have their challenges. This method
        /// determines currency interest rates from either:
        /// 
        ///     (1) global-rates.com, one page for all CB interest rates except
        ///         the Hong Kong Dollar (HKD); and,
        ///         
        ///     (2) tradingeconomics.com, for the Hong Kong Dollar (HKD)
        /// </remarks>
        //*********************************************************************
        public static bool
            InitCurrencyInterestRates
            ()
        {
            // Method members
            Match m;
            double rate = Double.MinValue;
            string result;

            // Regex to extract the interest rate for a given country
            string reCountryTpl = @"/{0}/interest-rate(.*?)>(.*?)<";

            try
            {
                // Initialize the currency interest rate dictionary
                InterestRateDict = new Dictionary<string, double>();

                // Read the global rates document
                result = Helper.GetWebPage(BaseUrl);

                // Save the read time
                TimeRead = DateTime.UtcNow;

                // Iterate through the country keys
                foreach (string key in CurrencyDict.Keys)
                {
                    // Create a Regex for this country
                    Regex reCountry = new Regex(string.Format(reCountryTpl, CurrencyDict[key]).ToLower().Replace(" ", "-"));

                    // Locate the country and the interest rate
                    m = reCountry.Match(result);

                    // Was the country found?
                    if (m.Groups.Count > 1)
                    {
                        // YES: Get the interest rate
                        rate = Double.Parse(m.Groups[2].Value.Replace("%", ""));
                    }
                    else
                    {
                        // NO: Look-up the counttry with the single page URL
                        rate = GetSingleCountryInterestRate(key);
                    }

                    // If interst rate error returned, skip this country
                    if (rate < -900.00) continue;

                    // Add currency interest rate and country to the dictionary
                    if (InterestRateDict.ContainsKey(key))
                    {
                        // UPDATE:
                        InterestRateDict[key] = rate;
                    }
                    else
                    {
                        // ADD:
                        InterestRateDict.Add(key, rate);
                    }
                }

                // Return a success code
                return true;
            }
            catch(Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("CANNOT_GET_INTEREST_RATES")}. " + e.Message);

                // Initialize the interest rate dictionary
                InterestRateDict = new Dictionary<string, double>();
            }

            // Return a failure code
            return false;
        }

        public static double
            GetSingleCountryInterestRate
            (
            string key
            )
        {
            // Method members
            Regex reCountry2 = new Regex(@"<td>(.*\d)</td>");
            string result;
            Match m;
            double rate = -999.99;

            // Get the single page for this country
            result = Helper.GetWebPage(string.Format(
                // Template
                SingleUrlTpl, 

                // Country name after looking it up and replace spaces with
                // hyphens all to lower case
                CurrencyDict[key].ToLower().Replace(" ", "-"))
                );

            // Was the page found?
            if (!string.IsNullOrEmpty(result))
            {
                // YES: Look for a match
                m = reCountry2.Match(result);

                // Match found?
                if (m.Success)
                {
                    // YES: Get the interest rate
                    rate = Convert.ToDouble(m.Groups[1].Value);
                }
            }

            // Return the interst rate found
            return rate;
        }

        //*********************************************************************
        //  Method: GetInterestRate
        //
        /// <summary>
        /// Get the current central bank interest rate for a given currency.
        /// </summary>
        /// 
        /// <param name="currency">
        /// The currency for which a CB interest rate is being sougth.
        /// </param>
        /// 
        /// <returns>
        /// A double value representing the interest rate for the currency.
        /// </returns>
        //*********************************************************************
        public static double
            GetInterestRate
            (
                string currency
            )
        {
            try
            {
                // Is the read time of the interest rates document older than the 
                // set amount?
                if (DateTime.UtcNow.Subtract(TimeRead).TotalHours
                    > ReadTimeExpiration || !InterestRateDict.ContainsKey(currency))
                {
                    // Re-initialize the currency interest rate table
                    InitCurrencyInterestRates();
                }

                // Return the rate converted to a decimal
                return InterestRateDict[currency];
            }
            catch (Exception e)
            {
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: For {currency} " + e.Message);
            }

            return 0.0;
        }

        //*********************************************************************
        //  Method: ComputeRollover
        //
        /// <summary>
        /// Computer thet FOREX rollover (interest debited or credited) to an
        /// account.
        /// </summary>
        /// 
        /// <param name="forexPair">
        /// The currency pair for mhich rollover  interest is being computed.
        /// </param>
        /// 
        /// <returns>
        /// A value representing the rollover interest for this currency pair.
        /// </returns>
        /// 
        /// <remarks>
        /// Rollover is computed based on the following formula:
        /// 
        /// RO = (IRB - IRQ)/(365 * E), where:
        /// 
        /// RO = The rollover rate
        /// IRB = The interest rate for the base currency (first in pair)
        /// IRQ = The interest rate for the quote currency (second in pair)
        /// E = The exchange rate for the currency pair
        /// </remarks>
        //*********************************************************************
        public static double
            ComputeRollover
            (
            string forexPair,
            double E
            )
        {
            // Method members
            double RO;

            // Get the IRB and IRQ
            double[] rates = GetCurrencyInterestRates(forexPair);

            // Compute the rollover rate
            RO = (rates[0] - rates[1]) / (365.0 * E);

            /*
            Console.WriteLine("RO = (IRB - IRQ)/(365 * E)");
            Console.WriteLine($"For {forexPair}...");
            Console.WriteLine($"    IRB  = {rates[0]}");
            Console.WriteLine($"    IRQ  = {rates[1]}");
            Console.WriteLine($"      E  = {E}");
            Console.WriteLine($"Rollover = {RO}\r\n");
            */

            // Return the rollover rate
            return RO;
        }
        #endregion CLASS METHODS (PUBLIC)
    }
}
