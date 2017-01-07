using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.Diagnostics;
using static Happy_language.GrammarParser;
using Antlr4.Runtime;

namespace Happy_language
{
    public class Visitor : GrammarBaseVisitor<int>
    {
        #region Attributes
        /// <summary>
        /// Symbol table for global variables and constants
        /// </summary>
        private SymbolTable globalTable = new SymbolTable();

        /// <summary>
        /// Local symbol table
        /// </summary>
        private SymbolTable localTable = new SymbolTable();

        /// <summary>
        /// List of generated instructions
        /// </summary>
        private List<Instruction> instructions = new List<Instruction>();

        /// <summary>
        /// Class for reporting compile errors
        /// </summary>
        private ErrorHandler handler;

        /// <summary>
        /// Flag representing whether the instruction for jumping to main function was already created
        /// </summary>
        private bool jmpToMainDone = false;

        /// <summary>
        /// Address of the jump instruction to main function
        /// </summary>
        private int jmpToMainIndex = 0;

        /// <summary>
        /// Flag representing whether the visitor is in function
        /// </summary>
        private bool inFunction = false;

        /// <summary>
        /// Address used for addressing in function
        /// </summary>
        private int inFunctionAddress = 3;

        /// <summary>
        /// Number of current instructions
        /// </summary>
        private int instructionCount = 0;

        /// <summary>
        /// Current global addres
        /// </summary>
        private int globalAddress = 4;  // funcReturnAddress + 1

        /// <summary>
        /// Current level
        /// </summary>
        private int level = 0;

        /// <summary>
        /// Address where the returned value is stored
        /// </summary>
        private int funcReturnAddress = 3;

        /// <summary>
        /// Flag representing whether all global variables were already defined
        /// </summary>
        private bool globalVarsDefined = false;
        #endregion

        /// <summary>
        /// Constructor of the Parse tree visitor
        /// </summary>
        /// <param name="handler">Instance handling compilation errors</param>

        public Visitor(ErrorHandler handler) : base()
        {
            this.handler = handler;
        }

        #region Util methods
        /// <summary>
        /// Static method to convert string representation of boolean to 0 or 1
        /// </summary>
        /// <param name="value">String representation of boolean</param>
        /// <returns>Integer representation of boolean</returns>
        private static string BoolToInt(string value)
        {
            if (value.Equals("True", StringComparison.OrdinalIgnoreCase))
            {
                return "1";
            }
            else if (value.Equals("False", StringComparison.OrdinalIgnoreCase))
            {
                return "0";
            }
            else
            {
                throw new Exception("Invalid Bool value: " + value);
            }
        }

        public List<Instruction> GetInstructions()
        {
            return this.instructions;
        }

        /// <summary>
        /// Assigns numbers to generated instructions
        /// </summary>
        public void numberInstructions()
        {
            for (int i = 0; i < instructions.Count; i++)
            {
                instructions[i].Number = i;
            }
        }
        #endregion

        #region Library Functions
        /// <summary>
        /// Prepares the library functions
        /// </summary>
        public void PrepareLibraryFunctions()
        {
            AddJMP(0);
            PreparePrintASCIIFunction();
            PreparePrintIntFunction();
            PreparePrintNewLineFunction();
            PreparePrintBoolFunction();
            PrepareBoolToIntFunction();
            PrepareIntToBoolFunction();
            PrepareMinFunction();
            PrepareMaxFunction();
            PrepareAbsFunction();
            ChangeJMP(instructionCount, 0);
        }

        /// <summary>
        /// Prepares the Abs library function
        /// :I Abs(: :I value :)
        /// </summary>
        private void PrepareAbsFunction()
        {
            AddINT(4);
            AddLOD(0, 3);
            AddLIT("0");
            AddOPR(Instruction.LESS);
            AddJMC(instructionCount + 6);
            AddLIT("0");
            AddLOD(0, 3);
            AddOPR(Instruction.SUB);
            AddSTO(0, 3);
            AddJMP(instructionCount + 1);
            AddLOD(0, 3);
            AddSTO(1, funcReturnAddress);
            AddRET();

            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter("value", DataType.Int));
            globalTable.AddFunction(new Function("Abs", DataType.Int, instructionCount - 13, parameters));
        }

        /// <summary>
        /// Prepares the Max library function
        /// :I Max(: :I a, :I b :)
        /// </summary>
        private void PrepareMaxFunction()
        {
            AddINT(5);
            AddLIT("0");
            AddLOD(0, 3);
            AddLOD(0, 4);
            AddOPR(Instruction.GREATER);
            AddJMC(instructionCount + 4);
            AddLOD(0, 3);
            AddSTO(0, 5);
            AddJMP(instructionCount + 3);
            AddLOD(0, 4);
            AddSTO(0, 5);
            AddLOD(0, 5);
            AddSTO(1, funcReturnAddress);
            AddRET();

            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter("a", DataType.Int));
            parameters.Add(new Parameter("b", DataType.Int));
            globalTable.AddFunction(new Function("Max", DataType.Int, instructionCount - 14, parameters));
        }

        /// <summary>
        /// Prepares the Min library function
        /// :I Min(: :I a, :I b :)
        /// </summary>
        private void PrepareMinFunction()
        {
            AddINT(5);
            AddLIT("0");
            AddLOD(0, 3);
            AddLOD(0, 4);
            AddOPR(Instruction.LESS);
            AddJMC(instructionCount + 4);
            AddLOD(0, 3);
            AddSTO(0, 5);
            AddJMP(instructionCount + 3);
            AddLOD(0, 4);
            AddSTO(0, 5);
            AddLOD(0, 5);
            AddSTO(1, funcReturnAddress);
            AddRET();

            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter("a", DataType.Int));
            parameters.Add(new Parameter("b", DataType.Int));
            globalTable.AddFunction(new Function("Min", DataType.Int, instructionCount - 14, parameters));
        }

        /// <summary>
        /// Prepares the IntToBool library function
        /// :B IntToBool(: :I val :)
        /// </summary>
        private void PrepareIntToBoolFunction()
        {
            AddINT(4);

            AddJMC(instructionCount + 5);
            AddLIT("1");
            AddSTO(1, funcReturnAddress);
            AddINT(-4);
            AddRET();

            AddLIT("0");
            AddSTO(1, funcReturnAddress);
            AddINT(-4);
            AddRET();

            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter("value", DataType.Int));
            globalTable.AddFunction(new Function("IntToBool", DataType.Bool, instructionCount - 10, parameters));
        }

        /// <summary>
        /// Prepares the BoolToInt library function
        /// :I BoolToInt(: :B val :)
        /// </summary>
        private void PrepareBoolToIntFunction()
        {
            AddINT(4);
            AddSTO(1, funcReturnAddress);
            AddINT(-3);
            AddRET();

            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter("value", DataType.Bool));
            globalTable.AddFunction(new Function("BoolToInt", DataType.Int, instructionCount - 4, parameters));
        }

        /// <summary>
        /// Prepares PrintNewLine library function
        /// :V PrintNewLine(::)
        /// </summary>
        private void PreparePrintNewLineFunction()
        {
            AddINT(3);
            AddLIT("13");
            AddWRI();
            AddINT(-1);
            AddLIT("10");
            AddWRI();
            AddINT(-4);
            AddRET();
            globalTable.AddFunction(new Function("PrintNewLine", DataType.Void, instructionCount - 8, new List<Parameter>()));
        }

        /// <summary>
        /// Prepares PrintASCII library function
        /// :V PrintASCII(: :I val :)
        /// </summary>
        private void PreparePrintASCIIFunction()
        {
            AddINT(4);
            AddWRI();
            AddINT(-4);
            AddRET();
            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter("value", DataType.Int));
            globalTable.AddFunction(new Function("PrintASCII", DataType.Void, instructionCount - 4, parameters));
        }

        /// <summary>
        /// Prepares the PrintBool library function
        /// :V PrintBool(: :B val :)
        /// </summary>
        private void PreparePrintBoolFunction()
        {
            AddINT(4);

            AddJMC(instructionCount + 14);
            AddLIT("116");
            AddWRI();
            AddINT(-1);
            AddLIT("114");
            AddWRI();
            AddINT(-1);
            AddLIT("117");
            AddWRI();
            AddINT(-1);
            AddLIT("101");
            AddWRI();
            AddINT(-4);
            AddRET();

            AddLIT("102");
            AddWRI();
            AddINT(-1);
            AddLIT("97");
            AddWRI();
            AddINT(-1);
            AddLIT("108");
            AddWRI();
            AddINT(-1);
            AddLIT("115");
            AddWRI();
            AddINT(-1);
            AddLIT("101");
            AddWRI();
            AddINT(-4);
            AddRET();

            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter("value", DataType.Bool));
            globalTable.AddFunction(new Function("PrintBool", DataType.Void, instructionCount - 31, parameters));
        }

        /// <summary>
        /// Prepares PrintInt library function
        /// :V PrintInt(: :I val :)
        /// </summary>
        private void PreparePrintIntFunction()
        {
            AddINT(4);

            AddLOD(0, 3);
            AddLIT("0");
            AddOPR(Instruction.LESS);
            AddJMC(instructionCount + 5);
            AddOPR(Instruction.UNARY_MINUS);
            AddLIT("45");
            AddWRI();
            AddINT(-1);

            AddLIT("-1");

            AddLOD(0, 3);
            AddLIT("10");
            AddOPR(Instruction.MOD);
            AddLOD(0, 3);
            AddLIT("10");
            AddOPR(Instruction.DIV);
            AddJMC(instructionCount + 4);
            AddINT(1);
            AddSTO(0, 3);
            AddJMP(instructionCount - 9);

            AddLIT("1");
            AddOPR(Instruction.ADD);
            AddJMC(instructionCount + 7);
            AddINT(1);
            AddLIT("47");
            AddOPR(Instruction.ADD);
            AddWRI();
            AddINT(-1);
            AddJMP(instructionCount - 8);

            AddINT(-4);

            AddRET();
            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter("value", DataType.Int));
            globalTable.AddFunction(new Function("PrintInt", DataType.Void, instructionCount - 31, parameters));
        }
        #endregion

        #region Instruction handling
        /// <summary>
        /// Add WRI instruction to the instruction list
        /// </summary>
        private void AddWRI()
        {
            instructions.Add(new Instruction(InstructionType.WRI, 0, "0"));
            instructionCount += 1;
        }

        /// <summary>
        /// Add PST instruction to the instruction list
        /// </summary>
        private void AddPST()
        {
            instructions.Add(new Instruction(InstructionType.PST, 0, "0"));
            instructionCount += 1;
        }

        /// <summary>
        /// Add PLD instruction to the instruction list
        /// </summary>
        private void AddPLD()
        {
            instructions.Add(new Instruction(InstructionType.PLD, 0, "0"));
            instructionCount += 1;
        }

        /// <summary>
        /// Add LIT instruction to the instruction list
        /// </summary>
        /// <param name="value">string representation of the value to add to stack</param>
        private void AddLIT(string value)
        {
            instructions.Add(new Instruction(InstructionType.LIT, 0, value));
            instructionCount += 1;
        }

        /// <summary>
        /// Add STO instruction to the instruction list
        /// </summary>
        /// <param name="level">Level of the address to store</param>
        /// <param name="address">Address where to store</param>
        private void AddSTO(int level, int address)
        {
            instructions.Add(new Instruction(InstructionType.STO, level, address.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Add OPR instruction to the instruction list
        /// </summary>
        /// <param name="opCode">Operator code</param>
        private void AddOPR(int opCode)
        {
            instructions.Add(new Instruction(InstructionType.OPR, 0, opCode.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Add JMP instruction to the instruction list
        /// </summary>
        /// <param name="codeAddress">Address where to jump</param>
        private void AddJMP(int codeAddress)
        {
            instructions.Add(new Instruction(InstructionType.JMP, 0, codeAddress.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Change the address where to jump of the instruction with given index
        /// </summary>
        /// <param name="codeAddress">Address where to jump</param>
        /// <param name="index">Index of the JMP instruction in the instruction list</param>
        private void ChangeJMP(int codeAddress, int index)
        {
            Debug.Assert(instructions[index].Type == InstructionType.JMP);
            instructions[index].Value = codeAddress.ToString();
        }

        /// <summary>
        /// Add INT instruction to the instruction list
        /// </summary>
        /// <param name="value">Value of how much the top of the stack will be increased (or decreased if the value is negative)</param>
        private void AddINT(int value)
        {
            instructions.Add(new Instruction(InstructionType.INT, 0, value.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Adds CAL instruction to the instruction list
        /// </summary>
        /// <param name="level">Level of the address that will be called</param>
        /// <param name="address">Address that will be called</param>
        private void AddCAL(int level, int address)
        {
            instructions.Add(new Instruction(InstructionType.CAL, level, address.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Changes the CAL instruction on given index
        /// </summary>
        /// <param name="level">Level of the address that will be called</param>
        /// <param name="address">Address that will be called</param>
        /// <param name="index">Index of the instruction that will be changed</param>
        private void ChangeCAL(int level, int address, int index)
        {
            Debug.Assert(instructions[index].Type == InstructionType.CAL);

            instructions[index].Level = level;
            instructions[index].Value = address.ToString();
        }

        /// <summary>
        /// Add RET instruction to the instruction list
        /// </summary>
        private void AddRET()
        {
            instructions.Add(new Instruction(InstructionType.RET, 0, "0"));
            instructionCount += 1;
        }

        /// <summary>
        /// Add LOD instruction to the instruction list
        /// </summary>
        /// <param name="level">Level of the address from where to load the value</param>
        /// <param name="address">Address from where to load the value</param>
        private void AddLOD(int level, int address)
        {
            instructions.Add(new Instruction(InstructionType.LOD, level, address.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Add JMC instruction to the instruction list
        /// </summary>
        /// <param name="codeAddress">Address where to jump</param>
        private void AddJMC(int codeAddress)
        {
            instructions.Add(new Instruction(InstructionType.JMC, 0, codeAddress.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Change JMC instruction on given address
        /// </summary>
        /// <param name="index">Index of modified instruction</param>
        /// <param name="codeAddress">Address where to jump</param>
        private void ChangeJMC(int index, int codeAddress)
        {
            Debug.Assert(instructions[index].Type == InstructionType.JMC);
            instructions[index].Value = codeAddress.ToString();
        }

        /// <summary>
        /// Adds instructions for logical negation
        /// !val = val == false
        /// </summary>
        private void AddNeg()
        {
            AddLIT("0");
            AddOPR(Instruction.EQ);
        }

        /// <summary>
        /// Add instructions for doing initial jump
        /// </summary>
        public void DoInitialJmp()
        {
            AddJMP(instructionCount + 1);
            AddINT(4);
        }

        private void DoMainJmp(int dest)
        {
            AddCAL(0, dest);
            jmpToMainIndex = instructionCount - 1;
            AddRET();
            jmpToMainDone = true;
        }

        private void changeJMPtoMain()
        {
            AddINT(3);
            ChangeCAL(0, instructionCount - 1, jmpToMainIndex);
        }

        private void endProgram()
        {
            AddRET();
        }
        #endregion

        #region Memory handling
        /// <summary>
        /// Creates the constant using data from current parse tree context
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="name">Name of the constant</param>
        /// <returns>Object representing created constant</returns>
        private Symbol createConst(Def_constContext context, string name)
        {
            DataType dt = DataType.Int;

            if (context.Data_type_bool() != null)
            {
                dt = DataType.Bool;
            }

            int addr = 0;
            if (inFunction)
                addr = inFunctionAddress;
            else
                addr = globalAddress;

            return new Symbol(name, SymbolType.Const, dt, context.start.Line, addr, level);
        }

        /// <summary>
        /// Creates the variable using data from current parse tree context
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="name">Name of the variable</param>
        /// <returns>Object representing created variable</returns>
        private Symbol createVar(Def_varContext context, string name)
        {
            DataType dt = DataType.Int;

            if (context.Data_type_bool() != null)
            {
                dt = DataType.Bool;
            }

            int addr = 0;
            if (inFunction)
                addr = inFunctionAddress;
            else
                addr = globalAddress;

            return new Symbol(name, SymbolType.Var, dt, context.start.Line, addr, level);
        }

        /// <summary>
        /// Creates the array using data from current parse tree context
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns>Object representing created array</returns>
        private Symbol createArray(Array_inicializationContext context)
        {
            string name = context.Identifier().GetText();
            DataType dt = DataType.Int;

            int length = 0;
            if (context.Int() != null)
            {
                length = Convert.ToInt32(context.Int().GetText());
            }
            else if (context.String() != null)
            {
                length = context.String().GetText().Length - 1;
            }
            else if (context.number_array_assign() != null)
            {
                Number_array_assignContext values = context.number_array_assign();
                while (values != null)
                {
                    length += 1;
                    values = values.number_array_assign();
                }
            }
            else if (context.bool_array_assign() != null)
            {
                Bool_array_assignContext values = context.bool_array_assign();
                while (values != null)
                {
                    length += 1;
                    values = values.bool_array_assign();
                }
            }

            if (context.Data_type_bool() != null) dt = DataType.Bool;

            int addr = 0;
            if (inFunction)
                addr = inFunctionAddress;
            else
                addr = globalAddress;

            return new Symbol(name, length, SymbolType.Var, dt, context.start.Line, addr, level); ;
        }

        /// <summary>
        /// Creates function from given parse tree context
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns>Object representing created function</returns>
        private Function createFunction(Def_functionContext context)
        {
            string name = context.Identifier().GetText(); ;
            DataType returnDataType = DataType.Int;
            List<Parameter> parameters = new List<Parameter>();

            if (context.function_return_data_typ().Data_type_void() != null) returnDataType = DataType.Void;
            else if (context.function_return_data_typ().data_type().Data_type_bool() != null) returnDataType = DataType.Bool;

            ParametersContext paramContext = context.parameters();
            while (paramContext != null && paramContext.Identifier() != null)
            {
                DataType dType = DataType.Int;
                if (paramContext.data_type().Data_type_bool() != null) dType = DataType.Bool;

                parameters.Add(new Parameter(paramContext.Identifier().GetText(), dType));
                paramContext = paramContext.parameters();
            }

            return new Function(name, returnDataType, instructionCount, parameters);
        }

        /// <summary>
        /// Validate the given array and if it is OK insert it in the symbol table
        /// </summary>
        /// <param name="array">Processed array</param>
        /// <returns>Result of validation. Zero if OK, negative value if array is not valid</returns>
        private int processArray(Symbol array)
        {
            if (array.GetLength() < 0)
            {
                return handler.reportError(array.GetDeclarationLine(), ErrorCode.arrayLengthNegative,
                    "Array has negative length");
            }

            if (inFunction)
            {
                if (!localTable.SymbolPresent(array.GetName()))
                    localTable.AddSymbol(array);
                else
                {
                    Symbol alreadyDeclared = localTable.GetSymbol(array.GetName());

                    return handler.reportError(array.GetDeclarationLine(), ErrorCode.arrayAlreadyExists,
                        "Variable with name '" + array.GetName() + "' already declared on line " + alreadyDeclared.GetDeclarationLine());
                }
                inFunctionAddress += array.GetLength();
            }
            else
            {
                if (!globalTable.SymbolPresent(array.GetName()))
                    globalTable.AddSymbol(array);
                else
                {
                    Symbol alreadyDeclared = localTable.GetSymbol(array.GetName());

                    return handler.reportError(array.GetDeclarationLine(), ErrorCode.arrayAlreadyExists,
                        "Variable with name '" + array.GetName() + "' already declared on line " + alreadyDeclared.GetDeclarationLine());
                }
                globalAddress += array.GetLength();
            }

            return 0;
        }

        /// <summary>
        /// Validate the given variable or constant and if it is OK insert it in the symbol table
        /// </summary>
        /// <param name="item">Processed variable or constant</param>
        /// <returns>0 if OK, negative value if the value or constant is not valid</returns>
        private int processVarConst(Symbol item)
        {
            if (inFunction)
            {
                if (!localTable.SymbolPresent(item.GetName()))
                    localTable.AddSymbol(item);
                else
                {
                    Symbol alreadyDeclared = localTable.GetSymbol(item.GetName());

                    return handler.reportError(item.GetDeclarationLine(), ErrorCode.varConstAlreadyExists,
                        "Variable with name '" + item.GetName() + "' already declared on line " + alreadyDeclared.GetDeclarationLine());
                }

                inFunctionAddress += 1;
            }
            else
            {
                if (!globalTable.SymbolPresent(item.GetName()))
                    globalTable.AddSymbol(item);
                else
                {
                    Symbol alreadyDeclared = localTable.GetSymbol(item.GetName());

                    return handler.reportError(item.GetDeclarationLine(), ErrorCode.varConstAlreadyExists,
                        "Variable with name '" + item.GetName() + "' already declared on line " + alreadyDeclared.GetDeclarationLine());
                }
                globalAddress += 1;
            }

            return 0;
        }

        /// <summary>
        /// Find the symbol name in symbol tables
        /// </summary>
        /// <param name="varConstName">Name of the symbol</param>
        /// <returns></returns>
        private Symbol GetSymbol(string varConstName)
        {
            if (localTable.SymbolPresent(varConstName))
                return localTable.GetSymbol(varConstName);
            else if (globalTable.SymbolPresent(varConstName))
                return globalTable.GetSymbol(varConstName);

            return null;
        }
        #endregion

        #region Visitors

        #region Definitions
        /// <summary>
        /// Constant definition
        /// </summary>
        /// <param name="context"></param>
        /// <returns>((int) DataType) if ok, negative value if semantic error</returns>
        public override int VisitDef_const([NotNull] Def_constContext context)
        {
            int result = 0;
            int column = 0;

            //for each left side assign given value
            Multiple_assignContext leftSides = context.multiple_assign();
            while (leftSides != null)
            {
                Symbol newConst = createConst(context, leftSides.Identifier().GetText());
                result = processVarConst(newConst);

                if (result < 0)
                    return result;

                result = VisitRightSide(context, out column);

                if (result < 0)
                    return result;

                if (newConst.GetDataType() != (DataType)result)
                {
                    return handler.reportError(context.start.Line, column, ErrorCode.assignmentMismatch,
                        "Cannot assign from " + ((DataType)result) + " to " + newConst.GetDataType());
                }

                leftSides = leftSides.multiple_assign();
            }

            return result;
        }

        /// <summary>
        /// Variable definition
        /// </summary>
        /// <param name="context"></param>
        /// <returns>((int) DataType) if ok, negative value if semantic error</returns>
        public override int VisitDef_var([NotNull] Def_varContext context)
        {
            int result = 0;
            int column = 0;
            if (context.array_inicialization() == null)
            {

                //for each leftSide assign given value
                Multiple_assignContext leftSides = context.multiple_assign();
                while (leftSides != null)
                {
                    Symbol newVar = createVar(context, leftSides.Identifier().GetText());
                    result = processVarConst(newVar);

                    if (result < 0)
                        return result;

                    result = VisitRightSide(context, out column);

                    if (result < 0)
                        return result;

                    if (newVar.GetDataType() != (DataType)result)
                    {
                        return handler.reportError(context.start.Line, column, ErrorCode.assignmentMismatch,
                        "Cannot assign from " + ((DataType)result) + " to " + newVar.GetDataType());
                    }

                    leftSides = leftSides.multiple_assign();
                }
            }
            else
            {
                //initialize array
                result = VisitArray_inicialization(context.array_inicialization());
            }

            return result;
        }

        /// <summary>
        /// Array inicialization
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
		public override int VisitArray_inicialization([NotNull] Array_inicializationContext context)
        {
            Symbol newArray = createArray(context);
            int result = processArray(newArray);

            if (result < 0)
                return result;

            if (context.Int() != null)
            {
                //if only size is given, fill memory with zeros
                for (int i = 0; i < newArray.GetLength(); i++)
                    AddLIT("0");
            }
            else if (context.String() != null)
            {
                //if string provided, fill memory with the characters ASCII values and add zero
                string content = context.String().GetText();
                for (int i = 1; i < newArray.GetLength(); i++)
                {
                    AddLIT(Convert.ToString(Convert.ToInt32(content[i])));
                }

                //null terminated string
                AddLIT("0");
            }
            else if (context.number_array_assign() != null)
            {
                //list of arithmetic expressions
                Number_array_assignContext values = context.number_array_assign();
                while (values != null)
                {
                    result = VisitExpression(values.expression());

                    if (result < 0)
                    {
                        return result;
                    }
                    else if (newArray.GetDataType() != (DataType)result)
                    {
                        return handler.reportError(values.start.Line, values.expression().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to array item of type " + newArray.GetDataType());
                    }

                    values = values.number_array_assign();
                }
            }
            else if (context.bool_array_assign() != null)
            {
                //list of logical expressions
                Bool_array_assignContext values = context.bool_array_assign();
                int column = 0;
                while (values != null)
                {
                    if (values.condition() != null)
                    {
                        result = Visit(values.condition());
                        column = values.condition().start.Column;
                    }
                    else if (values.expression() != null)
                    {
                        result = Visit(values.expression());
                        column = values.expression().Start.Column;
                    }

                    if (result < 0)
                    {
                        return result;
                    }
                    else if (newArray.GetDataType() != (DataType)result)
                    {
                        return handler.reportError(values.start.Line, column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to array item of type " + newArray.GetDataType());
                    }

                    values = values.bool_array_assign();
                }
            }

            return result;
        }

        /// <summary>
        /// Function definition
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitDef_function([NotNull] Def_functionContext context)
        {
            inFunction = true;
            globalVarsDefined = true;
            localTable = new SymbolTable();
            if (!jmpToMainDone) DoMainJmp(0);

            //check if the function is not already defined
            Function newItem = createFunction(context);
            if (!globalTable.FunctionPresent(newItem.GetName()))
            {
                globalTable.AddFunction(newItem);
            }
            else
            {
                return handler.reportError(context.Start.Line, ErrorCode.functionAlreadyExists,
                    "Function with name '" + newItem.GetName() + "' already defined.");
            }

            AddINT(3 + newItem.GetParameters().Count);

            level += 1;
            inFunctionAddress = 3;

            //process parameters
            for (int i = 0; i < newItem.GetParameters().Count; i++)
            {
                Symbol parItem = new Symbol(newItem.GetParameters()[i].getName(), SymbolType.Var,
                    newItem.GetParameters()[i].getDataType(), context.start.Line, inFunctionAddress, level);

                localTable.AddSymbol(parItem);
                inFunctionAddress += 1;
            }

            //process function block
            int result = Visit(context.block_function());

            if (result < 0)
            {
                return result;
            }

            //process return command
            result = Visit(context.function_return());

            if (result < 0)
            {
                return result;
            }

            if ((DataType)result != newItem.GetReturnDataType())
            {
                return handler.reportError(context.function_return().Start.Line, ErrorCode.returnTypeMismatch,
                    "Return value must be of " + newItem.GetReturnDataType() + " type");
            }

            level -= 1;

            AddRET();
            inFunction = false;
            localTable = null;

            return result;
        }

        /// <summary>
        /// Return command
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitFunction_return([NotNull] Function_returnContext context)
        {
            int result = 0;
            if (context.condition() == null && context.expression() == null && context.ternary_operator() == null)
            {
                //Void
                return result;
            }

            if (context.expression() != null)
            {
                result = VisitExpression(context.expression());
            }
            else if (context.condition() != null)
            {
                result = Visit(context.condition());
            }
            else if (context.ternary_operator() != null)
            {
                result = VisitTernary_operator(context.ternary_operator());
            }

            //Store the result on predefined address
            AddSTO(level, funcReturnAddress);

            return result;
        }
        #endregion

        #region Assignment
        /// <summary>
        /// Asignment command with possible multiple left sides
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitAssignment([NotNull] AssignmentContext context)
        {
            int result = 0;
            int column = 0;

            //for each left side assign right side
            Multiple_assignContext leftSides = context.multiple_assign();
            while (leftSides != null)
            {
                string leftSideName = leftSides.Identifier().GetText();
                Symbol leftSide = GetSymbol(leftSideName);

                if (leftSide == null)
                {
                    return handler.reportError(context.start.Line, ErrorCode.unknownSymbol,
                        "Unknown variable with name '" + leftSideName + "'");
                }

                if (leftSide.GetSymbolType() == SymbolType.Const)
                {
                    return handler.reportError(context.start.Line, ErrorCode.assignmentToConstant,
                        "Cannot assign to a constant");
                }

                //process right side
                result = VisitRightSide(context.assignment_right_side(), out column);
                if (result < 0)
                {
                    return result;
                }

                if (leftSide.GetDataType() != (DataType)result)
                {
                    return handler.reportError(context.Start.Line, column,
                        ErrorCode.assignmentMismatch,
                        "Cannot assign from " + ((DataType)result) + " to " + leftSide.GetDataType());
                }

                //store right side to given address
                int varLevel = leftSide.GetLevel();
                int varAddress = leftSide.GetAddress();
                int levelToMove = Math.Abs(level - varLevel);
                AddSTO(levelToMove, varAddress);

                if (result < 0)
                    return result;

                leftSides = leftSides.multiple_assign();
            }

            return result;
        }

        /// <summary>
        /// Assignment with only one variable on the left side
        /// It is also used in for loop
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitOne_assignment([NotNull] One_assignmentContext context)
        {
            int result = 0;
            int column = 0;
            string leftSideName = context.Identifier().GetText();
            Symbol leftSide = GetSymbol(leftSideName);

            if (leftSide == null)
            {
                return handler.reportError(context.start.Line, ErrorCode.unknownSymbol,
                    "Unknown variable with name '" + leftSideName + "'");
            }

            if (leftSide.GetSymbolType() == SymbolType.Const)
            {
                return handler.reportError(context.start.Line, ErrorCode.assignmentToConstant,
                    "Cannot assign to a constant");
            }

            //process right side
            result = VisitRightSide(context.assignment_right_side(), out column);

            if (result < 0)
            {
                return result;
            }

            if (leftSide.GetDataType() != (DataType)result)
            {
                return handler.reportError(context.start.Line, column,
                    ErrorCode.assignmentMismatch,
                    "Cannot assign from " + ((DataType)result) + " to " + leftSide.GetDataType());
            }
            
            //store right side to the given address
            int varLevel = leftSide.GetLevel();
            int varAddress = leftSide.GetAddress();
            int levelToMove = Math.Abs(level - varLevel);
            AddSTO(levelToMove, varAddress);

            leftSide = null;

            return result;
        }

        /// <summary>
        /// Right side of the assignment
        /// </summary>
        /// <param name="context">Given assignment context</param>
        /// <param name="column">output parameter determining the column where the right side is located</param>
        /// <returns></returns>
        public int VisitRightSide([NotNull] ParserRuleContext context, out int column)
        {
            if (context.GetChild<Condition_expressionContext>(0) != null)
            {
                //process logical expresssion
                Condition_expressionContext condition_expression = context.GetChild<Condition_expressionContext>(0);
                column = condition_expression.Start.Column;
                return Visit(condition_expression);
            }
            else if (context.GetChild<ConditionContext>(0) != null)
            {
                //process arithmetic expression
                ConditionContext condition = context.GetChild<ConditionContext>(0);
                column = condition.Start.Column;
                return Visit(condition);
            }
            else if (context.GetChild<ExpressionContext>(0) != null)
            {
                //process arithmetic expression
                ExpressionContext expression = context.GetChild<ExpressionContext>(0);
                column = expression.Start.Column;
                return Visit(expression);
            }
            else if (context.GetChild<Function_callContext>(0) != null)
            {
                //process function call
                Function_callContext function_call = context.GetChild<Function_callContext>(0);
                column = function_call.Start.Column;
                return Visit(function_call);
            }
            else
            {
                //process ternary operator
                Ternary_operatorContext ternary_operator = context.GetChild<Ternary_operatorContext>(0);
                column = ternary_operator.Start.Column;
                return Visit(ternary_operator);
            }
        }

        /// <summary>
        /// Assignment to item of array
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitAssignment_array([NotNull] Assignment_arrayContext context)
        {
            string arrayName = context.array_index().Identifier().GetText();
            Symbol leftSide = GetSymbol(arrayName);

            if (leftSide == null)
            {
                return handler.reportError(context.Start.Line, ErrorCode.arrayDoesNotExist, "Unknown array '" + arrayName + "'");
            }

            int result = 0;
            int column = 0;

            //process right side of the assignment command
            result = VisitRightSide(context, out column);

            if (result < 0)
                return result;

            if (leftSide.GetDataType() != (DataType)result)
            {
                return handler.reportError(context.start.Line, column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to array item of type " + leftSide.GetDataType());
            }

            IndexContext indexContext = context.array_index().index();

            int varLevel = leftSide.GetLevel();
            int levelToMove = Math.Abs(level - varLevel);

            if (indexContext.Int() != null)
            {
                //index given by literal
                int index = Convert.ToInt32(indexContext.Int().GetText());

                //the value is known on compile time - we can validate it
                if (index < 0)
                {
                    return handler.reportError(context.Start.Line, ErrorCode.negativeIndex, "Negative index");
                }

                if (index >= leftSide.GetLength())
                {
                    return handler.reportError(context.Start.Line, ErrorCode.indexOutOfBounds, "Index out of bounds");
                }

                AddSTO(levelToMove, leftSide.GetAddress() + index);
            }
            else if (indexContext.expression() != null)
            {
                //index given by variable

                //add level to the stack
                AddLIT(levelToMove.ToString());

                //add base address to the stack
                AddLIT(leftSide.GetAddress().ToString());

                //process arithmetic expression
                result = VisitExpression(indexContext.expression());

                if (result < 0)
                {
                    return result;
                }

                if (DataType.Int != ((DataType)result))
                {
                    return handler.reportError(context.Start.Line, indexContext.expression().start.Column, ErrorCode.indexTypeMismatch,
                            "Index must be of type Int");
                }


                //given item is on address base + top of the stack
                AddOPR(Instruction.ADD);
                AddPST();
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Visit Main entrance of the program
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitMain([NotNull] MainContext context)
        {
            globalVarsDefined = true;
            if (!jmpToMainDone) DoMainJmp(0);
            inFunction = true;
            inFunctionAddress = 3;
            level += 1;
            localTable = new SymbolTable();

            changeJMPtoMain();
            int result = base.VisitMain(context);
            endProgram();

            inFunction = false;
            localTable = null;
            level -= 1;
            return result;
        }

        /// <summary>
        /// Function call
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitFunction_call([NotNull] Function_callContext context)
        {
            string fName = context.Identifier().GetText();

            //check if function exists
            Function calledFce = null;
            if (globalTable.FunctionPresent(fName)) calledFce = globalTable.GetFunction(fName);
            else
            {
                return handler.reportError(context.start.Line, ErrorCode.functionDoesNotExist,
                    "Uknown function with name '" + fName + "'");
            }

            //increase top of the stack
            AddINT(3);
            
            //process all parameters
            List<Symbol> usedParameters = new List<Symbol>();
            Par_in_functionContext paramContext = context.par_in_function();
            while (paramContext != null)
            {
                Symbol par = null;

                if (paramContext.expression() != null)
                {
                    int result = VisitExpression(paramContext.expression());
                    par = new Symbol("", SymbolType.Var, (DataType)result, context.start.Line, 0, 0);
                }
                else if (paramContext.condition_expression() != null)
                {
                    VisitCondition_expression(paramContext.condition_expression());
                    par = new Symbol("", SymbolType.Var, DataType.Bool, context.start.Line, 0, 0);
                }

                if (par != null)
                {
                    usedParameters.Add(par);
                }
                paramContext = paramContext.par_in_function();
            }

            //check parameter count
            List<Parameter> requestedParameters = calledFce.GetParameters();
            if (requestedParameters.Count != usedParameters.Count)
            {
                return handler.reportError(context.Start.Line, context.par_in_function().Stop.Column,
                    ErrorCode.functionWrongParameterCount,
                    "Wrong parameter count, expected: " + requestedParameters.Count);
            }

            for (int i = 0; i < requestedParameters.Count; i++)
            {
                if (requestedParameters[i].getDataType() != usedParameters[i].GetDataType())
                {
                    return handler.reportError(context.Start.Line,
                        ErrorCode.functionParameterDataTypeMismatch,
                        "Type mismatch in parameter number " + (i + 1));
                }
            }

            //decrease the top of the stack
            AddINT(-1 * (3 + usedParameters.Count()));

            //call the function
            AddCAL(globalVarsDefined ? 1 : 0, calledFce.GetAddress());

            //if the function has return value load it to the top of the stack from predefined address
            if (calledFce.GetReturnDataType() != DataType.Void)
                AddLOD(level, funcReturnAddress);

            return (int)calledFce.GetReturnDataType();
        }


        #region Arithmetic expressions
        /// <summary>
        /// Arithmetic expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Data type, or negative value if error occured</returns>
        public override int VisitExpression([NotNull] ExpressionContext context)
        {
            int ret1 = 0, ret2 = 0;
            if (context.expression() != null)
            {
                ret1 = VisitExpression(context.expression());

                if (ret1 < 0)
                    return ret1;
            }
            if (context.expression_multiply() != null)
            {
                ret2 = VisitExpression_multiply(context.expression_multiply());

                if (ret1 < 0)
                    return ret2;
            }

            //operation +
            if (context.Add() != null)
            {
                //expressions on both sides must have same type
                if (ret1 != ret2)
                {
                    return handler.reportError(context.Start.Line, ErrorCode.assignmentMismatch,
                        "Canot perform '+' operation on conflicting data types");
                }

                //bool is not allowed here
                if ((DataType)ret1 == DataType.Bool)
                {
                    return handler.reportError(context.Start.Line, ErrorCode.operatorTypeMismatch, 
                        "Cannot perform '+' operation on Bool type");
                }

                AddOPR(Instruction.ADD);

            }
            //operation -
            else if (context.Sub() != null)
            {
                //expressions on both sides must have same type
                if (ret1 != ret2)
                {
                    return handler.reportError(context.Start.Line, ErrorCode.assignmentMismatch, 
                        "Canot perform '-' operation on conflicting data types");
                }

                //bool is not allowed here
                if ((DataType)ret1 == DataType.Bool)
                {
                    return handler.reportError(context.Start.Line, ErrorCode.operatorTypeMismatch, 
                        "Cannot perform '-' operation on Bool type");
                }

                AddOPR(Instruction.SUB);
            }

            return ret2;
        }

        /// <summary>
        /// Arithmetic expression with with '*' or '/'
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Data type, or negative value if error occured</returns>
        public override int VisitExpression_multiply([NotNull] Expression_multiplyContext context)
        {
            int ret1 = 0, ret2 = 0;
            if (context.expression_multiply() != null)
            {
                ret1 = VisitExpression_multiply(context.expression_multiply());
                if (ret1 < 0)
                    return ret1;
            }

            ret2 = VisitExpression_item(context.expression_item());

            if (ret2 < 0)
                return ret2;

            //operation '*'
            if (context.Mul() != null)
            {
                //both sides must be of the same type
                if (ret1 != ret2)
                {
                    return handler.reportError(context.Start.Line, ErrorCode.assignmentMismatch, 
                        "Canot perform '*' operation on conflicting data types");
                }

                //bool is not allowed here
                if ((DataType)ret1 == DataType.Bool)
                {
                    return handler.reportError(context.Start.Line, ErrorCode.operatorTypeMismatch, 
                        "Cannot perform '*' operation on Bool type");
                }

                AddOPR(Instruction.MUL);
            }
            //operation '/'
            else if (context.Div() != null)
            {
                //both sides must be of the same type
                if (ret1 != ret2)
                {
                    return handler.reportError(context.Start.Line, ErrorCode.assignmentMismatch,
                        "Canot perform '/' operation on conflicting data types");
                }

                //bool is not allowed here
                if ((DataType)ret1 == DataType.Bool)
                {
                    return handler.reportError(context.Start.Line, ErrorCode.operatorTypeMismatch,
                        "Cannot perform '/' operation on Bool type");
                }

                AddOPR(Instruction.DIV);
            }

            return ret2;
        }

        /// <summary>
        /// Item of the expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Data type, or negative value if error occured</returns>
        public override int VisitExpression_item([NotNull] Expression_itemContext context)
        {
            int result = 0;
            //variable
            if (context.Identifier() != null)
            {
                string varConstName = context.Identifier().GetText();

                //look up the variable in the symbol table
                Symbol symbol = GetSymbol(varConstName);
                if (symbol == null)
                {
                    return handler.reportError(context.Start.Line, ErrorCode.unknownSymbol,
                        "Unknown variable with name'" + varConstName + "'");
                }

                if (symbol.isArray()) {
                    return handler.reportError(context.Start.Line, ErrorCode.notIndexedArray,
                        "Cannot access array without specifying the index.");
                }

                //load the variable into stack
                int varLevel = symbol.GetLevel();
                int levelToMove = Math.Abs(level - varLevel);
                AddLOD(levelToMove, symbol.GetAddress());

                //add unary minus if it was specified
                if (context.Sub() != null)
                {
                    AddOPR(Instruction.UNARY_MINUS);
                }

                result = (int)symbol.GetDataType();
            }
            //array item
            else if (context.array_index() != null)
            {
                //load given item to the stack
                result = Visit(context.array_index());

                if (result < 0)
                {
                    return result;
                }

                //add unary minus if it was specified
                if (context.Sub() != null)
                {
                    if ((DataType) result != DataType.Int)
                    {
                        return handler.reportError(context.Start.Line, context.array_index().Start.Column,
                            ErrorCode.operatorTypeMismatch,
                            "Cannot use unary minus on " + (DataType) result);
                    }
                    AddOPR(Instruction.UNARY_MINUS);
                }

            }
            //function call
            else if (context.function_call() != null)
            {
                //call function so that when it finishes, result will be on the top of the stack
                result = VisitFunction_call(context.function_call());

                //add unary minus if it was specified
                if (context.Sub() != null)
                {
                    if ((DataType)result != DataType.Int)
                    {
                        return handler.reportError(context.Start.Line, context.function_call().Start.Column,
                            ErrorCode.operatorTypeMismatch,
                            "Cannot use unary minus on " + (DataType)result);
                    }

                    AddOPR(Instruction.UNARY_MINUS);
                }

                if (result < 0)
                {
                    return result;
                }
            }
            //arithmetic expression in brackets
            else if (context.expression() != null)
            {
                //perform the expression so that the result is on the top of the stack
                result = VisitExpression(context.expression());

                if (result < 0)
                {
                    return result;
                }
            }
            //literal
            else
            {
                //add literal to the stack
                AddLIT(context.Int().GetText());

                //add unary minus if it was specified
                if (context.Sub() != null)
                {
                    AddOPR(Instruction.UNARY_MINUS);
                }

                result = (int)DataType.Int;
            }

            return result;
        }

        /// <summary>
        /// Load array item on given index
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitArray_index([NotNull] Array_indexContext context)
        {
            int result = 0;

            //find array in symbol table
            Symbol array = GetSymbol(context.Identifier().GetText());
            if (array == null)
            {
                return handler.reportError(context.Start.Line, ErrorCode.arrayDoesNotExist,
                    "Unknown array '" + context.Identifier().GetText() + "'");
            }

            if (!array.isArray())
            {
                return handler.reportError(context.Start.Line, ErrorCode.notAnArray,
                    "Variable with name '" + array.GetName() + "' is not an array");
            }

            IndexContext indexContext = context.index();

            int varLevel = array.GetLevel();
            int varAddress = array.GetAddress();
            int levelToMove = Math.Abs(level - varLevel);

            //Index specified by literal
            if (indexContext.Int() != null)
            {
                int index = Convert.ToInt32(indexContext.Int().GetText());

                //We can actually validate the index
                if (index < 0)
                {
                    return handler.reportError(context.Start.Line, ErrorCode.negativeIndex, "Negative index");
                }
                if (index >= array.GetLength())
                {
                    return handler.reportError(context.Start.Line, ErrorCode.indexOutOfBounds, "Index out of bounds");
                }

                AddLOD(levelToMove, varAddress + index);
            }
            //Index specified by expresion
            else if (indexContext.expression() != null)
            {
                //add level to the stack
                AddLIT(levelToMove.ToString());

                //add base address to the stack
                AddLIT(varAddress.ToString());

                //calculate the index
                result = VisitExpression(indexContext.expression());

                if (result < 0)
                {
                    return result;
                }

                //check the data type of the index
                if (DataType.Int != ((DataType)result))
                {
                    return handler.reportError(context.Start.Line, indexContext.expression().start.Column,
                        ErrorCode.indexTypeMismatch,
                        "Index must be of type Int");
                }

                //address to load is base + index
                AddOPR(Instruction.ADD);
                AddPLD();
            }

            return (int)array.GetDataType();
        }
        #endregion

        #region Loops and conditional jumps
        /// <summary>
        /// Ternary operator
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitTernary_operator([NotNull] Ternary_operatorContext context)
        {
            //perform condition
            Visit(context.condition());

            //add conditional jump
            int jmcAddress = instructionCount;
            AddJMC(0);

            //process subexpression assigned when condition == true
            int ret1 = Visit(context.expression()[0]);
            if (ret1 < 0)
            {
                return ret1;
            }

            //change conditional jump and add jump to skip the second subexpression
            int jmpAddress = instructionCount;
            AddJMP(0);
            ChangeJMC(jmcAddress, instructionCount);
            
            //process subexpression assigned when condition == false
            int ret2 = Visit(context.expression()[1]);
            if (ret2 < 0)
            {
                return ret2;
            }

            //change the skip jump
            ChangeJMP(instructionCount, jmpAddress);

            //validate that both subexpressions are of the same type
            if (ret1 != ret2)
            {
                return handler.reportError(context.start.Line, context.start.Column,
                    ErrorCode.subExpressionMismatch,
                    "Subexpressions of ternary operator must have same data type");
            }

            return ret1;
        }

        /// <summary>
        /// If clause
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitIf([NotNull] IfContext context)
        {
            //process condition
            Visit(context.condition());

            //add conditional jump
            int jmcAddress = instructionCount;
            AddJMC(0);

            //process if block
            Visit(context.block());

            //add jump to skip else clause
            int jmpAddress = instructionCount;
            AddJMP(0);
            ChangeJMC(jmcAddress, instructionCount);

            //process else clause
            Visit(context.else_if());

            //change the skip jump
            ChangeJMP(instructionCount, jmpAddress);
            return 10;
        }

        /// <summary>
        /// Else and Else if clause
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitElse_if([NotNull] Else_ifContext context)
        {
            //else if clause is present
            if (context.If() != null)
            {
                //process condition
                Visit(context.condition());

                //add conditional jump
                int jmcAddress = instructionCount;
                AddJMC(0);

                //process  else if block
                Visit(context.block());

                //add jump to skip to the end
                int jmpAddress = instructionCount;
                AddJMP(0);
                ChangeJMC(jmcAddress, instructionCount);

                //process next else if clause
                Visit(context.else_if());

                //change the skip jump
                ChangeJMP(instructionCount, jmpAddress);
                return 11;
            }
            //no else if clause is present
            else
            {
                Visit(context.@else());
                return 12;
            }
        }

        /// <summary>
        /// Else clause
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitElse([NotNull] ElseContext context)
        {
            //if there is else clause, process its block
            if (context.block() != null)
            {
                Visit(context.block());
                return 0;
            }

            return 1;
        }

        /// <summary>
        /// While loop
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitWhile([NotNull] WhileContext context)
        {
            //process condition
            int conditionAddress = instructionCount;
            Visit(context.condition());

            //add conditional jump
            int jmcAddress = instructionCount;
            AddJMC(0);

            //process while block
            Visit(context.block());

            //jump back to condition
            AddJMP(conditionAddress);

            //change conditional jump to the end of the loop
            ChangeJMC(jmcAddress, instructionCount);

            return 11;
        }

        /// <summary>
        /// Do-while loop
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitDo_while([NotNull] Do_whileContext context)
        {

            int firstAddress = instructionCount;

            //process the block
            Visit(context.block());

            //process the conditon
            Visit(context.condition());

            //Value must be negated to jump when true
            AddNeg();

            //conditional jump when true to the beginning
            AddJMC(firstAddress);

            return 0;
        }

        /// <summary>
        /// For loop
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitFor([NotNull] ForContext context)
        {
            For_conditionContext forCondition = context.for_condition();

            //process assignment if not empty
            if (forCondition.one_assignment() != null)
            {
                Visit(forCondition.one_assignment());
            }

            int conditionAddress = instructionCount;
            int jmcAddress = 0;

            //process condition if not empty
            if (forCondition.condition() != null)
            {
                Visit(forCondition.condition());
                jmcAddress = instructionCount;
                AddJMC(0);
            }

            //process block
            Visit(context.block());

            //process increment
            Visit(forCondition.increment());

            //jump back to the condition
            AddJMP(conditionAddress);

            //change jmc to jump to the end when condition == false
            ChangeJMC(jmcAddress, instructionCount);

            return 0;
        }
        
        /// <summary>
        /// Increment in the for loop
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitIncrement([NotNull] IncrementContext context)
        {
            //if there is any assignment, process it
            if (context.one_assignment() != null)
            {
                return Visit(context.one_assignment());
            }

            //increment can be empty
            return 0;
        }
        #endregion

        #region Logical expressions
        /// <summary>
        /// Condition used in 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitCondition([NotNull] ConditionContext context)
        {
            if (context.condition_expression() != null)
            {
                Visit(context.condition_expression());
            }

            if (context.expression() != null)
            {
                int ret = Visit(context.expression());

                if (ret < 0)
                {
                    return ret;
                }

                if ((DataType)ret != DataType.Bool)
                {
                    return handler.reportError(context.Start.Line, context.expression().Start.Column,
                        ErrorCode.conditionTypeMismatch, "Expression in condition must be of the Bool type");
                }

                if (context.Negation() != null)
                {
                    AddNeg();
                }
            }

            ConditionContext condition1 = context.GetChild<ConditionContext>(0);
            if (condition1 != null)
            {
                Visit(condition1);

                if (context.Negation() != null)
                {
                    AddNeg();
                }
            }

            ConditionContext condition2 = context.GetChild<ConditionContext>(1);
            if (condition2 != null)
            {
                Visit(condition2);
            }

            ITerminalNode logicalOperator = context.Logical_operator();
            if (logicalOperator != null)
            {
                if (logicalOperator.GetText() == "||")
                {
                    AddOPR(Instruction.ADD);
                    AddLIT("1");
                    AddOPR(Instruction.GEQ);
                }
                else
                {
                    AddOPR(Instruction.MUL);
                }
            }

            return (int) DataType.Bool;
        }

        public override int VisitCondition_expression([NotNull] Condition_expressionContext context)
        {
            if (context.Bool() != null)
            {
                AddLIT(BoolToInt(context.Bool().GetText()));

                return (int)DataType.Bool;
            }

            int ret1 = Visit(context.condition_item()[0]);
            int ret2 = Visit(context.condition_item()[1]);

            if (ret1 < 0)
            {
                return ret1;
            }
            else if (ret2 < 0)
            {
                return ret2;
            }

            if (ret1 != ret2)
            {
                return handler.reportError(context.Start.Line, ErrorCode.cmpTypeMismatch,
                    "Cannot compare values of different data types");
            }

            switch (context.Operator_condition().GetText())
            {
                case "==": AddOPR(Instruction.EQ); break;
                case "!=": AddOPR(Instruction.NEQ); break;
                case "<": AddOPR(Instruction.LESS); break;
                case "<=": AddOPR(Instruction.LEQ); break;
                case ">": AddOPR(Instruction.GREATER); break;
                case ">=": AddOPR(Instruction.GEQ); break;
            }

            return (int)DataType.Bool;
        }

        public override int VisitCondition_item([NotNull] Condition_itemContext context)
        {
            if (context.Bool() != null)
            {
                AddLIT(BoolToInt(context.Bool().GetText()));
                if (context.Negation() != null)
                {
                    AddNeg();
                }

                return (int)DataType.Bool;
            }

            int ret = Visit(context.expression());

            if (context.Negation() != null)
            {
                if ((DataType)ret != DataType.Bool)
                {
                    return handler.reportError(context.Start.Line, ErrorCode.conditionTypeMismatch,
                        "Expression in condition must be of the Bool type.");
                }

                AddNeg();
            }

            return ret;
        }
        #endregion
        
        #endregion
    }
}