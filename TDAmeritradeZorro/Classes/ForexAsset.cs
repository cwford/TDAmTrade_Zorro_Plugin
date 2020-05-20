//*****************************************************************************
// File: ForexAsset.cs
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
namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Class: ForexAsset
    //
    /// <summary>
    /// The Forex asset add-on to the TDAsset class
    /// </summary>
    //*************************************************************************
    public class ForexAsset
    {
        //*********************************************************************
        //  Property: CounterCurrency
        //
        /// <summary>
        /// For FOREX currency pair.
        /// </summary>
        //*********************************************************************
        public string CounterCurrency { get; set; }

        //*********************************************************************
        //  Property: RolloverLong
        // 
        /// <summary>
        /// (OPTIONAL): Calculated rollover fee (i.e. interest added to or sub-
        /// tracted from the account) for long trades. TD Ameritrade only uses
        /// rollover fees for FOREX trading.
        /// </summary>
        //*********************************************************************
        public double RolloverLong { get; set; }

        //*********************************************************************
        //  Property: Asset
        // 
        /// <summary>
        /// (OPTIONAL): Calculated rollover fee (i.e. interest added to or sub-
        /// tracted from the account) for short trades. TD Ameritrade only uses
        /// rollover fees for FOREX trading.
        /// </summary>
        //*********************************************************************
        public double RolloverShort { get; set; }

        //*********************************************************************
        //  Constructor: ForexAsset
        //
        /// <summary>
        /// The class constructor
        /// </summary>
        //*********************************************************************
        public ForexAsset()
        {
            RolloverLong = RolloverShort = 0.0;
        }
    }
}
