﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happy_language
{
	public static class Error
    {
        public static int varConstDoNotExists= -1;
        public static int varConstAlreadyExists = -2;
        public static int functionDoNotExists = -3;
        public static int functionAlreadyExists = -4;
        public static int arrayDoNotExists = -5;
        public static int arrayAlreadyExists = -6;
        public static int arrayLengthNegative = -7;
        public static int arrayOutOfBounds = -8;
        public static int functionReturnTypesDoNotMatch = -9;
        public static int functionWrongParametersCount = -10;
        public static int functionParameterDataTypeDoNotMatch = -11;
        public static int arrayIndexNegative = -12;
        public static int cmpTypeMismatch = -13;
		public static int assignmentToConstant = -14;

	}
}
