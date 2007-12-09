\ ==============================================================================
\
\            dcn - the double linked cell list node in the ffl
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
\  $Date: 2007-12-09 07:23:15 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] dcn.version [IF]


include ffl/dnn.fs


( dcn = Double Linked Cell List Node )
( The dcn module implements a node in a dcl list.)


1 constant dcn.version


( Node structure )

begin-structure dcn%   ( -- n = Get the required space for a dcn node )
  dnn% 
  +field dcn>dnn        \ extend the dnn structure with ..
  field: dcn>cell       \ .. a cell
end-structure


( Node creation, initialisation and destruction )

: dcn-init     ( x dcn -- = Initialise the node with data x )
  dup  dnn-init
       dcn>cell !
;


: dcn-new      ( x -- dcn = Create a new node on the heap with data x )
  dcn% allocate  throw  tuck dcn-init
;


: dcn-free     ( dcn -- = Free the node from the heap )
  free throw
;


( Members words )

: dcn-cell@    ( dcn -- x = Get the cell data x from the node )
  dcn>cell @
;


: dcn-cell!    ( x dcn -- = Set the cell data x in the node )
  dcn>cell !
;


( Inspection )

: dcn-dump     ( dcn -- = Dump the node )
  dup dnn-dump
  ."  cell :" dcn>cell  ?  cr
;

[THEN]

\ ==============================================================================
