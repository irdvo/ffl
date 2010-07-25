\ ==============================================================================
\
\              bcn - the Binary tree cell node in the ffl
\
\             Copyright (C) 2006-2008  Dick van Oudheusden
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
\  $Date: 2008-04-15 17:13:54 $ $Revision: 1.7 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] bcn.version [IF]


include ffl/bnn.fs


( bcn = Binary cell tree node )
( The bcn module implements a node in an unbalanced binary tree that can )
( cell wide data.                                                        )

1 constant bcn.version


( Node structure )

begin-structure bcn%  ( -- n = Get the required space for a bcn node )
  bnn%
  +field  bcn>bnn            \ Extend the bnn structure with .. 
  field:  bcn>cell           \ .. cell data
end-structure



( Node creation, initialisation and destruction )

: bcn-init         ( x1 x2 bcn1 bcn2 -- = Initialise the node bcn2 with the parent bcn1, key x2 and data x1 )
  >r 
  r@ bnn-init
  r> bcn>cell !
;


: bcn-new          ( x1 x2 bcn1 -- bcn2 = Create a new node on the heap with the parent bcn1, key x2 and data x1 )
  bcn% allocate  throw   >r r@ bcn-init r>
;


: bcn-free         ( bcn -- = Free the node from the heap )
  free throw 
;


( Members words )

: bcn-cell@        ( bcn1 -- bcn2 = Get the left node from the node bcn1)
  bcn>cell @
;


: bcn-cell!        ( bcn1 -- bcn2 = Get the right node from the node bcn1)
  bcn>cell !
;


( Inspection )

: bcn-dump         ( bcn -- = Dump the node )
  dup bnn-dump
  ."   cell   :" bcn>cell ? cr
;

[THEN]

\ ==============================================================================
