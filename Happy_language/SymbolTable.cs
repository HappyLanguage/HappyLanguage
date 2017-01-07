using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happy_language
{
    public class SymbolTable
    {
        Dictionary<string, Symbol> symbolTable = new Dictionary<string, Symbol>();
        Dictionary<string, Function> functionTable = new Dictionary<string, Function>();

        #region Symbols
        public void AddSymbol(Symbol symbol)
        {
            symbolTable[symbol.GetName()] = symbol;
        }

        public Symbol GetSymbol(string name)
        {
            if(SymbolPresent(name))
                return symbolTable[name];

            return null;
        }

        public bool SymbolPresent(string key)
        {
            return symbolTable.ContainsKey(key);
        }
        #endregion

        #region Functions
        public void AddFunction(Function item)
        {
            functionTable[item.GetName()] = item;
        }

        public Function GetFunction(string name)
        {
            if (FunctionPresent(name))
                return functionTable[name];

            return null;
        }

        public bool FunctionPresent(string key)
        {
            return functionTable.ContainsKey(key);
        }
        #endregion

    }
}
