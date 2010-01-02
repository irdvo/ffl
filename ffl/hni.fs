\ ==============================================================================
\
\           hni - the generic hash table base iterator in the ffl
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
\  $Date: 2008-06-25 16:48:34 $ $Revision: 1.7 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] hni.version [IF]


include ffl/stc.fs
include ffl/hnt.fs
include ffl/hnn.fs


( hni = Generic Hash Table Iterator )
( The hni module implements an iterator on the generic hash table [hnt]. )


1 constant hni.version


( Iterator structure )

begin-structure hni%       ( -- n = Get the required space for a hash table base iterator variable )
  field: hni>table      \ Reference to the hash table
  field: hni>index      \ Index in the table
  field: hni>walk       \ Current node in the table
end-structure


( Private words )

: hni-search-table ( u1 hni -- nil | u2 hnn = Search in the table for a node from start u, resulting in index u2 )
  >r dup r>
  hni>table @
  hnt-table-bounds ?DO                \ end and start of table for DO; S: index
    I @
    nil<>? IF
      UNLOOP
      EXIT
    THEN
    1+
  cell +LOOP
  drop                                \ nothing found
  nil
;
    
    
( Iterator creation, initialisation and destruction words )

: hni-init     ( hnt hni -- = Initialise the iterator with the hash table hnt )
  tuck hni>table    !
  dup  hni>index   0!
       hni>walk  nil!
;


: hni-create   ( hnt "<spaces>name" -- ; -- hni = Create a named iterator in the dictionary on the hash table hnt )
  create 
    here  hni% allot  hni-init
;


: hni-new      ( hnt -- hni = Create an iterator on the heap on the hash table hnt)
  hni% allocate  throw  tuck hni-init
;


: hni-free     ( hni -- = Free iterator from heap )
  free throw
;


( Member words )

: hni-get      ( hni -- hnn | nil = Get the node from the current record )
  hni>walk @
;


: hni-key      ( hni -- c-addr u = Get the key from the current record )
  hni>walk @
  nil<>? IF
    hnn-key@
  ELSE
    exp-invalid-state throw
  THEN    
;


( Iterator words )

: hni-first    ( hni -- hnn | nil = Move the iterator to the first record, return the node in this record )
  >r
  r@ hni>index   0!
  r@ hni>walk  nil!
  
  0 r@ hni-search-table      \ search a node in the table
  
  nil<>? IF
    r@ hni>walk  !           \ save hcn and index
    r@ hni>index !
  THEN
  
  r> hni-get
;


: hni-next     ( hni -- hnn | nil = Move the iterator to the next record, return the node in this record )
  >r
  r@ hni>walk @              \ check if current node has a next node
  nil<>? IF
    hnn-next@
    nil<>? IF
      dup r@ hni>walk !
    ELSE                     \ else search the next node in the table 
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


: hni-first?   ( hni -- flag = Check if the iterator is on the first record )
  dup hni>walk @
  nil<>? IF
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


: hni-last?    ( hni -- flag = Check if the iterator is on the last record )
  dup hni>walk @
  nil<>? IF
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

: hni-dump     ( hni -- = Dump the iterator )
  ." hni:" dup . cr
  ."  table :" dup hni>table ?  cr
  ."  index :" dup hni>index ?  cr
  ."  walk  :"     hni>walk  ?  cr
;

[THEN]

\ ==============================================================================
