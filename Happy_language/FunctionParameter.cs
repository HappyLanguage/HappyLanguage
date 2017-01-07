using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happy_language
{
	public class FunctionParameter
    {
        private String name;
        private DataType dataType;

        public FunctionParameter(String name, DataType dataType)
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
}
