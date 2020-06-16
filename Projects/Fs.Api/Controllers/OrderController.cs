using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Fs.Core.Contracts;
using Fs.Data;
using Fs.Core.Interfaces.Services;
using Fs.Core.Exceptions;
using Fs.Business.Base;

namespace Fs.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class OrderController : CustomController
    {
        private readonly LoggerContext _context;
        private readonly IOrderService _service;

        public OrderController(ILogger<OrderController> logger, LoggerContext context, IOrderService service, IHttpContextAccessor httpContextAccessor)
            : base(logger, httpContextAccessor)
        {
            _context = context;
            _service = service;
        }

        // GET: api/Order
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            ReportUser("GetAllOrders()");

            IEnumerable<OrderDto> orders = await _service.GetOrders();

            return Ok(orders);

        }

        [HttpGet("search")]
        public async Task<IActionResult> GetOrders(string order)
        {
            ReportUser("GetAllOrders(string order)");

            IEnumerable<OrderDto> orders = await _service.GetOrders(order);

            return Ok(orders);
        }

        // GET: api/Order/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Order
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderDto order)
        {
            ReportUser("Post([FromBody] OrderDto order)");

            OrderDto savedOrder = await _service.SaveOrder(order);

            foreach (ReportDto report in savedOrder.Reports)
                report.Order = null;

            return Ok(savedOrder);
        }


        // PUT: api/Order/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        #region Reports

        [HttpGet("reports")]
        public async Task<IActionResult> GetReports()
        {
            ReportUser("GetReports()");

            IEnumerable<ReportDto> reports = await _service.GetReports();

            foreach (ReportDto report in reports)
                report.Order.Reports = null;

            return Ok(reports);

        }

        [HttpPost("reports")]
        public async Task<IActionResult> PostReport([FromBody] ReportDto report)
        {
            ReportUser("PostReport([FromBody] ReportDto report)");

            ReportDto savedReport = await _service.SaveReport(report);

            return Ok(savedReport);
        }

        [HttpPut("reports")]
        public async Task<IActionResult> PutReport([FromBody] ReportDto report)
        {
            ReportUser("PutReport([FromBody] ReportDto report)");

            ReportDto savedReport = await _service.SaveReport(report);

            return Ok(savedReport);
        }

        #endregion Reports
    }
}
