using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Diagnostics;

/*
 * ANTLR
 * http://stackoverflow.com/questions/19327831/antlr4-c-sharp-application-tutorial-example
 * https://github.com/tunnelvisionlabs/antlr4cs
 * https://groups.google.com/forum/#!topic/antlr-discussion/Gh_P6IiDrKU
 * 
 * TOP Antlr
 * http://elemarjr.com/en/2016/04/21/learning-antlr4-part-1-quick-overview/
 * http://www.theendian.com/blog/antlr-4-lexer-parser-and-listener-with-example-grammar/
 */

namespace Happy_language
{
	public class Program
    {
        static void Main(string[] args)
        {

            StreamReader pom = new System.IO.StreamReader("../../../sourceCode5.txt");

            AntlrInputStream inputStream = new AntlrInputStream(pom);
            GrammarLexer lexer = new GrammarLexer(inputStream);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new GrammarErrorListener());
            CommonTokenStream c = new CommonTokenStream(lexer);   
            GrammarParser helloParser = new GrammarParser(c);
            //IParseTree tree = helloParser.start();
            // ParseTreeWalker walker = new ParseTreeWalker();
            //walker.Walk(new TreeWalkerListener(), tree);
            helloParser.RemoveErrorListeners();
            helloParser.AddErrorListener(new GrammarErrorListener());
     

            Console.WriteLine("START");

            try
            {
                IParseTree tree = helloParser.start();
                Console.WriteLine("----------------Lexical analyzation OK----------------------");

                Visitor visitor = new Visitor();
                visitor.PrepareLibraryFunctions();
                visitor.DoInitialJmp();
                int t = visitor.Visit(tree);
                visitor.numberInstructions();

                Console.WriteLine(visitor.GetSymbolTable().VarConstToString());
                Console.WriteLine("-----------------------------------------");
                PrintInstructions(visitor.GetInstructions());
                WriteInstructions(visitor.GetInstructions(), "../../../insc.txt");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }

			Console.ReadLine();
            // skvelej napad, jednopruchod znamena dolu i nahoru, takze dolu udelam jen neco a smerem nahoru zbytek
        }

        public static void WriteInstructions(List<Instruction> instructions, String name_file)
        {

            String text = "";
            for(int i = 0; i < instructions.Count; i++)
            {
                text += instructions[i];
            }
            File.WriteAllText(name_file, text);
        }

		public static void PrintInstructions(List<Instruction> instructions)
        {
            for (int i = 0; i < instructions.Count; i++)
            {
                Console.Write(instructions[i]);
            }
        }

    }


}
