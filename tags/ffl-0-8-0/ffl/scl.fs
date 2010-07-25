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
\  $Date: 2008-02-21 20:31:19 $ $Revision: 1.16 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] scl.version [IF]


include ffl/stc.fs
include ffl/snl.fs
include ffl/scn.fs


( scl = Single Linked Cell List )
( The scl module implements a single linked list that can store cell wide data.)
  

4 constant scl.version


( List structure )

begin-structure scl%       ( -- n = Get the required space for a scl variable )
  snl% 
  +field  scl>snl            \ Extend the base list with ..
  field:  scl>compare        \ .. a compare token
end-structure


( List creation, initialisation and destruction )

: scl-init     ( scl -- = Initialise the scl list )
  dup snl-init
  ['] <=> swap scl>compare ! 
;


: scl-(free)   ( scl -- = Free the nodes from the heap )
  ['] scn-free swap snl-(free)
;


: scl-create   ( "<spaces>name" -- ; -- scl = Create a named scl list in the dictionary )
  create   here   scl% allot   scl-init
;


: scl-new      ( -- scl = Create a new scl list on the heap )
  scl% allocate  throw  dup scl-init
;


: scl-free     ( scl -- = Free the list from the heap )
  dup scl-(free)
  
  free  throw
;



( Member words )

: scl-empty?   ( scl -- flag = Check for an empty list )
  snl-empty?
;


: scl-length@  ( scl -- u = Get the number of nodes in the list )
  snl-length@
;


: scl-compare!     ( xt scl -- = Set the compare execution token for sorting the list )
  scl>compare !
;


: scl-compare@     ( scl -- xt = Get the compare execution token for sorting the list )
  scl>compare @
;



( List words )

: scl-clear    ( scl -- = Delete all nodes from the list )
  scl-(free)
;


: scl-append   ( x scl -- = Append the cell data x in the list )
  >r scn-new r> snl-append
;


: scl-prepend  ( x scl -- = Prepend the cell data x in the list )
  >r scn-new r> snl-prepend
;


( Index words )

: scl-index?   ( n scl -- flag = Check if the index n is valid for the list )
  snl-index?
;


: scl-set      ( x n scl -- = Set the cell data x in the nth node in the list )
  snl-get                    \ Find the node
  dup nil= exp-invalid-state AND throw
  scn-cell!                  \ Store the cell
;


: scl-get      ( n scl -- x = Get the cell data x from the nth node in the list )
  snl-get                    \ Find the node
  dup nil= exp-invalid-state AND throw
  scn-cell@                  \ Return the data
;


: scl-insert   ( x n scl -- = Insert cell data x at the nth node in the list )
  2>r scn-new 2r> snl-insert
;


: scl-delete   ( n scl -- x = Delete the nth node from the list, return the cell data from the deleted node )
  snl-delete                 \ Delete the node from the list
  dup  nil= exp-invalid-state AND throw
  dup  scn-cell@             \ Fetch the data
  swap scn-free              \ Free the node
;  


( Special words )

: scl-count    ( x scl -- u = Count the number of occurrences of the cell data x in the list )
  0 >r                       \ count = 0
  snl-first@                 \ walk = first
  BEGIN
    nil<>? 
  WHILE                      \ while walk <> nil do
    2dup
    scn-cell@ = IF           \  if walk->cell = x then
      r> 1+ >r               \   count++
    THEN
    snn-next@                \  walk = walk->next
  REPEAT
  drop
  r>
;


: scl-execute      ( i*x xt scl -- j*x = Execute xt for every cell data in list )
  snl-first@                 \ walk = first
  BEGIN
    nil<>?                   \ while walk<>nil do
  WHILE
    dup >r scn-cell@
    swap 
    dup >r execute           \  execute xt with cell
    r> r>
    snn-next@                \  walk = walk->next
  REPEAT
  drop
;


: scl-reverse  ( scl -- = Reverse or mirror the list )
  snl-reverse
;


( Private words )

: scl-search   ( x scl -- n scn1 scn2 = Search for the first node with the data x, return the leading node scn1 and the node itself scn2 )
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

: scl-find     ( x scl -- n = Find the first index for cell data x in the list, -1 for not found )
  scl-search nip
  nil= IF                    \ If walk = nil Then
    drop -1                  \   Return -1
  THEN
;


: scl-has?     ( x scl -- flag = Check if the cell data x is present in the list )
  scl-search nip nip nil<>
;


: scl-remove   ( x scl -- flag = Remove the first occurrence of the cell data x from the list, return success )
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

: scl-insert-sorted   ( x scl -- = Insert the cell data x sorted in the list )
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

: scl-dump     ( scl -- = Dump the list )
  dup snl-dump
  ." scl:" cr
  ."  compare:" dup scl>compare ? cr
  ."  nodes  :" ['] . swap scl-execute cr
;

[THEN]

\ ==============================================================================
