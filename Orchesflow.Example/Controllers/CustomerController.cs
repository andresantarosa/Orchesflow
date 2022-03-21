using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orchesflow.Example.Handlers.AddCustomer;
using Orchesflow.Orchestration;

namespace Orchesflow.Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IOrchestrator _orchestrator;

        public CustomerController(IOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(AddCustomerCommand command)
        {
            var result =
                await _orchestrator.SendCommand<AddCustomerCommand, AddCustomerCommandResponseViewModel>(command);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Messages);

        }
    }
}