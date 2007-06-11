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
\  $Date: 2007-03-04 08:38:31 $ $Revision: 1.13 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] scl.version [IF]


include ffl/stc.fs
include ffl/snl.fs
include ffl/scn.fs


( scl = Single Linked Cell List )
( The scl module implements a single linked list that can store cell wide data.)
  

3 constant scl.version


( List structure )

struct: scl%       ( - n = Get the required space for the scl data structure )
  snl% field: scl>snl        \ Extend the base list with ..
       cell:  scl>compare    \ .. a compare token
;struct


( List creation, initialisation and destruction )

: scl-init     ( w:scl - = Initialise the scl-list )
  dup snl-init
  ['] <=> swap scl>compare ! 
;


: scl-create   ( C: "name" - R: - w:scl = Create a named scl-list in the dictionary )
  create   here   scl% allot   scl-init
;


: scl-new      ( - w:scl = Create a new scl-list on the heap )
  scl% allocate  throw  dup scl-init
;


: scl-delete-all ( w:scl - = Delete all nodes in the list )
  BEGIN
    dup snl-remove-first dup nil<>    \ while remove first node <> nil do
  WHILE
    scn-free                          \   free node
  REPEAT
  2drop
;


: scl-free     ( w:scl - = Free the list from the heap )
  dup scl-delete-all
  
  free  throw
;



( Member words )

: scl-empty?   ( w:scl - f = Check for empty list )
  snl-empty?
;


: scl-length@  ( w:scl - u = Get the number of nodes in the list )
  snl-length@
;


: scl-compare!     ( xt w:scl - = Set the compare execution token for the sorting the list )
  scl>compare !
;


: scl-compare@     ( w:scl - xt = Get the compare execution token for the sorting the list )
  scl>compare @
;



( List words )

: scl-append   ( w w:scl - = Append the cell in the list )
  >r scn-new r> snl-append
;


: scl-prepend  ( w w:scl - = Prepend the cell in the list )
  >r scn-new r> snl-prepend
;


( Index words )

: scl-index?   ( n:index w:dcl - f = Check if index is valid for the list )
  snl-index?
;


: scl-set      ( w n:index w:scl - = Set the cell data in the indexth node in the list )
  snl-get                    \ Find the node
  dup nil= exp-invalid-state AND throw
  scn-cell!                  \ Store the cell
;


: scl-get      ( n:index w:scl - w = Get the cell data from the indexth node from the list )
  snl-get                    \ Find the node
  dup nil= exp-invalid-state AND throw
  scn-cell@                  \ Return the data
;


: scl-insert   ( w n:index w:scl - = Insert cell data at the indexth node in the list )
  2>r scn-new 2r> snl-insert
;


: scl-delete   ( n:index w:scl - w = Delete the indexth node from the list )
  snl-delete                 \ Delete the node from the list
  dup  nil= exp-invalid-state AND throw
  dup  scn-cell@             \ Fetch the data
  swap scn-free              \ Free the node
;  


( Special words )

: scl-count    ( w w:scl - u = Count the occurences of cell data in the list )
  0 >r                       \ count = 0
  snl-first@                 \ walk = first
  BEGIN
    dup nil<> 
  WHILE                      \ while walk <> nil do
    2dup
    scn-cell@ = IF           \  if walk->cell = w then
      r> 1+ >r               \   count++
    THEN
    snn-next@                \  walk = walk->next
  REPEAT
  2drop
  r>
;


: scl-execute      ( ... xt w:scl - ... = Execute xt for every cell data in list )
  snl-first@                 \ walk = first
  BEGIN
    dup nil<>                \ while walk<>nil do
  WHILE
    dup >r scn-cell@
    swap 
    dup >r execute           \  execute xt with cell
    r> r>
    snn-next@                \  walk = walk->next
  REPEAT
  2drop
;


: scl-reverse  ( w:scl - = Reverse or mirror the list )
  snl-reverse
;


( Private words )

: scl-search   ( w:data w:scl - n:index w:prev w:scn = Search for the first element with the data )
  swap >r                    \ save data
  0   swap                   \ index = 0
  nil swap                   \ prev  = nil
  snl-first@                 \ walk  = first
  BEGIN                      \ S: index prev walk
    dup nil<> IF             \ while walk <> nil and walk->cell <> w do
      dup scn-cell@ r@ <>
    ELSE
      false
    THEN
  WHILE
    nip
    swap 1+ swap             \  index = index + 1
    dup snn-next@            \  prev = walk, walk = walk->next
  REPEAT
  rdrop
;


( Search words )

: scl-find     ( w:data w:scl - n:index = Find the first index for cell data in the list, -1 for not found )
  scl-search nip
  nil= IF                    \ If walk = nil Then
    drop -1                  \   Return -1
  THEN
;


: scl-has?     ( w:data w:scl - f = Check if the cell data is present in the list )
  scl-search nip nip nil<>
;


: scl-remove   ( w:data w:scl - f = Remove the first occurence of the cell data from the list )
  tuck scl-search              \ Search the cell data
  nil<> IF                     \ If data found then
    nip
    dup nil= IF                \   If prev = nil Then
      drop snl-remove-first    \     Remove first node
    ELSE                       \   Else
      swap snl-remove-after    \     Remove after prev
    THEN
    scn-free                   \   Free the node
    true
  ELSE
    2drop
    drop
    false
  THEN
;


( Sort words )

: scl-insert-sorted   ( w:data w:scl - = Insert the cell data sorted in the list )
  dup scl-compare@ >r        \ Save the sort execution token
  
  tuck
  nil -rot                   \ prev = nil
  snl-first@                 \ walk = first
  
  BEGIN                      \ S: scl prev data walk
    dup nil<> IF             \ while walk <> nil and walk->cell <= w do
      2dup scn-cell@  
      r@ execute 0>=
    ELSE
      false
    THEN
  WHILE
    rot drop tuck            \  prev = walk
    snn-next@                \  walk = walk->next
  REPEAT
  rdrop
  
  drop scn-new 
  
  swap dup nil= IF           \ If prev = nil Then
    drop swap snl-prepend    \   Prepend the node
  ELSE                       \ Else
    rot snl-insert-after     \   Insert after the prev
  THEN
;


( Inspection )

: scl-dump     ( w:scl - = Dump the list )
  dup snl-dump
  ." scl:" cr
  ."  compare:" dup scl>compare ? cr
  ."  nodes  :" ['] . swap scl-execute cr
;

[THEN]

\ ==============================================================================
