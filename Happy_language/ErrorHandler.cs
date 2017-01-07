using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happy_language
{
    public enum ErrorCode
    {
        grammarError = 10,

        unknownSymbol = 20,
        varConstAlreadyExists = 21,


        functionDoesNotExist = 30,
        functionAlreadyExists = 31,
        functionWrongParameterCount = 32,
        functionParameterDataTypeMismatch = 33,
        returnTypeMismatch = 34,


        arrayDoesNotExist = 40,
        arrayAlreadyExists = 41,
        arrayLengthNegative = 42,
        indexOutOfBounds = 43,
        negativeIndex = 44,
        notAnArray = 45,
        notIndexedArray = 46,
        
        
        cmpTypeMismatch = 50,
        assignmentMismatch = 51,
        assignmentToConstant = 52,
        operatorTypeMismatch = 53,
        subExpressionMismatch = 54,
        indexTypeMismatch = 55,
        conditionTypeMismatch = 56,
        operatorConflictingTypes = 57    
    }

    public class Error
    {
        private ErrorCode errorCode;
        private int lineNumber;
        private int charPositionInLine;
        private string message;

        public ErrorCode ErrorCode
        {
            get { return ErrorCode; }
        }

        public int LineNumber
        {
            get { return lineNumber; }
        }

        public int CharPositionInLine
        {
            get { return charPositionInLine; }
        }

        public string Message
        {
            get { return message; }
        }

        public Error(ErrorCode errorCode, int lineNumber, int charPositionInLine, string message)
        {
            this.errorCode = errorCode;
            this.lineNumber = lineNumber;
            this.charPositionInLine = charPositionInLine;
            this.message = message;
        }

        public override string ToString()
        {
            return "Line " + lineNumber + ((charPositionInLine > 0) ? (":" + charPositionInLine) : "") + " Error(" + ((int)errorCode).ToString("000") + ") - " + message;
        }
    }

    public class ErrorHandler
    {
        List<Error> grammarErrors = new List<Error>();
        List<Error> visitorErrors = new List<Error>();

        public List<Error> GrammarErrors
        {
            get { return grammarErrors; }
            set { grammarErrors = value; }
        }

        public List<Error> VisitorErrors
        {
            get { return visitorErrors; }
            set { visitorErrors = value; }
        }

        public void reportError(int lineNumber, int charPositionInLine, string msg)
        {
            grammarErrors.Add(new Error(ErrorCode.grammarError, lineNumber, charPositionInLine, msg));
        }

        public int reportError(int lineNumber, ErrorCode errorCode, string msg)
        {
            visitorErrors.Add(new Error(errorCode, lineNumber, -1, msg));

            return -(int)errorCode;
        }

        public int reportError(int lineNumber, int charPositionInLine, ErrorCode errorCode, string msg)
        {
            visitorErrors.Add(new Error(errorCode, lineNumber, charPositionInLine, msg));

            return -(int)errorCode;
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
