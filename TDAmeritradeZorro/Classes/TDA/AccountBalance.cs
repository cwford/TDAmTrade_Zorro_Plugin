//*****************************************************************************
// File: AccountBalance.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The AccountBalance class object.
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
namespace TDAmeritradeZorro.Classes.TDA
{
    //*************************************************************************
    //  Class: AccountBalance
    //
    /// <summary>
    /// A class that encapsulates the information required by Zorro in response
    /// to the BrokerAccount command, which returns the current account status.
    /// </summary>
    //*************************************************************************
    public class AccountBalance
    {
        //*********************************************************************
        //  Member: AccountId
        //
        /// <summary>
        /// The TD Ameritrade account id.
        /// </summary>
        //*********************************************************************
        public string AccountId { get; set; }

        //*********************************************************************
        //  Member: Balance
        //
        /// <summary>
        /// The current balance on the account.
        /// </summary>
        //*********************************************************************
        public double Balance { get; set; }

        //*********************************************************************
        //  Member: TradeValue
        //
        /// <summary>
        /// The current value of all open trades, computed as:
        /// 
        /// TRADE VALUE = ACCOUNT EQUITY - ACCOUNT BALANCE
        /// 
        /// </summary>
        //*********************************************************************
        public double TradeValue { get; set; }

        //*********************************************************************
        //  Member: MarginValue
        //
        /// <summary>
        /// The total margin bound by all open trades.
        /// </summary>
        //*********************************************************************
        public double MarginValue { get; set; }

    }
}
