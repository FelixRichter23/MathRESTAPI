using MathAPI.Models;

namespace MathAPI.Repositories
{
    public class CalculationRepository : ICalculationRepository
    {
        private readonly ApplicationDBContext _context;
        public CalculationRepository(ApplicationDBContext context) {
            _context = context;
        }

        public Calculation CreateCalculation()
        {
            throw new NotImplementedException();
        }

        public Calculation DeleteCalculation(int id)
        {
            throw new NotImplementedException();
        }

        public Calculation GetCalculation(int id)
        {
            throw new NotImplementedException();
        }

        public Calculation UpdateCalculation(int id)
        {
            throw new NotImplementedException();
        }
    }
}
