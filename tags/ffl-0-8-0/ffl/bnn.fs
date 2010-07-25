\ ==============================================================================
\
\           bnn - the generic binary tree node in the ffl
\
\             Copyright (C) 2008  Dick van Oudheusden
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
\  $Date: 2008-04-10 16:12:01 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] bnn.version [IF]


include ffl/stc.fs


( bnn = Generic binary tree node )
( The bnn module implements a node in a generic unbalanced binary tree. )


1 constant bnn.version


( Node structure )

begin-structure bnn%       ( -- n = Get the required space for a bnn node )
  field:  bnn>parent    \ the parent node
  field:  bnn>left      \ the left node
  field:  bnn>right     \ the right node
  field:  bnn>key       \ the key
end-structure


( Node creation, initialisation and destruction )

: bnn-init         ( x bnn1 bnn2 -- = Initialise the node bnn2 with parent bnn1 and key x )
  tuck bnn>parent    !
  dup  bnn>left   nil!
  dup  bnn>right  nil!
       bnn>key       !
;


: bnn-new          ( x bnn1 -- bnn2 = Create a new node on the heap with parent bnn1 and key x )
  bnn% allocate  throw   >r r@ bnn-init r>
;


: bnn-free         ( bnn -- = Free the node from the heap )
  free throw 
;


( Private words )

: bnn-left@        ( bnn1 -- bnn2 = Get the left node from the node bnn1)
  bnn>left @
;


: bnn-right@       ( bnn1 -- bnn2 = Get the right node from the node bnn1)
  bnn>right @
;


: bnn-parent@      ( bnn1 -- bnn2 = Get the parent from the node bnn1 )
  bnn>parent @
;


: bnn-parent!      ( bnn1 bnn2 -- = Set for the node bnn2 the parent to bnn1, if bnn2 is not nil )
  nil<>? IF
    bnn>parent !
  ELSE
    drop
  THEN
;


: bnn-compare-key  ( x xt bnn -- x xt bnn n = Compare with xt the key in the node bnn with the key x resulting in n )
  >r
  2dup r@ bnn>key @ swap execute
  r> swap
;


( Member words )   

: bnn-key@         ( bnn -- x = Get the key from the node )
  bnn>key @
;


( Inspection )

: bnn-dump         ( bnn -- = Dump the node )
  ." bnn:" dup . cr
  ."   parent :" dup bnn>parent  ? cr
  ."   left   :" dup bnn>left    ? cr
  ."   right  :" dup bnn>right   ? cr
  ."   key    :"     bnn>key     ? cr
;

[THEN]

\ ==============================================================================
