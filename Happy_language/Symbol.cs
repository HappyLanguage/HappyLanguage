namespace Happy_language
{
    /// <summary>
    /// Access type of the symbol
    /// </summary>
    public enum SymbolType
    {
        /// <summary>
        /// Variable
        /// </summary>
        Var,

        /// <summary>
        /// Constant
        /// </summary>
        Const
    }

    /// <summary>
    /// Class representing variables, constants and arrays
    /// </summary>
    public class Symbol
    {
        /// <summary>
        /// Name of the symbol
        /// </summary>
        private string name;

        /// <summary>
        /// Acess type
        /// </summary>
        private SymbolType type;

        /// <summary>
        /// Data type
        /// </summary>
        private DataType dataType;

        /// <summary>
        /// Address of the symbol
        /// </summary>
        private int address;

        /// <summary>
        /// Level of memory where is the symbol located
        /// </summary>
        private int level;

        /// <summary>
        /// Flag representing whether the symbol is array
        /// </summary>
        private bool array;

        /// <summary>
        /// Size of the symbol
        /// </summary>
        private int length;

        /// <summary>
        /// Line where the symbol was declared
        /// </summary>
        private int declarationLine;


        /// <summary>
        /// Constructor for creating simple symbols - variables and constants
        /// </summary>
        /// <param name="name">Name of the symbol</param>
        /// <param name="type">Access type</param>
        /// <param name="dataType">Data type</param>
        /// <param name="declarationLine">Line where the symbol was declared</param>
        /// <param name="address">Address of the symbol</param>
        /// <param name="level">Level of memory where the symbol is located</param>
        public Symbol(string name, SymbolType type, DataType dataType, int declarationLine, int address, int level)
        {
            this.name = name;
            this.type = type;
            this.dataType = dataType;
            this.declarationLine = declarationLine;
            this.address = address;
            this.level = level;
            this.array = false;
        }

        /// <summary>
        /// Constructor for creating symbols with non default size - arrays
        /// </summary>
        /// <param name="name">Name of the symbol</param>
        /// <param name="length">Size of the array</param>
        /// <param name="type">Access type</param>
        /// <param name="dataType">Data type</param>
        /// <param name="declarationLine">Line where the symbol was declared</param>
        /// <param name="address">Address of the symbol</param>
        /// <param name="level">Level of memory where the symbol is located</param>
        public Symbol(string name, int length, SymbolType type, DataType dataType, int declarationLine, int address, int level)
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

        public bool isArray()
        {
            return this.array;
        }

        public override string ToString()
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

        public string GetName()
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
