\ ==============================================================================
\
\              dnn - the double linked list node in the ffl
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
\  $Date: 2006-12-28 20:08:19 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] dnn.version [IF]


include ffl/stc.fs


( dnn = Double Linked List Node )
( The dnn module implements the node in the scl-list.)


1 constant dnn.version


( Node structure )

struct: dnn%       ( - n = Get the required space for a dnn structure )
  cell: dnn>next
  cell: dnn>prev
;struct 


( Node creation, initialisation and destruction )

: dnn-init     ( w:dnn - = Initialise the node )
  dup  dnn>next  nil!
       dnn>prev  nil!
;


: dnn-new      ( - w:dnn = Create a new node on the heap )
  dnn% allocate  throw  dup dnn-init
;


: dnn-free     ( w:dnn - = Free the node from the heap )
  free throw
;


( Members words )

: dnn-next@    ( w:dnn - w:next = Get the next node )
  dnn>next @
;


: dnn-next!    ( w:next w:dnn - = Set the next node )
  dnn>next !
;


: dnn-prev@    ( w:dnn - w:prev = Get the previous node )
  dnn>prev @
;


: dnn-prev!    ( w:prev w:dnn - = Set the previous node )
  dnn>prev !
;


( Inspection )

: dnn-dump     ( w:dnn - = Dump the node )
  ." dnn:" dup . cr
  ."  next :" dup dnn>next  ?  cr
  ."  prev :"     dnn>prev  ?  cr
;

[THEN]

\ ==============================================================================
