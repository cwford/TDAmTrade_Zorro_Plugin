//*****************************************************************************
// File: Order.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The Order class object.
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
using System.Runtime.Serialization;

namespace TDAmeritradeZorro.Classes.TDA
{
    //*************************************************************************
    //  Class: Order
    //
    /// <summary>
    /// The TD Ameritrade Order class. See the TD Ameritrade API
    /// information at https://developer.tdameritrade.com/apis for details on
    /// the properties of this class.
    /// </summary>
    //*************************************************************************
    [DataContract]
    public class Order
    {
        [DataMember(Name = "session")]
        public string Session { get; set; }

        [DataMember(Name = "duration")]
        public string Duration { get; set; }

        [DataMember(Name = "orderType")]
        public string OrderType { get; set; }

        [DataMember(Name = "complexOrderStrategyType")]
        public string ComplexOrderStrategyType { get; set; }

        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }

        [DataMember(Name = "price")]
        public double Price { get; set; }

        [DataMember(Name = "stopPrice")]
        public double StopPrice { get; set; }

        [DataMember(Name = "stopPriceLinkBasis")]
        public string StopPriceLinkBasis { get; set; }

        [DataMember(Name = "stopPriceLinkType")]
        public string StopPriceLinkType { get; set; }

        [DataMember(Name = "stopPriceOffset")]
        public double StopPriceOffset { get; set; }

        [DataMember(Name = "stopType")]
        public double StopType { get; set; }

        [DataMember(Name = "filledQuantity")]
        public int FilledQuantity { get; set; }

        [DataMember(Name = "remainingQuantity")]
        public int RemainingQuantity { get; set; }

        [DataMember(Name = "requestedDestination")]
        public string RequestedDestination { get; set; }

        [DataMember(Name = "destinationLinkName")]
        public string DestinationLinkName { get; set; }

        [DataMember(Name = "orderLegCollection")]
        public List<OrderLeg> OrderLegCollection { get; set; }

        [DataMember(Name = "orderActivityCollection")]
        public List<OrderActivity> OrderAttivityCollection { get; set; }

        [DataMember(Name = "orderStrategyType")]
        public string OrderStrategyType { get; set; }

        [DataMember(Name = "orderId")]
        public long OrderId { get; set; }

        [DataMember(Name = "cancelable")]
        public bool IsCancelable { get; set; }

        [DataMember(Name = "editable")]
        public bool IsEditable { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "enteredTime")]
        public DateTime EnteredTime { get; set; }

        [DataMember(Name = "accountId")]
        public long AccountId { get; set; }
    }
}
