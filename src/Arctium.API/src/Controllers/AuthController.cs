// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Arctium.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        [HttpPost]
        public string Post([FromBody]string value)
        {
            var headers = HttpContext.Request?.Headers;

            // This return the sql data reader data as Json(object[][]).
            if (HttpContext.Request?.Headers?.TryGetValue("Entity", out var entityName) == true)
                return JsonConvert.SerializeObject(Database.Auth.ProcessApiRequest(value.ToString(), entityName));

            return JsonConvert.SerializeObject(new object[1][]);
        }
    }
}
