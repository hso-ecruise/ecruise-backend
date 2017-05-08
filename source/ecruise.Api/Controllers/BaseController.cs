using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecruise.Api.Controllers
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class BaseController : Controller
    {
    }
}