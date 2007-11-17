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
\  $Date: 2007-11-17 07:47:22 $ $Revision: 1.12 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] hct.version [IF]


include ffl/hnt.fs
include ffl/hcn.fs


( hct = Hash Cell Table )
( The hct module implements a hash table that stores cell wide data.)

  

2 constant hct.version


( Hash table structure )

hnt% constant hct%  ( - n = Get the required space for the hash table structure )


( Hash table creation, initialisation and destruction )

: hct-init     ( u w:hct - = Initialise the hash table with an initial size )
  hnt-init
;
     

: hct-create   ( C: "name" u - R: - w:hct = Create a named hash table with an initial size in the dictionary )
  hnt-create
;


: hct-new      ( u - w:hct = Create a hash table with an initial size on the heap )
  hnt-new
;


: hct-free     ( w:hct - = Free the hash table from the heap )
  hnt-free
;


( Module words )

: hct+hash     ( c-addr u - u = Calculate the hash value of a key )
  hnt+hash
;

  
( Member words )

: hct-empty?   ( w:hct - f = Check for empty table )
  hnt-empty?
;


: hct-length@  ( w:hct - u = Get the number of nodes in the table )
  hnt-length@
;


: hct-load@    ( w:hct - u = Get the load factor [*100%] )
  hnt-load@
;


: hct-load!    ( u w:hct - = Set the load factor [*100%] )
  hnt-load!
;


: hct-size!    ( u w:hct - = Resize the hash table )
  hnt-size!
;


( Hash table words )

: hct-insert   ( w:data c-addr u w:hct - = Insert cell data with a key in the table )
  >r 
  2dup r@ hnt-search         \ Search for key
    
  dup nil<> IF               \ if already present then
    nip nip nip
    hcn>cell !               \  update cell
  ELSE                       \ else
    drop                     \  S: w c-addr u u:hash
    hcn-new                  \  new hash table node
  
    r@ hnt-insert            \ insert the node in the table
  THEN
  rdrop
;


: hct-delete   ( c-addr u w:hct - false | w true = Delete the key from the table )
  hnt-delete 
  dup nil<> IF
    dup  hcn-cell@                \ Key deleted, then return cell data
    swap hcn-free
    true
  ELSE
    drop
    false
  THEN
;


: hct-get      ( c-addr u w:hct - false | w true = Get the cell data from the table )
  hnt-get 
  dup nil<> IF
    hcn-cell@
    true
  ELSE
    drop
    false
  THEN
;


: hct-has?     ( c-addr u w:hct - f = Check if a key is present in the table )
  hnt-has?
;


( Special words )

: hct-count    ( w:data w:hct - u = Count the occurences of cell data in the table )
  0 -rot                     \ counter = 0
  0 swap hnt-table-bounds DO \ Do for the table
    I @
    BEGIN
      dup nil<>              \  Walk the list of nodes
    WHILE
      >r
      r@ hcn-cell@
      over = IF
        swap 1+ swap         \  If cell data found, then increase the counter
      THEN
      r> hnn-next@
    REPEAT
    drop
    cell
  +LOOP
   drop
;


: hct-execute      ( ... xt w:hct - ... = Execute xt for every key and cell data in table )
  0 swap hnt-table-bounds DO  \ Do for the whole table
    I @
    BEGIN
      dup nil<>              \ Iterate the lists in the table
    WHILE
      >r
      r@ hcn-cell@ swap      \ execute xt with key and cell data
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


( Private words )

: hct-emit-element    ( w:data c-addr u - = Emit the key and cell data of the node )
  type [char] = emit 0 .r [char] ; emit
;


( Inspection )

: hct-dump     ( w:hct - = Dump the table )
  dup hnt-dump
  ."  nodes :" ['] hct-emit-element swap hct-execute cr
;

[THEN]

\ ==============================================================================
