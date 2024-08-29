using MathAPI.Models;
using SqlKata.Execution;

namespace MathAPI.Repositories
{
    public class CalculationRepository : ICalculationRepository
    {
        private readonly QueryFactory _db;
        private readonly string _tableName = "Calculations"; 
        public CalculationRepository(QueryFactory db) {
            _db = db;
        }

        public int SaveCalculation(Calculation calculation)
        {
            return _db.Query(_tableName).InsertGetId<int>(new
            {
                Expression = calculation.expressionWithRelations,
                Result = calculation.result,
                LastUpdate = calculation.lastUpdate
            });
        }

        public Calculation GetCalculation(int id)
        {
            return _db.Query(_tableName).Where("Id", id).FirstOrDefault<Calculation>();
        }

        public void UpdateCalculation(Calculation calculation)
        {
            _db.Query(_tableName).Where("Id",calculation.id).Update(new
            {
                Expression = calculation.expression,
                Result = calculation.result,
                LastUpdate = calculation.lastUpdate
            });
        }

        public void DeleteCalculation(int id)
        {
            _db.Query(_tableName).Where("Id", id).Delete();
        }
    }
}
