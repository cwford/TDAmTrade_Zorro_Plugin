//*****************************************************************************
// File: EnumExtensions.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: Various extensions to the Enum object
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
using System.Reflection;

namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Class: EnumExtensions
    //
    /// <summary>
    /// A class with several extensions to the Enum object, which allow for
    /// retrieving properties of that object.
    /// </summary>
    //*************************************************************************
    public static class EnumExtensions
    {

        //*********************************************************************
        //  Method: GetAttributeByPos
        //
        /// <summary>
        /// Get the value of a specific Enum attribute, by position, through
        /// reflection.
        /// </summary>
        /// 
        /// <param name="value">
        /// The Enum member.
        /// </param>
        /// 
        /// <param name="attrName">
        /// The attribute name (Name, Description, Order, etc.)
        /// </param>
        /// 
        /// <returns>
        /// Value of attribute as string, or empty string if value not found.
        /// </returns>
        /// 
        /// <remarks>
        /// Attribute name argument can be any case because method converts it
        /// to title case.
        /// </remarks>
        //*********************************************************************
        public static object 
            GetAttributeByPos
            (
            this Enum value, 
            int pos
            )
        {
            // If there is no enum, do not process
            if (value == null) return -1;

            // Use reflection to get the type, member info, and attributes of
            // this enum
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(false);

            // If the position of the attribute is greater than the number of
            // attributes the enum has, do no process any further
            if (pos > attributes.Length - 1) return null;
            
            // Return the attribute at the position requested
            return attributes[pos];
        }

        //*********************************************************************
        //  Method: GetAttribute
        //
        /// <summary>
        /// Get the value of a specific Enum attribute through reflection.
        /// </summary>
        /// 
        /// <param name="value">
        /// The Enum member.
        /// </param>
        /// 
        /// <param name="attrName">
        /// The attribute name (Name, Description, Order, etc.)
        /// </param>
        /// 
        /// <returns>
        /// Value of attribute as string, or empty string if value not found.
        /// </returns>
        /// 
        /// <remarks>
        /// Attribute name argument can be any case because method converts it
        /// to title case.
        /// </remarks>
        //*********************************************************************
        public static string
            GetAttribute
            (
            this Enum value,
            string attrName,
            bool resValue = false
            )
        {
            // Method members
            string strValue = string.Empty;
            string attrValue = string.Empty;

            // Get the attributes for this enum value, try first attribute set 
            var attribute = value.GetAttributeByPos(0);

            // Get the property information
            PropertyInfo PI = attribute.GetType().GetProperty(attrName);

            // Does the PI exist?
            if (PI == null)
            {
                // NO: Attempt to get PI from second attribute set
                attribute = value.GetAttributeByPos(1);
                if (attribute != null) PI = attribute.GetType().GetProperty(attrName);
            }

            // Does the PI now exist?
            if (PI != null)
            {
                // YES: Get its value
                attrValue = PI.GetValue(attribute, null).ToString();

                // If the attribute value exists, return it
                if (attrValue != null) return attrValue;
            }

            // Return an empty string
            return string.Empty;
        }
    }
}
