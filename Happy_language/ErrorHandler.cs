using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happy_language
{
    public enum ErrorCode
    {
        varConstDoesNotExist = 1,
        varConstAlreadyExists = 2,
        functionDoesNotExist = 3,
        functionAlreadyExists = 4,
        arrayDoesNotExist = 5,
        arrayAlreadyExists = 6,
        arrayLengthNegative = 7,
        arrayOutOfBounds = 8,
        functionWrongParametersCount = 10,
        functionParameterDataTypeMismatch = 11,
        arrayIndexNegative = 12,
        cmpTypeMismatch = 13,
        assignmentMismatch = 14,
        assignmentToConstant = 15,
        operatorTypeMismatch = 16,
        grammarError = 17,
        subExpressionMismatch = 18
}

    public class ErrorHandler
    {
        private class Error
        {
            private ErrorCode errorCode;
            private int lineNumber;
            private int charPositionInLine;
            private string message;

            public Error(ErrorCode errorCode, int lineNumber, int charPositionInLine, string message)
            {
                this.errorCode = errorCode;
                this.lineNumber = lineNumber;
                this.charPositionInLine = charPositionInLine;
                this.message = message;
            }

            public override string ToString()
            {
                return "Line " + lineNumber + ((charPositionInLine > 0) ? (":" + charPositionInLine) : "") + " Error(" + ((int) errorCode).ToString("000") + ") - " + message; 
            }
        }

        List<Error> grammarErrors = new List<Error>();
        List<Error> visitorErrors = new List<Error>();

        public void reportGrammarError(int lineNumber, int charPositionInLine, string msg)
        {
            grammarErrors.Add(new Error(ErrorCode.grammarError, lineNumber, charPositionInLine, msg));
        }

        public void reportVisitorError(int lineNumber, ErrorCode errorCode, string msg)
        {
            visitorErrors.Add(new Error(errorCode, lineNumber, -1, msg));
        }

        public void reportVisitorError(int lineNumber, int charPositionInLine, ErrorCode errorCode, string msg)
        {
            visitorErrors.Add(new Error(errorCode, lineNumber, charPositionInLine, msg));
        }

        public void printErrors()
        {
            if (errorsOccured())
            {
                Console.WriteLine("============================================================================================");
                Console.WriteLine("  Errors were found in the code:");
                Console.WriteLine("--------------------------------------------------------------------------------------------");

                if (grammarErrors.Count != 0)
                {
                    foreach (Error e in grammarErrors)
                    {
                        Console.WriteLine("  " + e);
                    }
                }

                if (visitorErrors.Count != 0)
                {
                    foreach (Error e in visitorErrors)
                    {
                        Console.WriteLine("  " + e);
                    }
                }

                Console.WriteLine("============================================================================================");
            }
        }

        public bool errorsOccured()
        {
            return grammarErrors.Count != 0 || visitorErrors.Count != 0;
        }
    }
}
