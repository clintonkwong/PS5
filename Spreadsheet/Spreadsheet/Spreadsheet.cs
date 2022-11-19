using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpreadsheetUtilities;
using Formatting = Newtonsoft.Json.Formatting;

namespace SS
{

    /// <summary> 
    /// Author:    Clinton Kwong 
    /// Partner:   None 
    /// Date:      2022/09/30
    /// Course:    CS 3500, University of Utah, School of Computing 
    /// Copyright: CS 3500 and Clinton Kwong - This work may not be copied for use in Academic Coursework. 
    /// 
    /// <summary>
    /// Class that implements the AbstractSpreadsheet interface.
    /// Represents a Spreadsheet using a DependencyGraph and Cell objects.
    /// </summary>

    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// graph of the dependencies of the cells
        /// </summary>
        private DependencyGraph dependencyGraph;
        /// <summary>
        /// a dictionary that maps a string to a Cell object
        /// </summary>
        private Dictionary<string, Cell> dictionaryCell;

        /// <summary>
        /// constructor initializes the dependency graph and the cell dictionary
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            // create two empty object to store the data
            dependencyGraph = new DependencyGraph();
            dictionaryCell = new Dictionary<string, Cell>();
            Changed = false;
        }


        /// <summary>
        /// constructor that takes three arguments
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalizer"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            // create two empty object to store the data
            dependencyGraph = new DependencyGraph();
            dictionaryCell = new Dictionary<string, Cell>();
            Changed = false;
        }


        /// <summary>
        /// constructor that takes four arguments
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isValid"></param>
        /// <param name="normalizer"></param>
        /// <param name="version"></param>
        public Spreadsheet(string path, Func<string, bool> isValid, Func<string, string> normalizer, string version) : base(isValid, normalizer, version)
        {
            // use the helper method to read the file and then throw the exception
            Spreadsheet file = Read(path);
            if (file.Version.Equals(version) == false)
            {
                throw new SpreadsheetReadWriteException("Version is wrong");
            }
            dependencyGraph = new DependencyGraph();
            dictionaryCell = file.dictionaryCell;
            // didn't change version
            Changed = false;
            // reset the cell key
            foreach (string strKey in dictionaryCell.Keys)
            {
                // find the original cell
                Cell storeCell = dictionaryCell[strKey];
                // new content
                SetContentsOfCell(strKey, storeCell.Form);
            }
        }


        /// <summary>
        /// returns the cell contents of the cell with the given name
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <returns>the value of the cell</returns>
        public override object GetCellContents(string name)
        {
            // use the helper method to read the file and then throw the exception
            if (!checkVariable(name))
            {
                throw new InvalidNameException();
            }
            //Normalize the name
            name = Normalize(name);
            // return content if exist
            if (dictionaryCell.ContainsKey(name))
            {
                return dictionaryCell[name].Content;
            }
            else
            {
                //return empty string if doesn't exist
                return "";
            }
        }

        /// <summary>
        /// returns the cell value of the cell with the given name
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <returns>the value of the cell</returns>
        public override object GetCellValue(string name)
        {

            // use the helper method to read the file and then throw the exception
            if (!checkVariable(name))
            {
                throw new InvalidNameException();
            }
            //Normalize the name
            name = Normalize(name);
            // return value if exist
            if (dictionaryCell.ContainsKey(name))
            {
                return dictionaryCell[name].Value;
            }
            else
            {
                //return empty string if doesn't exist
                return "";
            }
        }

        /// <summary>
        /// returns a IEnumerable of the names of all cells that have content.
        /// </summary>
        /// <returns>IEnumerable of strings.</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells() => dictionaryCell.Keys;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <exception cref="SpreadsheetReadWriteException"></exception>
        public override void Save(string filename)
        {
            //throw exception if the file name is empty string, 
            if (filename.Equals(""))
            {
                throw new SpreadsheetReadWriteException("File name can't be empty");
            }
            try
            {
                // use the JsonConvert to change the type and then used SerializeObject to serilaize the object 
                string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
                // write all the text
                File.WriteAllText(filename, jsonString);
            }
            // throw exceptions
            catch (IOException e)
            {
                throw new SpreadsheetReadWriteException(e.ToString());
            }
            catch (JsonException)
            {
                throw new SpreadsheetReadWriteException("Cannot serialize it");
            }
            // didn't change version
            Changed = false;
        }

        /// <summary>
        /// sets the cell to a content cell with the given value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            //Normalize the name
            name = Normalize(name);
            // use the helper method to read the file and then throw the exception
            if (!checkVariable(name))
            {
                throw new InvalidNameException();
            }
            IList<string> result = new List<string>();
            // content with =
            if (content.StartsWith("="))
            {
                content = content.Substring(1, content.Length - 1);
                Formula formulaResult = new Formula(content, Normalize, IsValid);
                result = new List<string>(SetCellContents(name, formulaResult));
            }
            else if (Double.TryParse(content, out double number))
            {
                result = new List<string>(SetCellContents(name, number));
            }
            else
            {
                result = new List<string>(SetCellContents(name, content));
            }
            // check dependees and then recaculate
            foreach (string element in result)
            {
                if (dictionaryCell.TryGetValue(element, out Cell? cell))
                {
                    cell.ReCalculate(Lookup);
                }
            }
            Changed = true;
            return result;
        }

        /// <summary>
        /// returns the dependents of a given cell
        /// </summary>
        /// <param name="name">the cell</param>
        /// <returns>the dependents</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return dependencyGraph.GetDependents(name);
        }


        /// <summary>
        /// sets the cell to a number cell with the given value
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <param name="number">the number to be in the cell</param>
        /// <returns>a list of dependees of the new cell joined with the new cell</returns>
        protected override IList<string> SetCellContents(string name, double number)
        {
            //create a new cell object
            Cell value = new Cell(number);
            //change it to number if the cell has already have content there; otherwise, add it to the dictionary
            if (dictionaryCell.ContainsKey(name))
            {
                dictionaryCell[name] = value;
            }
            else
            {
                dictionaryCell.Add(name, value);
            }
            //create dependees to empty list
            dependencyGraph.ReplaceDependees(name, new List<string>());
            //return the list of dependees
            List<string> result = new List<string>(GetCellsToRecalculate(name));
            return result;
        }

        /// <summary>
        /// sets the cell to a text cell with the given value
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <param name="text">the text to be in the cell</param>
        /// <returns>a list of dependees of the new cell joined with the new cell</returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            // create a new cell object
            Cell cell = new Cell(text);
            //change it to number if the cell has already have content there; otherwise, add it to the dictionary
            if (dictionaryCell.ContainsKey(name))
            {
                dictionaryCell[name] = cell;
            }
            else
            {
                dictionaryCell.Add(name, cell);
            }
            //remove the cell if the content is the empty content, 
            if (dictionaryCell[name].Content.Equals(""))
            {
                dictionaryCell.Remove(name);
            }
            //empty list
            dependencyGraph.ReplaceDependees(name, new List<string>());
            //return the list of dependees
            List<string> result = new List<string>(GetCellsToRecalculate(name));
            return result;
        }

        /// <summary>
        /// sets the cell to a formula cell with the given value
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <param name="formula">the formula to be in the cell</param>
        /// <returns>a list of dependees of the new cell joined with the new cell</returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            // get the previous version of the dependees
            IEnumerable<string> previous = dependencyGraph.GetDependees(name);
            // replace with the variables in formula
            dependencyGraph.ReplaceDependees(name, formula.GetVariables());

            // check for circular reference
            try
            {
                List<string> result = new List<string>(GetCellsToRecalculate(name));
                Cell value = new Cell(formula, Lookup);
                if (dictionaryCell.ContainsKey(name))
                {
                    dictionaryCell[name] = value;
                }
                else
                {
                    dictionaryCell.Add(name, value);
                }
                return result;
            }
            catch (CircularException)
            {
                dependencyGraph.ReplaceDependees(name, previous);
                throw new CircularException();
            }
        }


        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }



        /// <summary>
        /// help method to look up the cell value
        /// </summary>
        /// <param name="cellname"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private double Lookup(String cellname)
        {
            if (this.dictionaryCell.TryGetValue(cellname, out Cell? cell))
            {
                if (cell.Value is double)
                {
                    return (double)cell.Value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }


        private Spreadsheet Read(String file)
        {
            //throw exception if the file name is  empty string, 
            if (file.Equals(""))
            {
                throw new SpreadsheetReadWriteException("File name is empty");
            }

            // change type and then check for exception
            try
            {
                Spreadsheet? newSpreadsheets = new Spreadsheet();
                // used the StreamReader object to read the file
                using (StreamReader fileReader = new StreamReader(file))
                {
                    string jsonString = fileReader.ReadToEnd();
                    newSpreadsheets = JsonConvert.DeserializeObject<Spreadsheet>(jsonString);

                    //throw the exception if the object is null
                    if (newSpreadsheets != null)
                    {
                        return newSpreadsheets;
                    }
                    else
                        throw new SpreadsheetReadWriteException("can't change type");
                }
            }
            catch (JsonException)
            {
                throw new SpreadsheetReadWriteException("can't deserialize object");
            }
            catch (IOException e)
            {
                throw new SpreadsheetReadWriteException(e.ToString());
            }
        }

        /// <summary>
        /// help method to match method to check if the string Variable.
        /// </summary>
        /// <param name="cellName"></param> 
        /// <returns></returns>
        private bool checkVariable(string cellName)
        {
            if (Regex.IsMatch(cellName, @"^[a-zA-Z]+\d+$", RegexOptions.Singleline) && IsValid(cellName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
    /// <summary>
    /// Class for a cell.
    /// </summary>
    public class Cell
    {
        [JsonProperty]
        public string Form;
        private string contentType;
        public object Value { get; private set; }
        public object Content { get; private set; }


        /// <summary>
        /// default constructor to deserialize
        /// </summary>
        public Cell()
        {
            Form = "";
            contentType = "Object";
            Value = new Object();
            Content = new Object();
        }
        /// <summary>
        /// use this constructor if cell's content is double
        /// </summary>
        /// <param name="name"></param> initialize the content
        public Cell(double name)
        {
            Form = name.ToString();
            contentType = "double";
            Value = name;
            Content = name;
        }



        /// <summary>
        /// use this constructor if cell's content is string
        /// </summary>
        /// <param name="name"></param> initialize the content
        public Cell(string name)
        {
            Form = name;
            contentType = "string";
            Value = name;
            Content = name;
        }

        /// <summary>
        /// use this constructor if cell's content is formula
        /// </summary>
        /// <param name="name"></param>  initialize the content
        public Cell(Formula name, Func<string, double> lookup)
        {
            Form = "=" + name.ToString();
            contentType = "Formula";
            Value = name.Evaluate(lookup);
            Content = name;
        }

        /// <summary>
        /// use lookup method to recaculate the cell value
        /// </summary>
        /// <param name="lookup"></param>
        public void ReCalculate(Func<string, double> lookup)
        {
            if (contentType.Equals("Formula"))
            {
                Formula same = (Formula)Content;
                Value = same.Evaluate(lookup);
            }
        }
    }
}

