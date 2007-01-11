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
\  $Date: 2007-01-01 18:14:16 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] dcl.version [IF]


include ffl/stc.fs
include ffl/dnl.fs
include ffl/dcn.fs


( dcl = Double Linked Cell List )
( The dcl module implements a double linked list that can store cell wide data.)
  

1 constant dcl.version


( List structure )

struct: dcl%       ( - n = Get the required space for the dcl data structure )
  dnl% field: dcl>dnl        \ Extend the base list with ..
       cell:  dcl>compare    \ .. a compare token
;struct


( List creation, initialisation and destruction )

: dcl-init     ( w:dcl - = Initialise the dcl-list )
  dup dnl-init
  ['] <=> swap dcl>compare !  \ for sorting
;


: dcl-create   ( C: "name" - R: - w:dcl = Create a named dcl-list in the dictionary )
  create   here   dcl% allot   dcl-init
;


: dcl-new      ( - w:dcl = Create a new dcl-list on the heap )
  dcl% allocate  throw  dup dcl-init
;


: dcl-delete-all ( w:dcl - = Delete and free all nodes in the list )
  BEGIN
    dup dnl-pop dup nil<>    \ Pop last node
  WHILE
    dcn-free                 \ Free if exist
  REPEAT
  2drop
;


: dcl-free     ( w:dcl - = Free the list from the heap, including the nodes )
  dup dcl-delete-all
  
  free  throw
;


( Member words )

: dcl-empty?     ( w:dcl - f = Check for empty list )
  dnl-empty?
;


: dcl-length@    ( w:dcl - u = Get the number of nodes in the list )
  dnl-length@
;


: dcl-compare!   ( xt w:dcl - = Set the compare execution token for sorting the list )
  dcl>compare !
;


: dcl-compare@   ( w:dcl - xt = Get the compare execution token for sorting the list )
  dcl>compare @
;


( List words )

: dcl-append     ( w:data w:dcl - = Append the cell data in the list )
  >r dcn-new r> dnl-append
;


: dcl-prepend    ( w:data w:dcl - = Prepend the cell data in the list )
  >r dcn-new r> dnl-prepend
;


( Index words )

: dcl-index?   ( n:index w:dcl - f = Check if index is valid for the list )
  dnl-index?
;


: dcl-set      ( w:data n:index w:dcl - = Set the cell data in the indexth node in the list )
  dnl-get                    \ Find the node
  dup nil= exp-invalid-state AND throw
  dcn-cell!                  \ Store the cell
;


: dcl-get      ( n:index w:dcl - w:data = Get the cell data from the indexth node from the list )
  dnl-get                    \ Find the node
  dup nil= exp-invalid-state AND throw
  dcn-cell@                  \ Return the data
;


: dcl-insert   ( w n:index w:dcl - = Insert cell data at the indexth node in the list )
  2>r dcn-new 2r> dnl-insert
;


: dcl-delete   ( n:index w:dcl - w = Delete the indexth node from the list )
  dnl-delete                 \ Delete the node from the list
  dup  nil= exp-invalid-state AND throw
  dup  dcn-cell@             \ Fetch the data
  swap dcn-free              \ Free the node
;


( Special words )

: dcl-count    ( w:data w:dcl - u = Count the occurences of cell data in the list )
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


: dcl-execute      ( ... xt w:dcl - ... = Execute xt for every cell data in list )
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


: dcl-reverse  ( w:dcl - = Reverse or mirror the list )
  dnl-reverse
;


( Private words )

: dcl-search   ( w:data w:dcl - n:index w:dcn = Search for the first element with the data )
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

: dcl-find     ( w:data w:dcl - n:index = Find the first index for the cell data in the list, -1 for not found )
  dcl-search
  
  nil= IF                    \ if walk = nil then
    drop -1                  \   index = -1
  THEN
;


: dcl-has?     ( w:data w:dcl - f = Check if the cell data is present in the list )
  dcl-search nip nil<>
;


: dcl-remove   ( w:data w:dcl - f = Remove the first occurence of the cell data from the list )
  tuck dcl-search nip        \ Search the cell data
  dup nil<> IF               \ If dcn <> nil then
    dup rot dnl-remove       \  Remove from list
    dcn-free                 \  Free the node
    true
  ELSE                       \ else
    2drop                    \  not found
    false
  THEN
;


( Sort words )

: dcl-insert-sorted   ( w:data w:dcl - = Insert the cell data sorted in the list )
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

: dcl-dump     ( w:dcl - = Dump the list )
  dup dnl-dump
  ." dcl:" cr
  ."  compare:" dup dcl>compare ? cr
  ."  nodes  :" ['] . swap dcl-execute cr
;

[THEN]

\ ==============================================================================
