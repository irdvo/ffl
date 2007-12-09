\ ==============================================================================
\
\                ncn - the n-Tree cell node in the ffl
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
\  $Date: 2007-12-09 07:23:16 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] ncn.version [IF]


include ffl/nnn.fs


( ncn = n-Tree cell node )
( The ncn module implements a node in a n-tree [nct] that stores a cell value.)


1 constant ncn.version


( Node structure )

begin-structure ncn%       ( -- n = Get the required space for a ncn node )
  nnn% 
  +field ncn>nnn        \ Extend the nnn structure with ..
  field: ncn>cell       \ .. a cell
end-structure


( Node creation, initialisation and destruction )

: ncn-init     ( x ncn -- = Initialise the node with data x)
  dup  nnn-init
       ncn>cell !
;


: ncn-new      ( x -- ncn = Create a new node on the heap with data x)
  ncn% allocate  throw  tuck ncn-init
;


: ncn-free     ( ncn -- = Free the node from the heap )
  free throw
;


( Members words )

: ncn-cell@    ( ncn -- x = Get the cell data from the node )
  ncn>cell @
;


: ncn-cell!    ( x ncn -- = Set the cell data x in the node )
  ncn>cell !
;


( Inspection )

: ncn-dump     ( ncn -- = Dump the node )
  dup nnn-dump
  ." cell :" ncn>cell ? cr
;

[THEN]

\ ==============================================================================
