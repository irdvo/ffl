\ ==============================================================================
\
\               nct - the n-Tree cell module in the ffl
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
\  $Date: 2008-02-20 19:30:05 $ $Revision: 1.5 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nct.version [IF]


include ffl/nnt.fs
include ffl/ncn.fs


( nct = n-Tree cell module )
( The nct module implements a n-Tree that can store cell size data. It extends)
( the base n-Tree module, with cell based words. For adding and removing cells)
( to and from the tree, use the iterator [nci].                               )


2 constant nct.version


( Tree structure )

nnt% constant nct%  ( -- n = Get the required space for a nct variable )


( Tree creation, initialisation and destruction )

: nct-init     ( nct -- = Initialise the tree )
  nnt-init
;


: nct-(free)   ( nct -- = Free the nodes in the tree )
  ['] ncn-free swap nnt-(free)
;


: nct-create   ( "<spaces>name" -- ; -- nct = Create a named n-tree in the dictionary )
  create   here   nct% allot   nct-init
;


: nct-new      ( -- nct = Create a new n-tree on the heap )
  nct% allocate  throw  dup nct-init
;


: nct-free     ( nct -- = Free the tree from the heap )
  dup nct-(free)             \ Free all nodes from the tree
  
  free throw                 \ Free the tree
;


( Member words )

: nct-length@  ( nct -- u = Get the number of nodes in the tree )
  nnt-length@
;


: nct-empty?   ( nct -- flag = Check for empty tree )
  nnt-empty?
;


( Private words )

: nct+emit-node  ( x -- = Emit the cell data x from tree node )
  0 .r [char] ; emit
;


: nct+equal?     ( x1 x2 -- x1 flag = Check if data x2 is equal to the node data x1 )
  over =
;


: nct+count      ( +n1 x1 x2 -- +n2 x1 = Count in n if data x2 is equal to the node data x1 )
  over = IF
    swap 1+ swap
  THEN
;


( Tree words )

: nct-clear        ( nct -- = Delete all nodes from the tree )
  nct-(free)
;


: nct-execute      ( i*x xt nct -- j*x = Execute xt for every node in tree )
  nnt-root@                 \ walk = first
  BEGIN
    dup nil<>               \ while walk<>nil do
  WHILE
    2>r 
    2r@ ncn-cell@
    swap execute            \  execute xt with node
    2r>
    nnn-next                \  walk = walk next
  REPEAT
  2drop
;


: nct-execute?     ( i*x xt nct -- j*x flag = Execute xt for every node in the tree until xt returns true )
  nnt-root@                 \ walk = first
  false
  BEGIN
    over nil<> over 0= AND  \ while walk<>nil and flag = false do
  WHILE
    drop    
    2>r 
    2r@ ncn-cell@
    swap execute            \  execute xt with node
    2r>
    nnn-next                \  walk = walk next
    rot
  REPEAT
  nip nip
;
  

: nct-count      ( x nct -- u = Count the number of the occurrences of the cell data x in the tree )
  0 -rot
  ['] nct+count  swap nct-execute drop
;


: nct-has?       ( x nct -- flag = Check if the cell data x is present in the tree )
  ['] nct+equal? swap nct-execute? nip
;


( Inspection )

: nct-dump     ( nct -- = Dump the tree )
  dup nnt-dump
  ." nct:" dup . cr
  ."  nodes :" ['] nct+emit-node swap nct-execute cr
;

[THEN]

\ ==============================================================================
 
