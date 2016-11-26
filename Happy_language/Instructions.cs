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
        JMC

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

        private InstructionType instructionType;
        private int level;
        private int value;

        public Instruction(InstructionType type, int level, int value)
        {
            this.instructionType = type;
            this.level = level;
            this.value = value;
        }

        public InstructionType Type
        {
            get { return instructionType; }
            set { this.instructionType = value; }
        }

        public int Level
        {
            get { return level; }
            set { this.level = value; }
        }

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }


        public override string ToString()
        {
            return instructionType.ToString() + " " + level + " " + value;
        }

    }
}
