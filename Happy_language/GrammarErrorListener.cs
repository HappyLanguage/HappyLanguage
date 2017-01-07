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
        private ErrorHandler handler;

        public GrammarErrorListener(ErrorHandler handler)
        {
            this.handler = handler;
        }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            handler.reportError(line, charPositionInLine, "Unexpected symbol '" + offendingSymbol.Text + "'");
        }

        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            handler.reportError(line, charPositionInLine, "Unexpected symbol");
        }


    }
}
