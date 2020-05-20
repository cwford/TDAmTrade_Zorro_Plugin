//*****************************************************************************
// File: EquityETF.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The EquityETF class object.
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
    //  Class: EquityETF
    //
    /// The EquityETF class object. See the TD Ameritrade API information at 
    /// https://developer.tdameritrade.com/apis for details on the properties
    /// and usage of this class.
    //*************************************************************************
    [DataContract]
    public class EquityETF : OHLCBase, IPrice
    {
        [DataMember(Name = "bidPrice")]
        public double BidPrice { get; set; }

        [DataMember(Name = "bidSize")]
        public int BidSize { get; set; }

        [DataMember(Name = "bidId")]
        public string BidId { get; set; }

        [DataMember(Name = "askPrice")]
        public double AskPrice { get; set; }

        [DataMember(Name = "askSize")]
        public int AskSize { get; set; }

        [DataMember(Name = "askId")]
        public string AskId { get; set; }

        [DataMember(Name = "lastSize")]
        public int LastSize { get; set; }

        [DataMember(Name = "lastId")]
        public string LastId { get; set; }

        [DataMember(Name = "bidTick")]
        public string BidTick { get; set; }

        [DataMember(Name = "quoteTimeInLong")]
        public long QuoteTimeInLong { get; set; }

        [DataMember(Name = "mark")]
        public double Mark { get; set; }

        [DataMember(Name = "marginable")]
        public bool IsMarginable { get; set; }

        [DataMember(Name = "shortable")]
        public bool IsShortable { get; set; }

        [DataMember(Name = "volatility")]
        public double Volatility { get; set; }

        [DataMember(Name = "regularMarketLastPrice")]
        public double RegularMarketLastPrice { get; set; }

        [DataMember(Name = "regularMarketLastSize")]
        public int RegularMarketLastSize { get; set; }

        [DataMember(Name = "regularMarketNetChange")]
        public double RegularMarketNetChange { get; set; }

        [DataMember(Name = "regularMarketTradeTimeInLong")]
        public long RegularMarketTradeTimeInLong { get; set; }

        [DataMember(Name = "netPercentChangeInDouble")]
        public double NetPercentChangeInDouble { get; set; }

        [DataMember(Name = "markChangeInDouble")]
        public double MarkChangeInDouble { get; set; }

        [DataMember(Name = "markPercentChangeInDouble")]
        public double MarkPercentChangeInDouble { get; set; }

        [DataMember(Name = "regularMarketPercentChangeInDouble")]
        public double RegularMarketPercentChangeInDouble { get; set; }

        [DataMember(Name = "delayed")]
        public bool IsDelayed { get; set; }
    }
}
