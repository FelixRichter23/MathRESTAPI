using MathAPI.Repositories;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace MathAPI.Models
{
    public class Calculation
    {
        public int? id { get; private set; }
        public string expression { get; private set; }
        public double result { get; private set; }
        public DateTime lastUpdate { get; private set; }
        private Dictionary<int, Lazy<Calculation>> _relations;
        private readonly CalculationRepository _repository;
        public Calculation(string expression)
        {
            if (expression == null) throw new ArgumentNullException();

            expression.Replace(" ", string.Empty);

            if (expression == string.Empty) throw new ArgumentNullException();

            CheckBracklets(expression);

            LoadRelations(expression);

            this.expression = expression;
        }

        private static bool CheckBracklets(string expression)
        {
            int count = 0;
            int curlyCount = 0;

            foreach (char bracket in expression.Where(c =>
                c == '(' ||
                c == ')' ||
                c == '{' ||
                c == '}'))
            {
                if (bracket == '(') count++;
                if (bracket == ')') count--;
                if (bracket == '{') curlyCount++;
                if (bracket == '}') curlyCount--;
                if (count < 0 || curlyCount < 0) 
                    throw new ArgumentException("The brackets in the expression are not formatted correctly.");
            }

            if (count != 0 || curlyCount != 0) 
                throw new ArgumentException("The brackets in the expression are not formatted correctly.");

            return true;
        }

        private void LoadRelations(string expression) {
            int index = expression.IndexOf('{');

            while (index != -1) {
                index = expression.IndexOf('{', index);
                
                string idAsString = expression.Substring(index, expression.IndexOf('}', index) - index);

                if(!int.TryParse(idAsString, out int id)) { throw new ArgumentException("The relation id is not a number."); }

                _relations.Add(id, new Lazy<Calculation>(() => { return _repository.GetCalculation(id); }));
            }
        }
        // (1.5 +)*3 + 4^5/2 - log(8;2)

        // 1. Klammern vor Allem
        // 2. Punkt vor Strich
        // 3. Resliche brechnungen

        private double CalculateResult(string expression)
        {
            expression.IndexOf('(');

            return 0;
        }
    }
}
