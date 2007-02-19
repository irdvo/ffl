\ ==============================================================================
\
\               snn - the single linked node in the ffl
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
\  $Date: 2007-02-19 18:52:45 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] snn.version [IF]


include ffl/stc.fs


( snn = Single Linked Node )
( The snn module implements the node in the snl-list.)


1 constant snn.version


( Node structure )

struct: snn%       ( - n = Get the required space for a snn structure )
  cell: snn>next
;struct 


( Node creation, initialisation and destruction )

: snn-init     ( w:snn - = Initialise the single list node )
  snn>next nil!
;


: snn-new      (  - w:snn = Create a new single list node on the heap )
  snn% allocate  throw  dup snn-init
;


: snn-free     ( w:snn - = Free the single list node from the heap )
  free throw
;


( Member words )

: snn-next@    ( w:snn - w:next = Get the next node )
  snn>next @
;


: snn-next!    ( w:next w:snn - = Set the next node )
  snn>next !
;


( Inspection )

: snn-dump     ( w:snn - = Dump the single list node )
  ." snn:" dup . cr
  ."  next :"     snn>next  ?  cr
;

[THEN]

\ ==============================================================================
