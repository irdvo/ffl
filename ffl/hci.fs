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
\  $Date: 2006-07-26 06:50:20 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] hci.version [IF]


include ffl/stc.fs
include ffl/hct.fs
include ffl/hcn.fs


( hci = Hash Cell Table Iterator )
( The hci module implements an iterator on the hash cell table [hct]. )


1 constant hci.version


( Public structure )

struct: hci%       ( - n = Get the required space for a hci data structure )
  cell: hci>table       \ Refernce to the hash table
  cell: hci>index       \ Index in the table
  cell: hci>walk        \ Current node in the table
;struct 


( Public words )

: hci-init     ( w:hct w:hci - = Initialise the iterator with a hash table )
  tuck hci>table    !
  dup  hci>index   0!
       hci>walk  nil!
;


: hci-create   ( C: w:hct "name" - R: - w = Create a named iterator in the dictionary )
  create 
    here  hci% allot  hci-init
;


: hci-new      ( w:hct - w:hci = Create an iterator on the heap )
  hci% allocate  throw  tuck hci-init
;


: hci-free     ( w:hci - = Free iterator from heap )
  free throw
;


: hci-get      ( w:hci - false | w true = Get the cell data from the current record )
  \ ToDo
;


: hci-set      ( w w:hci - = Set the cell data for the current record )
  \ ToDo
;


: hci-first    ( w:hci - w true | false = Move the iterator to the first record )
  \ ToDo
;


: hci-next     ( w:hci - w true | false = Move the iterator to the next record )
  \ ToDo
;


: hci-move     ( w w:hci - f = Move the iterator to the <next?> record with the cell data )
  \ ToDo
;


: hci-first?   ( w:hci - f = Check if the iterator is on the first record )
  \ ToDo
;


: hci-last?    ( w:hci - f = Check if the iterator is on the last record )
  \ ToDo
;


: hci-dump     ( w:hci - = Dump the iterator )
  ." hci:" dup . cr
  ."  table :" dup hci>table ?  cr
  ."  walk  :"     hci>walk  ?  cr
;

[THEN]

\ ==============================================================================
