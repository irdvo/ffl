\ ==============================================================================
\
\              hnn - the hash table base node in the ffl
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
\  $Date: 2007-11-11 07:41:31 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] hnn.version [IF]


include ffl/stc.fs

( hnn = Hash Table base node )
( The hnn module implements a base node in a hash table.)


1 constant hnn.version


( Hash table node structure )

struct: hnn%       ( - n = Get the required space for a hash table node structure )
  cell: hnn>hash        \ the hash code
  cell: hnn>key         \ the pointer to the key
  cell: hnn>klen        \ the key length
  cell: hnn>next        \ the next node
  cell: hnn>prev        \ the previous node    
;struct 


( Node creation, initialisation and destruction )

: hnn-init     ( c-addr u u:hash w:hnn - = Initialise the base node with a key and hash )
  >r
  over 0= exp-invalid-parameters AND throw
  
      r@ hnn>hash !
  dup r@ hnn>klen !
  dup chars allocate throw
  dup r@ hnn>key  !
  swap cmove
      r@ hnn>next nil!
      r> hnn>prev nil!
;


: hnn-new      ( c-addr u u:hash - w:hnn = Create a hash table node on the heap )
  hnn% allocate  throw  dup >r hnn-init r>
;


: hnn-free     ( w:hnn - = Free the node from the heap )
  dup hnn>key @ free throw
  free throw
;


( Private words )

: hnn-next@    ( w:hnn - w:next = Get the next node )
  hnn>next @
;


: hnn-prev@    ( w:hnn - w:prev = Get the previous node )
  hnn>prev @
;


: hnn-hash@   ( w:hnn - u:hash = Get the hash value )
  hnn>hash @
;


( Inspection )

: hnn-dump     ( w:hnn - = Dump the node )
  ." hnn:" dup . cr
  ."  hash :" dup  hnn>hash  ?   cr
  ."  key  :" dup  hnn>key   @ 
              over hnn>klen  @   type cr
  ."  next :" dup  hnn>next  ?   cr
  ."  prev :"      hnn>prev  ?   cr 
;

[THEN]

\ ==============================================================================
