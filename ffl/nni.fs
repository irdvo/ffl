\ ==============================================================================
\
\             nni - the generic n-Tree iterator in the ffl
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
\  $Date: 2008-01-09 19:30:48 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nni.version [IF]


include ffl/nnt.fs


( nni = Generic n-Tree Iterator )
( The nni module implements an iterator on the generic n-Tree [nnt]. )


1 constant nni.version


( Iterator structure )

begin-structure nni%       ( -- n = Get the required space for a nni variable )
  field: nni>nnt
  field: nni>walk
end-structure


( Iterator creation, initialisation and destruction )

: nni-init     ( nnt nni -- = Initialise the iterator with the n-tree nnt )
  tuck nni>nnt     !
       nni>walk  nil!
;


: nni-create   ( nnt "<spaces>name" -- ; -- nni = Create a named iterator in the dictionary on the n-tree nnt )
  create 
    here  nni% allot  nni-init
;


: nni-new      ( nnt -- nni = Create an iterator on the heap on the n-tree nnt )
  nni% allocate  throw  tuck nni-init
;


: nni-free     ( nni -- = Free the iterator from the heap )
  free throw
;


( Member words )

: nni-get      ( nni -- nnn | nil = Get the current node )
  nni>walk @
;


( Private words )

: nni+throw     ( nnn -- nnn = Raise exception if nnn is nil )
  dup nil= exp-invalid-state AND throw
;


( Tree iterator words )

: nni-root?    ( nni -- flag = Check if the current node is the root node )
  dup 
  nni>nnt @ nnt-root@
  swap
  nni-get
  =
;


: nni-root     ( nni -- nnn | nil = Move the iterator to the root of the tree, return this node )
  dup
  nni>nnt @ nnt-root@
  dup rot
  nni>walk !
;


: nni-parent   ( nni -- nnn | nil = Move the iterator to the parent of the current node, return this node )
  nni>walk
  dup @
  
  nni+throw
  
  nnn-parent@
  dup rot !
;


: nni-children   ( nni -- n = Return the number of children of the current node )
  nni-get
  
  nni+throw 
  
  nnn>children dnl-length@
;


: nni-children?  ( nni -- flag = Check if the current node has children )
  nni-get
  
  nni+throw
  
  nnn>children dnl-first@ nil<>
;


: nni-child    ( nni -- nnn | nil = Move the iterator to the first child of the current node, return this node )
  nni>walk dup @
  
  nni+throw
  
  nnn>children dnl-first@
  dup rot !
;


: nni-prepend-child  ( nnn nni -- = Prepend a child to the children of the current node, iterator is moved to the new child )
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


: nni-append-child  ( nnn nni -- = Append a child to the children of the current node, iterator is moved to the new child )
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

: nni-first    ( nni -- nnn = Move the iterator to the first sibling, return this node )
  nni>walk
  dup @
  
  nni+throw
    
  nnn-parent@             
    
  nni+throw
  
  nnn>children dnl-first@          \  walk = walk.parent.children.first
  dup rot !
;


: nni-next     ( nni -- nnn | nil = Move the iterator to the next sibling, return this node )
  nni>walk 
  dup @
  
  nni+throw
  
  nnn>dnn dnn-next@                \   walk = walk.next
  dup rot !
;


: nni-prev     ( nni -- nnn | nil = Move the iterator to the previous sibling )
  nni>walk 
  dup @
  
  nni+throw
  
  nnn>dnn dnn-prev@                \   walk = walk.prev
  dup rot !
;


: nni-last     ( nni -- nnn = Move the iterator to the last sibling, return this node )
  nni>walk
  dup @
  
  nni+throw
  
  nnn-parent@
  
  nni+throw
  
  nnn>children dnl-last@           \  walk = walk.parent.children.last
  dup rot !
;


: nni-first?   ( nni -- flag = Check if the iterator is on the first sibling )
  nni-get dup
  
  nni+throw
  
  nnn-parent@
  
  nni+throw

  nnn>children dnl-first@          \  walk =?= walk.parent.children.first
  =
;


: nni-last?    ( nni -- flag = Check if the iterator is on the last sibling )
  nni-get dup
  
  nni+throw
  
  nnn-parent@
  
  nni+throw
  
  nnn>children dnl-last@           \  walk =?= walk.parent.children.last
  =
;


: nni-insert-before  ( nnn nni -- = Insert a sibling before the current sibling in the tree )
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


: nni-insert-after  ( nnn nni -- = Insert a sibling after the current sibling in the tree )
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


: nni-remove        ( nni -- nnn = Remove the current sibling without children from the tree, move the iterator to the next, previous or parent node, return the removed node )
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

: nni-dump     ( nni -- = Dump the iterator )
  ." nni:" dup . cr
  ."  tree:" dup nni>nnt  ?  cr
  ."  walk:"     nni>walk  ?  cr
;

[THEN]

\ ==============================================================================
