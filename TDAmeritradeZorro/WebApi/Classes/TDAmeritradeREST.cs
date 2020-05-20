//*****************************************************************************
// File: TDAmeritradeREST.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: Submit HTTP requests to the TD Ameritrade system through its
// API, and received responses.
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
using System.Linq;
using System.Net;
using TDAmeritradeZorro.Classes;
using TDAmeritradeZorro.Utilities;

namespace TDAmeritradeZorro.WebApi.Classes
{
    //************************************************************************
    //  Class: TDAmeritradeREST
    //
    /// <summary>
    /// Class that instantiates a query method to access all of the TD
    /// Ameritrade REST API methods.
    /// </summary>
    //************************************************************************
    public class TDAmeritradeREST
    {
        #region CLASS MEMBERS
        //*********************************************************************
        //  Member: BaseAddress
        //
        /// <summary>
        /// The base server address for the Web API
        /// </summary>
        //*********************************************************************
        private const string BaseAddress = "https://api.tdameritrade.com/v1";
        #endregion CLASS MEMBERS

        #region CLASS CONSTRUCTOR
        //*********************************************************************
        //  Constructor: TDAmeritradeREST
        //
        /// <summary>
        /// Constructor for the TD Ameritrade REST web API.
        /// </summary>
        //*********************************************************************
        public TDAmeritradeREST() { }
        #endregion CLASS CONSTRUCTOR

        #region PUBLIC METHODS
        //*********************************************************************
        //  Method: QueryApi
        //
        /// <summary>
        /// Access a TD Ameritrade Web API method
        /// </summary>
        /// 
        /// <param name="apiMethod">
        /// Enum for method being called.
        /// </param>
        /// 
        /// <param name="Params">
        /// Calling parameters.
        /// </param>
        /// 
        /// <returns>
        /// The response from the TD Ameritrade API as a string.
        /// </returns>
        //*********************************************************************
        public string
            QueryApi
            (
                ApiMethod apiMethod,
                object[] Params
            )
        {
            // The content returned to the caller
            string resultContent = null;

            // The URI called on the TD Ameritrade REST API
            string reqUri = string.Empty;

            try
            {
                // Get URI = base + method URI.
                reqUri = BaseAddress + apiMethod.GetAttribute("Name");

                // Are we adding a query string to the URI?
                if (Params[0] != null)

                    // YES: Add the query string after adding a "?"
                    reqUri += "?" + Params[0].ToString();

                // Add account id (param 4), if necessary
                if (Params[4] != null)
                    reqUri = reqUri.Replace("{account_id}", Params[4].ToString());

                // Add order id (param 5), if necessary
                if (Params[5] != null)
                    reqUri = reqUri.Replace("{order_id}", Params[5].ToString());

                // Create a new http web request object with the URI
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(reqUri);
                
                // Set the METHOD for the request in 'Prompt' attribute
                request.Method = apiMethod.GetAttribute("Prompt");

                // Add the content type of this query, in 'GroupName' attribute
                request.ContentType = apiMethod.GetAttribute("GroupName");

                // Add the content length header, if raw data is being sent
                if (Params[2] != null && Params[2].ToString().Length > 0)
                {
                    request.ContentLength = Params[2].ToString().Length;
                }

                // Request accepts JSON data, which is returned from TDA
                request.Accept = "application/json";

                // Add the authorization header, if included
                if (Params[3] != null)
                    request.Headers.Add("Authorization: Bearer " + Params[3].ToString());

                // Set the request time out at 10 minutes (in miliseconds)
                request.Timeout = 10 * 60 * 1000;

                // Need to send url-encoded form content or raw JSON content in
                // the request BODY?
                if (Params[1] != null || Params[2] != null)
 
                    // YES: Send it by writing to the request stream
                    using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                    {
                        // If sending url-encoded data, write it to the stream
                        if (Params[1] != null)
                            sw.Write(Params[1].ToString());
                        else
                            // If sending raw JSON data, write it to the stream
                            if (Params[2] != null)
                                sw.Write(Params[2].ToString());

                        // Close the stream writer
                        sw.Close();
                    }

                // Create and retrieve the web response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Was a 200 status code returned?
                    if (response.StatusCode == HttpStatusCode.OK ||
                        response.StatusCode == HttpStatusCode.Created ||
                        response.StatusCode == HttpStatusCode.Accepted)
                    {
                        // YES: Create a response stream
                        using (Stream stream = response.GetResponseStream())
                        {
                            // Create a stream reader to read the response stream
                            using (StreamReader sr = new StreamReader(stream))
                            {
                                // Read the response stream 
                                resultContent = sr.ReadToEnd();

                                // Close the stream reader
                                sr.Close();
                            }
                        }

                        // Is this a request for market hours?
                        if (apiMethod == ApiMethod.GetMarketHours)
                        {
                            // YES: Get the server GMT time
                            DateTime serverGMTTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(response.Headers["Date"]));

                            // Convert to OLE format
                            double oleDateTime = serverGMTTime.ToOADate();

                            // Combine server time and result content
                            resultContent = $"{oleDateTime}##{resultContent}";
                        }
                        else
                        // Was an order just placed?
                        if (apiMethod == ApiMethod.PlaceOrder)
                        {
                            // YES: The order number is the last URL segment,
                            // get it. First create a new URI.
                            Uri uri = new Uri(response.Headers.GetValues("Location")[0]);

                            // Place the last segment of the above URI into the content
                            resultContent = uri.Segments.Last();
                        }
                    }
                    else
                    {
                        // NO: Return the status error description
                        resultContent = "ERROR: " + response.StatusDescription;
                    }
                }
            }
            catch(Exception e)
            {
                // Error has occurred, capture the error message in result
                resultContent = "ERROR: " + e.Message;
            }

            // Return the result to the caller
            return resultContent;
        }
        #endregion PUBLIC METHODS
    }
}