﻿using System.Reflection;
using MultiFamilyPortal.Data.ComponentModel;

namespace MultiFamilyPortal.Data.Models
{
    public static class EnumExtensions
    {
        public static UnderwritingType GetLineItemType(this UnderwritingCategory category)
        {
            var type = typeof(UnderwritingCategory);
            var memberData = type.GetMember(category.ToString())?.FirstOrDefault();
            if (memberData is null)
                throw new InvalidOperationException($"Unable to find the memberdata for the Category {category}");

            return memberData.GetCustomAttribute<UnderwritingTypeAttribute>().Type;
        }
    }
}
