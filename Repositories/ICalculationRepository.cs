using MathAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MathAPI.Repositories
{
    public interface ICalculationRepository
    {
        public int SaveCalculation(Calculation calculation);

        public Calculation GetCalculation(int id);

        public void UpdateCalculation(Calculation calculation);

        public void DeleteCalculation(int id);
    }
}
