//*****************************************************************************
// File: DBSuccess.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: A success/failure class for the Sqlite database.
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
namespace TDAmeritradeZorro.Classes.DBLib
{
    //*************************************************************************
    //  Class: DBSuccess
    //
    /// <summary>
    /// A simple class to return the status of an operation to the SQLite DB.
    /// </summary>
    //*************************************************************************
    public class DBSuccess
    {
        //*********************************************************************
        //  Property: Success
        //
        /// <summary>
        /// Contains the results of a database operation. TRUE if the operation
        /// is successful, FALSE if it results in a failure.
        /// </summary>
        //*********************************************************************
        public bool Success { get; set; }

        //*********************************************************************
        //  Property: ErrorMsg
        //
        /// <summary>
        /// A string representing an error message, if available, when the
        /// property 'Success' is FALSE. If the proprety 'Success' is TRUE,
        /// then ErrorMsg is an empty string.
        /// </summary>
        //*********************************************************************
        public string ErrorMsg { get; set; }

        //*********************************************************************
        //  Constructor: DBSuccess
        //
        /// <summary>
        /// The constructor for this class.
        /// </summary>
        //*********************************************************************
        public DBSuccess()
        {
            // Initialize the class properties
            Success = true;
            ErrorMsg = string.Empty;
        }
    }
}
