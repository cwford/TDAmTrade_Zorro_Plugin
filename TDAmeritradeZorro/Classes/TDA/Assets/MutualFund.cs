//*****************************************************************************
// File: MutualFund.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The MutualFund class object.
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
using TDAmeritradeZorro.Interface;

namespace TDAmeritradeZorro.Classes.TDA.Assets
{
    //*************************************************************************
    //  Class: MutualFund
    //
    /// The MutualFund class object. See the TD Ameritrade API information at 
    /// https://developer.tdameritrade.com/apis for details on the properties
    /// and usage of this class.
    //*************************************************************************
    public class MutualFund : DivBase, IPrice
    {
        public double AskPrice { get; set; }
        public double BidPrice { get; set; }
        public double LastPrice { get; set; }

        public MutualFund()
        {
            AskPrice = BidPrice = LastPrice = 0.00;
        }
    }
}
