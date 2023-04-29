using System;

namespace Combat.States {
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ListenChangeAttribute : Attribute { }
}
