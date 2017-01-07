using System.Collections.Generic;

namespace Happy_language
{
    /// <summary>
    /// Symbol table holding information about declared symbols
    /// </summary>
    public class SymbolTable
    {
        /// <summary>
        /// Declared variables and constants
        /// </summary>
        Dictionary<string, Symbol> symbolTable = new Dictionary<string, Symbol>();

        /// <summary>
        /// Declared functions
        /// </summary>
        Dictionary<string, Function> functionTable = new Dictionary<string, Function>();

        #region Symbols
        /// <summary>
        /// Add symbol to the table
        /// </summary>
        /// <param name="symbol">Symbol to add</param>
        public void AddSymbol(Symbol symbol)
        {
            symbolTable[symbol.GetName()] = symbol;
        }

        /// <summary>
        /// Get symbol with given name from table
        /// </summary>
        /// <param name="name">Name fo the given symbol</param>
        /// <returns>The symbol, or null if the symbol is not in the table</returns>
        public Symbol GetSymbol(string name)
        {
            if (SymbolPresent(name))
                return symbolTable[name];

            return null;
        }

        /// <summary>
        /// Test whether the symbol with given name is in the table
        /// </summary>
        /// <param name="name">Name of the searched symbol</param>
        /// <returns>True if symbol is present, false otherwise</returns>
        public bool SymbolPresent(string name)
        {
            return symbolTable.ContainsKey(name);
        }
        #endregion

        #region Functions
        /// <summary>
        /// Add function to the function table
        /// </summary>
        /// <param name="item">Function to be added</param>
        public void AddFunction(Function item)
        {
            functionTable[item.GetName()] = item;
        }

        /// <summary>
        /// Get function with given name from table
        /// </summary>
        /// <param name="name">Name fo the given function</param>
        /// <returns>The function, or null if the function is not in the table</returns>
        public Function GetFunction(string name)
        {
            if (FunctionPresent(name))
                return functionTable[name];

            return null;
        }

        /// <summary>
        /// Test whether the function with given name is in the table
        /// </summary>
        /// <param name="name">Name of the searched function</param>
        /// <returns>True if function is present, false otherwise</returns>
        public bool FunctionPresent(string key)
        {
            return functionTable.ContainsKey(key);
        }
        #endregion

    }
}
