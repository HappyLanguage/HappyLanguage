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
        public static String SPACE = "\r\r\n";

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
            TestOutputFromFile("../../../TestFiles/Print/test-file-1.txt", "true" + SPACE + "false");
        }

        [TestMethod()]
        public void TestPrint2()
        {
            TestOutputFromFile("../../../TestFiles/Print/test-file-2.txt",
                "Hello!" + SPACE + "-1374" + SPACE + "125687" + SPACE + "0" + SPACE +
                "-789487" + SPACE + "2" + SPACE + "-5" + SPACE + "112" + SPACE
                );
        }

        [TestMethod()]
        public void TestPrint3()
        {
            TestOutputFromFile("../../../TestFiles/Print/test-file-3.txt", "true");
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

        public void TestOutputFromFile(String path, String output)
		{
			StreamReader pom = new System.IO.StreamReader(path);


			AntlrInputStream inputStream = new AntlrInputStream(pom);
			GrammarLexer lexer = new GrammarLexer(inputStream);
			lexer.RemoveErrorListeners();
			lexer.AddErrorListener(new GrammarErrorListener());
			CommonTokenStream c = new CommonTokenStream(lexer);
			GrammarParser helloParser = new GrammarParser(c);

			helloParser.RemoveErrorListeners();
			helloParser.AddErrorListener(new GrammarErrorListener());

			String path_file_ins = "insc1.txt";

			try
			{
				IParseTree tree = helloParser.start();


				Visitor visitor = new Visitor();
				visitor.PrepareLibraryFunctions();
				visitor.DoInitialJmp();
				int t = visitor.Visit(tree);
				visitor.numberInstructions();

				Program.WriteInstructions(visitor.GetInstructions(), path_file_ins);
			}
			catch (ArgumentException e)
			{
				Console.WriteLine(e.Message);
			}

			Process compiler = new Process();
			compiler.StartInfo.FileName = "../../../refint_pl0_ext.exe";
			compiler.StartInfo.Arguments = path_file_ins + " -s -l";
			compiler.StartInfo.UseShellExecute = false;
			compiler.StartInfo.RedirectStandardOutput = true;

			compiler.Start();

			StreamReader reader = new StreamReader(compiler.StandardOutput.BaseStream);

			if (!compiler.WaitForExit(15000))
			{
				Assert.IsFalse(true, "proces má pravděpodně nekončnou smyčku");
				compiler.Kill();
			}


			String output_from_interpret = reader.ReadToEnd();

			Assert.AreEqual("START PL/0\r\n" + output + " END PL/0\r\n", output_from_interpret);
		}
	}
}
