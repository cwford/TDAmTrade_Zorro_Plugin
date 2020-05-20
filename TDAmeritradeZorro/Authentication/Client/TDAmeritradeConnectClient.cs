//*****************************************************************************
// File: TDAmeritradeConnectClient.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: Implementation of the IClientConfiguation for the TD Ameritrade
// API.
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
    //  Class: TDAmeritradeConnectClient
    //
    /// <summary>
    /// TDAmeritrade Connect authentication class, implementing the IClient
    /// interface.
    /// </summary>
    //*************************************************************************
    public class TDAmeritradeConnectClient : IClient
    {
        #region PRIVATE AND PUBLIC MEMBERS
        //*********************************************************************
        //  Member: configuration
        //
        /// <summary>
        /// The configuration information for this class instance, which has
        /// the client id and the refresh token.
        /// </summary>
        //*********************************************************************
        private IClientConfiguration configuration;

        //*********************************************************************
        //  Member: Configuration
        //
        /// <summary>
        /// The interface implementation of the configuation information.
        /// </summary>
        //*********************************************************************
        public IClientConfiguration Configuration
        {
            get
            {
                return configuration;
            }
        }

        //*********************************************************************
        //  Member: Name
        //
        /// <summary>
        /// The name of this IClient implementation (TD Ameritrade)
        /// </summary>
        //*********************************************************************
        public string Name
        {
            get
            {
                return "TD Ameritrade Connect";
            }
        }
        #endregion PRIVATE AND PUBLIC MEMBERS

        #region CONSTRUCTOR
        //*********************************************************************
        //  Ctor: TDAmeritradeConnectClient
        //
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="TDAmeritradeConnectClient"/> class.
        /// </summary>
        /// 
        /// <param name="configuration">The configuration.</param>
        //*********************************************************************
        public TDAmeritradeConnectClient
            (
            IClientConfiguration configuration
            ) 
        {
            // Save the configuration information to this class instance
            this.configuration = configuration;
        }
        #endregion CONSTRUCTOR

    }
}
