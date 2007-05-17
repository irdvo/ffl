\ ==============================================================================
\
\           nfe - the non-deterministic finite automata expression
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
\  $Date: 2007-05-17 19:32:40 $ $Revision: 1.6 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nfe.version [IF]

include ffl/nfs.fs


( nfe = Non-deterministic finite automata expression )
( The nfe module implements an expression in a non-deterministic finite      )
( automata. An expression is a concatenation, repeation or alteration of     )
( non-deterministic finite automato states [nfs]. An not yet fully built     )
( expression consists of two cells on the stack: a list with the non resolved)
( out states and a list of [nfs] states.                                     )


1 constant nfe.version


( Expression structure )

struct: nfe%       ( - n = Get the required space for the nfe data structure )
  cell: nfe>expression \ the expression: a list of states
  cell: nfe>states     \ the number of states in the expression
  cell: nfe>level      \ the paren level in the expression
  cell: nfe>visit      \ the visit number
;struct


( Expression creation, initialisation and destruction )

: nfe-init         ( w:nfe - = Initialise the expression )
  dup nfe>expression nil!
  dup nfe>states       0!
  dup nfe>visit        0!
      nfe>level        0!
;


: nfe-create       ( C: "name" - R: - w:nfe = Create a named expression in the dictionary )
  create   here   nfe% allot   nfe-init
;


: nfe-new          ( - w:nfe = Create a new expression on the heap )
  nfe% allocate  throw  dup nfe-init
;


: nfe+free   ( w:start - = Free all states in an [sub]expression [recursive] )
  dup nil<> IF
    dup nfs-visit@ -1 <> IF     \ If start <> nil and not yet visited Then
      -1 over nfs-visit!        \   Set visited
      dup nfs-out1@ recurse     \   Recurse the out1 branch
      dup nfs-out2@ recurse     \   Recurse the out2 branch
      dup nfs-free              \   Free the state
    THEN
  THEN
  drop
;


: nfe-free   ( w:nfe - = Free the nfe structure from the heap )
  dup nfe>expression @ nfe+free
  
  free throw
;


( Private words )

: nfe+resolve   ( w:nfs w:outs - = Resolve all out states to nfs )
  BEGIN
    dup nil<>             \ while out <> nil do
  WHILE
    2dup  nfs-out1@       \   out = out.out1
    2swap nfs-out1!       \   out.out1 = nfs
  REPEAT
  2drop
;


: nfe-new-nfs  ( w:data n:type w:nfe - w:nfs = Create a new nfs in the expression )
  nfe>states dup 
  1+! @ nfs-new                \ create new nfs record with data, type and id
;
  
  
: nfe-nparen   ( w:outs w:start n:level w:nfe - w:outs w:start = Paren the expression with paren level n)
  2dup 
  nfs.lparen swap nfe-new-nfs  \ new state '(' with level
  >r
  rot r@ nfs-out1!             \ '('.out1 = start
  nfs.rparen swap nfe-new-nfs  \ new state ')'
  tuck swap nfe+resolve        \ outs = ')' start = '('
  r>
;


( Member words )

: nfe-visit@   ( w:rgx - n = Get the [unique] visit number )
  nfe>visit dup
  1+! @
;

: nfe-expression@   ( w:nfe - w:list = Get the list of states in the expression or nil )
  nfe>expression @
;


: nfe-states@   ( w:nfe - n = Get the number of states in the expression )
  nfe>states @
;

: nfe-parens@   ( w:nfe - n = Get the number of parens in the expression )
  nfe>level @ 1+
;


( Expression building words )

: nfe-clear   ( w:nfe - = Clear the expression )
  dup nfe-expression@ nfe+free
      nfe-init
;
    
    
: nfe-single   ( w:data w:type w:nfe - w:outs w:start = Start an expression with a single new state )
  nfe-new-nfs             \ new nfs with data, type and id
  dup                     \ start = nfs, outs = nfs
;


: nfe-concat   ( w:outs1 w:start1 w:outs2 w:start2 w:nfe - w:outs w:start = Concat the two expressions )
  drop
  rot >r                 \ outs = outs2
  rot nfe+resolve        \ resolve outs1 -> start2
  r>                     \ start = start1
;


: nfe-paren   ( w:outs w:start w:nfe - w:outs w:start = Paren the expression )
  dup nfe>level dup 1+! @   \ the next paren level
  swap nfe-nparen           \ paren the expression with this level
;


: nfe-alternation   ( w:outs2 w:start2 w:outs1 w:start1 w:nfe - w:outs w:start = Make an alternation [|] of two expressions )
  nil nfs.split rot
  nfe-new-nfs               \ new split state
  >r   r@ nfs-out1!         \ split.out1 = start1
  swap r@ nfs-out2!         \ split.out2 = start2
  tuck                      \ append outs2 to outs1
  BEGIN
    dup nfs-out1@ dup nil<>
  WHILE
    nip
  REPEAT
  drop nfs-out1!
  r>                        \ start = split state, outs = outs1 + outs2
;


: nfe-zero-or-one   ( w:outs w:start w:nfe - w:outs w:start = Repeat the expression one or zero [?] times )
  nil nfs.split rot
  nfe-new-nfs              \ new split state
  tuck nfs-out2!           \ split.out2 = start
  tuck nfs-out1!           \ split.out1 = outs
  dup                      \ outs = outs + split, start = split   
;  


: nfe-zero-or-more   ( w:outs w:start w:nfe - w:outs w:start = Repeat nfe zero or more [*] times )
  nil nfs.split rot
  nfe-new-nfs             \ new split state
  tuck nfs-out2!          \ split.out2 = start
  tuck swap nfe+resolve   \ resolve outs -> split
  dup                     \ outs = split, start = split
;


: nfe-one-or-more   ( w:outs w:start w:nfe - w:outs w:start = Repeat nfe one or more [+] times )
  swap >r
  nil nfs.split rot
  nfe-new-nfs             \ new split state
  r@ over nfs-out2!       \ split.out2 = start
  tuck swap nfe+resolve   \ resolve outs -> split
  r>                      \ outs = split, start = start
;


: nfe-close   ( w:outs w:start w:nfe - w:start = Close the expression by adding the match state )
  >r
  0 r@ nfe-nparen         \ paren the full expression with level 0
  nil nfs.match 
  r@ nfe-new-nfs          \ new match state
  rot nfe+resolve         \ resolve outs -> match
  r> nfe>expression !     \ save the expression
;


( Private inspection words )

: nfe+dump-out   ( w:nfs - = Dump the first out pointer )
  ." ->"        
  nfs-out1@ nfs-id@ 0 .r 
;


: nfe+dump   ( n:visit w:start - = Dump the expression [recursive] )
  dup nil<> IF
    2dup nfs-visit@ <> IF         \ If start <> nil and not yet visited Then
      2dup nfs-visit!             \   Set visited
      dup nfs-id@ 0 .r            \   Show id, type, optional data and nexts
      [char] : emit
      dup nfs-type@ 
      CASE
        nfs.char   OF 
          dup nfs-data@ emit 
          dup nfe+dump-out
          ENDOF
        nfs.any    OF 
          [char] . emit
          dup nfe+dump-out
          ENDOF
        nfs.class  OF 
          [char] [ emit
          dup nfe+dump-out 
          ENDOF
        nfs.lparen OF 
          [char] ( emit 
          dup nfs-data@ 0 .r 
          dup nfe+dump-out 
          ENDOF
        nfs.rparen OF 
          [char] ) emit 
          dup nfs-data@ 0 .r 
          dup nfe+dump-out
          ENDOF
        nfs.split  OF 
          [char] ? emit
          dup nfe+dump-out
          [char] , emit
          dup nfs-out2@ nfs-id@ 0 .r 
          ENDOF
        nfs.match  OF 
          [char] ! emit 
          ENDOF
      ENDCASE
      [char] ; emit space
      2dup nfs-out1@ recurse      \   Recurse the out1 branch
      2dup nfs-out2@ recurse      \   Recurse the out2 branch
    THEN
  THEN
  2drop
;


( Inspection )

: nfe-dump  ( w:nfe - = Dump the expression )
  ." nfe:" dup . cr
  ."  states    :" dup nfe-states@ . cr
  ."  level     :"  dup nfe>level   ? cr
  ."  visit     :" dup nfe>visit   ? cr
  ."  expression:" dup nfe-visit@ swap nfe-expression@ nfe+dump cr
;

[THEN]

\ ==============================================================================
 
