using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwrting;
using MultiFamilyPortal.Extensions;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingOurInfoTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private bool Refreshing;

        private readonly ObservableRangeCollection<UnderwritingAnalysisLineItem> Items = new ();

        protected override void OnParametersSet()
        {
            if (Property?.Ours?.Any() ?? false)
                Items.ReplaceRange(Property.Ours);
        }

        private async Task UpdateFromSeller()
        {
            Refreshing = true;

            var grouped = Property.Sellers.GroupBy(x => x.Category);

            var allCategories = Enum.GetValues<UnderwritingCategory>();
            var guidanceList = await _client.GetFromJsonAsync<IEnumerable<UnderwritingGuidance>>($"/api/admin/underwriting/guidance?market={Property.Market}");
            var updatedItems = new List<UnderwritingAnalysisLineItem>();
            foreach (var category in allCategories)
            {
                if(category == UnderwritingCategory.PhysicalVacancy)
                {
                    var vacancyRate = Math.Min(Property.PhysicalVacancy, Property.MarketVacancy);
                    if (vacancyRate < 0.05)
                        vacancyRate = 0.05;

                    var vacancy = new UnderwritingAnalysisLineItem {
                        Amount = vacancyRate * Property.GrossPotentialRent,
                        Category = category,
                        Description = category.GetDisplayName(),
                        ExpenseType = ExpenseSheetType.T12
                    };
                    updatedItems.Add(vacancy);
                    continue;
                }
                else if(category == UnderwritingCategory.Management)
                {
                    var management = new UnderwritingAnalysisLineItem {
                        Amount = Property.Management * Property.GrossPotentialRent,
                        Category = category,
                        Description = category.GetDisplayName(),
                        ExpenseType = ExpenseSheetType.T12
                    };
                    updatedItems.Add(management);
                    continue;
                }

                var group = grouped.FirstOrDefault(x => x.Key == category);
                double total = 0;
                if(group != null)
                {
                    total = group.Select(x => x.AnnualizedTotal).Sum();
                }

                var item = new UnderwritingAnalysisLineItem {
                    Amount = total,
                    Category = category,
                    Description = category.GetDisplayName(),
                    ExpenseType = ExpenseSheetType.T12
                };

                var guidance = guidanceList.FirstOrDefault(x => x.Category == category);
                if (total == 0 && guidance != null)
                {
                    item.UpdateFromGuidance(guidance, Property);
                }

                updatedItems.Add(item);
            }

            Property.ReplaceOurItems(updatedItems);
            Items.ReplaceRange(updatedItems);

            Refreshing = false;
        }
    }
}
