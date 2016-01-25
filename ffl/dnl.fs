\ ==============================================================================
\
\            dnl - the generic double linked list in the ffl
\
\               Copyright (C) 2006  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU Lesser General Public
\ License as published by the Free Software Foundation; either
\ version 3 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ Lesser General Public License for more details.
\
\ You should have received a copy of the GNU Lesser General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2008-02-21 20:31:18 $ $Revision: 1.10 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] dnl.version [IF]


include ffl/stc.fs
include ffl/dnn.fs


( dnl = Generic Double Linked List )
( The dnl module implements a double linked list that can handle variable size )
( nodes. It is the base module for more specialized modules, for example the   )
( double linked cell list <a href='dcl.html'>dcl</a> module. )


3 constant dnl.version


( List structure )

begin-structure dnl%       ( -- n = Get the required space for a dnl variable )
  field: dnl>first
  field: dnl>last
  field: dnl>length
end-structure


( Private word )

defer dnl.pop

( List creation, initialisation and destruction )

: dnl-init     ( dnl -- = Initialise the list )
  dup dnl>first   nil!
  dup dnl>last    nil!
      dnl>length    0!
;


: dnl-(free)   ( xt dnl -- = Free the nodes in the list from the heap using xt )
  swap
  BEGIN
    over dnl.pop nil<>?      \ Pop last node
  WHILE
    over execute             \ Free the node
  REPEAT
  2drop
;


: dnl-create   ( "<spaces>name" -- ; -- dnl = Create a named dnl list in the dictionary )
  create   here   dnl% allot   dnl-init
;


: dnl-new      ( -- dnl = Create a new dnl list on the heap )
  dnl% allocate  throw  dup dnl-init
;


: dnl-free     ( dnl -- = Free the list from the heap )
  ['] dnn-free over dnl-(free)
  
  free  throw
;


( Member words )

: dnl-length@  ( dnl -- u = Get the number of nodes in the list )
  dnl>length @
;


: dnl-empty?   ( dnl -- flag = Check for empty list )
  dnl-length@ 0=  
;


: dnl-first@   ( dnl -- dnn | nil  = Get the first node in the list )
  dnl>first @
;


: dnl-last@    ( dnl -- dnn | nil = Get the last node in the list )
  dnl>last @
;


( Private words )

: dnl+offset   ( n1 n2 -- n3 = Determine offset n3 from index n1 and length n2, incl. validation )
  tuck index2offset                    \ convert to offset
  dup rot 0 within                     \ check outside 0..length-1
  exp-index-out-of-range AND throw     \ raise exception
;


: dnl-node  ( n dnl -- dnn | nil = Get the nth node in the list )
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

: dnl-append   ( dnn dnl -- = Append the node dnn in the list )
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


: dnl-prepend  ( dnn dnl -- = Prepend the node dnn in the list )
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


: dnl-insert-before  ( dnn1 dnn2 dnl -- = Insert the new node dnn1 before the reference node dnn2 in the list )
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


: dnl-insert-after  ( dnn1 dnn2 dnl -- = Insert the new node dnn1 after the reference node dnn2 in the list )
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


: dnl-remove   ( dnn dnl -- = Remove the node dnn from the list )
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

: dnl-index?   ( n dnl -- flag = Check if the index n is valid in the list )
  dnl-length@
  tuck index2offset
  swap 0 swap within
;


: dnl-get      ( n dnl -- dnn = Get the nth node from the list )
  tuck dnl-length@ dnl+offset     \ S: dnl offset
  swap dnl-node                   \ S: dnn | nil
;


: dnl-insert   ( dnn n dnl -- = Insert the node dnn before the nth node in the list )
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


: dnl-delete   ( n dnl -- dnn = Delete the nth node from the list, return the deleted node )
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

: dnl-push     ( dnn dnl -- = Push the node dnn at the end of the list )
  dnl-append
;


: dnl-pop      ( dnl -- dnn | nil = Pop the node dnn from the end of the list )
  dup dnl-last@              \ dnn = dnl.last 
  dup nil<> IF               \ If dnn != nil Then
    dup rot dnl-remove
  ELSE
    nip
  THEN
;
' dnl-pop is dnl.pop


: dnl-tos      ( dnl -- dnn | nil = Get the node dnn from the end of the list )
  dnl-last@
;


( FIFO words )

: dnl-enqueue  ( dnn dnl -- = Enqueue the node dnn at the start of the list )
  dnl-prepend
;


: dnl-dequeue  ( dnl -- dnn | nil = Dequeue the node dnn from the end of the list )
  dnl-pop
;


( Special words )

: dnl-execute      ( i*x xt dnl -- j*x = Execute xt for every node in list )
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


: dnl-execute?     ( i*x xt dnl -- j*x flag = Execute xt for every node in list or until xt returns true, flag is true if xt returned true )
  dnl-first@ false           \ walk = first
  BEGIN
    over nil<> over 0= AND   \ while walk<>nil do
  WHILE
    drop
    2>r 
    2r@ swap execute         \  execute xt with node
    2r>
    dnn-next@                \  walk = walk->next
    rot
  REPEAT
  nip nip
;


: dnl-reverse  ( dnl -- = Reverse or mirror the list )
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


( Private sort words )

: dnl-merge-qmove  ( size p^ -- psize qsize p^ q^ = Move the q^ based on p^ and size )
  2dup 
  BEGIN                           \ S:qsize p^ psize q^
    2dup nil<> AND
  WHILE                           \ Move q^ until nil or size
    dnn-next@
    >r 1- r>
  REPEAT
  >r >r over r> - -rot r>         \ Rearrange the stack
;


: dnl-merge-node   ( dnn dnl -- = Append the dnn node to the dnl list during sorting )
  dup dnl-last@ nil<> IF          \ If last <> nil Then
    2dup dnl-last@ dnn>next !     \   last->next = node
  ELSE                            \ Else
    2dup dnl>first !              \   first = node
  THEN
  2dup dnl-last@ swap dnn>prev !  \ node->prev = last
  dnl>last !                      \ last = node
;


: dnl-merge-sort   ( psize qsize p^ q^ result dnl -- psize qsize p^ q^ = Based on the compare result move p^ or q^ to the list )
  >r
  0> IF                \ If p^ bigger Then
    dup r> dnl-merge-node
    dnn-next@          \  Append q^ and move to next
    2>r 1- 2r>
  ELSE                 \ Else
    over r> dnl-merge-node
    >r
    dnn-next@          \  Append p^ and move to next
    2>r 1- 2r>
    r>
  THEN
;


: dnl-merge-nodes  ( psize p^ dnl -- 0 p^ | psize nil == Move all nodes from p^ in the list )
  >r
  BEGIN                           \ S:psize p^
    2dup nil<> AND
  WHILE                           \ While nodes in psize
    dup r@ dnl-merge-node
    dnn-next@                     \ Append them in the list
    >r 1- r>
  REPEAT
  rdrop
;


( Sort word )

: dnl-sort         ( xt dnl -- = Sort the list dnl using mergesort, xt compares the nodes )
  >r >r
  1
  BEGIN                      \ S:size
    0
    r'@ dnl-first@           \ p^
    r'@ dnl>first nil!
    r'@ dnl>last  nil!
    BEGIN                    \ S:size steps p^
      dup nil<>
    WHILE
      >r 1+ over r>          \ steps++
      dnl-merge-qmove        \ Move the q^ max. size nodes
      BEGIN                  \ S:size steps psize qsize p^ q^
        2over 0>    AND
        over  nil<> AND
      WHILE
        2dup r@ execute      \ Compare the nodes
        r'@ dnl-merge-sort   \ Merge the node from p^ or q^ based on compare result
      REPEAT
      rot swap 2swap         \ S:size steps qsize q^ psize p^
      r'@ dnl-merge-nodes    \ Merge p^ if still nodes present
      2drop                  \ psize p^
      r'@ dnl-merge-nodes    \ Merge q^ if still nodes present
      nip                    \ Save q^ as p^, drop qsize
    REPEAT
    drop                     \ p^
    r'@ dnl-last@ dnn>next nil!  \ last->next = nil
    >r 2* r>                 \ size*=2
  2 < UNTIL                  \ until steps < 2
  drop                       \ size
  rdrop rdrop
;


( Inspection )

: dnl-dump     ( dnl -- = Dump the list )
  ." dnl:" dup . cr
  ."  first :" dup dnl>first ?  cr
  ."  last  :" dup dnl>last  ?  cr
  ."  length:" dup dnl>length ? cr
  
  ['] dnn-dump swap dnl-execute cr
;

[THEN]

\ ==============================================================================
