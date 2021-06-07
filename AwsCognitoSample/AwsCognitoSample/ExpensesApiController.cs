using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AwsCognitoSample
{
    [Authorize]
    [ApiController, Route("v1/expenses")]
    public class ExpensesApiController : ControllerBase
    {
        [ProducesResponseType(typeof(Expense), StatusCodes.Status200OK)]
        [HttpPost, Route("get-expense")]
        public async Task<IActionResult> GetExpense()
        {
            return Ok(new Expense
            {
                Id = Guid.NewGuid(),
                Date = DateTimeOffset.UtcNow,
                Title = "TestExpense",
                TotalAmount = 100
            });
        }
        
        public class Expense
        {
            public Guid Id { get; set; }
            public DateTimeOffset Date { get; set; }
            public string Title { get; set; }
            public decimal TotalAmount { get; set; }
        }
    }
}