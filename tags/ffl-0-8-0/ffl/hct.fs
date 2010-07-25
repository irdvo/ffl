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
\  $Date: 2008-02-21 20:31:18 $ $Revision: 1.15 $
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

hnt% constant hct%  ( -- n = Get the required space for a hash table variable )


( Hash table creation, initialisation and destruction )

: hct-init     ( u hct -- = Initialise the hash table with an initial size u )
  hnt-init
;
     

: hct-(free)   ( hct -- = Free the nodes from the hash table )
  ['] hcn-free swap hnt-(free)
;

: hct-create   ( u "<spaces>name" -- ; -- hct = Create a named hash table with an initial size u in the dictionary )
  hnt-create
;


: hct-new      ( u -- hct = Create a hash table with an initial size u on the heap )
  hnt-new
;


: hct-free     ( hct -- = Free the hash table from the heap )
  dup hct-(free)
  
  free throw
;


( Module words )

: hct+hash     ( c-addr1 u1 -- u2 = Calculate the hash value of a key )
  hnt+hash
;

  
( Member words )

: hct-empty?   ( hct -- flag = Check if the table is empty )
  hnt-empty?
;


: hct-length@  ( hct -- u = Get the number of nodes in the table )
  hnt-length@
;


: hct-load@    ( hct -- u = Get the load factor [*100%] )
  hnt-load@
;


: hct-load!    ( u hct -- = Set the load factor [*100%] )
  hnt-load!
;


: hct-size!    ( u hct -- = Resize the hash table to size u )
  hnt-size!
;


( Hash table words )

: hct-insert   ( x c-addr u hct -- = Insert cell data x with the key c-addr u in the table )
  >r 
  2dup r@ hnt-search         \ Search for key
    
  nil<>? IF                  \ if already present then
    nip nip nip
    hcn>cell !               \  update cell
  ELSE                       \ else S: w c-addr u u:hash
    hcn-new                  \  new hash table node
  
    r@ hnt-insert            \ insert the node in the table
  THEN
  rdrop
;


: hct-delete   ( c-addr u hct -- false | x true = Delete the key c-addr u from the table, return the cell data related to the key )
  hnt-delete 
  nil<>? IF
    dup  hcn-cell@                \ Key deleted, then return cell data
    swap hcn-free
    true
  ELSE
    false
  THEN
;


: hct-get      ( c-addr u hct -- false | x true = Get the cell data x related to the key c-addr u from the table )
  hnt-get 
  nil<>? IF
    hcn-cell@
    true
  ELSE
    false
  THEN
;


: hct-has?     ( c-addr u hct -- flag = Check if the key c-addr u is present in the table )
  hnt-has?
;


( Special words )

: hct-count    ( x hct -- u = Count the number of occurrences of the cell data x in the table )
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


: hct-execute      ( i*x xt hct -- j*x = Execute xt for every key and cell data in table )
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

: hct-emit-element    ( x c-addr u -- = Emit the key and cell data of the node )
  type [char] = emit 0 .r [char] ; emit
;


( Inspection )

: hct-dump     ( hct -- = Dump the hash table )
  dup hnt-dump
  ."  nodes :" ['] hct-emit-element swap hct-execute cr
;

[THEN]

\ ==============================================================================
