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

            StreamReader pom = new System.IO.StreamReader("../../../sourceCode.txt");

            AntlrInputStream inputStream = new AntlrInputStream(pom);
            GrammarLexer lexer = new GrammarLexer(inputStream);
            ErrorHandler handler = new ErrorHandler();
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new GrammarErrorListener(handler));
            CommonTokenStream c = new CommonTokenStream(lexer);   
            GrammarParser helloParser = new GrammarParser(c);
            helloParser.RemoveErrorListeners();
            helloParser.AddErrorListener(new GrammarErrorListener(handler));
     

            //Console.WriteLine("START");

            try
            {
                IParseTree tree = helloParser.start();
                pom.Close();

                if (handler.errorsOccured())
                {
                    handler.printErrors();
                    Console.ReadLine();
                    return;
                }

                Visitor visitor = new Visitor(handler);
                visitor.PrepareLibraryFunctions();
                visitor.DoInitialJmp();
                int t = visitor.Visit(tree);

                if (handler.errorsOccured())
                {
                    handler.printErrors();
                    Console.ReadLine();
                    return;
                }

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
