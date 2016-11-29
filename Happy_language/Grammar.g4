grammar Grammar;
/*
 * Parser Rules
 */
 
start
	: Start_prog Start_blok  def_con_var def_functions main End_blok;

def_con_var
    : def_const def_con_var
	| def_var def_con_var
    | ;	// empty

def_const
	: Const Data_type_bool Identifier Assign Bool Semi
	| Const Data_type_int Identifier Assign Int Semi
	| Const Data_type_double Identifier Assign Double Semi;

def_var
	: Data_type_bool Identifier Assign Bool Semi
	| Data_type_int Identifier Assign Int Semi
	| Data_type_double Identifier Assign Double Semi
	| array_inicialization;


def_var_expression
	: Data_type_int Identifier Assign expression Semi
	| Data_type_double Identifier Assign expression Semi
	| 'ass' Identifier Assign expression Semi;

def_var_from_function
	: Data_type_bool Identifier Assign function_call Semi
	| Data_type_int Identifier Assign function_call Semi
	| Data_type_double Identifier Assign function_call Semi;

array_inicialization
    : Data_type_bool '[:' Int ':]' Identifier Semi
    | Data_type_int '[:' Int ':]' Identifier Semi
    | Data_type_double '[:' Int ':]' Identifier Semi;

function_call
	: Identifier Bracket_left par_in_function Bracket_right;

def_var_blok									// definovane promenné jen na zaèátku funkce
	: def_var def_var_blok
	| def_var_from_function def_var_blok
	| def_var_expression def_var_blok
    | array_inicialization def_var_blok
	| ;  // empty

par_in_function
	: Identifier
	| Identifier ',' par_in_function
	| Bool
	| Bool ',' par_in_function
	| Int
	| Int ',' par_in_function
	| Double
	| Double ',' par_in_function
	| ;  // empty
 

 def_functions
	: def_one_function def_functions
	| ;

def_one_function
	: Function_def function_return_data_typ Identifier Bracket_left parameters  Bracket_right Start_blok blok_function function_return End_blok;

function_return
	: Return Int Semi
	| Return Double Semi
	| Return Bool Semi
	| Return Identifier Semi
    | Return function_call Semi
    | Return expression Semi
	| ;


function_return_data_typ
	: data_type
	| Data_type_void;

data_type
	: Data_type_int
	| Data_type_bool
	| Data_type_double;

main
	: Main_name Bracket_left Bracket_right Start_blok blok_function End_blok;

blok_function
	: def_var_blok blok
	| ; // empty

blok
	: assignment Semi blok
    | assignment_array Semi blok
	| function_call Semi blok
	| if blok
	| while blok
    | do_while
	| for blok
	| ; // empty

if
	: If Bracket_left condition Bracket_right Start_blok blok End_blok else_if;

else_if
    : Else If Bracket_left condition Bracket_right Start_blok blok End_blok else_if
    | else;

else
	: Else Start_blok blok End_blok
	| ;  // empty

while
	: 'while' Bracket_left condition Bracket_right Start_blok blok End_blok;

do_while
    : 'do' Start_blok blok End_blok 'while'  Bracket_left condition Bracket_right Semi;

for
	: 'for'  Bracket_left for_condition Bracket_right Start_blok blok End_blok;

for_condition
	: assignment Semi condition Semi expression
	| assignment Semi condition Semi
	| Semi condition Semi expression
	| Semi Semi;

expression 
	: expression '+' expression_multiply			
    | expression '-' expression_multiply	
	| expression_multiply;

expression_multiply
    : expression_multiply '*' expression_item 
    | expression_multiply '/' expression_item 
    | expression_item;
 
expression_item
    : Int
    | Double
    | Identifier
    | arrry_index
    | function_call
    | '(' expression ')';

condition_item
	: Identifier
	| Int
	| Double
	| Bool
	| '(' Identifier ')'
	| '(' expression ')'
	| '(' Int ')'
	| '(' Double ')'
	| '(' Bool ')';

condition_expression
	: condition_item Operator_condition condition_item
	| Bool;

condition
	: condition_expression
	| condition_expression Logical_operator condition
	//| '!'condition TODO vymyslet jak v PL/0 udelat negaci
	| '('condition')'
	| '('condition')' Logical_operator condition;

arrry_index
    : Identifier '[:'Int':]';

assignment_array
    :  Assign function_call
    | arrry_index Assign Identifier
    | arrry_index Assign Bool
    | arrry_index Assign Int
    | arrry_index Assign Double
    | arrry_index Assign expression;

assignment
	: Identifier Assign function_call
	| Identifier Assign Identifier
	| Identifier Assign Bool
	| Identifier Assign Int
	| Identifier Assign Double
	| Identifier Assign expression
	| Identifier Assign condition;

parameters
	: data_type Identifier
	| data_type Identifier ',' parameters
	| ;	//empty


// ========================================================


/*
 * Lexer Rules
*/
 
Return: 'ret';
Comment: ':*' .*? '*:' -> skip;
Line_comment: ':**' ~[\r\n]* -> skip;
Semi: ';)';
Assign: ':=';
Bracket_left: '(:';
Bracket_right: ':)';
Data_type_void: ':V';
Data_type_bool: ':B';
Data_type_double: ':D';
Data_type_int: ':I';
Function_def: 'def';
Const: 'const';
If: 'if';
Else : 'else';
Operator_condition: '==' | '!=' | '<=' | '>=' | '>' | '<' ;
Logical_operator: '&&' | '||' ;
Start_prog: 'happy_start';					// zaèátek programu
Main_name: 'mainSmile';							
Bool: 'true'| 'false';
Start_blok: '{:';
End_blok: ':}';
Int : [+-]?[0-9]+;								// jedno èíslo
Double : [+-]?[0-9]+'.'[0-9]+;
Identifier: [a-zA-Z]+[a-zA-Z0-9]*;
WS :  (' '|'\t'| '\r' | '\n' ) + -> channel(HIDDEN)	 ;				// pøeskoèit na další býlí znak



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
