using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Happy_language;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Diagnostics;

namespace Happy_language.Tests
{
	[TestClass()]
	public class TestProgram
	{
        public static String NEW_LINE = "\r\r\n";

        [TestMethod()]
        public void TestAssign1()
        {
            TestOutputFromFile("../../../TestFiles/Assign/test-file-1.txt", "-1374");
        }

        [TestMethod()]
        public void TestAssign2()
        {
            TestOutputFromFile("../../../TestFiles/Assign/test-file-2.txt", "45");
        }

        [TestMethod()]
        public void TestAssign3()
        {
            TestOutputFromFile("../../../TestFiles/Assign/test-file-3.txt", "100");
        }

        [TestMethod()]
        public void TestAssign4()
        {
            TestOutputFromFile("../../../TestFiles/Assign/test-file-4.txt", "-15");
        }

        [TestMethod()]
        public void TestAssign5()
        {
            TestOutputFromFile("../../../TestFiles/Assign/test-file-5.txt", "99");
        }

        [TestMethod()]
        public void TestAssignMulti1()
        {
            TestOutputFromFile("../../../TestFiles/AssignMulti/test-file-1.txt", "-876-876");
        }

        [TestMethod()]
        public void TestAssignMulti2()
        {
            TestOutputFromFile("../../../TestFiles/AssignMulti/test-file-2.txt", "40");
        }

        [TestMethod()]
        public void TestPrint1()
        {
            TestOutputFromFile("../../../TestFiles/Print/test-file-1.txt", "true" + NEW_LINE + "false");
        }

        [TestMethod()]
        public void TestPrint2()
        {
            TestOutputFromFile("../../../TestFiles/Print/test-file-2.txt",
                "Hello!" + NEW_LINE + "-1374" + NEW_LINE + "125687" + NEW_LINE + "0" + NEW_LINE +
                "-789487" + NEW_LINE + "2" + NEW_LINE + "-5" + NEW_LINE + "112" + NEW_LINE
                );
        }

        [TestMethod()]
        public void TestPrint3()
        {
            TestOutputFromFile("../../../TestFiles/Print/test-file-3.txt", "true");
        }

        [TestMethod()]
        public void TestPrint4()
        {
            TestOutputFromFile("../../../TestFiles/Print/test-file-4.txt", "false" + NEW_LINE + "true");
        }

        [TestMethod()]
        public void TestPrint5()
        {
            TestOutputFromFile("../../../TestFiles/Print/test-file-5.txt",
                "true" + NEW_LINE + "4" + NEW_LINE + "true" + NEW_LINE + "3" + NEW_LINE + "true" + NEW_LINE +
                "2" + NEW_LINE + "true" + NEW_LINE + "1" + NEW_LINE + "false");
        }

        [TestMethod()]
        public void TestReturn1()
        {
            TestOutputFromFile("../../../TestFiles/Return/test-file-1.txt", "40");
        }

        [TestMethod()]
        public void TestReturn2()
        {
            TestOutputFromFile("../../../TestFiles/Return/test-file-2.txt", "125");
        }

        [TestMethod()]
        public void TestReturn3()
        {
            TestOutputFromFile("../../../TestFiles/Return/test-file-3.txt",
                "100" + NEW_LINE + "false" + NEW_LINE + "true");
        }

        [TestMethod()]
        public void TestBoolToInt1()
        {
            TestOutputFromFile("../../../TestFiles/BoolToInt/test-file-1.txt", "10");
        }

        [TestMethod()]
        public void TestBoolToInt2()
        {
            TestOutputFromFile("../../../TestFiles/BoolToInt/test-file-2.txt", "1010642");
        }

        [TestMethod()]
        public void TestBoolToInt3()
        {
            TestOutputFromFile("../../../TestFiles/BoolToInt/test-file-3.txt", "10");
        }

        [TestMethod()]
        public void TestIntToBool1()
        {
            TestOutputFromFile("../../../TestFiles/IntToBool/test-file-1.txt",
                "true" + NEW_LINE + "false" + NEW_LINE + "true" + NEW_LINE + "true" +
                NEW_LINE + "true" + NEW_LINE + "false" + NEW_LINE + "true" + NEW_LINE + "false");
        }

        [TestMethod()]
        public void TestIntToBool2()
        {
            TestOutputFromFile("../../../TestFiles/IntToBool/test-file-2.txt", "-4-3-2-11234");
        }

        [TestMethod()]
        public void TestMin1()
        {
            TestOutputFromFile("../../../TestFiles/Min/test-file-1.txt",
                "56" + NEW_LINE + "-123" + NEW_LINE + "-92" + NEW_LINE + "55");
        }

        [TestMethod()]
        public void TestMax1()
        {
            TestOutputFromFile("../../../TestFiles/Max/test-file-1.txt",
                "72" + NEW_LINE + "10" + NEW_LINE + "55" + NEW_LINE + "55");
        }

        [TestMethod()]
        public void TestAbs1()
        {
            TestOutputFromFile("../../../TestFiles/Abs/test-file-1.txt",
                "159" + NEW_LINE + "0" + NEW_LINE + "951" + NEW_LINE + "56" + NEW_LINE +
                "123" + NEW_LINE + "92" + NEW_LINE + "55");
        }

        [TestMethod()]
        public void TestAbs2()
        {
            TestOutputFromFile("../../../TestFiles/Abs/test-file-2.txt",
                "20" + NEW_LINE + "30");
        }

        [TestMethod()]
		public void TestUnaryOperator1()
		{
			TestOutputFromFile("../../../TestFiles/UnaryOperator/test-file-1.txt", "-5");
		}

		[TestMethod()]
		public void TestUnaryOperator2()
		{
			TestOutputFromFile("../../../TestFiles/UnaryOperator/test-file-2.txt", "-6");
		}

		[TestMethod()]
		public void TestUnaryOperator3()
		{
			TestOutputFromFile("../../../TestFiles/UnaryOperator/test-file-3.txt", "11");
		}


		[TestMethod()]
		public void TestUnaryOperator4()
		{
			TestOutputFromFile("../../../TestFiles/UnaryOperator/test-file-4.txt", "1");
		}


		[TestMethod()]
		public void TestUnaryOperator5()
		{
			TestOutputFromFile("../../../TestFiles/UnaryOperator/test-file-5.txt", "14");
		}


		[TestMethod()]
		public void TestUnaryOperator6()
		{
			TestOutputFromFile("../../../TestFiles/UnaryOperator/test-file-6.txt", "-4-3-2-101234");
		}

		[TestMethod()]
		public void TestUnaryOperator7()
		{
			TestOutputFromFile("../../../TestFiles/UnaryOperator/test-file-7.txt", "-1");
		}

		[TestMethod()]
		public void TestTernaryOperator1()
		{
			TestOutputFromFile("../../../TestFiles/TernaryOperator/test-file-1.txt", "-5");
		}

		[TestMethod()]
		public void TestTernaryOperator2()
		{
			TestOutputFromFile("../../../TestFiles/TernaryOperator/test-file-2.txt", "-5");
		}

		[TestMethod()]
		public void TestTernaryOperator3()
		{
			TestOutputFromFile("../../../TestFiles/TernaryOperator/test-file-3.txt", "12");
		}

		[TestMethod()]
		public void TestTernaryOperator4()
		{
			TestOutputFromFile("../../../TestFiles/TernaryOperator/test-file-4.txt", "-99");
		}

		[TestMethod()]
		public void TestTernaryOperator5()
		{
			TestOutputFromFile("../../../TestFiles/TernaryOperator/test-file-5.txt", "55");
		}

		[TestMethod()]
        public void TestTernaryOperator6()
        {
            TestOutputFromFile("../../../TestFiles/TernaryOperator/test-file-6.txt", "-8811-10");
        }

        [TestMethod()]
        public void TestArithmetic1()
        {
            TestOutputFromFile("../../../TestFiles/Arithmetic/test-file-1.txt",
                "10" + NEW_LINE + "-30" + NEW_LINE + "-1" + NEW_LINE + "-360");
        }

        [TestMethod()]
        public void TestRecursion1()
        {
            TestOutputFromFile("../../../TestFiles/Recursion/test-file-1.txt",
                "012345" + NEW_LINE + "0123");
        }

        [TestMethod()]
        public void TestRecursion2()
        {
            TestOutputFromFile("../../../TestFiles/Recursion/test-file-2.txt",
                "Hello world!");
        }

        [TestMethod()]
        public void TestCycles1()
        {
            TestOutputFromFile("../../../TestFiles/Cycles/test-file-1.txt",
                "5724724244");
        }

        [TestMethod()]
        public void TestCycles2()
        {
            TestOutputFromFile("../../../TestFiles/Cycles/test-file-2.txt",
                "Hello world!");
        }

        [TestMethod()]
        public void TestCycles3()
        {
            TestOutputFromFile("../../../TestFiles/Cycles/test-file-3.txt",
                "35");
        }

        [TestMethod()]
        public void TestBubbleSort1()
        {
            TestOutputFromFile("../../../TestFiles/BubbleSort/test-file-1.txt",
                "_____aaahiinoorrrsssttty");
        }

        [TestMethod()]
        public void TestConditions1()
        {
            TestOutputFromFile("../../../TestFiles/Conditions/test-file-1.txt",
                "321134");
        }

        public void TestGrammarError(String path, ErrorCode expectedError)
        {
            StreamReader pom = new System.IO.StreamReader(path);

            ErrorHandler handler = new ErrorHandler();


            AntlrInputStream inputStream = new AntlrInputStream(pom);
            GrammarLexer lexer = new GrammarLexer(inputStream);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new GrammarErrorListener(handler));
            CommonTokenStream c = new CommonTokenStream(lexer);
            GrammarParser helloParser = new GrammarParser(c);

            helloParser.RemoveErrorListeners();
            helloParser.AddErrorListener(new GrammarErrorListener(handler));

            String path_file_ins = "insc1.txt";

            try
            {
                IParseTree tree = helloParser.start();
                pom.Close();

                if (handler.errorsOccured())
                {
                    List<Error> grammarErrors = handler.GrammarErrors;

                    Assert.AreEqual(1, grammarErrors.Count, "More errors occured than expected");
                    Assert.AreEqual(ErrorCode.grammarError, grammarErrors[0].ErrorCode, "Invalid error type.");
                }
                else
                {
                    Assert.Fail("No grammar error occured");
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

		public void TestOutputFromFile(String path, String output)
		{
			StreamReader pom = new System.IO.StreamReader(path);

            ErrorHandler handler = new ErrorHandler();


			AntlrInputStream inputStream = new AntlrInputStream(pom);
			GrammarLexer lexer = new GrammarLexer(inputStream);
			lexer.RemoveErrorListeners();
			lexer.AddErrorListener(new GrammarErrorListener(handler));
			CommonTokenStream c = new CommonTokenStream(lexer);
			GrammarParser helloParser = new GrammarParser(c);

			helloParser.RemoveErrorListeners();
			helloParser.AddErrorListener(new GrammarErrorListener(handler));

			String path_file_ins = "insc1.txt";

			try
			{
				IParseTree tree = helloParser.start();
                pom.Close();

                if (handler.errorsOccured())
                {
                    Assert.Fail("Errors detected during lexical or syntactic analysis.");
                }

                Visitor visitor = new Visitor(handler);
				visitor.PrepareLibraryFunctions();
				visitor.DoInitialJmp();
				int t = visitor.Visit(tree);

                if (handler.errorsOccured())
                {
                    Assert.Fail("Errors detected during semantic analysis.");
                }

                visitor.numberInstructions();

				Program.WriteInstructions(visitor.GetInstructions(), path_file_ins);
			}
			catch (ArgumentException e)
			{
				Console.WriteLine(e.Message);
			}

			Process interpreter = new Process();
			interpreter.StartInfo.FileName = "../../../refint_pl0_ext.exe";
			interpreter.StartInfo.Arguments = path_file_ins + " -s -l";
			interpreter.StartInfo.UseShellExecute = false;
			interpreter.StartInfo.RedirectStandardOutput = true;

			interpreter.Start();

			StreamReader reader = new StreamReader(interpreter.StandardOutput.BaseStream);

			if (!interpreter.WaitForExit(15000))
			{
				Assert.IsFalse(true, "proces má pravděpodně nekončnou smyčku");
				interpreter.Kill();
			}


			String output_from_interpret = reader.ReadToEnd();

			Assert.AreEqual("START PL/0\r\n" + output + " END PL/0\r\n", output_from_interpret);
		}
	}
}
