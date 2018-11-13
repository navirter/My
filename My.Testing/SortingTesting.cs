using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My;

namespace My.Testing
{
    [TestClass()]
    public class SortingTesting
    {
        [TestMethod()]
        public void different_ints()
        {
            List<int> ints = new List<int>() { -12, -15, -30, -100, 0, -50, 60, -2, 3, 4, 4, 6, 3, -3 };
            List<int> actual = Sorting.merge_sort(ints);
            List<int> expected = new List<int>() { -100, -50, -30, -15, -12, -3, -2, 0, 3, 3, 4, 4, 6, 60 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void few_ints()
        {
            List<int> ints = new List<int>() { 3, 2, 1, 0, -1 };
            List<int> actual = Sorting.merge_sort(ints);
            List<int> expected = new List<int>() { -1, 0, 1, 2, 3 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void different_strings()
        {
            string[] strings = { "a", "g", "b", "z", "y" };
            string[] actual = Sorting.merge_sort(new List<string>(strings)).ToArray();
            string[] expected = { "a", "b", "g", "y", "z" };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void one_digit()
        {
            var input = new List<int>() { 1 };
            var output = Sorting.merge_sort(input);
            var expected = new List<int>() { 1 };
            CollectionAssert.AreEqual(expected, output);
        }

        [TestMethod()]
        public void nothing()
        {
            int[] input = new int[] { };
            var output = Sorting.merge_sort(new List<int>(input));
            var expected = new List<int>();
            CollectionAssert.AreEqual(expected, output);
        }
    }
}




