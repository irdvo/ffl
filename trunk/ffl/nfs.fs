\ ==============================================================================
\
\           nfs - the non-deterministic finite automata state
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
\  $Date: 2007-05-09 05:38:00 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nfs.version [IF]


include ffl/stc.fs


( nfs = Non-deterministic finite automata state )
( The nfs module implements a state in a non-deterministic finite automata.   )


1 constant nfs.version


( State structure )

struct: nfs%       ( - n = Get the required space for the nfs data structure )
  cell: nfs>type
  cell: nfs>data
  cell: nfs>out1
  cell: nfs>out2
  cell: nfs>visit
;struct


( State types )

1 constant nfs.char   ( - n = State type char, data = char )
2 constant nfs.any    ( - n = State type any, no data )
3 constant nfs.class  ( - n = State type class, data = chs )
4 constant nfs.lparen ( - n = State type left paren, no data )
5 constant nfs.rparen ( - n = State type right paren, no data )
6 constant nfs.split  ( - n = State type split, no data )
7 constant nfs.match  ( - n = State type match, no data )


( State creation, initialisation and destruction )

: nfs-init     ( w:data n:type w:nfs - = Initialise the state )
  >r
  r@ nfs>type  !
  r@ nfs>data  !
  r@ nfs>out1  nil!
  r@ nfs>out2  nil!
  r> nfs>visit 0!
;


: nfs-new      ( w:data n:type - w:nfs = Create a new state on the heap )
  nfs% allocate  throw  >r r@ nfs-init r>
;


: nfs-free     ( w:nfs - = Free the state from the heap )
  free  throw
;


( Member words )

: nfs-type@  ( w:nfs - n:type = Get the type of the state )
  nfs>type @
;


: nfs-data@  ( w:nfs - w:data = Get the optional data of the state )
  nfs>data @
;


: nfs-out1!  ( w:nfs1 w:nfs - = Set the out1 in the state )
  nfs>out1 !
;


: nfs-out1@  ( w:nfs - w:nfs = Get the out1 of the state )
  nfs>out1 @
;


: nfs-out2!  ( w:nfs2 w:nfs - = Set the out2 in the state )
  nfs>out2 !
;


: nfs-out2@  ( w:nfs - w:nfs = Get the out2 of the state )
  nfs>out2 @
;


: nfs-visit!  ( n w:nfs - = Set the visit number [0>=] )
  nfs>visit !
;


: nfs-visit@  ( w:nfs - n = Get the visit number )
  nfs>visit @
;


( Inspection )

: nfs-dump     ( w:nfs - = Dump the state )
  ." nfs:" dup . cr
  ."  type :" dup nfs>type  ?  cr
  ."  data :" dup nfs>data  ? cr
  ."  out1 :" dup nfs>out1  ? cr
  ."  out2 :" dup nfs>out2  ? cr
  ."  visit:"     nfs>visit ? cr 
;

[THEN]

\ ==============================================================================
 
