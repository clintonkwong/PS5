﻿// Skeleton written by Profs Zachary, Kopta and Martin for CS 3500
// Read the entire skeleton carefully and completely before you
// do anything else!

// Change log:
// Last updated: 9/8, updated for non-nullable types 


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace SpreadsheetUtilities
{
    /// <summary> 
    /// Author:    Clinton Kwong 
    /// Partner:   None 
    /// Date:      2022/09/16
    /// Course:    CS 3500, University of Utah, School of Computing 
    /// Copyright: CS 3500 and Clinton Kwong - This work may not be copied for use in Academic Coursework. 
    /// 
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>


    public class Formula
    {
        /// <summary>
        /// list that contains a list of tokens of the expression.
        /// </summary>
        private List<string> formulaList;

        /// <summary>
        /// the string form of the formula
        /// </summary>
        private string formulaString;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            int parenCount = 0;
            bool expectVal = true;
            formulaList = new List<string>();

            foreach (string token in GetTokens(formula))
            {
                double value;
                if (expectVal && token == "(")
                {
                    parenCount++;
                    formulaList.Add(token);
                }
                else if (!expectVal && token == ")")
                {
                    parenCount--;
                    if (parenCount < 0)
                    {
                        throw new FormulaFormatException("unexpected token, ')'");
                    }
                    formulaList.Add(token);
                }
                else if (!expectVal && Regex.IsMatch(token, "^(-|\\+|\\*|/)$"))
                {
                    formulaList.Add(token);
                    expectVal = true;
                }
                else if (expectVal && double.TryParse(token, out value))
                {
                    formulaList.Add(value.ToString());
                    expectVal = false;
                }
                else if (expectVal && Regex.IsMatch(token, "^[a-zA-Z_][a-zA-Z0-9_]*$") && isValid(normalize(token)))
                {
                    formulaList.Add(normalize(token));
                    expectVal = false;
                }
                else
                {
                    throw new FormulaFormatException("unexpected token, '" + token + "'");
                }
            }
            if (formulaList.Count == 0)
            {
                throw new FormulaFormatException("empty formula is invalid");
            }
            if (expectVal)
            {
                throw new FormulaFormatException("invalid last token");
            }
            if (parenCount != 0)
            {
                throw new FormulaFormatException("expected ')', none found");
            }
            formulaString = "";
            foreach (string token in formulaList)
            {
                formulaString += token;
            }
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<string> operatorStack = new Stack<string>();
            Stack<double> valueStack = new Stack<double>();
            // loop through substrings
            try
            {
                foreach (string token in formulaList)
                {
                    if (Regex.IsMatch(token, "[a-zA-Z_0-9]"))
                    {
                        double value;
                        if (!double.TryParse(token, out value))
                        {
                            try
                            {
                                value = lookup(token);
                            }
                            catch (Exception e)
                            {
                                return new FormulaError("Look Up Function Error: " + e.Message);
                            }
                        } // checks if variable, then finds its value
                        valueStack.Push(value);
                        EvaluateOperator(operatorStack, valueStack, false);
                    }
                    // check for '+' or '-'
                    else if (Regex.IsMatch(token, "^(\\+|-)$"))
                    {
                        EvaluateOperator(operatorStack, valueStack, true);
                        operatorStack.Push(token);
                    }
                    // check for '(', '*', or '/'
                    else if (Regex.IsMatch(token, "^(\\(|\\*|/)$"))
                    {
                        operatorStack.Push(token);
                    }
                    // checks for ')'
                    else if (token == ")")
                    {
                        EvaluateOperator(operatorStack, valueStack, true);
                        operatorStack.Pop();
                        EvaluateOperator(operatorStack, valueStack, false);
                    }
                }
            }
            catch (DivideByZeroException)
            {
                return new FormulaError("Divide By Zero Error.");
            }

            // final check to make sure there is one value left
            EvaluateOperator(operatorStack, valueStack, true);

            // return solution
            return valueStack.Pop();
        }

        /// <summary>
        /// Helper method for evaluate method
        /// </summary>
        /// <param name="operatorStack">operator stack</param>
        /// <param name="valueStack">stack of values</param>
        /// <param name="isAdd">whether or not it is looking for + and -</param>
        private static void EvaluateOperator(Stack<string> operatorStack, Stack<double> valueStack, bool isAdd)
        {
            if (operatorStack.Count > 0)
            {
                if ((isAdd && Regex.IsMatch(operatorStack.Peek(), "^(\\+|-)$")) || (!isAdd && Regex.IsMatch(operatorStack.Peek(), "^(\\*|/)$")))
                {
                    switch (operatorStack.Pop())
                    {
                        case "+":
                            valueStack.Push(valueStack.Pop() + valueStack.Pop());
                            break;
                        case "-":
                            double value1 = valueStack.Pop();
                            valueStack.Push(valueStack.Pop() - value1);
                            break;
                        case "*":
                            valueStack.Push(valueStack.Pop() * valueStack.Pop());
                            break;
                        case "/":
                            double value2 = valueStack.Pop();
                            if (value2 == 0) {
                                throw new DivideByZeroException("div by 0");
                            }
                            valueStack.Push(valueStack.Pop() / value2);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return new HashSet<string>(formulaList.Where<string>(token => Regex.IsMatch(token, "[a-zA-Z_]")));
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return formulaString;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (!(obj is Formula))
            {
                return false;
            }
            return ToString() == obj.ToString();
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1 is null)
            {
                return f2 is null;
            }
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason {
            get; private set;
        }

        
    }
}