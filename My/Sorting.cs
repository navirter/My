using System;
using System.Collections.Generic;
using System.Linq;


namespace My
{    
    public static class Sorting
    {
        #region merge_sort
        public static List<T> merge_sort<T>(List<T> source) where T : IComparable
        {
            try
            {
                int count = source.Count();
                if (count <= 1)
                    return source;

                int half = count / 2;
                var left = source.Take(half).ToList();
                var right = source.Skip(half).ToList();

                left = merge_sort(left);
                right = merge_sort(right);

                return merge_parts(left, right);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        static List<T> merge_parts<T>(List<T> left, List<T> right) where T : IComparable
        {
            try
            {
                int total_count = left.Count() + right.Count();

                var result = new T[total_count];

                int left_indexer = 0;
                int right_indexer = 0;
                int result_indexer = 0;

                while (left_indexer < left.Count && right_indexer < right.Count)
                {
                    if (left[left_indexer].CompareTo(right[right_indexer]) <= 0)
                    {
                        result[result_indexer] = left[left_indexer];
                        result_indexer++; left_indexer++;
                    }
                    else
                    {
                        result[result_indexer] = right[right_indexer];
                        result_indexer++; right_indexer++;
                    }
                }
                while (left_indexer < left.Count)
                {
                    result[result_indexer] = left[left_indexer];
                    result_indexer++; left_indexer++;
                }
                while (right_indexer < right.Count)
                {
                    result[result_indexer] = right[right_indexer];
                    result_indexer++; right_indexer++;
                }
                return result.ToList();
            }
            catch (Exception e)
            { throw new Exception(e.Message); }
        }
        #endregion
    }
}
