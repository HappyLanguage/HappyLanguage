using System;
using System.Collections.Generic;

namespace Happy_language
{
    /// <summary>
    /// Enum representing all possible compilation errors
    /// </summary>
    public enum ErrorCode
    {
        /********************** Grammar errors **********************/
        /// <summary>
        /// This code represents all errors that occurred during lexical or syntactic analysis
        /// </summary>
        grammarError = 10,
        /************************************************************/

        /*********** Errors with variables and constants ************/
        /// <summary>
        /// Code representing errors when referencing symbols that are not in local or global table
        /// </summary>
        unknownSymbol = 20,

        /// <summary>
        /// Code representing errors when declaring symbol with name of already defined symbol
        /// </summary>
        symbolAlreadyExists = 21,
        /************************************************************/

        /********************* Function errors **********************/
        /// <summary>
        /// Code representing errors when calling not declared function
        /// </summary>
        unknownFunction = 30,

        /// <summary>
        /// Code representing errors when declaring function with name of already defined function
        /// </summary>
        functionAlreadyExists = 31,

        /// <summary>
        /// Code representing errors when calling function with wrong number of parameters
        /// </summary>
        functionWrongParameterCount = 32,

        /// <summary>
        /// Code representing errors when there is conflict in type of some parameters during function call
        /// </summary>
        functionParameterDataTypeMismatch = 33,

        /// <summary>
        /// Code representing errors when data type of the return value is other than the return data type of function
        /// </summary>
        returnTypeMismatch = 34,
        /************************************************************/

        /*********************** Array errors ***********************/
        /// <summary>
        /// Code representing errors when referencing not declared array
        /// </summary>
        unknownArray = 40,

        /// <summary>
        /// Code representing errors when declaring array with name of already defined symbol
        /// </summary>
        arrayAlreadyExists = 41,

        /// <summary>
        /// Code representing errors when declaring array with negative length
        /// </summary>
        arrayLengthNegative = 42,

        /// <summary>
        /// Code representing errors when indexing with literal bigger than the bounds of the array
        /// This error does not occur when using arithmetic expression for indexing
        /// </summary>
        indexOutOfBounds = 43,

        /// <summary>
        /// Code representing errors when indexing with negative literal
        /// This error does not occur when using arithmetic expression for indexing
        /// </summary>
        negativeIndex = 44,

        /// <summary>
        /// This code represents errors when indexing symbol that is not an array
        /// </summary>
        notAnArray = 45,

        /// <summary>
        /// Code representing errors when referencing array without indexing
        /// </summary>
        notIndexedArray = 46,
        /************************************************************/

        /******************* Type conflict errors *******************/
        /// <summary>
        /// Code representing errors when comparing two values of different data type
        /// </summary>
        cmpTypeMismatch = 50,

        /// <summary>
        /// Code representing data type conflicts in assignment
        /// </summary>
        assignmentMismatch = 51,

        /// <summary>
        /// Code representing errors when assigning to constant
        /// </summary>
        assignmentToConstant = 52,

        /// <summary>
        /// Code representing data type conflict in arithmetic expressions
        /// </summary>
        operatorTypeMismatch = 53,

        /// <summary>
        /// Code representing data type conflict of subexpressions of ternary operator
        /// </summary>
        subExpressionMismatch = 54,

        /// <summary>
        /// Code representing errors when indexing with value of other data type than :I
        /// </summary>
        indexTypeMismatch = 55,

        /// <summary>
        /// Code representing errors when using other data type then :B in condition
        /// </summary>
        conditionTypeMismatch = 56,
        /************************************************************/
    }

    /// <summary>
    /// Class representing errors that occurred during process 
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Error code representing the type of the error
        /// </summary>
        private ErrorCode errorCode;

        /// <summary>
        /// Number of the line on which the error occurred
        /// </summary>
        private int lineNumber;

        /// <summary>
        /// Position on line where the error occurred
        /// -1 if the position is not known
        /// </summary>
        private int charPositionInLine;

        /// <summary>
        /// Additional message to the error
        /// </summary>
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

        /// <summary>
        /// Constructor of the error
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <param name="lineNumber">Number of line where the error occured</param>
        /// <param name="charPositionInLine">Position on line where the error occurred</param>
        /// <param name="message">Additional message</param>
        public Error(ErrorCode errorCode, int lineNumber, int charPositionInLine, string message)
        {
            this.errorCode = errorCode;
            this.lineNumber = lineNumber;
            this.charPositionInLine = charPositionInLine;
            this.message = message;
        }

        /// <summary>
        /// Get string representation of this error
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            //position in line is only printed when known
            return "Line " + lineNumber + ((charPositionInLine > 0) ? (":" + charPositionInLine) : "") + " Error(" + ((int)errorCode).ToString("000") + ") - " + message;
        }
    }

    /// <summary>
    /// Class representing utility for collecting all errors that occured during process
    /// </summary>
    public class ErrorHandler
    {
        /// <summary>
        /// List of errors that occurred during lexical and syntactic analysis
        /// </summary>
        List<Error> grammarErrors = new List<Error>();

        /// <summary>
        /// List of errors that occured during semantic analysis
        /// </summary>
        List<Error> semanticErrors = new List<Error>();

        public List<Error> GrammarErrors
        {
            get { return grammarErrors; }
            set { grammarErrors = value; }
        }

        public List<Error> SemanticErrors
        {
            get { return semanticErrors; }
            set { semanticErrors = value; }
        }

        /// <summary>
        /// Method used for reporting errors that occurred during lexical or syntactic analysis
        /// </summary>
        /// <param name="lineNumber">Line on which the error occurred</param>
        /// <param name="charPositionInLine">Position in line where the error occurred</param>
        /// <param name="msg">Additional message</param>
        public void reportError(int lineNumber, int charPositionInLine, string msg)
        {
            grammarErrors.Add(new Error(ErrorCode.grammarError, lineNumber, charPositionInLine, msg));
        }

        /// <summary>
        /// Method used for reporting errors that occured during semantic analysis and their position in line is unknown
        /// </summary>
        /// <param name="lineNumber">Line on which the error occurred</param>
        /// <param name="errorCode">Type of the error</param>
        /// <param name="msg">Additional message</param>
        /// <returns>Negative value representing the error code</returns>
        public int reportError(int lineNumber, ErrorCode errorCode, string msg)
        {
            semanticErrors.Add(new Error(errorCode, lineNumber, -1, msg));

            return -(int)errorCode;
        }

        /// <summary>
        /// Method used for reporting errors that occured during semantic analysis with known position in line
        /// </summary>
        /// <param name="lineNumber">Line on which the error occurred</param>
        /// <param name="charPositionInLine">Position in line where the error occurred</param>
        /// <param name="errorCode">Type of the error</param>
        /// <param name="msg">Additional message</param>
        /// <returns>Negative value representing the error code</returns>
        public int reportError(int lineNumber, int charPositionInLine, ErrorCode errorCode, string msg)
        {
            semanticErrors.Add(new Error(errorCode, lineNumber, charPositionInLine, msg));

            return -(int)errorCode;
        }

        /// <summary>
        /// Prints all collected errors into the console
        /// </summary>
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

                if (semanticErrors.Count != 0)
                {
                    foreach (Error e in semanticErrors)
                    {
                        Console.WriteLine("  " + e);
                    }
                }

                Console.WriteLine("============================================================================================");
            }
        }

        /// <summary>
        /// Method representing whether the compilation process was successfull
        /// </summary>
        /// <returns>True when there were some errors, False if everything is OK</returns>
        public bool errorsOccured()
        {
            return grammarErrors.Count != 0 || semanticErrors.Count != 0;
        }
    }
}
