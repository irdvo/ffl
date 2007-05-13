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
\  $Date: 2007-05-13 05:30:41 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nfe.version [IF]

include ffl/nfs.fs


( nfe = Non-deterministic finite automata expression )
( The nfe module implements an expression in a non-deterministic finite      )
( automata. An expression is a concatenation, repeation or alteration of     )
( non-deterministic finite automato states [nfs]. An not yet fully built     )
( expression keeps two cells on the stack: the list with non resolved out    )
( states and the list of [nfs] states. A fully built expression keeps only   )
( one cell on the stack: the list of [nfs] states.                           )


1 constant nfe.version


( Private words )

: nfe+resolve ( w:nfs w:outs - = Resolve all out states to nfs )
  BEGIN
    dup nil<>             \ while out <> nil do
  WHILE
    2dup  nfs-out1@       \   out = out.out1
    2swap nfs-out1!       \   out.out1 = nfs
  REPEAT
  2drop
;


( Expression destruction word )

: nfe+free  ( w:start - = Free all states in an expression [recursive] )
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


( Expression building words )

: nfe+single  ( w:data w:type - w:outs w:start = Start an expression with a single new state )
  nfs-new                 \ new nfs
  dup                     \ start = nfs, outs = nfs
;


: nfe+concat  ( w:outs1 w:start1 w:outs2 w:start2 - w:outs w:start = Concat the two expressions )
  rot >r                 \ outs = outs2
  rot nfe+resolve        \ resolve outs1 -> start2
  r>                     \ start = start1
;


: nfe+paren  ( w:outs w:start - w:outs w:start = Paren the expression )
  nil nfs.lparen nfs-new    \ new state '('
  >r 
  r@ nfs-out1!              \ '('.out1 = start
  nil nfs.rparen nfs-new    \ new state ')'
  tuck swap nfe+resolve     \ resolve outs -> ')'
  r>                        \ outs = ')' start = '('
;


: nfe+alternation  ( w:outs2 w:start2 w:outs1 w:start1 - w:outs w:start = Make an alternation [|] of two expressions )
  nil nfs.split nfs-new    \ new split state
  >r   r@ nfs-out1!        \ split.out1 = start1
  swap r@ nfs-out2!        \ split.out2 = start2
  tuck                     \ append outs2 to outs1
  BEGIN
    dup nfs-out1@ dup nil<>
  WHILE
    nip
  REPEAT
  drop nfs-out1!
  r>                       \ start = split state, outs = outs1 + outs2
;


: nfe+zero-or-one  ( w:outs w:start - w:outs w:start = Repeat the expression one or zero [?] times )
  nil nfs.split nfs-new    \ new split state
  tuck nfs-out2!           \ split.out2 = start
  tuck nfs-out1!           \ split.out1 = outs
  dup                      \ outs = outs + split, start = split   
;  


: nfe+zero-or-more  ( w:outs w:start - w:outs w:start = Repeat nfe zero or more [*] times )
  nil nfs.split nfs-new   \ new split state
  tuck nfs-out2!          \ split.out2 = start
  tuck swap nfe+resolve   \ resolve outs -> split
  dup                     \ outs = split, start = split
;


: nfe+one-or-more  ( w:outs w:start - w:outs w:start = Repeat nfe one or more [+] times )
  >r
  nil nfs.split nfs-new   \ new split state
  r@ over nfs-out2!       \ split.out2 = start
  tuck swap nfe+resolve   \ resolve outs -> split
  r>                      \ outs = split, start = start
;


: nfe+close  ( w:outs w:start - w:start = Close the expression by adding the match state )
  nil nfs.match nfs-new   \ new match state
  rot nfe+resolve         \ resolve outs -> match
;


( Inspection )

: nfe+dump  ( n:visit w:start - = Dump the expression using an unique visit number [0>] [recursive] )
  dup nil<> IF
    2dup nfs-visit@ <> IF         \ If start <> nil and not yet visited Then
      2dup nfs-visit!             \   Set visited
      dup nfs-type@ dup .
      nfs.char = IF
        dup nfs-data@ emit space  \   Dump the state
      THEN
      2dup nfs-out1@ recurse      \   Recurse the out1 branch
      2dup nfs-out2@ recurse      \   Recurse the out2 branch
    THEN
  THEN
  2drop
;

[THEN]

\ ==============================================================================
 
