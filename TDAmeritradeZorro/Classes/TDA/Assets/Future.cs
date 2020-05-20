//*****************************************************************************
// File: Future.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The Future class object.
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
    //  Class: Future
    //
    /// The Future class object. See the TD Ameritrade API information at 
    /// https://developer.tdameritrade.com/apis for details on the properties
    /// and usage of this class.
    //*************************************************************************
    [DataContract]
    public class Future
    {
        [DataMember(Name = "changeInDouble")]
        public double ChangeInDouble { get; set; }

        [DataMember(Name = "lastId")]
        public string LastId { get; set; }

        [DataMember(Name = "bidId")]
        public string BidId { get; set; }

        [DataMember(Name = "askId")]
        public string AskId { get; set; }

        [DataMember(Name = "product")]
        public string Product { get; set; }

        [DataMember(Name = "futurePriceFormat")]
        public string FuturePriceFormat { get; set; }

        [DataMember(Name = "futureMultiplier")]
        public double FutureMultiplier { get; set; }

        [DataMember(Name = "futureSettlementPrice")]
        public double FutureSettlementPrice { get; set; }

        [DataMember(Name = "futureActiveSymbol")]
        public string FutureActiveSymbol { get; set; }
    }
}
