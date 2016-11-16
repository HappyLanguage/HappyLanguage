using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happy_language
{
    class VarConstItem
    {
        private String name;
        private String value;
        private VarConstType type;
        private DataType dataType;
        private int address;
        private int level;

        public VarConstItem(String name, String value, VarConstType type, DataType dataType , int address, int level)
        {
            this.name = name;
            this.value = value;
            this.type = type;
            this.dataType = dataType;
            this.address = address;
            this.level = level;
        }

        public String ToString()
        {
            return "Name: " + this.name + "; " + 
                   "Val: " + this.value + "; " + 
                   "Typ: " + this.type.ToString() + "; " + 
                   "Data t: " + this.dataType.ToString() + "; " +
                   "Addr: " + this.address + "; " + 
                   "Lev: " + this.level + "; "
                   ;
        }

        public String GetName()
        {
            return this.name;
        }

        public String GetValue()
        {
            return this.value;
        }

        public void SetValue(String value)
        {
            this.value = value;
        }

        public VarConstType GetType()
        {
            return this.type;
        }

        public DataType GetDataType()
        {
            return this.dataType;
        }

        public int GetAddress()
        {
            return this.address;
        }

        public int GetLevel()
        {
            return this.level;
        }
    }
}
