using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

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
    class Program
    {
        static void Main(string[] args)
        {

            StreamReader pom = new System.IO.StreamReader("../../../sourceCode.txt");

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
                visitor.DoInitialJmp(1);
                int t = visitor.Visit(tree);
                visitor.numberInstructions();

                Console.WriteLine(visitor.GetSymbolTable().VarConstToString());
                Console.WriteLine("-----------------------------------------");
                PrintInstructions(visitor.GetInstructions());
                WriteInstructions(visitor.GetInstructions());
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }

          

            Console.ReadLine();
            // skvelej napad, jednopruchod znamena dolu i nahoru, takze dolu udelam jen neco a smerem nahoru zbytek
        }

        private static void WriteInstructions(List<String> instructions)
        {

            String text = "";
            for(int i = 0; i < instructions.Count; i++)
            {
                text += instructions[i];
            }
            File.WriteAllText("../../../insc.txt", text);
        }

        private static void PrintInstructions(List<String> instructions)
        {
            for (int i = 0; i < instructions.Count; i++)
            {
                Console.Write(instructions[i]);
            }
        }

    }


}
