\ ==============================================================================
\
\         sni - the generic single linked list iterator in the ffl
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
\  $Date: 2008-06-25 16:48:34 $ $Revision: 1.5 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] sni.version [IF]


include ffl/stc.fs
include ffl/snl.fs
include ffl/snn.fs


( sni = Generic Single Linked List Iterator )
( The sni module implements an iterator on the generic single linked list [snl]. )


1 constant sni.version


( Iterator structure )

begin-structure sni%       ( -- n = Get the required space for a sni variable )
  field: sni>snl
  field: sni>walk
end-structure 


( Iterator creation, initialisation and destruction )

: sni-init     ( snl sni -- = Initialise the iterator with a snl list )
  tuck sni>snl     !
       sni>walk  nil!
;


: sni-create   ( snl "<spaces>name" -- ; -- sni = Create a named iterator in the dictionary on the snl list )
  create 
    here  sni% allot  sni-init
;


: sni-new      ( snl -- sni = Create an iterator on the snl list on the heap )
  sni% allocate  throw  tuck sni-init
;


: sni-free     ( sni -- = Free the iterator from the heap )
  free throw
;


( Member words )

: sni-get      ( sni -- snn | nil = Get the current node )
  sni>walk @
;


( Iterator words )

: sni-first    ( sni -- snn | nil  = Move the iterator to the first node, return this node )
  dup sni>snl @             
  snl-first@
  dup rot sni>walk !         \ walk = snl.first
;


: sni-next     ( sni -- snn | nil = Move the iterator to the next node, return this node )
  sni>walk 
  dup @
  nil<>? IF                  \ if walk <> nil then
    snn-next@                \   walk = walk.next
    dup rot !
  ELSE                       \ else
    exp-invalid-state throw  \   exception
  THEN
;


: sni-first?   ( sni -- flag = Check if the iterator is on the first node )
  dup sni>snl @
  snl-first@
  dup nil= IF
    2drop
    false
  ELSE
    swap sni-get =
  THEN
;


: sni-last?    ( sni -- flag = Check if the iterator is on the last node )
  dup sni>snl @
  snl-last@
  dup nil= IF
    2drop
    false
  ELSE
    swap sni-get =
  THEN
;


( Inspection )

: sni-dump     ( sni -- = Dump the iterator )
  ." sni:" dup . cr
  ."  snl :" dup sni>snl  ?  cr
  ."  walk:"     sni>walk  ?  cr
;

[THEN]

\ ==============================================================================
