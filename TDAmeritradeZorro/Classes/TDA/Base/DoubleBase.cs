//*****************************************************************************
// File: DoubleBale.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The Double base class object.
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

namespace TDAmeritradeZorro.Classes.TDA.Assets
{
    [DataContract]
    //*************************************************************************
    //  Class: DoubleBase
    //
    /// <summary>
    /// The Double base class object. See the TD Ameritrade API information at 
    /// https://developer.tdameritrade.com/apis for details on the properties
    /// and usage of this class.
    /// </summary>
    /// 
    /// <remarks>
    /// NOTE: Some properties are given by the TD Ameritrade API as floats and
    /// others of a similar name are given as doubles.
    /// </remarks>
    //*************************************************************************
    public class DoubleBase
    {
        [DataMember(Name = "bidPriceInDouble")]
        public double BidPriceInDouble { get; set; }

        [DataMember(Name = "askPriceInDouble")]
        public double AskPriceInDouble { get; set; }

        [DataMember(Name = "lastPriceInDouble")]
        public double LastPriceInDouble { get; set; }

        [DataMember(Name = "highPriceInDouble")]
        public double HighPriceInDouble { get; set; }

        [DataMember(Name = "lowPriceInDouble")]
        public double LowPriceInDouble { get; set; }

        [DataMember(Name = "closePriceInDouble")]
        public double ClosePriceInDouble { get; set; }

        [DataMember(Name = "openPriceInDouble")]
        public double OpenPriceInDouble { get; set; }

        [DataMember(Name = "mark")]
        public double Mark { get; set; }

        [DataMember(Name = "tick")]
        public double Tick { get; set; }

        [DataMember(Name = "tickAmount")]
        public double TickAmount { get; set; }
    }
}
