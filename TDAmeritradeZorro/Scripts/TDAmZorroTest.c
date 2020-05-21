// Trade Test for Broker API Verification ///////////////////

//#define BY_MARGIN
//#define LOG_ACCOUNT
//#define LOG_BOOK
//#define RESUME
//#define TYPE_TRADES	// last trade price instead of current ask/bid
//#define ROLLSTART	(1<<23)
//#define ORDERTEXT	"MOC/"
//#define EXCHANGE	"BTRX"
//#define RECORD
//#define MAXREQUESTS	10./60	// 10 requests per minute
//#define ASSETLIST	"AssetsSP30"
//#define ASSET "GRPN"
//#define ASSET "ES-FUT-20200320-ES-GLOBEX"
//#define ASSET "EUR-FUT-20181015-6E-GLOBEX-USD"
// Define a DUMMY asset.
#define PERCENTSTOP
#define MAXLOTS 50
#define MAXLIMIT 10
#define VERBOSE 7
#define LOG_TRADES
#define DIAGNOSTICS 1
#define LOG_VOL // log volume, spread, margin cost
#define ROUND_LIMIT
#define ASSET ""

//*****************************************************************************
//           T D  A M E R I T R A D E  U S E R  C O M M A N D S
//*****************************************************************************
#define SHOW_RESOURCE_STRING 4000 // get text string from *.resx file
#define REVIEW_LICENSE 4002		  // review the plug-in license
#define GET_ASSET_LIST 4004		  // current asset list
#define GET_TEST_ASSETS 4006	  // current asset list
#define SET_VERBOSITY 4008		  // command to set plug-in's verbosity level
#define SET_TESTMODE 4010		  // 1 = test mode; 0 = not test mode
#define TOTAL_RUNS 9 // The total number of runs for this test script

int AutoTrade = 0;
int OrderMode = 0;
int RunNum = 0;
int numTestAssets = 0;
int closingType = 1;
bool tradeType = true; i
char testAssetsArray[150];

//*****************************************************************************
//                             M E T H O D S
//*****************************************************************************
int tradeAdapt(
	var Step)
{
	if (!TradeIsOpen)
	{ // entry limit
		if (TradeIsLong)
		{
			if (OrderLimit > TradePriceOpen)
				return 0;		// try no more
			OrderLimit += Step; // adapt limit
		}
		else
		{ // short
			if (OrderLimit < TradePriceOpen - Spread)
				return 0;
			OrderLimit -= Step;
		}
	}
	else
	{ // exit limit
		if (TradeIsLong)
			OrderLimit -= Step;
		else
			OrderLimit += Step;
	}
	OrderLimit = roundto(OrderLimit, PIP / 2);
	return 1;
}

int tmf()
{
	//	printf("\nTMF: %f/%f\\%f/%f",priceOpen(),priceHigh(),priceLow(),priceClose());
	// adapt limit of FOK/IOC trades
	if (TradeIsMissed && OrderMode >= 2)
	{
		var Step = max(0.333 * Spread, 0.333 * PIP);
		if (OrderMode == 2)	 // FOK order
			OrderDelay = 30; // try again in 30 seconds
		else				 // GTC order, no delay needed
			OrderDelay = 0;
		if (!tradeAdapt(Step))
			return 1; // cancel trade
		else
		{
			printf("\n%s Limit %s", strtr(ThisTrade), sftoa(OrderLimit, 5));
			return ifelse(TradeIsOpen, 1 + 16, 2 + 16); // repeat order, and trigger tmf at next event
		}
	}
	return 16;
}

void setLimit(
	var Factor)
{
	OrderLimit = OrderDelay = OrderDuration = 0;
	resf(TradeMode, TR_GTC);
	if (OrderMode == 1)
	{
		OrderLimit = priceClose() + Factor * Spread;
		//printf(" Limit %s", sftoa(OrderLimit, 5));
	}
	else if (OrderMode == 2)
	{
		OrderLimit = priceClose() + ifelse(Factor < 0, -Spread, 0.);
		printf(" Adaptive %s", sftoa(OrderLimit, 5));
	}
	else if (OrderMode == 3)
	{
		OrderLimit = priceClose() + Factor * Spread;
		OrderDuration = 30;
		setf(TradeMode, TR_GTC);
		printf(" GTC %s", sftoa(OrderLimit, 5));
	}

#ifdef ROUND_LIMIT
	OrderLimit = roundto(OrderLimit, PIP);
	printf(" Closing Price: %.4f  Rounded Limit: %.4f", priceClose(), OrderLimit);
#endif

#ifdef ROLLSTART
	setf(AssetMode, ROLLSTART);
	RollLong = RollShort = -0.5;
#endif
}

void doTrade(
	int What,
	var Factor)
{
#ifdef BY_MARGIN
	Margin = slider(1);
#else
	Lots = slider(1); // get current slider position
#endif
#ifdef PERCENTSTOP
	if (slider(2) > 0)
		Stop = Trail = roundto(0.01 * priceClose() * slider(2), PIP);
	else
		Stop = 0;
#else
	Stop = Trail = PIP * slider(2);
#endif
	//if(OrderMode) TradeMode &= ~TR_MAIN; //else TradeMode |= TR_MAIN;
#ifdef ORDERTEXT
	brokerCommand(SET_ORDERTEXT, ORDERTEXT);
#endif
	setLimit(Factor);
	if (What == 1)
		enterLong(tmf);
	else if (What == 2)
		enterShort(tmf);
	else if (What == 3)
	{
#ifdef BY_MARGIN
		exitLong();
#else
		exitLong(0, 0, Lots);
#endif
	}
	else if (What == 4)
	{
#ifdef BY_MARGIN
		exitShort();
#else
		exitShort(0, 0, Lots);
#endif
	}
}

void loadTestAssets()
{
	// Method members
	int i;

	// Call the broker command to get the test assets
	brokerCommand(GET_TEST_ASSETS, testAssetsArray);

	// Message that we're loading assets
	//printf("\nLoading assets...");

	// Print out the test assets
	for (i = 0; i < 10; ++i)
	{
		if (*(testAssetsArray + (i * 8)) != '\0')
		{
			//printf("\n%s", ch_arr + (i * 8));
			// Add this asset to the asset list
			assetAdd(testAssetsArray + (i * 8));

			// Select this asset
			asset(testAssetsArray + (i * 8));

			// Save the number of test assets
			numTestAssets = i + 1;
		}
	}
}

void initRun()
{
	// Add a DUMMY asset, so everything else can proceed normally
	assetAdd("");
	asset("");

	// Set that the plug-in is in test mode
	brokerCommand(SET_TESTMODE, 1);

	// Show the TESTING header. Call must be after assets have been added
	printf("\n\n**********************************************************");
	brokerCommand(SHOW_RESOURCE_STRING, "TEST_HEADING");
	printf("\n**********************************************************\n\n");

	AutoTrade = 0;
	OrderMode = 0;
	Hedge = 0;

	// Set parameters based on DEFINE statements above
#ifdef USE_LOOKBACK
	LookBack = USE_LOOKBACK;
#endif

#ifndef RESUME
	SaveMode = 0;
#endif

#ifdef ASSETLIST
	assetList(ASSETLIST);
#endif

#ifdef EXCHANGE
	brokerCommand(SET_BROKER, EXCHANGE);
#endif

#ifdef BY_MARGIN
	Margin = slider(1, 100, 0, 100 * MAXLOTS, "Margin", 0);
#else
	Lots = slider(1, MAXLOTS / 4, 0, MAXLOTS, "Lots", 0);
#endif

#ifdef PERCENTSTOP
	Stop = Trail = 0.01 * priceClose() * slider(2, 0, 0, 40, "Stop %", 0);
#else
	Stop = Trail = PIP * slider(2, 0, 0, 50, "Stop", 0);
#endif

#ifdef DIAGNOSTICS
	brokerCommand(SET_DIAGNOSTICS, DIAGNOSTICS);
#endif
#ifdef TYPE_TRADES
	brokerCommand(SET_PRICETYPE, 2);
	brokerCommand(SET_VOLTYPE, 4);
#else
	brokerCommand(SET_PRICETYPE, 1);
#endif

#ifdef MAXREQUESTS
	MaxRequests = MAXREQUESTS;
#endif
	if (MaxRequests > 0)
		printf("\nMax Requests: %.1f / sec", MaxRequests);

	if (!is(LOOKBACK))
	{
#ifdef LOG_ACCOUNT
		_POS(10);
		printf("\nN %i H %i  B %s Eq %s M %s  Px %s",
			ifelse(is(NFA), 1, 0), Hedge,
			sftoa(Balance, 2), sftoa(Equity, 2), sftoa(MarginVal, 2),
			sftoa(priceClose(0), 5));
		_POS(20);
		var Pos = brokerCommand(GET_POSITION, Symbol);
		if (Pos != 0)
			printf(" T %.2f", Pos);
#endif
#ifdef LOG_TRADES
		for (open_trades)
		{
			if (TradeIsPending)
				printf("\n%s still pending", strtr(ThisTrade));
			else
				printf("\n%s Lots: %i Target: %i", strtr(ThisTrade), TradeLots, TradeLotsTarget);
		}
#endif
		int x = 1;
	}

#ifdef LOG_VOL
	if (!is(LOOKBACK))
		printf("\nVol %s  Spr %s  MCost %s", sftoa(marketVol(), 2), sftoa(Spread, 2), sftoa(MarginCost, 2));
#endif
#ifdef LOG_BOOK
	static T2 Quotes[MAX_QUOTES];
	brokerCommand(SET_SYMBOL, Symbol);
	int N = brokerCommand(GET_BOOK, Quotes);
	printf("\nOrderbook: %i quotes", N);
#endif
#ifdef RECORD
	History = "rec.t6";
	priceRecord();
#endif
}

void closing
()
{
	int i;

	if (closingType == 1)
		printf("\n\nClosing LONG trades for...");
	else
		printf("\n\nClosing SHORT trades for...");

	for (i = 0; i < numTestAssets; ++i)
	{
		// Print the asset name being closed
		printf("\nAsset = %s", testAssetsArray + (i * 8));

		// Select the asset at the ith position
		asset(testAssetsArray + (i * 8));

		if (closingType == 1)
			exitLong("*");
		else
			exitShort("*");
	}
	printf("\n");
}

void goingLong
()
{
	int i;
	OrderMode = 1;

	printf("\n\nGoing LONG for...");
	return;
	for (i = 0; i < numTestAssets; ++i)
	{
		// Print the asset being selected
		printf("\nAsset = %s", testAssetsArray + (i * 8));

		// Select the asset at the ith position
		asset(testAssetsArray + (i * 8));

		// Set the LIMIT price, below the current close
		setLimit(-MAXLIMIT);

		// Enter a LONG trade for this asset
		enterLong();
	}
	printf("\n");
}

void goingShort
()
{
	int i;
	OrderMode = 1;

	printf("\n\nGoing SHORT for...");
	for (i = 0; i < numTestAssets; ++i)
	{
		// Print the asset being selected
		printf("\nAsset = %s", testAssetsArray + (i * 8));

		// Select the asset at the ith position
		asset(testAssetsArray + (i * 8));

		// Set the LIMIT price, above the current close
		setLimit(MAXLIMIT);

		// Enter a SHORT trade for this asset
		enterShort();
	}
	printf("\n");
}

function run()
{
	// In test mode?
	if (is(TESTMODE))
	{
		// YES: Ask user to click the TRADE button
		quit("Click [Trade]!");

		// Exit script
		return;
	}

	// Set the plug-in's VERBOSITY level
	brokerCommand(SET_VERBOSITY, 16);

	BarPeriod = 1;
	PlotPeriod = 5;
	NumYears = 1;
	LookBack = 0;
	TradesPerBar = 1;
	Verbose = VERBOSE;
	TickTime = 10000;
	Weekend = 0;

	set(LOGFILE);

	if (is(INITRUN))
	{
		initRun();
		loadTestAssets();
		asset("");
	}

	printf("\nEntered TESTING RUN #%i...", RunNum);

	// Is this RUN beyond the initial RUN?
	if (RunNum > 0)
	{
		// YES: Is this run an even run?
		if (RunNum % 2 == 0)
		{
			// YES: Closing trades on this run
			closing();
		}
		else
		{
			// NO: Are we going long?
			if (tradeType)
			{
				// YES: Going long for this run
				goingLong(); `
					closingType = 1;
			}
			else
			{
				// NO: Going shont from this run
				goingShort();
				closingType = 0;
			}

			// Swap going long and going short
			tradeType = !tradeType;
		}
	}

	if (++RunNum == TOTAL_RUNS)
	{
		printf("\n******** TD Ameritrade-Zorro Plug-In Testing End ********");
		quit();
	}
}
