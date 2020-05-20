//*****************************************************************************
// File: SessionHours.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: Classs for storing start and end market hours.
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
using System.Collections.Generic;
using System.Runtime.Serialization;
using TDAmeritradeZorro.Classes.TDA;

namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Class: SessionHours
    //
    /// <summary>
    /// A class that has information about the three principal trading hours
    /// </summary>
    /// 
    /// <remarks>
    /// Hours in UTC (Zulu) time.
    /// </remarks>
    //*************************************************************************
    [DataContract]
    public class SessionHours
    {
        //*********************************************************************
        //  Property: PreMarket
        //
        /// <summary>
        /// Start and end hours for pre-market trading.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "preMarket")]
        public List<StartEndHours> PreMarket { get; set; }

        //*********************************************************************
        //  Property: RegularMarket
        //
        /// <summary>
        /// Start and end hours for regular market trading.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "regularMarket")]
        public List<StartEndHours> RegularMarket { get; set; }

        //*********************************************************************
        //  Property: PostMarket
        //
        /// <summary>
        /// Start and end hours for post-market trading.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "postMarket")]
        public List<StartEndHours> PostMarket { get; set; }
    }
}
