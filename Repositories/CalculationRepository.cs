using MathAPI.Data;
using MathAPI.Models;
using System.Data.SQLite;

namespace MathAPI.Repositories
{
    public class CalculationRepository : ICalculationRepository
    {
        private readonly ApplicationDbContext _context;
        public CalculationRepository(ApplicationDbContext context) {
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
