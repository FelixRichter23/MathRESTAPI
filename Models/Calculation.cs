using MathAPI.Repositories;
using System.Data;
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

        /// <summary>
        /// Validates and processes the expression, ensuring it conforms to expected syntax and symbols.
        /// Throws an exception if the expression is invalid.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if the expression is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown if the expression contains invalid characters, symbols, or consecutive operators.</exception>

        private void ValidateExpression()
        {
            if (string.IsNullOrEmpty(expression)) throw new ArgumentNullException();

            expression = expression.Replace(" ", string.Empty);
            expression = expression.Trim();
            expression = expression.Replace(".", ",");

            Regex validFormulaRegex = new Regex(@"^(?:log\(\d+;\d+\)|\d+(?:\,\d+)?(?:\^\d+(?:\,\d+)?)?|[+\-*\/()]|\{\d+\})+$");

            if (Regex.IsMatch(expression, @"[+\-*\/]{2,}"))
            {
                throw new ArgumentException("The expression contains invalid consecutive operators.");
            }

            if (!validFormulaRegex.IsMatch(expression))
            {
                throw new ArgumentException("The expression contains invalid symbols or characters.");
            }

            CheckParenthensis();
        }


        /// <summary>
        /// Ensures that the parentheses and curly brackets in the expression are correctly balanced and formatted.
        /// Throws an exception if the brackets are mismatched or incorrectly placed.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the brackets in the expression are not correctly balanced or formatted.</exception>

        private void CheckParenthensis()
        {
            int count = 0;
            int curlyCount = 0;

            foreach (char parenthensis in expression.Where(c => c == '(' ||c == ')'))
            {
                if (parenthensis == '(') count++;
                if (parenthensis == ')') count--;
                if (count < 0)
                {
                    throw new ArgumentException("The brackets in the expression are not formatted correctly.");
                }
            }

            foreach (char parenthensis in expression.Where(c => c == '{' || c == '}'))
            {
                if (parenthensis == '{') count++;
                if (parenthensis == '}') count--;
                if (count < 0 || count > 1)
                {
                    throw new ArgumentException("The curly brackets in the expression are not formatted correctly.");
                }
            }

            if (count != 0 || curlyCount != 0) 
                throw new ArgumentException("The brackets in the expression are not formatted correctly.");
        }

        /// <summary>
        /// Identifies and loads related calculations based on IDs found within curly braces in the expression.
        /// Replaces the IDs with the result of the referenced calculations.
        /// </summary>
        /// <param name="expression">The expression containing IDs within curly braces.</param>
        /// <returns>The expression with IDs replaced by the results of the referenced calculations.</returns>
        /// <exception cref="ArgumentException">Thrown if the expression references itself.</exception>

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
        /// Performs the calculation based on the expression and returns the calculation instance with the result.
        /// </summary>
        /// <returns>The current instance of Calculation with the computed result.</returns>

        public Calculation Calculate()
        {
            result = calculate(LoadRelations(expression));
            this.isDeprecated = false;
            return this;
        }

        /// <summary>
        /// Recursively calculates the value of the given expression.
        /// Supports basic arithmetic operations, exponentiation, and logarithms.
        /// </summary>
        /// <param name="expression">The expression to be calculated.</param>
        /// <returns>The calculated result as a double.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the expression is null or empty.</exception>
        /// <exception cref="DivideByZeroException">Thrown if a division by zero is attempted.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the expression is invalid or contains unsupported operations.</exception>

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
                    double divisor = calculate(expression.Substring(divisorIndex + 1, expression.Length - divisorIndex - 1));

                    if (divisor == 0)
                    {
                        throw new DivideByZeroException();
                    }

                    return
                       calculate(expression.Substring(0, divisorIndex))
                       / divisor;
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

        /// <summary>
        /// Retrieves a calculation by its ID from the repository. 
        /// If the calculation is marked as deprecated, it is recalculated and updated in the repository.
        /// </summary>
        /// <param name="id">The ID of the calculation to retrieve.</param>
        /// <param name="calculationRepo">The repository to retrieve the calculation from.</param>
        /// <returns>The retrieved or recalculated Calculation object.</returns>
        /// <exception cref="ArgumentException">Thrown if a calculation with the specified ID does not exist.</exception>

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

        /// <summary>
        /// Marks all dependent calculations as deprecated, recursively updating all calculations that depend on this one.
        /// </summary>

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

        /// <summary>
        /// Saves the current calculation to the repository. 
        /// If the calculation is new, it is added to the repository and assigned an ID.
        /// If the calculation already exists, it is updated in the repository.
        /// Additionally, the relations (dependencies) of the calculation are updated.
        /// </summary>

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
