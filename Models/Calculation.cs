using MathAPI.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace MathAPI.Models
{
    public class Calculation
    {
        public int? id { get; private set; }
        public string expression { get; private set; }
        public string expressionWithRelations { get; private set; }
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

            expression = expression.Replace(" ", string.Empty);
            this.expressionWithRelations = expression;
            expression = expression.Replace(".", ",");
            expression = expression.Trim();

            _relations = new Dictionary<int, Lazy<Calculation>>();

            _repository = repository;

            if (expression == string.Empty) throw new ArgumentNullException();

            CheckBracklets(expression);

            expression = LoadRelations(expression);

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

            foreach (char bracket in expression.Where(c => c == '(' ||c == ')'))
            {
                if (bracket == '(') count++;
                if (bracket == ')') count--;
                if (count < 0)
                {
                    throw new ArgumentException("The brackets in the expression are not formatted correctly.");
                }
            }

            foreach (char bracket in expression.Where(c => c == '[' || c == ']'))
            {
                if (bracket == '[') count++;
                if (bracket == ']') count--;
                if (count < 0 || count > 1)
                {
                    throw new ArgumentException("The curly brackets in the expression are not formatted correctly.");
                }
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

        private string LoadRelations(string expression) 
        {
            expression = expression.Replace("{", "[");
            expression = expression.Replace("}", "]");

            string pattern = @"\[(\d+)\]";

            MatchCollection matches = Regex.Matches(expression, pattern);

            foreach (Match match in matches)
            {
                if (int.TryParse(match.Groups[1].Value, out int number))
                {
                    expression = expression.Replace($"[{number}]", _repository.GetCalculation(number).expression);
                }
            }

            if (expression.Contains("["))
            {
                return LoadRelations(expression);
            }

            return expression;
        }

        /// <summary>
        /// Performs the calculation and returns it.
        /// </summary>
        /// <returns>Result as double</returns>

        public Calculation Calculate()
        {
            result = calculate(expression);
            lastUpdate = DateTime.Now;
            return this;
        }

        private double calculate(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException();
            }

            if (double.TryParse(expression, out double result))
            {
                return result;
            }

            var parenthensisPairs = expression.GetParenthensisIndices(('(', ')'));

            if (parenthensisPairs.Where(par => par.open == 0 && par.close == expression.Length - 1).Any())
            {
                expression = expression.Remove(0, 1);
                expression = expression.Remove(expression.Length - 1, 1);
            }

            if (expression.ContainsNotInParenthensis("+", out int plusIndex) | 
                expression.ContainsNotInParenthensis("-", out int minusIndex))
            {
                if (plusIndex > 0)
                {
                    return
                        calculate(expression.Substring(0, plusIndex))
                        + calculate(expression.Substring(plusIndex + 1, expression.Length - plusIndex - 1));
                }

                if (minusIndex > 0)
                {
                    return
                       calculate(expression.Substring(0, minusIndex))
                       - calculate(expression.Substring(minusIndex + 1, expression.Length - minusIndex - 1));
                }
            }

            if (expression.ContainsNotInParenthensis("*", out int factorIndex) | 
                expression.ContainsNotInParenthensis("/", out int divisorIndex))
            {
                if (factorIndex > 0)
                {
                    return
                        calculate(expression.Substring(0, factorIndex))
                        * calculate(expression.Substring(factorIndex + 1, expression.Length - factorIndex - 1));
                }

                if (divisorIndex > 0)
                {
                    return
                       calculate(expression.Substring(0, divisorIndex))
                       / calculate(expression.Substring(divisorIndex + 1, expression.Length - divisorIndex - 1));
                }
            }

            if (expression.ContainsNotInParenthensis("^", out int powerIndex) | 
                expression.ContainsNotInParenthensis("log", out int logIndex))
            {
                if (powerIndex > 0)
                {
                    return Math.Pow(calculate(expression.Substring(0, powerIndex)),
                        calculate(expression.Substring(powerIndex + 1, expression.Length - powerIndex - 1)));
                }

                if (logIndex >= 0)
                {
                    var logOpen = expression.IndexOf("(", logIndex);
                    var logClose = expression.IndexOf(")", logOpen);
                    var logStrings = expression.Substring(logOpen + 1, logClose - logOpen - 1).Split(";");

                    return Math.Log(calculate(logStrings[0]),
                        calculate(logStrings[1]));
                }
            }

            throw new InvalidOperationException();
        }

        public void Save()
        {
            if (id == null)
            {
                id = _repository.SaveCalculation(this);
            }
            else
            {
                _repository.UpdateCalculation(this);
            }
        }
    }
}
