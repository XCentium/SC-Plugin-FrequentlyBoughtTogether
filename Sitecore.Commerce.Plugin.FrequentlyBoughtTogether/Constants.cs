using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Commerce.Plugin.FrequentlyBoughtTogether
{
    public class Constants
    {
        internal struct View
        {

            /// <summary>
            /// 
            /// </summary>
            public const string FrequentlyBoughtTogetherProductsView = "FrequentlyBoughtTogetherProductsView";
        }

        internal struct Text
        {
            public const string GetFrequentlyBoughtTogetherViewBlock = "GetFrequentlyBoughtTogetherViewBlock";

            public const string CannotBeNull = "The argument cannot be null.";

            public const string ProductFieldDisplayName = "Frequently Bought Together Products";
        }

    }
}
