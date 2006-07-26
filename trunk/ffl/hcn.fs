\ ==============================================================================
\
\              hcn - the hash cell table node in the ffl
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
\  $Date: 2006-07-26 06:50:20 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] hcn.version [IF]


include ffl/stc.fs
include ffl/str.fs

( hcn = Hash Cell Table Node )
( The hcn module implements the node in the hash table.)


1 constant hcn.version


( Public structure )

struct: hcn%       ( - n = Get the required space for a hcn structure )
  cell: hcn>hash        \ the hash code
  cell: hcn>key         \ the pointer to the key
  cell: hcn>cell        \ the cell data
  cell: hcn>next        \ the next node
;struct 


( Public words )

: hcn-init     ( w w:hcn - = Initialise the node with cell data )
  tuck hcn>cell     !
       hcn>next  nil!
  \ ToDo
;


: hcn-new      ( w - w:hcn = Create a new node on the heap )
  hcn% allocate  throw  tuck hcn-init
  \ ToDo
;


: hcn-free     ( w:hcn - = Free the node from the heap )
  \ ToDo
  free throw
;


: hcn-dump     ( w:hcn - = Dump the node )
  ." hcn:" dup . cr
  ."  hash :" dup hcn>hash  ?  cr
  ."  key  :" dup hcn>key   str-dump cr
  ."  cell :" dup hcn>cell  ?  cr
  ."  next :"     hcn>next  ?  cr
;

[THEN]

\ ==============================================================================
