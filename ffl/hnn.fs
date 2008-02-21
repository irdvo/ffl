\ ==============================================================================
\
\             hnn - the generic hash table node in the ffl
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
\  $Date: 2008-02-21 20:31:18 $ $Revision: 1.8 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] hnn.version [IF]


include ffl/stc.fs

( hnn = Generic Hash Table Node )
( The hnn module implements a generic node in the hash table [hnt].)


1 constant hnn.version


( Hash table node structure )

begin-structure hnn%    ( -- n = Get the required space for a hnn node )
  field: hnn>hash       \ the hash code
  field: hnn>key        \ the pointer to the key
  field: hnn>klen       \ the key length
  field: hnn>next       \ the next node
  field: hnn>prev       \ the previous node    
end-structure


( Node creation, initialisation and destruction )

: hnn-init     ( c-addr u u2 hnn -- = Initialise the node with the key c-addr u and hash u2 )
  >r
  \ over 0= exp-invalid-parameters AND throw
  
      r@ hnn>hash !
  dup r@ hnn>klen !
      
  ?dup 0= IF
    r@ hnn>key nil!
    drop
  ELSE
    dup chars allocate throw
    dup r@ hnn>key  !
    swap cmove
  THEN
  
  r@ hnn>next nil!
  r> hnn>prev nil!
;


: hnn-(free)   ( hnn -- = Free the key from the heap )
  hnn>key @ ?free throw
;


: hnn-new      ( c-addr u u2 -- hnn = Create a new node on the heap with the key c-addr u and hash u2 )
  hnn% allocate  throw  dup >r hnn-init r>
;


: hnn-free     ( hnn -- = Free the node from the heap )
  dup hnn-(free)

  free throw
;


( Member words )

: hnn-key@     ( hnn -- c-addr u = Get the key from the node )
  dup  hnn>key  @
  swap hnn>klen @
;


( Private words )

: hnn-next@    ( hnn1 -- hnn2 = Get the next node hnn2 from the node hnn1 )
  hnn>next @
;


: hnn-prev@    ( hnn1 -- hnn2 = Get the previous node hnn2 from the node hnn1 )
  hnn>prev @
;


: hnn-hash@   ( hnn -- u = Get the hash value from the node )
  hnn>hash @
;


( Inspection )

: hnn-dump     ( hnn -- = Dump the node )
  ." hnn:" dup . cr
  ."  hash :" dup  hnn>hash  ?   cr
  ."  key  :" dup  hnn-key@  ?dup IF type ELSE drop THEN cr
  ."  next :" dup  hnn>next  ?   cr
  ."  prev :"      hnn>prev  ?   cr 
;

[THEN]

\ ==============================================================================
