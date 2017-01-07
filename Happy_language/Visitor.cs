using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.Diagnostics;

namespace Happy_language
{ 
    public class Visitor : GrammarBaseVisitor<int>
    {
        #region Attributes
        /// <summary>
        /// Symbol table for global variables and constants
        /// </summary>
        private SymbolTable globalSymbolTable = new SymbolTable();

        /// <summary>
        /// Local symbol table
        /// </summary>
        private SymbolTable localSymbolTable = new SymbolTable();

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
        public static string BoolToInt(string value)
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
            AddRET(0, 0);

            List<FunctionParameter> parameters = new List<FunctionParameter>();
            parameters.Add(new FunctionParameter("value", DataType.Int));
            globalSymbolTable.AddFuncItem(new FuncItem("Abs", DataType.Int, instructionCount - 13, parameters));
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
            AddRET(0, 0);

            List<FunctionParameter> parameters = new List<FunctionParameter>();
            parameters.Add(new FunctionParameter("a", DataType.Int));
            parameters.Add(new FunctionParameter("b", DataType.Int));
            globalSymbolTable.AddFuncItem(new FuncItem("Max", DataType.Int, instructionCount - 14, parameters));
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
            AddRET(0, 0);

            List<FunctionParameter> parameters = new List<FunctionParameter>();
            parameters.Add(new FunctionParameter("a", DataType.Int));
            parameters.Add(new FunctionParameter("b", DataType.Int));
            globalSymbolTable.AddFuncItem(new FuncItem("Min", DataType.Int, instructionCount - 14, parameters));
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
            AddRET(0, 0);

            AddLIT("0");
            AddSTO(1, funcReturnAddress);
            AddINT(-4);
            AddRET(0, 0);

            List<FunctionParameter> parameters = new List<FunctionParameter>();
            parameters.Add(new FunctionParameter("value", DataType.Int));
            globalSymbolTable.AddFuncItem(new FuncItem("IntToBool", DataType.Bool, instructionCount - 10, parameters));
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
            AddRET(0, 0);

            List<FunctionParameter> parameters = new List<FunctionParameter>();
            parameters.Add(new FunctionParameter("value", DataType.Bool));
            globalSymbolTable.AddFuncItem(new FuncItem("BoolToInt", DataType.Int, instructionCount - 4, parameters));
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
            AddRET(0, 0);
            globalSymbolTable.AddFuncItem(new FuncItem("PrintNewLine", DataType.Void, instructionCount - 8, new List<FunctionParameter>()));
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
            AddRET(0, 0);
            List<FunctionParameter> parameters = new List<FunctionParameter>();
            parameters.Add(new FunctionParameter("value", DataType.Int));
            globalSymbolTable.AddFuncItem(new FuncItem("PrintASCII", DataType.Void, instructionCount - 4, parameters));
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
            AddRET(0, 0);

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
            AddRET(0, 0);

            List<FunctionParameter> parameters = new List<FunctionParameter>();
            parameters.Add(new FunctionParameter("value", DataType.Bool));
            globalSymbolTable.AddFuncItem(new FuncItem("PrintBool", DataType.Void, instructionCount - 31, parameters));
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

            AddRET(0, 0);
            List<FunctionParameter> parameters = new List<FunctionParameter>();
            parameters.Add(new FunctionParameter("value", DataType.Int));
            globalSymbolTable.AddFuncItem(new FuncItem("PrintInt", DataType.Void, instructionCount - 31, parameters));
        }
        #endregion

        #region Instruction handling
        /// <summary>
        /// Add WRI instruction to the instruction list
        /// </summary>
        public void AddWRI()
        {
            instructions.Add(new Instruction(InstructionType.WRI, 0, "0"));
            instructionCount += 1;
        }

        /// <summary>
        /// Add PST instruction to the instruction list
        /// </summary>
        public void AddPST()
        {
            instructions.Add(new Instruction(InstructionType.PST, 0, "0"));
            instructionCount += 1;
        }

        /// <summary>
        /// Add PLD instruction to the instruction list
        /// </summary>
        public void AddPLD()
        {
            instructions.Add(new Instruction(InstructionType.PLD, 0, "0"));
            instructionCount += 1;
        }

        /// <summary>
        /// Add LIT instruction to the instruction list
        /// </summary>
        /// <param name="value">string representation of the value to add to stack</param>
        public void AddLIT(string value)
        {
            instructions.Add(new Instruction(InstructionType.LIT, 0, value));
            instructionCount += 1;
        }

        /// <summary>
        /// Add STO instruction to the instruction list
        /// </summary>
        /// <param name="level">Level of the address to store</param>
        /// <param name="address">Address where to store</param>
        public void AddSTO(int level, int address)
        {
            instructions.Add(new Instruction(InstructionType.STO, level, address.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Add OPR instruction to the instruction list
        /// </summary>
        /// <param name="opCode">Operator code</param>
        public void AddOPR(int opCode)
        {
            instructions.Add(new Instruction(InstructionType.OPR, 0, opCode.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Add JMP instruction to the instruction list
        /// </summary>
        /// <param name="codeAddress">Address where to jump</param>
        public void AddJMP(int codeAddress)
        {
            instructions.Add(new Instruction(InstructionType.JMP, 0, codeAddress.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Change the address where to jump of the instruction with given index
        /// </summary>
        /// <param name="codeAddress">Address where to jump</param>
        /// <param name="index">Index of the JMP instruction in the instruction list</param>
        public void ChangeJMP(int codeAddress, int index)
        {
            Debug.Assert(instructions[index].Type == InstructionType.JMP);
            instructions[index].Value = codeAddress.ToString();
        }

        /// <summary>
        /// Add INT instruction to the instruction list
        /// </summary>
        /// <param name="value">Value of how much the top of the stack will be increased (or decreased if the value is negative)</param>
        public void AddINT(int value)
        {
            instructions.Add(new Instruction(InstructionType.INT, 0, value.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Adds CAL instruction to the instruction list
        /// </summary>
        /// <param name="level">Level of the address that will be called</param>
        /// <param name="address">Address that will be called</param>
        public void AddCAL(int level, int address)
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
        public void ChangeCAL(int level, int address, int index)
        {
            Debug.Assert(instructions[index].Type == InstructionType.CAL);

            instructions[index].Level = level;
            instructions[index].Value = address.ToString();
        }

        /// <summary>
        /// Add RET instruction to the instruction list
        /// </summary>
        /// <param name="level">Level of the address where to return</param>
        /// <param name="codeAddress">Address where to return</param>
        public void AddRET(int level, int codeAddress)
        {
            instructions.Add(new Instruction(InstructionType.RET, level, codeAddress.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Add LOD instruction to the instruction list
        /// </summary>
        /// <param name="level">Level of the address from where to load the value</param>
        /// <param name="address">Address from where to load the value</param>
        public void AddLOD(int level, int address)
        {
            instructions.Add(new Instruction(InstructionType.LOD, level, address.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Add JMC instruction to the instruction list
        /// </summary>
        /// <param name="codeAddress">Address where to jump</param>
        public void AddJMC(int codeAddress)
        {
            instructions.Add(new Instruction(InstructionType.JMC, 0, codeAddress.ToString()));
            instructionCount += 1;
        }

        /// <summary>
        /// Change JMC instruction on given address
        /// </summary>
        /// <param name="index">Index of modified instruction</param>
        /// <param name="codeAddress">Address where to jump</param>
        public void ChangeJMC(int index, int codeAddress)
        {
            Debug.Assert(instructions[index].Type == InstructionType.JMC);
            instructions[index].Value = codeAddress.ToString();
        }

        /// <summary>
        /// Adds instructions for logical negation
        /// !val = val == false
        /// </summary>
        public void AddNeg()
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
            AddRET(0, 0);
            jmpToMainDone = true;
        }

        private void changeJMPtoMain()
        {
            AddINT(3);
            ChangeCAL(0, instructionCount - 1, jmpToMainIndex);
        }

        private void endProgram()
        {
            AddRET(0, 0);
        }
        #endregion

        #region Memory handling
        /// <summary>
        /// Creates the constant using data from current parse tree context
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="name">Name of the constant</param>
        /// <returns>Object representing created constant</returns>
        private VarConstItem createConst(GrammarParser.Def_constContext context, string name)
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

            return new VarConstItem(name, VarConstType.Const, dt, context.start.Line, addr, level);
        }

        /// <summary>
        /// Creates the variable using data from current parse tree context
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="name">Name of the variable</param>
        /// <returns>Object representing created variable</returns>
        private VarConstItem createVar(GrammarParser.Def_varContext context, string name)
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

            return new VarConstItem(name, VarConstType.Var, dt, context.start.Line, addr, level);
        }

        /// <summary>
        /// Creates the array using data from current parse tree context
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns>Object representing created array</returns>
        private VarConstItem createArray(GrammarParser.Array_inicializationContext context)
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
                GrammarParser.Number_array_assignContext values = context.number_array_assign();
                while (values != null)
                {
                    length += 1;
                    values = values.number_array_assign();
                }
            }
            else if (context.bool_array_assign() != null)
            {
                GrammarParser.Bool_array_assignContext values = context.bool_array_assign();
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

            return new VarConstItem(name, length, VarConstType.Var, dt, context.start.Line, addr, level); ;
        }

        /// <summary>
        /// Creates function from given parse tree context
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns>Object representing created function</returns>
        private FuncItem createFunction(GrammarParser.Def_functionContext context)
        {
            string name = context.Identifier().GetText(); ;
            DataType returnDataType = DataType.Int;
            List<FunctionParameter> parameters = new List<FunctionParameter>();

            if (context.function_return_data_typ().Data_type_void() != null) returnDataType = DataType.Void;
            else if (context.function_return_data_typ().data_type().Data_type_bool() != null) returnDataType = DataType.Bool;
     
            GrammarParser.ParametersContext paramContext = context.parameters();
            while (paramContext != null && paramContext.Identifier() != null)
            {
                DataType dType = DataType.Int;
				if (paramContext.data_type().Data_type_bool() != null) dType = DataType.Bool;

                parameters.Add(new FunctionParameter(paramContext.Identifier().GetText(), dType));
                paramContext = paramContext.parameters();
            }

            return new FuncItem(name, returnDataType, instructionCount, parameters);
        }

        /// <summary>
        /// Validate the given array and if it is OK insert it in the symbol table
        /// </summary>
        /// <param name="array">Processed array</param>
        /// <returns>Result of validation. Zero if OK, negative value if array is not valid</returns>
        private int processArray(VarConstItem array)
        {
            if (array.GetLength() < 0)
            {
                //error
                handler.reportVisitorError(array.GetDeclarationLine(), ErrorCode.arrayLengthNegative, "Array has negative length");
                return Error.arrayLengthNegative;
            }

            if (inFunction)
            {
                if (!localSymbolTable.ContainsVarConstItem(array.GetName()))
                    localSymbolTable.AddVarConstItem(array);
                else
                {
                    VarConstItem alreadyDeclared = localSymbolTable.GetVarConstItemByName(array.GetName());

                    handler.reportVisitorError(array.GetDeclarationLine(), ErrorCode.arrayAlreadyExists,
                        "Variable with name '" + array.GetName() + "' already declared on line " + alreadyDeclared.GetDeclarationLine());

                    return Error.arrayAlreadyExists;
                }
                inFunctionAddress += array.GetLength();
            }
            else
            {
                if (!globalSymbolTable.ContainsVarConstItem(array.GetName()))
                    globalSymbolTable.AddVarConstItem(array);
                else
                {
                    VarConstItem alreadyDeclared = localSymbolTable.GetVarConstItemByName(array.GetName());

                    handler.reportVisitorError(array.GetDeclarationLine(), ErrorCode.arrayAlreadyExists,
                        "Variable with name '" + array.GetName() + "' already declared on line " + alreadyDeclared.GetDeclarationLine());

                    return Error.arrayAlreadyExists;
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
        private int processVarConst(VarConstItem item)
        {
            if (inFunction)
            {
                if (!localSymbolTable.ContainsVarConstItem(item.GetName()))
                    localSymbolTable.AddVarConstItem(item);
                else
                {
                    //Console.WriteLine("Promena " + item.GetName() + " uz existuje!\n");

                    VarConstItem alreadyDeclared = localSymbolTable.GetVarConstItemByName(item.GetName());

                    handler.reportVisitorError(item.GetDeclarationLine(), ErrorCode.varConstAlreadyExists,
                        "Variable with name '" + item.GetName() + "' already declared on line " + alreadyDeclared.GetDeclarationLine());

                    return Error.varConstAlreadyExists;
                }
                inFunctionAddress += 1;
            }
            else
            {
                if (!globalSymbolTable.ContainsVarConstItem(item.GetName()))
                    globalSymbolTable.AddVarConstItem(item);
                else
                {
                    VarConstItem alreadyDeclared = localSymbolTable.GetVarConstItemByName(item.GetName());

                    handler.reportVisitorError(item.GetDeclarationLine(), ErrorCode.varConstAlreadyExists,
                        "Variable with name '" + item.GetName() + "' already declared on line " + alreadyDeclared.GetDeclarationLine());

                    return Error.varConstAlreadyExists;
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
        public VarConstItem GetVarConst(string varConstName)
        {
            if (localSymbolTable.ContainsVarConstItem(varConstName))
                return localSymbolTable.GetVarConstItemByName(varConstName);
            else if (globalSymbolTable.ContainsVarConstItem(varConstName))
                return globalSymbolTable.GetVarConstItemByName(varConstName);

            return null;
        }
        #endregion

        #region Visitors
        /// <summary>
        /// Constant definition
        /// </summary>
        /// <param name="context"></param>
        /// <returns>((int) DataType) if ok, negative value if semantic error</returns>
        public override int VisitDef_const([NotNull] GrammarParser.Def_constContext context)
        {
            int result = 0;
            GrammarParser.Multiple_assignContext leftSides = context.multiple_assign();
            while (leftSides != null)
            {
                VarConstItem newConst = createConst(context, leftSides.Identifier().GetText());
                result = processVarConst(newConst);

                if (result < 0)
                    return result;
                
                if (context.condition_expression() != null)
                {
                    result = VisitCondition_expression(context.condition_expression());

                    if (result < 0)
                        return result;

                    if (newConst.GetDataType() != (DataType) result)
                    {
                        handler.reportVisitorError(context.start.Line, context.condition_expression().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + newConst.GetDataType());

                        return Error.assignmentMismatch;
                    }
                }
                //else if (context.function_call() != null)
                //{
                //    result = VisitFunction_call(context.function_call());

                //    if (result < 0)
                //        return result;

                //    if (newConst.GetDataType() != (DataType)result)
                //    {
                //        handler.reportVisitorError(context.start.Line, context.function_call().start.Column, ErrorCode.assignmentMismatch,
                //            "Cannot assign from " + ((DataType)result) + " to " + newConst.GetDataType());

                //        return Error.assignmentMismatch;
                //    }
                //}
				else if (context.expression() != null)
				{
					result = VisitExpression(context.expression());

					if (result < 0)
						return result;

					if (newConst.GetDataType() != (DataType)result)
					{
                        handler.reportVisitorError(context.start.Line, context.expression().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + newConst.GetDataType());

                        return Error.assignmentMismatch;
                    }
				}
				else if (context.ternary_operator() != null)
				{
					result = VisitTernary_operator(context.ternary_operator());

					if (result < 0)
						return result;

					if (newConst.GetDataType() != (DataType)result)
					{
                        handler.reportVisitorError(context.start.Line, context.ternary_operator().start.Column, ErrorCode.assignmentMismatch,
                             "Cannot assign from " + ((DataType)result) + " to " + newConst.GetDataType());

                        return Error.assignmentMismatch;
                    }
				}
				
				if (result < 0)
                    return result;

                leftSides = leftSides.multiple_assign();
            }

            return result;
        }

        /// <summary>
        /// Variable definition
        /// </summary>
        /// <param name="context"></param>
        /// <returns>((int) DataType) if ok, negative value if semantic error</returns>
        public override int VisitDef_var([NotNull] GrammarParser.Def_varContext context)
        {
            int result = 0;
            if (context.array_inicialization() == null)
            {
                GrammarParser.Multiple_assignContext leftSides = context.multiple_assign();
                while (leftSides != null)
                {
                    VarConstItem newVar = createVar(context, leftSides.Identifier().GetText());
                    result = processVarConst(newVar);              

                    if (result < 0)
                        return result;

                    if (context.condition_expression() != null)
                    {
                        result = VisitCondition_expression(context.condition_expression());

                        if (result < 0)
                            return result;

                        if (newVar.GetDataType() != (DataType)result)
                        {
                            handler.reportVisitorError(context.start.Line, context.condition_expression().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + newVar.GetDataType());

                            return Error.assignmentMismatch;
                        }
                    }
                    else if (context.function_call() != null)
                    {
                        result = VisitFunction_call(context.function_call());

                        if (result < 0)
                            return result;

                        if (newVar.GetDataType() != (DataType)result)
                        {
                            handler.reportVisitorError(context.start.Line, context.function_call().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + newVar.GetDataType());

                            return Error.assignmentMismatch;
                        }
                    }
                    else if (context.expression() != null)
                    {
						result = VisitExpression(context.expression());

						if (result < 0)
							return result;

						if (newVar.GetDataType() != (DataType)result)
						{
                            handler.reportVisitorError(context.start.Line, context.expression().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + newVar.GetDataType());

                            return Error.assignmentMismatch;
                        }

					}
					else if (context.ternary_operator() != null)
					{
						result = VisitTernary_operator(context.ternary_operator());

						if (result < 0)
							return result;

						if (newVar.GetDataType() != (DataType)result)
						{
                            handler.reportVisitorError(context.start.Line, context.ternary_operator().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + newVar.GetDataType());

                            return Error.assignmentMismatch;
                        }
					}

					if (result < 0)
                        return result;

                    leftSides = leftSides.multiple_assign();
                }
            }
            else
            {
                result = VisitArray_inicialization(context.array_inicialization());
            }

            return result;
        }

        /// <summary>
        /// Array inicialization
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
		public override int VisitArray_inicialization([NotNull] GrammarParser.Array_inicializationContext context)
        {
            VarConstItem newArray = createArray(context);
            int result = processArray(newArray);

            if (result < 0)
                return result;

            if (context.Int() != null)
            {
                for (int i = 0; i < newArray.GetLength(); i++)
                    AddLIT("0");
            }
            else if (context.String() != null)
            {
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
                GrammarParser.Number_array_assignContext values = context.number_array_assign();
                while (values != null)
                {
                    result = VisitExpression(values.expression());

                    if (result < 0)
                    {
                        return result;
                    }
                    else if (newArray.GetDataType() != (DataType) result)
                    {
                        handler.reportVisitorError(values.start.Line, values.expression().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to array item of type " + newArray.GetDataType());

                        return Error.assignmentMismatch;
                    }

                    values = values.number_array_assign();
                }
            }
            else if (context.bool_array_assign() != null)
            {
				GrammarParser.Bool_array_assignContext values = context.bool_array_assign();
				while (values != null)
                {
                    if (values.condition_expression() != null)
                    {
                        result = VisitCondition_expression(values.condition_expression());

                        if (result < 0)
                        {
                            return result;
                        }
                        else if (newArray.GetDataType() != (DataType)result)
                        {
                            handler.reportVisitorError(values.start.Line, values.condition_expression().start.Column, ErrorCode.assignmentMismatch,
                                "Cannot assign from " + ((DataType)result) + " to array item of type " + newArray.GetDataType());

                            return Error.assignmentMismatch;
                        }
                    }
                    else if (values.function_call() != null)
                    {
                        result = VisitFunction_call(values.function_call());

                        if (result < 0)
                        {
                            return result;
                        }
                        else if (newArray.GetDataType() != (DataType)result)
                        {
                            handler.reportVisitorError(values.start.Line, values.function_call().start.Column, ErrorCode.assignmentMismatch,
                                "Cannot assign from " + ((DataType)result) + " to array item of type " + newArray.GetDataType());

                            return Error.assignmentMismatch;
                        }
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
        public override int VisitDef_function([NotNull] GrammarParser.Def_functionContext context)
        {
            inFunction = true;
            globalVarsDefined = true;
            localSymbolTable = new SymbolTable();
            if (!jmpToMainDone) DoMainJmp(0);

            FuncItem newItem = createFunction(context);
            if (!globalSymbolTable.ContainsFuncItem(newItem.GetName()))
            {
                globalSymbolTable.AddFuncItem(newItem);
            }
            else
            {
                handler.reportVisitorError(context.Start.Line, ErrorCode.functionAlreadyExists,
                    "Function with name '" + newItem.GetName() + "' already defined.");
                return Error.functionAlreadyExists;
            }

            AddINT(3 + newItem.GetParameters().Count);

            level += 1;
            inFunctionAddress = 3;
            for (int i = 0; i < newItem.GetParameters().Count; i++)
            {
                VarConstItem parItem = new VarConstItem(newItem.GetParameters()[i].getName(),
                                                        VarConstType.Var, newItem.GetParameters()[i].getDataType(), context.start.Line, inFunctionAddress, level);
                localSymbolTable.AddVarConstItem(parItem);
                inFunctionAddress += 1;
            }

            int result = base.VisitDef_function(context);
            level -= 1;

            AddRET(0, 0);
            inFunction = false;
            localSymbolTable = null;

            return result;
        }

        /// <summary>
        /// Return command
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override int VisitFunction_return([NotNull] GrammarParser.Function_returnContext context)
        {
            int result = 0;
			if (context.condition_expression() == null && context.expression() == null && context.ternary_operator() == null)
            {
				return result;
            }

            if (context.expression() != null)
            {
                result = VisitExpression(context.expression());
            }
			else if (context.condition_expression() != null)
			{
				result = VisitCondition_expression(context.condition_expression());
			}
			else if (context.ternary_operator() != null)
			{
				result = VisitTernary_operator(context.ternary_operator());
			}

            //Store the result on predefined address
			AddSTO(level, funcReturnAddress);

            return result;
        }

        public override int VisitAssignment([NotNull] GrammarParser.AssignmentContext context)
        {
            int result = 0;
            GrammarParser.Multiple_assignContext leftSides = context.multiple_assign();
            while (leftSides != null)
            {
                string retValToName = leftSides.Identifier().GetText();
                VarConstItem retValTo = GetVarConst(retValToName);

                if (retValTo == null)
                {
                    handler.reportVisitorError(context.start.Line, ErrorCode.unknownSymbol, "Unknown variable with name '" + retValToName + "'");
					return Error.unknownSymbol;
                }

                if (retValTo.GetVarConstType() == VarConstType.Const)
                {
                    handler.reportVisitorError(context.start.Line, ErrorCode.assignmentToConstant, "Cannot assign to a constant");
                    return Error.assignmentToConstant;
                }

                if (context.condition_expression() != null)
                {
                    result = VisitCondition_expression(context.condition_expression());

                    if (result < 0)
                        return result;

                    if (retValTo.GetDataType() != (DataType)result)
                    {
                        handler.reportVisitorError(context.start.Line, context.condition_expression().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + retValTo.GetDataType());

                        return Error.assignmentMismatch;
                    }
                }
                else if (context.expression() != null)
                {
                    result = VisitExpression(context.expression());

                    if (result < 0)
                        return result;

                    if (retValTo.GetDataType() != (DataType)result)
                    {
                        handler.reportVisitorError(context.start.Line, context.expression().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + retValTo.GetDataType());

                        return Error.assignmentMismatch;
                    }
                }
                else if (context.condition() != null)
                {
                    result = VisitCondition(context.condition());

                    if (result < 0)
                        return result;

                    if (retValTo.GetDataType() != (DataType)result)
                    {
                        handler.reportVisitorError(context.start.Line, context.condition().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + retValTo.GetDataType());

                        return Error.assignmentMismatch;
                    }
				}
				else if (context.ternary_operator() != null)
				{
					result = VisitTernary_operator(context.ternary_operator());

					if (result < 0)
						return result;

					if (retValTo.GetDataType() != (DataType)result)
					{
                        handler.reportVisitorError(context.start.Line, context.ternary_operator().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + retValTo.GetDataType());

                        return Error.assignmentMismatch;
                    }
				}

				int varLevel = retValTo.GetLevel();
                int varAddress = retValTo.GetAddress();
                int levelToMove = Math.Abs(level - varLevel);
                AddSTO(levelToMove, varAddress);

                if (result < 0)
                    return result;

                leftSides = leftSides.multiple_assign();
            }

            return result;
        }

        public override int VisitOne_assignment([NotNull] GrammarParser.One_assignmentContext context)
        {
			int result = 0;
			string retValToName = context.Identifier().GetText();
            VarConstItem retValTo = null;

            if (localSymbolTable.ContainsVarConstItem(retValToName)) retValTo = localSymbolTable.GetVarConstItemByName(retValToName);
            else if (globalSymbolTable.ContainsVarConstItem(retValToName)) retValTo = globalSymbolTable.GetVarConstItemByName(retValToName);
            else
            {
                handler.reportVisitorError(context.start.Line, ErrorCode.unknownSymbol, "Unknown variable with name '" + retValToName + "'");
                return Error.unknownSymbol;
            }

            if (retValTo.GetVarConstType() == VarConstType.Const)
            {
                handler.reportVisitorError(context.start.Line, ErrorCode.assignmentToConstant, "Cannot assign to a constant");
                return Error.assignmentToConstant;
            }

            if (context.condition_expression() != null)
            {
                result = VisitCondition_expression(context.condition_expression());

                if (result < 0)
                    return result;

                if (retValTo.GetDataType() != (DataType)result)
                {
                    handler.reportVisitorError(context.start.Line, context.condition_expression().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + retValTo.GetDataType());

                    return Error.assignmentMismatch;
                }
            }
			else if (context.expression() != null)
			{
				result = VisitExpression(context.expression());

				if (result < 0)
					return result;

				if (retValTo.GetDataType() != (DataType)result)
				{
                    handler.reportVisitorError(context.start.Line, context.expression().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + retValTo.GetDataType());

                    return Error.assignmentMismatch;
                }
			}

			else if (context.ternary_operator() != null)
			{
				result = VisitTernary_operator(context.ternary_operator());

				if (result < 0)
					return result;

				if (retValTo.GetDataType() != (DataType)result)
				{
                    handler.reportVisitorError(context.start.Line, context.ternary_operator().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + retValTo.GetDataType());

                    return Error.assignmentMismatch;
                }
			}
			else if (context.condition() != null)
            {
                result = VisitCondition(context.condition());

                if (result < 0)
                    return result;

                if (retValTo.GetDataType() != (DataType)result)
                {
                    handler.reportVisitorError(context.start.Line, context.condition().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to " + retValTo.GetDataType());

                    return Error.assignmentMismatch;
                }
            }

            int varLevel = retValTo.GetLevel();
            int varAddress = retValTo.GetAddress();
            int levelToMove = Math.Abs(level - varLevel);
            AddSTO(levelToMove, varAddress);

            retValTo = null;

            return result;
        }

        public override int VisitMain([NotNull] GrammarParser.MainContext context)
        {
            globalVarsDefined = true;
            if (!jmpToMainDone) DoMainJmp(0);
            inFunction = true;
            inFunctionAddress = 3;
            level += 1;
            localSymbolTable = new SymbolTable();

            changeJMPtoMain();
            int result = base.VisitMain(context);
            endProgram();

            inFunction = false;
            localSymbolTable = null;
            level -= 1;
            return result;
        }

        public override int VisitFunction_call([NotNull] GrammarParser.Function_callContext context)
        {
            string fName = context.Identifier().GetText();

            FuncItem calledFce = null;
            if (globalSymbolTable.ContainsFuncItem(fName)) calledFce = globalSymbolTable.GetFuncItemByName(fName);
            else
            {
                handler.reportVisitorError(context.start.Line, ErrorCode.functionDoesNotExist, "Uknown function with name '" + fName + "'");
                return Error.functionDoesNotExist;
            }

            AddINT(3);
            List<VarConstItem> usedParameters = new List<VarConstItem>();
            GrammarParser.Par_in_functionContext paramContext = context.par_in_function();
            while (paramContext != null)
            {
                VarConstItem par = null;

                if (paramContext.expression() != null)
                {
                    int result = VisitExpression(paramContext.expression());
                    par = new VarConstItem("", VarConstType.Var, (DataType) result, context.start.Line, 0, 0);
                }
                else if (paramContext.condition_expression() != null)
                {
                    VisitCondition_expression(paramContext.condition_expression());
                    par = new VarConstItem("", VarConstType.Var, DataType.Bool, context.start.Line, 0, 0);
                }

                if (par != null)
                {
                    usedParameters.Add(par);
                }
                paramContext = paramContext.par_in_function();
            }

            List<FunctionParameter> requestedParameters = calledFce.GetParameters();
            if (requestedParameters.Count != usedParameters.Count)
            {
                handler.reportVisitorError(context.Start.Line, context.par_in_function().Stop.Column, ErrorCode.functionWrongParametersCount,
                    "Wrong parameter count, expected: " + requestedParameters.Count);

                return Error.functionWrongParametersCount;
            }
            for (int i = 0; i < requestedParameters.Count; i++)
            {
                if (requestedParameters[i].getDataType() != usedParameters[i].GetDataType())
                {
                    handler.reportVisitorError(context.Start.Line, ErrorCode.functionParameterDataTypeMismatch,
                        "Type mismatch in parameter number " + (i + 1));

                    return Error.functionParameterDataTypeMismatch;
                }
            }

            AddINT(-1 * (3 + usedParameters.Count()));
            AddCAL(globalVarsDefined ? 1 : 0, calledFce.GetAddress());

            if (calledFce.GetReturnDataType() != DataType.Void)
                AddLOD(level, funcReturnAddress);

            return (int) calledFce.GetReturnDataType();
        }

        public override int VisitTernary_operator([NotNull] GrammarParser.Ternary_operatorContext context)
        {
            Visit(context.condition());
            int jmcAddress = instructionCount;
            AddJMC(0);

            int ret1 = Visit(context.expression()[0]);
            int jmpAddress = instructionCount;
            AddJMP(0);
            ChangeJMC(jmcAddress, instructionCount);
            int ret2 = Visit(context.expression()[1]);
            ChangeJMP(instructionCount, jmpAddress);

            if (ret1 < 0)
            {
                return ret1;
            }


            if (ret2 < 0)
            {
                return ret2;
            }

            if (ret1 != ret2)
            {
                handler.reportVisitorError(context.start.Line, context.start.Column, ErrorCode.subExpressionMismatch,
                    "Subexpressions of ternary operator must have same data type");

                return Error.subExpressionMismatch;
            }

            return ret1;
        }

        public override int VisitExpression([NotNull] GrammarParser.ExpressionContext context)
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

            if (context.Add() != null)
            {
                if (ret1 != ret2)
                {
                    //Console.WriteLine("Expression - Add: Type mismatch");
                    handler.reportVisitorError(context.Start.Line, ErrorCode.assignmentMismatch, "Canot perform '+' operation on conflicting data types");
                    return Error.assignmentMismatch;
                }

                if ((DataType) ret1 == DataType.Bool)
                {
                    handler.reportVisitorError(context.Start.Line, ErrorCode.operatorTypeMismatch, "Cannot perform '+' operation on Bool type");

                    return Error.operatorTypeMismatch;
                }

                AddOPR(Instruction.ADD);

            }
            else if (context.Sub() != null)
            {
                if (ret1 != ret2)
                {
                    handler.reportVisitorError(context.Start.Line, ErrorCode.assignmentMismatch, "Canot perform '-' operation on conflicting data types");
                    return Error.assignmentMismatch;
                }

                if ((DataType)ret1 == DataType.Bool)
                {
                    handler.reportVisitorError(context.Start.Line, ErrorCode.operatorTypeMismatch, "Cannot perform '-' operation on Bool type");

                    return Error.operatorTypeMismatch;
                }

                AddOPR(Instruction.SUB);
            }

            return ret2;
        }

        public override int VisitExpression_multiply([NotNull] GrammarParser.Expression_multiplyContext context)
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

            if (context.Mul() != null)
            {
                if (ret1 != ret2)
                {
                    handler.reportVisitorError(context.Start.Line, ErrorCode.assignmentMismatch, "Canot perform '*' operation on conflicting data types");
                    return Error.assignmentMismatch;
                }

                if ((DataType)ret1 == DataType.Bool)
                {
                    handler.reportVisitorError(context.Start.Line, ErrorCode.operatorTypeMismatch, "Cannot perform '*' operation on Bool type");

                    return Error.operatorTypeMismatch;
                }

                AddOPR(Instruction.MUL);
            }
            else if (context.Div() != null)
            {
                if (ret1 != ret2)
                {
                    handler.reportVisitorError(context.Start.Line, ErrorCode.assignmentMismatch, "Canot perform '/' operation on conflicting data types");
                    return Error.assignmentMismatch;
                }

                if ((DataType)ret1 == DataType.Bool)
                {
                    handler.reportVisitorError(context.Start.Line, ErrorCode.operatorTypeMismatch, "Cannot perform '/' operation on Bool type");

                    return Error.operatorTypeMismatch;
                }

                AddOPR(Instruction.DIV);
            }

            return ret2;
        }

        public override int VisitExpression_item([NotNull] GrammarParser.Expression_itemContext context)
        {
            int result = 0;
            if (context.Identifier() != null)
            {
                string varConstName = context.Identifier().GetText();
                VarConstItem varConst = GetVarConst(varConstName);
                if (varConst == null)
                {
                    handler.reportVisitorError(context.Start.Line, ErrorCode.unknownSymbol, "Unknown variable '" + varConstName + "'");

                    return Error.unknownSymbol;
                }

                int varLevel = varConst.GetLevel();
                int levelToMove = Math.Abs(level - varLevel);
                AddLOD(levelToMove, varConst.GetAddress());

				if (context.Sub() != null)
				{
					AddOPR(Instruction.UNARY_MINUS);
				}

				result = (int) varConst.GetDataType();
            }
            else if (context.array_index() != null)
            {


                VarConstItem array = GetVarConst(context.array_index().Identifier().GetText());
                if (array == null)
                {
                    handler.reportVisitorError(context.Start.Line, ErrorCode.arrayDoesNotExist, "Unknown array '" + context.array_index().Identifier().GetText() + "'");

                    return Error.arrayDoesNotExist;
                }

                GrammarParser.IndexContext indexContext = context.array_index().index();

                int varLevel = array.GetLevel();
                int varAddress = array.GetAddress();
                int levelToMove = Math.Abs(level - varLevel);

                if (indexContext.Int() != null)
                {
                    int index = Convert.ToInt32(indexContext.Int().GetText());

                    if (index < 0)
                    {
                        handler.reportVisitorError(context.Start.Line, ErrorCode.arrayIndexNegative, "Negative index");

                        return Error.arrayIndexNegative;
                    }
                    if (index >= array.GetLength())
                    {
                        handler.reportVisitorError(context.Start.Line, ErrorCode.arrayOutOfBounds, "Index out of bounds");

                        return Error.arrayOutOfBounds;
                    }

                    AddLOD(levelToMove, varAddress + index);
                }
                else if (indexContext.expression() != null)
                {
                    AddLIT(levelToMove.ToString());

                    AddLIT(varAddress.ToString());

                    result = VisitExpression(indexContext.expression());

                    if (result < 0)
                    {
                        return result;
                    }

                    if (DataType.Int != ((DataType)result))
                    {
                        handler.reportVisitorError(context.Start.Line, indexContext.expression().start.Column, ErrorCode.indexTypeMismatch,
                                "Index must be of type Int");

                        return Error.indexTypeMismatch;
                    }


                    AddOPR(Instruction.ADD);
                    AddPLD();
                }



                result = (int) array.GetDataType();
            }
            else if (context.function_call() != null)
            {
                result = VisitFunction_call(context.function_call());

				if (context.Sub() != null)
				{
					AddOPR(Instruction.UNARY_MINUS);
				}

				if (result < 0)
                {
                    return result;
                }
            }
            else if (context.expression() != null)
            {
                result = VisitExpression(context.expression());

                if (result < 0)
                {
                    return result;
                }
            }
            else
            {
                AddLIT(context.Int().GetText());
				if (context.Sub() != null)
				{
					AddOPR(Instruction.UNARY_MINUS);
				}

				result = (int) DataType.Int;
            }

            return result;
        }

        public override int VisitAssignment_array([NotNull] GrammarParser.Assignment_arrayContext context)
        {
            string arrayName = context.array_index().Identifier().GetText();
            VarConstItem leftSide = GetVarConst(arrayName);

            if (leftSide == null)
            { 
                handler.reportVisitorError(context.Start.Line, ErrorCode.arrayDoesNotExist, "Unknown array '" + arrayName + "'");

                return Error.arrayDoesNotExist;
            }

            int result = 0;
            if (context.expression() != null)
            {
                result = VisitExpression(context.expression());

                if (result < 0)
                {
                    return result;
                }

                if (leftSide.GetDataType() != ((DataType) result))
                {
                    handler.reportVisitorError(context.Start.Line, context.expression().start.Column, ErrorCode.assignmentMismatch,
                            "Cannot assign from " + ((DataType)result) + " to array item of type " + leftSide.GetDataType());

                    return Error.assignmentMismatch;
                }
            }
            else if (context.condition_expression() != null)
            {
                result = VisitCondition_expression(context.condition_expression());

                if (result < 0)
                {
                    return result;
                }

                if (leftSide.GetDataType() != ((DataType)result))
                {
                    handler.reportVisitorError(context.start.Line, context.condition_expression().start.Column, ErrorCode.assignmentMismatch,
                                "Cannot assign from " + ((DataType)result) + " to array item of type " + leftSide.GetDataType());

                    return Error.assignmentMismatch;
                }
			}
			else if (context.ternary_operator() != null)
			{
				result = VisitTernary_operator(context.ternary_operator());

				if (result < 0)
					return result;

				if (leftSide.GetDataType() != (DataType)result)
				{
                    handler.reportVisitorError(context.start.Line, context.ternary_operator().start.Column, ErrorCode.assignmentMismatch,
                                "Cannot assign from " + ((DataType)result) + " to array item of type " + leftSide.GetDataType());

                    return Error.assignmentMismatch;
                }
			}

            GrammarParser.IndexContext indexContext = context.array_index().index();

            int varLevel = leftSide.GetLevel();
            int levelToMove = Math.Abs(level - varLevel);

            if (indexContext.Int() != null)
            {
                int index = Convert.ToInt32(indexContext.Int().GetText());
                if (index < 0)
                {
                    handler.reportVisitorError(context.Start.Line, ErrorCode.arrayIndexNegative, "Negative index");

                    return Error.arrayIndexNegative;
                }

                if (index >= leftSide.GetLength())
                {
                    handler.reportVisitorError(context.Start.Line, ErrorCode.arrayOutOfBounds, "Index out of bounds");

                    return Error.arrayOutOfBounds;
                }

                AddSTO(levelToMove, leftSide.GetAddress() + index);
            }
            else if (indexContext.expression() != null)
            {
                AddLIT(levelToMove.ToString());

                AddLIT(leftSide.GetAddress().ToString());

                result = VisitExpression(indexContext.expression());

                if (result < 0)
                {
                    return result;
                }

                if (DataType.Int != ((DataType)result))
                {
                    handler.reportVisitorError(context.Start.Line, indexContext.expression().start.Column, ErrorCode.indexTypeMismatch,
                            "Index must be of type Int");

                    return Error.indexTypeMismatch;
                }

                AddOPR(Instruction.ADD);
                AddPST();
            }

            return result;
        }

        public override int VisitIf([NotNull] GrammarParser.IfContext context)
        {
            Visit(context.condition());
            int jmcAddress = instructionCount;
            AddJMC(0);
            Visit(context.blok());
            int jmpAddress = instructionCount;
            AddJMP(0);
            ChangeJMC(jmcAddress, instructionCount);
            Visit(context.else_if());
            ChangeJMP(instructionCount, jmpAddress);
            return 10;
        }

        public override int VisitElse_if([NotNull] GrammarParser.Else_ifContext context)
        {
            if (context.If() != null)
            {
				Visit(context.condition());
				int jmcAddress = instructionCount;
				AddJMC(0);
				Visit(context.blok());
				int jmpAddress = instructionCount;
				AddJMP(0);
				ChangeJMC(jmcAddress, instructionCount);
				Visit(context.else_if());
				ChangeJMP(instructionCount, jmpAddress);
				return 11;
			}
            else
            {
                Visit(context.@else());
                return 12;
            }
        }

        public override int VisitElse([NotNull] GrammarParser.ElseContext context)
        {
            if (context.blok() != null)
            {
                Visit(context.blok());
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public override int VisitWhile([NotNull] GrammarParser.WhileContext context)
        {
            int conditionAddress = instructionCount;
			Visit(context.condition());

			int jmcAddress = instructionCount;
            AddJMC(0);

            Visit(context.blok());
            AddJMP(conditionAddress);

            ChangeJMC(jmcAddress, instructionCount);

            return 11;
        }

        public override int VisitDo_while([NotNull] GrammarParser.Do_whileContext context)
        {

            int firstAddress = instructionCount;
            Visit(context.blok());
            Visit(context.condition());

            //Value must be negated to jump when not true
            AddNeg();

            AddJMC(firstAddress);

            return 0;
        }

        public override int VisitFor([NotNull] GrammarParser.ForContext context)
        {
            GrammarParser.For_conditionContext forCondition = context.for_condition();

            if (forCondition.one_assignment() != null)
            {
                Visit(forCondition.one_assignment());
            }

            int conditionAddress = instructionCount;
            int jmcAddress = 0;
            
            if (forCondition.condition() != null)
            {
                Visit(forCondition.condition());
                jmcAddress = instructionCount;
                AddJMC(0);
            }

            Visit(context.blok());

            Visit(forCondition.increment());

            AddJMP(conditionAddress);

            ChangeJMC(jmcAddress, instructionCount);

            return 0;
        }

        public override int VisitIncrement([NotNull] GrammarParser.IncrementContext context)
        {
            if (context.one_assignment() != null)
            {
                return Visit(context.one_assignment());
            }

            return 0;
        }

        public override int VisitCondition([NotNull] GrammarParser.ConditionContext context)
        {
            if (context.condition_expression() != null)
            {
                Visit(context.condition_expression());
            }

            if (context.expression() != null)
            {
                int ret = Visit(context.expression());

                if ((DataType) ret != DataType.Bool)
                {
                    handler.reportVisitorError(context.Start.Line, context.expression().Start.Column, ErrorCode.conditionTypeMismatch, "Expression in condition must be of the Bool type");
                }

                if (context.Negation() != null)
                {
                    AddNeg();
                }
            }

            GrammarParser.ConditionContext condition1 = context.GetChild<GrammarParser.ConditionContext>(0);
            if (condition1 != null)
            {
                Visit(condition1);

                if (context.Negation() != null)
                {
                    AddNeg();
                }
            }

            GrammarParser.ConditionContext condition2 = context.GetChild<GrammarParser.ConditionContext>(1);
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

            return 0;
        }

        public override int VisitCondition_expression([NotNull] GrammarParser.Condition_expressionContext context)
        {
            if (context.Bool() != null)
            {
                AddLIT(BoolToInt(context.Bool().GetText()));

                return (int) DataType.Bool;
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
                handler.reportVisitorError(context.Start.Line, ErrorCode.cmpTypeMismatch, "Cannot compare values of different data types");

                return Error.cmpTypeMismatch;
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

            return (int) DataType.Bool;
        }

        public override int VisitCondition_item([NotNull] GrammarParser.Condition_itemContext context)
        {
            if (context.Bool() != null)
            {
                AddLIT(BoolToInt(context.Bool().GetText()));
                if (context.Negation() != null)
                {
                    AddNeg();
                }

                return (int) DataType.Bool;
            }

            int ret = Visit(context.expression());

            if (context.Negation() != null)
            {
                if ((DataType) ret != DataType.Bool)
                {
                    handler.reportVisitorError(context.Start.Line, ErrorCode.conditionTypeMismatch, "");
                }

                AddNeg();
            }

            return ret;
        }
    }
    #endregion
}
