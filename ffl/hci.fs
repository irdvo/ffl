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
\  $Date: 2007-11-10 07:20:08 $ $Revision: 1.6 $
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

hni% constant hci%  ( - n = Get the required space for a hash cell table iterator data structure )


( Iterator creation, initialisation and destruction words )

: hci-init     ( w:hct w:hci - = Initialise the iterator with a hash table )
  hni-init
;


: hci-create   ( C: w:hct "name" - R: - w = Create a named iterator in the dictionary )
  hni-create
;


: hci-new      ( w:hct - w:hci = Create an iterator on the heap )
  hni-new
;


: hci-free     ( w:hci - = Free iterator from heap )
  hni-free
;


( Private words )

: hci+get      ( w:hcn - false | w true = Get the cell data from the node )
  dup nil<> IF
    hcn-cell@
    true
  ELSE
    drop
    false
  THEN
;


( Member words )

: hci-get      ( w:hci - false | w true = Get the cell data from the current record )
  hni-get hci+get
;


: hci-key      ( w:hci - c-addr u = Get the key from the current record )
  hni-key
;


: hci-set      ( w w:hci - = Set the cell data for the current record )
  hni-get dup nil<> IF
    hcn>cell !
  ELSE
    drop
  THEN    
;


( Iterator words )

: hci-first    ( w:hci - w true | false = Move the iterator to the first record )
  hni-first hci+get
;


: hci-next     ( w:hci - w true | false = Move the iterator to the next record )
  hni-next hci+get
;


: hci-move     ( w w:hci - f = Move the iterator to the next record with the cell data )
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


: hci-first?   ( w:hci - f = Check if the iterator is on the first record )
  hni-first?
;


: hci-last?    ( w:hci - f = Check if the iterator is on the last record )
  hni-last?
;


( Inspection )

: hci-dump     ( w:hci - = Dump the iterator )
  hni-dump
;

[THEN]

\ ==============================================================================
