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
\  $Date: 2007-03-11 07:56:07 $ $Revision: 1.2 $
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


( Private words )

: nni+throw     ( w:nnn - w:nnn = Raise exception if nnn is nil )
  dup nil= exp-invalid-state AND throw
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
  
  nni+throw
  
  nnn-parent@
  dup rot !
;


: nni-children   ( w:nni - n = Return the number of children of the current node )
  nni-get
  
  nni+throw 
  
  nnn>children dnl-length@
;


: nni-children?  ( w:nni - f = Check if the current node has children )
  nni-get
  
  nni+throw
  
  nnn>children dnl-first@ nil<>
;


: nni-child    ( w:nni - w:nnn | nil = Move the iterator to the first child of the current node )
  nni>walk dup @
  
  nni+throw
  
  nnn>children dnl-first@
  dup rot !
;


: nni-prepend-child  ( w:nnn w:nni - = Prepend a child to the children of the current node, iterator is moved to the new child )
  2dup nni>nnt @ nnt>root
  dup @ nil= IF                      \ If nnt.root = nil Then  S: nnn nni nnn root^
    !                                \   root       = nnn
    dup nni>nnt @ nnt>length 0!      \   nnt.length = 0
    nil                              \   parent     = nil
  ELSE                               \ Else
    drop
    over nni-get                     \   If nni.walk <> nil Then
      
    nni+throw
    
    tuck                             \     Parent = walk
    nnn>children dnl-prepend         \     Prepend nnn to nni.walk.children
  THEN
  rot tuck                           \ nnn.parent = parent
  nnn-parent!
  over nni>walk !                    \ nni.walk   = nnn
  nni>nnt @ nnt>length 1+!           \ nnt.length++
;


: nni-append-child  ( w:nnn w:nni - = Append a child to the children of the current node, iterator is moved to the new child )
  2dup nni>nnt @ nnt>root
  dup @ nil= IF                      \ If nnt.root = nil Then  S: nnn nni nnn root^
    !                                \   root       = nnn
    dup nni>nnt @ nnt>length 0!      \   nnt.length = 0
    nil                              \   parent     = nil
  ELSE                               \ Else
    drop
    over nni-get                     \   If nni.walk <> nil Then
      
    nni+throw
      
    tuck                             \     Parent = walk
    nnn>children dnl-append          \     Append nnn to nni.walk.children
  THEN
  rot tuck                           \ nnn.parent = parent
  nnn-parent!
  over nni>walk !                    \ nni.walk   = nnn
  nni>nnt @ nnt>length 1+!           \ nnt.length++
;


( Sibling iterator words )

: nni-first    ( w:nni - w:nnn = Move the iterator to the first sibling )
  nni>walk
  dup @
  
  nni+throw
    
  nnn-parent@             
    
  nni+throw
  
  nnn>children dnl-first@          \  walk = walk.parent.children.first
  dup rot !
;


: nni-next     ( w:nni - w:nnn | nil = Move the iterator to the next sibling )
  nni>walk 
  dup @
  
  nni+throw
  
  nnn>dnn dnn-next@                \   walk = walk.next
  dup rot !
;


: nni-prev     ( w:nni - w:nnn | nil = Move the iterator to the previous sibling )
  nni>walk 
  dup @
  
  nni+throw
  
  nnn>dnn dnn-prev@                \   walk = walk.prev
  dup rot !
;


: nni-last     ( w:nni - w:nnn = Move the iterator to the last sibling )
  nni>walk
  dup @
  
  nni+throw
  
  nnn-parent@
  
  nni+throw
  
  nnn>children dnl-last@           \  walk = walk.parent.children.last
  dup rot !
;


: nni-first?   ( w:nni - f = Check if the iterator is on the first sibling )
  nni-get dup
  
  nni+throw
  
  nnn-parent@
  
  nni+throw

  nnn>children dnl-first@          \  walk =?= walk.parent.children.first
  =
;


: nni-last?    ( w:nni - f = Check if the iterator is on the last sibling )
  nni-get dup
  
  nni+throw
  
  nnn-parent@
  
  nni+throw
  
  nnn>children dnl-last@           \  walk =?= walk.parent.children.last
  =
;


: nni-insert-before  ( w:nnn w:nni - = Insert a sibling before the current sibling in the tree)
  tuck
  nni-get 2dup
  
  nni+throw
  
  nnn-parent@              \ S: nni nnn walk nnn parent
    
  nni+throw
  
  tuck
  swap nnn-parent!         \ nnn.parent = walk.parent
  nnn>children             \ Insert before in the children list
  dnl-insert-before        
  nni>nnt @ nnt>length 1+! \ tree.length++
;


: nni-insert-after  ( w:nnn w:nni - = Insert a sibling after the current sibling in the tree)
  tuck
  nni-get 2dup
  
  nni+throw
  
  nnn-parent@              \ S: nni nnn walk nnn parent
  
  nni+throw
  
  tuck
  swap nnn-parent!         \ nnn.parent = walk.parent
  nnn>children             \ Insert after in the children list
  dnl-insert-after       
  nni>nnt @ nnt>length 1+! \ tree.length++
;


: nni-remove        ( w:nni - w:nnn = Remove the current sibling without children from the tree, move the iterator to the next, previous or parent node )
  dup nni-get
  \ Checks
  nni+throw                              \ Error if iterator is not on a node
  
  dup nnn>children dnl-first@            \ Error if node has children
  
  nil<> exp-invalid-state AND throw
  
  tuck
  
  \ Next iterator S: nnn nni nnn
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
  over nni>walk !
  
  \ Remove node from list S: nnn nni
  over dup nnn-parent@ dup nil= IF       \ If parent = nil Then (root node)
    2drop dup nni>nnt @ nnt>root nil!    \   Remove node by clearing root node
  ELSE                                   \ Else
    over nnn>parent nil!                 \   Parent = nil
    nnn>children dnl-remove              \   Parent.children remove node
  THEN
  nni>nnt @ nnt>length 1-!               \ Tree.length--
;

  
( Inspection )

: nni-dump     ( w:nni - = Dump the iterator )
  ." nni:" dup . cr
  ."  tree:" dup nni>nnt  ?  cr
  ."  walk:"     nni>walk  ?  cr
;

[THEN]

\ ==============================================================================
