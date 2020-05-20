//*****************************************************************************
// File: Positions.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The Position class object.
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

namespace TDAmeritradeZorro.Classes.TDA
{
    //*************************************************************************
    //  Class: Position
    //
    /// <summary>
    /// The TD Ameritrade Position class. See the TD Ameritrade API
    /// information at https://developer.tdameritrade.com/apis for details on
    /// the properties of this class.
    /// </summary>
    //*************************************************************************
    [DataContract]
    public class Position
    {
        [DataMember(Name = "shortQuantity")]
        public double ShortQuantity { get; set; }

        [DataMember(Name = "averagePrice")]
        public double AveragePrice { get; set; }

        [DataMember(Name = "currentDayProfitLoss")]
        public double CurrentDayProfitLoss { get; set; }

        [DataMember(Name = "currentDayProfitLossPercentage")]
        public double CurrentDayProfitLossPercentage { get; set; }

        [DataMember(Name = "longQuantity")]
        public double LongQuantity { get; set; }

        [DataMember(Name = "settledLongQuantity")]
        public double SettledLongQuantity { get; set; }

        [DataMember(Name = "settledShortQuantity")]
        public double SettledShortQuantity { get; set; }

        [DataMember(Name = "instrument")]
        public Instrument Instrument { get; set; }

        [DataMember(Name = "marketValue")]
        public double MarketValue { get; set; }
    }
}
