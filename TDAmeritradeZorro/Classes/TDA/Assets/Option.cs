//*****************************************************************************
// File: Option.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The Option class object.
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
    //  Class: Option
    //
    /// The Option class object. See the TD Ameritrade API information at 
    /// https://developer.tdameritrade.com/apis for details on the properties
    /// and usage of this class.
    //*************************************************************************
    [DataContract]
    public class Option : OHLCBase, IPrice
    {
        [DataMember(Name = "bidPrice")]
        public double BidPrice { get; set; }

        [DataMember(Name = "bidSize")]
        public double BidSize { get; set; }

        [DataMember(Name = "askPrice")]
        public double AskPrice { get; set; }

        [DataMember(Name = "askSize")]
        public double AskSize { get; set; }

        [DataMember(Name = "quoteTimeInLong")]
        public long QuoteTimeInLong { get; set; }

        [DataMember(Name = "mark")]
        public double Mark { get; set; }

        [DataMember(Name = "openInterest")]
        public double OpenInterest { get; set; }

        [DataMember(Name = "volatility")]
        public double Volatility { get; set; }

        [DataMember(Name = "moneyIntrinsicValue")]
        public double MoneyIntrinsicValue { get; set; }

        [DataMember(Name = "multiplier")]
        public double Multiplier { get; set; }

        [DataMember(Name = "strikePrice")]
        public double StrikePrice { get; set; }

        [DataMember(Name = "timeValue")]
        public double TimeValue { get; set; }

        [DataMember(Name = "delta")]
        public double Delta { get; set; }

        [DataMember(Name = "gamma")]
        public double Gamma { get; set; }

        [DataMember(Name = "theta")]
        public double Theta { get; set; }

        [DataMember(Name = "vega")]
        public double Vega { get; set; }

        [DataMember(Name = "rho")]
        public double Rho { get; set; }

        [DataMember(Name = "theoreticalOptionValue")]
        public double TheoreticalOptionValue { get; set; }

        [DataMember(Name = "underlyingPrice")]
        public double UnderlyingPrice { get; set; }

        [DataMember(Name = "deliverables")]
        public string Deliverables { get; set; }

        [DataMember(Name = "contractType")]
        public string contractType { get; set; }

        [DataMember(Name = "underlying")]
        public string Underlying { get; set; }

        [DataMember(Name = "uvExpirationType")]
        public string UVExpirationType { get; set; }

        [DataMember(Name = "settlementType")]
        public string SettlementType { get; set; }
    }
}
