//*****************************************************************************
// File: TDAmeritradeConnectConfiguration.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The TDAmeritradeConnectConfiguration class object, which 
// implements the IClientConfiguration interface.
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
namespace TDAmeritradeZorro.Authentication.Configuration
{
    //*************************************************************************
    //  Class: TDAmeritradeConnectConfiguration
    //
    /// <summary>
    /// Implementation of the IClientConfiguration interface for the TD 
    /// Ameritrade broker plug-in.
    /// </summary>
    //*************************************************************************
    public class TDAmeritradeConnectConfiguration : IClientConfiguration
    {
        //*********************************************************************
        //  Property: AuthUrl
        //
        /// <summary>
        /// URL for authentication.
        /// </summary>
        //*********************************************************************
        public string AuthUri
        {
            get
            {
                return "https://api.tdameritrade.com/v1/oauth2/token";
            }
        }

        //*********************************************************************
        //  Property: ClientId
        //
        /// <summary>
        /// TD Ameritrade client id (consumer key) for the app used to run
        /// the broker plug-in interface with Zorro.
        /// </summary>
        //*********************************************************************
        private string _ClientId;
        public string ClientId
        {
            get
            {
                return _ClientId;
            }
        }

        //*********************************************************************
        //  Property: RefreshToken
        //
        /// <summary>
        /// The Refresh token used to obtain an access token.
        /// </summary>
        //*********************************************************************
        private string _RefreshToken;
        public string RefreshToken
        {
            get
            {
                return _RefreshToken;
            }

            set
            {
                _RefreshToken = value;
            }
        }


        //*********************************************************************
        //  Property: IsEnabled
        //
        /// <summary>
        /// True if the TD Ameritrade API connection is enabled, false if not.
        /// </summary>
        //*********************************************************************
        public bool IsEnabled
        {
            get
            {
                return true;
            }
        }

        //*********************************************************************
        //  Constructor: TDAmeritradeConnectConfiguration
        //
        /// <summary>
        /// The class constructor.
        /// </summary>
        /// 
        /// <param name="clientId">
        /// The client id (consumer key) given by TD Ameritrade for the app
        /// established by the user for use with Zorro.
        /// </param>
        //*********************************************************************
        public TDAmeritradeConnectConfiguration
            (
            string clientId
            )
        {
            _ClientId = clientId;
        }
    }
}
