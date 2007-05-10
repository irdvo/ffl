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
\  $Date: 2007-05-10 19:35:18 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] rgx.version [IF]


include ffl/nfe.fs


( rgx = Regular expression )
( The rgx module implements words for using regular expressions.   )


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

100 constant rgx.eos          ( - n = End of pattern reached )
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


( Member words )

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

nil value rgx.parse-alternation

: rgx-parse-single   ( w:rgx - expr true | false = Parse a single token )
  >r
  r@ rgx-scan-token
  dup nfs.lparen = IF
    2drop  r@ rgx-scan-next
    rgx.parse-alternation execute IF
      r@ rgx-scan-token
      dup nfs.rparen = IF
        2drop  r@ rgx-scan-next
        nfe+paren
        true
      ELSE
        nfe+free
        false
      THEN
    ELSE
      false
    THEN
  ELSE
    dup nfs.char = IF
      nfe+single true
      r@ rgx-scan-next
    ELSE
      dup nfs.any = IF
        nfe+single true
        r@ rgx-scan-next
      ELSE
        2drop false
      THEN
    THEN
  THEN
  rdrop
;


: rgx-parse-repeat   ( w:rgx - expr true | false = Parse a repeat token )
  >r
  r@ rgx-parse-single IF
    BEGIN                          \ until
      r@ rgx-scan-token
      dup rgx.zero-or-one = IF
        2drop
        r@ rgx-scan-next
        nfe+zero-or-one 
        true
      ELSE
        dup rgx.zero-or-more = IF
          2drop  
          r@ rgx-scan-next
          nfe+zero-or-more 
          true
        ELSE
          dup rgx.one-or-more = IF
            2drop
            r@ rgx-scan-next
            nfe+one-or-more
            true
          ELSE
            2drop 
            false
          THEN
        THEN
      THEN
    WHILE
    REPEAT
    true
  ELSE
    false
  THEN
  rdrop
;


: rgx-parse-concat   ( w:rgx - expr true | false = Parse a concatenation of repeat tokens )
  >r
  r@ rgx-parse-repeat IF
    BEGIN
      r@ rgx-parse-repeat
    WHILE
      nfe+concat
    REPEAT
    true
  ELSE
    false
  THEN
  rdrop
;


: rgx-parse-alternation   ( w:rgx - expr true | false = Parse an alternation of two expressions )
  >r
  r@ rgx-parse-concat IF
    true
    BEGIN
      dup r@ rgx-scan-token rgx.alternation = AND
    WHILE
      drop 
      r@ rgx-scan-next
      r@ rgx-parse-concat IF
        nfe+alternation
      ELSE
        0=
      THEN
    REPEAT
    \ ToDo: false = nfe-free
  ELSE
    false
  THEN
  rdrop
;

' rgx-parse-alternation to rgx.parse-alternation


( Regular expression words )

: rgx-compile  ( c-addr u w:rgx - true | n false = Compile a pattern as regular expression )
  >r
  nil r@ rgx>expression @! nfe+free   \ Free the current expression
  
  r@ rgx-scan-init                    \ Initialise the scanner
  
  \ ToDo
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
 
