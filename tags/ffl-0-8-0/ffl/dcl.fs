\ ==============================================================================
\
\             dcl - the double linked cell list in the ffl
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
\  $Date: 2008-02-21 20:31:18 $ $Revision: 1.5 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] dcl.version [IF]


include ffl/stc.fs
include ffl/dnl.fs
include ffl/dcn.fs


( dcl = Double Linked Cell List )
( The dcl module implements a double linked list that can store cell wide data.)
  

2 constant dcl.version


( List structure )

begin-structure dcl%       ( -- n = Get the required space for a dcl variable )
  dnl% 
  +field  dcl>dnl       \ Extend the base list with ..
  field:  dcl>compare   \ .. a compare token
end-structure


( List creation, initialisation and destruction )

: dcl-init     ( dcl -- = Initialise the dcl list )
  dup dnl-init
  ['] <=> swap dcl>compare !  \ for sorting
;


: dcl-(free)   ( dnl -- = Free the nodes from the heap )
  ['] dcn-free swap dnl-(free)
;


: dcl-create   ( "<spaces>name" -- ; -- dcl = Create a named dcl list in the dictionary )
  create   here   dcl% allot   dcl-init
;


: dcl-new      ( -- dcl = Create a new dcl list on the heap )
  dcl% allocate  throw  dup dcl-init
;


: dcl-free     ( dcl -- = Free the list from the heap, including the nodes )
  dup dcl-(free)
  
  free  throw
;


( Member words )

: dcl-empty?     ( dcl -- flag = Check for empty list )
  dnl-empty?
;


: dcl-length@    ( dcl -- u = Get the number of nodes in the list )
  dnl-length@
;


: dcl-compare!   ( xt dcl -- = Set the compare execution token for sorting the list )
  dcl>compare !
;


: dcl-compare@   ( dcl -- xt = Get the compare execution token for sorting the list )
  dcl>compare @
;


( List words )

: dcl-clear      ( dnl -- = Delete all nodes from the list )
  dcl-(free)
;


: dcl-append     ( x dcl -- = Append the cell data x in the list )
  >r dcn-new r> dnl-append
;


: dcl-prepend    ( x dcl -- = Prepend the cell data x in the list )
  >r dcn-new r> dnl-prepend
;


( Index words )

: dcl-index?   ( n dcl -- flag = Check if the index n is valid for the list )
  dnl-index?
;


: dcl-set      ( x n dcl -- = Set the cell data x in the nth node in the list )
  dnl-get                    \ Find the node
  dup nil= exp-invalid-state AND throw
  dcn-cell!                  \ Store the cell
;


: dcl-get      ( n dcl -- x = Get the cell data x from the nth node from the list )
  dnl-get                    \ Find the node
  dup nil= exp-invalid-state AND throw
  dcn-cell@                  \ Return the data
;


: dcl-insert   ( x n dcl -- = Insert cell data x at the nth node in the list )
  2>r dcn-new 2r> dnl-insert
;


: dcl-delete   ( n dcl -- x = Delete the nth node from the list, return the cell data x )
  dnl-delete                 \ Delete the node from the list
  dup  nil= exp-invalid-state AND throw
  dup  dcn-cell@             \ Fetch the data
  swap dcn-free              \ Free the node
;


( Special words )

: dcl-count    ( x dcl -- u = Count the number of occurrences of the cell data x in the list )
  0 >r                       \ count = 0
  dnl-first@                 \ walk = first
  BEGIN
    dup nil<> 
  WHILE                      \ while walk <> nil do
    2dup
    dcn-cell@ = IF           \  if walk->cell = w then
      r> 1+ >r               \   count++
    THEN
    dnn-next@                \  walk = walk->next
  REPEAT
  2drop
  r>
;


: dcl-execute      ( i*x xt dcl -- j*x = Execute xt for every cell data in list )
  dnl-first@                 \ walk = first
  BEGIN
    dup nil<>                \ while walk<>nil do
  WHILE
    dup >r dcn-cell@
    swap 
    dup >r execute           \  execute xt with cell
    r> r>
    dnn-next@                \  walk = walk->next
  REPEAT
  2drop
;


: dcl-reverse  ( dcl -- = Reverse or mirror the list )
  dnl-reverse
;


( Private words )

: dcl-search   ( x dcl -- n dcn = Search for the first element with the cell data x, return offset and node )
  0 -rot                     \ index = 0
  dnl-first@                 \ walk = first
  BEGIN
    dup nil<> IF             \ while walk <> nil and walk->cell <> w do
      2dup dcn-cell@ <>
    ELSE
      false
    THEN
  WHILE
    2>r 1+ 2r>
    dnn-next@                \  walk = walk->next
  REPEAT
  nip
;


( Search words )

: dcl-find     ( x dcl -- n = Find the first index for the cell data x in the list, -1 for not found )
  dcl-search
  
  nil= IF                    \ if walk = nil then
    drop -1                  \   index = -1
  THEN
;


: dcl-has?     ( x dcl -- flag = Check if the cell data x is present in the list )
  dcl-search nip nil<>
;


: dcl-remove   ( x dcl -- flag = Remove the first occurrence of the cell data x from the list, return success )
  tuck dcl-search nip        \ Search the cell data
  nil<>? IF                  \ If dcn <> nil then
    dup rot dnl-remove       \  Remove from list
    dcn-free                 \  Free the node
    true
  ELSE                       \ else
    drop                     \  not found
    false
  THEN
;


( Sort words )

: dcl-insert-sorted   ( x dcl -- = Insert the cell data x sorted in the list )
  dup dcl-compare@ >r        \ save the sort execution token
  
  tuck
  dnl-first@                 \ walk = first
  
  BEGIN
    dup nil<> IF             \ while walk <> nil and walk->cell <= w do
      2dup dcn-cell@  
      r@ execute 0>=
    ELSE
      false
    THEN
  WHILE
    dnn-next@                \  walk = walk->next
  REPEAT
  rdrop
  
  >r dcn-new swap r>         \ Create a new node S: new dcl walk
  
  dup nil= IF                \ If all nodes are smaller Then
    drop dnl-append          \   Append
  ELSE                       \ Else
    swap dnl-insert-before   \   Insert before walk
  THEN
;


( Inspection )

: dcl-dump     ( dcl -- = Dump the list )
  dup dnl-dump
  ." dcl:" cr
  ."  compare:" dup dcl>compare ? cr
  ."  nodes  :" ['] . swap dcl-execute cr
;

[THEN]

\ ==============================================================================
