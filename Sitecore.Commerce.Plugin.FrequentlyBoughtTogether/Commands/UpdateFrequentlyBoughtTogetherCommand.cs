
using System.Linq;

namespace Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Entities;
    using Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Models;
    using Sitecore.Commerce.Plugin.Orders;


    public class UpdateFrequentlyBoughtTogetherCommand : CommerceCommand
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;
        private readonly GetSellableItemCommand _getSellableItemCommand;
        private readonly IFindEntityPipeline _findEntity;

        public UpdateFrequentlyBoughtTogetherCommand(IPersistEntityPipeline persistEntityPipeline, GetSellableItemCommand getSellableItemCommand, IFindEntityPipeline findEntityPipeline)
        {
            _persistEntityPipeline = persistEntityPipeline;
            _getSellableItemCommand = getSellableItemCommand;
            _findEntity = findEntityPipeline;
        }
        public async Task<Order> Process(CommerceContext commerceContext, Order order)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {

                 // if no order lines, return
                if (!order.Lines.Any()) return order;

                // order.Line < 2 return
                if (order.Lines.Count < 2) return order;

                // get lines
                var lines = order.Lines.ToList();

                // get sellable items list
                var sellableItemsList = new List<SellableItem>();

                foreach(var cartLineComponent in lines)
                {
                    var sellableItem = await _getSellableItemCommand.Process(commerceContext, cartLineComponent.ItemId, false).ConfigureAwait(false);
                    if (sellableItem != null)
                    {
                        sellableItemsList.Add(sellableItem);
                    }
                }

                if (!sellableItemsList.Any() || sellableItemsList.Count < 2) return order;

            // check the BoughtTogetherComponent on each sellable item and update it accordingly
            foreach (var sellableItem in sellableItemsList)
            {
                // get its sellable item and get the guid
                var sitecoreGuid = sellableItem.SitecoreId;

                // get others
                var otherSellableItems = sellableItemsList.Where(x => x.FriendlyId != sellableItem.FriendlyId).ToList();

                // loop through other product lines
                foreach (var otherSellableItem in otherSellableItems)
                {

                    // Get Entity
                    var fbtEntity = await _findEntity.Run(new FindEntityArgument(typeof(FbtEntity), $"Entity-Fbt-{otherSellableItem.ProductId}", false), commerceContext.PipelineContext.ContextOptions).ConfigureAwait(false) as FbtEntity;

                    if (fbtEntity == null)
                    {

                        fbtEntity = new FbtEntity
                        {
                            Id = $"Entity-Fbt-{otherSellableItem.ProductId}",
                            FriendlyId = $"Entity-Fbt-{otherSellableItem.ProductId}",
                        };
                    }

                    // if the entity has data, get it and update it accordingly

                    if (fbtEntity.BoughtTogetherProducts.Any())
                    {
                        var addedList = fbtEntity.BoughtTogetherProducts.ToList();

                        // if sellableitem with matching ID exists on addedList, increment its count by 1 else, add it

                        var exists = addedList.FirstOrDefault(x => x.ProductId == sellableItem.FriendlyId);

                        if (exists != null)
                        {
                            // update in list
                            addedList.Where(x => x.ProductId == sellableItem.FriendlyId).ToList().Select(x => { x.Times = x.Times + 1; return x; });
                        }
                        else
                        {
                            addedList.Add(
                                new BoughtTogetherProduct
                                {
                                    ProductId = sellableItem.ProductId,
                                    Name = sellableItem.Name,
                                    SitecoreId = sellableItem.SitecoreId,
                                    Times = 1
                                }
                            );
                        }

                        fbtEntity.BoughtTogetherProducts = addedList;

                    }
                    else
                    {
                        var addedList = fbtEntity.BoughtTogetherProducts.ToList();
                        addedList.Add(
                            new BoughtTogetherProduct
                            {
                                ProductId = sellableItem.ProductId,
                                Name = sellableItem.Name,
                                SitecoreId = sellableItem.SitecoreId,
                                Times = 1
                            }
                        );

                        fbtEntity.BoughtTogetherProducts = addedList;

                    }

                }
            }

                return order;
            }
        }
    }
}
