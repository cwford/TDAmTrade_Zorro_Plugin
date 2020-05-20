//*****************************************************************************
// File: MarketHours.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The SecuritiesAccount class object.
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
    //  Class: MarketHours
    //
    /// <summary>
    /// The TD Ameritrade MarketHours class. See the TD Ameritrade API
    /// information at https://developer.tdameritrade.com/apis for details on
    /// the properties of this class.
    /// </summary>
    //*************************************************************************    
    [DataContract]
    public class MarketHours
    {
        [DataMember(Name = "date")]
        public string Date { get; set; }

        [DataMember(Name = "marketType")]
        public string MarketType { get; set; }

        [DataMember(Name = "exchange")]
        public string Exchange { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "product")]
        public string Product { get; set; }

        [DataMember(Name = "productName")]
        public string ProductName { get; set; }

        [DataMember(Name = "isOpen")]
        public bool IsOpen { get; set; }

        [DataMember(Name = "sessionHours")]
        public SessionHours HoursOpen { get; set; }

        public double ServerTime { get; set; }
    }
}
