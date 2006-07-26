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
\  $Date: 2006-07-26 06:50:20 $ $Revision: 1.1 $
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

: hct-init     ( w:hct - = Initialise the hct-list )
  dup hct>table   nil!
  dup hct>size      0!
  dup hct>length    0!
  100 over hct>load  !
  101 swap htc-size!
;


: hct-create   ( "name" - = Create a named hash table in the dictionary )
  create   here   hct% allot   hct-init
;


: hct-new      ( - w:hct = Create a new hash table on the heap )
  hct% allocate  throw  dup hct-init
;


: hct-free     ( w:hct - = Free the table from the heap )
  \ ToDo
  
  free  throw
;



( Module words )

: hct+hash     ( c-addr u - u = Calculate the hash value of a key )
  
( Member words )

: hct-empty?   ( w:hct - f = Check for empty table )
  hct>length @ 0=  
;


: hct-length@  ( w:hct - u = Get the number of nodes in the list )
  hct>length @
;


: hct-load@    ( w:hct - u = Get the load factor [*100] )
  hct>load @
;


: hct-load!    ( u w:hct - = Set the load factor [*100] )
  hct>load !
;


: hct-size!    ( u w:hct - = Resize the hash table )
  \ ToDo
;


( Table words )

: hct-insert-with-hash ( w c-addr u u:hash w:hct - = Insert a cell with a key and hash in the table )
  \ ToDo
;


: hct-insert   ( w c-addr u w:hct - = Insert a cell with a key in the table )
  \ ToDo
;


: hct-delete   ( c-addr u w:hct - false | w true = Delete key from the table )
  \ ToDo
;


: hct-has?     ( c-addr u w:hct - f = Check if key is present in the table )
  \ ToDo
;


: hct-get      ( c-addr u w:hct - false | w true = Get the cell from the table )
  \ ToDo
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
  ."  table :" dup hct>first  ?  cr
  ."  size  :" dup hct>last   ?  cr
  ."  length:" dup hct>length ?  cr
  ."  load  :"     hct>load   ?  cr
  
  \ ToDo
;

[THEN]

\ ==============================================================================
