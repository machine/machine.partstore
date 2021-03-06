using System;
using System.Collections.Generic;
using System.Text;

using Machine.Core;

namespace Machine.Partstore.Utility
{
  public static class StringHelper
  {
    public static string FindLongestCommonPrefix(ICollection<string> strings)
    {
      List<string> all = new List<string>(strings);
      if (all.Count == 0)
      {
        throw new YouFoundABugException("Can't find longest common prefix of empty set!");
      }
      if (all.Count == 1)
      {
        return all[0];
      }
      all.Sort(delegate(string x, string y) { return x.Length.CompareTo(y.Length); });
      for (int i = 0; i < all[0].Length; ++i)
      {
        char c = all[0][i];
        for  (int j = 1; j < all.Count; ++j)
        {
          if (c != all[j][i])
          {
            return all[0].Substring(0, i);
          }
        }
      }
      return String.Empty;
    }

    public static IEnumerable<string> RemoveNulls(this IEnumerable<string> values)
    {
      foreach (string value in values)
      {
        if (value != null)
        {
          yield return value;
        }
      }
    }

    public static string Join(this IEnumerable<string> values, string separator)
    {
      StringBuilder sb = new StringBuilder();
      foreach (string flag in values)
      {
        if (sb.Length != 0)
        {
          sb.Append(separator);
        }
        sb.Append(flag);
      }
      return sb.ToString();
    }

    public static IEnumerable<string> QuoteEach(this IEnumerable<string> values)
    {
      return QuoteEach(values, @"""");
    }

    public static IEnumerable<string> QuoteEach(this IEnumerable<string> values, string token)
    {
      foreach (string value in values)
      {
        yield return token + value + token;
      }
    }
  }
}