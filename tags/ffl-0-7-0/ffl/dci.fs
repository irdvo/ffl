\ ==============================================================================
\
\          dci - the double linked cell list iterator in the ffl
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
\  $Date: 2008-06-25 16:48:34 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] dci.version [IF]


include ffl/dcl.fs
include ffl/dni.fs


( dci = Double Linked Cell List Iterator )
( The dci module implements an iterator on the double linked cell list [dcl]. )


1 constant dci.version


( Iterator structure )

dni% constant dci%  ( -- n = Get the required space for a dci variable )


( Iterator creation, initialisation and destruction )

: dci-init     ( dcl dci -- = Initialise the iterator with a dcl list )
  dni-init
;


: dci-create   ( dcl "<spaces>name" -- ; -- dci = Create a named iterator in the dictionary with a dcl list )
  create   here  dci% allot  dci-init
;


: dci-new      ( dcl -- dci = Create an iterator on the heap with a dcl list )
  dci% allocate  throw  tuck dci-init
;


: dci-free     ( dci -- = Free the iterator from the heap )
  dni-free
;


( Private words )

: dci+get      ( dcn -- x true | false = Get the cell data x from the node )
  nil<>? IF
    dcn-cell@ true
  ELSE
    false
  THEN
;


( Member words )

: dci-get      ( dci -- x true | false = Get the cell data x from the current record )
  dni-get dci+get
;


: dci-set      ( x dci -- = Set the cell data x for the current record )
  dni-get
  nil<>? IF
    dcn-cell!
  ELSE
    exp-invalid-state throw
  THEN
;


( Iterator words )

: dci-first    ( dci -- x true | false = Move the iterator to the first record, return the cell data x )
  dni-first dci+get
;


: dci-next     ( dci -- x true | false = Move the iterator to the next record, return the cell data x )
  dni-next dci+get
;


: dci-prev     ( dci -- x true | false = Move the iterator to the previous record, return the cell data x )
  dni-prev dci+get
;


: dci-last     ( dci -- x true | false = Move the iterator to the last record, return the cell data x )
  dni-last dci+get
;


: dci-move     ( x dci -- false = Move the iterator to the next record with the cell data x )
  swap
  BEGIN
    over dci-next IF
      over = 
    ELSE
      true
    THEN
  UNTIL
  drop
  dni-get nil<>
;


: dci-first?   ( dci -- flag = Check if the iterator is on the first record )
  dni-first?
;


: dci-last?    ( dci -- flag = Check if the iterator is on the last record )
  dni-last?
;


: dci-insert-after ( x dci -- = Insert the cell data x after the current record )
  dup dni>dnl @
  swap dni-get
  nil<>? IF
    2>r dcn-new 2r>
    swap dnl-insert-after
  ELSE
    exp-invalid-state throw
  THEN
;


( Inspection )

: dci-dump     ( dci -- = Dump the iterator )
  dni-dump
;

[THEN]

\ ==============================================================================
