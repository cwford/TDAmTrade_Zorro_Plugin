//*****************************************************************************
// File: ProjectBalances.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The ProjectedBalances class object.
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
    //  Class: ProjectedBalances
    //
    /// <summary>
    /// The TD Ameritrade ProjectedBalances class. See the TD Ameritrade API
    /// information at https://developer.tdameritrade.com/apis for details on
    /// the properties of this class.
    /// </summary>
    //*************************************************************************
    [DataContract(Name = "projectedBalances")]
    public class ProjectedBalances
    {
        [DataMember(Name = "availableFunds")]
        public double AvailableFunds { get; set; }

        [DataMember(Name = "availableFundsNonMarginableTrade")]
        public double AvailableFundsNonMarginableTrade { get; set; }

        [DataMember(Name = "buyingPower")]
        public double BuyingPower { get; set; }

        [DataMember(Name = "dayTradingBuyingPower")]
        public double DayTradingBuyingPower { get; set; }

        [DataMember(Name = "dayTradingBuyingPowerCall")]
        public double DayTradingBuyingPowerCall { get; set; }

        [DataMember(Name = "maintenanceCall")]
        public double MaintenanceCall { get; set; }

        [DataMember(Name = "regTCall")]
        public double RegTCall { get; set; }

        [DataMember(Name = "isInCall")]
        public bool IsInCall { get; set; }

        [DataMember(Name = "stockBuyingPower")]
        public double StockBuyingPower { get; set; }
    }
}
