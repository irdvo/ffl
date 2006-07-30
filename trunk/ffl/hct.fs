\ ==============================================================================
\
\                hct - the hash cell table in the ffl
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
\  $Date: 2006-07-30 07:06:02 $ $Revision: 1.5 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] hct.version [IF]


include ffl/stc.fs
include ffl/hcn.fs


( hct = Hash Cell Table )
( The hct module implements a hash table that can store cell wide data.)
  

1 constant hct.version


( Public structure )

struct: hct%       ( - n = Get the required space for the hct data structure )
  cell: hct>table        \ array of pointers to hcn
  cell: hct>size         \ number of elements in the array of pointers
  cell: hct>length       \ number of nodes in the hash table
  cell: hct>load         \ load factor (* 100)
;struct


( Private words )

: hct-allocate-table ( u - w:table = allocate a table with size u )
  dup 0= exp-invalid-parameters AND throw  
  
  dup cells allocate throw             \ table
  
  dup rot 0 DO                         \ fill table with nils
    dup nil!
    cell+
  LOOP
  drop
;


: hct-table@       ( w:hct - w:table = Get the table )
  hct>table @
;


: hct-size@        ( w:hct - u:size = Get the size )
  hct>size @
;


( Public words )

: hct-init     ( u w:hct - = Initialise the hash table with an initial size )
      dup  hct>length 0!
  100 over hct>load    !     \ load = 100%
      
  over hct-allocate-table
  
  over hct>table !
       hct>size  !
;
     

: hct-create   ( u "name" - = Create a named hash table with an initial size in the dictionary )
  create   here   hct% allot   hct-init
;


: hct-new      ( u - w:hct = Create a new hash table with an initial size on the heap )
  hct% allocate  throw  tuck hct-init
;


: hct-free     ( w:hct - = Free the table from the heap )
  dup hct-table@             \ Free the nodes:
  over hct-size@ 0 DO        \ Do for the whole table
    dup @
    BEGIN
      dup nil<>              \ Iterate the lists in the table
    WHILE
      dup hcn-next@
      swap hcn-free          \ Free the node
    REPEAT
    drop
    cell+
  LOOP
  drop
  
  dup hct-table@ free throw  \ free the table
  
  free throw                 \ free the hash table
;


( Module words )

: hct+hash     ( c-addr u - u = Calculate the hash value of a key )
  0 -rot
  bounds ?DO
    dup 5 lshift +
    I c@ +
    1 chars 
  +LOOP
;

  
( Private words )

: hct-table-pntr ( u:hash w:hct - w:pntr = Get pointer in table for hash )
  >r 
  r@ hct-size@ mod cells
  r> hct-table@ +
;


: hct-search   ( c-addr u w:hct - w:hcn = Search the node based on the key )
  over 0= exp-invalid-parameters AND throw
  
  >r
  2dup hct+hash dup
  r> hct-table-pntr @        \ offset in table -> node in table
                             \ c-addr u u:hash w:hcn
  BEGIN                      \ look for the key
    dup nil<> IF
      2dup hcn-hash@ = IF
        dup >r 2over  r@ hcn>key @ r> hcn>klen @ compare 0<>
      ELSE                   \ check hash and key
        true
      THEN
    ELSE
      false
    THEN
  WHILE
    hcn-next@                \ next hash table node
  REPEAT
  
  nip nip nip
;


: hct-insert-node ( w:hcn :u:size w:table - = Insert the node in the table )
  >r
  over swap
  over hcn-hash@
  swap mod cells
  r> +                       \ table element
  @!                         \ insert the new node, fetch the current node in table
  dup nil<> IF         
    2dup
    swap hcn>next !          \ if previous value, then link the node
         hcn>prev !
  ELSE
    2drop                    \ done
  THEN
;


( Member words )

: hct-empty?   ( w:hct - f = Check for empty table )
  hct>length @ 0=  
;


: hct-length@  ( w:hct - u = Get the number of nodes in the list )
  hct>length @
;


: hct-load@    ( w:hct - u = Get the load factor [*100%] )
  hct>load @
;


: hct-load!    ( u w:hct - = Set the load factor [*100] )
  hct>load !
;


: hct-size!    ( u w:hct - = Resize the hash table )
  >r
  r@ hct-table@ 
  swap dup hct-allocate-table     \ allocate the new table
  r@ hct-size@ 0 DO               \ S: old-table new-size new-table
    2>r
    dup @                         \ Get node from table
    BEGIN
      dup nil<>                   \ Walk through the list in the table
    WHILE
      dup hcn-next@ swap
      dup hcn>next nil!           \ Clear the node from the old table and ..
      dup hcn>prev nil!
      2r@ hct-insert-node         \ .. insert it in the new table
    REPEAT
    drop
    cell+
    2r>
  LOOP
  r@ hct>table @! free throw      \ store the new table, fetch the old and free
  r> hct>size !
  drop                            \ table-pntr
;


( Hashtable words )

: hct-insert   ( w c-addr u w:hct - = Insert a cell with a key in the table )
  >r 
  2dup hct+hash              \ calculate the hash
  hcn-new                    \ new hash table node
  
  r@ hct>size  @             \ offset in table, based on hash
  r@ hct-table@              \ table element
  
  hct-insert-node            \ insert the node in the table
  
  r@ hct>length 1+!          \ one more node in the hash table
  
                             \ test for rehash of table
  r@ hct-length@  r@ hct-size@  r@ hct-load@ 100 */  > IF
    r@ hct-size@ 2* 1+ r@ hct-size!
  THEN
  rdrop
;


: hct-delete   ( c-addr u w:hct - false | w true = Delete key from the table )
  >r
  r@ hct-search
  
  dup nil<> IF                              \ if node found then
    r@ hct>length 1-!                       \   one node less
    
    dup hcn-next@ dup nil<> IF              \   if next node present then
      hcn>prev over hcn-prev@ swap !        \     next->prev = prev node
    ELSE
      drop
    THEN
    
    dup hcn-prev@ dup nil<> IF              \   if prev node present then
      hcn>next over hcn-next@ swap !        \     prev->next = next node
    ELSE
      drop                                  \   else
      dup hcn-next@                         \     table-pntr = next node
      over hcn-hash@ r@ hct-table-pntr !
    THEN
    
    dup hcn-cell@ swap                      \   fetch the cell
    
    hcn-free                                \   free the node
    
    true
  ELSE
    drop 
    false
  THEN
  rdrop
;


: hct-get      ( c-addr u w:hct - false | w true = Get the cell from the table )
  hct-search
  
  dup nil<> IF
    hcn-cell@ true
  ELSE
    drop false
  THEN
;


: hct-has?     ( c-addr u w:hct - f = Check if key is present in the table )
  hct-search nil<> 
;


( Special words )

: hct-count    ( w w:hct - u = Count the occurences of cell data in the table )
  0 -rot                     \ counter = 0
  dup hct-table@
  swap hct-size@             \ Do for the table
  0 DO
    >r
    r@ @
    BEGIN
      dup nil<>              \  Walk the list of nodes
    WHILE
      >r
      r@ hcn-cell@
      over = IF
        swap 1+ swap         \  If cell data found, then increase the counter
      THEN
      r> hcn-next@
    REPEAT
    drop
    r>
    cell+
  LOOP
  2drop
;


: hct-execute      ( ... xt w:hct - ... = Execute xt for every key and cell data in table )
  dup hct-table@
  swap hct-size@ 0 DO        \ Do for the whole table
    >r
    r@ @
    BEGIN
      dup nil<>              \ Iterate the lists in the table
    WHILE
      >r
      r@ hcn-cell@ swap      \ execute xt with key and cell data
      r@ hcn>key  @ swap
      r@ hcn>klen @ swap
      >r r@ execute r>       \ execute without private data
      r>
      hcn-next@
    REPEAT
    drop
    r>
    cell+
  LOOP
  2drop
;


( Private words )

: hct-emit-element    ( w c-addr u - = Emit the key and cell data of the node )
  type [char] = emit 0 .r [char] ; emit
;


( Inspection )

: hct-dump     ( w:hct - = Dump the table )
  ." hct:" dup . cr
  ."  table :" dup hct>table  ?  cr
  ."  size  :" dup hct>size   ?  cr
  ."  length:" dup hct>length ?  cr
  ."  load  :" dup hct>load   ?  cr
  ."  nodes :" ['] hct-emit-element swap hct-execute cr
;

[THEN]

\ ==============================================================================
