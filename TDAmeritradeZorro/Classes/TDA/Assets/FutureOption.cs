//*****************************************************************************
// File: FutureOption.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The FutureOption class object.
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
    //*************************************************************************
    //  Class: FutureOption
    //
    /// The FutureOption class object. See the TD Ameritrade API information at 
    /// https://developer.tdameritrade.com/apis for details on the properties
    /// and usage of this class.
    //*************************************************************************
    [DataContract]
    public class FutureOption : FutureBase
    {
        [DataMember(Name = "volatility")]
        public double Volatility { get; set; }

        [DataMember(Name = "netChangeInDouble")]
        public double NetChange { get; set; }

        [DataMember(Name = "moneyIntrinsicValueInDouble")]
        public double MoneyIntrinsicValue { get; set; }

        [DataMember(Name = "multiplierInDouble")]
        public double Multiplier { get; set; }

        [DataMember(Name = "strikePriceInDouble")]
        public double StrikePrice { get; set; }

        [DataMember(Name = "timeValueInDouble")]
        public double TimeValue { get; set; }

        [DataMember(Name = "deltaInDouble")]
        public double Delta { get; set; }

        [DataMember(Name = "gammaInDouble")]
        public double Gamma { get; set; }

        [DataMember(Name = "thetaInDouble")]
        public double Theta { get; set; }

        [DataMember(Name = "vegaInDouble")]
        public double Vega { get; set; }

        [DataMember(Name = "rhoInDouble")]
        public double Rho { get; set; }

        [DataMember(Name = "expirationType")]
        public string expirationType { get; set; }

        [DataMember(Name = "exerciseType")]
        public string exerciseType { get; set; }

        [DataMember(Name = "inTheMoney")]
        public string inTheMoney { get; set; }

        [DataMember(Name = "contractType")]
        public string contractType { get; set; }

        [DataMember(Name = "underlying")]
        public string underlying { get; set; }
    }
}
