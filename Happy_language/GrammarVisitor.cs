using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;


//http://stackoverflow.com/questions/23092081/antlr4-visitor-pattern-on-simple-arithmetic-example
namespace Happy_language
{

    public class GrammarVisitor : GrammarBaseVisitor<int> {

        public override int VisitData_type([NotNull] GrammarParser.Data_typeContext context)
        {
            String op = context.GetText();
            Console.WriteLine(op);
            return base.VisitData_type(context);
        }
    }
}
