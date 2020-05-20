//*****************************************************************************
// File: AuthForm.cs
//
// Author: Clyde W. Ford
//
// Date: May 1, 2020
//
// Description: The Authorization Form (Windows Forms) used to interact with
// TD Ameritrade to get the firs access token.
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
using Microsoft.Win32;
using System;
using System.Collections.Specialized;
using System.Web;
using System.Windows.Forms;
using TDAmeritradeZorro.Classes;
using TDAmeritradeZorro.Utilities;

namespace TDAmeritradeZorro.WebApi
{
    //*************************************************************************
    //  Class: AuthForm
    //
    /// <summary>
    /// Create a Windows From that holds a Web Browser control that can be used
    /// to interact with the TD Ameritrade HTML servers to interactively allow
    /// the user to get the first access token.
    /// </summary>
    //*************************************************************************
    public partial class AuthForm : Form
    {
        #region CLASS MEMBERS
        //*********************************************************************
        //  Member: redirectUri
        //
        /// <summary>
        /// Holds the URL that TD Ameritrade will redirect to with the authen-
        /// tication code is the query string of address on the address bar.
        /// </summary>
        /// 
        /// <remarks>
        /// The plug-in uses a redirect URL of http://127.0.0.1.
        /// </remarks>
        //*********************************************************************
        private String redirectUri;

        //*********************************************************************
        //  Member: query
        //
        /// <summary>
        /// Holds the query string returned when an authenticate code is found.
        /// </summary>
        /// 
        /// <remarks>
        /// Query string is held as a name-value pair collection.
        /// </remarks>
        //*********************************************************************
        private NameValueCollection query;
        public NameValueCollection Query
        {
            get { return query; }
        }
        #endregion CLASS MEMBERS

        #region CONSTRUCTOR
        //*********************************************************************
        //  Constructor: AuthForm
        //
        /// <summary>
        /// The constructor used to create the Windows Form that holds the
        /// browser control for interacting with the TD Ameritrade HTML servers.
        /// </summary>
        /// 
        /// <param name="loginLinkUri">
        /// The URL that causes TD Ameritrade to display an authentification
        /// login screen.
        /// </param>
        /// 
        /// <param name="redirectUri">
        /// Redirect URL used when sending back a page that has a '404 Error' 
        /// but whose address bar contaain an authentication code.
        /// </param>
        //*********************************************************************
        public AuthForm
            (
            string loginLinkUri, 
            string redirectUri
            )
        {
            // Normal component initializatin
            InitializeComponent();

            // Center the Form
            this.CenterToScreen();

            // Save the redirecty URL
            this.redirectUri = redirectUri;

            // Create a web browser control that opens the login page from TD
            // Ameritrade
            this.webBrowser.Url = new Uri(loginLinkUri);

            // Set an event handler to handle the web browser navigating to any
            // new page
            this.webBrowser.Navigated += WebBrowser_Navigated;
        }
        #endregion CONSTRUCTOR

        #region PRIVATE METHODS
        //*********************************************************************
        //  Method: WebBrowser_Navigated
        //
        /// <summary>
        /// Event handler invoked whenever the web browser control navigates to
        /// a new page
        /// </summary>
        /// 
        /// <param name="sender">
        /// The web browser control giving rise to this event.
        /// </param>
        /// 
        /// <param name="e">
        /// The event arguments
        /// </param>
        //*********************************************************************
        private void 
            WebBrowser_Navigated
            (
            object sender, 
            WebBrowserNavigatedEventArgs e
            )
        {
            // Get the query string as a name/value pair collection
            NameValueCollection query = HttpUtility.ParseQueryString(e.Url.Query);

            // Is this the browser navigating to the '404 Error' page, where
            // the authentication code is in the URL?
            if (e.Url.AbsoluteUri.StartsWith(this.redirectUri) && query["code"] != null)
            {
                // YES: Save the query
                this.query = query;

                // Set a success dialog result
                this.DialogResult = DialogResult.OK;

                // Close this web browser dialog
                this.Close();
            }
        }


        //*********************************************************************
        //  Method: EnsureBrowserEmulationEnabled
        //
        /// <summary>
        /// Modify the Windows Registry so that browser emulation supports
        /// modern browsers like IE Edge.
        /// </summary>
        /// 
        /// <param name="exename">
        /// The name of the .exe file, which this method is registered with.
        /// </param>
        /// 
        /// <param name="uninstall">
        /// TRUE to install the modifications in the registry, FALSE to unin-
        /// stall them.
        /// </param>
        //*********************************************************************
        private void
            EnsureBrowserEmulationEnabled
            (
            string exename = "Zorro.exe",
            bool uninstall = false
            )
        {
            try
            {
                // Get the registry subkey for browser emulation
                using (
                    var rk = Registry.CurrentUser.OpenSubKey(
                            @"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true)
                            //@"SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true)
                )
                {
                    // Are we installing expanded emulation?
                    if (!uninstall)
                    {
                        // YES: Get the current value
                        dynamic value = rk.GetValue(exename);

                        // Change the current value of the key, if needed
                        if (value == null)
                            rk.SetValue(exename, (uint)11001, RegistryValueKind.DWord);
                    }
                    else
                        // NO: Uninstall the key's value in the registery
                        rk.DeleteValue(exename);
                }
            }
            catch (Exception e)
            {
                // Log the error in the log file only
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("ENABLING_BROWSER_EMULATION")}. " + e.Message);
            }
        }
        #endregion PRIVATE METHODS
    }
}