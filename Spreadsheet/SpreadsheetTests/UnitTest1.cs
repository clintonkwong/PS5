using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using System.Collections.Generic;
using SpreadsheetUtilities;

namespace SpreadsheetTests
{

    /// <summary> 
    /// Author:    Clinton Kwong 
    /// Partner:   None 
    /// Date:      2022/09/30
    /// Course:    CS 3500, University of Utah, School of Computing 
    /// Copyright: CS 3500 and Clinton Kwong - This work may not be copied for use in Academic Coursework. 
    /// 
    /// I, Clinton Kwong, certify that I wrote this code from scratch and did not copy it in part or whole from  
    /// another source.  All references used in the completion of the assignment are cited in my README file. 
    /// 
    /// File Contents 
    /// Test class for Spreadsheet class. Each test name represent the test summary
    ///    
    /// </summary>
    /// 
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestConstructor()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            Assert.AreEqual(testSheet.GetCellContents("_5-2_"), "");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContents1()
        {
            Spreadsheet testSheet = new Spreadsheet();
            Assert.AreEqual(testSheet.GetCellContents("___________"), "");
        }



       

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContents2()
        {
            Spreadsheet testsheet = new Spreadsheet();
            testsheet.GetCellContents("25525L");
        }





        [TestMethod]
        public void TestSetCellContentsdouble1()
        {
            Spreadsheet testsheet = new Spreadsheet();
            testsheet.SetContentsOfCell("A1", "252");
            Assert.AreEqual(testsheet.GetCellContents("A1"), 252.00);
        }

        [TestMethod]
        public void TestSetCellContentsdouble2()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "255225");
            sheet.SetContentsOfCell("a1", "0.0");
            Assert.AreEqual(sheet.GetCellContents("a1"), 0.00);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsdouble3()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("1!!!?a", "25225");
        }

        [TestMethod]
        public void TestSetCellContentsstring1()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "kld");
            Assert.AreEqual(sheet.GetCellContents("a1"), "kld");
        }

        [TestMethod]
        public void TestSetCellContentsstring2()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("x1", "kld");
            sheet.SetContentsOfCell("x1", "kgk");
            Assert.AreEqual(sheet.GetCellContents("x1"), "kgk");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsstring3()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("2sssss", "abcdefg");
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsstring4()
        {
            Spreadsheet sheet = new Spreadsheet();
            string? s = null;
            sheet.SetContentsOfCell("abc", s);
        }

       

        [TestMethod]
        public void TestSetCellContentsstring5()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("b1", "");
            Assert.AreEqual(sheet.GetCellContents("b1"), "");
        }

        [TestMethod]
        public void TestSetCellContentsstring6()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "  ");
            Assert.AreEqual(sheet.GetCellContents("a1"), "  ");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsformula1()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("5sss", "=1+2-3");
        }


        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellContentsformula2()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("c1", "=c1*c2");
        }



        [TestMethod]
        public void TestGetNamesOfAllNonemptyCells()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=1");
            sheet.SetContentsOfCell("b2", "=2");
            sheet.SetContentsOfCell("c3", "=3");
            Assert.IsTrue(new HashSet<string>(sheet.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "a1", "b2", "c3" }));
        }

        [TestMethod]
        public void TestSave1()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "5");
            testSheet.SetContentsOfCell("A2", "abc");
            testSheet.SetContentsOfCell("A3", "=B4");
            testSheet.Save("saveTest.txt");
        }



        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestNewConstrutor1()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet("q:\\missing\\save.txt", s => true, s => s, "");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestRead1()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet("", s => true, s => s, "");
        }



        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSave2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("");
            ss = new Spreadsheet("save5.txt", s => true, s => s, "version");
        }


        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void TestGetNamesOfAllNonemptyCells2()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "5");
            testSheet.SetContentsOfCell("A2", "abc");
            testSheet.SetContentsOfCell("A3", "=B4");
            List<string> testList = new List<string> { "A1", "A2", "A3" };
            Assert.IsTrue(new List<string>(testSheet.GetNamesOfAllNonemptyCells()).Equals(testList));
        }

        [TestMethod]
        public void TestGetCellValue1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "8");
            s.SetContentsOfCell("A1", "9");
            s.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(8.0, (double)s.GetCellValue("B1"), 1e-9);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValue2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "8");
            s.SetContentsOfCell("A1", "9");
            s.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(8.0, (double)s.GetCellValue(""), 1e-9);
        }

        [TestMethod]
        public void TestGetCellValue3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5"); ;
            Assert.AreEqual("", s.GetCellValue("b1"));
        }

        [TestMethod()]
        public void TestSave3()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
            s1.SetContentsOfCell("A1", "hi");
            s1.Save("save1.txt");
            Assert.AreEqual("hi", s1.GetCellContents("A1"));
        }


        [TestMethod()]
        public void TestNormalize()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("a1", "5");
            ss.SetContentsOfCell("A1", "6");
            ss.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]

        public void TestHello()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("hello", "25");
            s.SetContentsOfCell("B1", "=23");
            Assert.AreEqual(new Formula("25"), (Formula)s.GetCellContents("A1"));
        }
    }
}