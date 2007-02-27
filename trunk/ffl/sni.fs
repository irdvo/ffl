\ ==============================================================================
\
\         sni - the single linked node list iterator in the ffl
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
\  $Date: 2007-02-27 19:54:10 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] sni.version [IF]


include ffl/stc.fs
include ffl/snl.fs
include ffl/snn.fs


( sni = Single Linked Node List Iterator )
( The sni module implements an iterator on the single linked node list [snl]. )


1 constant sni.version


( Iterator structure )

struct: sni%       ( - n = Get the required space for a sni data structure )
  cell: sni>snl
  cell: sni>walk
;struct 


( Iterator creation, initialisation and destruction )

: sni-init     ( w:snl w:sni - = Initialise the iterator with a snl list )
  tuck sni>snl     !
       sni>walk  nil!
;


: sni-create   ( C: w:snl "name" - R: - w = Create a named iterator in the dictionary )
  create 
    here  sni% allot  sni-init
;


: sni-new      ( w:snl - w:sni = Create an iterator on the heap )
  sni% allocate  throw  tuck sni-init
;


: sni-free     ( w:sni - = Free iterator from heap )
  free throw
;


( Member words )

: sni-get      ( w:sni - w:snn | nil = Get the current node )
  sni>walk @
;


( Iterator words )

: sni-first    ( w:sni - w:snn | nil  = Move the iterator to the first node )
  dup sni>snl @             
  snl-first@
  dup rot sni>walk !         \ walk = snl.first
;


: sni-next     ( w:sni - w:snn | nil = Move the iterator to the next node )
  sni>walk 
  dup @
  dup nil<> IF               \ if walk <> nil then
    snn-next@                \   walk = walk.next
    dup rot !
  ELSE                       \ else
    exp-invalid-state throw  \   exception
  THEN
;


: sni-first?   ( w:sni - f = Check if the iterator is on the first node )
  dup sni>snl @
  snl-first@
  dup nil= IF
    2drop
    false
  ELSE
    swap sni-get =
  THEN
;


: sni-last?    ( w:sni - f = Check if the iterator is on the last node )
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

: sni-dump     ( w:sni - = Dump the iterator )
  ." sni:" dup . cr
  ."  snl :" dup sni>snl  ?  cr
  ."  walk:"     sni>walk  ?  cr
;

[THEN]

\ ==============================================================================
