//*****************************************************************************
// File: OHLC.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The OHLC (open, high, low, close) class object.
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
    //  Class: OHLC
    //
    /// <summary>
    /// The TD Ameritrade OHLC (open, high, low, close) class. See the TD 
    /// Ameritrade API information at https://developer.tdameritrade.com/apis 
    /// for details on the properties of this class.
    /// </summary>
    //*************************************************************************    
    [DataContract]
    public class OHLC
    {
        [DataMember(Name = "open")]
        public double Open { get; set; }

        [DataMember(Name = "high")]
        public double High { get; set; }

        [DataMember(Name = "low")]
        public double Low { get; set; }

        [DataMember(Name = "close")]
        public double Close { get; set; }

        [DataMember (Name = "volume")]
        public int Volume { get; set; }

        [DataMember (Name = "datetime")]
        public long Date { get; set; }
    }
}
