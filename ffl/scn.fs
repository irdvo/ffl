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
\  $Date: 2007-12-09 07:23:16 $ $Revision: 1.6 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] scn.version [IF]


include ffl/snn.fs


( scn = Single Linked Cell Node )
( The scn module implements a node in the single linked list [scl].          )


2 constant scn.version


( Node structure )

begin-structure scn%       ( -- n = Get the required space for a scn node )
  snn% 
  +field scn>snn        \ Extend the snn structure with ..
  field: scn>cell       \ .. a cell
end-structure


( Node creation, initialisation and destruction )

: scn-init     ( x scn -- = Initialise the node with cell data x )
  dup  snn-init
       scn>cell !
;


: scn-new      ( x -- scn = Create a new node on the heap with cell data x )
  scn% allocate  throw  tuck scn-init
;


: scn-free     ( scn -- = Free the node from the heap )
  free throw
;


( Members words )

: scn-cell@    ( scn -- x = Get the cell data x from the node )
  scn>cell @
;


: scn-cell!    ( x scn -- = Set the cell data x in the node )
  scn>cell !
;


( Inspection )

: scn-dump     ( scn -- = Dump the node )
  dup snn-dump
  ."  cell :" scn>cell  ?  cr
;

[THEN]

\ ==============================================================================
