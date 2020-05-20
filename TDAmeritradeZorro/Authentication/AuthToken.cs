//*****************************************************************************
// File: AuthToken.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: Class structure and methods for obtaining authorization and
// refresh token required for interacting with the TD Ameritrade API.
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
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Web;
using TDAmeritradeZorro.Classes;
using TDAmeritradeZorro.Utilities;
using TDAmeritradeZorro.WebApi;
using TDAmeritradeZorro.WebApi.Classes;

namespace TDAmeritradeZorro.Authentication
{
    //*************************************************************************
    //  Class: AuthToken
    //
    /// <summary>
    /// This is the class used to retrieve access and refresh tokens using the
    /// TD Ameritrade API, and store them locally for use with the the various
    /// methods of this plug-in.
    /// </summary>
    /// 
    /// <remarks>
    /// NATE: The class is under a DataContract because its properties are
    /// populated by using a JSON deserializer to convert a string JSON object,
    /// obtained from using TD Ameritrade API, into a C# classs object. The
    /// string JSON object is the response to a query against the TD Ameritrade
    /// API and contains the access and refresh tokens, and information about
    /// them.
    /// </remarks>
    //*************************************************************************
    [DataContract]
    public class AuthToken
    {
        #region CLASS PROPERTIES
        //*********************************************************************
        //  Property: AccessToken
        //
        /// <summary>
        /// The access token used by most of the methods of this plug-in, when
        /// requesting services from the TD Ameritrade API.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        //*********************************************************************
        //  Property: AccessTokenExpiresIn
        //
        /// <summary>
        /// The number of seconds in which the access token expires.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "expires_in")]
        public long AccessTokenExpiresIn { get; set; }

        //*********************************************************************
        //  Property: TokenType
        //
        /// <summary>
        /// The access token type, which will always be 'Bearer.'
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        //*********************************************************************
        //  Property: Scope
        //
        /// <summary>
        /// Which TD Ameritrade functions the access token grants access to,
        /// for normal users:
        /// 
        /// PlaceTrades Account Access MoveMoney
        /// 
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        //*********************************************************************
        //  Property: RefreshToken
        //
        /// <summary>
        /// A refresh token used to obtain an access token from the TD
        /// Ameritrade API.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        //*********************************************************************
        //  Property: RefreshTokenExpiresIn
        //
        /// <summary>
        /// The number of seconds in which the refresh token expires.
        /// </summary>
        //*********************************************************************
        [DataMember(Name = "refresh_token_expires_in")]
        public long RefreshTokenExpiresIn { get; set; }

        //*********************************************************************
        //  Property: AccessTokenExpiresAt
        //
        /// <summary>
        /// The date and time at which the access token expires.
        /// </summary>
        //*********************************************************************
        public DateTime AccessTokenExpiresAt { get; set; }

        //*********************************************************************
        //  Property: RefreshTokenExpiresAt
        //
        /// <summary>
        /// The date and time at which the refresh token expires.
        /// </summary>
        //*********************************************************************
        public DateTime RefreshTokenExpiresAt { get; set; }
        #endregion CLASS PROPERTIES

        #region CLASS MEMBERS
        //*********************************************************************
        //  Member: rest
        //
        /// <summary>
        /// The TD Ameritrade REST API object used to communicate with the TD
        /// Ameritrade broker.
        /// </summary>
        //*********************************************************************
        public static TDAmeritradeREST rest;
        #endregion CLASS MEMBERS

        #region CLASS METHODS (PUBLIC)
        //*********************************************************************
        //  Method: Save
        //
        /// <summary>
        /// Save the current token data in an encrypted storage file.
        /// </summary>
        /// 
        /// <param name="clientId">
        /// The client id which is used as the key for storing the tokens in
        /// protected file storage.
        /// </param>
        //*********************************************************************
        public void 
            Save
            (
                string clientId
            )
        {
            // Compute the expiration times for the access
            AccessTokenExpiresAt = DateTime.UtcNow.AddSeconds(AccessTokenExpiresIn);

            // Compute the expiration times for the refresh token
            RefreshTokenExpiresAt = DateTime.UtcNow.AddSeconds(RefreshTokenExpiresIn);

            // Write the refresh token to encrypted storage file
            Crypto.EncryptAndSave(
                // Client Id (Used as key)
                clientId,

                // Refresh token to store
                string.Format("{0}##{1}##{2}##{3}##{4}##{5}", 
                    AccessToken, RefreshToken, AccessTokenExpiresAt.ToString("s"), RefreshTokenExpiresAt.ToString("s"),
                    AccessTokenExpiresIn.ToString(), RefreshTokenExpiresIn.ToString()),

                // File to store refresh token in
                Broker.tokenDataFile);

        }
        #endregion CLASS METHODS (PUBLIC)

        #region STATIC METHODS (PUBLIC)
        //*********************************************************************
        //  Method: AuthenticateUser
        //
        /// <summary>
        /// Authenticate the plug-in with TD Ameritrade and return an initial
        /// authentication and refresh token.
        /// </summary>
        /// 
        /// <returns>
        /// An authentication token.
        /// </returns>
        /// 
        /// <remarks>
        /// The first user authentication requires an interaction with the TD
        /// Ameritrade HTML servers and the submission of a username and pass-
        /// word. This is handled by creating a web browser control on a
        /// Windows form and capturing the TD Ameritrade pages on that web
        /// browser control.
        /// </remarks>
        //*********************************************************************
        public static AuthToken
            AuthenticateUser
            (
            string clientId
            )
        {
            // Method members
            string authCode = string.Empty;
            string postData = string.Empty;
            string apiResult = string.Empty;

            try
            {
                // Create the initial authentication form for the user from TD
                // Ameritrade
                AuthForm authForm = new AuthForm(ApiMethod.GetAuthCode
                    .GetAttribute("Name")
                    .Replace("{account_id}", clientId), "https://127.0.0.1");

                // Show the authentication form
                authForm.ShowDialog();

                // Destroy the Windows Form Icon
                bool bDestroy = Helper.DestroyIcon(authForm.icon.Handle);
                if (!bDestroy) LogHelper.Log(LogLevel.Error, $"{Resx.GetString("WINDOWS_FORM_ICON_NOT_DESTROYED")}");

                // Once here, the dialog control will be closed and the code 
                // needed to request the first access token will be in the 
                // querystring
                if (authForm.Query != null && !string.IsNullOrEmpty(authForm.Query["code"]))
                {
                    // Get the authorization code, which will be automatically
                    // url decoded
                    authCode = authForm.Query["code"];

                    // Authorization code is present in the query string. Form
                    // the url-encoded post data to retrieve an access token
                    postData = $"grant_type=authorization_code&refresh_token=&access_type=offline&code={HttpUtility.UrlEncode(authCode)}";
                    postData += $"&client_id={HttpUtility.UrlEncode(clientId)}&redirect_uri={HttpUtility.UrlEncode("http://127.0.0.1")}";

                    // Get a TD Ameritrade REST API interface object, if needed
                    if (rest == null) rest = new TDAmeritradeREST();

                    // Execute a query to the TD Ameritrade API for an access 
                    // token
                    apiResult = rest.QueryApi(

                            // The method on the TD Ameritrade REST API being 
                            // called
                            ApiMethod.PostAccessToken,

                            // The data helper method
                            ApiHelper.UrlEncodedData(

                                // The encoded form data
                                postData,

                                // Not using authentication in order to receive
                                // the first access token, which will then be
                                // used whenever subsequent authentication is
                                // needed
                                false
                                )
                            );

                    // Was an error encountered?
                    if (!apiResult.StartsWith("ERROR:"))
                    {
                        // NO: Get a json serializer based on the Token
                        // Model class
                        AuthToken token = Broker.DeserializeJson<AuthToken>(apiResult);

                        // Write the refresh token to encrypted file storage
                        token.Save(clientId);

                        // Return the refresh token
                        return token;
                    }
                }
            }
            catch (Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("GETTING_ACCESS_TOKEN")}. " + e.Message);

                // Return an empty access token
                return null;
            }


            // Log any errors
            LogHelper.Log(LogLevel.Error, apiResult);

            // Return an empty access token
            return null;
        }

        //*********************************************************************
        //  Method: GetAuthToken
        //
        /// <summary>
        /// Get an access token used to authenticate for any of the TD Ameri-
        /// trade API methods that need authentication.
        /// </summary>
        /// 
        /// <param name="clientId">
        /// The client id which is used as the key for storing the tokens in
        /// protected file storage.
        /// </param>
        /// 
        /// <returns>
        /// A valid TD Ameritrade acces token.
        /// </returns>
        //*********************************************************************
        public static AuthToken
            GetAuthToken
            ()
        {
            // Method members
            AuthToken token;
            string refreshToken = Broker.oAuthConfiguration.RefreshToken;
            string result = null;
            string[] keyParts;


            // Does the encrypted data file of tokens exist?
            if (File.Exists(Broker.tokenDataFile))
            {
                // YES: Get the stored keys
                result = Crypto.RetrieveAndDecrypt(Broker.oAuthConfiguration.ClientId, Broker.tokenDataFile);

                // Does data exist?
                if (!string.IsNullOrEmpty(result))
                {
                    // YES: Split apart the stored data at the double hash signs
                    keyParts = Helper.SeparateCreds(result);

                    // Are there 6 key parts?
                    if (keyParts.Length == 6)
                    {
                        // YES: Create a new authentication token
                        token = new AuthToken
                        {
                            AccessToken = keyParts[0],
                            RefreshToken = keyParts[1],
                            AccessTokenExpiresAt = DateTime.Parse(keyParts[2]),
                            RefreshTokenExpiresAt = DateTime.Parse(keyParts[3]),
                            AccessTokenExpiresIn = Convert.ToInt64(keyParts[4]),
                            RefreshTokenExpiresIn = Convert.ToInt64(keyParts[5]),
                            TokenType = "access_token"
                        };

                        // Set a new refresh token
                        refreshToken = token.RefreshToken;

                        // Is this authentication token expired, or about to expire?
                        if (token.AccessTokenExpiresAt.Subtract(DateTime.UtcNow) >
                            new TimeSpan(0, 5, 0))
                        {
                            // NO: Return the authentication token
                            return token;
                        }
                    }
                }
            }

            // If code reaches here, then we need a new set of tokens. Use the
            // refresh token to get them
            token = GetNewAuthToken(refreshToken, Broker.oAuthConfiguration.ClientId);

            // Return the token set
            return token;
        }
        #endregion STATIC METHODS (PUBLIC)

        #region STATIC METHODS (PRIVATE)
        //*********************************************************************
        //  Method: GetNewAuthToken
        //
        /// <summary>
        /// Get a new set of access and refresh tokens, along with a new set of
        /// expiration dates.
        /// </summary>
        /// 
        /// <param name="refreshToken">
        /// The refresh token used to get a new access token. NOTE: A new 
        /// refresh token will also be returned.
        /// </param>
        /// 
        /// <param name="clientId">
        /// The client id which is used as the key for storing the tokens in
        /// protected file storage.
        /// </param>
        /// 
        /// <returns>
        /// A valid TD Ameritrade acces token.
        /// </returns>
        //*********************************************************************
        private static AuthToken
            GetNewAuthToken
            (
            string refreshToken,
            string clientId
            )
        {
            // Method member
            string apiResult = string.Empty;

            try
            {
                // Create a TD Ameritrade REST API object, if needed
                if (rest == null) rest = new TDAmeritradeREST();

                // Create the post data for the REST API call
                string postData = string.Format(
                    // The form data template, each pair separated with an '&'
                    "grant_type={0}&refresh_token={1}&access_type={2}&client_id={3}",

                    // Type of grant being asked of TD Ameritrade API
                    HttpUtility.UrlEncode("refresh_token"),

                    // Refresh token used to request access token
                    HttpUtility.UrlEncode(refreshToken),

                    // Access type specified as 'offline' so TD Ameritrade also
                    // returns a new refresh token
                    HttpUtility.UrlEncode("offline"),

                    // The client Id used for requesting an access token
                    HttpUtility.UrlEncode(clientId));

                // Execute a query for the access token
                apiResult = rest.QueryApi(

                        // The method on the TD Ameritrade REST API being called
                        ApiMethod.PostAccessToken,

                        // The data helper method
                        ApiHelper.UrlEncodedData(
                            // The encoded form data
                            postData,

                            // Not using authentication
                            false
                            )
                        );

                // Was an error encountered?
                if (!apiResult.StartsWith("ERROR:"))
                {
                    // NO: Get a json serializer based on the Token
                    // Model class
                    AuthToken token = Broker.DeserializeJson<AuthToken>(apiResult);

                    // Write the refresh token to encrypted file storage
                    token.Save(clientId);

                    // Return the refresh token
                    return token;
                }
            }
            catch(Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, $"    {Resx.GetString("ERROR")}: {Resx.GetString("GETTING_NEW_ACCESS_TOKEN")}. " + e.Message);

                // Return an empty access token
                return null;
            }

            // Log any errors
            LogHelper.Log(LogLevel.Error, apiResult);

            // Return an empty access token
            return null;
        }
        #endregion STATIC METHODS (PRIVATE)
    }
}
