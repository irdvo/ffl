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
\  $Date: 2007-06-02 05:59:48 $ $Revision: 1.11 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] rgx.version [IF]


include ffl/nfe.fs


( rgx = Regular expressions )
( The rgx module implements words for compiling and matching regular         )
( expressions. The module uses the nfe module for the actual expression      )
( building and matching.                                                     )
( <pre>                                                                      )
(                                                                            )
(     This module uses the following syntax:                                 )
(      .   Match any char [incl. newline]     *   Match zero or more         )
(      +   Match one or more                  ?   Match zero or one          )
(      |   Match alternatives                                                )
(          Group or subexpression                                            )
(                                                                            )
(     Backslash characters:                                                  )
(      \.  Character .                       \*   Character *                )
(      \+  Character +                       \?   Character ?                )
(      \|  Character |                       \\   Backslash                  )
(                                                                            )
(      \r  Carriage return                   \n   Line feed                  )
(      \t  Horizontal tab                    \e   Escape                     )
(                                                                            )
(      \d  Digits class: [0-9]               \D   No digits: [^0-9]          )
(      \w  Word class: [0-9a-zA-Z_]          \W   No word: [^0-9a-zA-Z_]     )
(      \s  Whitespace                        \S   No whitespace              )
(                                                                            )
(      All other backslash characters simply return the trailing character,  )
(      but this can change in future versions.                               )
( </pre> )


1 constant rgx.version


( Regular expression structure )

struct: rgx%       ( - n = Get the required space for the rgx data structure )
  nfe% field: rgx>nfe       \ the regular expression is a non-deterministic finite automate expression
       cell:  rgx>pattern   \ the pattern during scanning
       cell:  rgx>length    \ the length of the pattern during scanning
       cell:  rgx>next      \ the length of the last scanned token
       cell:  rgx>index     \ the index in the pattern during scanning
;struct


( Private scanner types )

100 constant rgx.eos          ( - n = End of pattern )
101 constant rgx.alternation  ( - n = Alternation [|] )
102 constant rgx.zero-or-one  ( - n = Zero or one [?] )
103 constant rgx.zero-or-more ( - n = Zero or more [*] )
104 constant rgx.one-or-more  ( - n = One or more [+] )


( Regular expression creation, initialisation and destruction )

: rgx-init     ( w:rgx - = Initialise the regular expression )
  dup nfe-init              \ Initialise the base expression
  dup rgx>pattern    nil!
  dup rgx>length       0!
  dup rgx>next         0!
      rgx>index        0!
;


: rgx-create   ( C: "name" - R: - w:rgx = Create a named regular expression in the dictionary )
  create   here   rgx% allot   rgx-init
;


: rgx-new   ( w:data n:type - w:rgx = Create a new regular expression on the heap )
  rgx% allocate  throw  dup rgx-init
;


: rgx-free   ( w:rgx - = Free the regular expression from the heap )
  nfe-free
;


( Private scanner words )

: rgx-scan-init  ( c-addr u w:rgx - = Initialise the regular expression scanner )
  tuck rgx>length   !
  tuck rgx>pattern  !
       rgx>index   0!
;


: rgx-scan-backslash   ( w:rgx - w:data w:token = Scan for a backslash token )
  >r
  r@ rgx>index @ 1+ dup  r@ rgx>length @ < IF   \ If there one more character Then
    chars r@ rgx>pattern @ + c@                 \   Fetch the char
    r@ rgx>next 1+!                             \   Process two characters
    CASE
      [char] n OF chr.lf                                  nfs.char  ENDOF
      [char] r OF chr.cr                                  nfs.char  ENDOF
      [char] t OF chr.ht                                  nfs.char  ENDOF
      [char] e OF chr.esc                                 nfs.char  ENDOF
      [char] d OF chs-new dup             chs-set-digit   nfs.class ENDOF
      [char] D OF chs-new dup chs-set dup chs-reset-digit nfs.class ENDOF
      [char] w OF chs-new dup             chs-set-word    nfs.class ENDOF
      [char] W OF chs-new dup chs-set dup chs-reset-word  nfs.class ENDOF
      [char] s OF chs-new dup             chs-set-space   nfs.class ENDOF
      [char] S of chs-new dup chs-set dup chs-reset-space nfs.class ENDOF
      nfs.char over
    ENDCASE
  ELSE
    drop
    nil rgx.eos
  THEN
  rdrop
;


: rgx-scan-token  ( w:rgx - w:data w:token = Scan the pattern for the current token )
  >r
  r@ rgx>index @ dup  r@ rgx>length @ < IF   \ If there is still a character Then
    chars  r@ rgx>pattern @ + c@
    1 r@ rgx>next !
    CASE
      [char] . OF nil nfs.any           ENDOF
      [char] | OF nil rgx.alternation   ENDOF
      [char] ? OF nil rgx.zero-or-one   ENDOF
      [char] * OF nil rgx.zero-or-more  ENDOF
      [char] + OF nil rgx.one-or-more   ENDOF
      [char] ( OF nil nfs.lparen        ENDOF
      [char] ) OF nil nfs.rparen        ENDOF
      [char] \ OF r@ rgx-scan-backslash ENDOF
      nfs.char over
    ENDCASE
  ELSE
    drop
    r@ rgx>next 0!
    nil rgx.eos
  THEN
  rdrop
;


: rgx-scan-next  ( w:rgx - = Move the scanner to the next token )
  dup rgx>next @ swap rgx>index +!
;


( Private parser words )

: rgx-cleanup   ( w:outs w:start - = Cleanup error expression )
  swap nil swap nfe+resolve             \ Resolve the open outs to nil
  nfe+free-expression                   \ Free the expression
;


defer rgx.parse-alternation

: rgx-parse-single   ( w:rgx - expr true | false = Parse a single token )
  >r
  r@ rgx-scan-token                     \ Scan the current token
  dup nfs.lparen = IF                   \ If token = ( Then
    2drop  
    r@ rgx>index @                      \   Save scanner state for error recovery
    r@ nfe-level+@                      \   Get the paren level
    r@ rgx-scan-next                    \   Move to next token
    r@ rgx.parse-alternation IF         \   If an alternation expression is parsed Then
      r@ rgx-scan-token nip
      nfs.rparen = IF                   \     If current token = ) Then
        rot r@ nfe-paren                \      Paren the expression with the paren level
        r@ rgx-scan-next                \      Move to the next token
        rot drop                        \      Remove scanner state
        true
      ELSE                              \     Else (error)
        rgx-cleanup                     \       Cleanup
        drop
        r@ rgx>index !                  \       Restore the scanner state
        false
      THEN
    ELSE                                \   Else (error)
      drop
      r@ rgx>index !                    \     Restore the scanner state
      false
    THEN
  ELSE                                  \ Else
    dup  nfs.char = 
    over nfs.any  = OR 
    over nfs.class = OR IF              \ If token = character or . or class Then
      r@ nfe-single                     \     Create single expression
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
          r@ nfe-zero-or-one      \     Change the expression 
          r@ rgx-scan-next        \     Move to the next token
          false                   \     Continu scanning
          ENDOF
        rgx.zero-or-more OF
          r@ nfe-zero-or-more 
          r@ rgx-scan-next
          false
          ENDOF
        rgx.one-or-more OF
          r@ nfe-one-or-more
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
      r@ nfe-concat               \     Concat the expressions
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
        r@ nfe-alternation        \       Put the two expressions as alternation
        true
      ELSE                        \     Else (error)
        rgx-cleanup               \       Cleanup
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
  r@ nfe-clear                        \ Free the current expression
  
  r@ rgx-scan-init                    \ Initialise the scanner
  
  r@ rgx-parse-alternation IF         \ If an expression is parsed Then
    r@ rgx-scan-token nip 
    rgx.eos = IF                      \   If the expression ends Then
      r@ nfe-close                    \     Close the expression: match state, paren and storing
      true
    ELSE                              \   Else (error)
      rgx-cleanup                     \     Cleanup
      r@ rgx>index @
      false
    THEN
  ELSE                                \ Else
    r@ rgx>index @                    \   Error
    false
  THEN
  rdrop
;


: rgx-cmatch?   ( c-addr u w:rgx - f = Match case-sensitive a string with the regular expression )
  false swap nfe-match?
;


: rgx-imatch?   ( c-addr u w:rgx - f = Match case-insensitive a string with the regular expression )
  true swap nfe-match?
;


: rgx-csearch   ( c-addr u w:rgx - n:index = Search case-sensitive in a string for the first match of the regular expression )
  false swap nfe-search
;


: rgx-isearch   ( c-addr u w:rgx - n:index = Search case-insensitive in a string for the first match of the regular expression )
  true swap nfe-search
;


: rgx-result   ( n:index w:rgx - n:ep n:sp = Get the match result of the indexth grouping )
  nfe-result
;


( Inspection )

: rgx-dump     ( w:rgx - = Dump the regular expression )
  dup nfe-dump
  ." rgx:" dup . cr
  ."  pattern   :" dup rgx>pattern ? cr
  ."  length    :" dup rgx>length  ? cr
  ."  index     :"     rgx>index   ? cr
;

[THEN]

\ ==============================================================================
 
