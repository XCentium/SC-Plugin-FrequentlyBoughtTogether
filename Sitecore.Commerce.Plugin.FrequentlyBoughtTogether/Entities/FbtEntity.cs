using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Entities
{
    public class FbtEntity : CommerceEntity
    {

        public FbtEntity()
        {
            this.BoughtTogetherProducts = (IList<BoughtTogetherProduct>)new List<BoughtTogetherProduct>();
        }

        public IList<BoughtTogetherProduct> BoughtTogetherProducts { get; set; } = (IList<BoughtTogetherProduct>)new List<BoughtTogetherProduct>();

    }
}
