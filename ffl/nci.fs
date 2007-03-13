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
\  $Date: 2007-03-13 06:03:05 $ $Revision: 1.1 $
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


( Member words )

: nni+get      ( w:ncn - w:data true | false = Get the cell data from the current node )
  dup nil<> IF
    ncn-cell@ true
  ELSE
    drop false
  THEN
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


\ ToDo
: nci-prepend-child  ( w:nnn w:nci - = Prepend a child to the children of the current node, iterator is moved to the new child )
  2dup nci>nnt @ nnt>root
  dup @ nil= IF                      \ If nnt.root = nil Then  S: nnn nci nnn root^
    !                                \   root       = nnn
    dup nci>nnt @ nnt>length 0!      \   nnt.length = 0
    nil                              \   parent     = nil
  ELSE                               \ Else
    drop
    over nci-get                     \   If nci.walk <> nil Then
      
    nci+throw
    
    tuck                             \     Parent = walk
    nnn>children dnl-prepend         \     Prepend nnn to nci.walk.children
  THEN
  rot tuck                           \ nnn.parent = parent
  nnn-parent!
  over nci>walk !                    \ nci.walk   = nnn
  nci>nnt @ nnt>length 1+!           \ nnt.length++
;

\ ToDo
: nci-append-child  ( w:nnn w:nci - = Append a child to the children of the current node, iterator is moved to the new child )
  2dup nci>nnt @ nnt>root
  dup @ nil= IF                      \ If nnt.root = nil Then  S: nnn nci nnn root^
    !                                \   root       = nnn
    dup nci>nnt @ nnt>length 0!      \   nnt.length = 0
    nil                              \   parent     = nil
  ELSE                               \ Else
    drop
    over nci-get                     \   If nci.walk <> nil Then
      
    nci+throw
      
    tuck                             \     Parent = walk
    nnn>children dnl-append          \     Append nnn to nci.walk.children
  THEN
  rot tuck                           \ nnn.parent = parent
  nnn-parent!
  over nci>walk !                    \ nci.walk   = nnn
  nci>nnt @ nnt>length 1+!           \ nnt.length++
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


\ ToDo
: nci-insert-before  ( w:nnn w:nci - = Insert a sibling before the current sibling in the tree)
  tuck
  nci-get 2dup
  
  nci+throw
  
  nnn-parent@              \ S: nci nnn walk nnn parent
    
  nci+throw
  
  tuck
  swap nnn-parent!         \ nnn.parent = walk.parent
  nnn>children             \ Insert before in the children list
  dnl-insert-before        
  nci>nnt @ nnt>length 1+! \ tree.length++
;

\ ToDo
: nci-insert-after  ( w:nnn w:nci - = Insert a sibling after the current sibling in the tree)
  tuck
  nci-get 2dup
  
  nci+throw
  
  nnn-parent@              \ S: nci nnn walk nnn parent
  
  nci+throw
  
  tuck
  swap nnn-parent!         \ nnn.parent = walk.parent
  nnn>children             \ Insert after in the children list
  dnl-insert-after       
  nci>nnt @ nnt>length 1+! \ tree.length++
;

\ ToDo
: nci-remove        ( w:nci - w:nnn = Remove the current sibling without children from the tree, move the iterator to the next, previous or parent node )
  dup nci-get
  \ Checks
  nci+throw                              \ Error if iterator is not on a node
  
  dup nnn>children dnl-first@            \ Error if node has children
  
  nil<> exp-invalid-state AND throw
  
  tuck
  
  \ Next iterator S: nnn nci nnn
  nnn>dnn
  
  dup dnn-next@ dup nil<> IF             \ If next <> nil Then
    nip                                  \   walk = next
  ELSE
    drop dup dnn-prev@ dup nil<> IF      \ Else if prev <> nil Then
      nip                                \   walk = next
    ELSE                                 \ Else
      2drop over                         \   walk = parent
      nnn-parent@
    THEN
  THEN
  over nci>walk !
  
  \ Remove node from list S: nnn nci
  over dup nnn-parent@ dup nil= IF       \ If parent = nil Then (root node)
    2drop dup nci>nnt @ nnt>root nil!    \   Remove node by clearing root node
  ELSE                                   \ Else
    over nnn>parent nil!                 \   Parent = nil
    nnn>children dnl-remove              \   Parent.children remove node
  THEN
  nci>nnt @ nnt>length 1-!               \ Tree.length--
;

  
( Inspection )

: nci-dump     ( w:nci - = Dump the iterator )
  nni-dump
;

[THEN]

\ ==============================================================================
