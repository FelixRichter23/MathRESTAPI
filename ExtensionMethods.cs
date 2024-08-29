using System.Runtime.InteropServices.Marshalling;

namespace MathAPI
{
    public static class ExtensionMethods
    {
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

        private static IEnumerable<int> AllIndexesOf(this string str, string search)
        {
            int minIndex = str.IndexOf(search);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(search, minIndex + search.Length);
            }
        }

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
