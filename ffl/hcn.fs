\ ==============================================================================
\
\              hcn - the hash table cell node in the ffl
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
\  $Date: 2008-02-21 20:31:18 $ $Revision: 1.10 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] hcn.version [IF]


include ffl/hnn.fs

( hcn = Hash Table Cell Node )
( The hcn module implements a node that stores cell wide data in a hash table.)


2 constant hcn.version


( Hash table node structure )

begin-structure hcn%    ( - n = Get the required space for a hcn node )
  hnn%
  +field hcn>hnn        \ the hcn node extends the hnn node
  field: hcn>cell       \ the cell data
end-structure


( Node creation, initialisation and destruction )

: hcn-init     ( x c-addr u u2 hcn -- = Initialise the node with the hash u2, the key c-addr u and cell data x )
  >r
  r@ hnn-init  
  r> hcn>cell !
;


: hcn-(free)   ( hcn -- = Free the key from the heap )
  hnn-(free)
;


: hcn-new      ( x c-addr u u2 -- hcn = Create a new node on the heap with the hash u2, the key c-addr u and cell data x )
  hcn% allocate  throw  dup >r hcn-init r>
;


: hcn-free     ( hcn -- = Free the node from the heap )
  dup hcn-(free)
  
  free throw
;


( Private words )

: hcn-cell@   ( hcn -- x = Get the cell value from the node )
  hcn>cell @
;


( Inspection )

: hcn-dump     ( hcn -- = Dump the node )
  dup hnn-dump
  ."  cell :" hcn>cell ? cr
;

[THEN]

\ ==============================================================================
