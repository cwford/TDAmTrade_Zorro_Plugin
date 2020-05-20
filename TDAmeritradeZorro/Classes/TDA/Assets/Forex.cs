//*****************************************************************************
// File: Forex.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The Forex class object, used in trading currency pairs.
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
using System.Runtime.Serialization;
using TDAmeritradeZorro.Interface;

namespace TDAmeritradeZorro.Classes.TDA.Assets
{
    //*************************************************************************
    //  Class: Forex
    //
    /// The Forex class object. See the TD Ameritrade API information at 
    /// https://developer.tdameritrade.com/apis for details on the properties
    /// and usage of this class.
    //*************************************************************************
    [DataContract]
    public class Forex : ForexBase, IPrice
    {
        [DataMember(Name = "changeInDouble")]
        public double Change { get; set; }

        [DataMember(Name = "52WkHighInDouble")]
        public double High52Wk { get; set; }

        [DataMember(Name = "52WkLowInDouble")]
        public double Low52Wk { get; set; }

        [DataMember(Name = "percentChange")]
        public double PercentChange { get; set; }

        [DataMember(Name = "product")]
        public string Product { get; set; }

        [DataMember(Name = "tradingHours")]
        public string TradingHours { get; set; }

        [DataMember(Name = "isTradable")]
        public bool IsTradable { get; set; }

        [DataMember(Name = "marketMaker")]
        public string marketMaker { get; set; }

    }
}
