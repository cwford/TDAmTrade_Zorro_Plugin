//*****************************************************************************
// File: CONTRACT.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: CONTRACT structure for reporting an option chain.
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
using System.Runtime.InteropServices;
namespace TDAmeritradeZorro.Structs
{
    //*************************************************************************
    //  Struct: CONTRACT
    //
    /// <summary>
    /// Structure used to report the results of a query of an option chain for
    /// a given symbol.
    /// </summary>
    /// 
    /// <remarks>
    /// This structure is used by Zorro's GET_OPTIONS Broker Command.
    /// </remarks>
    //*************************************************************************
    [StructLayout(LayoutKind.Sequential)]
    public struct CONTRACT
    {
        //*********************************************************************
        //  Member: time
        //
        /// <summary>
        /// The timestamp of the price information in UTC time given as an OLE
        /// time value (i.e. a double).
        /// </summary>
        //*********************************************************************
        public double time;

        //*********************************************************************
        //  Member: fAsk
        //
        /// <summary>
        /// The ask price of the asset during the time period used.
        /// </summary>
        //*********************************************************************
        public float fAsk;

        //*********************************************************************
        //  Member: fBid
        //
        /// <summary>
        /// The bid price of the asset during the time period used.
        /// </summary>
        //*********************************************************************
        public float fBid;

        //*********************************************************************
        //  Member: fVal
        //
        /// <summary>
        /// The time value of the asset.
        /// </summary>
        //*********************************************************************
        public float fVal;

        //*********************************************************************
        //  Member: fVol
        //
        /// <summary>
        /// The volume of the asset during the time period used.
        /// </summary>
        //*********************************************************************
        public float fVol;

        //*********************************************************************
        //  Member: fUnl
        //
        /// <summary>
        /// The price of the underlying asset
        /// </summary>
        //*********************************************************************
        public float fUnl;

        //*********************************************************************
        //  Member: fStrike
        //
        /// <summary>
        /// The strike price of the underlying asset
        /// </summary>
        //*********************************************************************
        public float fStrike;

        //*********************************************************************
        //  Member: Expiry
        //
        /// <summary>
        /// The expiration date of the contract
        /// </summary>
        /// 
        /// <remarks>
        /// Long value but in form of YYYYMMDD
        /// </remarks>
        //*********************************************************************
        public long Expiry;

        //*********************************************************************
        //  Member: Type
        //
        /// <summary>
        /// The type of contract (PUT, CALL, FUTURE, EUROPEAN, BINARY, etc.)
        /// </summary>
        //*********************************************************************
        public long Type;

        public CONTRACT
            (
            double dTime,
            float ask,
            float bid,
            float val,
            float vol,
            float unl,
            float strike,
            long expiry,
            long type
            )
        {
            time = dTime;
            fAsk = ask;
            fBid = bid;
            fVal = val;
            fVol = vol;
            fUnl = unl;
            fStrike = strike;
            Expiry = expiry;
            Type = type;
        }
    }
}
