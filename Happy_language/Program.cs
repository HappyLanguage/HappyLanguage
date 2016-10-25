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

            Console.WriteLine("START");
            Console.WriteLine("Zpracovavam výraz: ");

            AntlrInputStream inputStream = new AntlrInputStream(pom);
            GrammarLexer lexer = new GrammarLexer(inputStream);
            CommonTokenStream c = new CommonTokenStream(lexer);
            GrammarParser helloParser = new GrammarParser(c);

            /*IParseTree tree = helloParser.;*/
            IParseTree tree = helloParser.start();

            //ParseTreeWalker walker = new ParseTreeWalker();
            //walker.Walk(new TreeWalkerListener(), tree);

           // Console.WriteLine(tree.ToStringTree(helloParser));
            GrammarVisitor visitor = new GrammarVisitor();
           // Console.WriteLine(visitor.Visit(tree));

            //Console.WriteLine(tree.ToStringTree() + " ");

            //Console.WriteLine(tree.GetText() + " ");
            
            Console.ReadLine();

        }

        /*
          public void Run()
        {
            try
            {
                Console.WriteLine("START");
                RunParser();
                Console.Write("DONE. Hit RETURN to exit: ");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
                Console.Write("Hit RETURN to exit: ");
            }
            Console.ReadLine();
        }

        private void RunParser()
        {
            AntlrInputStream inputStream = new AntlrInputStream("hello best world 54\n");
            HelloLexer helloLexer = new HelloLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(helloLexer);
            HelloParser helloParser = new HelloParser(commonTokenStream);
            HelloParser.RContext rContext = helloParser.r();
            MyVisitor visitor = new MyVisitor();
            visitor.VisitR(rContext);
        }*/
    }
}
