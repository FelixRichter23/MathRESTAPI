using System.Runtime.InteropServices.Marshalling;

namespace MathAPI
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Determines whether the specified search string exists outside of any parentheses in the given input string.
        /// </summary>
        /// <param name="input">The string to search within.</param>
        /// <param name="search">The substring to search for.</param>
        /// <param name="index">Outputs the index of the found substring outside of parentheses, or -1 if not found.</param>
        /// <returns>Returns true if the search string is found outside of any parentheses, otherwise false.</returns>

        public static bool ContainsNotInParenthensis(this string value, string search, out int index)
        {
            var allParenthensis = value.GetParenthensisIndices(('(', ')'));

            if (!allParenthensis.Any())
            {
                index = value.IndexOf(search);
                return index != -1;
            }

            var searchedIndex = value.AllIndexesOf(search).ToList();

            foreach (var item in value.AllIndexesOf(search).ToList())
            {
                foreach (var (open, close) in allParenthensis)
                {
                    if (open < item && item < close)
                    {
                        searchedIndex.Remove(item);
                        break;
                    }
                }
            }

            if (searchedIndex.Any())
            {
                index = searchedIndex[0];
                return true;
            }

            index = -1;
            return false;
        }

        /// <summary>
        /// Gets all indexes of an specific search string.
        /// </summary>
        /// <param name="str">The string to search within.</param>
        /// <param name="search">The substring to search for.</param>
        /// <returns>IEnumerable with indices.</returns>

        private static IEnumerable<int> AllIndexesOf(this string str, string search)
        {
            int minIndex = str.IndexOf(search);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(search, minIndex + search.Length);
            }
        }

        /// <summary>
        /// Returns a list of tuples representing the indices of matching opening and closing parentheses in the input string.
        /// </summary>
        /// <param name="input">The string to search for parentheses pairs.</param>
        /// <param name="parentheses">A tuple specifying the opening and closing parentheses characters to look for.</param>
        /// <returns>A list of tuples, where each tuple contains the indices of a matching pair of opening and closing parentheses.</returns>
        /// <exception cref="InvalidOperationException">Thrown when an unmatched opening or closing parenthesis is found.</exception>

        public static List<(int open, int close)> GetParenthensisIndices(this string input, (char open, char close) par)
        {
            Stack<int> stack = new Stack<int>();
            List<(int open, int close)> parenthensisPairs = new List<(int open, int close)>();

            for (int i = 0; i < input.Length; i++)
            {
                char currentChar = input[i];

                if (currentChar == par.open)
                {
                    stack.Push(i);
                }
                else if (currentChar == par.close)
                {
                    if (stack.Count > 0)
                    {
                        int openingIndex = stack.Pop();
                        parenthensisPairs.Add((openingIndex, i));
                    }
                    else
                    {
                        throw new InvalidOperationException("Unmatched closing bracket found.");
                    }
                }
            }

            if (stack.Count > 0)
            {
                throw new InvalidOperationException("Unmatched opening bracket(s) found.");
            }

            return parenthensisPairs;
        }
    }
}
