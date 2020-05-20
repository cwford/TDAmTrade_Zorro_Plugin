//*****************************************************************************
// File: MutualFundInfo.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: Retrieve basic information about a mutual fund.
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

namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Class: MutualFundFinder
    //
    /// <summary>
    /// A class to hold basic information about a mutual.
    /// </summary>
    //*************************************************************************

    public class MutualFundInfo
    {
        #region CLASS MEMBERS
        //*********************************************************************
        //  Member: baseUrl
        //
        /// <summary>
        /// The url template used to look-up information about a mutual fund.
        /// </summary>
        //*********************************************************************
        // This is the base URL. Currently using data from MaxFunds.Com
        private static readonly string baseUrl = "http://www.maxfunds.com/funds/data.php?ticker={0}";

        //*********************************************************************
        //  Members: reTopData, liData, spanData, tdData
        //
        /// <summary>
        /// Regex strings used to find HTML elements
        /// </summary>
        /// 
        /// <remarks>
        /// reTopData = to find the "topData" <DIV> element.
        /// liData = to find all <li> elements.
        /// spanData = to find all <span> elements.
        /// tdData = to find all <td> elements.
        /// </remarks>
        //*********************************************************************
        private static Regex reTopData = new Regex("<div class=\"dataTop\">(.*?)div>");
        private static Regex liData = new Regex("<li>(.*?)</li>");
        private static Regex spanData = new Regex("<span class=\"dataspan\">(.*?)</span>");
        private static Regex tdData = new Regex("<td(.*?)>(.*?)</td>");

        #endregion CLASS MEMBERS

        #region CLASS PROPERTIES
        //*********************************************************************
        //  Property: Symbol
        //
        /// <summary>
        /// The mutual fund symbol
        /// </summary>
        //*********************************************************************
        public string Symbol { get; set; }

        //*********************************************************************
        //  Property: Type
        //
        /// <summary>
        /// The mutual fund type (NO LOAD, LOAD, etc).
        /// </summary>
        //*********************************************************************
        public string Type { get; set; }

        //*********************************************************************
        //  Property: MinimumInvestment
        //
        /// <summary>
        /// The initial minimum investment in the fund.
        /// </summary>
        //*********************************************************************
        public double MinimumInvestment { get; set; }

        //*********************************************************************
        //  Property: InitialFees
        //
        /// <summary>
        /// The initial fees for the fund.
        /// </summary>
        //*********************************************************************
        public double InitialFees { get; set; }

        //*********************************************************************
        //  Property: DeferredFees
        //
        /// <summary>
        /// The deferred fees for the fund.
        /// </summary>
        //*********************************************************************
        public double DeferredFees { get; set; }

        //*********************************************************************
        //  Property: RedemptionFees
        //
        /// <summary>
        /// The redemption fees for the fund.
        /// </summary>
        //*********************************************************************
        public double RedemptionFees { get; set; }
        #endregion CLASS MEMBERS

        #region CONSTRUCTOR
        //*********************************************************************
        //  Property: MutualFundInfo
        //
        /// <summary>
        /// The class constructor.
        /// </summary>
        //*********************************************************************
        public MutualFundInfo
            ()
        {

        }
        #endregion CONSTRUCTOR

        #region PUBLIC STATIC METHODS
        //*********************************************************************
        //  Member: GutFundInfo
        //
        /// <summary>
        /// Get information about a mutual fund
        /// </summary>
        /// 
        /// <param name="symbol">
        /// The stock ticker symbol for the mutual fund.
        /// </param>
        //*********************************************************************
        public static MutualFundInfo
            GetFundInfo
            (
            string symbol
            )
        {
            // Method members
            MutualFundInfo mfi = new MutualFundInfo();
            string[] liInfo = new string[4];
            string result;
            Match m;
            bool found = false;
            MatchCollection mc;

            // Read the information page for this fund
            result = Helper.GetWebPage(string.Format(baseUrl, symbol));
            result = result.Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // Isolate the top line data on the fund
            m = reTopData.Match(result);

            // Was a match found?
            if (m.Success)
            {
                // YES: Get the <li> data in this match
                mc = liData.Matches(m.Groups[1].Value);

                // <li> data found?
                if (mc.Count == 4)
                {
                    // YES: Create a new mutual fund object
                    mfi = new MutualFundInfo();

                    // Iterate through the <li> elements and get the data
                    for (int i = 0; i < mc.Count; ++i)
                    {
                        m = spanData.Match(mc[i].Value);
                        liInfo[i] = m.Groups[1].Value;
                        if (!string.IsNullOrEmpty(m.Groups[1].Value)) found = true;
                    }

                    // Was any data found
                    if (found)
                    {
                        // Add the <li> info to the object

                        // Type of fund
                        mfi.Type = liInfo[1];

                        // Minimum investment
                        mfi.MinimumInvestment = GetNumber(liInfo[2]);

                        // If we got here, then add the ticker symbol
                        mfi.Symbol = symbol;

                        // Look for the costs and fees
                        mc = tdData.Matches(result);

                        // Were costs and fees found?
                        if (mc.Count > 0)
                        {
                            // MAYBE: Iterate through all the TD elements
                            for (int i = 0; i < mc.Count; ++i)
                            {
                                // Is this the TD element for "Initial Fees"
                                if (mc[i].Groups[0].Value.Contains("Initial"))
                                {
                                    // YES: Get the value
                                    mfi.InitialFees = GetNumber(mc[i + 1].Groups[2].Value);
                                }

                                // Is this the TD element for "Deferred Fees"
                                if (mc[i].Groups[0].Value.Contains("Deferred"))
                                {
                                    // YES: Get the value
                                    mfi.DeferredFees = GetNumber(mc[i + 1].Groups[2].Value);
                                }

                                if (mc[i].Groups[0].Value.Contains("Redemption"))
                                {
                                    // YES: Get the value
                                    mfi.RedemptionFees = GetNumber(mc[i + 1].Groups[2].Value);
                                }
                            }
                        }
                    }
                }
            }

            // Return the fund info class object
            return mfi;
        }
        #endregion PUBLIC STATIC METHODS

        #region PRIVATE STATIC METHODS
        //*********************************************************************
        //  Method: GetNumber
        //
        /// <summary>
        /// Get a number from a string that has alphabetic multipliers.
        /// </summary>
        /// 
        /// <param name="field">
        /// The number string.
        /// </param>
        /// 
        /// <returns>
        /// A double value representing the number.
        /// </returns>
        //*********************************************************************
        private static double
            GetNumber
            (
            string field
            )
        {
            // Method membors
            double dVal;
            string dStr;
            string multiplier;

            // Replace the dollar sign
            field = field.Replace("$", "").Trim();

            // Get the field without the multiplier
            dStr = Regex.Replace(field, "[A-Za-z]", "").Trim();

            // Get the multiplier by removing all numbers and periods and 
            // commas from string
            multiplier = Regex.Replace(field, "[0-9.,]", "").Trim();

            // Attempt to convert the number
            if (Double.TryParse(dStr, out dVal))
            {
                // Use the mutiplier
                switch(multiplier)
                {
                    case "K":
                        dVal *= 1000.0;
                        break;

                    case "M":
                        dVal *= 1000000.0;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                dVal = 0.0;
            }

            // Return the double value
            return dVal;
        }
        #endregion PRIVATE STATIC METHODS
    }
}
