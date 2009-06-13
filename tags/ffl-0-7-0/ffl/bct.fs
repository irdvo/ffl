\ ==============================================================================
\
\            bct - the binary cell tree module in the ffl
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
\  $Date: 2008-04-24 16:50:23 $ $Revision: 1.14 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] bct.version [IF]


include ffl/bcn.fs
include ffl/bnt.fs


( bct = binary cell tree module )
( The bct module implements an unbalanced binary tree with the key and data )
( cell based. The implementation is non-recursive.                          )


2 constant bct.version


( Binary tree structure )

bnt% constant bct%  ( -- n = Get the required space for a bct variable )


( Tree creation, initialisation and destruction )

: bct-init         ( bct -- = Initialise the tree )
  bnt-init
;


: bct-(free)       ( bct -- = Free the nodes from the heap )
  ['] bcn-free swap bnt-(free)
;


: bct-create       ( "<spaces>name" -- ; -- bct = Create a named binary tree in the dictionary )
  create  here bct% allot  bct-init
;


: bct-new          ( -- bct = Create a new binary tree on the heap )
  bct% allocate throw   dup bct-init
;


: bct-free         ( bct -- = Free the tree node from the heap )
  dup bct-(free)                  \ Free the nodes 
  
  free throw                      \ Free the tree
;


( Member words )

: bct-length@      ( bct -- u = Get the number of elements in the tree )
  bnt-length@
;


: bct-empty?       ( bct -- flag = Check for an empty tree )
  bnt-empty?
;


: bct-compare@     ( bct -- xt = Get the compare execution token for comparing keys )
  bnt-compare@
;


: bct-compare!     ( xt bct -- = Set the compare execution token for comparing keys )
  bnt-compare!
;


( Tree words )

: bct-clear        ( bct -- = Delete all nodes in the tree )
  bct-(free)
;


: bct-insert       ( x1 x2 bct -- = Insert data x1 with key x2 in the tree )
  >r ['] bcn-new swap r>
  bnt-insert IF
    drop                     \ Node inserted
  ELSE
    bcn-cell!                \ Node not unique, update cell value 
  THEN
;


: bct-delete       ( x1 bct -- false | x2 true = Delete key x1 from the tree, return the cell data x2 )
  bnt-delete IF
    dup  bcn-cell@
    swap bct-free
    true
  ELSE
    false
  THEN
;


: bct-get          ( x1 bct -- false | x2 true = Get the data x2 related to key x1 from the tree )
  bnt-search-node
  nil<>? IF
    bcn-cell@
    true
  ELSE
    false
  THEN
;


: bct-has?         ( x1 bct -- flag = Check if the key x1 is present in the tree )
  bnt-has?
;


: bct-execute      ( i*x xt bct -- j*x = Execute xt for every key and data in the tree )
  bnt>root @
  bnt-smallest-node               \ find the smallest node
  BEGIN                           \ while not nil do
    dup nil<>
  WHILE
    2>r                           \   clear the stack
    2r@
    dup  bcn-cell@
    swap bnn-key@
    rot  execute                  \   execute with key and data
    2r>
    
    bnt-next-node                 \   find the next node
  REPEAT
  2drop  
;


: bct-execute?     ( i*x xt bct -- j*x flag = Execute xt for every key and data in the tree until xt returns true )
  bnt>root @
  bnt-smallest-node               \ find the smallest node
  false
  BEGIN                           \ while not nil do
    over nil<> over 0= AND
  WHILE
    drop
    2>r                           \   clear the stack
    2r@
    dup  bcn-cell@
    swap bnn-key@
    rot  execute                  \   execute with key and data
    2r>
    bnt-next-node                 \   find the next node
    rot
  REPEAT
  nip nip  
;

( Private words )

: bct-emit-node ( x1 x2 -- = Emit the key x2 and cell data x1 )
  0 .r [char] = emit 0 .r [char] ; emit
;


( Inspection )

: bct-dump         ( bct -- = Dump the tree node structure )
  dup bnt-dump
  ." bct:" dup . cr
  ."   nodes  :" ['] bct-emit-node swap bct-execute cr
;

[THEN]

\ ==============================================================================
