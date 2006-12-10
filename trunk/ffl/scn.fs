\ ==============================================================================
\
\             scn - the single linked cell node in the ffl
\
\               Copyright (C) 2005  Dick van Oudheusden
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
\  $Date: 2006-12-10 07:47:29 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] scn.version [IF]


include ffl/stc.fs


( scn = Single Linked Cell Node )
( The scn module implements the node in the scl-list.)


1 constant scn.version


( Node structure )

struct: scn%       ( - n = Get the required space for a scn structure )
  cell: scn>cell
  cell: scn>next
;struct 


( Node creation, initialisation and destruction )

: scn-init     ( w w:scn - = Initialise the node with cell data )
  tuck scn>cell     !
       scn>next  nil!
;


: scn-new      ( w - w:scn = Create a new node on the heap )
  scn% allocate  throw  tuck scn-init
;


: scn-free     ( w:scn - = Free the node from the heap )
  free throw
;


( Inspection )

: scn-dump     ( w:scn - = Dump the node )
  ." scn:" dup . cr
  ."  cell :" dup scn>cell  ?  cr
  ."  next :"     scn>next  ?  cr
;

[THEN]

\ ==============================================================================
