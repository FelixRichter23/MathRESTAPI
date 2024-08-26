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
        private readonly ICalculationRepository _repository;

        public Calculation()
        {

        }

        public Calculation(string expression, ICalculationRepository repository, int? id = null)
        {
            this.id = id;

            if (expression == null) throw new ArgumentNullException();

            expression = expression.Replace(" ", string.Empty);

            _relations = new Dictionary<int, Lazy<Calculation>>();

            _repository = repository;

            if (expression == string.Empty) throw new ArgumentNullException();

            CheckBracklets(expression);

            LoadRelations(expression);

            this.expression = expression;
        }

        /// <summary>
        /// Ensures that brackets in the expression are correctly balanced and formatted.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>True if the brackets are in a valid format.</returns>
        /// <exception cref="ArgumentException"></exception>

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

        /// <summary>
        /// Identifies and loads related calculations referenced by IDs within curly braces in the expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <exception cref="ArgumentException"></exception>

        private void LoadRelations(string expression) {
            int index = expression.IndexOf('{');

            while (index != -1) {
                index = expression.IndexOf('{', index);
                
                string idAsString = expression.Substring(index, expression.IndexOf('}', index) - index);

                if(!int.TryParse(idAsString, out int id)) { throw new ArgumentException("The relation id is not a number."); }

                _relations.Add(id, new Lazy<Calculation>(() => { return _repository.GetCalculation(id); }));
            }
        }

        /// <summary>
        /// Performs the calculation and returns it.
        /// </summary>
        /// <returns>Result as double</returns>

        public Calculation Calculate()
        {
            result = calculate();
            lastUpdate = DateTime.Now;
            return this;
        }

        private double calculate()
        {
            result = EvaluateExpression(expression);
            lastUpdate = DateTime.Now;
            return result;
        }
    }
}
