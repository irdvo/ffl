\ ==============================================================================
\
\           dnn - the generic double linked list node in the ffl
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
\  $Date: 2008-01-09 19:30:48 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] dnn.version [IF]


include ffl/stc.fs


( dnn = Generic Double Linked List Node )
( The dnn module implements a node in a dnl list.                            )


1 constant dnn.version


( Node structure )

begin-structure dnn%   ( -- n = Get the required space for a dnn structure )
  field: dnn>next
  field: dnn>prev
end-structure


( Node creation, initialisation and destruction )

: dnn-init     ( dnn -- = Initialise the node )
  dup  dnn>next  nil!
       dnn>prev  nil!
;


: dnn-new      ( -- dnn = Create a new node on the heap )
  dnn% allocate  throw  dup dnn-init
;


: dnn-free     ( dnn -- = Free the node from the heap )
  free throw
;


( Members words )

: dnn-next@    ( dnn1 -- dnn2 = Get the next node dnn2 from node dnn1)
  dnn>next @
;


: dnn-next!    ( dnn1 dnn2 -- = Set for node dnn2 the next node to dnn1 )
  dnn>next !
;


: dnn-prev@    ( dnn1 -- dnn2 = Get from node dnn1 the previous node )
  dnn>prev @
;


: dnn-prev!    ( dnn1 dnn2 -- = Set for node dnn2 the previous node to dnn1 )
  dnn>prev !
;


( Inspection )

: dnn-dump     ( dnn -- = Dump the node )
  ." dnn:" dup . cr
  ."  next :" dup dnn>next  ?  cr
  ."  prev :"     dnn>prev  ?  cr
;

[THEN]

\ ==============================================================================
