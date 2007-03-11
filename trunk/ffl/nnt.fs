\ ==============================================================================
\
\               nnt - the n-Tree base module in the ffl
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
\  $Date: 2007-03-11 07:56:07 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nnt.version [IF]


include ffl/nnn.fs


( nnt = n-Tree base module )
( The nnt module implements a n-Tree that can handle variable size nodes. It  )
( is the base module for more specialised trees, for example the nct module   )
( [n-Tree cell module]. Due to the structure of a n-tree the words for        )
( changing the tree [adding,removing children] are part of the iterator [nni].)


1 constant nnt.version


( Tree structure )

struct: nnt%       ( - n = Get the required space for the nnt data structure )
  cell: nnt>root
  cell: nnt>length
;struct


( Tree creation, initialisation and destruction )

: nnt-init     ( w:nnt - = Initialise the nnt-tree )
  dup nnt>root    nil!
     nnt>length   0!
;


: nnt-create   ( C: "name" - R: - w:nnt = Create a named nnt-tree in the dictionary )
  create   here   nnt% allot   nnt-init
;


: nnt-new      ( - w:nnt = Create a new nnt-tree on the heap )
  nnt% allocate  throw  dup nnt-init
;


: nnt-free     ( w:nnt - = Free the tree from the heap )
  free  throw
;


( Member words )

: nnt-length@  ( w:nnt - u = Get the number of nodes in the tree )
  nnt>length @
;


: nnt-empty?   ( w:nnt - f = Check for empty tree )
  nnt-length@ 0=  
;


: nnt-root@   ( w:nnt - w:nnn | nil  = Get the root of the tree )
  nnt>root @
;


( Tree words )

: nnt-execute      ( ... xt w:nnt - ... = Execute xt for every node in tree )
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


: nnt-execute?     ( ... xt w:nnt - ... f = Execute xt for every node in the tree until xt returns true )
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

: nnt-emit-node  ( w:nnn - = Emit the tree node )
  0 .r [char] ; emit
;


( Inspection )

: nnt-dump     ( w:nnt - = Dump the tree )
  ." nnt:" dup . cr
  ."  root  :" dup nnt>root ?  cr
  ."  length:" dup nnt>length ? cr
  ."  nodes :" ['] nnt-emit-node swap nnt-execute cr
;

[THEN]

\ ==============================================================================
 
