\ ==============================================================================
\
\           nft - the non-deterministic finite automata thread
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
\  $Date: 2007-05-17 19:32:40 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nft.version [IF]


include ffl/nfs.fs


( nft = Non-deterministic finite automata thread )
( The nft module implements a dynamic memory structure to store the state    )
( during matching of a non-deterministic finite automata expression.         )


1 constant nft.version

( Thread structure )

struct: nft%
  cell: nft>parens     \ the number of parens in the expression
  cell: nft>length     \ the number of nfs states in the thread
;struct


( State creation, initialisation and destruction )

: nft-init     ( n:parens w:nft - = Initialise the thread )
  tuck nft>parens  !
       nft>length 0!
;


: nft-new      ( n:parens n:states - w:nft = Create a new thread on the heap )
  over 2* 1+ * cells     \ thread size: ((parens * 2) + 1) * states cells
  nft% +                 \ include the struture
  allocate throw
  tuck nft-init 
;


: nft-free     ( w:nft - = Free the thread from the heap )
  free  throw
;


( Member words )

: nft-length@    ( w:nft - n:length = Get the length of the thread )
  nft>length @
;


( Thread words )

: nft-clear   ( w:nft - = Clear the thread )
  dup nft>length 0!
;


: nft-get   ( n:offset w:nft - w:matches w:nfs = Get the matches and nfs state )
  nft>parens @ 
  2* 1+ cells * nft% +
  dup @ 
  swap
;


: nft-add   ( w:matches w:nfs w:nft - = Add the nfs state with the matches to the thread )
  >r
  r@ nft>length dup @ swap 1+!   \ fetch the length and increase
  r@ nft>parens @
  2* 1+ cells * nft% +
  tuck !                         \ thread.state
  cell+                          \ thread.matches
  r> nft>parens @ 2* cells 1 chars /
  move
;

( Inspection )

: nft-dump     ( w:nft - = Dump the state )
  ." nft:" dup . cr
  ."  parens:" dup nft>parens  ? cr
  ."  length:"     nft>length  ? cr
  \ ToDo: states and matches
;

[THEN]

\ ==============================================================================
 
