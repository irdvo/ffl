\ ==============================================================================
\
\           rgx - the regular expression module in the ffl
\
\               Copyright (C) 2007  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public
\ License as published by the Free Software Foundation; either
\ version 2 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ General Public License for more details.
\
\ You should have received a copy of the GNU General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2007-05-13 05:30:41 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] rgx.version [IF]


include ffl/nfe.fs


( rgx = Regular expression )
( The rgx module implements words for compiling and matching regular         )
( expressions. ToDo: syntax rules                                            )


1 constant rgx.version


( Regular expression structure )

struct: rgx%       ( - n = Get the required space for the rgx data structure )
  cell: rgx>pattern      \ the pattern during scanning
  cell: rgx>length       \ the length of the pattern during scanning
  cell: rgx>index        \ the index in the pattern during scanning
  cell: rgx>expression   \ the non-deterministic finite automata expression after parsing
  cell: rgx>visit        \ the [unique] visit number
;struct


( Private scanner types )

100 constant rgx.eos          ( - n = End of pattern )
101 constant rgx.alternation  ( - n = Alternation [|] )
102 constant rgx.zero-or-one  ( - n = Zero or one [?] )
103 constant rgx.zero-or-more ( - n = Zero or more [*] )
104 constant rgx.one-or-more  ( - n = One or more [+] )


( Regular expression creation, initialisation and destruction )

: rgx-init     ( w:rgx - = Initialise the regular expression )
  dup rgx>pattern    nil!
  dup rgx>length       0!
  dup rgx>index        0!
  dup rgx>expression nil!
      rgx>visit        0!
;


: rgx-create   ( C: "name" - R: - w:rgx = Create a named regular expression in the dictionary )
  create   here   rgx% allot   rgx-init
;


: rgx-new   ( w:data n:type - w:rgx = Create a new regular expression on the heap )
  rgx% allocate  throw  dup rgx-init
;


: rgx-free   ( w:rgx - = Free the regular expression from the heap )
  dup rgx>expression @ nfe+free   \ free the stored expression
  
  free  throw
;


( Private member words )

: rgx-visit@   ( w:rgx - n = Get the [unique] visit number )
  rgx>visit dup
  1+! @
;


( Private scanner words )

: rgx-scan-init  ( c-addr u w:rgx - = Initialise the regular expression scanner )
  tuck rgx>length   !
  tuck rgx>pattern  !
       rgx>index   0!
;


: rgx-scan-token  ( w:rgx - w:data w:token = Scan the pattern for the current token )
  dup rgx>index @  over rgx>length @ < IF
    dup rgx>index @ chars swap rgx>pattern @ + c@
    CASE
      [char] . OF nil nfs.any          ENDOF
      [char] | OF nil rgx.alternation  ENDOF
      [char] ? OF nil rgx.zero-or-one  ENDOF
      [char] * OF nil rgx.zero-or-more ENDOF
      [char] + OF nil rgx.one-or-more  ENDOF
      [char] ( OF nil nfs.lparen       ENDOF
      [char] ) OF nil nfs.rparen       ENDOF
      nfs.char over
    ENDCASE
  ELSE
    drop 
    nil rgx.eos
  THEN
;


: rgx-scan-next  ( w:rgx - = Move the scanner to the next token )
  rgx>index 1+!
;


( Private parser words )

defer rgx.parse-alternation

: rgx-parse-single   ( w:rgx - expr true | false = Parse a single token )
  >r
  r@ rgx-scan-token                     \ Scan the current token
  dup nfs.lparen = IF                   \ If token = ( Then
    2drop  
    r@ rgx-scan-next                    \   Move to next token
    r@ rgx.parse-alternation execute IF \   If an alternation expression is parsed Then
      r@ rgx-scan-token nip
      nfs.rparen = IF                   \     If current token = ) Then
        nfe+paren                       \      Paren the expression
        r@ rgx-scan-next                \      Move to the next token
        true
      ELSE                              \     Else (error)
        nip nfe+free                    \       Free the expression
        false
      THEN
    ELSE                                \   Else (error)
      false
    THEN
  ELSE                                  \ Else
    dup nfs.char = over nfs.any = OR IF \   If token = character or . Then
      nfe+single                        \     Create single expression
      r@ rgx-scan-next                  \     Move to the next token
      true
    ELSE                                \   Else (error)
      2drop 
      false
    THEN
  THEN
  rdrop
;


: rgx-parse-repeat   ( w:rgx - expr true | false = Parse a repeat token )
  >r
  r@ rgx-parse-single IF          \ If a single expression is parsed Then
    BEGIN
      r@ rgx-scan-token nip       \   Scan the current token
      CASE
        rgx.zero-or-one OF        \   If the token is ? Then
          nfe+zero-or-one         \     Change the expression 
          r@ rgx-scan-next        \     Move to the next token
          false                   \     Continu scanning
          ENDOF
        rgx.zero-or-more OF
          nfe+zero-or-more 
          r@ rgx-scan-next
          false
          ENDOF
        rgx.one-or-more OF
          nfe+one-or-more
          r@ rgx-scan-next
          false
          ENDOF                   \ Else
        true swap                 \   Done, no more repeat operators
      ENDCASE
    UNTIL
    true
  ELSE
    false
  THEN
  rdrop
;


: rgx-parse-concat   ( w:rgx - expr true | false = Parse a concatenation of repeat tokens )
  >r
  r@ rgx-parse-repeat IF          \ If a repeat expression is parsed Then
    BEGIN
      r@ rgx-parse-repeat         \   While a second repeat expression is parsed Do
    WHILE
      nfe+concat                  \     Concat the expressions
    REPEAT
    true
  ELSE
    false
  THEN
  rdrop
;


: rgx-parse-alternation   ( w:rgx - expr true | false = Parse an alternation of two expressions )
  >r
  r@ rgx-parse-concat IF          \ If a concatted expression is parsed Then
    true 
    BEGIN                         \   While ok and current token = | Do
      dup r@ rgx-scan-token nip rgx.alternation = AND
    WHILE
      drop 
      r@ rgx-scan-next            \     Move to next token
      r@ rgx-parse-concat IF      \     If a concatted expression is parsed Then
        nfe+alternation           \       Put the two expressions as alternation
        true
      ELSE                        \     Else (error)
        nip nfe+free              \       Free the expression
        false
      THEN
    REPEAT
  ELSE
    false
  THEN
  rdrop
;

' rgx-parse-alternation is rgx.parse-alternation


( Regular expression words )

: rgx-compile  ( c-addr u w:rgx - true | n:index false = Compile a pattern as regular expression)
  >r
  nil r@ rgx>expression @! nfe+free   \ Free the current expression
  
  r@ rgx-scan-init                    \ Initialise the scanner
  
  r@ rgx-parse-alternation IF         \ If an expression is parsed Then
    r@ rgx-scan-token rgx.eos = IF    \   If the expression ends Then
      drop
      nfe+paren                       \     Paren the full expression
      nfe+close                       \     Add the match state
      
      r@ rgx>expression !             \     Save it for matching
      true
    ELSE                              \   Else
      nip nfe+free                    \     Cleanup, error
      r@ rgx>index @
      false
    THEN
  ELSE                                \ Else
    r@ rgx>index @                    \   Error
    false
  THEN
  rdrop
;


: rgx-match  ( c-addr u w:rgx - f = Match case sensitive a string with the regular expression )
  \ ToDo
;


: rgx-imatch  ( c-addr u w:rgx - f = Match case insensitive a string with the regular expression )
  \ ToDo
;


( Inspection )

: rgx-dump     ( w:rgx - = Dump the regular expression )
  ." rgx:" dup . cr
  ."  pattern   :" dup rgx>pattern ? cr
  ."  length    :" dup rgx>length  ? cr
  ."  index     :" dup rgx>index   ? cr
  ."  expression:" dup rgx-visit@ swap rgx>expression @ nfe+dump
;

[THEN]

\ ==============================================================================
 
