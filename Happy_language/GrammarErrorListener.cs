using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Happy_language
{

    public class GrammarErrorListener:BaseErrorListener, IAntlrErrorListener<int>
    {
        //BaseErrorListener implementation

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            //throw new ArgumentException("Syntax error on line: " + line + " (" + msg + ")", msg, e);
            throw new ArgumentException("Syntax error on line: " + line + ":"+ charPositionInLine + " '" + offendingSymbol.Text + "'");
        }

        //IAntlrErrorListener<int> implementation;

        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            //throw new ArgumentException("Syntax error on line: " + line + " (" + msg + ")", msg, e);
            throw new ArgumentException("Syntax error on line: " + line + ":" + charPositionInLine + " '" + offendingSymbol + "'");
        }


    }
}
