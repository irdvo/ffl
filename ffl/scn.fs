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
\  $Date: 2007-03-04 08:38:31 $ $Revision: 1.5 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] scn.version [IF]


include ffl/snn.fs


( scn = Single Linked Cell Node )
( The scn module implements the node in the scl-list.)


2 constant scn.version


( Node structure )

struct: scn%       ( - n = Get the required space for a scn structure )
  snn% field: scn>snn        \ Extend the snn structure with ..
       cell:  scn>cell       \ .. a cell
;struct 


( Node creation, initialisation and destruction )

: scn-init     ( w w:scn - = Initialise the node with cell data )
  dup  snn-init
       scn>cell     !
;


: scn-new      ( w - w:scn = Create a new node on the heap )
  scn% allocate  throw  tuck scn-init
;


: scn-free     ( w:scn - = Free the node from the heap )
  free throw
;


( Members words )

: scn-cell@    ( w:scn - w:data = Get the cell data from the node )
  scn>cell @
;


: scn-cell!    ( w:data w:scn - = Set the cell data in the node )
  scn>cell !
;



( Inspection )

: scn-dump     ( w:scn - = Dump the node )
  dup snn-dump
  ."  cell :" scn>cell  ?  cr
;

[THEN]

\ ==============================================================================
