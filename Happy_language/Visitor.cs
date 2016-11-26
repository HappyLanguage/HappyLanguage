using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

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
        private List<String> instructions = new List<String>();
        
        private Boolean initialJmpDone = false;
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

        private int globalAddress = 3;
        private int level = 0;

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

        public List<String> GetInstructions()
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
                instructions[i] = i + " " + instructions[i];
            }
        }

        public void AddLIT(String value)
        {
            instructions.Add(InstructionType.LIT.ToString() + " " + 0 + " " + value + Environment.NewLine);
            instructionCount += 1;
        }

        public void AddSTO(int level, int address)
        {
            instructions.Add(InstructionType.STO.ToString() + " " + level + " " + address + Environment.NewLine);
            instructionCount += 1;
        }

        public void AddOPR(int opCode)
        {
            instructions.Add(InstructionType.OPR.ToString() + " " + 0 + " " + opCode + Environment.NewLine);
            instructionCount += 1;
        }

        public void AddJMP(int codeAddress)
        {
            instructions.Add(InstructionType.JMP.ToString() + " " + 0 + " " + codeAddress + Environment.NewLine);
            instructionCount += 1;
        }

        public void ChangeJMP(int codeAddress, int index)
        {
            instructions[index] = InstructionType.JMP.ToString() + " " + 0 + " " + codeAddress + Environment.NewLine;
        }

        public void AddINT(int value)
        {
            instructions.Add(InstructionType.INT.ToString() + " " + 0 + " " + value + Environment.NewLine);
            instructionCount += 1;
        }

        public void AddCAL(int level, int address)
        {
            instructions.Add(InstructionType.CAL.ToString() + " " + level + " " + address + Environment.NewLine);
            instructionCount += 1;
        }

        public void ChangeCAL(int level, int address, int index)
        {
            instructions[index] = InstructionType.CAL.ToString() + " " + level + " " + address + Environment.NewLine;
        }

        public void AddRET(int level, int codeAddress)
        {
            instructions.Add(InstructionType.RET.ToString() + " " + level + " " + codeAddress + Environment.NewLine);
            instructionCount += 1;
        }

        public void AddLOD(int level, int address)
        {
            instructions.Add(InstructionType.LOD.ToString() + " " + level + " " + address + Environment.NewLine);
            instructionCount += 1;
        }

        public void AddJMC(int address)
        {
            instructions.Add(InstructionType.JMC.ToString() + " 0 " + address + Environment.NewLine);
        }

        public void ChangeJMC(int index, int address)
        {
            instructions[index] = InstructionType.JMC.ToString() + " 0 " + address + Environment.NewLine;
        }

        public void DoInitialJmp(int dest)
        {
            AddJMP(dest);
            AddINT(3);

            initialJmpDone = true;
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
            if (!initialJmpDone)  DoInitialJmp(1);
            VarConstItem newItem = createConst(context);

            if (inFunction)
            {
                if (!localSymbolTable.ContainsVarConstItem(newItem.GetName()))
                    localSymbolTable.AddVarConstItem(newItem);
                else
                {
                    errors.Add("Promena " + newItem.GetName() + " uz existuje!\n");
                    Console.WriteLine("Promena " + newItem.GetName() + " uz existuje!\n");
                }
                inFunctionAddress += 1;
            }
            else
            {
                if (!globalSymbolTable.ContainsVarConstItem(newItem.GetName()))
                    globalSymbolTable.AddVarConstItem(newItem);
                else
                {
                    errors.Add("Promena " + newItem.GetName() + " uz existuje!\n");
                    Console.WriteLine("Promena " + newItem.GetName() + " uz existuje!\n");
                }
                globalAddress += 1;
            }
            

            Boolean isNeg = isNegative(newItem.GetValue());
            String value = removeUnaryOperator(newItem.GetValue());
            AddLIT(value);
            if(isNeg) AddOPR(Instruction.UNARY_MINUS);

            base.VisitDef_const(context);
            return 20;
        }

        public override int VisitDef_var([NotNull] GrammarParser.Def_varContext context)
        {
            if (!initialJmpDone) DoInitialJmp(1);
            VarConstItem newItem = createVar(context);

            if (inFunction)
            {
                if (!localSymbolTable.ContainsVarConstItem(newItem.GetName()))
                    localSymbolTable.AddVarConstItem(newItem);
                else
                {
                    errors.Add("Promena " + newItem.GetName() + " uz existuje!\n");
                    Console.WriteLine("Promena " + newItem.GetName() + " uz existuje!\n");
                }
                inFunctionAddress += 1;
            }
            else
            {
                if (!globalSymbolTable.ContainsVarConstItem(newItem.GetName()))
                    globalSymbolTable.AddVarConstItem(newItem);
                else
                {
                    errors.Add("Promena " + newItem.GetName() + " uz existuje!\n");
                    Console.WriteLine("Promena " + newItem.GetName() + " uz existuje!\n");
                }
                globalAddress += 1;
            }      

            Boolean isNeg = isNegative(newItem.GetValue());
            String value = removeUnaryOperator(newItem.GetValue());
            AddLIT(value);
            if (isNeg) AddOPR(Instruction.UNARY_MINUS);
          
            base.VisitDef_var(context);
            return 0;
        }

        // kdyz vlezu do smeru dolu tak level zvetsim asi a kdyz se vyleze tam zmensit
        public override int VisitDef_one_function([NotNull] GrammarParser.Def_one_functionContext context)
        {
            inFunction = true;
            localSymbolTable = new SymbolTable();
            if (!initialJmpDone) DoInitialJmp(1);
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
            // tady muzu dat jen lit
            // jestli to bude potreba vyzvednout se bude resit az ve function call
            if(context.Int() != null)
                AddLIT(context.Int().GetText());
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

            /*int varLevel = retValTo.GetLevel();
            int varAddress = retValTo.GetAddress();
            int levelToMove = Math.Abs(level - varLevel);
            AddSTO(levelToMove, varAddress);*/
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
            }

            int varLevel = rightSideVar.GetLevel();
            int levelToMove = Math.Abs(level - varLevel);
            AddLOD(levelToMove, rightSideVar.GetAddress());

            /*varLevel = retValTo.GetLevel();
            levelToMove = Math.Abs(level - varLevel);
            AddSTO(levelToMove, retValTo.GetAddress());*/
        }

        public override int VisitAssignment([NotNull] GrammarParser.AssignmentContext context)
        {
            String retValToName = context.Identifier(0).GetText();
            if (localSymbolTable.ContainsVarConstItem(retValToName)) retValTo = localSymbolTable.GetVarConstItemByName(retValToName);
            else if (globalSymbolTable.ContainsVarConstItem(retValToName)) retValTo = globalSymbolTable.GetVarConstItemByName(retValToName);
            else
            {
                Console.WriteLine("Promena na levy strane neexistuje");
            }
            
            if (context.function_call() != null)
            {
                Console.WriteLine("PRAVA strana: function");
            }
            else if (context.expression() != null)
            {
                Console.WriteLine("PRAVA strana: expression");
            }
            else if (context.Identifier(1) != null)
            {
                Console.WriteLine("PRAVA strana: identifier");
                rightSideVarConst(context);
            }
            else  // jinak je na pravo nejaka hodnota int, double, bool
            {
                Console.WriteLine("PRAVA strana: int, double, bool hodnota");
                rightSideValue(context);
            }

            base.VisitAssignment(context);

            // pokud se neco returnuje, tady muzu resit kam to dat
            // z VisitFunction_return bude navrcholu zasobniku nejaky cislo, pokud teda bude
            // tyhle radky maj vsechny prirazeni spolecny, vsechny nechaj na vrcholu neco, co se potom musi storenout nekam (krome kdyz se jen vola fce)
            // funkce se jeste musi udelat pres tu specialni misto v pameti asi
            if (retValTo != null) 
            {
                int varLevel = retValTo.GetLevel();
                int varAddress = retValTo.GetAddress();
                int levelToMove = Math.Abs(level - varLevel);
                AddSTO(levelToMove, varAddress);
            }
            
            /*int varLevel = retValTo.GetLevel();
            int levelToMove = Math.Abs(level - varLevel);
            AddSTO(levelToMove, retValTo.GetAddress());*/

            retValTo = null;

            return 0;
        }

        public override int VisitMain([NotNull] GrammarParser.MainContext context)
        {
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
            }

            VarConstItem destForVal = retValTo;
            if(calledFce.GetReturnDataType() != destForVal.GetDataType())   // overeni navratove hodnoty a dat. typu promeny
            {
                errors.Add("Navratovy typ funkce se neshoduje s datovym typem promene!\n");
                Console.WriteLine("Navratovy typ funkce se neshoduje s datovym typem promene!\n");
            }

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
                    }
                }
                else if(paramContext.Int() != null)
                {
                    par = new VarConstItem("", paramContext.Int().GetText(), VarConstType.Var, DataType.Int, 0, 0);
                }
                else if(paramContext.Bool() != null)
                {
                    par = new VarConstItem("", paramContext.Bool().GetText(), VarConstType.Var, DataType.Int, 0, 0);
                }
                else if (paramContext.Double() != null)
                {
                    par = new VarConstItem("", paramContext.Double().GetText(), VarConstType.Var, DataType.Int, 0, 0);
                }  
                
                if(par != null)
                {
                    usedParameters.Add(par);
                }
                paramContext = paramContext.par_in_function();
            }

            // z tabulky vythnout pozadovane parametry
            List<FunctionParameter> requestedParameters = calledFce.GetParameters();
            if(requestedParameters.Count != usedParameters.Count)
            {
                Console.WriteLine("Spatne parametry!");
                errors.Add("Spatne parametry");
            }
            for(int i = 0; i < requestedParameters.Count; i++)
            {
                if(requestedParameters[i].getDataType() != usedParameters[i].GetDataType())
                {
                    Console.WriteLine("Nespravny datovy typ parametru!");
                    errors.Add("Nespravny datovy typ parametru");
                    break;
                }
            }

            AddINT(3);
            for(int i = 0; i < usedParameters.Count; i++)
            {
                AddLIT(usedParameters[i].GetValue());
            }
            AddINT(-1 * (3 + usedParameters.Count()));
            AddCAL(1, calledFce.GetAddress());


            base.VisitFunction_call(context);

            return 451;
        }

        public override int VisitExpression([NotNull] GrammarParser.ExpressionContext context)
        {

            return base.VisitExpression(context);
        }

        public override int VisitIf([NotNull] GrammarParser.IfContext context)
        {
            Visit(context.condition());
            int ifAddress = instructionCount;
            AddJMC(0);
            Visit(context.blok());
            ChangeJMC(ifAddress, instructionCount + 1);
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
    }
}
