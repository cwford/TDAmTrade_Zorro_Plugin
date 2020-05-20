<p align="center">
  <img height="120" src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/zorro-tda.png">
</p>
<p align="center">
  <strong>Version 1.01</strong>
</p>
<p align="center">
  <em><strong>A TD Ameritrade Broker Plug-In for Zorro written in C#.</strong></em>
</p>

<p align="center">
  Copyright &copy; 2020. Clyde W. Ford. All rights reserved.
</p>


<p align="center">
  <strong style="font-size:9pt">Significant financial risks accompany the use of this<br>plug-in to trade securities and other financial instruments.<br>USE AT YOUR OWN RISK.</strong>
</p>

## Overview
TDAmTrade_Zorro_Plugin is a 32-bit broker plug-in for Zorro, allowing it to communicate with TD Ameritrade using the TD Ameritrade REST API. This plug-in is free for non-commercial use. With this plug-in, you can use Zorro to automatically trade stocks, ETFs and options through your TD Ameritrade brokerage account.

The following Zorro broker methods are implemented:

* **BrokerOpen**
* **BrokerLogin**
* **BrokerBuy2**
* **BrokerSell2**
* **BrokerAsset**
* **BrokerAccount**
* **BrokerHistory2**
* **BrokerTrade**
* **BrokerTime**

**BrokerCommands:***
* **GET_COMPLIANCE** (Set NFA compliance, TD Ameritrade is fully NFA compliant)
* **GET_POSITION** (Required for some Z-strategies)
* **GET_OPTIONS** (Retrieve an option chain for an underlying asset)
* **SET_SYMBOL** (Set the current asset symbol)
* **SET_COMBO_LEGS** (For combination option orders)

**Supplemental BrokerCommands:****
* **SHOW_RESOURCE_STRING** (Show a globalized resource string from the plug-in)
* **REVIEW_LICENSE** (Display the plug-in license and force re-acceptance)
* **GET_ASSET_LIST** (Retrieve a list of subscribed to assets)
* **GET_TEST_ASSETS** (Get the test assets included in the Settings file. See [Setting file](https://github.com/cwford/TDAmTrade_Zorro_Plugin#initializing-the-settings-file) for more.)
* **SET_VERBOSITY** (Set the diagnostic verbosity level. See [Verbosity level](https://github.com/cwford/TDAmTrade_Zorro_Plugin#error-messages) for more.)
* **SET_TESTMODE** (Set whether the plug-in is in testing mode.)
* **SET_SELL_SELL_SHORT** (Set what to do if selling more shares of an asset than owned)

&ast;NOTE: The **BrokerStop** method is **not implemented.**

&ast;&ast;NOTE: Set below

In order to use this plug-in with Zorro, several DLLs need to be manually copied from the **Lib** folder of this solution to the Zorro **Plugin** folder. 

|**In Lib Folder As**|**Copied to Zorro Plugin Folder As**|**Description**
|----------------------|--------------------------------|---------------------|
|TDAmeritradeZorro.dll|TDAmeritrade.dll|Main broker plug-in|
|Microsoft.Data.Sqlite (v 1.1.1)|Microsoft.Data.Sqlite|Microsoft Sqlite ADO.NET library|
|sqlite3.dll|sqlite3.dll|Native Sqlite library|
|DBLib.dll|DbLib.dll|Data access library for Sqlite|

Below is a view of the Zorro plug-in folder after the required files have been copied there:


<p align="center">
  <img src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/plugin_dir.png">
</p>


**Please read through the entire documentation before attempting to use the plug-in.**


The plug-in is written entirely in C#. The codebase is extensively documented making modifications easy for any programmer with a background in the C# language.

## Getting Started
Prior to using this plug-in, the user should create a TD Ameritrade brokerage account funded with a minimum of USD $2000.00. See https://www.tdameritrade.com to open a new account. This account should have margin, option, and Forex privileges.

Once a TD Ameritrade account is opened, the user needs to create a developer application and obtain a consumer key (secret key) for that developer application. This is an essential step. The plug-in will not operate without a consumer key, and a consumer key cannot be obtained without first creating a developer application.

### Obtaining a Consumer Key
Obtaining a consumer key is an essential step in using this Broker Plug-In. A consumer key is enteedr into the User ID input textbox of the LogIn section on the Zorro window. Without a consumer key this plug-in will not work. Follow these steps to create a consumer key.

#### Register as a Developer with TD Ameritrade
Go to the TD Ameritrade Developer Home Page at:
https://developer.tdameritrade.com/
and click the Register link on the right-hand side of the tool bar.


<p align="center">
  <img height="35" src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/Register.png">
</p>

After clicking this link a pop-up window with a registration form will appear. Fill out all fields of that form. <strong>NOTE:</strong> The checkbox for accepting the Developer License Agreement may be invisible or hidden in the middle of the link to accept the agreement. In either case, clicking on the blue-highlighted link will also cause the checkbox to be checked.


<p align="center">
  <img height="500" src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/register_form.png">
</p>

After filing out the form, click on the blue button at the bottom left-hand part of the form that is labeled <strong>Create new account</strong>. You will next be taken to a login form that asks for your username and password. **NOTE:** The username and password asked for here is not the username and password of your TD Ameritrade brokerage account. It is the username and password of you developer account. 

But at this point you do not have a password for your developer account. So, click <strong>Forgot your password?</strong> at the bottom of the form.


<p align="center">
  <img height="400" src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/forgot_password.png">
</p>

You will next be taken to a page with a form where you will enter your username or e-mail address and report the CAPTCHA image that you see. Click the blue button marked <strong>E-mail new password</strong>.



<p align="center">
  <img height="400" src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/email_password.png">
</p>

You will then receive an e-mail message with a link that, when clicked, takes you to your profile page on the TD Ameritrade Developer site. Enter a password of your chcoosing (that complies with the required guidelines) and add or modify any other information you wish.


<p align="center">
  <img height="750" src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/edit_profile.png">
</p>

Once you have entered that password, and made any changes, click the blue button at the bottom left of the page marked <strong>Save and login as XXX</strong>, where <strong>XXX</strong> is your username.


You will then go to a page which confirms that the changes you just made have been saved. On the tool bar of this page the link which reads <strong>My Apps</strong>.



<p align="center">
  <img height="400" src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/goto_myapps.png">
</p>

The next page shows you have no apps but gives you a chance to <strong>Add a New App</strong>. Click that button.



<p align="center">
  <img height="150" src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/my_apps.png">
</p>

On the <strong>Add App</strong> page you are next shown, enter information about you app.

<strong>NOTE:</strong> The name of the app can only be 15 characters with no spaces. Also, use the callback URL, http://127.0.0.1.

Place any descriptive information you desire in the textbox for the purpose of your app. Then click the blue <strong>Create App</strong> button at the bottom left of the form.



<p align="center">
  <img height="350" src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/add_app.png">
</p>

You will now see a page which confirms that you have created you Developer App. Click on the name of the application you just created.



<p align="center">
  <img height="200" src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/explore_apps.png">
</p>

Finally, you will be shown a page with your <strong>Consumer Key</strong>. Copy that key and save it in a safe place. <strong>You will need your consumer key to use this TD Ameritrade Plug-In.</strong> Along with your TD Ameritrade account number, the consumer key allows access to your account to enter trades and move money. This is why you must keep this consumer key safe.


<p align="center">
  <img height="400" src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/consumer_key.png">
</p>


## Initializing the Settings File
You need to create a Settings file to hold several application settings this broker plug-in requires. The settings file is located at: ../Zorro/Data/tda.json. Use **Notepad** or any text-editor of your choosing a create a new Settings file with the following data:

```javascript
{
   // The TD Ameritrade account currency units
    "currency" : "USD",

    // The TD Ameritrade account number
    "tdaAccountNum" : "NNNNNNNNN",

    // The default language resource file
    "langResx" : "en-US",
    
    // The testing asset symbols separated by commas (change to suit your needs)
    "testAssets": "GRPN,HUSA,LLEX,ZOM"
}
```

The following data that must be present in the Settings file:

* **currency** - The account current (USD, EUR, JPY, etc).
* **tdaAccountNum** - Your TD Ameritrade Account Number. This is not your consumer key.
* **langResx** - The default language resource file (see [Globalization](Documentation/Languages.md) for a list of languages currently supported by the plug-in and for language specifications).
* **testAssets** - The ticker symbols for assets that are used by the plug-in for self-diagnostic testing.

**NOTE:** You can use typical C# comments in the Settings file and those lines will be ignored. Any other files related to this plug-in found in the Data directory should not be removed or altered, or the plug-in might not operate.


## Using the TD Ameritrade Broker Plug-in
If you have not done so already, copy TDAmeritrade.dll to the Zorro plug-in directory at ../Zorro/Plugin. Open the Zorro executable to bring up the main trading window.

<p align="center">
  <img src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/zorro_main.png">
</p>

Make sure that you:

* Select the TD Ameritrade broker plug-in from the drop down list of plug-ins
* Select **Demo** the first time you run Zorro with this plug-in
* Enter your **Consumer Key** in the User ID field of the **Login** area of the window
* Select any trading script such as Z8

**NOTE:** TD Ameritrade does not have a true Demo mode. The API does not support "paper trading." Placing the Zorro trading engine in Demo mode, and selecting the TD Ameritrade broker plug-in, causes the plug-in to authenticate the user and run through a number of self-diagnostic tests. **YOU MUST USE DEMO MODE THE FIRST TIME YOU RUN ZORRO WITH THIS PLUG-IN.**

**LANGUAGE SPECIFICATION:** If you wish for the plug-in to provide messages in a language different from the default language enter the consumer key as: **consumer key**##**language specification** (the consumer key followed by two number signs followed by the lanugage specification). For example 12345ABC##es represents as consumer key of 12345ABC and a language of es (standard Spanish). See [Globalization](Documentation/Languages.md) for a list of languages currently supported by the plug-in and for language specifications.

#### License Acceptance
After clicking the **Trade** button, you will be asked to accept the license for this plug-in. This plug-in is available free, as non-commercial open-source software. The license is governed by the GNU LPL 3.0 license, with the addition of the Commons Clause restriction for non-commercial use and several addenda found at the end of the license. If you would like to use the plug-in for commercial purposes please contact the developer via the **Issues** section of this GitHub project. Please scroll down to read the license, and all of its addenda carefully before clicking the **I Accept** button.

<p align="center">
  <img src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/license.png">
</p>

After accepting the license, you will see a TD Ameritrade authentication login window pop-up on your screen.

<p align="center">
  <img src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/auth_screen_1.png">
</p>

Enter your username and password.

**NOTE:** This is the username and password of your TD Ameritrade trading account not the username and password of your Developer's App account. 

Click the **Log in** button at the bottom left of the log-in form. The next screen you will see is part of a two-factor authentication system. You can choose to receive an authentication code on your cellphone, at the number you supplied when obtaining a TD Ameritrade trading account, or you can choose to answer a **security question.** 

<p align="center">
  <img src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/auth_screen_2.png">
</p>

If you choose to receive an authentication code via your cellphone, click the **Continue** button and check your cellphone for that code. 

<p align="center">
  <img src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/auth_screen_3.png">
</p>

If you choose to answer a **security question** click the **Can't get the text message?** link, then click the **Answer a security question** link. The following screen will now appear:

<p align="center">
  <img src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/auth_screen_4.png">
</p>

In the space provide, supply your answer to the security question, then click the **Continue** button at the bottom left of the form. An access screen will appear showing the capabilities you have on the TD Ameritrade site via the REST API.

<p align="center">
  <img src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/auth_screen_5.png">
</p>

Congratulations! You have now authenticated the Zorro TD Ameritrade plug-in. The Zorro trading engine, still in Demo mode, will continue to run through its self-diagnostic tests. When it is finished you may either consult the Zorro message window (you will probably need to expand it) or consult the log file at ../Zorro/Log/Zx_demo.log, where x is the number of the **Z strategy** you chose prior to pressing the **Trade** button.

If all self-diagnostic tests were successful, you will receive an output log that looks similar to the following:

<p align="center">
  <img src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/success.png">
</p>

**Also available** for testing purposes is the script TDAmZorrTestTrade.c which you can find in the main folder of this plug-in. The differences between the two modes are testing are this: the self-diagnostic tests (with the plug-in in 'Demo' mode) test the internal broker functions, while the test script tests the communications between the Zorro trading engine, the plug-in, and the broker API.

#### Limitations of Testing
Testing is set-up to use trades that minimize financial risk. For example, LIMIT orders at improbable limit prices are placed, then immediately canceled, to test the BUY and SELL functions of the plug-in. Assets with extremely low prices are used, so that if a trade is entered, and not canceled, it will be ordinarily for less than USD $2.00. That does not mean that testing is without financial risk. You should recognize the financial risk inherent in any use of this plug-in, and take steps, accordingly, to minimize that risk. Such steps include, but are not limited to: maintaining a low account balance while testing, making sure your account privileges are correct, and [modifying the testing assets](https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/README.md#initializing-the-settings-file) used in the SETTINGS file, so they reflect assets with acceptably low prices for you.

While both the self-diagnostic tests, and the testing script, may reveal no errors that does not guarantee that this broker plug-in will function without error or exception. Many factors (such as account privileges, account balance, asset type, and trade type) go into successful trading. It is your responsibility to determine that these factors are correct prior to entering into testing or into any live trading through this plug-in. **Remember that significant financial risk may accompany the use of this plug-in to trade financial instruments, and that by using this plug-in you fully accept this risk as your responsibility alone.**

Once the diagnostic tests are all successful, you may begin using this plug-in to trade in Real mode. **Prior to using the plug-in in Real mode, if you are using any of the Z-strategies, be sure to read the Zorro documentation on what additional requirements, such as CSV files, those strategies need.**

### Supplemental Broker Commands
This plug-in implements the following broker commands which can be called from a Zorro script:

|**Command Name**|**#**|**Parameters**|**Description**|
|------------|-|----------|-----------|
|SHOW_RESOURCE_STRING| 4000 | Name of String|Show a globalized text string in the Zorro message window based on the current language.|
|REVIEW_LICENSE| 4002 | |Display the plug-in license and ask for user acceptance.|
|GET_ASSET_LIST| 4004 | |Get list of assets the plug-in is currently subscribed to.|
|GET_TEST_ASSETS| 4006 | |Get the test assets included in the Settings file. See [Setting file](https://github.com/cwford/TDAmTrade_Zorro_Plugin#initializing-the-settings-file) for more.)|
|SET_VERBOSITY| 4008 |Verbosity Level|Set the diagnostic verbosity level. See [Verbosity level](https://github.com/cwford/TDAmTrade_Zorro_Plugin#error-messages) for more.|
|SET_TESTMODE| 4010 |1 = Enable<br>0 = Disable|Entable or disable plug-in test mode.|
|SET_SELL_SELL_SHORT| 4012 | 1 = Cancel<br>2 = Adjust<br>3&nbsp;=&nbsp;Do&nbsp;nothing|Tell the plug-in what to do with a SELL order placed for more shares of an asset than user currently owns. Normally, TD Ameritrade will issue a SELL order for the shares currently owned and a SELL SHORT order for the balance of shares but will not return an order number for the SELL SHORT trade. Zorro will not be able to track this SHORT position.|

Supplemental commands are entered in a script as: brokerCommand(Command #, Parameter). See [brokerCommand](https://manual.zorro-project.com/brokerplugin.htm) for more.

### Do Not Manually Trade
It is very unwise to manually trade from the same TD Ameritrade brokerage account that the Zorro trading engine is automatically trading from. Errors, with serious financial consequences, can result. For example, if you manually sell some of the shares that Zorro has bought, then Zorro issues a sell order for the shares it believes to be in your account, you will be faced with a situation where you are trying to sell more shares than you currently own. If SET_SELL_SELL_SHORT has been set at 3 (do nothing), TD Ameritrade will sell any shares that it can from the Zorro order, but it will short sell any remaining shares that are not in the account. Zorro does not receive notice of the secondary short sell order, and it may get fulfilled and the short position may be held undetected. If the underlying asset rises in value, you may be faced with significant financial losses.

**If you want to manually trade on TD Ameritrade, open a new, separate brokerage account and do not allow Zorro to trade on that new account.**

### Error Messages
The TD Ameritrade/Zorro plug-in provides extensive diagnostics messaging and implements a broker command (#4008), SET_VERBOSITY, which allows the user to set the verbosity level of the diagnostic messages generated by the plug-in from a Zorro script. **The verbosity level of the plug-in is similar to, but not the same as, the DIAGNOSTIC level of a script.** The syntax of the SET_VERBOSITY command is:

```javascript
define SET_VERBOSITY 4008

int vLevel;
.
.
.
brokerCommand(SET_VERBOSITY, vLevel)
```

where vLevel is set to:

| **Verbosity** |  **Description of Verbosity Level**                                                                                 |
|-----------|-----------------------------------------------------------------------------------------------------------------|
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**1**    | **Basic.** Info and Caution messages not shown. Critical, Error, and Warning messages written to log file.          |
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**2**    | **Intermediate.** Info messages not shown. Critical and Error messages shown in Zorro window. Warning and Caution messages written to log file.|
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**4**    | **Advanced.** Critical, Error, and Warning messages shown in Zorro window. Caution and Info messsages written to log file.                 |
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**8**    | **Extensive.** Other than Info messages, all messages shown in Zorro window. Info messages written to log file.              |
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**16**    | **Extreme.** All messages written to Zorro window and to log file.                                                   |
|&nbsp;&nbsp;&nbsp;&nbsp;**+32**    | **Add time stamp of message.**                                                                                                 |
|&nbsp;&nbsp;&nbsp;&nbsp;**+64**    | **Add originating method and line number of message.**                                                                         |

For example, the command **brokerCommand(SET_VERBOSITY, 112)** (vLevel = 16 + 32 + 64 = 112) would tell the plug-in to show all messages in the Zorro window and log file, and those messages would be prepended with a time stamp, line number, and method of origination.

### Asset Symbols
This plug-in uses asset designators as described in the section [**IB Symbols**](https://manual.zorro-project.com/ib.htm) of the Zorro Manual. Asset lists and CSV asset files must conform to these symbol designators for trades to be successful.

### Modifying the Plug-In
If you are planning to modify the plug-in, or wish to examine the plug-in's code, please first consult the section of this documentation entitled [Details About the Plug-in](Documentation/Modify.md).

### Features
This Zorro broker plug-in has the following features:
* **Allow U.S. users to use a popular broker.** TD Ameritrade is a well-known, large and popular broker. Many other brokers that work with Zorro do not allow U.S. customers to use their features.
* **Written in C#.** The code base is well-documented and free for non-commercial use to modify and adapt. Any competent C# programmer should be able to follow the code as written to modify or extend it when desired.
* **Globalization.** The plug-in features language support in English, German, French, and Spanish with existing language text easily modified, and new language support added through resource files.

### Limitations
In general, the TD Ameritrade REST API is a poorly documented interface, as any developer who writes against it soons discovers. The online guide provides few examples and the online documentation for the interface is inconsistent, incomplete, and sometimes incorrect. The ThinkOrSwim platform available to TD Ameritrade users does not employ the TD Ameritrade REST API available to users. So, there is no opportunity to use it as a basis for understanding the REST API because the two are completely different systems. TD Ameritrade may answer questions concerning the REST API after weeks of waiting, or they may not answer questions at all. That said, this Zorro broker plug-in currently has the following limitations:
* **Windows 10.** The plug-in was developed and tested on a Windows 10 system. It was designed and developed to operate on a Windows system and it may not function on any other systems such as Linux, Android, or OSX.
* **OPTIONS trading.** TD Ameritrade imposes account requirements for OPTIONS trading. Consult your TD Ameritrade account profile to determine if it qualifies for OPTIONS trading, and to apply. 
* **COMBO LEG** orders may only be placed for **OPTIONS**. Unoptioned stocks or ETFs do not qualify for **COMBO LEG** orders. All **COMBO LEG** orders must be at **MARKET.** No **COMBO LEG** OPTION orders can have any leg at **LIMIT.** **COMBO LEG OPTION orders violating these rules will be rejected.** Execute a **SET_COMBO_LEGS** broker command prior to placing trades for OPTIONS on a **COMBO LEG** order. Read more about the [**SET_COMBO_LEGS**](https://manual.zorro-project.com/brokercommand.htm) before trading OPTIONS in your script.
* **INDEX OPTIONS** cannot be traded through the TD Ameritrade REST API only options derived from single underlying equities. Those wishing to trade index options might consider trading options against ETFs related to a particular index.
* **FOREX trading.** Currently, the TD Ameritrade REST API does not allow for currency trading. From inspection of the JSON objects returned from API requests, it appears as though some FOREX trading capabilities are built-in. Actual FOREX trading through the API may be available at some future time.
* **MUTUAL FUND trading.** Many mutual funds have an initual minimum investment amount. In order to trade mutual funds your account, or your first trade order, must meet that minimum. If not, a trade order for the fund will fail. As of May 15, 2020, the plug-in does not support MUTUAL FUND trading. While the TD Ameritrade documentatinn states that mutual fund trading is possible, no order to BUY or SELL mutual funds has been accepted by the API. This may change at some point in the future.
* **Can manage only one TD Ameritrade trading account** (for multiple accounts, run multiple instances of Zorro).
* **STOP MARKET and STOP LIMIT orders.** The TD Ameritrade REST API appears to allow for STOP MARKET or STOP LIMIT. But as of 05/15/2020 no STOP or STOP LIMIT orders were accepted. TD Ameritrade has been notified and may resolve this matter. But as of this release of the plug-in **STOP MARKET and STOP LIMIT orders** will not be traded. Zorro notes on the **BrokerBuy2** command state that the **STOP DISTANCE** included as a parameter for this command, "is not the real stop loss, which is handled by the trade engine. Placing the stop is not mandatory." Consequently, **STOP MARKET and STOP LIMIT orders** will not be placed by this version of the plug-in.
* **FUTURES trading.** Currently, the TD Ameritrade REST API does not allow for **FUTURES** or **FUTURE OPTIONS** trading. From inspection of the JSON objects returned from API requests, it appears as though some FUTURES trading capabilities are built-in. Actual FUTURES trading through the API may be available at some future time.
* **Specific Exchanges.** TD Ameritrade uses a **SMART** exchange system whereby orders are placed at the most readily available and appropriate exchange. This plug-in does not support orders directed to a specific exchange. All orders will be placed through TD Ameritrade's **SMART** exchange system.

### Why Trades May Fail
This Zorro plug-in will successfully submit trade orders using the TD Ameritrade API but those orders may still fail. There are many reasons for this. Some common reasons a trade may fail are:
* **Trade violates one of TD Ameritrade's policies.** Before using this plug-in, make sure you thoroughly review the TD Ameritrade documentation for your brokerage account. TD Ameritrade has a number of rules regarding the amount of money that should be present in an account at any time, for different kinds of assets, that control whether a trade order will be placed.
* **Trade violates TD Ameritrade's LIMIT policies.** TD Ameritrade has policies about how far away from the current Ask price a MARKET LIMIT order can be. It's not exactly clear what that distance is, but sometimes a trade order may be rejected because it falls outside of limit policies.
* **Too frequent BUY and SELL orders for same asset.** If you buy and sell the same asset too frequently, TD Ameritrade may flag your account as a "pattern trading" account and require a steep balance in that account along with a 90-day period during which your account will be closed to further trading. Consult the TD Ameritrade documentation for more information about "pattern trading."
* **Trade placed for unsupported products.** Only stocks, options, ETFs, and mutual funds are currently supported by the TD Ameritrade REST API. Trades placed for non-supported products will fail, even though Zorro supports such products for inclusion in asset lists.
* **Price unaligned for asset.** If a mutual fund has an minimum initial buy-in cost, and if you have not previously bought into the fund at this initial minimum, subsequent trade orders will fail.
