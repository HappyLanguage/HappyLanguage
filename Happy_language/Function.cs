using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happy_language
{
    public class Parameter
    {
        private String name;
        private DataType dataType;

        public Parameter(String name, DataType dataType)
        {
            this.name = name;
            this.dataType = dataType;
        }

        public override string ToString()
        {
            return this.dataType + " " + this.name;
        }

        public String getName()
        {
            return this.name;
        }

        public DataType getDataType()
        {
            return this.dataType;
        }
    }


    public class Function
    {
        private String name;
        private DataType returnDataType;
        private int address;
        private List<Parameter> parameters;

        public Function(String name, DataType returnDataType, int address, List<Parameter> parameters)
        {
            this.name = name;
            this.returnDataType = returnDataType;
            this.address = address;
            this.parameters = parameters;
        }

        public override string ToString()
        {

            string param = "";
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

        public List<Parameter> GetParameters()
        {
            return this.parameters;
        }

    }
}
