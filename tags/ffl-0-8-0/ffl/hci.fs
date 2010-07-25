\ ==============================================================================
\
\             hci - the hash cell table iterator in the ffl
\
\               Copyright (C) 2006  Dick van Oudheusden
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
\  $Date: 2008-02-03 07:09:34 $ $Revision: 1.8 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] hci.version [IF]


include ffl/hni.fs
include ffl/hct.fs
include ffl/hcn.fs


( hci = Hash Cell Table Iterator )
( The hci module implements an iterator on the hash cell table [hct]. )


1 constant hci.version


( Iterator structure )

hni% constant hci%  ( -- n = Get the required space for a hash cell table iterator )


( Iterator creation, initialisation and destruction words )

: hci-init     ( hct hci -- = Initialise the iterator with a hash table )
  hni-init
;


: hci-create   ( hct "<spaces>name" -- ; -- hci = Create a named iterator in the dictionary on the hash table hct )
  hni-create
;


: hci-new      ( hct -- hci = Create an iterator on the heap on the hash table hct )
  hni-new
;


: hci-free     ( hci -- = Free the iterator from heap )
  hni-free
;


( Private words )

: hci+get      ( hcn -- false | x true = Get the cell data x from the node )
  nil<>? IF
    hcn-cell@
    true
  ELSE
    false
  THEN
;


( Member words )

: hci-get      ( hci -- false | x true = Get the cell data x from the current record )
  hni-get hci+get
;


: hci-key      ( hci -- c-addr u = Get the key from the current record )
  hni-key
;


: hci-set      ( x hci -- = Set the cell data x in the current record )
  hni-get nil<>? IF
    hcn>cell !
  THEN    
;


( Iterator words )

: hci-first    ( hci -- x true | false = Move the iterator to the first record, return the cell data of this record )
  hni-first hci+get
;


: hci-next     ( hci -- x true | false = Move the iterator to the next record, return the cell data from this record )
  hni-next hci+get
;


: hci-move     ( x hci -- flag = Move the iterator to the next record with the cell data x, return success )
  swap
  BEGIN
    over hci-next IF
      over = 
    ELSE
      true
    THEN
  UNTIL
  drop
  hni>walk @ nil<>
;


: hci-first?   ( hci -- flag = Check if the iterator is on the first record )
  hni-first?
;


: hci-last?    ( hci -- flag = Check if the iterator is on the last record )
  hni-last?
;


( Inspection )

: hci-dump     ( hci -- = Dump the iterator )
  hni-dump
;

[THEN]

\ ==============================================================================
