using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwrting;
using MultiFamilyPortal.Extensions;
using Telerik.Blazor.Components;
using Telerik.DataSource;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingSection
    {
        private PortalNotification notification { get; set; }

        [Required]
        [Parameter]
        public UnderwritingColumn Column { get; set; }

        [Required]
        [Parameter]
        public UnderwritingType Type { get; set; }

        [Required]
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public RenderFragment TopLevelMenu { get; set; }

        private TelerikGrid<UnderwritingAnalysisLineItem> grid { get; set; }

        private UnderwritingAnalysisLineItem NewItem;
        private UnderwritingAnalysisLineItem EditItem;
        private readonly ObservableRangeCollection<UnderwritingAnalysisLineItem> Items = new ObservableRangeCollection<UnderwritingAnalysisLineItem>();
        private IEnumerable<ExpenseSheetType> ExpenseTypes = new[]{ExpenseSheetType.T12, ExpenseSheetType.T6, ExpenseSheetType.T4, ExpenseSheetType.T3, ExpenseSheetType.T1};

        private IEnumerable<UnderwritingAnalysisLineItem> _allItems =>
            Column == UnderwritingColumn.Sellers ? Property.Sellers : Property.Ours;
        protected override void OnInitialized()
        {
            if(Column == UnderwritingColumn.Sellers)
            {
                Items.ReplaceRange(Property.Sellers.Where(x => x.Category.GetLineItemType() == Type).OrderBy(x => x.Category));
            }
            else
            {
                Items.ReplaceRange(Property.Ours.Where(x => x.Category.GetLineItemType() == Type).OrderBy(x => x.Category));
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                var desiredState = new GridState<UnderwritingAnalysisLineItem>
                {
                    GroupDescriptors = new List<GroupDescriptor> {
                        new GroupDescriptor {
                            Member = nameof(UnderwritingAnalysisLineItem.Category),
                            MemberType = typeof(UnderwritingCategory),
                        }
                    }
                };
                await grid.SetState(desiredState);
            }
        }


        private IEnumerable<UnderwritingCategory> AllowableCategories()
        {
            if (Column == UnderwritingColumn.Ours)
            {
                if (Type == UnderwritingType.Income)
                    return IncomeCategories.Where(x => x != UnderwritingCategory.PhysicalVacancy);
                return ExpenseCategories.Where(x => x != UnderwritingCategory.Management);
            }

            if (Type == UnderwritingType.Income)
                return IncomeCategories;

            return ExpenseCategories;
        }

        private void OnCreate()
        {
            NewItem = new UnderwritingAnalysisLineItem()
            {
                ExpenseType = ExpenseSheetType.T12
            };
        }

        private void SaveNewLineItem()
        {
            if (NewItem is null)
                return;
            if (string.IsNullOrEmpty(NewItem.Description))
                NewItem.Description = NewItem.Category.GetDisplayName();

            NewItem.Id = Guid.NewGuid();
            Items.Add(NewItem);
            if(Column == UnderwritingColumn.Sellers)
            {
                Property.AddSellerItem(NewItem);
            }
            else
            {
                Property.AddOurItem(NewItem);
            }

            notification.ShowSuccess($"{Type} line item {NewItem.Category.GetDisplayName()}");
            NewItem = null;
        }

        private void ShowEditDialog(GridCommandEventArgs args)
        {
            EditItem = args.Item as UnderwritingAnalysisLineItem;
        }

        private void SaveEditLineItem()
        {
            if (EditItem is null)
                return;
            if (string.IsNullOrEmpty(EditItem.Description))
                EditItem.Description = EditItem.Category.GetDisplayName();
            //Items.Add(EditItem);

            notification.ShowSuccess($"{Type} line item {EditItem.ExpenseType.GetDisplayName()}");
            EditItem = null;
        }

        private void OnDelete(GridCommandEventArgs args)
        {
            var item = args.Item as UnderwritingAnalysisLineItem;
            Items.Remove(item);
            if(Column == UnderwritingColumn.Sellers)
            {
                Property.RemoveSellerItem(item);
            }
            else
            {
                Property.RemoveOurItem(item);
            }
        }

        private static readonly IEnumerable<UnderwritingCategory> IncomeCategories =
            Enum.GetValues<UnderwritingCategory>().Where(x => x.GetLineItemType() == UnderwritingType.Income);

        private static readonly IEnumerable<UnderwritingCategory> ExpenseCategories =
            Enum.GetValues<UnderwritingCategory>().Where(x => x.GetLineItemType() == UnderwritingType.Expense);
    }
}
