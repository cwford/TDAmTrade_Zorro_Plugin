//*****************************************************************************
// File: Tick.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: Tick structure for reporting OHLC data on an asset.
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
    //  Struct: Tick
    //
    /// <summary>
    /// Structure used to report the results of a query from the price history
    /// of a given asset.
    /// </summary>
    /// 
    /// <remarks>
    /// This structure is used by Zorro's "BrokerHistory" method. Essentially,
    /// a candlestick is returned (i.e. Open, High, Low, Close) along with the
    /// timestamp of the price in OLE format.
    /// </remarks>
    //*************************************************************************
    [StructLayout(LayoutKind.Sequential)]
    public struct Tick
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
        //  Member: fHigh
        //
        /// <summary>
        /// The high price of the asset during the time period used.
        /// </summary>
        //*********************************************************************
        public float fHigh;

        //*********************************************************************
        //  Member: fLow
        //
        /// <summary>
        /// The low price of the asset during the time period used.
        /// </summary>
        //*********************************************************************
        public float fLow;

        //*********************************************************************
        //  Member: fOpen
        //
        /// <summary>
        /// The opening price of the asset.
        /// </summary>
        //*********************************************************************
        public float fOpen;

        //*********************************************************************
        //  Member: fClose
        //
        /// <summary>
        /// The closing price of the asset.
        /// </summary>
        //*********************************************************************
        public float fClose;

        //*********************************************************************
        //  Member: fVal
        //
        /// <summary>
        /// The value of the asset during the time period used.
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

        public Tick
            (
            double dTime,
            float open,
            float close,
            float high,
            float low,
            float val,
            float vol
            )
        {
            time = dTime;
            fOpen = open;
            fClose = close;
            fHigh = high;
            fLow = low;
            fVal = val;
            fVol = vol;
        }
    }
}

