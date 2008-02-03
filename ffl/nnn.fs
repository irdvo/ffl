\ ==============================================================================
\
\              nnn - the generic n-Tree node in the ffl
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
\  $Date: 2008-02-03 07:09:34 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nnn.version [IF]


include ffl/dnl.fs


( nnn = Generic n-Tree Node )
( The nnn module implements a generic node in a n-tree [nnt].                )


1 constant nnn.version


( Node structure )

begin-structure nnn%      ( -- n = Get the required space for a nnn node )
  dnn% 
  +field  nnn>dnn         \ the siblings )
  field:  nnn>parent      \ nnn -- nnn = the parent )
  dnl% 
  +field  nnn>children    \ nnn -- dnl = the children )
end-structure


( Node creation, initialisation and destruction )

: nnn-init     ( nnn -- = Initialise the node )
  dup  nnn>dnn      dnn-init
  dup  nnn>parent   nil!
       nnn>children dnl-init
;


: nnn-new      ( -- nnn = Create a new node on the heap )
  nnn% allocate  throw  dup nnn-init
;


: nnn-free     ( nnn -- = Free the node from the heap )
  free throw
;


( Members words )

: nnn-parent@    ( nnn1 -- nnn2 = Get from node nnn1 the parent node )
  nnn>parent @
;


: nnn-parent!   ( nnn1 nnn2 -- = Set for node nnn2 the parent to nnn1 )
  nnn>parent !
;


( Private words )

: nnn-next      ( nnn1 -- nnn2 | nil = Find the next node for node nnn1 )
  dup nnn>children dnl-first@    \ If the node has children Then
  nil<>? IF
    nip                          \   Next = first child
  ELSE                           \ Else
    dup nnn>dnn dnn-next@        \   If node has siblings Then
    nil<>? IF
      nip                        \     Next = sibling
    ELSE                         \   Else
      nnn-parent@                \     Find sibling of a parent
      BEGIN
        dup nil<> IF 
          dup dnn-next@ nil=     \     Check if parent has siblings
        ELSE
          false
        THEN
      WHILE
        nnn-parent@
      REPEAT
      
      dup nil<> IF               \     If parent found Then
        dnn-next@                \       Move to sibling
      THEN
    THEN
  THEN
;


: nnn-prev      ( nnn1 -- nnn2 | nil = Find the previous node for node nnn1 )
  dup nnn>children dnl-last@     \ If the node has children Then
  nil<>? IF
    nip                          \   Next = last child
  ELSE                           \ Else
    dup nnn>dnn dnn-prev@        \   If node has siblings Then
    nil<>? IF
      nip                        \     Next = sibling
    ELSE                         \   Else
      nnn-parent@                \     Find sibling of a parent
      BEGIN
        dup nil<> IF 
          dup dnn-prev@ nil=     \     Check if parent has siblings
        ELSE
          false
        THEN
      WHILE
        nnn-parent@
      REPEAT
      
      dup nil<> IF               \     If parent found Then
        dnn-prev@                \       Move to sibling
      THEN
    THEN
  THEN
;


( Inspection )

: nnn-dump     ( nnn -- = Dump the node )
  dup nnn>dnn      dnn-dump
  dup nnn>children dnl-dump
  ." nnn:"  cr
  ."  parent :" nnn>parent ?  cr
;

[THEN]

\ ==============================================================================
