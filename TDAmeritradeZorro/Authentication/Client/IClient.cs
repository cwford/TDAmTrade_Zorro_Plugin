//*****************************************************************************
// File: IClient.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: Interface for using third-party authentication services.
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
using TDAmeritradeZorro.Authentication.Configuration;

namespace TDAmeritradeZorro.Authentication.Client
{
    //*************************************************************************
    //  Interface: IClient
    //
    /// <summary>
    /// An interface for using the authentication services of a third-party.
    /// </summary>
    /// 
    /// <remarks>
    /// TD Ameritrade authentication flow is:
    /// 
    /// (1) Client Id (secret key, consumer id( is used to generate a login link.
    /// (2) Hosting app (TD Ameritrade) renders page with generated login link.
    ///     In this plug-in that page is seen on a Windows Form with a browser
    ///     control.
    /// (3) User supplies username and password and clicks login link.
    /// (4) Redirection to TD Ameritrade authentication site
    /// (5) User authenticates and allows app access their account
    /// </remarks>
    //*************************************************************************
    public interface IClient
    {
        //**********************************************************************
        //  Member: Name
        //
        /// <summary>
        /// Unique friendly name of authentication service. 
        /// </summary>
        //**********************************************************************
        string Name { get; }

        //**********************************************************************
        //  Member: Configuration
        //
        /// <summary>
        /// Client configuration object.
        /// </summary>
        //**********************************************************************
        IClientConfiguration Configuration { get; }
    }
}