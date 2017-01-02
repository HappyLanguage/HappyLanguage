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
        subExpressionMismatch = 18,
        indexTypeMismatch = 19
    }

    public class Error
    {
        public static int varConstDoesNotExist = -1;
        public static int varConstAlreadyExists = -2;
        public static int functionDoesNotExist = -3;
        public static int functionAlreadyExists = -4;
        public static int arrayDoesNotExist = -5;
        public static int arrayAlreadyExists = -6;
        public static int arrayLengthNegative = -7;
        public static int arrayOutOfBounds = -8;
        //public static int functionReturnTypesDoNotMatch = -9;
        public static int functionWrongParametersCount = -10;
        public static int functionParameterDataTypeMismatch = -11;
        public static int arrayIndexNegative = -12;
        public static int cmpTypeMismatch = -13;
        public static int assignmentMismatch = -14;
        public static int assignmentToConstant = -15;
        public static int operatorTypeMismatch = -16;
        public static int subExpressionMismatch = -18;
        public static int indexTypeMismatch = -19;

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
