using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happy_language
{
    class FuncItem
    {
        private String name;
        private DataType returnDataType;
        private int address;
        private List<FunctionParameter> parameters;

        public FuncItem(String name, DataType returnDataType, int address, List<FunctionParameter> parameters)
        {
            this.name = name;
            this.returnDataType = returnDataType;
            this.address = address;
            this.parameters = parameters;
        }

        public String ToString()
        {

            String param = "";
            for(int i = 0; i < parameters.Count; i++)
            {
                param += parameters[i] + ";"; 
            }

            return "Name: " + this.name + "; " +
                   "ret type: " + this.returnDataType.ToString() + "; " +
                   "pars: " + param
                ;
        }

        public String GetName()
        {
            return this.name;
        }

        public DataType GetReturnDataType()
        {
            return this.returnDataType;
        }

        public int GetAddress()
        {
            return this.address;
        }

        public List<FunctionParameter> GetParameters()
        {
            return this.parameters;
        }

    }
}
