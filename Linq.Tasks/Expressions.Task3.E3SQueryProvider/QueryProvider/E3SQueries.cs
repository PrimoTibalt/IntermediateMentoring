using System;
using System.Collections.Generic;

namespace Expressions.Task3.E3SQueryProvider.QueryProvider
{
    internal static class E3SQueries
    {
        public static readonly IDictionary<string, Func<string, string>> Methods = new Dictionary<string, Func<string, string>>
        {
            { nameof(StartsWith) , StartsWith },
            { nameof(EndsWith) , EndsWith },
            { nameof(Contains) , Contains },
            { nameof(Equals) , Equals },
        };

        private static string StartsWith(string argument) => $"({argument}*)";

        private static string EndsWith(string argument) => $"(*{argument})";

        private static string Contains(string argument) => $"(*{argument}*)";

        private static string Equals(string argument) => $"({argument})";
    }
}
