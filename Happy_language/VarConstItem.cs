using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happy_language
{
	public class VarConstItem
    {
        private String name;
        private VarConstType type;
        private DataType dataType;
        private int address;
        private int level;
        private Boolean array;
        private int length;

        public VarConstItem(String name, VarConstType type, DataType dataType , int address, int level)
        {
            this.name = name;
            this.type = type;
            this.dataType = dataType;
            this.address = address;
            this.level = level;
            this.array = false;
        }

        public VarConstItem(String name, int length, VarConstType type, DataType dataType, int address, int level)
        {
            this.name = name;
            this.type = type;
            this.dataType = dataType;
            this.address = address;
            this.level = level;

            this.length = length;
            this.array = true;
        }

        public int GetLength()
        {
            return this.length;
        }

        public Boolean isArray()
        {
            return this.array;
        }

        public String ToString()
        {
            if (this.array)
            {
                return "Name: " + this.name + "; " +
                       "Len: " + this.length + "; " +
                       "Typ: array" + "; " +
                       "Data t: " + this.dataType.ToString() + "; " +
                       "Addr: " + this.address + "; " +
                       "Lev: " + this.level + "; "
                       ;
            }
            return "Name: " + this.name + "; " +  
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
