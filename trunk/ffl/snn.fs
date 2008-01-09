\ ==============================================================================
\
\             snn - the generic single linked node in the ffl
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
\  $Date: 2008-01-09 19:30:48 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] snn.version [IF]


include ffl/stc.fs


( snn = Generic Single Linked List Node )
( The snn module implements a node in a generic single linked list [snl].    )


1 constant snn.version


( Node structure )

begin-structure snn%       ( - n = Get the required space for a snn node )
  field: snn>next
end-structure


( Node creation, initialisation and destruction )

: snn-init     ( snn -- = Initialise the node )
  snn>next nil!
;


: snn-new      (  -- snn = Create a new node on the heap )
  snn% allocate  throw  dup snn-init
;


: snn-free     ( snn -- = Free the node from the heap )
  free throw
;


( Member words )

: snn-next@    ( snn1 -- snn2 = Get the next node snn2 from the node snn1 )
  snn>next @
;


: snn-next!    ( snn1 snn2 -- = Set for the node snn1 the next node to snn2 )
  snn>next !
;


( Inspection )

: snn-dump     ( snn -- = Dump the single list node )
  ." snn:" dup . cr
  ."  next :"     snn>next  ?  cr
;

[THEN]

\ ==============================================================================
