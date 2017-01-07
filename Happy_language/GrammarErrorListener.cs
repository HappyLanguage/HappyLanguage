using Antlr4.Runtime;


namespace Happy_language
{
    /// <summary>
    /// Class listening for grammar errors
    /// </summary>
    public class GrammarErrorListener : BaseErrorListener, IAntlrErrorListener<int>
    {
        /// <summary>
        /// Handler to which the errors are reported
        /// </summary>
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
