
namespace Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Components
{
    using Sitecore.Commerce.Core;
    using System.Collections.Generic;

    public class FrequentlyBoughtTogetherComponent : Component
    {
        public FrequentlyBoughtTogetherComponent()
        {
            this.FrequentlyBoughtTogetherProductList = (IList<string>)new List<string>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        //public string FrequentlyBoughtTogetherProductList { get; set; }

        public IList<string> FrequentlyBoughtTogetherProductList { get; set; } = (IList<string>)new List<string>();
    }
}
