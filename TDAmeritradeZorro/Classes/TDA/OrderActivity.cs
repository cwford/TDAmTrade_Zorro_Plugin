using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TDAmeritradeZorro.Classes.TDA
{
    [DataContract]
    public class OrderActivity
    {
        [DataMember(Name = "activityType")]
        public string ActivityType { get; set; }

        [DataMember(Name = "executionType")]
        public string ExecutionType { get; set; }

        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }

        [DataMember(Name = "orderRemainingQuantity")]
        public int OrderRemainingQuantity { get; set; }

        [DataMember(Name = "executionLegs")]
        public List<ExecutionLeg> ExecutionLegs { get; set; }
    }
}
