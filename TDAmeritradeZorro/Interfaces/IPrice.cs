//*****************************************************************************
// File: IPrice.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: An interface for capturing price data on an asset
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
namespace TDAmeritradeZorro.Interface
{
    //*************************************************************************
    //  Interface: IPrice
    //
    /// <summary>
    /// An interface for describing price data related to an asset.
    /// </summary>
    //*************************************************************************
    public interface IPrice
    {
        //*********************************************************************
        //  Property: BidPrice
        //
        /// <summary>
        /// The bid price of the asset. The highest price a buyer will pay for
        /// the asset.
        /// </summary>
        //*********************************************************************
        double BidPrice { get; set; }

        //*********************************************************************
        //  Property: AskPrice
        //
        /// <summary>
        /// The ask price of the asset. The lowest price a seller will accept
        /// for the asset.
        /// </summary>
        //*********************************************************************
        double AskPrice { get; set; }

        //*********************************************************************
        //  Property: LastPrice
        //
        /// <summary>
        /// The last price of the asset. The price of the last trade done for
        /// the asset.
        /// </summary>
        //*********************************************************************
        double LastPrice { get; set; }

        //*********************************************************************
        //  Property: ClosePrice
        //
        /// <summary>
        /// The close price of the asset. The volume weighted average price of
        /// all the trades that were done for an asset during the last half an
        /// hour of the trading session, usually between 3:00 and 3:30 pm for
        /// the NYSE.
        /// </summary>
        //*********************************************************************
        double ClosePrice { get; set; }
    }
}
