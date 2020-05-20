//*****************************************************************************
// File: TradeId.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: Class that holds the next Zorro trade id number.
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
using DBLib.Classes;

namespace TDAmeritradeZorro.Classes.TDA
{
    //*************************************************************************
    //  Classe: TradeId
    //
    /// <summary>
    /// A simple class that just holds the current Zorro Trade number, used for
    /// assigning an INTEGER to a Zorro trade because TD Ameritrade assigns a 
    /// LONG to a trade id.
    /// </summary>
    //*************************************************************************
    public class TradeId
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

        [NotNull]
        public int NextZorroId { get; set; }
    }
}
