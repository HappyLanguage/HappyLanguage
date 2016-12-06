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
    class Visitor : GrammarBaseVisitor<int>
    {
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
        private VarConstItem retValTo = null;

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

            for(int i = 0; i < instructions.Count; i++)
            {
                instructions[i].Number = i;
            }
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

        public void DoInitialJmp(int dest)
        {
            AddJMP(dest);
            AddINT(3);
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

        private VarConstItem createConst(GrammarParser.Def_constContext context)
        {
            String name, value = "";
            DataType dt = DataType.Int;

            name = context.Identifier().GetText();
            if (context.Data_type_bool() != null)
            {
                dt = DataType.Bool;
                value = context.Bool().GetText();
            }
            else if (context.Data_type_double() != null)
            {
                dt = DataType.Double;
                value = context.Double().GetText();
            }
            else value = context.Int().GetText();  

            int addr = 0;
            if (inFunction)
                addr = inFunctionAddress;
            else
                addr = globalAddress;

            return new VarConstItem(name, value, VarConstType.Const, dt, addr, level);
        }

        private String removeUnaryOperator(String value)
        {
            if (value[0] == '+' || value[0] == '-')
                value = value.Substring(1);

            return value;
        }

        private Boolean isNegative(String value)
        {
            return value[0] == '-';
        }

        private VarConstItem createArray(GrammarParser.Array_inicializationContext context)
        {
            String name = context.Identifier().GetText();
            int length = Convert.ToInt32(context.Int().GetText());
            DataType dt = DataType.Int;

            if (context.Data_type_bool() != null) dt = DataType.Bool;
            else if (context.Data_type_double() != null) dt = DataType.Double;

            int addr = 0;
            if (inFunction)
                addr = inFunctionAddress;
            else
                addr = globalAddress;

            return new VarConstItem(name, length, VarConstType.Var, dt, addr, level); ;
        }

        private VarConstItem createVar(GrammarParser.Def_varContext context)
        {
            String name, value = "";
            DataType dt = DataType.Int;

            name = context.Identifier().GetText();
            if (context.Data_type_bool() != null)
            {
                dt = DataType.Bool;
                value = BoolToInt(context.Bool().GetText());
            }
            else if (context.Data_type_double() != null)
            {
                dt = DataType.Double;
                value = context.Double().GetText();
            }
            else value = context.Int().GetText();

            int addr = 0;
            if (inFunction)
                addr = inFunctionAddress;
            else
                addr = globalAddress;
            return new VarConstItem(name, value, VarConstType.Var, dt, addr, level);
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

        private Boolean processArray(VarConstItem array)
        {
            if (array.GetLength() < 0)
            {
                //error
                Console.WriteLine("Pole " + array.GetName() + " ma zapornou delku.");
                return false;
            }

            if (inFunction)
            {
                if (!localSymbolTable.ContainsVarConstItem(array.GetName()))
                    localSymbolTable.AddVarConstItem(array);
                else
                {
                    errors.Add("Pole " + array.GetName() + " uz existuje!\n");
                    Console.WriteLine("Pole " + array.GetName() + " uz existuje!\n");
                }
                inFunctionAddress += array.GetLength();
            }
            else
            {
                if (!globalSymbolTable.ContainsVarConstItem(array.GetName()))
                    globalSymbolTable.AddVarConstItem(array);
                else
                {
                    errors.Add("Pole " + array.GetName() + " uz existuje!\n");
                    Console.WriteLine("Pole " + array.GetName() + " uz existuje!\n");
                }
                globalAddress += array.GetLength();
            }

            AddINT(array.GetLength());

            return true;
        }

        private Boolean processVarConst(VarConstItem item)
        {
            if (inFunction)
            {
                if (!localSymbolTable.ContainsVarConstItem(item.GetName()))
                    localSymbolTable.AddVarConstItem(item);
                else
                {
                    errors.Add("Promena " + item.GetName() + " uz existuje!\n");
                    Console.WriteLine("Promena " + item.GetName() + " uz existuje!\n");
                }
                inFunctionAddress += 1;
            }
            else
            {
                if (!globalSymbolTable.ContainsVarConstItem(item.GetName()))
                    globalSymbolTable.AddVarConstItem(item);
                else
                {
                    errors.Add("Promena " + item.GetName() + " uz existuje!\n");
                    Console.WriteLine("Promena " + item.GetName() + " uz existuje!\n");
                }
                globalAddress += 1;
            }

            Boolean isNeg = isNegative(item.GetValue());
            String value = removeUnaryOperator(item.GetValue());
            AddLIT(value);
            if (isNeg) AddOPR(Instruction.UNARY_MINUS);

            return true;
        }

        public VarConstItem GetVarConst(String varConstName)
        {
            if (localSymbolTable.ContainsVarConstItem(varConstName))
                return localSymbolTable.GetVarConstItemByName(varConstName);
            else if (globalSymbolTable.ContainsVarConstItem(varConstName))
                return globalSymbolTable.GetVarConstItemByName(varConstName);

            return null;
        }

        public FuncItem GetFunction(String functionName)
        {
            if (globalSymbolTable.ContainsFuncItem(functionName))
                return globalSymbolTable.GetFuncItemByName(functionName);

            return null;
        }

        /* 
         * ======================================================================================================
         * ======================================================================================================
         * ===                                                                                                ===
         * ===                                      VISITORS                                                  ===
         * ===                                                                                                ===
         * ======================================================================================================
         * ======================================================================================================
        */

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
            VarConstItem newItem = createConst(context);
            processVarConst(newItem);

            //base.VisitDef_const(context);
            return 20;
        }

        public override int VisitDef_var([NotNull] GrammarParser.Def_varContext context)
        {
            if (context.array_inicialization() == null)
            {
                VarConstItem newItem = createVar(context);
                processVarConst(newItem);
            }
            else
            {
                VisitArray_inicialization(context.array_inicialization());
            }
          
            //base.VisitDef_var(context);
            return 0;
        }

        public override int VisitArray_inicialization([NotNull] GrammarParser.Array_inicializationContext context)
        {
            VarConstItem newArray = createArray(context);
            processArray(newArray);

            return 0; //base.VisitArray_inicialization(context);
        }

        // kdyz vlezu do smeru dolu tak level zvetsim asi a kdyz se vyleze tam zmensit
        public override int VisitDef_one_function([NotNull] GrammarParser.Def_one_functionContext context)
        {
            inFunction = true;
            localSymbolTable = new SymbolTable();
            if (!jmpToMainDone) DoMainJmp(0);
            

            FuncItem newItem = createFunction(context);
            if (!globalSymbolTable.ContainsFuncItem(newItem.GetName())) globalSymbolTable.AddFuncItem(newItem);
            else {
                //Console.WriteLine("Funkce s timhle jmenem uz existuje!\n");
                errors.Add("Funkce " + newItem.GetName() + " už existuje!\n");
            }
            AddINT(3 + newItem.GetParameters().Count);
            
            level += 1;
            //inFunctionAddress = 3 + newItem.GetParameters().Count;
            inFunctionAddress = 3;
            for (int i = 0; i < newItem.GetParameters().Count; i++)
            {

                VarConstItem parItem = new VarConstItem(newItem.GetParameters()[i].getName(), "neni potreba", 
                                                        VarConstType.Var, newItem.GetParameters()[i].getDataType(), inFunctionAddress, 0);
                localSymbolTable.AddVarConstItem(parItem);
                inFunctionAddress += 1;
            }
            
            base.VisitDef_one_function(context);
            level -= 1;
            Console.WriteLine(localSymbolTable.VarConstToString());

            AddRET(0, 0);
            inFunction = false;
            localSymbolTable = null;

            return 456;
        }

        public override int VisitFunction_return([NotNull] GrammarParser.Function_returnContext context)
        {
            if(context.function_call() != null)
            {
                VisitFunction_call(context.function_call());
                AddLOD(level, funcReturnAddress);
            }
            else if (context.expression() != null)
            {
                VisitExpression(context.expression());
            }
            else if (context.Identifier() != null)
            {
               // rightSideVarConst(context);
            }
            else
            {
                //rightSideValue(context);
            }
            if (context.Int() != null)
                AddLIT(context.Int().GetText());


            //STO na functionAdress return nebo jak se to jmenuje
            AddSTO(level, funcReturnAddress);
            return base.VisitFunction_return(context);

        }

        private void rightSideValue(GrammarParser.AssignmentContext context)
        {
            String newValue = "";
            if (context.Int() != null) newValue = context.Int().GetText();
            else if (context.Bool() != null) newValue = BoolToInt(context.Bool().GetText());
            else if (context.Double() != null) newValue = context.Double().GetText();

            Boolean isNeg = isNegative(newValue);
            String value = removeUnaryOperator(newValue);

            AddLIT(value);
            if (isNeg) AddOPR(Instruction.UNARY_MINUS);
        }

        private void rightSideVarConst(GrammarParser.AssignmentContext context)
        {
            String rightSideName = context.Identifier(1).GetText();
            VarConstItem rightSideVar = null;
            if (localSymbolTable.ContainsVarConstItem(rightSideName)) rightSideVar = localSymbolTable.GetVarConstItemByName(rightSideName);
            else if (globalSymbolTable.ContainsVarConstItem(rightSideName)) rightSideVar = globalSymbolTable.GetVarConstItemByName(rightSideName);
            else
            {
                Console.WriteLine("Promena na prave strane neexistuje");
                // error
            }

            int varLevel = rightSideVar.GetLevel();
            int levelToMove = Math.Abs(level - varLevel);
            AddLOD(levelToMove, rightSideVar.GetAddress());
        }

        private void rightSideExpression(GrammarParser.AssignmentContext context)
        {
            String rightSide = Convert.ToString(VisitExpression(context.expression()));

            //AddLIT(rightSide);
            //if (isNegative(rightSide)) AddOPR(Instruction.UNARY_MINUS);
        }

        public override int VisitAssignment([NotNull] GrammarParser.AssignmentContext context)
        {
            String retValToName = context.Identifier(0).GetText();
            if (localSymbolTable.ContainsVarConstItem(retValToName)) retValTo = localSymbolTable.GetVarConstItemByName(retValToName);
            else if (globalSymbolTable.ContainsVarConstItem(retValToName)) retValTo = globalSymbolTable.GetVarConstItemByName(retValToName);
            else
            {
                Console.WriteLine("Promena na levy strane neexistuje");
                // error
            }
            
            if (context.function_call() != null)
            {
                VisitFunction_call(context.function_call());
                AddLOD(level, funcReturnAddress);
            }
            else if (context.expression() != null)
            {
                VisitExpression(context.expression());
            }
            else if (context.Identifier(1) != null)
            {
                rightSideVarConst(context);
            }
            else
            {
                rightSideValue(context);
            }

            int varLevel = retValTo.GetLevel();
            int varAddress = retValTo.GetAddress();
            int levelToMove = Math.Abs(level - varLevel);
            AddSTO(levelToMove, varAddress);

            retValTo = null;

            return 0;
        }

        public override int VisitMain([NotNull] GrammarParser.MainContext context)
        {
            if (!jmpToMainDone) DoMainJmp(0);
            inFunction = true;
            inFunctionAddress = 3;
            level += 1;
            localSymbolTable = new SymbolTable();

            changeJMPtoMain();
            base.VisitMain(context);
            endProgram();

            inFunction = false;
            localSymbolTable = null;
            level -= 1;
            return 856;
        }

        public override int VisitBlok_function([NotNull] GrammarParser.Blok_functionContext context)
        {    
            base.VisitBlok_function(context);

            return 0;
        }

        public override int VisitDef_var_blok([NotNull] GrammarParser.Def_var_blokContext context)
        {
            return base.VisitDef_var_blok(context);
        }
        
        public override int VisitBlok([NotNull] GrammarParser.BlokContext context)
        {
            base.VisitBlok(context);

            return 45;
        }


        public override int VisitFunction_call([NotNull] GrammarParser.Function_callContext context)
        {  
            String fName = context.Identifier().GetText();
            FuncItem calledFce = null;
            if (globalSymbolTable.ContainsFuncItem(fName)) calledFce = globalSymbolTable.GetFuncItemByName(fName);
            else
            {
                errors.Add("Funkce " + fName + " neexistuje!\n");
                Console.WriteLine("Funkce " + fName + " neexistuje!");
                // error
            }

            VarConstItem destForVal = retValTo;
            if(destForVal != null && (calledFce.GetReturnDataType() != destForVal.GetDataType()))   // overeni navratove hodnoty a dat. typu promeny
            {
                errors.Add("Navratovy typ funkce se neshoduje s datovym typem promene!\n");
                Console.WriteLine("Navratovy typ funkce se neshoduje s datovym typem promene!\n");
                // error
            }

            AddINT(3);
            List<VarConstItem> usedParameters = new List<VarConstItem>();
            GrammarParser.Par_in_functionContext paramContext = context.par_in_function();
            while (paramContext != null)
            {
                VarConstItem par = null;
                if (paramContext.Identifier() != null)
                {
                    String parName = paramContext.Identifier().GetText();
                    if (localSymbolTable.ContainsVarConstItem(parName)) par = localSymbolTable.GetVarConstItemByName(parName); // tohle hodit do metody
                    else if (globalSymbolTable.ContainsVarConstItem(parName)) par = globalSymbolTable.GetVarConstItemByName(parName);
                    else
                    {
                        Console.WriteLine("Parametr neexistuje!");
                        errors.Add("Parametr neexistuje!");
                        // error
                    }
    
                    int varLevel = par.GetLevel();
                    int levelToMove = Math.Abs(level - varLevel);
                    AddLOD(levelToMove, par.GetAddress());
                }
                else if(paramContext.Int() != null)
                {
                    par = new VarConstItem("", paramContext.Int().GetText(), VarConstType.Var, DataType.Int, 0, 0);
                    Boolean isNeg = isNegative(paramContext.Int().GetText());
                    String value = removeUnaryOperator(paramContext.Int().GetText());

                    AddLIT(value);
                    if (isNeg) AddOPR(Instruction.UNARY_MINUS);
                }
                else if(paramContext.Bool() != null)
                {
                    par = new VarConstItem("", paramContext.Bool().GetText(), VarConstType.Var, DataType.Bool, 0, 0);
                    AddLIT(BoolToInt(paramContext.Bool().GetText()));
                }
                else if (paramContext.Double() != null)
                {
                    par = new VarConstItem("", paramContext.Double().GetText(), VarConstType.Var, DataType.Double, 0, 0);
                }  
                
                usedParameters.Add(par);
                paramContext = paramContext.par_in_function();
            }

            // z tabulky vythnout pozadovane parametry
            List<FunctionParameter> requestedParameters = calledFce.GetParameters();
            if(requestedParameters.Count != usedParameters.Count)
            {
                Console.WriteLine("Spatne pocet parametrů!");
                errors.Add("Spatne parametrů");
                // error
            }
            for(int i = 0; i < requestedParameters.Count; i++)
            {
                if(requestedParameters[i].getDataType() != usedParameters[i].GetDataType())
                {
                    Console.WriteLine("Nespravny datovy typ parametru!");
                    errors.Add("Nespravny datovy typ parametru");
                    // error
                    break;
                }
            }

            AddINT(-1 * (3 + usedParameters.Count()));
            AddCAL(1, calledFce.GetAddress());

            return 451;
        }

        public override int VisitExpression([NotNull] GrammarParser.ExpressionContext context)
        {
            Console.WriteLine("---VISITING EXPRESSION---");

            int leftSide = 0, rightSide = 0, result = 0;
            if (context.expression() != null)
            {
                leftSide = VisitExpression(context.expression());
            }
            if(context.expression_multiply() != null)
            {
                rightSide = VisitExpression_multiply(context.expression_multiply());
            }

            if(context.Add() != null)
            {
                result = leftSide + rightSide;
                AddOPR(Instruction.ADD);
                
            }
            else if(context.Sub() != null)
            {
                result = leftSide - rightSide;
                AddOPR(Instruction.SUB);
            }
            else
            {
                result = rightSide;
            }

            Console.WriteLine("E: " + result);
            return result;
        }

        public override int VisitExpression_multiply([NotNull] GrammarParser.Expression_multiplyContext context)
        {
            Console.WriteLine("---VISITING MULTIPLY---");

            int leftSide = 0, rightSide = 0, result = 0;
            if (context.expression_multiply() != null) 
            {
                leftSide = VisitExpression_multiply(context.expression_multiply());
            }
            rightSide = VisitExpression_item(context.expression_item());

            // z VisitExpression itemu mam neco na vrcholu
            // tady se daj instrkce na zpracování 
            if (context.Mul() != null)
            {
                result = leftSide * rightSide;
                AddOPR(Instruction.MUL);
            }
            else if (context.Div() != null)
            {
                result = leftSide / rightSide;
                AddOPR(Instruction.DIV);
            }
            else
            {
                result = rightSide;
            }

            return result;
        }

        public override int VisitExpression_item([NotNull] GrammarParser.Expression_itemContext context)
        {
            int result = 0;
            if(context.Int() != null)
            {
                result = Convert.ToInt32(context.Int().GetText());
                AddLIT(context.Int().GetText());
            }
            else if(context.Identifier() != null)
            {
                String varConstName = context.Identifier().GetText();
                VarConstItem varConst = null;
                if (localSymbolTable.ContainsVarConstItem(varConstName)) varConst = localSymbolTable.GetVarConstItemByName(varConstName);
                else if (globalSymbolTable.ContainsVarConstItem(varConstName)) varConst = globalSymbolTable.GetVarConstItemByName(varConstName);
                else
                {
                    Console.WriteLine("Promena ve vyrazu neexistuje");
                    // error
                }
                result = Convert.ToInt32(varConst.GetValue());

                int varLevel = varConst.GetLevel();
                int levelToMove = Math.Abs(level - varLevel);
                AddLOD(levelToMove, varConst.GetAddress());
            }
            else if(context.array_index() != null)
            {
                VarConstItem array = GetVarConst(context.Identifier().GetText());
                if(array == null)
                {
                    Console.WriteLine("Pole ve vyrazu neexistuje");
                    // error
                }

                int index = Convert.ToInt32(context.Int().GetText());

                if (index < 0)
                {
                    Console.WriteLine("Pole nejde indexovat do minusu!");
                    // error
                }
                if (index >= array.GetLength())
                {
                    Console.WriteLine("OutOfBounds, idex pole vyjel za length pole");
                    // error
                }

                int varLevel = array.GetLevel();
                int varAddress = array.GetAddress();
                int levelToMove = Math.Abs(level - varLevel);
                AddLOD(levelToMove, varAddress + index);
            }
            else if(context.function_call() != null)
            {
                if (!globalSymbolTable.ContainsFuncItem(context.function_call().Identifier().GetText())) {
                    Console.WriteLine("Funkce volana ve vyrazu neexistuje!");
                    // error
                }
                
                result = VisitFunction_call(context.function_call());
                AddLOD(level, funcReturnAddress);
            }
            else if(context.expression() != null)
            {
                result = VisitExpression(context.expression());
            }       

            return result; 
        }

        private void rightSideValue(GrammarParser.Assignment_arrayContext context)
        {
            String newValue = "";
            if (context.Int() != null) newValue = context.Int().GetText();
            else if (context.Bool() != null) newValue = BoolToInt(context.Bool().GetText());
            else if (context.Double() != null) newValue = context.Double().GetText();

            Boolean isNeg = isNegative(newValue);
            String value = removeUnaryOperator(newValue);

            AddLIT(value);
            if (isNeg) AddOPR(Instruction.UNARY_MINUS);
        }

        private void rightSideVarConst(GrammarParser.Assignment_arrayContext context)
        {
            String rightSideName = context.Identifier().GetText();
            VarConstItem rightSideVar = null;
            if (localSymbolTable.ContainsVarConstItem(rightSideName)) rightSideVar = localSymbolTable.GetVarConstItemByName(rightSideName);
            else if (globalSymbolTable.ContainsVarConstItem(rightSideName)) rightSideVar = globalSymbolTable.GetVarConstItemByName(rightSideName);
            else
            {
                Console.WriteLine("Promena na prave strane neexistuje");
                // error
            }

            int varLevel = rightSideVar.GetLevel();
            int levelToMove = Math.Abs(level - varLevel);
            AddLOD(levelToMove, rightSideVar.GetAddress());
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
                // error
            }

            int index = Convert.ToInt32(context.array_index().Int().GetText());
            if(index < 0)
            {
                Console.WriteLine("Index nemuze bejt zapornej");
                // error
            }
            if (index >= leftSide.GetLength())
            {
                Console.WriteLine("OutOfBounds, idex pole vyjel za length pole");
                // error
            }

            if (context.Identifier() != null)
            {
                rightSideVarConst(context);
            }
            else if (context.expression() != null)
            {
                VisitExpression(context.expression());
            }
            else if(context.function_call() != null)
            {
                VisitFunction_call(context.function_call());
                AddLOD(level, funcReturnAddress);
            }
            else
            {
                rightSideValue(context);
            }

            int varLevel = leftSide.GetLevel();
            int levelToMove = Math.Abs(level - varLevel);
            AddSTO(levelToMove, leftSide.GetAddress() + index);

            return 0; // base.VisitAssignment_array(context);
        }


        public override int VisitIf([NotNull] GrammarParser.IfContext context)
        {
            instructions.Add(new Instruction(InstructionType.DEBUG, 0, "fdasfdf"));
            instructionCount++;
            Visit(context.condition());
            int ifAddress = instructionCount;
            AddJMC(0);
            Visit(context.blok());
            ChangeJMC(ifAddress, instructionCount);
            return 10;
        }

        public override int VisitCondition([NotNull] GrammarParser.ConditionContext context)
        {
            if (context.condition_expression() != null)
            {
                Visit(context.condition_expression());
            }

            if (context.GetChild<GrammarParser.ConditionContext>(0) != null)
            {
                Visit(context.GetChild<GrammarParser.ConditionContext>(0));
            }

            if (context.GetChild<GrammarParser.ConditionContext>(1) != null)
            {
                Visit(context.GetChild<GrammarParser.ConditionContext>(1));
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
                return 1;
            }

            Visit(context.condition_item()[0]);
            Visit(context.condition_item()[1]);
            //TODO takhle to funguje jen pro int a boolean... double se takhle jednoduse neporovna...

            switch(context.Operator_condition().GetText())
            {
                case "==": AddOPR(Instruction.EQ); break;
                case "!=": AddOPR(Instruction.NEQ); break;
                case "<": AddOPR(Instruction.LESS); break;
                case "<=": AddOPR(Instruction.LEQ); break;
                case ">": AddOPR(Instruction.GREATER); break;
                case ">=": AddOPR(Instruction.GEQ); break;
            }

            return 2;
        }

        public override int VisitCondition_item([NotNull] GrammarParser.Condition_itemContext context)
        {
            Console.WriteLine(context.GetText());
            return base.VisitCondition_item(context);
        }
    }
}
