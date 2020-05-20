using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Class: LogHelper
    //
    /// <summary>
    /// A class to assist in showing log messages in the Zorro window and the
    /// log file.
    /// </summary>
    //*************************************************************************
    public static class LogHelper
    {
        //*********************************************************************
        //  Member: LINE_FORMATSTR
        //
        /// <summary>
        /// Format string for log entry with line numbers only.
        /// </summary>
        //*********************************************************************
        private readonly static string LINE_FORMATSTR = "[%sourcefile.%method (%linenumber)] %level - %message%newline";

        //*********************************************************************
        //  Member: TIME_FORMATSTR
        //
        /// <summary>
        /// Format string for log entry with time only.
        /// </summary>
        //*********************************************************************
        private readonly static string TIME_FORMATSTR = "%timestamp(yyyy-MM-dd HH:mm:ss.ff) %level - %message%newline";

        //*********************************************************************
        //  Member: TIME_LINE_FORMATSTR
        //
        /// <summary>
        /// Format string for log entry with both time and line numbers
        /// </summary>
        //*********************************************************************
        private readonly static string TIME_LINE_FORMATSTR = "%timestamp(yyyy-MM-dd HH:mm:ss.ff) [%sourcefile.%method (%linenumber)] %level - %message%newline";

        // Newline character
        private readonly static string NEWLINE = "\r\n";

        // Dictionary with string tokens and format token position
        private readonly static Dictionary<string, string> fmtDictionary = new Dictionary<string, string>
        {
            {"%timestamp", "{0}" },
            {"%sourcefile", "{1}" },
            {"%method", "{2}" },
            {"%linenumber", "{3}" },
            {"%level", "{4}" },
            {"%message", "{5}" },
        };

        // Regex to get the date format string
        private static Regex reDate = new Regex(@"%timestamp\((.*?)\)");

        // Regex for finding a numbered filename
        private static Regex reNum = new Regex(@"\((.*)\)");

        // The date format string
        private static string dateFmt;

        //*********************************************************************
        //  Method: ParseFormat
        //
        /// <summary>
        /// Parse the format string for a log entry.
        /// </summary>
        /// 
        /// <returns>
        /// A string format template that can be used to populate the string
        /// with values either passed with the call or inherent to OS.
        /// </returns>
        //*********************************************************************
        private static string
            ParseFormat
            (
                bool timeStamp,
                bool lineNumbers
            )
        {
            // Get the format string
            string fmtString = TIME_LINE_FORMATSTR;

            // Make sure correct format string is obtained
            if (timeStamp & !lineNumbers) fmtString = TIME_FORMATSTR;
            else if (lineNumbers & !timeStamp) fmtString = LINE_FORMATSTR;

            // Work around memory leak in Regex
            Regex.CacheSize = 0;

            // Look for the date format string
            Match m = reDate.Match(fmtString);

            // Remove the date format string, if found
            if (m.Success)
                fmtString = fmtString.Replace(m.Groups[0].Value, "%timestamp");

            // Replace the newline tokens
            fmtString = fmtString.Replace("%newline", NEWLINE);

            // Iterate through the format dictionary, and...
            foreach (string key in fmtDictionary.Keys)

                // Replace the key with its value in the format string
                fmtString = fmtString.Replace(key, $"{fmtDictionary[key]}");

            // Return the format string
            return fmtString;
        }

        //*********************************************************************
        //  Method: Log
        //
        /// <summary>
        /// Log a message.
        /// </summary>
        /// 
        /// <param name="text">
        /// Text of message to log.
        /// </param>
        /// 
        /// <param name="file">
        /// Autopopulated with the source file from which this method was
        /// called.
        /// </param>
        /// 
        /// <param name="method">
        /// Autopopulated with the method withing the source file from which
        /// this method was called.
        /// </param>
        /// 
        /// <param name="line">
        /// Autopopulated with the line number of the source file from which 
        /// this method was called.
        /// </param>
        /// 
        /// <remarks>
        /// Based on the current verbosity level, determines if a message is 
        /// shown and whether it is display in the Zorro Window, the log file,
        /// or both.
        /// </remarks>
        //*********************************************************************
        public static void
            Log
            (
            LogLevel level,
            string text,
            [CallerFilePath] string file = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int line = 0
            )
        {
            // Basic verbosity flags
            bool byPass = false;
            bool logOnly = false;

            // Determine if the verbosity level includes flags for adding time
            // stamp and line number to messages printed to log.
            bool timeStamp = TDAmerAPI.verbosityLevel.HasFlag(Verbosity.TimeStamp);
            bool lineNumbers = TDAmerAPI.verbosityLevel.HasFlag(Verbosity.LineNumbers);

            // Remove the timestamp and line number flags from the verbosity
            // level of the plug-in
            Verbosity vLevel = TDAmerAPI.verbosityLevel & ~Verbosity.TimeStamp;
            vLevel &= ~Verbosity.LineNumbers;

            // Switch based on the current verbosity level
            switch (vLevel)
            {
                // Only message level severe to show in Zorro Window and file
                // Message level Warning and Error show in log file only.
                // Anything else does not show at all
                case Verbosity.Basic:
                    switch (level)
                    {
                        case LogLevel.Error:
                        case LogLevel.Warning:
                            logOnly = true;
                            break;

                        case LogLevel.Info:
                        case LogLevel.Caution:
                            byPass = true;
                            break;
                    }
                    break;

                // Message levels Critical and Error to show in Zorro Window
                // Message levels Warning and Cautionto show in log file only
                // Anything else does not show at all
                case Verbosity.Intermediate:
                    switch (level)
                    {
                        case LogLevel.Caution:
                        case LogLevel.Warning:
                            logOnly = true;
                            break;

                        case LogLevel.Info:
                            byPass = true;
                            break;
                    }
                    break;

                // Message levels Critical, Error, and Warning to show in
                // Zorro Window. Message levels Caution and Info to show in
                // log file only.
                case Verbosity.Advanced:
                    switch (level)
                    {
                        case LogLevel.Caution:
                        case LogLevel.Info:
                            logOnly = true;
                            break;
                    }
                    break;

                // Message levels other than Info to show in Zorro Window.
                // Info level in log only
                case Verbosity.Extensive:
                    switch (level)
                    {
                        case LogLevel.Info:
                            logOnly = true;
                            break;
                    }
                    break;

                case Verbosity.Extreme:
                    logOnly = true;
                    break;
            }

            // Is message to be bypassed at this VERBOSITY level?
            if (!byPass)
            {
                // NO: Is message for log file only
                if (logOnly)
                {
                    // YES: Using a timestamp or a line number?
                    if (timeStamp || lineNumbers)
                    {
                        // YES: Write the msg to log file with these prepended
                        text = "#" + string.Format(

                                // The log entry template
                                ParseFormat(timeStamp, lineNumbers),

                                // The current timestamp (0)
                                DateTime.Now.ToString(dateFmt),

                                // The source file (1)
                                Path.GetFileNameWithoutExtension(file),

                                // The method (2)
                                method,

                                // The line number (3)
                                line,

                                // The severity level
                                level.ToString().ToUpper(),

                                // The message
                                text
                                );

                    }
                }

                // SHow the message
                TDAmerAPI.BrokerError(text);
            }
        }

        //*********************************************************************
        //  Method: Log (Overload)
        //
        /// <summary>
        /// Show a log message.
        /// </summary>
        /// 
        /// <param name="text">
        /// The message string to show.
        /// </param>
        /// 
        /// <param name="file">
        /// Autopopulated with the source file from which this method was
        /// called.
        /// </param>
        /// 
        /// <param name="method">
        /// Autopopulated with the method withing the source file from which
        /// this method was called.
        /// </param>
        /// 
        /// <param name="line">
        /// Autopopulated with the line number of the source file from which 
        /// this method was called.
        /// </param>
        /// 
        /// <remarks>
        /// Overload of log method for showing message at default setting of
        /// LogLevel = Info.
        /// </remarks>
        //*********************************************************************
        public static void
            Log
            (
            string text,
            [CallerFilePath] string file = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int line = 0
            )
        {
            // Call log method with info level and other data set
            LogHelper.Log(LogLevel.Info, text, file, method, line);
        }
    }
}
