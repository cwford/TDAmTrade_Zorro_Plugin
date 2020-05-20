//*****************************************************************************
// File: IClientConfiguration.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The client configuration interface object.
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
    //  Interface: IClientConfiguration
    //
    /// <summary>
    /// Interface inforamtion for the authentication of a service client.
    /// </summary>
    //*************************************************************************
    public interface IClientConfiguration
    {
        //*********************************************************************
        // Property: AuthUri
        //
        /// <summary>
        /// Auth code URI, needed for TD Ameritrade to return the auth code.
        /// </summary>
        //*********************************************************************
        string AuthUri { get; }

        //*********************************************************************
        // Property: RefreshToken
        //
        /// <summary>
        /// The Refresh token used to obtain a new access token.
        /// </summary>
        //*********************************************************************
        string RefreshToken { get; set;  }

        //*********************************************************************
        // Property: ClientId
        //
        /// <summary>
        /// Client Id (ID, consumer key, of your TDAmeritrade API application).
        /// </summary>
        //*********************************************************************
        string ClientId { get; }

        //*********************************************************************
        // Property: IsEnabled
        //
        /// <summary>
        /// True if client state (connection to TD Ameritrade) is enabled,
        /// false if it is not.
        /// </summary>
        //*********************************************************************
        bool IsEnabled { get; }
    }
}