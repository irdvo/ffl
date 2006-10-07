\ ==============================================================================
\
\           bcn - the Binary tree cell node module in the ffl
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
\  $Date: 2006-10-07 06:09:27 $ $Revision: 1.2 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] bcn.version [IF]


include ffl/stc.fs


( bcn = Binary tree cell node module )
( The bcn module implements the a node in an unbalanced binary tree )


1 constant bcn.version


( Tree node structure )

struct: bcn%       ( - n = Get the required space for the bcn structure )
  cell:  bcn>parent          \ the parent node
  cell:  bcn>left            \ the left node
  cell:  bcn>right           \ the right node
  cell:  bcn>key             \ the key
  cell:  bcn>cell            \ the cell data
;struct



( Structure creation, initialisation and destruction )

: bcn-init         ( w:data w:key w:parant w:bcn - = Initialise the bcn structure with a key, data and parent )
  tuck bcn>parent    !
  dup  bcn>left   nil!
  dup  bcn>right  nil!
  tuck bcn>key       !
       bcn>cell      !
;


: bcn-new          ( w:data w:key w:parent - w:bcn = Create a new tree node on the heap )
  bcn% allocate  throw   >r r@ bcn-init r>
;


: bcn-free         ( w:bcn - = Free the tree node from the heap )
  free throw 
;


( Private words )


: bcn-left@        ( w:bcn - w:left = Get the left tree node )
  bcn>left @
;


: bcn-right@       ( w:bcn - w:right = Get the right tree node )
  bcn>right @
;


: bcn-parent@      ( w:bcn - w:parent = Get the parent tree node )
  bcn>parent @
;


: bcn-parent!      ( w:parent w:bcn - = Set the parent, if bcn is not nil )
  dup nil<> IF
    bcn>parent !
  ELSE
    2drop
  THEN
;


: bcn-compare-key  ( w:key xt w:node - w:key xt w:node w:result = Compare the key in the node with the key )
  >r
  2dup r@ bcn>key @ swap execute
  r> swap
;


( Inspection )

: bcn-dump         ( w:bcn - = Dump the tree node structure )
  ." bcn:" dup . cr
  ."   parent :" dup bcn>parent  ? cr
  ."   left   :" dup bcn>left    ? cr
  ."   right  :" dup bcn>right   ? cr
  ."   key    :" dup bcn>key     ? cr
  ."   cell   :"     bcn>cell    ? cr
;

[THEN]

\ ==============================================================================
