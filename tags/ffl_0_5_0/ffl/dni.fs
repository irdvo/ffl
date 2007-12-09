\ ==============================================================================
\
\         dni - the double linked node list iterator in the ffl
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
\  $Date: 2007-01-11 19:22:04 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] dni.version [IF]


include ffl/stc.fs
include ffl/dnl.fs
include ffl/dnn.fs


( dni = Double Linked Node List Iterator )
( The dni module implements an iterator on the double linked node list [dnl]. )


1 constant dni.version


( Iterator structure )

struct: dni%       ( - n = Get the required space for a dni data structure )
  cell: dni>dnl
  cell: dni>walk
;struct 


( Iterator creation, initialisation and destruction )

: dni-init     ( w:dnl w:dni - = Initialise the iterator with a dnl list )
  tuck dni>dnl     !
       dni>walk  nil!
;


: dni-create   ( C: w:dnl "name" - R: - w = Create a named iterator in the dictionary )
  create 
    here  dni% allot  dni-init
;


: dni-new      ( w:dnl - w:dni = Create an iterator on the heap )
  dni% allocate  throw  tuck dni-init
;


: dni-free     ( w:dni - = Free iterator from heap )
  free throw
;


( Member words )

: dni-get      ( w:dni - w:dnn | nil = Get the current node )
  dni>walk @
;


( Iterator words )

: dni-first    ( w:dni - w:dnn | nil  = Move the iterator to the first node )
  dup dni>dnl @             
  dnl-first@
  dup rot dni>walk !         \ walk = dnl.first
;


: dni-next     ( w:dni - w:dnn | nil = Move the iterator to the next node )
  dni>walk 
  dup @
  dup nil<> IF               \ if walk <> nil then
    dnn-next@                \   walk = walk.next
    dup rot !
  ELSE                       \ else
    exp-invalid-state throw  \   exception
  THEN
;


: dni-prev     ( w:dni - w:dnn | nil = Move the iterator to the previous node )
  dni>walk 
  dup @
  dup nil<> IF               \ if walk <> nil then
    dnn-prev@                \   walk = walk.prev
    dup rot !
  ELSE                       \ else
    exp-invalid-state throw  \   exception
  THEN
;


: dni-last     ( w:dni - w:dnn | nil  = Move the iterator to the last node )
  dup dni>dnl @             
  dnl-last@
  dup rot dni>walk !         \ walk = dnl.last
;


: dni-first?   ( w:dni - f = Check if the iterator is on the first node )
  dup dni>dnl @
  dnl-first@
  dup nil= IF
    2drop
    false
  ELSE
    swap dni-get =
  THEN
;


: dni-last?    ( w:dni - f = Check if the iterator is on the last node )
  dup dni>dnl @
  dnl-last@
  dup nil= IF
    2drop
    false
  ELSE
    swap dni-get =
  THEN
;


( Inspection )

: dni-dump     ( w:dni - = Dump the iterator )
  ." dni:" dup . cr
  ."  dnl :" dup dni>dnl  ?  cr
  ."  walk:"     dni>walk  ?  cr
;

[THEN]

\ ==============================================================================