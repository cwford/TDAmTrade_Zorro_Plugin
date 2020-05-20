using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using TDAmeritradeZorro.Classes;

namespace TDAmeritradeZorro.Utilities
{
    //*************************************************************************
    //  Class: ResxHelper
    //
    /// <summary>
    /// A class that supports the use of resource files and globalization.
    /// </summary>
    /// 
    /// <remarks>
    /// Currently English (U.S.) is the only language supported by this plug-in.
    /// </remarks>
    //*************************************************************************
    public static class Resx
    {
        private static Dictionary<string, string> LangDict = new Dictionary<string, string>
        {
            {"Afrikaans", "af"},
            {"Albanian", "sq"},
            {"Arabic (Algeria)", "ar-DZ"},
            {"Arabic (Bahrain)", "ar-BH"},
            {"Arabic (Egypt)", "ar-EG"},
            {"Arabic (Iraq)", "ar-IQ"},
            {"Arabic (Jordan)", "ar-JO"},
            {"Arabic (Kuwait)", "ar-KW"},
            {"Arabic (Lebanon)", "ar-LB"},
            {"Arabic (Libya)", "ar-LY"},
            {"Arabic (Morocco)", "ar-MA"},
            {"Arabic (Oman)", "ar-OM"},
            {"Arabic (Qatar)", "ar-QA"},
            {"Arabic (Saudi Arabia)", "ar-SA"},
            {"Arabic (Syria)", "ar-SY"},
            {"Arabic (Tunisia)", "ar-TN"},
            {"Arabic (U.A.E.)", "ar-AE"},
            {"Arabic (Yemen)", "ar-YE"},
            {"Basque", "eu"},
            {"Belarusian", "be"},
            {"Bulgarian", "bg"},
            {"Catalan", "ca"},
            {"Chinese (Hong Kong)", "zh-HK"},
            {"Chinese (PRC)", "zh-CN"},
            {"Chinese (Singapore)", "zh-SG"},
            {"Chinese (Taiwan)", "zh-TW"},
            {"Croatian", "hr"},
            {"Czech", "cs"},
            {"Danish", "da"},
            {"Dutch (Belgium)", "nl-BE"},
            {"Dutch (Standard)", "nl"},
            {"English", "en"},
            {"English (Australia)", "en-AU"},
            {"English (Belize)", "en-BZ"},
            {"English (Canada)", "en-CA"},
            {"English (Ireland)", "en-IE"},
            {"English (Jamaica)", "en-JM"},
            {"English (New Zealand)", "en-NZ"},
            {"English (South Africa)", "en-ZA"},
            {"English (Trinidad)", "en-TT"},
            {"English (United Kingdom)", "en-GB"},
            {"English (United States)", "en-US"},
            {"Estonian", "et"},
            {"Faeroese", "fo"},
            {"Farsi", "fa"},
            {"Finnish", "fi"},
            {"French (Belgium)", "fr-BE"},
            {"French (Canada)", "fr-CA"},
            {"French (Luxembourg)", "fr-LU"},
            {"French (Standard)", "fr"},
            {"French (Switzerland)", "fr-CH"},
            {"Gaelic (Scotland)", "gd"},
            {"German (Austria)", "de-AT"},
            {"German (Liechtenstein)", "de-LI"},
            {"German (Luxembourg)", "de-LU"},
            {"German (Standard)", "de"},
            {"German (Switzerland)", "de-CH"},
            {"Greek", "el"},
            {"Hebrew", "he"},
            {"Hindi", "hi"},
            {"Hungarian", "hu"},
            {"Icelandic", "is"},
            {"Indonesian", "id"},
            {"Irish", "ga"},
            {"Italian (Standard)", "it"},
            {"Italian (Switzerland)", "it-CH"},
            {"Japanese", "ja"},
            {"Korean", "ko"},
            {"Korean (Johab)", "ko"},
            {"Kurdish", "ku"},
            {"Latvian", "lv"},
            {"Lithuanian", "lt"},
            {"Macedonian (FYROM)", "mk"},
            {"Malayalam", "ml"},
            {"Malaysian", "ms"},
            {"Maltese", "mt"},
            {"Norwegian", "no"},
            {"Norwegian (Bokmål)", "nb"},
            {"Norwegian (Nynorsk)", "nn"},
            {"Polish", "pl"},
            {"Portuguese (Brazil)", "pt-BR"},
            {"Portuguese (Portugal)", "pt"},
            {"Punjabi", "pa"},
            {"Rhaeto-Romanic", "rm"},
            {"Romanian", "ro"},
            {"Romanian (Republic of Moldova)", "ro-MD"},
            {"Russian", "ru"},
            {"Russian (Republic of Moldova)", "ru-MD"},
            {"Serbian", "sr"},
            {"Slovak", "sk"},
            {"Slovenian", "sl"},
            {"Sorbian", "sb"},
            {"Spanish (Argentina)", "es-AR"},
            {"Spanish (Bolivia)", "es-BO"},
            {"Spanish (Chile)", "es-CL"},
            {"Spanish (Colombia)", "es-CO"},
            {"Spanish (Costa Rica)", "es-CR"},
            {"Spanish (Dominican Republic)", "es-DO"},
            {"Spanish (Ecuador)", "es-EC"},
            {"Spanish (El Salvador)", "es-SV"},
            {"Spanish (Guatemala)", "es-GT"},
            {"Spanish (Honduras)", "es-HN"},
            {"Spanish (Mexico)", "es-MX"},
            {"Spanish (Nicaragua)", "es-NI"},
            {"Spanish (Panama)", "es-PA"},
            {"Spanish (Paraguay)", "es-PY"},
            {"Spanish (Peru)", "es-PE"},
            {"Spanish (Puerto Rico)", "es-PR"},
            {"Spanish (Spain)", "es"},
            {"Spanish (Uruguay)", "es-UY"},
            {"Spanish (Venezuela)", "es-VE"},
            {"Swedish", "sv"},
            {"Swedish (Finland)", "sv-FI"},
            {"Thai", "th"},
            {"Tsonga", "ts"},
            {"Tswana", "tn"},
            {"Turkish", "tr"},
            {"Ukrainian", "uk"},
            {"Urdu", "ur"},
            {"Venda", "ve"},
            {"Vietnamese", "vi"},
            {"Welsh", "cy"},
            {"Xhosa", "xh"},
            {"Yiddish", "ji"},
            {"Zulu", "zu"}
        };

        //*********************************************************************
        //  Member: rm
        //
        /// <summary>
        /// The resource manager used to access the current resource file.
        /// </summary>
        //*********************************************************************
        private static ResourceManager rm;

        //*********************************************************************
        //  Member: GetString
        //
        /// <summary>
        /// Get the string resource associated with a given key.
        /// </summary>
        /// 
        /// <param name="key">
        /// The key for which a string resource is being sought.
        /// </param>
        //*********************************************************************
        public static string
            GetString
            (
            string key
            )
        {
            // Initialize the resource manager if necessary
            if (rm == null)
                rm = new ResourceManager($"TDAmeritradeZorro.Resources.{Broker.settings.LangResx}", typeof(Resx).Assembly);

            // Get the string resource
            string text = rm.GetString(key);

            // If resource found, return it; if not, return an empty string
            return text == null ? "" : text.Replace("\\r\\n", Environment.NewLine);
        }

        //*********************************************************************
        //  Member: GetStringFromLang
        //
        /// <summary>
        /// Get the string resource associated with a given key.
        /// </summary>
        /// 
        /// <param name="key">
        /// The key for which a string resource is being sought.
        /// </param>
        /// 
        /// <param name="lang">
        /// The language to use.
        /// </param>
        //*********************************************************************
        public static string
            GetStringFromLang
            (
            string key,
            string lang
            )
        {
            // Initialize the resource manager if necessary
            if (rm == null)
                rm = new ResourceManager($"TDAmeritradeZorro.Resources.{lang}", typeof(Resx).Assembly);

            // Get the string resource
            string text = rm.GetString(key);

            // If resource found, return it; if not, return an empty string
            return text == null ? "" : text.Replace("\\r\\n", Environment.NewLine);
        }

        //*********************************************************************
        //  Method: ValidateLang
        //
        /// <summary>
        /// Validate a language specification.
        /// </summary>
        /// 
        /// <param name="lang">
        /// Language specification entered as xx-YY where xx is the language,
        /// and YY is the country or region. -YY is optional.
        /// </param>
        /// 
        /// <returns>
        ///  1 = Language specification is valid
        /// -1 = Language specification is invalid
        /// -2 = Language specification is valid, but language not on-file
        ///  0 = Unspecified error
        /// </returns>
        //*********************************************************************
        public static int
            ValidateLang
            (
            string lang
            )
        {
            // Method member
            string resxString;
            string[] resxNames;

            try
            {
                // Is the language specification valid?
                if (LangDict.ContainsValue(lang))
                {
                    // YES: Form resource string
                    resxString = $"TDAmeritradeZorro.Resources.{lang}.resources";

                    // Get all the resoure names in this assembly
                    resxNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

                    // Is the language spec in the resources names?
                    foreach (string resxName in resxNames)
                        if (resxName == resxString) return 1;

                    // Language spec not in assembly
                    return -2;
                }
                else
                {
                    // NO: Return invalid language code
                    return -1;
                }
            }
            catch(Exception)
            {
                // ERROR: Return unspecified error code
                return 0;
            }
        }
    }
}
