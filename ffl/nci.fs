\ ==============================================================================
\
\               nci - the cell n-Tree iterator in the ffl
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
\  $Date: 2007-03-14 06:27:42 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nci.version [IF]


include ffl/nni.fs
include ffl/nct.fs


( nci = n-Tree cell Iterator )
( The nci module implements an iterator on a cell n-Tree [nct]. )


1 constant nci.version


( Iterator structure )

nni% constant nci% ( - n = Get the required space for a nci data structure )


( Iterator creation, initialisation and destruction )

: nci-init     ( w:nct w:nci - = Initialise the iterator with a n-tree )
  nni-init
;


: nci-create   ( C: w:nct "name" - R: - w = Create a named iterator in the dictionary )
  create  here  nci% allot  nci-init
;


: nci-new      ( w:nct - w:nci = Create an iterator on the heap )
  nci% allocate  throw  tuck nci-init
;


: nci-free     ( w:nci - = Free iterator from heap )
  free throw
;


( Private words )

: nni+get      ( w:ncn - w:data true | false = Get the cell data from the current node )
  dup nil<> IF
    ncn-cell@ true
  ELSE
    drop false
  THEN
;


( Member words )

: nci-get      ( w:nci - w:data true | false = Get the cell data from the current node )
  nni-get nni+get
;


( Tree iterator words )

: nci-root?    ( w:nci - f = Check if the current node is the root node )
  nni-root?
;


: nci-root     ( w:nci - w:data true | false = Move the iterator to the root of the tree )
  nni-root nni+get
;


: nci-parent   ( w:nci - w:data true | false = Move the iterator to the parent of the current node )
  nni-parent nni+get
;


: nci-children   ( w:nci - n = Return the number of children of the current node )
  nni-children
;


: nci-children?  ( w:nci - f = Check if the current node has children )
  nni-children?
;


: nci-child    ( w:nci - w:data true | false = Move the iterator to the first child of the current node )
  nni-child nni+get
;


: nci-prepend-child  ( w:data w:nci - = Prepend data as child to the children of the current node, iterator is moved to the new child )
  >r ncn-new r> nni-prepend-child
;


: nci-append-child  ( w:data w:nci - = Append a child to the children of the current node, iterator is moved to the new child )
  >r ncn-new r> nni-append-child
;


( Sibling iterator words )

: nci-first    ( w:nci - w:data true | false = Move the iterator to the first sibling )
  nni-first nni+get
;


: nci-next     ( w:nci - w:data true | false = Move the iterator to the next sibling )
  nni-next nni+get
;


: nci-prev     ( w:nci - w:data true | false = Move the iterator to the previous sibling )
  nni-prev nni+get
;


: nci-last     ( w:nci - w:data true | false = Move the iterator to the last sibling )
  nni-last nni+get
;


: nci-first?   ( w:nci - f = Check if the iterator is on the first sibling )
  nni-first?
;


: nci-last?    ( w:nci - f = Check if the iterator is on the last sibling )
  nni-last?
;


: nci-insert-before  ( w:nnn w:nci - = Insert a sibling before the current sibling in the tree)
  >r ncn-new r> nni-insert-before
;


: nci-insert-after  ( w:nnn w:nci - = Insert a sibling after the current sibling in the tree)
  >r ncn-new r> nni-insert-after
;


: nci-remove        ( w:nci - w:data true | false = Remove the current sibling without children from the tree, move the iterator to the next, previous or parent node )
  nni-remove nni+get
;

  
( Inspection )

: nci-dump     ( w:nci - = Dump the iterator )
  nni-dump
;

[THEN]

\ ==============================================================================
