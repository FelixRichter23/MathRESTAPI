using MathAPI.Models;
using MathAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MathAPI.Controllers
{
    [Route("api/calculations")]
    [ApiController]
    public class CalculationsController : ControllerBase
    {
        private readonly ICalculationRepository _calculationsRepo;

        public CalculationsController(ICalculationRepository calculationsRepo) {
            _calculationsRepo = calculationsRepo;
        }

        [HttpPost]
        public ActionResult<Calculation> CreateCalculation([FromBody] string expression)
        {
            return new Calculation("");
        }

        [HttpGet("{id:int}")]
        public ActionResult<Calculation> GetCalculation(int id)
        {
            return new OkObjectResult(new
            {
                Calculation = "Test"
            });
        }

        [HttpPut("{id:int}")]
        public ActionResult<Calculation> UpdateCalculation(int id)
        {
            return new Calculation("");
        }

        [HttpDelete("{id:int}")]
        public ActionResult<Calculation> DeleteCalculation(int id)
        {
            return new Calculation("");
        }
    }
}
