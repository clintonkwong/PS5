using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace FormulaEvaluator
{
    /// <summary>
    /// Author:    Clinton Kwong 
    /// Partner:   None 
    /// Date:      2022/09/02
    /// Course:    CS 3500, University of Utah, School of Computing
    /// 
    /// Evaluator Static class containing a method to evaluate infix expressions.
    /// </summary>
    public static class Evaluator
    {
        public delegate int Lookup(string v);

        private static Stack<int> valueStack; // Stack for value
        private static Stack<string> operatorStack; // Stack for operator

        /// <summary>
        /// Evaluates a given infix expression. Supports string variables, integer literals, and operations +, -, *, and /.
        /// </summary>
        /// <param name="exp">A string expression to be evaluated.</param>
        /// <param name="variableEvaluator">A function that takes a string and returns its corresponding value as an int.</param>
        /// <returns>An integer evaluation of the expression given or error.</returns>
        public static int Evaluate(string exp, Lookup variableEvaluator)
        {
            valueStack = new Stack<int>(); // int Stack for value
            operatorStack = new Stack<string>(); // string Stack for operator
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            try
            {
                // loop through substrings
                foreach (string token in substrings)
                {
                    string s = token.Trim();

                    // skip whitespace
                    if (s == "") {
                        continue;
                    } 

                    // check for a number or variable
                    if (Regex.IsMatch(s, "^[a-zA-Z]*[0-9]*$") && Regex.IsMatch(s, "[0-9]$"))
                    {
                        int value;
                        // checks if variable, then finds its value
                        if (!int.TryParse(s, out value)) {
                            value = variableEvaluator(s);
                        } 
                        valueStack.Push(value);
                        EvaluateHelper(false);
                    }

                    // check for '+' or '-' case
                    else if (Regex.IsMatch(s, "^(\\+|-)$"))
                    {
                        EvaluateHelper(true);
                        operatorStack.Push(s);
                    }

                    // check for '(', '*', or '/' case
                    else if (Regex.IsMatch(s, "^(\\(|\\*|/)$")) {
                        operatorStack.Push(s);
                    }

                    // checks for ')' case
                    else if (s == ")")
                    {
                        EvaluateHelper(true);
                        if (operatorStack.Pop() != "(") {
                            throw new ArithmeticException("invalid expression");
                        }
                        EvaluateHelper(false);
                    }
                    else {
                        throw new Exception("invalid token");
                    }
                }

                // final check to make sure there is one value left
                EvaluateHelper(true);
                if (valueStack.Count != 1) {
                    throw new Exception("more than one item left on stack");
                }

                // return solution
                return (int)valueStack.Pop();
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Helper function for Evaluate function
        /// </summary>
        /// <param name="boolean">Bool that is true if the operations to be done are + and -, false if * and /.</param>
        private static void EvaluateHelper(bool boolean)
        {
            if (operatorStack.Count > 0)
            {
                if (boolean && (Regex.IsMatch(operatorStack.Peek(), "^(\\+|-)$")) || (!boolean && Regex.IsMatch(operatorStack.Peek(), "^(\\*|/)$") ))
                {
                    switch (operatorStack.Pop())
                    {
                        case "-":
                            int minus = valueStack.Pop();
                            valueStack.Push(valueStack.Pop() - minus);
                            break;
                        case "+":
                            valueStack.Push(valueStack.Pop() + valueStack.Pop());
                            break;
                        case "/":
                            int divide = valueStack.Pop();
                            valueStack.Push(valueStack.Pop() / divide);
                            break;
                        case "*":
                            valueStack.Push(valueStack.Pop() * valueStack.Pop());
                            break;
                    }
                }
            }
        }
    }
}