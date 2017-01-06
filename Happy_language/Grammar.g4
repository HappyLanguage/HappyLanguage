grammar Grammar;
/*
 * Parser Rules
 */
 
start
	: Start_prog Start_blok  def_con_var def_functions main End_blok;

//============== Var ==============
def_con_var
	: def_const def_con_var
	| def_var def_con_var
	| ;	// empty

def_const
	: Const Data_type_bool multiple_assign Assign condition_expression Semi
    | Const Data_type_bool multiple_assign Assign function_call Semi
	| Const Data_type_int multiple_assign Assign expression Semi
	| Const Data_type_int multiple_assign Assign ternary_operator Semi;
	
def_var
	: Data_type_bool multiple_assign Assign condition_expression Semi
	| Data_type_bool multiple_assign Assign function_call Semi
	| Data_type_int multiple_assign Assign expression Semi
	| Data_type_int multiple_assign Assign ternary_operator Semi
	| array_inicialization;


array_inicialization
	: Data_type_bool '[:' Int ':]' Identifier Semi
   	| Data_type_int '[:' Int ':]' Identifier Semi
	| Data_type_bool '[:'':]' Identifier Assign  Start_blok  bool_array_assign  End_blok Semi
	| Data_type_int '[:'':]' Identifier Assign  Start_blok number_array_assign  End_blok Semi
	| Data_type_bool '[:'':]' Identifier Assign  String  Semi
	| Data_type_int '[:'':]' Identifier Assign  String  Semi;

multiple_assign
	: Identifier ',' multiple_assign
	| Identifier;
	

bool_array_assign
	: condition_expression ',' bool_array_assign
	| function_call ',' bool_array_assign
	| condition_expression
	| function_call ;

number_array_assign
	: expression ',' number_array_assign
	| expression ;

//============== functions ==============

def_functions
	: def_one_function def_functions
	| ;

def_one_function
	: Function_def function_return_data_typ Identifier Bracket_left parameters  Bracket_right Start_blok blok_function function_return End_blok;

parameters
	: data_type Identifier
	| data_type Identifier ',' parameters
	| ;	//empty

blok_function
	: def_var_blok blok
	| ; // empty

blok
	: assignment Semi blok
	| assignment_array Semi blok
	| function_call Semi blok
	| if blok
	| while blok
	| do_while blok
	| for blok
	| ; // empty

def_var_blok
	: def_var def_var_blok
	| array_inicialization def_var_blok
	| ;  // empty

par_in_function
	: expression
	| expression ',' par_in_function
	| condition_expression
	| condition_expression ',' par_in_function
	| ;  // empty
 

function_return
	: Return condition_expression Semi
	| Return expression Semi
	| Return ternary_operator Semi
	| ;
	
function_call
	: Identifier Bracket_left par_in_function Bracket_right;

function_return_data_typ
	: data_type
	| Data_type_void;

main
	: Main_name Bracket_left Bracket_right Start_blok blok_function End_blok;
	
//============== Data type ==============

data_type
	: Data_type_int
	| Data_type_bool;


//============== (if, for, while..) ==============

if
	: If Bracket_left condition Bracket_right Start_blok blok End_blok else_if;

else_if
	: Else If Bracket_left condition Bracket_right Start_blok blok End_blok else_if
	| else;

else
	: Else Start_blok blok End_blok
	| ;  // empty

while
	: While Bracket_left condition Bracket_right Start_blok blok End_blok;

do_while
	: Do Start_blok blok End_blok While Bracket_left condition Bracket_right Semi;

for
	: For Bracket_left for_condition Bracket_right Start_blok blok End_blok;

for_condition
	: one_assignment Semi condition Semi increment
	| Semi condition Semi increment
	| Semi Semi increment;

increment
	: one_assignment
	|;//empty

//============== expression ==============

expression 
	: expression Add expression_multiply			
	| expression Sub expression_multiply	
	| expression_multiply;

expression_multiply
	: expression_multiply Mul expression_item 
	| expression_multiply Div expression_item
	| expression_item;
 
expression_item
	: Int
	| Identifier
	| function_call
	| array_index
	| '(' expression ')'		
	| Add Int	
	| Sub Int
	| Sub Identifier
	| Add Identifier
	| Sub array_index
	| Add array_index
	| Add function_call
	| Sub function_call;

condition_item
	: Bool
	| Negation Bool
	| expression
	| '(' Bool ')'
	| Negation '(' Bool ')';

condition_expression
	: condition_item Operator_condition condition_item
	| Bool;

condition
	: condition_expression
	| condition_expression Logical_operator condition
	| '('condition')'
	| Negation '('condition')'
	| '('condition')' Logical_operator condition
	| Negation '('condition')' Logical_operator condition;

array_index
	: Identifier '[:' index ':]';

index
	: Int
	| expression
	/*| ternary_operator*/;


ternary_operator
	: condition '?' expression ':' expression;

assignment_array
	: array_index Assign condition_expression
	| array_index Assign expression
	| array_index Assign ternary_operator;

one_assignment
	: Identifier Assign expression
	| Identifier Assign condition_expression
	| Identifier Assign condition
	| Identifier Assign ternary_operator;


assignment
	: multiple_assign Assign expression
	| multiple_assign Assign condition_expression
	| multiple_assign Assign condition
	| multiple_assign Assign ternary_operator;



// ========================================================


/*
 * Lexer Rules
*/

Add: '+';
Sub: '-';
Mul: '*';
Div: '/';
Return: 'ret';
Comment: ':*' .*? '*:' -> skip;
Line_comment: '://' ~[\r\n]* -> skip;
Semi: ';)';
Assign: ':=';
Bracket_left: '(:';
Bracket_right: ':)';
Data_type_void: ':V';
Data_type_bool: ':B';
Data_type_int: ':I';
Function_def: 'def';
Const: 'const';
If: 'if';
While: 'while';
Do: 'do';
For: 'for';
Else : 'else';
Operator_condition: '==' | '!=' | '<=' | '>=' | '>' | '<' ;
Logical_operator: '&&' | '||' ;
Negation: '!';
Start_prog: 'happy_start';					
Main_name: 'mainSmile';							
Bool: 'true'| 'false';
Start_blok: '{:';
End_blok: ':}';
Int : [0-9]+;								
Identifier: [a-zA-Z]+[a-zA-Z0-9]*;
String: '\'' ( '$' '\''? | ~('$' | '\'') )* '\'';
WS :  (' '|'\t'| '\r' | '\n' ) + -> channel(HIDDEN)	 ;				



/**
The Language Processing is done in two strictly separated phases:

Lexing, i.e. partitioning the text into tokens
Parsing, i.e. building a parse tree from the tokens
Since lexing must preceed parsing there is a consequence: The lexer is independent of the parser, the parser cannot influence lexing.

Lexing

Lexing in ANTLR works as following:

all rules with uppercase first character are lexer rules
the lexer starts at the beginning and tries to find a rule that matches best to the current input
a best match is a match that has maximum length, i.e. if then next input character is append to the maximum length match, the result is not matched by any lexer rule
tokens are generated from matches:
if one rule matches the maximum length match the corresponding token is pushed into the token stream
if multiple rules match the maximum length match the first defined token in the grammar is pushed to the token stream
Example: What is wrong with your grammar

Your grammar has two rules that are critical:

FILEPATH: ('A'..'Z'|'a'..'z'|'0'..'9'|':'|'\\'|'/'|' '|'-'|'_'|'.')+ ;
TITLE: ('A'..'Z'|'a'..'z'|' ')+ ;
Each match, that is matched by TITLE will also be matched by FILEPATH. And FILEPATH is defined before TITLE: So each token that you expect to be a title would be a FILEPATH.

There are two hints for that:

keep your lexer rules disjunct (no token should match a superset of another)
if your tokens intentionally match the same strings, than put them into the right order (in your case this will be sufficient).
if you need a parser driven lexer you have to change to another parser generator: PEG-Parsers or GLR-Parsers will do that (but of course this can produce other problems)

*/
