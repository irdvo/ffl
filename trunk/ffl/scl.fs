\ ==============================================================================
\
\            scl - the single linked cell list in the ffl
\
\               Copyright (C) 2005  Dick van Oudheusden
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
\  $Date: 2006-04-10 17:01:21 $ $Revision: 1.7 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] scl.version [IF]


include ffl/stc.fs
include ffl/scn.fs


( scl = Single Linked Cell List )
( The scl module implements a single linked list that can store cell wide data.)
  

2 constant scl.version


( Public structure )

struct: scl%       ( - n = Get the required space for the scl data structure )
  cell: scl>first
  cell: scl>last
  cell: scl>length
  cell: scl>compare
;struct


( Private words )

: scl-add      ( w w:scl w:prev - = Add new node after prev in list )
  swap >r                     
  swap scn-new               \ Create new scn
  
  swap dup nil= IF           \ if prev = nil then
    over
    r@ scl>first @
    swap scn>next !          \  new->next = first
    over
    r@ scl>first !           \  first = new
  ELSE                       \ else
    2dup
    scn>next @
    swap scn>next !          \  new->next = prev->next
    2dup
    scn>next !               \  prev->next = new
  THEN
  
  r@ scl>last
  tuck @ = IF                \ if prev = last then
    !                        \   last = prev
  ELSE
    2drop
  THEN
  
  r> scl>length 1+!          \   length++
;


: scl-del    ( w:scl w:prev - = Delete the element after prev in the list )
  dup nil= IF                \ if prev = nil then
    drop
    dup scl>first
    dup @
    dup scn>next @           \   first = first->next
    rot !                    \   free first
    
    over scl>first @ nil= IF \   if first = nil then
      over scl>last nil!     \     last = nil
    THEN
  ELSE                       \ else
    2dup scn>next @ swap scl>last @ = IF  \ if last = prev->next then
      2dup swap scl>last !                \   last = prev
    THEN
    
    scn>next
    dup @
    dup scn>next @
    rot !                    \   prev->next = prev->next->next
  THEN                       \   free prev->next
  scn-free
  
  scl>length 1-!
;


: scl-offset ( n w:scl - n = Determine offset from index, incl. validation )
  scl>length @
  
  over 0< IF                 \ if index < 0 then
    tuck + swap              \   index = index + length
  THEN
  
  over <= over 0< OR IF      \ if index < 0 or index >= length
    exp-index-out-of-range throw 
  THEN
;


: scl-node     ( n w:scl - w:scn-prev w:scn-cur = Get the nth element in the list )
  nil -rot                   \ prev = nil
  scl>first @                \ cur  = first
  
  BEGIN
    2dup nil<> swap 0> AND   \ while n>0 and cur<> nil do
  WHILE
    rot drop dup -rot        \  prev = cur
    scn>next @               \  cur  = cur->next
    swap 1- swap             \  n--
  REPEAT
  
  nip
;


: scl-search   ( w w:scl - w:scn-prev w:scn-cur = Search for the first element )
  nil -rot                   \ prev = nil
  scl>first @                \ walk = first
  
  BEGIN
    dup nil<> IF             \ while walk <> nil and walk->cell <> w do
      2dup scn>cell @ <>
    ELSE
      false
    THEN
  WHILE
    rot drop dup -rot        \  prev = walk
    scn>next @               \  walk = walk->next
  REPEAT
  
  nip
;


( Public words )

: scl-init     ( w:scl - = Initialise the scl-list )
  dup scl>first   nil!
  dup scl>last    nil!
  dup scl>length    0!
  ['] - swap scl>compare ! 
;


: scl-create   ( "name" - = Create a named scl-list in the dictionary )
  create   here   scl% allot   scl-init
;


: scl-new      ( - w:scl = Create a new scl-list on the heap )
  scl% allocate  throw  dup scl-init
;


: scl-delete-all ( w:scl - = Delete all nodes in the list )
  BEGIN
    dup scl>first @ nil<>    \ while first<>nil 
  WHILE
    dup nil scl-del          \  delete first
  REPEAT
  drop
;


: scl-free     ( w:scl - = Free the list from the heap )
  dup scl-delete-all
  
  free  throw
;



( Member words )

: scl-empty?   ( w:scl - f = Check for empty list )
  scl>length @ 0=  
;


: scl-length@  ( w:scl - u = Get the number of nodes in the list )
  scl>length @
;


: scl-compare!     ( xt w:scl - = Set the compare execution token for the sorting the list )
  scl>compare !
;


: scl-compare@     ( w:scl - xt = Get the compare execution token for the sorting the list )
  scl>compare @
;



( List words )

: scl-append   ( w w:scl - = Append the cell in the list )
  dup scl>last @  scl-add
;


: scl-prepend  ( w w:scl - = Prepend the cell in the list )
  nil scl-add
;


: scl-set      ( w n:index w:scl - = Set the cell data in the indexth node in the list )
  tuck scl-offset swap       \ index > offset
  scl-node                   \ offset > prev + curr
  nip                        \ 
  scn>cell !                 \ cell -> curr
;


: scl-get      ( n:index w:scl - w = Get the cell data from the indexth node from the list )
  tuck scl-offset swap       \ index > offset
  scl-node                   \ offset > element
  scn>cell @                 \ element > cell
  nip
;


: scl-insert   ( w n:index w:scl - = Insert cell data at the indexth node in the list )
  tuck scl-offset over
  scl-node
  drop scl-add  
;


: scl-delete   ( n:index w:scl - w = Delete the indexth node from the list )
  tuck scl-offset over       \ index > offset
  scl-node                   \ offet > prev + curr
  
  scn>cell @                 \ save curr->cell
  
  -rot scl-del               \ delete curr (via prev)
;


: scl-remove   ( w w:scl - f = Remove the first occurence of the cell data from the list )
  tuck scl-search            \ cell > prev + curr
  nil<> IF                   \ if curr <> nil then
    scl-del                  \  delete curr
    true
  ELSE                       \ else
    2drop                    \  not found
    false
  THEN
;



( Special words )

: scl-count    ( w w:scl - u = Count the occurences of cell data in the list )
  0 >r                       \ count = 0
  scl>first @                \ walk = first
  BEGIN
    dup nil<> 
  WHILE                      \ while walk <> nil do
    2dup
    scn>cell @ = IF          \  if walk->cell = w then
      r> 1+ >r               \   count++
    THEN
    scn>next @               \  walk = walk->next
  REPEAT
  2drop
  r>
;

: scl-execute      ( ... xt w:scl - ... = Execute xt for every cell data in list )
  scl>first @                \ walk = first
  BEGIN
    dup nil<>                \ while walk<>nil do
  WHILE
    dup >r scn>cell @
    swap 
    dup >r execute           \  execute xt with cell
    r> r>
    scn>next @               \  walk = walk->next
  REPEAT
  2drop
;


: scl-find     ( w w:scl - n:index = Find the first index for cell data in the list, -1 for not found )
  0 -rot                     \ index = 0
  scl>first @                \ walk = first
  
  BEGIN
    dup nil<> IF             \ while walk <> nil and walk->cell <> w do
      2dup scn>cell @ <>
    ELSE
      false
    THEN
  WHILE
    rot 1+ -rot              \  index++
    scn>next @               \  walk = walk->next
  REPEAT
  
  nip
  nil= IF                    \ if walk = nil then
    drop -1                  \   index = -1
  THEN
;


: scl-has?     ( w w:scl - f = Check if the cell data is present in the list )
  scl-find 0>=
;


: scl-reverse  ( w:scl - = Reverse or mirror the list )
  nil over
  scl>first @                \ walk = first
  
  BEGIN
    dup nil<>
  WHILE                      \ while walk<>nil do
    dup scn>next @
    >r
    tuck scn>next !          \  walk->next = prev
    r>
  REPEAT
  2drop
  
  dup  scl>first @
  over dup scl>last @       
  swap scl>first !           \ first = last
  swap scl>last  !           \ last  = first
;



( Sort words )

: scl-insert-sorted   ( w w:scl - = Insert the cell data sorted in the list )
  dup scl>compare @ >r       \ save the sort execution token
  
  tuck
  nil -rot                   \ prev = nil
  scl>first @                \ walk = first
  
  BEGIN
    dup nil<> IF             \ while walk <> nil and walk->cell <= w do
      2dup scn>cell @  r@ execute 0>=
    ELSE
      false
    THEN
  WHILE
    rot drop dup -rot        \  prev = walk
    scn>next @               \  walk = walk->next
  REPEAT
  
  drop -rot scl-add          \ add the node
  
  rdrop
;



( Inspection )

: scl-dump     ( w:scl - = Dump the list )
  ." scl:" dup . cr
  ."  first :" dup scl>first ?  cr
  ."  last  :" dup scl>last  ?  cr
  ."  length:" dup scl>length ? cr
  
  scl>first @
  BEGIN
    dup nil<> 
  WHILE
    dup scn-dump
    
    scn>next @
  REPEAT
  
  drop
;

[THEN]

\ ==============================================================================
