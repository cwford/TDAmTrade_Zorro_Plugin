//*****************************************************************************
// File: DivBase.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The Div (Dividend) base class object.
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
    //  Class: DivBase
    //
    /// <summary>
    /// The Div base class object. See the TD Ameritrade API information at 
    /// https://developer.tdameritrade.com/apis for details on the properties
    /// and usage of this class.
    /// </summary>
    /// 
    /// <remarks>
    /// This class is related to dividends for a given asset.
    /// </remarks>
    //*************************************************************************
    [DataContract]
    public class DivBase : AssetBase
    {
        [DataMember(Name = "nAV")]
        public double NAV { get; set; }

        [DataMember(Name = "peRatio")]
        public double PERatio { get; set; }

        [DataMember(Name = "divAmount")]
        public double DivAmount { get; set; }

        [DataMember(Name = "divYield")]
        public double DivYield { get; set; }

        [DataMember(Name = "divDate")]
        public string DivDate { get; set; }
    }
}
