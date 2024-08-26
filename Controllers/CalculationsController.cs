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

        public CalculationsController(ICalculationRepository calculationsRepo) {
            _calculationsRepo = calculationsRepo;
        }

        [HttpPost]
        public ActionResult<Calculation> CreateCalculation([FromBody] string expression)
        {
            return temp(null, _calculationsRepo, expression);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Calculation> GetCalculation(int id)
        {
            return new OkObjectResult(_calculationsRepo.GetCalculation(id));
        }

        [HttpPut("{id:int}")]
        public ActionResult<Calculation> UpdateCalculation(int id, [FromBody] string expression)
        {
            return temp(id, _calculationsRepo, expression);
        }

        [HttpDelete("{id:int}")]
        public ActionResult DeleteCalculation(int id)
        {
            _calculationsRepo.DeleteCalculation(id);

            return new OkResult();
        }

        private ActionResult<Calculation> temp(int? id, ICalculationRepository _calculationsRepo, string expression)
        {
            Calculation calculation = null;

            try
            {
                calculation = new Calculation(expression, _calculationsRepo, id);
                calculation.Calculate();
                calculation.Save();
            }
            catch (ArgumentException ae)
            {
                return new BadRequestObjectResult(ae.Message);
            }
            catch (Exception e)
            {
                return new ObjectResult(new { StatusCode = 500, MessageContent = e.Message });
            }

            if (calculation is not null)
            {
                return new OkObjectResult(calculation);
            }

            return new NoContentResult();
        }
    }
}
