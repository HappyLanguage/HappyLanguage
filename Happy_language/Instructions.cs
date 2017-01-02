using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happy_language
{
    public enum InstructionType
    {
        LIT,
        STO,
        OPR,
        JMP,
        INT,
        RET,
        CAL,
        LOD,
        JMC,
        WRI,
        PST,
        PLD
    }

    public class Instruction
    {
        public const int UNARY_MINUS = 1;
        public const int ADD = 2;
        public const int SUB = 3;
        public const int MUL = 4;
        public const int DIV = 5;
        public const int MOD = 6;
        public const int ODD = 7;
        public const int EQ = 8;
        public const int NEQ = 9;
        public const int LESS = 10;
        public const int GEQ = 11;
        public const int GREATER = 12;
        public const int LEQ = 13;

        public Instruction(InstructionType type, int level, string value)
        {
            this.Type = type;
            this.Level = level;
            this.Value = value;
        }

        public InstructionType Type
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public int Number
        {
            get;
            set;
        }



        public override string ToString()
        {
            return Number + " " + Type.ToString() + " " + Level + " " + Value + Environment.NewLine;
        }

    }
}
