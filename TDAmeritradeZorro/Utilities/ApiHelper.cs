//*****************************************************************************
// File: ApiHelper.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: Helper class for TD Ameritrade API requests.
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
using TDAmeritradeZorro.Authentication;

namespace TDAmeritradeZorro.Utilities
{
    //*************************************************************************
    //  Class: ApiHelper
    //
    /// <summary>
    /// A helper class for the Web API functions, which builds the JSON or
    /// url-encoded payload for API requests.
    /// </summary>
    //*************************************************************************
    public static class ApiHelper
    {
        //*********************************************************************
        //  Method: JsonRawData
        //
        /// <summary>
        /// Create a Web API data object for raw data sent in the body of the
        /// request.
        /// </summary>
        /// 
        /// <param name="Data">
        /// Json data.
        /// </param>
        /// 
        /// <returns>
        /// Web API data object with Json data as the raw data parameter.
        /// </returns>
        //*********************************************************************
        public static object[]
            JsonRawData
            (
            string Data,
            string AccountId,
            bool UseAccessToken
            )
        {
            // Return a data object for the Web API call
            return new object[]
            {
                // Header data
                null,

                // www-form-urlencoded-data
                null,

                // Raw data
                Data,

                // Get an access token, if needed
                UseAccessToken ? AuthToken.GetAuthToken().AccessToken : null,

                // Account id
                AccountId,

                // Order id
                null
            };
        }

        //*********************************************************************
        //  Method: JsonUrlEncodedData
        //
        /// <summary>
        /// Create a Web API data object for raw data sent in the header of the
        /// request.
        /// </summary>
        /// 
        /// <param name="Data">
        /// Json data.
        /// </param>
        /// 
        /// <returns>
        /// Web API data object with Json data as the url-encoded data 
        /// parameter.
        /// </returns>
        //*********************************************************************
        public static object[]
            UrlEncodedData
            (
            string Data,
            bool UseAccessToken
            )
        {
            // Return a data object for the Web API call
            return new object[]
            {
                // Header data
                null,

                // www-form-urlencoded-data
                Data,

                // Raw data
                null,

                // Get an access token, if needed
                UseAccessToken ? AuthToken.GetAuthToken().AccessToken : null,

                // Account id
                null,

                // Order id
                null
            };
        }

        //*********************************************************************
        //  Method: AccountData
        //
        /// <summary>
        /// Create a Web API data object for id data sent in the querystring of
        /// the request.
        /// </summary>
        /// 
        /// <param name="Data">
        /// Id data.
        /// </param>
        /// 
        /// <returns>
        /// Web API data object with string data as the Id data parameter.
        /// </returns>
        //*********************************************************************
        public static object[]
            AccountDataWithQueryString
            (
            string Data,
            string QueryString,
            bool UseAccessToken
            )
        {
            // Return a data object for the Web API call
            return new object[]
            {
                // Header data
                QueryString,

                // www-form-urlencoded-data
                null,

                // Raw data
                null,

                // Get an access token, if needed
                UseAccessToken ? AuthToken.GetAuthToken().AccessToken : null,

                // Account id
                Data,

                // Order id
                null
            };
        }

        //*********************************************************************
        //  Method: AccountOrderData
        //
        /// <summary>
        /// Create a Web API data object for id data sent in the querystring of
        /// the request.
        /// </summary>
        /// 
        /// <param name="Data">
        /// Id data.
        /// </param>
        /// 
        /// <returns>
        /// Web API data object with string data as the Id data parameter.
        /// </returns>
        //*********************************************************************
        public static object[]
            AccountOrderData
            (
            string AccountData,
            string OrderData,
            bool UseAccessToken
            )
        {
            // Return a data object for the Web API call
            return new object[]
            {
                // Header data
                null,

                // www-form-urlencoded-data
                null,

                // Raw data
                null,

                // Get an access token, if needed
                UseAccessToken ? AuthToken.GetAuthToken().AccessToken : null,

                // Account id
                AccountData,

                // Order id
                OrderData
            };
        }
    }
}
