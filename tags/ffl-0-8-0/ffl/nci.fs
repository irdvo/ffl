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
\  $Date: 2008-02-03 07:09:34 $ $Revision: 1.4 $
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

nni% constant nci% ( -- n = Get the required space for a nci variable )


( Iterator creation, initialisation and destruction )

: nci-init     ( nct nci -- = Initialise the iterator with the n-tree nct )
  nni-init
;


: nci-create   ( nct "<spaces>name" -- ; -- nci = Create a named iterator in the dictionary on the n-tree nct )
  create  here  nci% allot  nci-init
;


: nci-new      ( nct -- nci = Create an iterator on the heap on the n-tree nct )
  nci% allocate  throw  tuck nci-init
;


: nci-free     ( nci -- = Free the iterator from heap )
  free throw
;


( Private words )

: nni+get      ( ncn -- x true | false = Get the cell data x from the current node )
  nil<>? IF
    ncn-cell@ true
  ELSE
    false
  THEN
;


( Member words )

: nci-get      ( nci -- x true | false = Get the cell data x from the current node )
  nni-get nni+get
;


( Tree iterator words )

: nci-root?    ( nci -- flag = Check if the current node is the root node )
  nni-root?
;


: nci-root     ( nci -- x true | false = Move the iterator to the root of the tree, return the cell data x from this node )
  nni-root nni+get
;


: nci-parent   ( nci -- x true | false = Move the iterator to the parent of the current node, return the cell data x from this node )
  nni-parent nni+get
;


: nci-children   ( nci -- n = Return the number of children of the current node )
  nni-children
;


: nci-children?  ( nci -- flag = Check if the current node has children )
  nni-children?
;


: nci-child    ( nci -- x true | false = Move the iterator to the first child of the current node, return the cell data x of this node )
  nni-child nni+get
;


: nci-prepend-child  ( x nci -- = Prepend data x as child to the children of the current node, iterator is moved to the new child )
  >r ncn-new r> nni-prepend-child
;


: nci-append-child  ( x nci -- = Append data x as child to the children of the current node, iterator is moved to the new child )
  >r ncn-new r> nni-append-child
;


( Sibling iterator words )

: nci-first    ( nci -- x true | false = Move the iterator to the first sibling, return the cell data x from this node )
  nni-first nni+get
;


: nci-next     ( nci -- x true | false = Move the iterator to the next sibling, return the cell data x from this node )
  nni-next nni+get
;


: nci-prev     ( nci -- x true | false = Move the iterator to the previous sibling, return the cell data x from this node )
  nni-prev nni+get
;


: nci-last     ( nci -- x true | false = Move the iterator to the last sibling, return the cell data x from this node )
  nni-last nni+get
;


: nci-first?   ( nci -- flag = Check if the iterator is on the first sibling )
  nni-first?
;


: nci-last?    ( nci -- flag = Check if the iterator is on the last sibling )
  nni-last?
;


: nci-insert-before  ( x nci -- = Insert data x as sibling before the current sibling in the tree )
  >r ncn-new r> nni-insert-before
;


: nci-insert-after  ( x nci -- = Insert data x as sibling after the current sibling in the tree )
  >r ncn-new r> nni-insert-after
;


: nci-remove        ( nci -- x true | false = Remove the current sibling without children from the tree, move the iterator to the next, previous or parent node, return the cell data x of the removed node )
  nni-remove nni+get
;

  
( Inspection )

: nci-dump     ( nci -- = Dump the iterator )
  nni-dump
;

[THEN]

\ ==============================================================================
