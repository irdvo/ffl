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
\  $Date: 2007-01-07 08:07:01 $ $Revision: 1.6 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] dnl.version [IF]


include ffl/stc.fs
include ffl/dnn.fs


( dnl = Double Linked Node List )
( The dnl module implements a double linked list that can handle variable size )
( nodes. It is the base module for more specialised modules, for example the   }
{ dcl [double linked cell list] module. )
  

1 constant dnl.version


( List structure )

struct: dnl%       ( - n = Get the required space for the dnl data structure )
  cell: dnl>first
  cell: dnl>last
  cell: dnl>length
;struct


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


( Private words )

: dnl+offset   ( n:index n:length - n:offset = Determine offset from index, incl. validation )
  tuck index2offset                    \ convert to offset
  dup rot 0 within                     \ check outside 0..length-1
  exp-index-out-of-range AND throw     \ raise exception
;


: dnl-node  ( n:offset w:dnl - w:dnn | nil = Get the nth node in the list )
  dnl-first@                 \ cur  = first
  BEGIN
    2dup nil<> swap 0> AND   \ while n>0 and cur<> nil do
  WHILE
    dnn-next@                \  cur  = cur->next
    swap 1- swap             \  n--
  REPEAT
  nip
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


: dnl-insert-before  ( w:new w:ref w:dnl - = Insert a new node before the reference node in the list )
  dup dnl>length 1+!
  >r
  2dup swap dnn-next!        \ new.next = ref
  over swap dnn>prev @!      \ ref.prev = new
  2dup swap dnn-prev!        \ new.prev = ref.prev
  r>
  over nil= IF               \ If ref.prev = nil Then
    nip dnl>first !          \   dnl.first = new
  ELSE                       \ Else
    drop dnn-next!           \   ref.prev.next = new
  THEN
;


: dnl-insert-after  ( w:new w:ref w:dnl - = Insert a new node after the reference node in the list )
  dup dnl>length 1+!
  >r
  2dup swap dnn-prev!        \ new.prev = ref
  over swap dnn>next @!      \ ref.next = new
  2dup swap dnn-next!        \ new.next = ref.next
  r>
  over nil= IF               \ If ref.next = nil Then
    nip dnl>last !           \   dnl.last = new
  ELSE                       \ Else
    drop dnn-prev!           \   ref.next.prev = new
  THEN
;


: dnl-remove   ( w:dnn w:dnl - = Remove a node from the list )
  swap
  dup nil= exp-invalid-parameters AND throw
  
  nil over dnn>next @! swap
  nil swap dnn>prev @!            \ S: dnl next prev
  
  dup nil= IF                     \ If prev = nil Then
    >r 2dup swap dnl>first ! r>   \   dnl.first = next
  ELSE                            \ Else
    2dup dnn-next!                \   prev.next = next
  THEN
  swap
  dup nil= IF                     \ If next = nil Then
    drop over dnl>last !          \   dnl.last = prev
  ELSE                            \ Else
    dnn-prev!                     \   next.prev = prev
  THEN                            \ S: dnl
  
  dnl>length 1-!
;
  

( Index words )

: dnl-index?   ( n:index w:dnl - f = Check if an index is valid in the list )
  dnl-length@
  tuck index2offset
  swap 0 swap within
;


: dnl-get      ( n:index w:dnl - w:dnn = Get the indexth node from the list )
  tuck dnl-length@ dnl+offset     \ S: dnl offset
  swap dnl-node                   \ S: dnn | nil
;


: dnl-insert   ( w:dnn n:index w:dnl - = Insert a node before the indexth node in the list )
  tuck dnl-length@ 1+ dnl+offset  \ S: dnn dnl offset
  ?dup 0= IF
    dnl-prepend
  ELSE
    over dnl-length@ over = IF
      drop dnl-append
    ELSE                          \ Insert the new node
      over dnl-node               \ S: dnn2 dnl dnn1 | nil
      dup  nil= exp-invalid-state AND throw
      swap dnl-insert-before      \ Insert before dnn1
    THEN
  THEN
;


: dnl-delete   ( n:index w:dnl - w:dnn = Delete the indexth node from the list )
  tuck dnl-length@ dnl+offset     \ S: dnl offset
  ?dup 0= IF                      \ If offset = 0 Then
    dup dnl-first@                \   First node
  ELSE                            \ Else
    over dnl-length@ 1- over = IF \   If offset = length-1 Then
      drop dup dnl-last@          \     Last node
    ELSE                          \   Else
      over dnl-node               \     Offset -> node
    THEN
  THEN                            \ S: dnl dnn
  dup rot dnl-remove              \ Remove the node
;


( LIFO words )

: dnl-push     ( w:dnn w:dnl - = Push the node at the end of the list )
  dnl-append
;


: dnl-pop      ( w:dnl - w:dnn | nil = Pop the node at the end of the list )
  dup dnl-last@              \ dnn = dnl.last 
  dup nil<> IF               \ If dnn != nil Then
    dup rot dnl-remove
  ELSE
    nip
  THEN
;


: dnl-tos      ( w:dnl - w:dnn | nil = Get the node at the end of the list )
  dnl-last@
;


( FIFO words )

: dnl-enqueue  ( w:dnn w:dnl - = Enqueue the node at the start of the list )
  dnl-prepend
;


: dnl-dequeue  ( w:dnl - w:dnn | nil = Dequeue the node at the end of the list )
  dnl-pop
;


( Special words )

: dnl-execute      ( ... xt w:dnl - ... = Execute xt for every node in list )
  dnl-first@                 \ walk = first
  BEGIN
    dup nil<>                \ while walk<>nil do
  WHILE
    2>r 
    2r@ swap execute         \  execute xt with node
    2r>
    dnn-next@                \  walk = walk->next
  REPEAT
  2drop
;


: dnl-reverse  ( w:dnl - = Reverse or mirror the list )
  dup dnl-first@             \ walk = first
  BEGIN
    dup nil<>
  WHILE                      \ while walk<>nil do
    dup  dnn-next@
    swap
    2dup dnn>prev @!
    swap dnn-next!
  REPEAT
  drop
  
  dup  dnl-first@
  swap dup
  dnl-last@ over
  dnl>first !                \ first = last
  dnl>last  !                \ last  = first
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
