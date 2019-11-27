using Serilog;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Components;
using Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Entities;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Pipelines.Blocks.EntityViews
{
    public class GetFrequentlyBoughtTogetherViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public GetFrequentlyBoughtTogetherViewBlock(ViewCommander viewCommander, IFindEntityPipeline findEntityPipeline)
        {
            _viewCommander = viewCommander;
            _findEntity = findEntityPipeline;
        }

        private readonly ViewCommander _viewCommander;
        private readonly IFindEntityPipeline _findEntity;

        public override async Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: {Constants.Text.CannotBeNull}");

            var request = this._viewCommander.CurrentEntityViewArgument(context.CommerceContext);
            var catalogViewsPolicy = context.GetPolicy<KnownCatalogViewsPolicy>();

            try

            {

                var isConnectView = arg.Name.Equals(catalogViewsPolicy.ConnectSellableItem, StringComparison.OrdinalIgnoreCase);
                var isMasterView = arg.Name.Equals(catalogViewsPolicy.Master, StringComparison.OrdinalIgnoreCase);

                var isVariationView = request.ViewName.Equals(catalogViewsPolicy.Variant, StringComparison.OrdinalIgnoreCase);
                var isSellableItemView = request.ViewName.Equals(catalogViewsPolicy.SellableItem, StringComparison.OrdinalIgnoreCase);

                //var ismasterView = arg.Name.Equals(catalogViewsPolicy.SellableItem, StringComparison.OrdinalIgnoreCase) && request.EntityId.ToLower().Contains("sellableitem");


                var targetView = arg;


                // Make sure that we target the correct views
                if (string.IsNullOrEmpty(request.ViewName) ||
                    !request.ViewName.Equals(catalogViewsPolicy.Master, StringComparison.OrdinalIgnoreCase) &&
                    !request.ViewName.Equals(catalogViewsPolicy.Details, StringComparison.OrdinalIgnoreCase) &&
                    !request.ViewName.Equals(catalogViewsPolicy.SellableItem, StringComparison.OrdinalIgnoreCase) &&
                    !request.ViewName.Equals(Constants.View.FrequentlyBoughtTogetherProductsView, StringComparison.OrdinalIgnoreCase) &&
                    !isConnectView)
                {
                    return arg;
                }


                // Only proceed if the current entity is a sellable item
                var sellableItem = request.Entity as SellableItem;
                if (sellableItem == null)
                {
                    return arg;
                }

                var variationId = string.Empty;



                // Check if the edit action was requested
                var isEditView = !string.IsNullOrEmpty(arg.Action) && arg.Action.Equals(Constants.View.FrequentlyBoughtTogetherProductsView, StringComparison.OrdinalIgnoreCase);
                if (!isEditView)
                {
                    // Create a new view and add it to the current entity view.
                    var view = new EntityView
                    {
                        Name = Constants.View.FrequentlyBoughtTogetherProductsView,
                        DisplayName = Constants.Text.ProductFieldDisplayName,
                        EntityId = arg.EntityId,
                        ItemId = string.Empty,
                        EntityVersion = sellableItem.EntityVersion
                    };

                    arg.ChildViews.Add(view);

                    targetView = view;
                }


                // Get Entity
                var fbtEntity = await _findEntity.Run(new FindEntityArgument(typeof(FbtEntity), $"Entity-Fbt-{sellableItem.ProductId}", false), context.ContextOptions).ConfigureAwait(false) as FbtEntity;



                if (isConnectView || isEditView || isMasterView)
                {

                    var component = sellableItem.GetComponent<BoughtTogetherComponent>(variationId);

                    AddPropertiesToView(targetView, fbtEntity, !isEditView);

                }


            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{Constants.Text.GetFrequentlyBoughtTogetherViewBlock}: {ex.Message}");
                throw;
            }
            return arg;
        }

        private void AddPropertiesToView(EntityView entityView, FbtEntity fbtEntity, bool isReadOnly)
        {
            var sitecoreIds = string.Empty;
            var productIds = string.Empty;
            var productNames = string.Empty;

            if (fbtEntity.BoughtTogetherProducts != null && fbtEntity.BoughtTogetherProducts.Any() && fbtEntity.BoughtTogetherProducts.Count > 0)
            {
                var products = fbtEntity.BoughtTogetherProducts.OrderByDescending(x => x.Times).Take(10).ToList();

                var sitecoreIdList = products.Select(p => p.SitecoreId).ToList();
                var ProductIdList = products.Select(p => p.ProductId).ToList();
                var ProductNamesList = products.Select(p => p.Name + "(" + p.Times + ")").ToList();


                sitecoreIds = string.Join("|", sitecoreIdList);
                productIds = string.Join("|", ProductIdList);
                productNames = string.Join("|", ProductNamesList);

            }

            entityView.Properties.Add(

            new ViewProperty
            {
                Name = "ProductNames",
                DisplayName = "Bough Together Names and Times",
                RawValue = productNames,
                 Value = productNames,
                IsReadOnly = isReadOnly,
                IsRequired = false
            });

           entityView.Properties.Add(
           new ViewProperty
            {
                Name = "ProductIds",
                DisplayName = "Bough Together Product Ids",
                RawValue = productIds,
               Value = productNames,
               IsReadOnly = isReadOnly,
                IsRequired = false
            });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "SitecoreIds",
                    DisplayName = "Bough Together Sitecore Guids",
                    RawValue = sitecoreIds,
                    Value = productNames,
                    IsReadOnly = isReadOnly,
                    IsRequired = false
                }
             );

        }

    }
}
