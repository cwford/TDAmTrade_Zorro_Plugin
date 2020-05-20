//*****************************************************************************
// File: Helper.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: Helper methods static class.
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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using TDAmeritradeZorro.Classes;
using DBLib.Classes;
using TDAmeritradeZorro.Classes.TDA;

namespace TDAmeritradeZorro.Utilities
{
    //*************************************************************************
    //  Class: Helper
    //
    /// <summary>
    /// A static class used for helper functions.
    /// </summary>
    //*************************************************************************
    public static class Helper
    {
        // The separator characters used for breaking apart the usernam/refresh
        // token pair and the password/access token pair
        private static readonly string SEP = ";#|.";

        //*********************************************************************
        //  Members: iconLicenseStr, iconAuthStr
        //
        /// <summary>
        /// The base-64 icon strings for the Windows Forms license and authori-
        /// zation forms.
        /// </summary>
        //*********************************************************************
        private static string iconLicenseStr = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAQXSURBVFhH7ZZdiJRVGMefsx+uhuu6i/mBYLtuwyKGrOwaJYEXETMUBbUDiheVF5lIiohIQR+UVNtFgSmkFSFZwdJKZqUzIIgXIYJ2tyUqautikR/rarW6tvv6+78fw+bM7OuM6I3+4TfPOe975jzPOed5zzl218uls6kObEVQLVmD3cnMjyrQTxtmtsqhPM+zf53ZcXN2mrraBm9GSQEMYauDask6Q6czVaCfTzEvqVxA8rEPNhNVdkcqe81/isodeZzk8G8Y8Wtm4yAJu5xze9KZ5BT/KVIAdXDfjRDp61hfntl6TF4baIZ8ed4S/nQ/pYn89wHsMjgErIg9bs7tJohJlK2KKRxU4UZ1ZJK5aUJDxdoV0VB3KnMlLPfCNpboS+wnsBwWEMQr2Pdu1xLkiQFoOdZBn//AbBVB1d2xACSCuIz5PqjZdL6Sx2IDmHzhqj258/dZrGU7zIeHoDF87Yt6TeLIQO2sk5ctceSiPdt1oolnc6E2bDJae0LLKtijSoqCIgfWk7EfvL/6gDUfHfCcJz85rsIliLK8erjSNZBiVXpbMeKN0H6Y5/3wMk52+q0Q0/4w5gBo8D/EzsAIYY6SKvrPBJgGM2Aq1HkVzoahctgznKuN9hZ9CWqXE9N+CvNfULOa2AA+eqPVup5PfE7x6QJoNC1iw7vt3/bMa6CYk2bna9jm10LtSGX+wmh2pJbYAM5PGW/dS5t7GPruAhyGkzTrW/5xT/38Q+eCP5mdhRcE77VcxeRiA4gTS16J6ZzZ9492OuttnGjfLEusxvFXEOVITulsUtMU+T12ywGgNKwBd+LBSdb5Tpt9t3i2PrfC8lwTv9HZM3hLATD6JzDboaq3sdbr3NBmZ6cpP8eQ8xMz8vtr2QHgXEevTkCN5szGV+f91N9Qo1dx8pcq1MGyAsC5TreNoA1pAJ7rbar9AzumOrIpHWDPBDU7D/vKnYG18BQQi71Gsh3Uw7HEBqQd5S2K0aVlK1tzf8kB4FE3nzeB/vxPUSdcUaUzqXqca8/QFqxjXfqN4/5DFUoKAOf6hLpAmaZr1krIl3PbcdoHlwhTS7MLorX/hY6S3IouqHLTAeBcI94EuoToxrOCBzrrC6kedFXTYaTM1M6nC8mL9LOQu4KC96VOC4rDqJXD6BGV+dPPrFc7RW3JCvoz/qiLRU6MdhFmTlDL6Qp7/1EW/xT2T7bhvI3ppkQALdAP9GPHIXenu+3C2TjYHzq/BkqoOyccvh06F1vgf3lDndPXqkGB1oxC9dgcK5oDkjrB6PjUzZmq7QVdRHQAqXNZJVkCxof1yKkutcdgDU4O+09KFR4bQdMezUA56KAqqjFnQKID3elbg5o/C8rkyAqNVGe+Pklty3o3GfS56kr2BU70/J4KyOw6pTBm76maw9QAAAAASUVORK5CYII=";
        private static string iconAuthStr = "/9j/4AAQSkZJRgABAQEAYABgAAD/4QBoRXhpZgAATU0AKgAAAAgABAEaAAUAAAABAAAAPgEbAAUAAAABAAAARgEoAAMAAAABAAIAAAExAAIAAAARAAAATgAAAAAAAXbyAAAD6AABdvIAAAPocGFpbnQubmV0IDQuMi4xMAAA/9sAQwACAQEBAQECAQEBAgICAgIEAwICAgIFBAQDBAYFBgYGBQYGBgcJCAYHCQcGBggLCAkKCgoKCgYICwwLCgwJCgoK/9sAQwECAgICAgIFAwMFCgcGBwoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoK/8AAEQgAGAAYAwEhAAIRAQMRAf/EAB8AAAEFAQEBAQEBAAAAAAAAAAABAgMEBQYHCAkKC//EALUQAAIBAwMCBAMFBQQEAAABfQECAwAEEQUSITFBBhNRYQcicRQygZGhCCNCscEVUtHwJDNicoIJChYXGBkaJSYnKCkqNDU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6g4SFhoeIiYqSk5SVlpeYmZqio6Slpqeoqaqys7S1tre4ubrCw8TFxsfIycrS09TV1tfY2drh4uPk5ebn6Onq8fLz9PX29/j5+v/EAB8BAAMBAQEBAQEBAQEAAAAAAAABAgMEBQYHCAkKC//EALURAAIBAgQEAwQHBQQEAAECdwABAgMRBAUhMQYSQVEHYXETIjKBCBRCkaGxwQkjM1LwFWJy0QoWJDThJfEXGBkaJicoKSo1Njc4OTpDREVGR0hJSlNUVVZXWFlaY2RlZmdoaWpzdHV2d3h5eoKDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uLj5OXm5+jp6vLz9PX29/j5+v/aAAwDAQACEQMRAD8A8s+AP/BK79o/9pjwzoniL4UeLvhzdzeILZp9P0Sfx5ax6jtXcWDW2fMVgEZiMcKM9K2tU/4I8ftLaLqE2k6r8U/g7b3VtM0Nzb3HxU0+OSGRThkZWYFWBBBBGQRzX4TT4YzCth41lOnyy2vNLonb111XQ/mGjwfmdbCxxEalPlls3UiuidvVJq63R5p+1T+wZ+0z+xtDpOqfG3wRbw6Rr650TxBpOpw31henbu2rNCxAbbyFbBIBIyBmivHx2CxGX4mVCurSXzWqumn1TR4WYZfisrxcsNiFaSt1ummrpprdNHq3/BCdc/8ABUn4aNn7setH/wAo97X0b/wWb07S7f8A4J//AA5v7fT4Uubj4zeKjNcLCoeT/iZan1YDJ/GvrsrS/wBUcQ33n/7hPt8oUf8AUfFX7z/9wHK+Obi7v/8Ag3A8Jt43ZpHt/iUy+F2uiSwQXl0MR57AG5Ax2yKK8fiD4sLff2NO/wBz/Sx4PE13PCN7+wpX+52/Cx8Tfsp/tL+Ov2QPjvov7Qnw203TbvWtBW5Fnb6vC8lu3n20lu+5UdGOElYjDDnHXpXv0/8AwWU+LXiD4dWPwu+Jf7Mvwg8ZaTpusX2p2UPirwzcXfk3F1czXEjKGucD5p3UYGduASeSZy3iDEZbhXh1ThODbbUk3e/L5r+RW+ZGVcSYrKsHLCxpQnBttqabvzct+q/kVjzL9rf/AIKFfHb9sLw14f8Ah54z07w74d8IeF/m0Hwd4N0gWOnWr7SvmBNzEsFLKMthQzYA3Nkrz8wx9bMsU69WybskkrJJKySXZI8zM8yxGbYx4iskm7JJKySSskl0SR//2Q==";

        //*********************************************************************
        //  Method: SeparateCreds
        //
        /// <summary>
        /// Separate username/refresh and password/accesss token pairs.
        /// </summary>
        /// 
        /// <param name="inputValue">
        /// The input string eneterd into the username or password field
        /// </param>
        /// 
        /// <returns>
        /// A string array with the credential is the first element of the 
        /// array and the token in the second element.
        /// </returns>
        //*********************************************************************
        public static string[]
            SeparateCreds
            (
                string inputValue
            )
        {
            // Method members
            string SEP_CHAR = string.Empty;

            // Iterate through the separator characters
            for (int i = 0; i < SEP.Length; ++i)
            {
                // Form the separator character
                SEP_CHAR = SEP.Substring(i, 1);

                // Does separator character appear twice in the input value?
                if (inputValue.Contains(SEP_CHAR + SEP_CHAR))
                {
                    // YES: Use it to break-up the input value and return the
                    // separated values. The credential is the first element,
                    // the token is the second element.
                    string[] STR_SEP = new string[] { SEP_CHAR + SEP_CHAR };
                    return inputValue.Split(STR_SEP, StringSplitOptions.None);
                }
            }

            // Return a one-element string array with the given input value
            return new string[] { inputValue };
        }

        //*********************************************************************
        //  Method: GetWebPage
        //
        /// <summary>
        /// Get a web page from an HTML server.
        /// </summary>
        /// 
        /// <param name="url">
        /// The URL of the page to be retrieved.
        /// </param>
        /// 
        /// <returns>
        /// The web page as a string of HTML.
        /// </returns>
        //*********************************************************************
        public static string
            GetWebPage
            (
            string url
            )
        {
            // Method member
            string result;

            // Create a new web client
            var myClient = new WebClient();

            // Add the user-agent header so the request can go through
            myClient.Headers.Add("User-Agent: Other");

            // Get the response stream from opening and reading the URL
            using (Stream response = myClient.OpenRead(url))
            {
                // Create a stream reader to read the response
                StreamReader sr = new StreamReader(response);

                // Read the response
                result = sr.ReadToEnd();

                // Close the respones stream
                response.Close();
            }

            // Return the result of reading the response stream served as a
            // result of the original request
            return result;
        }

        //*********************************************************************
        //  Method ZorroToTDAssetType
        //
        /// <summary>
        /// Convert a Zorro asset type to a TD Ameritrade asset type
        /// </summary>
        /// 
        /// <param name="ZorroAssetType">
        /// The Zorro asset type.
        /// </param>
        /// 
        /// <returns>
        /// Formatted JSON string.
        /// </returns>
        //*********************************************************************
        public static string
            ZorroToTDAssetType
            (
            string ZorroAssetType
            )
        {
            // Do conversion via a switch statement
            switch(ZorroAssetType)
            {
                case "STK":
                case "ETF":
                    return "EQUITY";

                case "FUND":
                    return "MUTUAL_FUND";

                case "OPT":
                    return "OPTION";

                default:
                    return "";
            }
        }

        //*********************************************************************
        //  Method FormatJson
        //
        /// <summary>
        /// Formats a JSON string and returns beautified JSON.
        /// </summary>
        /// 
        /// <param name="json">
        /// The JSON string to format.
        /// </param>
        /// 
        /// <param name="indent">
        /// How much to indent each subordinate property group.
        /// </param>
        /// 
        /// <returns>
        /// Formatted JSON string.
        /// </returns>
        //*********************************************************************
        public static string
            FormatJson
            (
            string json,
            string indent = "  "
            )
        {
            var indentation = 0;
            var quoteCount = 0;
            var escapeCount = 0;
            json = json.Replace("\r\n", "");

            var result =
                from ch in json ?? string.Empty
                let escaped = (ch == '\\' ? escapeCount++ : escapeCount > 0 ? escapeCount-- : escapeCount) > 0
                let quotes = ch == '"' && !escaped ? quoteCount++ : quoteCount
                let unquoted = quotes % 2 == 0
                let colon = ch == ':' && unquoted ? ": " : null
                let nospace = char.IsWhiteSpace(ch) && unquoted ? string.Empty : null
                let lineBreak = ch == ',' && unquoted ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(indent, indentation)) : null
                let openChar = (ch == '{' || ch == '[') && unquoted ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(indent, ++indentation)) : ch.ToString()
                let closeChar = (ch == '}' || ch == ']') && unquoted ? Environment.NewLine + string.Concat(Enumerable.Repeat(indent, --indentation)) + ch : ch.ToString()
                select colon ?? nospace ?? lineBreak ?? (
                    openChar.Length > 1 ? openChar : closeChar
                );

            return string.Concat(result);
        }

        //*********************************************************************
        //  Method: GetWindowsFormIcon
        //
        /// <summary>
        /// Convert the base-64 string of the TD Ameritrade image into an icon
        /// for the Windows Form.
        /// </summary>
        //*********************************************************************
        public static Icon
            GetWindowsFormIcon
            (
            FormType formType
            )
        {
            // Get the correct icon base-64 string
            string iconStr = formType == FormType.License ? iconLicenseStr : iconAuthStr;

            // Convert icon string to byte array
            byte[] ba = Convert.FromBase64String(iconStr);

            // Convert from byte array to bit map
            Bitmap bm = (Bitmap)((new ImageConverter()).ConvertFrom(ba));

            // Convert bitmap into an icon handle
            IntPtr iHandle = bm.GetHicon();

            // Return the icon
            return Icon.FromHandle(iHandle);
        }

        //*********************************************************************
        //  Method: DestroyIcon
        //
        /// <summary>
        /// Win32 method to destroy the icon used by the Window Form.
        /// </summary>
        /// 
        /// <param name="handle">
        /// Handle of the icon to destroy.
        /// </param>
        /// 
        /// <returns>
        /// True if successful, false if not.
        /// </returns>
        //*********************************************************************
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static bool DestroyIcon
            (
            IntPtr handle
            );


    }
}
