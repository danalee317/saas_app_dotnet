using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.Data.ComponentModel
{
    internal class UnderwritingTypeAttribute : Attribute
    {
        public UnderwritingTypeAttribute(UnderwritingType type)
        {
            Type = type;
        }

        public UnderwritingType Type { get; }
    }
}
