//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.5.3
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:\Users\jdvorak\Dropbox\Skola\FJP\HappyLanguage\Happy_language\Grammar.g4 by ANTLR 4.5.3

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace Happy_language {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="GrammarParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.5.3")]
[System.CLSCompliant(false)]
public interface IGrammarVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.start"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStart([NotNull] GrammarParser.StartContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.def_con_var"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDef_con_var([NotNull] GrammarParser.Def_con_varContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.def_const"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDef_const([NotNull] GrammarParser.Def_constContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.def_var"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDef_var([NotNull] GrammarParser.Def_varContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.def_var_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDef_var_expression([NotNull] GrammarParser.Def_var_expressionContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.def_var_from_function"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDef_var_from_function([NotNull] GrammarParser.Def_var_from_functionContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.function_call"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction_call([NotNull] GrammarParser.Function_callContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.def_var_blok"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDef_var_blok([NotNull] GrammarParser.Def_var_blokContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.par_in_function"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPar_in_function([NotNull] GrammarParser.Par_in_functionContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.def_functions"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDef_functions([NotNull] GrammarParser.Def_functionsContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.def_one_function"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDef_one_function([NotNull] GrammarParser.Def_one_functionContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.function_return"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction_return([NotNull] GrammarParser.Function_returnContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.function_return_data_typ"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction_return_data_typ([NotNull] GrammarParser.Function_return_data_typContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.data_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitData_type([NotNull] GrammarParser.Data_typeContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.main"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMain([NotNull] GrammarParser.MainContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.blok_function"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBlok_function([NotNull] GrammarParser.Blok_functionContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.blok"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBlok([NotNull] GrammarParser.BlokContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.if"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIf([NotNull] GrammarParser.IfContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.else"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitElse([NotNull] GrammarParser.ElseContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.while"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitWhile([NotNull] GrammarParser.WhileContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.for"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFor([NotNull] GrammarParser.ForContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.for_condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFor_condition([NotNull] GrammarParser.For_conditionContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression([NotNull] GrammarParser.ExpressionContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.condition_item"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCondition_item([NotNull] GrammarParser.Condition_itemContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.condition_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCondition_expression([NotNull] GrammarParser.Condition_expressionContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCondition([NotNull] GrammarParser.ConditionContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.assignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssignment([NotNull] GrammarParser.AssignmentContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="GrammarParser.parameters"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParameters([NotNull] GrammarParser.ParametersContext context);
}
} // namespace Happy_language
