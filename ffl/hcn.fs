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
\  $Date: 2006-07-30 07:06:02 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] hcn.version [IF]


include ffl/stc.fs

( hcn = Hash Cell Table Node )
( The hcn module implements the node in the hash table.)


1 constant hcn.version


( Public structure )

struct: hcn%       ( - n = Get the required space for a hcn structure )
  cell: hcn>hash        \ the hash code
  cell: hcn>key         \ the pointer to the key
  cell: hcn>klen        \ the key length
  cell: hcn>cell        \ the cell data
  cell: hcn>next        \ the next node
  cell: hcn>prev        \ the previous node    
;struct 


( Public words )

: hcn-init     ( w c-addr u u:hash w:hcn - = Initialise the node with cell data, key and hash )
  >r
  
  over 0= exp-invalid-parameters AND throw
  
      r@ hcn>hash !
  dup r@ hcn>klen !
  dup chars allocate throw
  dup r@ hcn>key  !
  swap cmove
      r@ hcn>cell !
      r@ hcn>next nil!
      r> hcn>prev nil!
;


: hcn-new      ( w c-addr u u:hash - w:hcn = Create a new node on the heap )
  hcn% allocate  throw  dup >r hcn-init r>
;


: hcn-free     ( w:hcn - = Free the node from the heap )
  dup hcn>key @ free throw
  free throw
;


( Private words )

: hcn-next@    ( w:hcn - w:next = Get the next node )
  hcn>next @
;


: hcn-prev@    ( w:hcn - w:prev = Get the previous node )
  hcn>prev @
;


: hcn-hash@   ( w:hcn - u:hash = Get the hash value )
  hcn>hash @
;


: hcn-cell@   ( w:hcn - w:cell = Get the cell value )
  hcn>cell @
;


( Inspection )

: hcn-dump     ( w:hcn - = Dump the node )
  ." hcn:" dup . cr
  ."  hash :" dup  hcn>hash  ?   cr
  ."  key  :" dup  hcn>key   @ 
              over hcn>klen  @   type cr
  ."  cell :" dup  hcn>cell  ?   cr
  ."  next :" dup  hcn>next  ?   cr
  ."  prev :"      hcn>prev  ?   cr 
;

[THEN]

\ ==============================================================================
