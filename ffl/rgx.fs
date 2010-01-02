\ ==============================================================================
\
\           rgx - the regular expression module in the ffl
\
\            Copyright (C) 2007-2008  Dick van Oudheusden
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
\  $Date: 2008-10-07 16:45:41 $ $Revision: 1.17 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] rgx.version [IF]


include ffl/nfe.fs


( rgx = Regular expressions )
( The rgx module implements regular expressions. It supports words for       )
( compiling and matching regular expressions. The module uses the [nfe]      )
( module for the actual expression building and matching.                    )
( {{{                                                                        )
(                                                                            )
(     This module uses the following syntax:                                 )
(      .   Match any char [incl. newline]     *   Match zero or more         )
(      +   Match one or more                  ?   Match zero or one          )
(      |   Match alternatives                 []  Class                      )
(      &lb;&rb;  Group or subexpression                                      )
(                                                                            )
(     Backslash characters:                                                  )
(      \.  Character .                       \*   Character *                )
(      \+  Character +                       \?   Character ?                )
(      \|  Character |                       \\   Backslash                  )
(      \[  Character [                                                       )
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
(                                                                            )
(      Classes:                                                              )
(       [abc]  - match a or b or c                                           )
(       [^abc] - match everything except a or b or c                         )
(       [a-z]  - match a or b or .. z                                        )
(       [-abc] - match - or a or b or c                                      )
(       []abc] - match ] or a or b or c                                      )
(       [\d\n] - match digit or line feed                                    )
(                                                                            )
(      Backslash characters in classes:                                      )
(       \r  Carriage return                \n    Line feed                   )
(       \t  Horizontal tab                 \e    Escape                      )
(       \]  Character ]                    \-    Character -                 )
(       \d  Digits class: [0-9]            \w    Word class: [0-9a-zA-Z_]    )
(       \s  Whitespace                                                       )
(                                                                            )
(      All other backslash characters simply return the trailing character,  )
(      but this can change in future versions.                               )
( }}} )


2 constant rgx.version


( Regular expression structure )

begin-structure rgx%     ( -- n = Get the required space for a rgx variable )
  nfe% 
  +field rgx>nfe       \ the regular expression is a non-deterministic finite automate expression
  field: rgx>pattern   \ the pattern during scanning
  field: rgx>length    \ the length of the pattern during scanning
  field: rgx>next      \ the length of the last scanned token
  field: rgx>index     \ the index in the pattern during scanning
end-structure


( Private scanner types )

-1  constant rgx.error        ( -- n = Error in pattern )
100 constant rgx.eos          ( -- n = End of pattern )
101 constant rgx.alternation  ( -- n = Alternation [|] )
102 constant rgx.zero-or-one  ( -- n = Zero or one [?] )
103 constant rgx.zero-or-more ( -- n = Zero or more [*] )
104 constant rgx.one-or-more  ( -- n = One or more [+] )


( Regular expression creation, initialisation and destruction )

: rgx-init     ( rgx -- = Initialise the regular expression )
  dup nfe-init              \ Initialise the base expression
  dup rgx>pattern    nil!
  dup rgx>length       0!
  dup rgx>next         0!
      rgx>index        0!
;


: rgx-create   ( "<spaces>name" -- ; -- rgx = Create a named regular expression in the dictionary )
  create   here   rgx% allot   rgx-init
;


: rgx-new   ( -- rgx = Create a new regular expression on the heap )
  rgx% allocate  throw  dup rgx-init
;


: rgx-free   ( rgx -- = Free the regular expression from the heap )
  dup nfe-(free)
  
  free throw
;


( Private scanner words )

: rgx-scan-init    ( c-addr u rgx -- = Initialise the regular expression scanner )
  tuck rgx>length   !
  tuck rgx>pattern  !
       rgx>index   0!
;


: rgx-scan-char    ( rgx -- char true | false = Get a character from the pattern buffer )
  dup  rgx>index @ over rgx>next @ +
  tuck over rgx>length @ < IF           \ If index < length Then
    rgx>pattern @ swap chars + c@ true  \   Return the character
  ELSE
    2drop false
  THEN
;


: rgx-scan-incr    ( rgx -- = Increment the next pointer )
  rgx>next 1+!
;


: rgx-scan-class-backslash  ( chs rgx -- chs = Scan the backslash character in the class )
  >r
  r@ rgx-scan-char IF
    CASE
      [char] n OF chr.lf  over chs-set-char  ENDOF
      [char] r OF chr.cr  over chs-set-char  ENDOF
      [char] t OF chr.ht  over chs-set-char  ENDOF
      [char] e OF chr.esc over chs-set-char  ENDOF
      [char] d OF         dup  chs-set-digit ENDOF
      [char] w OF         dup  chs-set-word  ENDOF
      [char] s OF         dup  chs-set-space ENDOF
      2dup swap chs-set-char                        \ All others
    ENDCASE
    r@ rgx-scan-incr
  THEN
  rdrop
;


: rgx-scan-class-range  ( chs char rgx -- chs = Scan the range character in the class )
  >r
  ?dup IF
    over
    r@ rgx-scan-char IF
      dup [char] \ <> IF          \ No range with backslash characters
        swap chs-set-chars
        r@ rgx-scan-incr
      ELSE
        2drop drop
      THEN
    ELSE
      2drop
    THEN
  THEN
  rdrop
;


: rgx-scan-class   ( rgx -- x n = Scan for a class, return the type n and the data x for the expression )
  >r
  false chs-new 0                 \ S: invert class last-char
  
  r@ rgx-scan-char IF             \ Check the first special char after the [: ^,] and -
    dup [char] ] = over [char] - = OR IF
      nip 2dup swap chs-set-char
      r@ rgx-scan-incr
    ELSE [char] ^ = IF
      2>r 0= 2r>
      r@ rgx-scan-incr
    THEN THEN
  THEN

  BEGIN                           \ Process all characters in the class
    r@ rgx-scan-char
  WHILE
    r@ rgx-scan-incr

    dup [char] ] = IF             \ If ']' Then done
      2drop
      swap IF                     \   Invert the class if there was a '^'
        dup chs-invert
      THEN
      nfs.class
      rdrop
      EXIT
    ELSE dup [char] \ = IF        \ If '\' Then do backslash character
      2drop
      r@ rgx-scan-class-backslash
      0                           \   No range 
    ELSE dup [char] - = IF
      drop
      r@ rgx-scan-class-range      \ If '-' Then range character
      0
    ELSE
      nip 2dup swap chs-set-char  \ All others: add to the set
    THEN THEN THEN
  REPEAT
  drop chs-free drop              \ Error: no ] found
  r> rgx>next 0!
  nil rgx.error
;


: rgx-scan-backslash   ( rgx -- x n = Scan for a backslash token, return the type n and data x for the expression )
  >r
  r@ rgx-scan-char IF                            \ If there one more character Then
    r@ rgx-scan-incr                             \   Process the (two) characters
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


: rgx-scan-token  ( rgx -- x n = Scan the pattern for the expression, return the type n and data x )
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
      [char] [ OF r@ rgx-scan-class     ENDOF
      nfs.char over
    ENDCASE
  ELSE
    drop
    r@ rgx>next 0!
    nil rgx.eos
  THEN
  rdrop
;


: rgx-scan-next  ( rgx -- = Move the scanner to the next token )
  dup rgx>next @ swap rgx>index +!
;


( Private parser words )

: rgx-cleanup   ( nfs1 nfs2 -- = Cleanup an error expression )
  swap nil swap nfe+resolve             \ Resolve the open outs to nil
  nfe+free-expression                   \ Free the expression
;


defer rgx.parse-alternation

: rgx-parse-single   ( rgx -- nfs1 nfs2 true | false = Parse a single token, return the expression )
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


: rgx-parse-repeat   ( rgx -- nfs1 nfs2 true | false = Parse a repeat token, return the expression )
  >r
  r@ rgx-parse-single IF          \ If a single expression is parsed Then
    BEGIN
      r@ rgx-scan-token nip       \   Scan the current token
      CASE
        rgx.zero-or-one OF        \   If the token is ? Then
          r@ nfe-zero-or-one      \     Change the expression 
          r@ rgx-scan-next        \     Move to the next token
          false                   \     Continue scanning
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


: rgx-parse-concat   ( rgx -- nfs1 nfs2 true | false = Parse a concatenation of repeat tokens, return the expression )
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


: rgx-parse-alternation   ( rgx -- nfs1 nfs2 true | false = Parse an alternation of two expressions, return the expression )
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

: rgx-compile  ( c-addr u rgx -- true | n false = Compile a pattern as regular expression, return success and optional the error offset n )
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


: rgx-cmatch?   ( c-addr u rgx -- flag = Match case-sensitive a string with the regular expression, return match result )
  false swap nfe-match?
;


: rgx-imatch?   ( c-addr u rgx -- flag = Match case-insensitive a string with the regular expression, return match result )
  true swap nfe-match?
;


: rgx-csearch   ( c-addr u rgx -- n = Search case-sensitive in a string for the first match of the regular expression, return offset in string, or -1 for not found )
  false swap nfe-search
;


: rgx-isearch   ( c-addr u rgx -- n:index = Search case-insensitive in a string for the first match of the regular expression, return offset in string, or -1 if not found )
  true swap nfe-search
;


: rgx-result   ( n rgx -- n1 n2 = Get the match result of the nth grouping, return match start n2 and end n1 )
  nfe-result
;


( Inspection )

: rgx-dump     ( rgx -- = Dump the regular expression )
  dup nfe-dump
  ." rgx:" dup . cr
  ."  pattern   :" dup rgx>pattern ? cr
  ."  length    :" dup rgx>length  ? cr
  ."  next      :" dup rgx>next    ? cr
  ."  index     :"     rgx>index   ? cr
;

[THEN]

\ ==============================================================================
