using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happy_language
{
    public class SymbolTable
    {
        Dictionary<String, VarConstItem> varConstTable = new Dictionary<String, VarConstItem>();
        Dictionary<String, FuncItem> funcTable = new Dictionary<String, FuncItem>();

        public void AddVarConstItem(VarConstItem item)
        {
            varConstTable[item.GetName()] = item;
        }

        public VarConstItem GetVarConstItemByName(String name)
        {
            if(ContainsVarConstItem(name))
                return varConstTable[name];

            return null;
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

        public Boolean ContainsVarConstItem(String key)
        {
            return varConstTable.ContainsKey(key);
        }

        /* FUNCKCE */

        public Boolean Exists(FuncItem item)
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

        public Boolean ContainsFuncItem(String key)
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
