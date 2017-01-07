using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Happy_language
{
    /// <summary>
    /// Main class of this program
    /// </summary>
	public class Program
    {
        /// <summary>
        /// Instance for handling all errors
        /// </summary>
        private static ErrorHandler handler;

        /// <summary>
        /// Stream for reading input file
        /// </summary>
        private static StreamReader streamReader;

        /// <summary>
        /// Lexer
        /// </summary>
        private static GrammarLexer lexer;

        /// <summary>
        /// Parser
        /// </summary>
        private static GrammarParser parser;

        /// <summary>
        /// Parse tree created during syntactic analysis
        /// </summary>
        private static IParseTree parseTree;

        /// <summary>
        /// Visitor of the parse tree
        /// During the tree traversal, instructions are generated
        /// </summary>
        private static Visitor visitor;

        /// <summary>
        /// Name of the input file
        /// </summary>
        private static string inputFile = null;

        /// <summary>
        /// Name of the output file
        /// </summary>
        private static string outputFile = null;

        /// <summary>
        /// Prints a program logo into command line
        /// </summary>
        static void PrintLogo()
        {

            Console.WriteLine("  _    _                           _                                              ");
            Console.WriteLine(" | |  | |                         | |                                             ");
            Console.WriteLine(" | |__| | __ _ _ __  _ __  _   _  | |     __ _ _ __   __ _ _   _  __ _  __ _  ___ ");
            Console.WriteLine(" |  __  |/ _` | '_ \\| '_ \\| | | | | |    / _` | '_ \\ / _` | | | |/ _` |/ _` |/ _ \\");
            Console.WriteLine(" | |  | | (_| | |_) | |_) | |_| | | |___| (_| | | | | (_| | |_| | (_| | (_| |  __/");
            Console.WriteLine(" |_|  |_|\\__,_| .__/| .__/ \\__, | |______\\__,_|_| |_|\\__, |\\__,_|\\__,_|\\__, |\\___|");
            Console.WriteLine("              | |   | |     __/ |                     __/ |             __/ |     ");
            Console.WriteLine("              |_|   |_|    |___/                     |___/             |___/      ");
        }

        /// <summary>
        /// Prints a help into the command line
        /// </summary>
        static void PrintHelp()
        {
            Console.WriteLine("Usage:\n");
            Console.WriteLine("HappyLanguage.exe <input file> [output file]");
            Console.WriteLine("\tinput file: File from which the code will be parsed.");
            Console.WriteLine("\toutput file: File to which the result will be written. If no output file is specified, 'out.pl0' is used.");
            Console.WriteLine("\nor\n\nHappyLanguage.exe -h\n\t to print this help");
        }

        /// <summary>
        /// Main entrance of the program
        /// </summary>
        /// <param name="args">Arguments from the command line</param>
        static void Main(string[] args)
        {
            PrintLogo();

            checkArgs(args);

            Prepare(inputFile);

            ParseFile();

            GenerateInstructions();

            WriteInstructions(visitor.GetInstructions(), outputFile);
        }

        /// <summary>
        /// Validate arguments from command line
        /// </summary>
        /// <param name="args">Arguments from command line</param>
        private static void checkArgs(string[] args)
        {
            if (args.Length == 1)
            {
                if (string.Equals(args[0], "-h"))
                {
                    PrintHelp();
                    Environment.Exit(0);
                }

                inputFile = args[0];
                outputFile = "out.pl0";
            }
            else if (args.Length == 2)
            {
                inputFile = args[0];
                outputFile = args[1];
            }
            else
            {
                PrintHelp();
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Prepare all the things for the process
        /// </summary>
        /// <param name="inputFile">Name of the input file</param>
        private static void Prepare(string inputFile)
        {
            handler = new ErrorHandler();

            PrepareInput(inputFile);

            PrepareLexer();

            PrepareParser();
        }

        /// <summary>
        /// Prepare the input stream
        /// </summary>
        /// <param name="inputFile">Name of the input file</param>
        private static void PrepareInput(string inputFile)
        {
            try
            {
                streamReader = new System.IO.StreamReader(inputFile);
            }
            catch (IOException ioe)
            {
                Console.Error.WriteLine(ioe.Message);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Prepare the lexer
        /// </summary>
        private static void PrepareLexer()
        {
            AntlrInputStream inputStream = new AntlrInputStream(streamReader);

            lexer = new GrammarLexer(inputStream);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new GrammarErrorListener(handler));
        }

        /// <summary>
        /// Prepare the parser
        /// </summary>
        private static void PrepareParser()
        {
            CommonTokenStream tokenStream = new CommonTokenStream(lexer);

            parser = new GrammarParser(tokenStream);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new GrammarErrorListener(handler));
        }

        /// <summary>
        /// Parse the file and create the parse tree
        /// </summary>
        private static void ParseFile()
        {
            Console.WriteLine("\nParsing input file...");
            parseTree = parser.start();
            streamReader.Close();


            if (handler.errorsOccured())
            {
                handler.printErrors();
                Environment.Exit(1);
            }

            Console.WriteLine("Done.\n");
        }

        /// <summary>
        /// Traverse the parse tree and generate instructions
        /// </summary>
        private static void GenerateInstructions()
        {
            Console.WriteLine("Generating instructions...");
            visitor = new Visitor(handler);

            //initial preparation
            visitor.PrepareLibraryFunctions();
            visitor.DoInitialJmp();

            visitor.Visit(parseTree);

            if (handler.errorsOccured())
            {
                handler.printErrors();
                Environment.Exit(1);
            }

            visitor.numberInstructions();

            Console.WriteLine("Done.\n");
            Console.WriteLine("Generated " + visitor.GetInstructions().Count + " instructions.");
        }

        /// <summary>
        /// Write given instruction into file
        /// </summary>
        /// <param name="instructions">Instructions to write</param>
        /// <param name="name_file">Name of the output file</param>
        public static void WriteInstructions(List<Instruction> instructions, String name_file)
        {
            String text = "";
            for (int i = 0; i < instructions.Count; i++)
            {
                text += instructions[i];
            }
            File.WriteAllText(name_file, text);
            Console.WriteLine("\n\nInstructions written into the file '" + name_file + "'.");
        }
    }


}
