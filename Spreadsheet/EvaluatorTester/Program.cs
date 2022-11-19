using System;
using FormulaEvaluator;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

namespace FormulaEvaluatorTest
{
    /// <summary>
    /// Class for testing FormulaEvaluator.
    /// </summary>
    class Program
    {
        /// <summary>
        /// testing FormulaEvaluator.
        /// </summary>
        /// <param name="args">.</param>
        static void Main(string[] args)
        {


            //case 1
            try
            {
                int result = Evaluator.Evaluate("4 * 3", s => 0);
                if (result == 12) { 
                Console.WriteLine("case 1 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 1 error!\n" + e.Message);
            }

            //case 2
            try
            {
                int result = Evaluator.Evaluate("5 - (8 * 3) + 12 / 3", s => 0);
                if (result == -15)
                {
                    Console.WriteLine("case 2 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 2 error!\n" + e.Message);
            }

            //case 3
            try
            {
                int result = Evaluator.Evaluate("(4*2) / 3", s => 0);
                if (result == 2)
                {
                    Console.WriteLine("case 3 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 3 error!\n" + e.Message);
            }

            //case 4
            try
            {
                int result = Evaluator.Evaluate("5 + 2", s => 0);
                if (result == 7)
                {
                    Console.WriteLine("case 4 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 4 error!\n" + e.Message);
            }

            //case 5
            try
            {
                int result = Evaluator.Evaluate("5 + 2 - 3 * 3", s => 0);
                if (result == -2)
                {
                    Console.WriteLine("case 5 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 5 error!\n" + e.Message);
            }

            //case 6
            try
            {
                int result = Evaluator.Evaluate("(9) / (2) + 4", s => 0);
                if (result == 8)
                {
                    Console.WriteLine("case 6 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 6 error!\n" + e.Message);
            }

            //case 7
            try
            {
                int result = Evaluator.Evaluate("0", s => 0);
                if (result == 0)
                {
                    Console.WriteLine("case 7 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 7 error!\n" + e.Message);
            }

            //case 8
            try
            {
                int result = Evaluator.Evaluate("4 * 0", s => 0);
                if (result == 0)
                {
                    Console.WriteLine("case 8 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 8 error!\n" + e.Message);
            }

            //case 9
            try
            {
                int result = Evaluator.Evaluate("0 + 1/3", s => 0);
                if (result == 0)
                {
                    Console.WriteLine("case 9 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 9 error!\n" + e.Message);
            }

            //case 10
            try
            {
                int result = Evaluator.Evaluate("(4*3)/(3*2)-2", s => 0);
                if (result == 0)
                {
                    Console.WriteLine("case 10 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 10 error!\n" + e.Message);
            }

            //case 11
            try
            {
                int result = Evaluator.Evaluate("(((3)))", s => 0);
                if (result == 3)
                {
                    Console.WriteLine("case 11 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 11 error!\n" + e.Message);
            }

            //case 12
            try
            {   
                int result = Evaluator.Evaluate("(3 + 4 + 9 - 5) * (6 - 1 + 8 - 4)", s => 0);
                if (result == 99)
                {
                    Console.WriteLine("case 12 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 12 error!\n" + e.Message);
            }

            //case 13
            try
            {
                int result = Evaluator.Evaluate("A1", s => 25);
                if (result == 25)
                {
                    Console.WriteLine("case 13 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 13 error!\n" + e.Message);
            }

            //case 14
            try
            {
                int result = Evaluator.Evaluate("AB33", s => 25);
                if (result == 25)
                {
                    Console.WriteLine("case 14 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 14 error!\n" + e.Message);
            }

            //case 15
            try
            {
                int result = Evaluator.Evaluate("((()))", s => 0);
                if (result == 0)
                {
                    Console.WriteLine("case 15 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 15 error!\n" + e.Message);
            }

            //case 16
            try
            {
                int result = Evaluator.Evaluate("abaa", s => 0);
                if (result == 0)
                {
                    Console.WriteLine("case 16 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 16 error!\n" + e.Message);
            }

            //case 17
            try
            {
                int result = Evaluator.Evaluate("a7a", s => 0);
                if (result == 0)
                {
                    Console.WriteLine("case 17 success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("case 17 error!\n" + e.Message);
            }


        }
    }
}