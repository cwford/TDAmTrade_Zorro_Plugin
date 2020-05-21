//*****************************************************************************
// File: TradeXref.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: A trade cross-reference class.
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
using TDAmeritradeZorro.Classes.DBLib;

namespace TDAmeritradeZorro.Classes.TDA
{
    //*************************************************************************
    //  Class: TradeXref
    // 
    /// <summary>
    /// A cross reference file of trades.
    /// </summary>
    /// 
    /// <remarks>
    /// TD Ameritrade orders sometimes spawn supplemental orders. These supple-
    /// mental orders are not accounted for by Zorro, meaning that orphan trades
    /// can be created. It is important that whenever a main trade is processed,
    /// any supplemental trades associated with this main trade are also process-
    /// ed. This class, and its related database table, keeps track of all trades
    /// associated with orders from the Zorro trading engine.
    /// </remarks>
    //*************************************************************************
    public class TradeXref
    {
        //*********************************************************************
        //  Property: Id
        //
        /// <summary>
        /// Primary key, and auto-incremented id for database table record.
        /// </summary>
        //*********************************************************************
        [PrimaryKey]
        [AutoIncrement]
        [NotNull]
        public int Id { get; set; }

        //*********************************************************************
        //  Property: PrimaryTDAId
        //
        /// <summary>
        /// The primary TD Ameritrade Id number for the original trade order
        /// submitted by Zorro.
        /// </summary>
        //*********************************************************************
        [NotNull]
        public long PrimaryTDAId { get; set; }

        //*********************************************************************
        //  Property: SecondaryTDAId
        //
        /// <summary>
        /// The secondary, or xref'ed, TD Ameritrade order number for a trade
        /// spawned by submission of the primary order.
        /// </summary>
        //*********************************************************************
        [NotNull]
        public long SecondaryTDAId { get; set; }

        //*********************************************************************
        //  Property: DateEntered
        //
        /// <summary>
        /// The date and time this trade order was entered.
        /// </summary>
        //*********************************************************************
        [NotNull]
        public DateTime DateEntered { get; set; }

        public TradeXref()
        {
            DateEntered = DateTime.UtcNow;
        }
    }
}
