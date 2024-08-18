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
        public Calculation CreateCalculation()
        {
            return new Calculation {};
        }

        [HttpGet("{id:int}")]
        public Calculation GetCalculation(int id)
        {
            return new Calculation {};
        }

        [HttpPut("{id:int}")]
        public Calculation UpdateCalculation(int id)
        {
            return new Calculation {};
        }

        [HttpDelete("{id:int}")]
        public Calculation DeleteCalculation(int id)
        {
            return new Calculation {};
        }
    }
}
