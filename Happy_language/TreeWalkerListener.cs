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

        public override void EnterEveryRule([NotNull] ParserRuleContext context)
        {
            Console.WriteLine(context.ToStringTree() + " ");
            base.EnterEveryRule(context);
        }
    }

}
