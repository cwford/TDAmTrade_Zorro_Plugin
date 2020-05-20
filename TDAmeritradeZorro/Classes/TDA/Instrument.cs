//*****************************************************************************
// File: Instrument.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The Instrument class object.
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
    //  Class: Instrument
    //
    /// <summary>
    /// The TD Ameritrade Instrument class. See the TD Ameritrade API
    /// information at https://developer.tdameritrade.com/apis for details on
    /// the properties of this class.
    /// </summary>
    //*************************************************************************
    [DataContract(Name = "instrument")]
    public class Instrument
    {
        [DataMember(Name = "assetType")]
        public string AssetType { get; set; }

        [DataMember(Name = "cusip")]
        public string Cusip { get; set; }

        [DataMember(Name = "symbol")]
        public string Symbol { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
