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
\  $Date: 2007-03-04 08:38:31 $ $Revision: 1.5 $
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

sni% constant sci%  ( - n = Get the required space for a sci data structure )


( Iterator creation, initialisation and destruction )

: sci-init     ( w:scl w:sci - = Initialise the iterator with a scl list )
  sni-init
;


: sci-create   ( C: w:scl "name" - R: - w = Create a named iterator in the dictionary )
  create 
    here  sci% allot  sci-init
;


: sci-new      ( w:scl - w:sci = Create an iterator on the heap )
  sci% allocate  throw  tuck sci-init
;


: sci-free     ( w:sci - = Free iterator from heap )
  sni-free
;


( Private words )

: sni+get      ( w:scn - w true | false = Get the cell data from the current record )
  dup nil<> IF               \ if current <> nil then
    scn-cell@ true           \   fetch cell
  ELSE
    drop false
  THEN
;


( Member words )

: sci-get      ( w:sci - w true | false = Get the cell data from the current record )
  sni-get sni+get
;


: sci-set      ( w w:sci - = Set the cell data for the current record )
  sni-get
  dup nil<> IF
    scn-cell!
  ELSE
    exp-invalid-state throw
  THEN
;


( Iterator words )

: sci-first    ( w:sci - w true | false = Move the iterator to the first record )
  sni-first sni+get
;


: sci-next     ( w:sci - w true | false = Move the iterator to the next record )
  sni-next sni+get
;


: sci-move     ( w w:sci - f = Move the iterator to the next record with the cell data )
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


: sci-first?   ( w:sci - f = Check if the iterator is on the first record )
  sni-first?
;


: sci-last?    ( w:sci - f = Check if the iterator is on the last record )
  sni-last?
;


: sci-insert-after ( w:data w:sci - = Insert the cell data after the current record )
  dup sni>snl @
  swap sni-get
  dup nil<> IF
    2>r scn-new 2r>
    swap snl-insert-after
  ELSE
    exp-invalid-state throw
  THEN
;


( Inspection )

: sci-dump     ( w:sci - = Dump the iterator )
  sni-dump
;

[THEN]

\ ==============================================================================
