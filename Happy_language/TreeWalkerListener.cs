using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Happy_language
{
    class TreeWalkerListener : GrammarBaseListener
    {
        SymbolTable s = new SymbolTable();

        private int i = 0;
        public override void EnterEveryRule([NotNull] ParserRuleContext context)
        {
            base.EnterEveryRule(context);
        }


        public override void EnterBlok([NotNull] GrammarParser.BlokContext context)
        {
            base.EnterBlok(context);
            //Console.WriteLine("\n" + context.Stop.Text);
        }

        public override void ExitBlok([NotNull] GrammarParser.BlokContext context)
        {
            base.ExitBlok(context);
        }

        public override void EnterStart([NotNull] GrammarParser.StartContext context)
        {
            base.EnterStart(context);

            //Console.WriteLine("\n" + context.Start.Text);
        }

        public override void EnterFunction_return_data_typ([NotNull] GrammarParser.Function_return_data_typContext context)
        {
            base.EnterFunction_return_data_typ(context);

            //Console.WriteLine("\n" + "Navrat. hodnota fce: " + context.start.Text +  "\n");

        }




    }
}
