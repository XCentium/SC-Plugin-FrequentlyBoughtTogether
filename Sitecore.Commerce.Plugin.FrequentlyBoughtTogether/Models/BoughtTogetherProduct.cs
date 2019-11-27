using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Models
{
    public class BoughtTogetherProduct
    {

        public string ProductId { get; set; }
        public string Name { get; set; }
        public string SitecoreId { get; set; }
        public int Times { get; set; }
    }
}
