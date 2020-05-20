//*****************************************************************************
// File: OptionAsset.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: Forex asset class.
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
namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Class: ForexAsset
    //
    /// <summary>
    /// The Option asset add-on to the TDAsset class
    /// </summary>
    //*************************************************************************
    public class OptionAsset
    {
        //*********************************************************************
        //  Property: ExpirationDate
        //
        /// <summary>
        /// Expiration date for on OPTION contract.
        /// </summary>
        //*********************************************************************
        public DateTime ExpirationDate { get; set; }

        //*********************************************************************
        //  Property: StrikePrice
        //
        /// <summary>
        /// Strike (contract) price for an OPTION contract.
        /// </summary>
        //*********************************************************************
        public double StrikePrice { get; set; }

        //*********************************************************************
        //  Property: PutCallType
        //
        /// <summary>
        /// "P" = PUT; "C" = CALL
        /// </summary>
        //*********************************************************************
        public string PutCallType { get; set; }

        //*********************************************************************
        //  Property: OptionAsset
        //
        /// <summary>
        /// Class constructor
        /// </summary>
        //*********************************************************************
        public OptionAsset()
        {
            ExpirationDate = DateTime.MinValue;
            StrikePrice = 0.0;
            PutCallType = string.Empty;
        }
    }
}
