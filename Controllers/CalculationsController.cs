using MathAPI.Models;
using MathAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MathAPI.Controllers
{
    [Route("api/calculations")]
    [ApiController]
    public class CalculationsController : ControllerBase
    {
        private readonly ICalculationRepository _calculationsRepo;
        private readonly IRelationRepository _relationRepository;

        public CalculationsController(ICalculationRepository calculationsRepo, IRelationRepository relationRepo) {
            _calculationsRepo = calculationsRepo;
            _relationRepository = relationRepo;
        }

        [HttpPost]
        public ActionResult<Calculation> CreateCalculation([FromBody] string expression)
        {
            Calculation calc;

            try
            {
                calc = new Calculation(expression, _calculationsRepo, _relationRepository);

                calc.Calculate();

                calc.Save();
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return new OkObjectResult(calc);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Calculation> GetCalculation(int id)
        {
            Calculation calc;

            try 
            {
                calc = Calculation.GetCalculation(id, _calculationsRepo);

                if (calc == null) 
                {
                    throw new ArgumentException($"A calculation with id {id} does not exist.");
                }
            } 
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }

            return new OkObjectResult(calc);
        }

        [HttpPut("{id:int}")]
        public ActionResult<Calculation> UpdateCalculation(int id, [FromBody] string expression)
        {
            Calculation calc;

            try
            {
                calc = _calculationsRepo.GetCalculation(id);

                if (calc == null)
                {
                    throw new ArgumentException($"A calculation with id {id} does not exist.");
                }

                calc._calculationRepository = _calculationsRepo;
                calc._relationRepository = _relationRepository;

                calc.expression = expression;

                calc.Calculate();
                calc.Save();
                calc.MarkDependentsAsDeprecated();

            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return new OkObjectResult(calc);
        }

        [HttpDelete("{id:int}")]
        public ActionResult DeleteCalculation(int id)
        {
            try
            {
                _calculationsRepo.DeleteCalculation(id);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }

            return new OkResult();
        }
    }
}
