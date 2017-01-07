using System;
using System.Collections.Generic;

namespace Happy_language
{
    /// <summary>
    /// Class representing function parameters
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        private String name;

        /// <summary>
        /// Parameter's data type
        /// </summary>
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

    /// <summary>
    /// Class representing declared functions
    /// </summary>
    public class Function
    {
        /// <summary>
        /// Name of the function
        /// </summary>
        private String name;

        /// <summary>
        /// Return data type
        /// </summary>
        private DataType returnDataType;

        /// <summary>
        /// Address where the function starts
        /// </summary>
        private int address;

        /// <summary>
        /// List of parameters
        /// </summary>
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
            for (int i = 0; i < parameters.Count; i++)
            {
                param += parameters[i] + ";";
            }

            return "Name: " + this.name + "; " +
                   "ret type: " + this.returnDataType.ToString() + "; " +
                   "pars: " + param;
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
