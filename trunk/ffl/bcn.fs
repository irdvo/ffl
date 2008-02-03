\ ==============================================================================
\
\           bcn - the Binary tree cell node module in the ffl
\
\             Copyright (C) 2006-2007  Dick van Oudheusden
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
\  $Date: 2008-02-03 07:09:33 $ $Revision: 1.6 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] bcn.version [IF]


include ffl/stc.fs


( bcn = Binary tree cell node module )
( The bcn module implements a node in an unbalanced binary tree. )


1 constant bcn.version


( Node structure )

begin-structure bcn%       ( -- n = Get the required space for a bcn node )
  field:  bcn>parent         \ the parent node
  field:  bcn>left           \ the left node
  field:  bcn>right          \ the right node
  field:  bcn>key            \ the key
  field:  bcn>cell           \ the cell data
end-structure



( Node creation, initialisation and destruction )

: bcn-init         ( x1 x2 bcn1 bcn2 -- = Initialise the node bcn2 with the parent bcn1, key x2 and data x1 )
  tuck bcn>parent    !
  dup  bcn>left   nil!
  dup  bcn>right  nil!
  tuck bcn>key       !
       bcn>cell      !
;


: bcn-new          ( x1 x2 bcn1 -- bcn2 = Create a new node on the heap with the parent bcn1, key x2 and data x1 )
  bcn% allocate  throw   >r r@ bcn-init r>
;


: bcn-free         ( bcn -- = Free the node from the heap )
  free throw 
;


( Private words )


: bcn-left@        ( bcn1 -- bcn2 = Get the left node from the node bcn1)
  bcn>left @
;


: bcn-right@       ( bcn1 -- bcn2 = Get the right node from the node bcn1)
  bcn>right @
;


: bcn-parent@      ( bcn1 -- bcn2 = Get the parent from the node bcn1 )
  bcn>parent @
;


: bcn-parent!      ( bcn1 bcn2 -- = Set for the node bcn2 the parent to bcn1, if bcn2 is not nil )
  nil<>? IF
    bcn>parent !
  ELSE
    drop
  THEN
;


: bcn-compare-key  ( x xt bcn -- x xt bcn n = Compare with xt the key in the node bcn with the key x resulting in n )
  >r
  2dup r@ bcn>key @ swap execute
  r> swap
;


( Inspection )

: bcn-dump         ( bcn -- = Dump the node )
  ." bcn:" dup . cr
  ."   parent :" dup bcn>parent  ? cr
  ."   left   :" dup bcn>left    ? cr
  ."   right  :" dup bcn>right   ? cr
  ."   key    :" dup bcn>key     ? cr
  ."   cell   :"     bcn>cell    ? cr
;

[THEN]

\ ==============================================================================
