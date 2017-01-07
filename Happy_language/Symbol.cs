using System;

namespace Happy_language
{
    public enum SymbolType
    {
        Var,
        Const
    }

    public class Symbol
    {
        private String name;
        private SymbolType type;
        private DataType dataType;
        private int address;
        private int level;
        private Boolean array;
        private int length;
        private int declarationLine;

        public Symbol(String name, SymbolType type, DataType dataType, int declarationLine, int address, int level)
        {
            this.name = name;
            this.type = type;
            this.dataType = dataType;
            this.declarationLine = declarationLine;
            this.address = address;
            this.level = level;
            this.array = false;
        }

        public Symbol(String name, int length, SymbolType type, DataType dataType, int declarationLine, int address, int level)
        {
            this.name = name;
            this.type = type;
            this.dataType = dataType;
            this.declarationLine = declarationLine;
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

        public override String ToString()
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

        public SymbolType GetSymbolType()
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

        public int GetDeclarationLine()
        {
            return declarationLine;
        }
    }
}
