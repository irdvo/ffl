\ ==============================================================================
\
\               nni - the base n-Tree iterator in the ffl
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
\  $Date: 2007-03-05 06:08:11 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nni.version [IF]


include ffl/nnt.fs


( nni = n-Tree base Iterator )
( The nni module implements an iterator on the base n-Tree [nnt]. )


1 constant nni.version


( Iterator structure )

struct: nni%       ( - n = Get the required space for a nni data structure )
  cell: nni>nnt
  cell: nni>walk
;struct 


( Iterator creation, initialisation and destruction )

: nni-init     ( w:nnt w:nni - = Initialise the iterator with a n-tree )
  tuck nni>nnt     !
       nni>walk  nil!
;


: nni-create   ( C: w:nnt "name" - R: - w = Create a named iterator in the dictionary )
  create 
    here  nni% allot  nni-init
;


: nni-new      ( w:nnt - w:nni = Create an iterator on the heap )
  nni% allocate  throw  tuck nni-init
;


: nni-free     ( w:nni - = Free iterator from heap )
  free throw
;


( Member words )

: nni-get      ( w:nni - w:nnn | nil = Get the current node )
  nni>walk @
;


( Tree iterator words )

: nni-root?    ( w:nni - f = Check if the current node is the root node )
  dup 
  nni>nnt @ nnt-root@
  swap
  nni-get
  =
;


: nni-root     ( w:nni - w:nnn | nil = Move the iterator to the root of the tree )
  dup
  nni>nnt @ nnt-root@
  dup rot
  nni>walk !
;


: nni-parent   ( w:nni - w:nnn | nil = Move the iterator to the parent of the current node )
  nni>walk
  dup @
  dup nil<> IF
    nnn-parent@
    dup rot !
  ELSE
    exp-invalid-state throw
  THEN
;


: nni-children   ( w:nni - n = Return the number of children of the current node )
  nni-get
  dup nil<> IF
    nnn>children
    dnl-length@
  ELSE
    exp-invalid-state throw
  THEN
;


: nni-children?  ( w:nni - f = Check if the current node has children )
  nni-get
  dup nil<> IF
    nnn>children
    dnl-first@ 
    nil<>
  ELSE
    exp-invalid-state throw
  THEN
;


: nni-child    ( w:nni - w:nnn = Move the iterator to the first child of the current node )
  nni>walk
  dup @
  dup nil<> IF
    nnn>children
    dnl-first@
    dup rot !
  ELSE
    exp-invalid-state throw
  THEN
;


: nni-prepend-child  ( w:nnn w:nni - Prepend a child to the children of the current node, iterator is moved to the new child )
;


: nni-append-child  ( w:nnn w:nni - = Append a child to the children of the current node, iterator is moved to the new child )
;


( Sibling iterator words )

: nni-first    ( w:nni - w:nnn = Move the iterator to the first sibling )
  nni>walk
  dup @
  dup nil<> IF               \ If walk <> nil Then
    nnn-parent@             
    dup nil<> IF
      nnn>children
      dnl-first@             \  walk = walk.parent.children.first
      dup rot !
    ELSE
      exp-invalid-state throw
    THEN
  ELSE
    exp-invalid-state throw
  THEN
;


: nni-next     ( w:nni - w:nnn | nil = Move the iterator to the next sibling )
  nni>walk 
  dup @
  dup nil<> IF               \ if walk <> nil then
    nnn>dnn
    dnn-next@                \   walk = walk.next
    dup rot !
  ELSE                       \ else
    exp-invalid-state throw  \   exception
  THEN
;


: nni-prev     ( w:nni - w:nnn | nil = Move the iterator to the previous sibling )
  nni>walk 
  dup @
  dup nil<> IF               \ if walk <> nil then
    nnn>dnn
    dnn-prev@                \   walk = walk.prev
    dup rot !
  ELSE                       \ else
    exp-invalid-state throw  \   exception
  THEN
;


: nni-last     ( w:nni - w:nnn = Move the iterator to the last sibling )
  nni>walk
  dup @
  dup nil<> IF               \ If walk <> nil Then
    nnn-parent@             
    dup nil<> IF
      nnn>children
      dnl-last@              \  walk = walk.parent.children.last
      dup rot !
    ELSE
      exp-invalid-state throw
    THEN
  ELSE
    exp-invalid-state throw
  THEN
;


: nni-first?   ( w:nni - f = Check if the iterator is on the first sibling )
  nni-get dup
  dup nil<> IF               \ If walk <> nil Then
    nnn-parent@             
    dup nil<> IF
      nnn>children
      dnl-first@             \  walk =?= walk.parent.children.last
      =
    ELSE
      exp-invalid-state throw
    THEN
  ELSE
    exp-invalid-state throw
  THEN
;


: nni-last?    ( w:nni - f = Check if the iterator is on the last sibling )
  nni-get dup
  dup nil<> IF               \ If walk <> nil Then
    nnn-parent@             
    dup nil<> IF
      nnn>children
      dnl-last@              \  walk =?= walk.parent.children.last
      =
    ELSE
      exp-invalid-state throw
    THEN
  ELSE
    exp-invalid-state throw
  THEN
;


: nni-insert-before  ( w:nnn w:nni - = Insert the sibling before the current sibling )
;


: nni-insert-after  ( w:nnn w:nni - = Insert the sibling after the current sibling )
;


( Inspection )

: nni-dump     ( w:nni - = Dump the iterator )
  ." nni:" dup . cr
  ."  tree:" dup nni>nnt  ?  cr
  ."  walk:"     nni>walk  ?  cr
;

[THEN]

\ ==============================================================================
