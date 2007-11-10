\ ==============================================================================
\
\             hni - the hash table base iterator in the ffl
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
\  $Date: 2007-11-10 07:20:08 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] hni.version [IF]


include ffl/stc.fs
include ffl/hnt.fs
include ffl/hnn.fs


( hni = Hash Cell Table Iterator )
( The hni module implements an iterator on the hash cell table [hct]. )


1 constant hni.version


( Iterator structure )

struct: hni%       ( - n = Get the required space for a hash table base iterator data structure )
  cell: hni>table       \ Refernce to the hash table
  cell: hni>index       \ Index in the table
  cell: hni>walk        \ Current node in the table
;struct 


( Private words )

: hni-search-table ( u:start w:hni - nil | u:index w:hnn = Search in the tabel for a node )
  >r dup r>
  hni>table @
  hnt-table-bounds ?DO                \ end and start of table for DO; S: index
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
    
    
( Iterator creation, initialisation and destruction words )

: hni-init     ( w:hnt w:hni - = Initialise the iterator with a hash table )
  tuck hni>table    !
  dup  hni>index   0!
       hni>walk  nil!
;


: hni-create   ( C: w:hnt "name" - R: - w = Create a named iterator in the dictionary )
  create 
    here  hni% allot  hni-init
;


: hni-new      ( w:hct - w:hni = Create an iterator on the heap )
  hni% allocate  throw  tuck hni-init
;


: hni-free     ( w:hni - = Free iterator from heap )
  free throw
;


( Member words )

: hni-get      ( w:hni - w:hnn | nil = Get the node from the current record )
  hni>walk @
;


: hni-key      ( w:hni - c-addr u = Get the key from the current record )
  hni>walk @
  dup nil<> IF
    dup  hnn>key @
    swap hnn>klen @
  ELSE
    exp-invalid-state throw
  THEN    
;


( Iterator words )

: hni-first    ( w:hni - w:hnn | nil = Move the iterator to the first record )
  >r
  r@ hni>index   0!
  r@ hni>walk  nil!
  
  0 r@ hni-search-table      \ search a node in the table
  
  dup nil<> IF
    r@ hni>walk  !           \ save hcn and index
    r@ hni>index !
  ELSE
    drop
  THEN
  
  r> hni-get
;


: hni-next     ( w:hni - w:hnn | nil = Move the iterator to the next record )
  >r
  r@ hni>walk @              \ check if current node has a next node
  dup nil<> IF
    hnn-next@
    dup nil<> IF
      dup r@ hni>walk !
    ELSE                     \ else search the next node in the table 
      drop
      
      r@ hni>index @ 1+  r@ hni-search-table
      
      dup r@ hni>walk !
      
      dup nil<> IF
        swap
        r@ hni>index !
      ELSE
        r@ hni>index 0!
      THEN
    THEN
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
;


: hni-first?   ( w:hni - f = Check if the iterator is on the first record )
  dup hni>walk @
  dup nil<> IF
    hnn-prev@                     \ if there is a previous record, then not the first
    nil<> IF
      false
    ELSE
      0 over hni-search-table
      nil<> IF
        over hni>index @ =        \ if a previous table element is present, then not the first 
      ELSE
        exp-invalid-state throw
      THEN
    THEN
  ELSE
    exp-invalid-state throw
  THEN
  nip
;


: hni-last?    ( w:hni - f = Check if the iterator is on the last record )
  dup hni>walk @
  dup nil<> IF
    hnn-next@
    nil<> IF                      \ if there is a next record, then not the last
      false
    ELSE
      dup hni>index @ 1+ over hni-search-table
      nil= dup 0= IF              \ if a next table element is present, then not the last
        nip
      THEN
    THEN
  ELSE
    exp-invalid-state throw
  THEN
  nip
;


( Inspection )

: hni-dump     ( w:hni - = Dump the iterator )
  ." hni:" dup . cr
  ."  table :" dup hni>table ?  cr
  ."  index :" dup hni>index ?  cr
  ."  walk  :"     hni>walk  ?  cr
;

[THEN]

\ ==============================================================================
