using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingBucketlistTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        private IEnumerable<string> Classes = new []{"A", "B", "C", "D"};
        private string propertyClass;
        private string neighborhoodClass;
        private string HowUnderwritingWasDeterminedPlaceholder = @"Where did you get the numbers that you used?
(P&L last 12 months trailing, 201X YTD, 201X annualized, 201X Actual, or Proforma??)";
        protected override void OnInitialized()
        {
            propertyClass = GetClass(Property.PropertyClass);
            neighborhoodClass = GetClass(Property.NeighborhoodClass);
        }

        private void UpdateProperty()
        {
            Property.PropertyClass = GetClass(propertyClass);
        }

        private void UpdateNeighborhood()
        {
            Property.NeighborhoodClass = GetClass(neighborhoodClass);
        }

        private PropertyClass GetClass(string value)
        {
            return Enum.Parse<PropertyClass>($"Class{value}");
        }

        private string GetClass(PropertyClass @class)
        {
            return @class switch
            {
                PropertyClass.ClassA => "A",
                PropertyClass.ClassB => "B",
                PropertyClass.ClassC => "C",
                PropertyClass.ClassD => "D",
                _ => "Unknown"
            };
        }
    }
}
