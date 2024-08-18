using MathAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MathAPI.Repositories
{
    public interface ICalculationRepository
    {
        public Calculation CreateCalculation();

        public Calculation GetCalculation(int id);

        public Calculation UpdateCalculation(int id);

        public Calculation DeleteCalculation(int id);
    }
}
