using System.Runtime.Serialization;

namespace TDAmeritradeZorro.Classes.TDA
{
    [DataContract]
    public class OrderLeg
    {
        [DataMember(Name = "orderLegType")]
        public string OrderItemType { get; set; }

        [DataMember(Name = "legId")]
        public int LegId { get; set; }

        [DataMember(Name = "instrument")]
        public Instrument Instrument { get; set; }

        [DataMember(Name = "instruction")]
        public string Instruction { get; set; }

        [DataMember(Name = "positionEffect")]
        public string PositionEffect { get; set; }

        [DataMember(Name = "quantity")]
        public double Quantity { get; set; }
    }
}
