//*****************************************************************************
// File: StartEndHours.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: Start and end hours class
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
using System;
using System.Runtime.Serialization;

namespace TDAmeritradeZorro.Classes.TDA
{
    //*************************************************************************
    //  Class: StartEndHours
    //
    /// <summary>
    /// A class that holds stsarting and ending hours as date time objects. This
    /// class is used by the SessionHours object to hold start and end times for
    /// the three principal markets.
    /// </summary>
    //*************************************************************************
    [DataContract]
    public class StartEndHours
    {
        //*********************************************************************
        //  Property: Start
        //
        /// <summary>
        /// The start hour.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "start")]
        public DateTime Start { get; set; }

        //*********************************************************************
        //  Property: End
        //
        /// <summary>
        /// The ending hour.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "end")]
        public DateTime End { get; set; }
    }
}
