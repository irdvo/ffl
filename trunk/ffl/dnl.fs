\ ==============================================================================
\
\            dnl - the double linked node list in the ffl
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
\  $Date: 2006-12-30 06:19:02 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] dnl.version [IF]


include ffl/stc.fs
include ffl/dnn.fs


( dnl = Double Linked Node List )
( The dnl module implements a double linked list that can handle variable size )
( nodes. It is the base module for ..afgeleide.. modules, for example the dcl  )
( module. )
  

1 constant dnl.version


( List structure )

struct: dnl%       ( - n = Get the required space for the dnl data structure )
  cell: dnl>first
  cell: dnl>last
  cell: dnl>length
;struct


( Private words )

: dnl-offset   ( n w:dnl - n = Determine offset from index, incl. validation )
  dnl>length @
  
  over 0< IF                 \ if index < 0 then
    tuck + swap              \   index = index + length
  THEN
  
  over <= over 0< OR IF      \ if index < 0 or index >= length
    exp-index-out-of-range throw 
  THEN
;


: dnl-indexth  ( n w:dnl - w:dnn = Get the nth element in the list )
  \ ToDo
  nil -rot                   \ prev = nil
  dnl>first @                \ cur  = first
  
  BEGIN
    2dup nil<> swap 0> AND   \ while n>0 and cur<> nil do
  WHILE
    rot drop dup -rot        \  prev = cur
    dnn>next @               \  cur  = cur->next
    swap 1- swap             \  n--
  REPEAT
  
  nip
;


( List creation, initialisation and destruction )

: dnl-init     ( w:dnl - = Initialise the dnl-list )
  dup dnl>first   nil!
  dup dnl>last    nil!
      dnl>length    0!
;


: dnl-create   ( C: "name" - R: - w:dnl = Create a named dnl-list in the dictionary )
  create   here   dnl% allot   dnl-init
;


: dnl-new      ( - w:dnl = Create a new dnl-list on the heap )
  dnl% allocate  throw  dup dnl-init
;


: dnl-free     ( w:dnl - = Free the list from the heap )
  free  throw
;


( Member words )

: dnl-length@  ( w:dnl - u = Get the number of nodes in the list )
  dnl>length @
;


: dnl-empty?   ( w:dnl - f = Check for empty list )
  dnl-length@ 0=  
;


: dnl-first@   ( w:dnl - w:dnn | nil  = Get the first node )
  dnl>first @
;


: dnl-last@    ( w:dnl - w:dnn | nil = Get the last node  )
  dnl>last @
;


( List words )

: dnl-append   ( w:dnn w:dnl - = Append a node in the list )
  dup  dnl>length 1+!        \ dnl.length++
  over swap
  dup dnl-first@ nil= IF     \ If dnl.first = nil Then
    2dup dnl>first !         \   dnl.first = dnn
  THEN
  dnl>last @!                \ dnl.last = dnn
  dup nil<> IF               \ If dnl.last != nil Then
    2dup dnn-next!           \   dnl.last.next = dnn
  THEN
  over dnn-prev!             \ dnn.prev = dnl.last
  dnn>next nil!              \ dnn.next = nil
;


: dnl-prepend  ( w:dnn w:dnl - = Prepend a node in the list )
  dup  dnl>length 1+!        \ dnl.length++
  over swap
  dup dnl-last@ nil= IF      \ If dnl.last = nil Then
    2dup dnl>last !          \   dnl.last = dnn
  THEN
  dnl>first @!               \ dnl.first = dnn
  dup nil<> IF               \ If dnl.first != nil Then
    2dup dnn-prev!           \   dnl.first.prev = dnn
  THEN
  over dnn-next!             \ dnn.next = dnl.first
  dnn>prev nil!              \ dnn.prev = nil
;


( Index words )

: dnl-get      ( n:index w:dnl - w:dnn = Get the node from the indexth node from the list )
  \ ToDo
;


: dnl-insert   ( w:dnn n:index w:dnl - = Insert a node after the indexth node in the list )
  \ ToDo
;


: dnl-delete   ( n:index w:dnl - w:dnn = Delete the indexth node from the list )
  \ ToDo
;


( FIFO words )

: dnl-push     ( w:dnn w:dnl - = Push the node at the end of the list )
  dnl-append
;


: dnl-pop      ( w:dnl - w:dnn | nil = Pop the node at the end of the list )
  dup dnl-last@              \ dnn = dnl.last 
  dup nil<> IF               \ If dnn != nil Then
    swap                     
    dup dnl>length 1-!       \ dnl.length--
    over dnn-prev@
    dup nil<> IF             \ If dnn.prev <> nil Then
      nil over dnn-next!     \   dnn.prev.next = nil
    ELSE                     \ Else
      over dnl>first nil!    \   dnl.first = nil
    THEN
    swap dnl>last !          \ dnl.last = dnn.prev
    nil over dnn-prev!       \ dnn.prev = nil
  ELSE
    nip
  THEN
;


: dnl-tos      ( w:dnl - w:dnn | nil = Get the node at the end of the list )
  dnl-last@
;


( LIFO words )

: dnl-enqueue  ( w:dnn w:dnl - = Enqueue the node at the start of the list )
  dnl-prepend
;


: dnl-dequeue  ( w:dnl - w:dnn | nil = Dequeue the node at the end of the list )
  dnl-pop
;


( Special words )

: dnl-execute      ( ... xt w:dnl - ... = Execute xt for every node in list )
  \ ToDo
  dnl>first @                \ walk = first
  BEGIN
    dup nil<>                \ while walk<>nil do
  WHILE
    dup >r 
    swap 
    dup >r execute           \  execute xt with node
    r> r>
    dnn>next @               \  walk = walk->next
  REPEAT
  2drop
;


: dnl-reverse  ( w:dnl - = Reverse or mirror the list )
  \ ToDo
  nil over
  dnl>first @                \ walk = first
  
  BEGIN
    dup nil<>
  WHILE                      \ while walk<>nil do
    dup dnn>next @
    >r
    tuck dnn>next !          \  walk->next = prev
    r>
  REPEAT
  2drop
  
  dup  dnl>first @
  over dup dnl>last @       
  swap dnl>first !           \ first = last
  swap dnl>last  !           \ last  = first
;


( Inspection )

: dnl-dump     ( w:dnl - = Dump the list )
  ." dnl:" dup . cr
  ."  first :" dup dnl>first ?  cr
  ."  last  :" dup dnl>last  ?  cr
  ."  length:" dup dnl>length ? cr
  
  ['] dnn-dump swap dnl-execute cr
;

[THEN]

\ ==============================================================================
