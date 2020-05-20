//*****************************************************************************
// File: SecuritiesAccount.cs
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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TDAmeritradeZorro.Classes.TDA
{
    //*************************************************************************
    //  Class: SecuritiesAccount
    //
    /// <summary>
    /// The TD Ameritrade SecuritiesAccount class. See the TD Ameritrade API
    /// information at https://developer.tdameritrade.com/apis for details on
    /// the properties of this class.
    /// </summary>
    //*************************************************************************
    [DataContract(Name = "securitiesAccount")]
    public class SecuritiesAccount
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "accountId")]
        public string AccountId { get; set; }

        [DataMember(Name = "roundTrips")]
        public int RoundTrips { get; set; }

        [DataMember(Name = "isDayTrader")]
        public bool IsDayTrader { get; set; }

        [DataMember(Name = "isClosingOnlyRestricted")]
        public bool IsClosingOnlyRestricted { get; set; }
        [DataMember(Name = "positions")]
        public List<Position> Positions { get; set; }

        [DataMember(Name = "orderStrategies")]
        public List<Order> Orders { get; set; }

        [DataMember(Name = "initialBalances")]
        public InitialBalances InitBalances { get; set; }

        [DataMember(Name = "currentBalances")]
        public CurrentBalances CurrBalances { get; set; }

        [DataMember(Name = "projectedBalances")]
        public ProjectedBalances ProjBalances { get; set; }
        public SecuritiesAccount()
        {
        }
    }
}
