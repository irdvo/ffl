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
\  $Date: 2007-01-07 08:07:01 $ $Revision: 1.1 $
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

dni% constant dci%  ( - n = Get the required space for a dci data structure )


( Iterator creation, initialisation and destruction )

: dci-init     ( w:dcl w:dci - = Initialise the iterator with a dcl list )
  dni-init
;


: dci-create   ( C: w:dcl "name" - R: - w = Create a named iterator in the dictionary )
  create   here  dci% allot  dci-init
;


: dci-new      ( w:dcl - w:dci = Create an iterator on the heap )
  dci% allocate  throw  tuck dci-init
;


: dci-free     ( w:dci - = Free the iterator from the heap )
  dni-free
;


( Private words )

: dci+get      ( w:dcn - w:data true | false = Get the cell data from the node )
  dup nil<> IF
    dcn-cell@ true
  ELSE
    drop false
  THEN
;


( Member words )

: dci-get      ( w:dci - w true | false = Get the cell data from the current record )
  dni-get dci+get
;


: dci-set      ( w w:dci - = Set the cell data for the current record )
  dni-get
  dup nil<> IF
    dcn-cell!
  ELSE
    exp-invalid-state throw
  THEN
;


( Iterator words )

: dci-first    ( w:dci - w true | false = Move the iterator to the first record )
  dni-first dci+get
;


: dci-next     ( w:dci - w true | false = Move the iterator to the next record )
  dni-next dci+get
;


: dci-prev     ( w:dci - w true | false = Move the iterator to the previous record )
  dni-prev dci+get
;


: dci-last     ( w:dci - w true | false = Move the iterator to the last record )
  dni-last dci+get
;


: dci-move     ( w w:dci - f = Move the iterator to the <next?> record with the cell data )
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


: dci-first?   ( w:dci - f = Check if the iterator is on the first record )
  dni-first?
;


: dci-last?    ( w:dci - f = Check if the iterator is on the last record )
  dni-last?
;


: dci-insert-after ( w w:dci - = Insert the cell data after the current record )
  dup dni>dnl @
  swap dni-get
  dup nil<> IF
    2>r dcn-new 2r>
    swap dnl-insert-after
  ELSE
    exp-invalid-state throw
  THEN
;


( Inspection )

: dci-dump     ( w:dci - = Dump the iterator )
  dni-dump
;

[THEN]

\ ==============================================================================
