// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Pipelines.Blocks;
using Sitecore.Commerce.Plugin.Orders;

namespace Sitecore.Commerce.Plugin.FrequentlyBoughtTogether
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Pipelines.Blocks.EntityViews;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// The configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config

             .AddPipeline<IPersistOrderPipeline, PersistOrderPipeline>(
                    configure =>
                        {
                            configure.Add<UpdateFrequentlyBoughtTogetherBlock>().After<PersistOrderByOrderIdIndexBlock>();
                        })                    
                        .ConfigurePipeline<IGetEntityViewPipeline>(c =>                                
                        {
                            c.Add<GetFrequentlyBoughtTogetherViewBlock>().After<GetSellableItemDetailsViewBlock>();
                        })

               .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>()));

            services.RegisterAllCommands(assembly);
        }
    }
}