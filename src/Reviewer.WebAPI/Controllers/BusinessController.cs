using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reviewer.Services;
using Reviewer.SharedModels;

namespace Reviewer.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Business")]
    public class BusinessController : Controller
    {
        // api/business
        [HttpGet]
        public async Task<IActionResult> Get() 
        {
            try
            {
                var cosmosService = new CosmosDataService();

                var businesses = await cosmosService.GetBusinesses();

                return new OkObjectResult(businesses);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write($"*** ERROR: {ex.Message}");

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}