\ ==============================================================================
\
\                hnt - the base hash table in the ffl
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
\  $Date: 2007-11-11 07:41:31 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] hnt.version [IF]


include ffl/stc.fs
include ffl/hnn.fs


( hnt = Base Hash Table )
( The hnt module implements a base hash table that can handle variable size  )
( nodes. It is the base module for more specialised hash tables, for example )
( the hct module.                                                            )
  

1 constant hnt.version


( Hash table structure )

struct: hnt%       ( - n = Get the required space for the hash table structure )
  cell: hnt>table        \ array of pointers to the nodes
  cell: hnt>size         \ number of elements in the array of pointers
  cell: hnt>length       \ number of nodes in the hash table
  cell: hnt>load         \ load factor (* 100)
;struct


( Private words )

: hnt-allocate-table ( u - w:table = allocate a table with size u )
  dup 0= exp-invalid-parameters AND throw  
  
  dup cells allocate throw             \ table
  
  dup rot 0 DO                         \ fill table with nils
    dup nil!
    cell+
  LOOP
  drop
;


: hnt-table@       ( w:hnt - w:table = Get the table )
  hnt>table @
;


: hnt-size@        ( w:hnt - u:size = Get the size )
  hnt>size @
;


( Hash table creation, initialisation and destruction )

: hnt-init     ( u w:hnt - = Initialise the hash table with an initial size )
      dup  hnt>length 0!
  100 over hnt>load    !     \ load = 100%
      
  over hnt-allocate-table
  
  over hnt>table !
       hnt>size  !
;
     

: hnt-create   ( C: "name" u - R: - w:hnt = Create a named hash table with an initial size in the dictionary )
  create   here   hnt% allot   hnt-init
;


: hnt-new      ( u - w:hnt = Create a hash table with an initial size on the heap )
  hnt% allocate  throw  tuck hnt-init
;


: hnt-free     ( w:hnt - = Free the table from the heap )
  dup hnt-table@             \ Free the nodes:
  over hnt-size@ 0 DO        \ Do for the whole table
    dup @
    BEGIN
      dup nil<>              \ Iterate the lists in the table
    WHILE
      dup hnn-next@
      swap hnn-free          \ Free the node
    REPEAT
    drop
    cell+
  LOOP
  drop
  
  dup hnt-table@ free throw  \ free the table
  
  free throw                 \ free the hash table
;


( Module words )

: hnt+hash     ( c-addr u - u = Calculate the hash value of a key )
  0 -rot
  bounds ?DO
    dup 5 lshift +
    I c@ +
    1 chars 
  +LOOP
;

  
( Private words )

: hnt-table-pntr ( u:hash w:hnt - w:pntr = Get pointer in table for hash )
  >r 
  r@ hnt-size@ mod abs cells
  r> hnt-table@ +
;


: hnt-table-bounds ( u:start w:hnt - w:table-end w:table-start = Get the table bounds for DO )
  dup hnt-table@ >r
  hnt-size@ cells r@ +       \ table-end
  swap cells r> +            \ table-start
;


: hnt-insert-node ( u:size w:table w:hnn - = Insert the node in the table )
  >r
  swap
  r@ hnn-hash@
  swap mod cells
  +                          \ table element
  r@ r> rot @!               \ insert the new node, fetch the current node
  dup nil<> IF         
    2dup
    swap hnn>next !          \ if previous value, then link the node
         hnn>prev !
  ELSE
    2drop                    \ done
  THEN
;


( Member words )

: hnt-empty?   ( w:hnt - f = Check for an empty table )
  hnt>length @ 0=  
;


: hnt-length@  ( w:hnt - u = Get the number of nodes in the table )
  hnt>length @
;


: hnt-load@    ( w:hnt - u = Get the load factor [*100%] )
  hnt>load @
;


: hnt-load!    ( u w:hnt - = Set the load factor [*100%] )
  hnt>load !
;


: hnt-size!    ( u w:hnt - = Resize the hash table )
  >r
  dup hnt-allocate-table          \ allocate the new table
  0 r@ hnt-table-bounds DO        \ S: old-table new-size new-table
    I @                           \ Get node from table
    BEGIN
      dup nil<>                   \ Walk through the list in the table
    WHILE
      dup hnn-next@ swap
      dup hnn>next nil!           \ Clear the node from the old table and ..
      dup hnn>prev nil!
      2over rot hnt-insert-node   \ .. insert it in the new table
    REPEAT
    drop
    cell
  +LOOP
  r@ hnt>table @! free throw      \ store the new table, fetch the old and free
  r> hnt>size !
;


( Hash table words )

: hnt-search   ( c-addr u w:hnt - u:hash w:hnn = Search the node based on the key )
  over 0= exp-invalid-parameters AND throw
  
  >r
  2dup hnt+hash dup
  r> hnt-table-pntr @        \ offset in table -> node in table
                             \ c-addr u u:hash w:hnn
  BEGIN                      \ look for the key                             
    dup nil<> IF
      2dup hnn-hash@ = IF
        dup >r 2over  r@ hnn>key @ r> hnn>klen @ compare 0<>
      ELSE                   \ check hash and key
        true
      THEN
    ELSE
      false
    THEN
  WHILE
    hnn-next@                \ next hash table node
  REPEAT
  
  2>r 2drop 2r>              \ keep hash and node, drop key
;


: hnt-insert   ( w:hnn w:hnt - = Insert a node with a key in the table, double keys are NOT checked )
  >r 
                                      
  r@ hnt-size@ swap          \ offset in table, based on hash
  r@ hnt-table@ swap         \ table element
  
  hnt-insert-node            \ insert the node in the table
  
  r@ hnt>length 1+!          \ one more node in the hash table
  
                             \ test for rehash of table
  r@ hnt-length@  r@ hnt-size@  r@ hnt-load@ 100 */  > IF
    r@ hnt-size@ 2* 1+ r@ hnt-size!
  THEN
  rdrop
;


: hnt-delete   ( c-addr u w:hnt - nil | w:hnn = Delete the key from the table )
  >r
  r@ hnt-search nip
  
  dup nil<> IF                              \ if node found then
    r@ hnt>length 1-!                       \   one node less
    
    dup hnn-next@ dup nil<> IF              \   if next node present then
      hnn>prev over hnn-prev@ swap !        \     next->prev = prev node
    ELSE
      drop
    THEN
    
    dup hnn-prev@ dup nil<> IF              \   if prev node present then
      hnn>next over hnn-next@ swap !        \     prev->next = next node
    ELSE
      drop                                  \   else
      dup hnn-next@                         \     table-pntr = next node
      over hnn-hash@ r@ hnt-table-pntr !
    THEN
  THEN
  rdrop
;


: hnt-get      ( c-addr u w:hnt - nil | hnn = Get the node from the table )
  hnt-search nip
;


: hnt-has?     ( c-addr u w:hnt - f = Check if key is present in the table )
  hnt-search nip nil<> 
;


( Special words )

: hnt-execute      ( ... xt w:hnt - ... = Execute xt for every key and node in table )
  0 swap hnt-table-bounds DO  \ Do for the whole table
    I @
    BEGIN
      dup nil<>              \ Iterate the lists in the table
    WHILE
      >r
      r@            swap      \ execute xt with key and cell data
      r@ hnn>key  @ swap
      r@ hnn>klen @ swap
      >r r@ execute r>       \ execute without private data
      r>
      hnn-next@
    REPEAT
    drop
    cell
  +LOOP
  drop
;


( Inspection )

: hnt-dump     ( w:hnt - = Dump the table )
  ." hnt:" dup . cr
  ."  table :" dup hnt>table  ?  cr
  ."  size  :" dup hnt>size   ?  cr
  ."  length:" dup hnt>length ?  cr
  ."  load  :"     hnt>load   ?  cr
;

[THEN]

\ ==============================================================================
