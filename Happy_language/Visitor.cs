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
    // http://elemarjr.com/en/2016/04/21/learning-antlr4-part-1-quick-overview/
    public class Visitor : GrammarBaseVisitor<int>
    {
        #region Attributes
        /// <summary>
        /// Tabulka symbolů pro globalni proměnný a konstanty
        /// </summary>
        private SymbolTable globalSymbolTable = new SymbolTable();

        /// <summary>
        /// Tabulka symbolů pro proměnný ( i parametry) funkcí a mainu
        /// </summary>
        private SymbolTable localSymbolTable = null;

        private List<String> errors = new List<String>();

        /// <summary>
        /// Seznam vygenerovaných instrukcí
        /// </summary>
        private List<Instruction> instructions = new List<Instruction>();

        private Boolean jmpToMainDone = false;

        /// <summary>
        /// Index na které pozici v seznamu instrukcí je instrukce skoku na první instrukci mainu
        /// </summary>
        private int jmpToMainIndex = 0;

        /// <summary>
        /// Indikace jestli se prochazi funkce
        /// </summary>
        private Boolean inFunction = false;
        /// <summary>
        /// Adresa která se používá pro adresování proměnných ve funkci a v mainu
        /// </summary>
        private int inFunctionAddress = 3;

        /// <summary>
        /// Počet instrukcí, používá se i  např. jako adresa pro funkce
        /// </summary>
        private int instructionCount = 0;

        private int globalAddress = 4;  // o jedna vetsi nez je funcReturnAddress
        private int level = 0;
        private int funcReturnAddress = 3;

        /// <summary>
        /// Proměnná na levé straně přiřazení
        /// </summary>
        //private VarConstItem retValTo = null;

        #endregion

        #region Library Functions
        public void PrepareLibraryFunctions()
        {
            AddJMP(0);
            PreparePrintASCIIFunction();
            PreparePrintIntFunction();
            PreparePrintNewLineFunction();
            PreparePrintBoolFunction();
            PrepareBoolToIntFunction();
            PrepareIntToBoolFunction();
            ChangeJMP(instructionCount, 0);
        }

        public void PrepareIntToBoolFunction()
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

        public void PrepareBoolToIntFunction()
        {
            AddINT(4);
            AddSTO(1, funcReturnAddress);
            AddINT(-3);
            AddRET(0, 0);

            List<FunctionParameter> parameters = new List<FunctionParameter>();
            parameters.Add(new FunctionParameter("value", DataType.Bool));
            globalSymbolTable.AddFuncItem(new FuncItem("BoolToInt", DataType.Int, instructionCount - 4, parameters));
        }

        public void PreparePrintNewLineFunction()
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

        public void PreparePrintASCIIFunction()
        {
            AddINT(4);
            AddWRI();
            AddINT(-4);
            AddRET(0, 0);
            List<FunctionParameter> parameters = new List<FunctionParameter>();
            parameters.Add(new FunctionParameter("value", DataType.Int));
            globalSymbolTable.AddFuncItem(new FuncItem("PrintASCII", DataType.Void, instructionCount - 4, parameters));
        }

        public void PreparePrintBoolFunction()
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

        public void PreparePrintIntFunction()
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

        public List<String> GetErrors()
        {
            return errors;
        }

        public List<Instruction> GetInstructions()
        {
            return this.instructions;
        }

        public SymbolTable GetSymbolTable()
        {
            return this.globalSymbolTable;
        }

        public void numberInstructions()
        {
            int c = 0;

            for (int i = 0; i < instructions.Count; i++)
            {
                instructions[i].Number = i;
            }
        }

        #region Instruction handling
        public void AddWRI()
        {
            instructions.Add(new Instruction(InstructionType.WRI, 0, "0"));
            instructionCount += 1;
        }

        public void AddLIT(String value)
        {
            instructions.Add(new Instruction(InstructionType.LIT, 0, value));
            instructionCount += 1;
        }

        public void AddSTO(int level, int address)
        {
            instructions.Add(new Instruction(InstructionType.STO, level, address.ToString()));
            instructionCount += 1;
        }

        public void AddOPR(int opCode)
        {
            instructions.Add(new Instruction(InstructionType.OPR, 0, opCode.ToString()));
            instructionCount += 1;
        }

        public void AddJMP(int codeAddress)
        {
            instructions.Add(new Instruction(InstructionType.JMP, 0, codeAddress.ToString()));
            instructionCount += 1;
        }

        public void ChangeJMP(int codeAddress, int index)
        {
            Debug.Assert(instructions[index].Type == InstructionType.JMP);
            instructions[index].Value = codeAddress.ToString();
        }

        public void AddINT(int value)
        {
            instructions.Add(new Instruction(InstructionType.INT, 0, value.ToString()));
            instructionCount += 1;
        }

        public void AddCAL(int level, int address)
        {
            instructions.Add(new Instruction(InstructionType.CAL, level, address.ToString()));
            instructionCount += 1;
        }

        public void ChangeCAL(int level, int address, int index)
        {
            Debug.Assert(instructions[index].Type == InstructionType.CAL);

            instructions[index].Level = level;
            instructions[index].Value = address.ToString();
        }

        public void AddRET(int level, int codeAddress)
        {
            instructions.Add(new Instruction(InstructionType.RET, level, codeAddress.ToString()));
            instructionCount += 1;
        }

        public void AddLOD(int level, int address)
        {
            instructions.Add(new Instruction(InstructionType.LOD, level, address.ToString()));
            instructionCount += 1;
        }

        public void AddJMC(int codeAddress)
        {
            instructions.Add(new Instruction(InstructionType.JMC, 0, codeAddress.ToString()));
            instructionCount += 1;
        }

        public void ChangeJMC(int index, int codeAddress)
        {
            Debug.Assert(instructions[index].Type == InstructionType.JMC);
            instructions[index].Value = codeAddress.ToString();
        }

        public void AddDebug(string msg)
        {
            instructions.Add(new Instruction(InstructionType.DEBUG, 0, msg));
            instructionCount += 1;
        }

        public void AddNeg()
        {
            AddLIT("0");
            AddOPR(Instruction.EQ);
        }

        public void DoInitialJmp()
        {
            AddJMP(instructionCount + 1);
            AddINT(4);
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
        private VarConstItem createConst(GrammarParser.Def_constContext context, String name)
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

            return new VarConstItem(name, VarConstType.Const, dt, addr, level);
        }

        private VarConstItem createVar(GrammarParser.Def_varContext context, String name)
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

            return new VarConstItem(name, VarConstType.Var, dt, addr, level);
        }

        private VarConstItem createArray(GrammarParser.Array_inicializationContext context)
        {
            String name = context.Identifier().GetText();
            DataType dt = DataType.Int;

            int length = 0;
            if (context.Int() != null)
            {
                length = Convert.ToInt32(context.Int().GetText());
            }
            else if (context.String() != null)
            {
                length = context.String().GetText().Length;
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

            return new VarConstItem(name, length, VarConstType.Var, dt, addr, level); ;
        }

        private void DoMainJmp(int dest)
        {
            AddCAL(0, dest);
            jmpToMainIndex = instructionCount - 1;
            AddRET(0, 0);
            //initialJmpDone = true;
            jmpToMainDone = true;
        }

        private FuncItem createFunction(GrammarParser.Def_one_functionContext context)
        {
            String name = context.Identifier().GetText(); ;
            DataType returnDataType = DataType.Int;
            List<FunctionParameter> parameters = new List<FunctionParameter>();

            if (context.function_return_data_typ().Data_type_void() != null) returnDataType = DataType.Void;
            else if (context.function_return_data_typ().data_type().Data_type_bool() != null) returnDataType = DataType.Bool;
            else if (context.function_return_data_typ().data_type().Data_type_double() != null) returnDataType = DataType.Double;

            GrammarParser.ParametersContext paramContext = context.parameters();
            while (paramContext != null && paramContext.Identifier() != null)
            {
                DataType dType = DataType.Int;
                if (paramContext.data_type().Data_type_double() != null) dType = DataType.Double;
                else if (paramContext.data_type().Data_type_bool() != null) dType = DataType.Bool;

                parameters.Add(new FunctionParameter(paramContext.Identifier().GetText(), dType));
                paramContext = paramContext.parameters();
            }

            return new FuncItem(name, returnDataType, instructionCount, parameters);
        }

        private int processArray(VarConstItem array)
        {
            if (array.GetLength() < 0)
            {
                //error
                Console.WriteLine("Pole " + array.GetName() + " ma zapornou delku.");
                return Error.arrayLengthNegative;
            }

            if (inFunction)
            {
                if (!localSymbolTable.ContainsVarConstItem(array.GetName()))
                    localSymbolTable.AddVarConstItem(array);
                else
                {
                    Console.WriteLine("Pole " + array.GetName() + " uz existuje!\n");
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
                    Console.WriteLine("Pole " + array.GetName() + " uz existuje!\n");
                    return Error.arrayAlreadyExists;
                }
                globalAddress += array.GetLength();
            }

            //AddINT(array.GetLength());

            return 0;
        }

        private int processVarConst(VarConstItem item)
        {
            if (inFunction)
            {
                if (!localSymbolTable.ContainsVarConstItem(item.GetName()))
                    localSymbolTable.AddVarConstItem(item);
                else
                {
                    Console.WriteLine("Promena " + item.GetName() + " uz existuje!\n");
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
                    Console.WriteLine("Promena " + item.GetName() + " uz existuje!\n");
                    return Error.varConstAlreadyExists;
                }
                globalAddress += 1;
            }

            return 0;
        }

        public VarConstItem GetVarConst(String varConstName)
        {
            if (localSymbolTable.ContainsVarConstItem(varConstName))
                return localSymbolTable.GetVarConstItemByName(varConstName);
            else if (globalSymbolTable.ContainsVarConstItem(varConstName))
                return globalSymbolTable.GetVarConstItemByName(varConstName);

            return null;
        }

        public Boolean isExpressionFunctionCall(GrammarParser.ExpressionContext context)
        {
            return true;
        }
        #endregion

        #region Visitors
        public override int VisitStart([NotNull] GrammarParser.StartContext context)
        {
            return base.VisitStart(context);
        }

        public override int VisitDef_con_var([NotNull] GrammarParser.Def_con_varContext context)
        {
            return base.VisitDef_con_var(context);
        }

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

                //retValTo = newConst;
                if (localSymbolTable == null)
                    localSymbolTable = new SymbolTable();

                if (context.condition_expression() != null)
                {
                    result = VisitCondition_expression(context.condition_expression());

                    if (result < 0)
                        return result;

                    if (newConst.GetDataType() != (DataType) result)
                    {
                        Console.WriteLine("Type mismatch.");
                        return Error.assignmentMismatch;
                    }
                }
                else if (context.function_call() != null)
                {
                    result = VisitFunction_call(context.function_call());

                    if (result < 0)
                        return result;

                    if (newConst.GetDataType() != (DataType)result)
                    {
                        Console.WriteLine("Type mismatch.");
                        return Error.assignmentMismatch;
                    }
                    //AddLOD(level, funcReturnAddress);
                }
                else if (context.expression() != null)
                {
                    result = VisitExpression(context.expression());

                    if (result < 0)
                        return result;

                    if (newConst.GetDataType() != (DataType)result)
                    {
                        Console.WriteLine("Type mismatch.");
                        return Error.assignmentMismatch;
                    }
                }
                //retValTo = null;
                if (result < 0)
                    return result;

                leftSides = leftSides.multiple_assign();
            }

            return result;
        }

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

                    //retValTo = newVar;
                    if (localSymbolTable == null)
                        localSymbolTable = new SymbolTable();
                    if (context.condition_expression() != null)
                    {
                        result = VisitCondition_expression(context.condition_expression());

                        if (result < 0)
                            return result;

                        if (newVar.GetDataType() != (DataType)result)
                        {
                            Console.WriteLine("Type mismatch.");
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
                            Console.WriteLine("Type mismatch.");
                            return Error.assignmentMismatch;
                        }
                        //AddLOD(level, inFunctionAddress);
                    }
                    else if (context.expression() != null)
                    {
                        result = VisitExpression(context.expression());

                        if (result < 0)
                            return result;

                        if (newVar.GetDataType() != (DataType)result)
                        {
                            Console.WriteLine("Type mismatch.");
                            return Error.assignmentMismatch;
                        }

                    }
                    //retValTo = null;
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
                String content = context.String().GetText();
                for (int i = 0; i < newArray.GetLength(); i++)
                    AddLIT(Convert.ToString(Convert.ToInt32(content[i])));
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
                        Console.WriteLine("Do tohodle nemuzes prirazovat tenhle typ.");
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
                            Console.WriteLine("Do tohodle nemuzes prirazovat tenhle typ.");
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
                            Console.WriteLine("Do tohodle nemuzes prirazovat tenhle typ.");
                            return Error.assignmentMismatch;
                        }
                        //AddLOD(level, funcReturnAddress);
                    }
                    values = values.bool_array_assign();
                }
            }

            return result;
        }

        public override int VisitDef_one_function([NotNull] GrammarParser.Def_one_functionContext context)
        {
            inFunction = true;
            localSymbolTable = new SymbolTable();
            if (!jmpToMainDone) DoMainJmp(0);

            FuncItem newItem = createFunction(context);
            if (!globalSymbolTable.ContainsFuncItem(newItem.GetName())) globalSymbolTable.AddFuncItem(newItem);
            else
            {
                Console.WriteLine("Funkce s timhle jmenem uz existuje!\n");
                return Error.functionAlreadyExists;
            }
            AddINT(3 + newItem.GetParameters().Count);

            level += 1;
            inFunctionAddress = 3;
            for (int i = 0; i < newItem.GetParameters().Count; i++)
            {
                VarConstItem parItem = new VarConstItem(newItem.GetParameters()[i].getName(),
                                                        VarConstType.Var, newItem.GetParameters()[i].getDataType(), inFunctionAddress, level);
                localSymbolTable.AddVarConstItem(parItem);
                inFunctionAddress += 1;
            }

            int result = base.VisitDef_one_function(context);
            level -= 1;

            AddRET(0, 0);
            inFunction = false;
            localSymbolTable = null;

            return result;
        }

        public override int VisitFunction_return([NotNull] GrammarParser.Function_returnContext context)
        {
            int result = 0;
            if (context.condition_expression() == null && context.expression() == null)
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

            AddSTO(level, funcReturnAddress);

            return result;
        }

        public override int VisitAssignment([NotNull] GrammarParser.AssignmentContext context)
        {
            int result = 0;
            GrammarParser.Multiple_assignContext leftSides = context.multiple_assign();
            while (leftSides != null)
            {
                String retValToName = leftSides.Identifier().GetText();
                VarConstItem retValTo = null;

                if (localSymbolTable.ContainsVarConstItem(retValToName)) retValTo = localSymbolTable.GetVarConstItemByName(retValToName);
                else if (globalSymbolTable.ContainsVarConstItem(retValToName)) retValTo = globalSymbolTable.GetVarConstItemByName(retValToName);
                else
                {
                    Console.WriteLine("Promena na leve strane neexistuje");
                    return Error.varConstDoNotExists;
                }

                if (retValTo.GetType() == VarConstType.Const)
                {
                    Console.WriteLine("Nelze prirazovat do konstanty");
                    return Error.assignmentToConstant;
                }

                if (context.condition_expression() != null)
                {
                    result = VisitCondition_expression(context.condition_expression());

                    if (result < 0)
                        return result;

                    if (retValTo.GetDataType() != (DataType)result)
                    {
                        Console.WriteLine("Type mismatch.");
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
                        Console.WriteLine("Type mismatch.");
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
                        Console.WriteLine("Type mismatch.");
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
            String retValToName = context.Identifier().GetText();
            VarConstItem retValTo = null;

            if (localSymbolTable.ContainsVarConstItem(retValToName)) retValTo = localSymbolTable.GetVarConstItemByName(retValToName);
            else if (globalSymbolTable.ContainsVarConstItem(retValToName)) retValTo = globalSymbolTable.GetVarConstItemByName(retValToName);
            else
            {
                Console.WriteLine("Promena na leve strane neexistuje");
                return Error.varConstDoNotExists;
            }

            if (retValTo.GetType() == VarConstType.Const)
            {
                Console.WriteLine("Nelze prirazovat do konstanty");
                return Error.assignmentToConstant;
            }

            if (context.condition_expression() != null)
            {
                result = VisitCondition_expression(context.condition_expression());

                if (result < 0)
                    return result;

                if (retValTo.GetDataType() != (DataType)result)
                {
                    Console.WriteLine("Type mismatch.");
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
                    Console.WriteLine("Type mismatch.");
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
                    Console.WriteLine("Type mismatch.");
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

        public override int VisitBlok_function([NotNull] GrammarParser.Blok_functionContext context)
        {
            return base.VisitBlok_function(context);
        }

        public override int VisitDef_var_blok([NotNull] GrammarParser.Def_var_blokContext context)
        {
            return base.VisitDef_var_blok(context);
        }

        public override int VisitBlok([NotNull] GrammarParser.BlokContext context)
        {
            return base.VisitBlok(context);
        }

        public override int VisitFunction_call([NotNull] GrammarParser.Function_callContext context)
        {
            String fName = context.Identifier().GetText();

            FuncItem calledFce = null;
            if (globalSymbolTable.ContainsFuncItem(fName)) calledFce = globalSymbolTable.GetFuncItemByName(fName);
            else
            {
                Console.WriteLine("Funkce " + fName + " neexistuje!");
                return Error.functionDoNotExists;
            }

            //if (retValTo != null && (calledFce.GetReturnDataType() != retValTo.GetDataType()))
            //{
            //    Console.WriteLine("Navratovy typ funkce se neshoduje s datovym typem promene!\n");
            //    return Error.functionReturnTypesDoNotMatch;
            //}

            AddINT(3);
            List<VarConstItem> usedParameters = new List<VarConstItem>();
            GrammarParser.Par_in_functionContext paramContext = context.par_in_function();
            while (paramContext != null)
            {
                VarConstItem par = null;

                if (paramContext.expression() != null)
                {
                    int result = VisitExpression(paramContext.expression());
                    par = new VarConstItem("", VarConstType.Var, (DataType) result, 0, 0);
                }
                else if (paramContext.condition_expression() != null)
                {
                    VisitCondition_expression(paramContext.condition_expression());
                    par = new VarConstItem("", VarConstType.Var, DataType.Bool, 0, 0);
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
                Console.WriteLine("Spatne pocet parametrů!");
                return Error.functionWrongParametersCount;
            }
            for (int i = 0; i < requestedParameters.Count; i++)
            {
                if (requestedParameters[i].getDataType() != usedParameters[i].GetDataType())
                {
                    Console.WriteLine("Nespravny datovy typ parametru!");
                    return Error.functionParameterDataTypeDoNotMatch;
                }
            }

            AddINT(-1 * (3 + usedParameters.Count()));
            AddCAL(1, calledFce.GetAddress());

            if (calledFce.GetReturnDataType() != DataType.Void)
                AddLOD(level, funcReturnAddress);

            return (int) calledFce.GetReturnDataType();
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
                    Console.WriteLine("Expression - Add: Type mismatch");
                    return Error.assignmentMismatch;
                }

                if ((DataType) ret1 == DataType.Bool)
                {
                    Console.WriteLine("Cannot use operation '+' on :B");
                    return Error.operatorTypeMismatch;
                }

                AddOPR(Instruction.ADD);

            }
            else if (context.Sub() != null)
            {
                if (ret1 != ret2)
                {
                    Console.WriteLine("Expression - Sub: Type mismatch");
                    return Error.assignmentMismatch;
                }

                if ((DataType)ret1 == DataType.Bool)
                {
                    Console.WriteLine("Cannot use operation '-' on :B");
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
                    Console.WriteLine("Expression - Mul: Type mismatch");
                    return Error.assignmentMismatch;
                }

                if ((DataType)ret1 == DataType.Bool)
                {
                    Console.WriteLine("Cannot use operation '*' on :B");
                    return Error.operatorTypeMismatch;
                }

                AddOPR(Instruction.MUL);
            }
            else if (context.Div() != null)
            {
                if (ret1 != ret2)
                {
                    Console.WriteLine("Expression - Div: Type mismatch");
                    return Error.assignmentMismatch;
                }

                if ((DataType)ret1 == DataType.Bool)
                {
                    Console.WriteLine("Cannot use operation '/' on :B");
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
                String varConstName = context.Identifier().GetText();
                VarConstItem varConst = null;
                if (localSymbolTable.ContainsVarConstItem(varConstName)) varConst = localSymbolTable.GetVarConstItemByName(varConstName);
                else if (globalSymbolTable.ContainsVarConstItem(varConstName)) varConst = globalSymbolTable.GetVarConstItemByName(varConstName);
                else
                {
                    Console.WriteLine("Promena ve vyrazu neexistuje");
                    return Error.varConstDoNotExists;
                }

                int varLevel = varConst.GetLevel();
                int levelToMove = Math.Abs(level - varLevel);
                AddLOD(levelToMove, varConst.GetAddress());

                result = (int) varConst.GetDataType();
            }
            else if (context.array_index() != null)
            {
                VarConstItem array = GetVarConst(context.Identifier().GetText());
                if (array == null)
                {
                    Console.WriteLine("Pole ve vyrazu neexistuje");
                    return Error.arrayDoNotExists;
                }

                int index = Convert.ToInt32(context.Int().GetText());

                if (index < 0)
                {
                    Console.WriteLine("Pole nejde indexovat do minusu!");
                    return Error.arrayIndexNegative;
                }
                if (index >= array.GetLength())
                {
                    Console.WriteLine("OutOfBounds, index pole vyjel za length pole");
                    return Error.arrayOutOfBounds;
                }

                int varLevel = array.GetLevel();
                int varAddress = array.GetAddress();
                int levelToMove = Math.Abs(level - varLevel);
                AddLOD(levelToMove, varAddress + index);

                result = (int) array.GetDataType();
            }
            else if (context.function_call() != null)
            {
                if (!globalSymbolTable.ContainsFuncItem(context.function_call().Identifier().GetText()))
                {
                    Console.WriteLine("Funkce volana ve vyrazu neexistuje!");
                    return Error.functionDoNotExists;
                }

                result = VisitFunction_call(context.function_call());

                if (result < 0)
                {
                    return result;
                }

                //AddLOD(level, funcReturnAddress);
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
            String arrayName = context.array_index().Identifier().GetText();
            VarConstItem leftSide = null;
            if (localSymbolTable.ContainsVarConstItem(arrayName)) leftSide = localSymbolTable.GetVarConstItemByName(arrayName);
            else if (globalSymbolTable.ContainsVarConstItem(arrayName)) leftSide = globalSymbolTable.GetVarConstItemByName(arrayName);
            else
            {
                Console.WriteLine("Pole na leve strane neexistuje");
                return Error.arrayDoNotExists;
            }

            int index = Convert.ToInt32(context.array_index().Int().GetText());
            if (index < 0)
            {
                Console.WriteLine("Index nemuze bejt zapornej");
                return Error.arrayIndexNegative;
            }
            if (index >= leftSide.GetLength())
            {
                Console.WriteLine("OutOfBounds, idex pole vyjel za length pole");
                return Error.arrayOutOfBounds;
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
                    Console.WriteLine("Nemuzes priradit tenhle typ do tohodle pole");
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
                    Console.WriteLine("Nemuzes priradit tenhle typ do tohodle pole");
                    return Error.assignmentMismatch;
                }
            }

            int varLevel = leftSide.GetLevel();
            int levelToMove = Math.Abs(level - varLevel);
            AddSTO(levelToMove, leftSide.GetAddress() + index);
            return result;
        }

        public override int VisitIf([NotNull] GrammarParser.IfContext context)
        {
            //AddDebug("IF");
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
            //AddDebug("WHILE");
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
                Console.WriteLine("Cannot compare values of different data types.");
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

            return Visit(context.expression());
        }
    }
    #endregion
}
