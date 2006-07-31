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
\  $Date: 2006-07-31 16:50:42 $ $Revision: 1.3 $
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


( Private words )

: hci-search-table ( u:start w:hci - nil | u:index w:hcn = Search in the tabel for a node )
  >r dup r>
  hci>table @
  hct-table-bounds ?DO                \ end and start of table for DO; S: index
    I @
    dup nil<> IF
      UNLOOP
      EXIT
    ELSE
      drop
    THEN
    1+
  cell +LOOP
  drop                                \ nothing found
  nil
;
    
    
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
  hci>walk @
  dup nil<> IF
    hcn-cell@
    true
  ELSE
    drop
    false
  THEN
;


: hci-key      ( w:hci - c-addr u = Get the key from the current record )
  hci>walk @
  dup nil<> IF
    dup  hcn>key @
    swap hcn>klen @
  ELSE
    exp-invalid-state throw
  THEN    
;

: hci-set      ( w w:hci - = Set the cell data for the current record )
  hci>walk @
  dup nil<> IF
    hcn>cell !
  ELSE
    exp-invalid-state throw
  THEN    
;


: hci-first    ( w:hci - w true | false = Move the iterator to the first record )
  >r
  r@ hci>index   0!
  r@ hci>walk  nil!
  
  0 r@ hci-search-table      \ search a node in the table
  
  dup nil<> IF
    r@ hci>walk  !           \ save hcn and index
    r@ hci>index !
  ELSE
    drop
  THEN
  
  r> hci-get
;


: hci-next     ( w:hci - w true | false = Move the iterator to the next record )
  >r
  r@ hci>walk @              \ check if current node has a next node
  dup nil<> IF
    hcn-next@
    dup nil<> IF
      dup r@ hci>walk !
      hcn-cell@ true
    ELSE                     \ else search the next node in the table 
      drop
      
      r@ hci>index @ 1+  r@ hci-search-table
      
      dup nil<> IF
        tuck
        r@ hci>walk !
        r@ hci>index !
        
        hcn-cell@ 
        true
      ELSE
        r@ hci>walk   !
        r@ hci>index 0!
        false
      THEN
    THEN
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
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
  hci>walk @ nil<>
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
  ."  index :" dup hci>index ?  cr
  ."  walk  :"     hci>walk  ?  cr
;

[THEN]

\ ==============================================================================
