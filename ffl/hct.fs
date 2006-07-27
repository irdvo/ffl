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
\  $Date: 2006-07-27 18:08:01 $ $Revision: 1.2 $
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


( Public words )

: hct-init     ( u w:hct - = Initialise the hash table with an initial size )
  over 0= exp-invalid-parameters AND throw  
  
      dup  hct>length 0!
  100 over hct>load    !     \ load = 100%
      
  over cells allocate throw  \ table
  rot 2dup
  0 DO                       \ fill table with nils
    dup nil!
    cell+
  LOOP
  drop rot
  
  tuck hct>size  !
       hct>table !
;


: hct-create   ( u "name" - = Create a named hash table with an initial size in the dictionary )
  create   here   hct% allot   hct-init
;


: hct-new      ( u - w:hct = Create a new hash table with an initial size on the heap )
  hct% allocate  throw  tuck hct-init
;


: hct-free     ( w:hct - = Free the table from the heap )
  \ ToDo
  
  free  throw
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
  \ ToDo
;


( Private words )

: hct-search-node ( c-addr u w:hct - w:hcn = Search the node based on the key )
  over 0= exp-invalid-parameters AND throw
  
  >r
  2dup hct+hash dup
  r@ hct>size @ mod cells    \ offset in table, based on hash
  r> hct>table @ + @         \ hash node in table
                             \ c-addr u u:hash w:hcn
  BEGIN                      \ look for the key
    dup nil<> IF
      2dup hcn>hash @ = IF
        dup >r 2over  r@ hcn>key @ r> hcn>klen @ compare 0<>
      ELSE                   \ check hash and key
        true
      THEN
    ELSE
      false
    THEN
  WHILE
    hcn>next @               \ next hash table node
  REPEAT
  
  nip nip nip
;


( Table words )

: hct-insert   ( w c-addr u w:hct - = Insert a cell with a key in the table )
  >r 2dup hct+hash           \ calculate the hash
  dup >r hcn-new dup r>      \ new hash table node
  
  r@ hct>size @ mod cells    \ offset in table, based on hash
  r@ hct>table @ +           \ table element
  @!                         \ insert the new node, fetch the current node in table
  dup nil<> IF         
    2dup
    swap hcn>next !          \ if previous value, then link the node
         hcn>prev !
  ELSE
    2drop                    \ done
  THEN
  r> hct>length 1+!          \ one more node in the hash table
  \ ToDo resize table by load
;


: hct-delete   ( c-addr u w:hct - false | w true = Delete key from the table )
  \ ToDo
;


: hct-get      ( c-addr u w:hct - false | w true = Get the cell from the table )
  hct-search-node
  
  dup nil<> IF
    hcn>cell @ true
  ELSE
    drop false
  THEN
;


: hct-has?     ( c-addr u w:hct - f = Check if key is present in the table )
  hct-search-node nil<> 
;


( Special words )

: hct-count    ( w w:hct - u = Count the occurences of cell data in the table )
  \ ToDo
;


: hct-execute      ( ... xt w:hct - ... = Execute xt for every cell data in table )
  \ ToDo
;


( Inspection )

: hct-dump     ( w:hct - = Dump the table )
  ." hct:" dup . cr
  ."  table :" dup hct>table  ?  cr
  ."  size  :" dup hct>size   ?  cr
  ."  length:" dup hct>length ?  cr
  ."  load  :"     hct>load   ?  cr
  
  \ ToDo
;

[THEN]

\ ==============================================================================
