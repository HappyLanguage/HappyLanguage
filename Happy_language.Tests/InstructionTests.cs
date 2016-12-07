using Microsoft.VisualStudio.TestTools.UnitTesting;
using Happy_language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happy_language.Tests
{
    [TestClass()]
    public class InstructionTests
    {
        Instruction inst;

        [TestMethod()]
        public void InstructionTest()
        {
            Instruction inst = new Instruction(InstructionType.CAL, 0, "0");

            Assert.AreEqual(inst.Type, InstructionType.CAL);
            Assert.AreEqual(inst.Level, 0);
            Assert.IsTrue(inst.Value.Equals("0"));
        }

        [TestMethod()]
        public void ToStringTest()
        {
            inst = new Instruction(InstructionType.INT, 1, "1");

            string test = "0 INT 1 1" + Environment.NewLine;

            Assert.IsTrue(inst.ToString().Equals(test), "Given strings are not equal: <" + inst + ">, <" + test + ">");
        }
    }
}