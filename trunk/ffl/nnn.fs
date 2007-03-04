\ ==============================================================================
\
\                nnn - the n-Tree base node in the ffl
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
\  $Date: 2007-03-04 08:38:31 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nnn.version [IF]


include ffl/dnl.fs


( nnn = n-Tree base node )
( The nnn module implements the base node in a n-tree.)


1 constant nnn.version


( Node structure )

struct: nnn%       ( - n = Get the required space for a nnn structure )
  dnn% field: nnn>dnn         ( nnn - dnn = the siblings )
       cell:  nnn>parent      ( nnn - dnn = the parent )
  dnl% field: nnn>children    ( nnn - dnl = the children )
;struct 


( Node creation, initialisation and destruction )

: nnn-init     ( w:nnn - = Initialise the node )
  dup  nnn>dnn      dnn-init
  dup  nnn>parent   nil!
       nnn>children dnl-init
;


: nnn-new      ( - w:nnn = Create a new node on the heap )
  nnn% allocate  throw  dup nnn-init
;


: nnn-free     ( w:nnn - = Free the node from the heap )
  free throw
;


( Members words )

: nnn-parent@    ( w:nnn - w:parent = Get the parent node )
  nnn>parent @
;


: nnn-parent!   ( w:parent w:nnn - = Set the parent node )
  nnn>parent !
;


( Private words )

: nnn-next      ( w:nnn - w:nnn | nil = Move to the next node in the tree)
  dup nnn>children dnl-first@    \ If the node has children Then
  dup nil<> IF
    nip                          \   Next = first child
  ELSE                           \ Else
    drop
    dup nnn>dnn dnn-next@        \   If node has siblings Then
    dup nil<> IF
      nip                        \     Next = sibling
    ELSE
      drop                       \   Else
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


: nnn-prev      ( w:nnn - w:nnn | nil = Move to the previous node in the tree)
  dup nnn>children dnl-last@     \ If the node has children Then
  dup nil<> IF
    nip                          \   Next = last child
  ELSE                           \ Else
    drop
    dup nnn>dnn dnn-prev@        \   If node has siblings Then
    dup nil<> IF
      nip                        \     Next = sibling
    ELSE
      drop                       \   Else
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

: nnn-dump     ( w:nnn - = Dump the node )
  dup nnn>dnn      dnn-dump
  dup nnn>children dnl-dump
  ." nnn:"  cr
  ."  parent :" nnn>parent ?  cr
;

[THEN]

\ ==============================================================================
