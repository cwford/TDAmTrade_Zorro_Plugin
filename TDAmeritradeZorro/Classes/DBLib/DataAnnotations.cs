//*****************************************************************************
// File: DataAnnotations.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: Data annotation claas fro defining persisted properties from a
// class to the Sqlite database.
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
using System.Diagnostics.Contracts;

namespace TDAmeritradeZorro.Classes.DBLib
{
    //*************************************************************************
    //  Class: AutoIncrementAttribute
    //
    /// <summary>
    /// Data annotation attributes used for the Data Access Library
    /// </summary>
    //*************************************************************************
    [AttributeUsage(AttributeTargets.Property)]
    //*************************************************************************
    //  Attribute: AutoIncrementAttribute
    //
    /// <summary>
    /// The AUTOINCREMENT attribute used for the primary key of a table.
    /// </summary>
    //*************************************************************************
    public class AutoIncrementAttribute : Attribute
    {
        public AutoIncrementAttribute
            ()
        {
        }

        //*********************************************************************
        //  Method: ObjectInvariant
        //
        /// <summary>
        /// Method used for attributes.
        /// </summary>
        //*********************************************************************
        [ContractInvariantMethod]
        private void
            ObjectInvariant
            ()
        {
        }
    }

    //*********************************************************************
    //  Attribute: PrimaryKeyAttribute
    //
    /// <summary>
    /// The PRIMARYKEY attribute used on a database property to designate it
    /// as a primary key.
    /// </summary>
    //*********************************************************************
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute()
        {
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
        }
    }

    //*********************************************************************
    //  Attribute: NotNullAttribute
    //
    /// <summary>
    /// The NOT TULL attribute used on a database property which is 
    /// required to be present and with a value.
    /// </summary>
    //*********************************************************************
    [AttributeUsage(AttributeTargets.Property)]
    public class NotNullAttribute : Attribute
    {
        public NotNullAttribute()
        {
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
        }
    }
}
