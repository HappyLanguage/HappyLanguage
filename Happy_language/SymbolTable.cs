using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happy_language
{
    class SymbolTable
    {
        private int address;

        Dictionary<String, VarConstItem> varConstTable = new Dictionary<String, VarConstItem>();
        Dictionary<String, FuncItem> funcTable = new Dictionary<String, FuncItem>();

        public void AddVarConstItem(VarConstItem item)
        {
            varConstTable[item.GetName()] = item;
        }

        public VarConstItem GetVarConstItemByName(String name)
        {
            return varConstTable[name];
        }

        public String VarConstToString()
        {
            String s = "";
            foreach (String key in varConstTable.Keys)
            {
                s += varConstTable[key].ToString() + "\n";
            }
            return s;
        }

        public Boolean DoContainsVarConstItem(String key)
        {
            return varConstTable.ContainsKey(key);
        }

        /* FUNCKCE */

        public Boolean DoExist(FuncItem item)
        {
            Boolean exist = true;

            return exist;
        }

        public void AddFuncItem(FuncItem item)
        {
            funcTable[item.GetName()] = item;
        }

        public FuncItem GetFuncItemByName(String name)
        {
            return funcTable[name];
        }

        public Boolean DoContainsFuncItem(String key)
        {
            return funcTable.ContainsKey(key);
        }

        public String FuncsToString()
        {
            String s = "";
            foreach (String key in varConstTable.Keys)
            {
                s += funcTable[key].ToString() + "\n";
            }
            return s;
        }

    }
}
