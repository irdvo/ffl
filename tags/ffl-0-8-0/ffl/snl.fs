\ ==============================================================================
\
\           snl - the generic single linked list in the ffl
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
\  $Date: 2008-03-05 20:35:13 $ $Revision: 1.7 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] snl.version [IF]


include ffl/stc.fs
include ffl/snn.fs


( snl = Generic Single Linked List )
( The snl module implements a single linked list that can store variable size  )
( nodes. It is the base module for more specialized modules, for example the   )
( single linked cell list [scl] module. )


2 constant snl.version


( List structure )

begin-structure snl%       ( -- n = Get the required space for a snl variable )
  field: snl>first
  field: snl>last
  field: snl>length
end-structure


( Private defer )

defer snl.remove-first

( List creation, initialisation and destruction )

: snl-init     ( snl -- = Initialise the snl list )
  dup snl>first   nil!
  dup snl>last    nil!
      snl>length    0!
;


: snl-(free)   ( xt scl -- = Free the nodes from the heap using xt )
  swap
  BEGIN
    over snl.remove-first nil<>?      \ while remove first node <> nil do
  WHILE
    over execute                      \   Free the node
  REPEAT
  2drop
;


: snl-create   ( "<spaces>name" -- ; -- snl = Create a named snl list in the dictionary )
  create   here   snl% allot   snl-init
;


: snl-new      ( -- snl = Create a new snl list on the heap )
  snl% allocate  throw  dup snl-init
;


: snl-free     ( snl -- = Free the list from the heap )
  ['] snn-free over snl-(free)
  
  free  throw
;


( Member words )

: snl-length@  ( snl -- u = Get the number of nodes in the list )
  snl>length @
;


: snl-empty?   ( snl -- flag = Check for an empty list )
  snl-length@ 0=  
;


: snl-first@   ( snl -- snn | nil  = Get the first node from the list )
  snl>first @
;


: snl-last@    ( snl -- snn | nil = Get the last node from the list )
  snl>last @
;


( Private words )

: snl+offset   ( n1 n2 -- n3 = Determine the offset n3 from the index n1 and length n2, incl. validation )
  tuck index2offset                    \ convert to offset
  dup rot 0 within                     \ check outside 0..length-1
  exp-index-out-of-range AND throw     \ raise exception
;


: snl-node  ( n snl -- snn1 | nil  snn2 | nil = Get the nth node in the list, return this node snn2 and its leading node snn1 )
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

: snl-append   ( snn snl -- = Append the node snn to the list )
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


: snl-prepend  ( snn snl -- = Prepend the node snn in the list )
  dup  snl>length 1+!        \ snl.length++
  over swap
  dup snl-last@ nil= IF      \ If snl.last = nil Then
    2dup snl>last !          \   snl.last = snn
  THEN
  snl>first @!               \ snl.first = snn
  swap snn-next!             \ snn.next = snl.first
;


: snl-insert-after  ( snn1 snn2 snl -- = Insert the node snn1 after the reference node snn2 in the list )
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


: snl-remove-first   ( snl -- snn | nil = Remove the first node from the list, return the removed node )
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
' snl-remove-first is snl.remove-first


: snl-remove-after   ( snn1 snl -- snn2 | nil = Remove the node after the reference node snn1 from the list, return the removed node )
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

: snl-index?   ( n snl -- flag = Check if the index n is valid in the list )
  snl-length@
  tuck index2offset
  swap 0 swap within
;


: snl-get      ( n snl -- snn = Get the nth node from the list )
  tuck snl-length@ snl+offset     \ S: snl offset
  swap snl-node nip               \ S: snn | nil
;


: snl-insert   ( snn n snl -- = Insert a node before the nth node in the list )
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


: snl-delete   ( n snl -- snn = Delete the nth node from the list, return the deleted node )
  tuck snl-length@ snl+offset     \ S: snl offset
  ?dup 0= IF                      \ If offset = 0 Then
    snl-remove-first              \   First node
  ELSE                            \ Else
    over snl-node drop            \   Offset -> node S: snl prv
    swap snl-remove-after         \   Remove after prv
  THEN                            \ S: snn
;


( LIFO words )

: snl-push     ( snn snl -- = Push the node snn at the top of the stack [= start of the list] )
  snl-prepend
;


: snl-pop      ( snl -- snn | nil = Pop the node at the top of the stack [= start of the list], return the popped node )
  snl-remove-first
;


: snl-tos      ( snl -- snn | nil = Get the node at the top of the stack [= start of the list], return this node )
  snl-first@
;


( FIFO words )

: snl-enqueue  ( snn snl -- = Enqueue the node snn at the start of the queue [=end of the list] )
  snl-append
;


: snl-dequeue  ( snl -- snn | nil = Dequeue the node at the end of the queue [= start of the list], return this node )
  snl-remove-first
;


( Special words )

: snl-execute      ( i*x xt snl -- j*x = Execute xt for every node in list )
  snl-first@                 \ walk = first
  BEGIN
    nil<>?                   \ while walk<>nil do
  WHILE
    2>r 
    2r@ swap execute         \  execute xt with node
    2r>
    snn-next@                \  walk = walk->next
  REPEAT
  drop
;


: snl-execute?     ( i*x xt snl -- j*x flag = Execute xt for every node in the list until xt returns true )
  snl-first@                 \ walk = first
  false                      \ keep searching
  BEGIN
    over nil<> over 0= AND   \ while walk <> nil and keep searching do
  WHILE
    drop
    2>r
    2r@ swap execute         \   execute xt with node
    2r>
    snn-next@                \   walk = walk->next
    rot                      \   keep searching to tos
  REPEAT
  nip nip
;


: snl-reverse  ( snl -- = Reverse or mirror the list )
  nil over
  snl-first@                 \ walk = first
  
  BEGIN
    nil<>?
  WHILE                      \ while walk<>nil do
    dup snn-next@
    >r
    tuck snn-next!           \  walk->next = prev
    r>
  REPEAT
  drop
  
  dup  snl-first@
  over dup snl-last@       
  swap snl>first !           \ first = last
  swap snl>last  !           \ last  = first
;


( Inspection )

: snl-dump     ( snl -- = Dump the list )
  ." snl:" dup . cr
  ."  first :" dup snl>first ?  cr
  ."  last  :" dup snl>last  ?  cr
  ."  length:" dup snl>length ? cr
  
  ['] snn-dump swap snl-execute cr
;

[THEN]

\ ==============================================================================
