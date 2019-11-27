
namespace Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Components
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Models;
    using System;
    using System.Collections.Generic;

    public class BoughtTogetherComponent : Component
    {
        public BoughtTogetherComponent()
        {
            this.BoughtTogetherProducts = (IList<BoughtTogetherProduct>)new List<BoughtTogetherProduct>();
        }

        public IList<BoughtTogetherProduct> BoughtTogetherProducts { get; set; } = (IList<BoughtTogetherProduct>)new List<BoughtTogetherProduct>();


    }
}
