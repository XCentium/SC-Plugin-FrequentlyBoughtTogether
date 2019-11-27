// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Plugin.FrequentlyBoughtTogether
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http.OData;

    using Microsoft.AspNetCore.Mvc;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.FrequentlyBoughtTogether.Commands;
    using Sitecore.Commerce.Plugin.Orders;

    /// <inheritdoc />
    /// <summary>
    /// Defines a controller
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.CommerceController" />
    public class CommandsController : CommerceController
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Commerce.Plugin.Sample.CommandsController" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="globalEnvironment">The global environment.</param>
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
            : base(serviceProvider, globalEnvironment)
        {
        }


        /// <summary>
        /// Rebuild Frequently Bought Together Record
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RebuildFrequentlyBoughtTogether()")]
        public async Task<IActionResult> RebuildFrequentlyBoughtTogether([FromBody] ODataActionParameters value)
        {

            CommandsController commandsController = this;

            var orders = (IEnumerable<Order>)(await commandsController.Command<FindEntitiesInListCommand>().Process<Order>(this.CurrentContext, CommerceEntity.ListName<Order>(), 0, int.MaxValue)).Items.ToList<Order>();

            if (orders.Any())
            {
                foreach(var order in orders)
                {
                    var command = this.Command<UpdateFrequentlyBoughtTogetherCommand>();
                    var result = await command.Process(this.CurrentContext, order);
                }
            }


            return new ObjectResult(new { Success = true});
        }


    }
}

