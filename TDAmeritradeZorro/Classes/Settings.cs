//*****************************************************************************
// File: Settings.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The setting class object
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
using System.IO;
using System.Runtime.Serialization;
using TDAmeritradeZorro.Utilities;

namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Class: Settings
    //
    /// <summary>
    /// A class that encapsulates settings information entered by the user into
    /// a text file as JSON data, which is then converted to this C# object.
    /// </summary>
    //*************************************************************************
    [DataContract]
    public class Settings
    {
        //*********************************************************************
        //  Property: Currency
        //
        /// <summary>
        /// The account currency (USD, EUR, JPY) etc.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        //*********************************************************************
        //  Property: TdaAccountNum
        //
        /// <summary>
        /// The TD Ameritrade user account number.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "tdaAccountNum")]
        public string TdaAccountNum { get; set; }

        //*********************************************************************
        //  Property: clientId
        //
        /// <summary>
        /// The TD Ameritrade client id (consumer key) for the developer app
        /// related to the user.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "clientId")]
        public string ClientId { get; set; }

        //*********************************************************************
        //  Property: LangResx
        //
        /// <summary>
        /// The default language resource file specified as ll-CC, where ll =
        /// the language (i.e. en, es, de) and CC = the country (i.e. US, GB,
        /// MX).
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "langResx")]
        public string LangResx { get; set; }

        //*********************************************************************
        //  Property: testAssets
        //
        /// <summary>
        /// Ticker symbols for the asset used for testing the plug-in.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "testAssets")]
        public string testAssets { get; set; }

        //*********************************************************************
        //  Method: Read
        //
        /// <summary>
        /// Read user settings from the settings file
        /// </summary>
        /// 
        /// <returns>
        /// A Settings object.
        /// </returns>
        //*********************************************************************
        public static Settings
            Read
            ()
        {
            // Method members
            string settingsFile = Broker.WORKING_DIR + Broker.SETTINGS_FILE;
            string json = string.Empty;

            try
            {
                // Does the settings file exist?
                if (File.Exists(settingsFile))
                {
                    // YES: Iterate through each line of the settings file
                    foreach (string line in File
                        .ReadAllLines(Broker.WORKING_DIR + Broker.SETTINGS_FILE))
                    {
                        // Do not add line if it contains a double-slash or is blank
                        if (line.Contains("//") ||
                            string.IsNullOrEmpty(line.Trim())) continue;

                        // Add line to json string
                        json += line;
                    }

                    // Does json string have data?
                    if (!string.IsNullOrEmpty(json))

                        // YES: Convert to Settings class object and return
                        return Broker.DeserializeJson<Settings>(json);
                    else

                        // NO: Log this error
                        LogHelper.Log(LogLevel.Error, $"{Resx.GetString("ERROR")}: tda.json {Resx.GetString("INCORRECT_FORMAT")}.");
                }
                else
                {
                    // Settings file does not exist, log that error
                    LogHelper.Log(LogLevel.Critical, $"{Resx.GetString("ERROR")}: tda.json {Resx.GetString("FILE_NOT_FOUND")}.");
                }
            }
            catch(Exception e)
            {
                // A file I/O error occurred, log it
                LogHelper.Log(LogLevel.Critical, $"{Resx.GetString("ERROR")}: {Resx.GetString("FILE_IO_ERROR")} {Resx.GetString("READING").ToLower()} tda.json.");
            }

            // If we get here return a NULL class object
            return null;
        }
    }
}