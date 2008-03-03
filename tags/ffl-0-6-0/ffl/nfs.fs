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
\  $Date: 2007-12-09 07:23:16 $ $Revision: 1.6 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nfs.version [IF]


include ffl/stc.fs


( nfs = Non-deterministic finite automata state )
( The nfs module implements a state in a non-deterministic finite automata.   )


1 constant nfs.version


( State structure )

begin-structure nfs%       ( -- n = Get the required space for a nfs state )
  field: nfs>id         \ the state id
  field: nfs>type       \ the state type
  field: nfs>data       \ the state optional data
  field: nfs>out1       \ the next state (during building also used as next pointer )
  field: nfs>out2       \ the split state (during building also used as next pointer )
  field: nfs>visit      \ the visit number
end-structure


( State types )

1 constant nfs.char   ( -- n = State type char, data = char )
2 constant nfs.any    ( -- n = State type any, no data )
3 constant nfs.class  ( -- n = State type class, data = chs )
4 constant nfs.lparen ( -- n = State type left paren, data = paren level )
5 constant nfs.rparen ( -- n = State type right paren, data = paren level )
6 constant nfs.split  ( -- n = State type split, no data )
7 constant nfs.match  ( -- n = State type match, no data )


( State creation, initialisation and destruction )

: nfs-init     ( x n1 n2 nfs -- = Initialise the nfs state with data x, type n1 and id n2 )
  >r
  r@ nfs>id    !
  r@ nfs>type  !
  r@ nfs>data  !
  r@ nfs>out1  nil!
  r@ nfs>out2  nil!
  r> nfs>visit 0!
;


: nfs-new      ( x n1 n2 -- nfs = Create a new nfs state on the heap with data x, type n1 and id n2 )
  nfs% allocate  throw  >r r@ nfs-init r>
;


: nfs-free     ( nfs -- = Free the state from the heap )
  free  throw
;


( Member words )

: nfs-id@    ( nfs -- n = Get the id of the state )
  nfs>id @
;


: nfs-type@  ( nfs -- n = Get the type of the state )
  nfs>type @
;


: nfs-data@  ( nfs -- x = Get the optional data of the state )
  nfs>data @
;


: nfs-out1!  ( nfs1 nfs2 -- = Set out1 in the nfs2 state to the nfs1 state )
  nfs>out1 !
;


: nfs-out1@  ( nfs1 -- nfs2 = Get the out1 state of the nfs1 state )
  nfs>out1 @
;


: nfs-out2!  ( nfs1 nfs2 -- = Set out2 in the nfs2 state to the nfs1 state )
  nfs>out2 !
;


: nfs-out2@  ( nfs1 -- nfs2 = Get the out2 nfs state of nfs1 state )
  nfs>out2 @
;


: nfs-visit!  ( n nfs -- = Set the visit number [0>=] )
  nfs>visit !
;


: nfs-visit@  ( nfs -- n = Get the visit number )
  nfs>visit @
;


( Inspection )

: nfs-dump     ( nfs -- = Dump the nfs state )
  ." nfs:" dup . cr
  ."  id   :" dup nfs>id    ? cr
  ."  type :" dup nfs>type  ? cr
  ."  data :" dup nfs>data  ? cr
  ."  out1 :" dup nfs>out1  ? cr
  ."  out2 :" dup nfs>out2  ? cr
  ."  visit:"     nfs>visit ? cr 
;

[THEN]

\ ==============================================================================
 
