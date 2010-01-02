\ ==============================================================================
\
\             nnt - the generic n-Tree module in the ffl
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


[UNDEFINED] nnt.version [IF]


include ffl/nnn.fs


( nnt = Generic n-Tree )
( The nnt module implements a n-Tree that can store variable size nodes. It   )
( is the base module for more specialized trees, for example the cel n-tree   )
( [nct]. Due to the structure of a n-tree the words for changing the tree,    )
( adding and removing children, are part of the iterator [nni].               )


2 constant nnt.version


( Tree structure )

begin-structure nnt%       ( -- n = Get the required space for a nnt variable )
  field: nnt>root
  field: nnt>length
end-structure


( Tree creation, initialisation and destruction )

: nnt-init     ( nnt -- = Initialise the n-tree )
  dup nnt>root    nil!
     nnt>length   0!
;


: nnt-(free)   ( xt nnt -- = Free all nodes in the tree with xt )
  swap >r
  dup nnt>root @                \ node = root
  BEGIN
    dup nil<>
  WHILE                         \ while node <> nil Do
    dup nnn>children dnl-first@ dup nil<> IF
      nip                       \   if node.children <> nil Then node = first child
    ELSE
      drop
      dup nnn-parent@ dup nil= IF  \ else if node.parent = nil Then
        swap r@ execute            \   remove (root) node
      ELSE                         \ else
        2dup                       \   remove child node
        nnn>children dnl-remove
        swap r@ execute
      THEN
    THEN
  REPEAT
  drop rdrop
  dup nnt>root   nil!
      nnt>length   0!
;


: nnt-create   ( "<spaces>name" -- ; -- nnt = Create a named n-tree in the dictionary )
  create   here   nnt% allot   nnt-init
;


: nnt-new      ( -- nnt = Create a new n-tree on the heap )
  nnt% allocate  throw  dup nnt-init
;


: nnt-free     ( nnt -- = Free the tree from the heap )
  ['] nnn-free over nnt-(free)   \ Free all tree nodes
  
  free  throw                    \ Free the tree
;


( Member words )

: nnt-length@  ( nnt -- u = Get the number of nodes in the tree )
  nnt>length @
;


: nnt-empty?   ( nnt -- flag = Check for an empty tree )
  nnt-length@ 0=  
;


: nnt-root@   ( nnt -- nnn | nil  = Get the root of the tree )
  nnt>root @
;


( Tree words )

: nnt-execute      ( i*x xt nnt -- j*x = Execute xt for every node in tree )
  nnt-root@                 \ walk = first
  BEGIN
    dup nil<>                \ while walk<>nil do
  WHILE
    2>r 
    2r@ swap execute         \  execute xt with node
    2r>
    nnn-next                 \  walk = walk next
  REPEAT
  2drop
;


: nnt-execute?     ( i*x xt nnt -- j*x flag = Execute xt for every node in the tree until xt returns true )
  nnt-root@                 \ walk = first
  false
  BEGIN
    over nil<> over 0= AND   \ while walk<>nil and flag = false do
  WHILE
    drop    
    2>r 
    2r@ swap execute         \  execute xt with node
    2r>
    nnn-next                 \  walk = walk next
    rot
  REPEAT
  nip nip
;
  

( Private words )

: nnt-emit-node  ( nnn -- = Emit the tree node )
  0 .r [char] ; emit
;


( Inspection )

: nnt-dump     ( nnt -- = Dump the tree )
  ." nnt:" dup . cr
  ."  root  :" dup nnt>root ?  cr
  ."  length:" dup nnt>length ? cr
  ."  nodes :" ['] nnt-emit-node swap nnt-execute cr
;

[THEN]

\ ==============================================================================
 
