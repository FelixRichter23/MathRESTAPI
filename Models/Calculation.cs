using MathAPI.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MathAPI.Models
{
    public class Calculation
    {
        public int? id { get; private set; }
        public string expression { get; set; }
        public double result { get; private set; }
        public bool isDeprecated { get; set; } = true;

        private Dictionary<int, Lazy<Calculation>> _relations;

        public ICalculationRepository _calculationRepository;
        public IRelationRepository _relationRepository;
        private List<int> _dependencies { get; set; } = new List<int>();

        public Calculation()
        {

        }

        public Calculation(string expression, ICalculationRepository calcRepo, IRelationRepository relationRepo, int? id = null)
        {
            this.id = id;
            _relations = new Dictionary<int, Lazy<Calculation>>();
            _calculationRepository = calcRepo;
            _relationRepository = relationRepo;  
            this.expression = expression;
            ValidateExpression();
        }

        private void ValidateExpression()
        {
            if (string.IsNullOrEmpty(expression)) throw new ArgumentNullException();

            expression = expression.Replace(" ", string.Empty);
            expression = expression.Trim();
            expression = expression.Replace(".", ",");

            Regex validFormulaRegex = new Regex(@"^(?:log\(\d+;\d+\)|\d+(?:\,\d+)?(?:\^\d+(?:\,\d+)?)?|[+\-*\/()]|\{\d+\})+$");

            if (!validFormulaRegex.IsMatch(expression))
            {
                throw new ArgumentException("The expression contains invalid symbols or characters.");
            }

            CheckBracklets();
        }


        /// <summary>
        /// Ensures that brackets in the expression are correctly balanced and formatted.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>True if the brackets are in a valid format.</returns>
        /// <exception cref="ArgumentException"></exception>

        private void CheckBracklets()
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

            foreach (char bracket in expression.Where(c => c == '{' || c == '}'))
            {
                if (bracket == '{') count++;
                if (bracket == '}') count--;
                if (count < 0 || count > 1)
                {
                    throw new ArgumentException("The curly brackets in the expression are not formatted correctly.");
                }
            }

            if (count != 0 || curlyCount != 0) 
                throw new ArgumentException("The brackets in the expression are not formatted correctly.");
        }

        /// <summary>
        /// Identifies and loads related calculations referenced by IDs within curly braces in the expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <exception cref="ArgumentException"></exception>

        private string LoadRelations(string expression) 
        {
            var matches = Regex.Matches(expression, @"{(\d+)}");
            foreach (Match match in matches)
            {
                int refId = int.Parse(match.Groups[1].Value);

                if (refId == this.id)
                {
                    throw new ArgumentException("An expression cannot refer to itself.");
                }

                _dependencies.Add(refId);

                var refCalculation = GetCalculation(refId, _calculationRepository);

                expression = expression.Replace(match.Value, refCalculation.result.ToString());
            }
            return expression;
        }

        /// <summary>
        /// Performs the calculation and returns it.
        /// </summary>
        /// <returns>Result as double</returns>

        public Calculation Calculate()
        {
            result = calculate(LoadRelations(expression));
            this.isDeprecated = false;
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

        public static Calculation GetCalculation(int id, ICalculationRepository calculationRepo)
        {
            Calculation calculation;

            try
            {
                calculation = calculationRepo.GetCalculation(id);
                calculation._calculationRepository = calculationRepo;
            }
            catch ( Exception e )
            {
                throw new ArgumentException($"A calculation with id {id} does not exist.");
            }

            if (calculation.isDeprecated)
            {
                calculation.Calculate();
                calculation._calculationRepository.UpdateCalculation(calculation);
            }

            return calculation;
        }

        public void MarkDependentsAsDeprecated()
        {
            if (this.id == null) return;

            // Suchen Sie nach allen Berechnungen, die von der geänderten Berechnung abhängen
            var dependents = _relationRepository.GetDependents((int)this.id);

            foreach (var dependent in dependents)
            {
                Calculation calculation = _calculationRepository.GetCalculation(dependent);
                calculation.isDeprecated = true;
                calculation._calculationRepository = _calculationRepository;
                calculation._relationRepository = _relationRepository;

                calculation.Save();

                if(calculation.id != null)
                {
                    calculation.MarkDependentsAsDeprecated();
                }
            }
        }

        public void Save()
        {
            if (this.id == null)
            {
                this.id = _calculationRepository.SaveCalculation(this);
            }
            else
            {
                _calculationRepository.UpdateCalculation(this);

                _relationRepository.DeleteRelation((int)this.id);
            }

            foreach (var dependency in _dependencies)
            {
                _relationRepository.SaveRelation((int)this.id, dependency);
            }
        }
    }
}
