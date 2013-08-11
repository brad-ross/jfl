grammar JFL;

options {
language='CSharp3';
output=AST;
}

tokens {
BLOCK;
FILTER;
EXPR;
}

@rulecatch {
	catch (RecognitionException e) {
		throw e;
	}
}

public jfl	:	block
	;
	
block	:	'{' properties? '}' -> ^(BLOCK properties?)
	;
	
properties
	:	property (','! property)*
	;
	
property 
	:	'!'? propertyName filter? (':' block)? -> ^(propertyName '!'? filter? block?)
	;

propertyName 
	:	ID
	|	ESC_ID
	|	REGEX
	|	'*'
	;
	
filter	:	'[' filterExpr ']' -> ^(FILTER filterExpr)
	;

filterExpr
	:	(comparison -> comparison) (CHAIN_OPERATOR comp=comparison -> ^(CHAIN_OPERATOR $filterExpr $comp))*
	;
	
comparison
	:	(atom -> atom) (COMPARATOR a=atom -> ^(COMPARATOR $comparison $a))*
	;

atom
	:	propertyChain'?'?  -> ^(EXPR  propertyChain '?'?)
	|	'!' propertyChain -> ^(EXPR propertyChain '!')
	|	STRING
	|	REGEX
	|	NUMBER
	|	TRUE
	|	FALSE
	|	NULL
	|	'!'? '(' filterExpr ')' -> ^(EXPR filterExpr '!'?)
	;
	
propertyChain
	:	(ID | ESC_ID) ^ ('.'! (ID | ESC_ID))*
	;

CHAIN_OPERATOR
	:	'&'
	|	'|'
	;

COMPARATOR
	:	'='
	|	'!='
	|	'>'
	|	'<'
	|	'>='
	|	'<='
	;
	
TRUE
	:	'true'
	;
	
FALSE
	:	'false'
	;
	
NULL
	:	'null'
	;
	
REGEX
	:	'/' .* '/'
	;
	
NUMBER
	:	'-'? DIGIT+ ( '.' DIGIT+)? EXPONENT?
	;

fragment
DIGIT
	 :	'0'..'9'
	 ;	
	
fragment 
EXPONENT: ('e'|'E') '-'? DIGIT+;

ID  :	('a'..'z'|'A'..'Z'|'_'|'-')+
    ;
    
ESC_ID :
	'\'' ESC_STRING '\''
	;
	
STRING  :
    '"' ESC_STRING '"'
    ;
    
fragment
ESC_STRING
	:	( ESCAPE_SEQ | ~('\u0000'..'\u001f' | '\\' | '\"' ) )*
	;
 
WS: (' '|'\n'|'\r'|'\t')+ { $channel=Hidden; } ;
 
fragment
ESCAPE_SEQ
        :   '\\' (UNICODE_ESC |'b'|'t'|'n'|'f'|'r'|'\"'|'\''|'\\')
        ;
 
fragment UNICODE_ESC
    : 'u' HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
    ;
 
fragment HEX_DIGIT
    : '0'..'9' | 'A'..'F' | 'a'..'f'
    ;