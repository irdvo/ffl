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
\  $Date: 2006-12-31 06:50:17 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] dcn.version [IF]


include ffl/dnn.fs


( dcn = Double Linked Cell List Node )
( The dcn module implements the node in the dcl-list.)


1 constant dcn.version


( Node structure )

struct: dcn%       ( - n = Get the required space for a dcn structure )
  dnn% field: dcn>dnn        \ extend the dnn structure with ..
       cell:  dcn>cell       \ .. a cell
;struct 


( Node creation, initialisation and destruction )

: dcn-init     ( w:data w:dcn - = Initialise the node )
  dup  dnn-init
       dcn>cell !
;


: dcn-new      ( w:data - w:dcn = Create a new node on the heap )
  dcn% allocate  throw  tuck dcn-init
;


: dcn-free     ( w:dcn - = Free the node from the heap )
  free throw
;


( Members words )

: dcn-cell@    ( w:dcn - w:data = Get the cell data from the node )
  dcn>cell @
;


: dcn-cell!    ( w:data w:dcn - = Set the cell data in the node )
  dcn>cell !
;


( Inspection )

: dcn-dump     ( w:dcn - = Dump the node )
  dup dnn-dump
  ."  cell :" dcn>cell  ?  cr
;

[THEN]

\ ==============================================================================
