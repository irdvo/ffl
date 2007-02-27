\ ==============================================================================
\
\            snl - the single linked node list in the ffl
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
\  $Date: 2007-02-27 06:06:15 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] snl.version [IF]


include ffl/stc.fs
include ffl/snn.fs


( snl = Single Linked Node List )
( The snl module implements a single linked list that can handle variable size )
( nodes. It is the base module for more specialised modules, for example the   )
( scl [single linked cell list] module. )
  

1 constant snl.version


( List structure )

struct: snl%       ( - n = Get the required space for the snl data structure )
  cell: snl>first
  cell: snl>last
  cell: snl>length
;struct


( List creation, initialisation and destruction )

: snl-init     ( w:snl - = Initialise the snl-list )
  dup snl>first   nil!
  dup snl>last    nil!
      snl>length    0!
;


: snl-create   ( C: "name" - R: - w:snl = Create a named snl-list in the dictionary )
  create   here   snl% allot   snl-init
;


: snl-new      ( - w:snl = Create a new snl-list on the heap )
  snl% allocate  throw  dup snl-init
;


: snl-free     ( w:snl - = Free the list from the heap )
  free  throw
;


( Member words )

: snl-length@  ( w:snl - u = Get the number of nodes in the list )
  snl>length @
;


: snl-empty?   ( w:snl - f = Check for empty list )
  snl-length@ 0=  
;


: snl-first@   ( w:snl - w:snn | nil  = Get the first node )
  snl>first @
;


: snl-last@    ( w:snl - w:snn | nil = Get the last node  )
  snl>last @
;


( Private words )

: snl+offset   ( n:index n:length - n:offset = Determine offset from index, incl. validation )
  tuck index2offset                    \ convert to offset
  dup rot 0 within                     \ check outside 0..length-1
  exp-index-out-of-range AND throw     \ raise exception
;


: snl-node  ( n:offset w:snl - w:prv | nil  w:snn | nil = Get the nth node in the list )
  nil -rot                   \ prv  = nil
  snl-first@                 \ cur  = first
  swap                       \ S: prv cur off
  BEGIN
    2dup 0> swap nil<> AND   \ while n>0 and cur<> nil do
  WHILE
    1- >r                    \  off--
    nip                      \  prv = cur
    dup snn-next@            \  cur = cur->next
    r>
  REPEAT
  drop
;


( List words )

: snl-append   ( w:snn w:snl - = Append a node in the list )
  dup  snl>length 1+!        \ snl.length++
  over swap
  dup snl-first@ nil= IF     \ If snl.first = nil Then
    2dup snl>first !         \   snl.first = snn
  THEN
  snl>last @!                \ snl.last = snn
  dup nil<> IF               \ If snl.last != nil Then
    2dup snn-next!           \   snl.last.next = snn
  THEN
  drop snn>next nil!         \ snn.next = nil
;


: snl-prepend  ( w:snn w:snl - = Prepend a node in the list )
  dup  snl>length 1+!        \ snl.length++
  over swap
  dup snl-last@ nil= IF      \ If snl.last = nil Then
    2dup snl>last !          \   snl.last = snn
  THEN
  snl>first @!               \ snl.first = snn
  swap snn-next!             \ snn.next = snl.first
;


: snl-insert-after  ( w:new w:ref w:snl - = Insert a new node after the reference node in the list )
  dup snl>length 1+!
  >r
  over swap snn>next @!      \ ref.next = new
  2dup swap snn-next!        \ new.next = ref.next
  r>
  swap nil= IF               \ If ref.next = nil Then
    snl>last !               \   snl.last = new
  ELSE
    2drop
  THEN
;


: snl-remove-first   ( w:snl - w:snn | nil = Remove the first node from the list )
  dup snl-first@
  dup nil<> IF               \ If first <> nil Then
    2dup snn-next@
    dup nil= IF              \   If first.next = nil Then
      over snl>last nil!     \     last.nil
    THEN
    swap snl>first !         \   first = first.next
    swap snl>length 1-!      \   length--
  ELSE
    nip
  THEN
;


: snl-remove-after   ( w:prv w:snl - w:snn | nil = Remove the node after the reference node from the list )
  swap
  dup nil= exp-invalid-parameters AND throw
  
  dup snn-next@ >r                \ S: snl prv
  r@ nil<> IF                     \ If prv.next <> nil Then
    nil r@ snn>next @!            \   prv.next.next = nil
    dup nil= IF                   \   If prv.next.next = nil Then
      >r
      2dup swap snl>last !        \     snl.last = prv
      r>
    THEN
    swap snn-next!                \   prv.next = prv.next.next
    snl>length 1-!
  ELSE
    2drop
  THEN
  r>
;
  

( Index words )

: snl-index?   ( n:index w:snl - f = Check if an index is valid in the list )
  snl-length@
  tuck index2offset
  swap 0 swap within
;


: snl-get      ( n:index w:snl - w:snn = Get the indexth node from the list )
  tuck snl-length@ snl+offset     \ S: snl offset
  swap snl-node nip               \ S: snn | nil
;


: snl-insert   ( w:snn n:index w:snl - = Insert a node before the indexth node in the list )
  tuck snl-length@ 1+ snl+offset  \ S: snn snl offset
  ?dup 0= IF
    snl-prepend
  ELSE
    over snl-length@ over = IF
      drop snl-append
    ELSE                          \ Insert the new node
      over snl-node drop          \ S: snn snl prv | nil
      dup  nil= exp-invalid-state AND throw
      swap snl-insert-after       \ Insert after prv
    THEN
  THEN
;


: snl-delete   ( n:index w:snl - w:snn = Delete the indexth node from the list )
  tuck snl-length@ snl+offset     \ S: snl offset
  ?dup 0= IF                      \ If offset = 0 Then
    snl-remove-first              \   First node
  ELSE                            \ Else
    over snl-node drop            \   Offset -> node S: snl prv
    swap snl-remove-after         \   Remove after prv
  THEN                            \ S: snn
;


( LIFO words )

: snl-push     ( w:snn w:snl - = Push the node at the top of the stack [= start of the list] )
  snl-prepend
;


: snl-pop      ( w:snl - w:snn | nil = Pop the node at the top of the stack [= start of the list] )
  snl-remove-first
;


: snl-tos      ( w:snl - w:snn | nil = Get the node at the top of the stack [= start of the list] )
  snl-first@
;


( FIFO words )

: snl-enqueue  ( w:snn w:snl - = Enqueue the node at the start of the queue [=end of the list] )
  snl-append
;


: snl-dequeue  ( w:snl - w:snn | nil = Dequeue the node at the end of the queue [= start of the list] )
  snl-remove-first
;


( Special words )

: snl-execute      ( ... xt w:snl - ... = Execute xt for every node in list )
  snl-first@                 \ walk = first
  BEGIN
    dup nil<>                \ while walk<>nil do
  WHILE
    2>r 
    2r@ swap execute         \  execute xt with node
    2r>
    snn-next@                \  walk = walk->next
  REPEAT
  2drop
;


: snl-reverse  ( w:snl - = Reverse or mirror the list )
  nil over
  snl-first@                 \ walk = first
  
  BEGIN
    dup nil<>
  WHILE                      \ while walk<>nil do
    dup snn-next@
    >r
    tuck snn-next!           \  walk->next = prev
    r>
  REPEAT
  2drop
  
  dup  snl-first@
  over dup snl-last@       
  swap snl>first !           \ first = last
  swap snl>last  !           \ last  = first
;


( Inspection )

: snl-dump     ( w:snl - = Dump the list )
  ." snl:" dup . cr
  ."  first :" dup snl>first ?  cr
  ."  last  :" dup snl>last  ?  cr
  ."  length:" dup snl>length ? cr
  
  ['] snn-dump swap snl-execute cr
;

[THEN]

\ ==============================================================================
