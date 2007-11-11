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
\  $Date: 2007-11-11 07:41:31 $ $Revision: 1.8 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] hcn.version [IF]


include ffl/hnn.fs

( hcn = Hash Table Cell Node )
( The hcn module implements a node that stores cell wide data in a hash table.)


1 constant hcn.version


( Hash table node structure )

struct: hcn%       ( - n = Get the required space for a hash table node structure )
  hnn%
  field: hcn>hnn        \ the hcn node extends the hnn node
  cell: hcn>cell        \ the cell data
;struct 


( Node creation, initialisation and destruction )

: hcn-init     ( w c-addr u u:hash w:hcn - = Initialise the node with cell data, key and hash )
  >r
  r@ hnn-init  
  r> hcn>cell !
;


: hcn-new      ( w c-addr u u:hash - w:hcn = Create a hash table node on the heap )
  hcn% allocate  throw  dup >r hcn-init r>
;


: hcn-free     ( w:hcn - = Free the node from the heap )
  hnn-free
;


( Private words )

: hcn-cell@   ( w:hnn - w:cell = Get the cell value )
  hcn>cell @
;


( Inspection )

: hcn-dump     ( w:hcn - = Dump the node )
  dup hnn-dump
  ."  cell :" hcn>cell ? cr
;

[THEN]

\ ==============================================================================
