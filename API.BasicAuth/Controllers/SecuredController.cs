    // -------------------------------------------------------------------------------------------------
    // Copyright (c) Johan Boström. All rights reserved.
    // Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
    // -------------------------------------------------------------------------------------------------

    namespace API.BasicAuth.Controllers
    {
        using System.Collections.Generic;
        using Attributes;
        using Microsoft.AspNetCore.Mvc;

        [BasicAuthorize("my-example-realm.com")]
        [Route("api/[controller]")]
        public class SecuredController : Controller
        {
            // GET api/secured
            [HttpGet]
            public IEnumerable<string> Get()
            {
                return new[] { "Secret 1", "Secret 2" };
            }
        }
    }