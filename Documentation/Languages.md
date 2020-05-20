# Globalization
This broker plug-in can be configured to support any language through the use of resource files.

## Currently Supported Languages

* English (United States) **en-US**
* French (Standard) **fr**
* German (Standard) **de**
* Spanish (Spain) **es**

**NOTE:** The French, German, and Spanish language resources are translated from English with Google Translate. They may not be the most accurate translations of the corresponding English language phrases. Native language speakers are encouraged to make changes to the appropriate language resource files, incorporate them into a new broker plugi-in for testing, and share them with the Zorro community.

## Where to Specify a Language
You inform this plug-in of the language to use by appending a language specification to the **consumer key** entered into the User ID field of the Zorro trading engine window. See the documentation for information on [appending a language specification](https://github.com/cwford/TDAmTrade_Zorro_Plugin#using-the-td-ameritrade-broker-plug-in) to a **consumer key**. You can also [change the default langugae specification](https://github.com/cwford/TDAmTrade_Zorro_Plugin#initializing-the-settings-file) in the Settings file.

## Modifying and Adding Languages
Globalization language support in this plug-in is achieved through the use of language-specific resource files. These resource files are located in the plug-in solution folder at ../Resources/xx-YY.resx, where xx is the two-letter language code and YY is the country or region code, if required. For example, **en-US** represents the English language as spoken in the United States, while **en-NZ** represents the English language as spoken in New Zealand. See [below](https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Languages.md#language-specifications) for a complete list of language specifications.

**To modify an existing language resource file in Visual Studio:**
1. Open the corresponding xx-YY.resx resource file.
2. Make appropriate changes.
3. Save and close the file.
4. Re-build the solution.
5. Copy the new DLL to the Zorro Plugin folder.

**To add a new language, follow these steps:**
1. Create a new resource file in the **Resources** folder of the plug-in solution. It should be named xx-YY, where xx is the two-letter language code, and YY is the region or country code, if needed. See [below](https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Languages.md#language-specifications) for a complete list of language specifications.
2. Copy the left-hand column of the "en-US.resx" file to the left-hand column of this new language resource file.
3. Add translations of the middle column of the "en-US.resx" file to the middle column of this new language resource file.
4. Save and close the file
5. Re-build the solution
6. Copy the new DLL to the Zorro Plugin folder.

## Language Specifications

* Afrikaans **af**
* Albanian **sq**
* Arabic (Algeria) **ar-DZ**
* Arabic (Bahrain) **ar-BH**
* Arabic (Egypt) **ar-EG**
* Arabic (Iraq) **ar-IQ**
* Arabic (Jordan) **ar-JO**
* Arabic (Kuwait) **ar-KW**
* Arabic (Lebanon) **ar-LB**
* Arabic (Libya) **ar-LY**
* Arabic (Morocco) **ar-MA**
* Arabic (Oman) **ar-OM**
* Arabic (Qatar) **ar-QA**
* Arabic (Saudi Arabia) **ar-SA**
* Arabic (Syria) **ar-SY**
* Arabic (Tunisia) **ar-TN**
* Arabic (U.A.E.) **ar-AE**
* Arabic (Yemen) **ar-YE**
* Basque **eu**
* Belarusian **be**
* Bulgarian **bg**
* Catalan **ca**
* Chinese (Hong Kong) **zh-HK**
* Chinese (PRC) **zh-CN**
* Chinese (Singapore) **zh-SG**
* Chinese (Taiwan) **zh-TW**
* Croatian **hr**
* Czech **cs**
* Danish **da**
* Dutch (Belgium) **nl-BE**
* Dutch (Standard) **nl**
* English **en**
* English (Australia) **en-AU**
* English (Belize) **en-BZ**
* English (Canada) **en-CA**
* English (Ireland) **en-IE**
* English (Jamaica) **en-JM**
* English (New Zealand) **en-NZ**
* English (South Africa) **en-ZA**
* English (Trinidad) **en-TT**
* English (United Kingdom) **en-GB**
* English (United States) **en-US**
* Estonian **et**
* Faeroese **fo**
* Farsi **fa**
* Finnish **fi**
* French (Belgium) **fr-BE**
* French (Canada) **fr-CA**
* French (Luxembourg) **fr-LU**
* French (Standard) **fr**
* French (Switzerland) **fr-CH**
* Gaelic (Scotland) **gd**
* German (Austria) **de-AT**
* German (Liechtenstein) **de-LI**
* German (Luxembourg) **de-LU**
* German (Standard) **de**
* German (Switzerland) **de-CH**
* Greek **el**
* Hebrew **he**
* Hindi **hi**
* Hungarian **hu**
* Icelandic **is**
* Indonesian **id**
* Irish **ga**
* Italian (Standard) **it**
* Italian (Switzerland) **it-CH**
* Japanese **ja**
* Korean **ko**
* Korean (Johab) **ko**
* Kurdish **ku**
* Latvian **lv**
* Lithuanian **lt**
* Macedonian (FYROM) **mk**
* Malayalam **ml**
* Malaysian **ms**
* Maltese **mt**
* Norwegian **no**
* Norwegian (Bokmål) **nb**
* Norwegian (Nynorsk) **nn**
* Polish **pl**
* Portuguese (Brazil) **pt-BR**
* Portuguese (Portugal) **pt**
* Punjabi **pa**
* Rhaeto-Romanic **rm**
* Romanian **ro**
* Romanian (Republic of Moldova) **ro-MD**
* Russian **ru**
* Russian (Republic of Moldova) **ru-MD**
* Serbian **sr**
* Slovak **sk**
* Slovenian **sl**
* Sorbian **sb**
* Spanish (Argentina) **es-AR**
* Spanish (Bolivia) **es-BO**
* Spanish (Chile) **es-CL**
* Spanish (Colombia) **es-CO**
* Spanish (Costa Rica) **es-CR**
* Spanish (Dominican Republic) **es-DO**
* Spanish (Ecuador) **es-EC**
* Spanish (El Salvador) **es-SV**
* Spanish (Guatemala) **es-GT**
* Spanish (Honduras) **es-HN**
* Spanish (Mexico) **es-MX**
* Spanish (Nicaragua) **es-NI**
* Spanish (Panama) **es-PA**
* Spanish (Paraguay) **es-PY**
* Spanish (Peru) **es-PE**
* Spanish (Puerto Rico) **es-PR**
* Spanish (Spain) **es**
* Spanish (Uruguay) **es-UY**
* Spanish (Venezuela) **es-VE**
* Swedish **sv**
* Swedish (Finland) **sv-FI**
* Thai **th**
* Tsonga **ts**
* Tswana **tn**
* Turkish **tr**
* Ukrainian **uk**
* Urdu **ur**
* Venda **ve**
* Vietnamese **vi**
* Welsh **cy**
* Xhosa **xh**
* Yiddish **ji**
* Zulu **zu**}
