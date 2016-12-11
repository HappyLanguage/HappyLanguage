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

		[TestMethod()]
		public void TestAplication()
		{
			TestOutputFromFile("../../../sourceCode3.txt", "A");
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
