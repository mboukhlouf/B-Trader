using System;

namespace B_Trader.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FormField : Attribute
    {
        public String Name { get; set; }
    }
}
