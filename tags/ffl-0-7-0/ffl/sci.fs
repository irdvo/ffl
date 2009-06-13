\ ==============================================================================
\
\          sci - the single linked cell list iterator in the ffl
\
\               Copyright (C) 2005  Dick van Oudheusden
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
\  $Date: 2008-02-03 07:09:34 $ $Revision: 1.7 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] sci.version [IF]


include ffl/scl.fs
include ffl/scn.fs
include ffl/sni.fs

( sci = Single Linked Cell List Iterator )
( The sci module implements an iterator on the single linked cell list [scl]. )


1 constant sci.version


( Iterator structure )

sni% constant sci%  ( -- n = Get the required space for a sci variable )


( Iterator creation, initialisation and destruction )

: sci-init     ( scl sci -- = Initialise the iterator with a scl list )
  sni-init
;


: sci-create   ( scl "<spaces>name" -- ; -- sci = Create a named iterator in the dictionary on the scl list )
  create 
    here  sci% allot  sci-init
;


: sci-new      ( scl -- sci = Create an iterator on the heap on the scl list)
  sci% allocate  throw  tuck sci-init
;


: sci-free     ( sci -- = Free the iterator from the heap )
  sni-free
;


( Private words )

: sni+get      ( scn -- x true | false = Get the cell data x from the scn node )
  nil<>? IF                  \ if current <> nil then
    scn-cell@ true           \   fetch cell
  ELSE
    false
  THEN
;


( Member words )

: sci-get      ( sci -- x true | false = Get the cell data x from the current node )
  sni-get sni+get
;


: sci-set      ( x sci -- = Set the cell data x for the current node )
  sni-get
  nil<>? IF
    scn-cell!
  ELSE
    exp-invalid-state throw
  THEN
;


( Iterator words )

: sci-first    ( sci -- x true | false = Move the iterator to the first node, return the cell data x from this node )
  sni-first sni+get
;


: sci-next     ( sci -- x true | false = Move the iterator to the next node, return the cell data x from this node )
  sni-next sni+get
;


: sci-move     ( x sci -- flag = Move the iterator to the next node with the cell data x )
  swap
  BEGIN
    over sci-next IF
      over = 
    ELSE
      true
    THEN
  UNTIL
  drop
  sni-get nil<>
;


: sci-first?   ( sci -- flag = Check if the iterator is on the first node )
  sni-first?
;


: sci-last?    ( sci -- flag = Check if the iterator is on the last node )
  sni-last?
;


: sci-insert-after ( x sci -- = Insert the cell data x after the current node )
  dup sni>snl @
  swap sni-get
  nil<>? IF
    2>r scn-new 2r>
    swap snl-insert-after
  ELSE
    exp-invalid-state throw
  THEN
;


( Inspection )

: sci-dump     ( sci -- = Dump the iterator )
  sni-dump
;

[THEN]

\ ==============================================================================
