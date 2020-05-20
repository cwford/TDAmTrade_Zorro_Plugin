//*****************************************************************************
// File: UserAccount.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: The TD Ameritrade user account class.
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
using TDAmeritradeZorro.Classes.TDA;

namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Class: UserAccount
    //
    /// <summary>
    /// A class created after converting a JSON string to a C# UserAccount
    /// object.
    /// </summary>
    //*************************************************************************
    [DataContract]
    public class UserAccount
    {
        //*********************************************************************
        //  Property: UserAccount
        //
        /// <summary>
        /// All information for a particular JSON securitiesAccount object
        /// returned from the TD Ameritrade API.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "securitiesAccount")]
        public SecuritiesAccount Account{ get; set; }
    }
}
