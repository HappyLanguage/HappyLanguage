grammar Grammar;
/*
 * Parser Rules
 */
 
start
	: Start_prog Start_blok  def_con_var def_function main End_blok;

def_con_var
    : const_and_var def_con_var
    | ;	// empty

const_and_var
	: def_var
	| Const Data_type_bool Identifier Assign Bool Semi
	| Const Data_type_int Identifier Assign Int Semi
	| Const Data_type_double Identifier Assign Double Semi
	| Const Data_type_double Identifier Assign Int Semi;

def_var
	: Data_type_bool Identifier Assign Bool Semi
	| Data_type_int Identifier Assign Int Semi
	| Data_type_double Identifier Assign Int Semi
	| Data_type_double Identifier Assign Double  Semi;


def_var_expression
	: Data_type_int Identifier Assign expression Semi
	| Data_type_double Identifier Assign expression Semi
	| Identifier Assign expression Semi;

def_var_from_function
	: Data_type_bool Identifier Assign function_call
	| Data_type_int Identifier Assign function_call
	| Data_type_double Identifier Assign function_call;

function_call
	: Identifier Bracket_left par_in_function Bracket_right Semi;

def_var_blok									// definovane promenné jen na zaèátku funkce
	: def_var def_var_blok
	| def_var_from_function def_var_blok
	| def_var_expression def_var_blok
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
 

def_function
	: Function_def function_return_data_typ Identifier Bracket_left parametrs  Bracket_right Start_blok blok_function End_blok def_function
	| ; // empty


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
	: assignment blok
	| if blok
	| while blok
	| for blok
	| ; // empty

if
	: If Bracket_left condition Bracket_right Start_blok blok End_blok else;
else
	: Else Start_blok blok End_blok
	| ;  // empty

while
	: 'while' Bracket_left condition Bracket_right Start_blok blok End_blok;

for
	: 'for'  Bracket_left for_condition Bracket_right Start_blok blok End_blok;

for_condition
	: assignment condition Semi expression
	| assignment condition Semi;

expression 
	:	expression '+' expression			
    |   expression '-' expression	
	|   expression '/' expression			
	|   expression '*' expression					
    |   Int											
	|   Double										
    |   Identifier									
    |   '(' expression ')';  
     
condition_item
	:Identifier
	| Int
	| Double
	| Bool
	| '(' Identifier ')'
	| '(' Int ')'
	| '(' Double ')'
	| '(' Bool ')';

condition_expression
	: condition_item Operator_condition condition_item
	;
condition
	: condition_expression
	| condition_expression '||' condition
	| condition_expression '&&' condition
	| '('condition')' 
	| '('condition')' '&&' condition
	| '('condition')' '||' condition
	| Bool ;


assignment
	: Identifier Assign function_call
	| Identifier Assign Identifier Semi
	| Identifier Assign Bool Semi
	| Identifier Assign Int Semi
	| Identifier Assign Double Semi
	| Identifier Assign expression Semi;

parametrs
	: data_type Identifier
	| data_type Identifier ',' parametrs
	| ;	//empty


// ========================================================


/*
 * Lexer Rules
*/
 
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
