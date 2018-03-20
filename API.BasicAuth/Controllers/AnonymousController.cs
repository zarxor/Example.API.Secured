// -------------------------------------------------------------------------------------------------
//  <copyright file="SecuredController.cs" company="Bonnierförlagen">
//      © Bonnierförlagen 2018
//  </copyright>
// -------------------------------------------------------------------------------------------------

namespace API.BasicAuth.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class AnonymousController : Controller
    {
        // GET api/anonymous
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] { "Open 1", "Open 2" };
        }
    }
}