using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Commands;
using Sitecore.Commerce.Plugin.Orders;

using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Pipelines.Blocks
{
    public class UpdateFrequentlyBoughtTogetherBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public UpdateFrequentlyBoughtTogetherBlock(CommerceCommander commerceCommander)
        {
            _commerceCommander = commerceCommander;
        }
        public override async Task<Order> Run(Order order, CommercePipelineExecutionContext context)
        {
            var process = await _commerceCommander.Command<UpdateFrequentlyBoughtTogetherCommand>().Process(context.CommerceContext, order);

            return order;
        }
    }
}
