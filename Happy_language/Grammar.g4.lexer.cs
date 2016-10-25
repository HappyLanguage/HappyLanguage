using System;

namespace Happy_language
{
    partial class GrammarLexer
    {
        public override string GetErrorDisplay(string s)
        {
            Console.WriteLine("GetErrorDisplay() = " + s);
            return base.GetErrorDisplay(s);
        }

        public override string GetCharErrorDisplay(int c)
        {
            Console.WriteLine("GetCharErrorDisplay() = " + c);
            return base.GetCharErrorDisplay(c);
        }


    }
}
