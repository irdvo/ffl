\ ==============================================================================
\
\         dni - the generic double linked list iterator in the ffl
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
\  $Date: 2008-06-25 16:48:34 $ $Revision: 1.5 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] dni.version [IF]


include ffl/stc.fs
include ffl/dnl.fs
include ffl/dnn.fs


( dni = Generic Double Linked List Iterator )
( The dni module implements an iterator on the generic double linked list [dnl]. )


1 constant dni.version


( Iterator structure )

begin-structure dni%       ( -- n = Get the required space for a dni variable )
  field: dni>dnl
  field: dni>walk
end-structure


( Iterator creation, initialisation and destruction )

: dni-init     ( dnl dni -- = Initialise the iterator with a dnl list )
  tuck dni>dnl     !
       dni>walk  nil!
;


: dni-create   ( dnl "<spaces>name" -- ; -- dni = Create a named iterator in the dictionary with a dnl list )
  create 
    here  dni% allot  dni-init
;


: dni-new      ( dnl -- dni = Create an iterator on the heap )
  dni% allocate  throw  tuck dni-init
;


: dni-free     ( dni -- = Free the iterator from the heap )
  free throw
;


( Member words )

: dni-get      ( dni -- dnn | nil = Get the current node )
  dni>walk @
;


( Iterator words )

: dni-first    ( dni -- dnn | nil  = Move the iterator to the first node, return this node )
  dup dni>dnl @             
  dnl-first@
  dup rot dni>walk !         \ walk = dnl.first
;


: dni-next     ( dni -- dnn | nil = Move the iterator to the next node, return this node )
  dni>walk 
  dup @
  dup nil<> IF               \ if walk <> nil then
    dnn-next@                \   walk = walk.next
    dup rot !
  ELSE                       \ else
    exp-invalid-state throw  \   exception
  THEN
;


: dni-prev     ( dni -- dnn | nil = Move the iterator to the previous node, return this node )
  dni>walk 
  dup @
  dup nil<> IF               \ if walk <> nil then
    dnn-prev@                \   walk = walk.prev
    dup rot !
  ELSE                       \ else
    exp-invalid-state throw  \   exception
  THEN
;


: dni-last     ( dni -- dnn | nil  = Move the iterator to the last node, return this node )
  dup dni>dnl @             
  dnl-last@
  dup rot dni>walk !         \ walk = dnl.last
;


: dni-first?   ( dni -- flag = Check if the iterator is on the first node )
  dup dni>dnl @
  dnl-first@
  dup nil= IF
    2drop
    false
  ELSE
    swap dni-get =
  THEN
;


: dni-last?    ( dni -- flag = Check if the iterator is on the last node )
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

: dni-dump     ( dni -- = Dump the iterator variable )
  ." dni:" dup . cr
  ."  dnl :" dup dni>dnl  ?  cr
  ."  walk:"     dni>walk  ?  cr
;

[THEN]

\ ==============================================================================
