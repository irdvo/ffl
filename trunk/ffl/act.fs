\ ==============================================================================
\
\            act - the AVL binary tree cell module in the ffl
\
\               Copyright (C) 2006  Dick van Oudheusden
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
\  $Date: 2006-10-23 17:39:45 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] act.version [IF]


include ffl/stc.fs
include ffl/acn.fs
include ffl/bct.fs

( act = AVL balanced binary tree cell module )
( The act module implements an AVL binary tree with the key and data cell based.  )
( The act module is a specialisation of the bct module, so the words bct-length@, )
( bct-empty?, bct-compare@, bct-compare!, bct-get, bct-has?, bct-execute and      )
( bct-dump are inherited by act from bct. Also the tree iterator bci can be used  )
( for an act tree. The implementation is non-recursive.                           )


1 constant act.version


( Private words )

: act-rotate-left  ( w:root w:bct - w:root' = Rotate nodes to the left for balance )
  >r
  dup bcn-right@                       \ root' = root.right
  2dup bcn-left@ swap bcn>right !      \ root.right = root'.left
  2dup bcn-left@ bcn-parent!           \ root'.left.parent = root
  2dup bcn>left !                      \ root'.left = root
  swap
  2dup bcn>parent @!                   \ root.parent = root'
  dup nil= IF                          \ IF root.parent = nil Then
    2drop
    dup r@ bct>root !                  \   tree.root = root
    nil
  ELSE                                 \ Else
    tuck bcn-left@ = IF                \  IF root = parent.left Then
      2dup bcn>left !                  \    parent.left = root'
    ELSE                               \  Else
      2dup bcn>right !                 \    parent.right = root'
    THEN
  THEN
  over bcn-parent!                     \ root'.parent = parent or nil
  rdrop
;


: act-rotate-right ( w:root w:bct - w:root' = Rotate nodes to the right for balance )
  >r
  dup bcn-left@                        \ root' = root.left
  2dup bcn-right@ swap bcn>left !      \ root.left = root'.right
  2dup bcn-right@ bcn-parent!          \ root'.right.parent = root
  2dup bcn>right !                     \ root'.right = root
  swap
  2dup bcn>parent @!                   \ root.parent = root'
  dup nil= IF                          \ IF root.parent = nil Then
    2drop
    dup r@ bct>root !                  \   tree.root = root
    nil
  ELSE                                 \ Else
    tuck bcn-left@ = IF                \  IF root = parent.left Then
      2dup bcn>left !                  \    parent.left = root'
    ELSE                               \  Else
      2dup bcn>right !                 \    parent.right = root'
    THEN
  THEN
  over bcn-parent!                     \ root'.parent = parent or nil
  rdrop
;


: act-heavy-left   ( w:acn w:bct - w:acn = Change the tree if tree is left heavy )
  >r
  dup bcn-left@ acn-balance@ 0< IF     \ If node.left.balance = LEFT Then
    dup acn>balance 0!                 \   node.balance = equal
    dup bcn-left@ acn>balance 0!       \   node.left.balance = equal
    r@ act-rotate-right                \   rotate subtree right
  ELSE                                 \ Else
    >r                                 \   If node.left.right.balance = equal Then
    0 r@ bcn-left@ bcn-right@ acn>balance @! ?dup 0= IF
      0 0                              \     equal, equal
    ELSE                               \   Else
      0< IF                            \     If ...balance = LEFT Then
        1 0                            \       right, equal
      ELSE                             \     Else ...balance = RIGHT 
        0 -1                           \       equal, left
      THEN
    THEN
    r@ bcn-left@ acn>balance !         \   Set node.left.balance
    r@ acn>balance !                   \   Set node.balance
    r>
    dup bcn-left@ r@ act-rotate-left drop \  rotate left subtree to the left
    r@ act-rotate-right                \  rotate subtree to the right
  THEN
  rdrop
;


: act-heavy-right  ( w:acn w:bct - w:acn = Change the tree if tree is right heavy )
  >r
  dup bcn-right@ acn-balance@ 0> IF    \ If node.right.balance = RIGHT Then
    dup acn>balance 0!                 \   node.balance = equal
    dup bcn-right@ acn>balance 0!      \   node.right.balance = equal
    r@ act-rotate-left                 \   rotate subtree left
  ELSE                                 \ Else
    >r                                 \   If node.right.left.balance = equal Then
    0 r@ bcn-right@ bcn-left@ acn>balance @! ?dup 0= IF
      0 0                              \     equal, equal
    ELSE                               \   Else
      0< IF                            \     If ...balance = LEFT Then
        0 1                            \       equal, right
      ELSE                             \     Else ...balance = RIGHT 
        -1 0                           \       left, equal
      THEN
    THEN
    r@ bcn-right@ acn>balance !        \   Set node.right.balance
    r@ acn>balance !                   \   Set node.balance
    r>
    dup bcn-right@ r@ act-rotate-right drop \  rotate right subtree to the right
    r@ act-rotate-left                 \  rotate subtree to the left
  THEN
  rdrop
;


: act-grown-left   ( w:acn w:bct - w:acn f:rebalance? = Update balance for the node after grown to the left )
  >r
  dup acn-balance@ ?dup 0= IF               \ If this node was balanced Then
    -1 over acn>balance !                   \   Heavy to the left
    true
  ELSE
    0< IF                                   \ Else If alreay heavy to the left Then
      r@ act-heavy-left                     \   Rotate to make it balanced
    THEN                                    \ Else if heavy to the right Then
    dup acn>balance 0!                      \   Balanced.
    false
  THEN
  rdrop
;


: act-grown-right  ( w:acn w:bct - w:acn f:rebalance? = Update balance for the node after grown to the right )
  >r
  dup acn-balance@ ?dup 0= IF               \ If this node was balanced Then
    1 over acn>balance !                    \   Heavy to the right
    true
  ELSE
    0> IF                                   \ Else If alreay heavy to the right Then
      r@ act-heavy-right                    \   Rotate to make it balanced
    THEN                                    \ Else if heavy to the left Then
    dup acn>balance 0!                      \   Balanced.
    false
  THEN
  rdrop
;

 

: act-delete-node  ( w:key xt w:node - false | w:data true f:rebalance w:node = Search and delete the node )
  dup nil= IF                          \ If nil then
    nip nip
    false false rot                    \  key not present in tree
  ELSE
    >r
    2dup r@ bcn>key @ swap execute
    r>
    swap
    ?dup 0= IF                         \ If key found
      nip nip
      dup bcn>cell @
      swap 
      true true rot
      \ act-remove-node: fix tree and remove node
    ELSE
      0< IF
        >r
        r@ bcn-left@ recurse dup
        r@ bcn>left !
        r@ swap bcn-parent!
        r> \ act-shrink-left
      ELSE
        >r
        r@ bcn-right@ recurse dup
        r@ bcn>right !
        r@ swap bcn-parent!
        r> \ act-shrink-right
      THEN
    THEN
  THEN
;
 
   
( Tree words )

: act-insert       ( w:data w:key w:bct - = Insert data with a key in the tree )
  >r ['] acn-new -rot r@
  bct-insert-node
  
  IF                                   \ If inserted Then
    dup bcn-parent@ true               \   Balance the tree
    BEGIN
      over nil<> AND                   \   While nodes and not balanced Do
    WHILE
      tuck bcn-left@ = IF              \   If walked left Then
        r@ act-grown-left              \     Grown left
      ELSE                             \   Else
        r@ act-grown-right             \     Grown right
      THEN
      >r dup bcn-parent@ r>            \   Move to the parent
    REPEAT
    drop
  THEN
  drop
  rdrop
;


: act-delete       ( w:key w:bct - false | w:data true = Delete the key from the tree )
  >r
  r@ bct>compare @
  r@ bct>root @
  act-delete-node
  r@ bct>root !
  drop dup IF
    -1 r@ bct>length +!
  THEN
  rdrop
;


( Private words )

: act-test-node    ( w:node - = Test node )
  dup nil<> IF                        \ If the node exists
    >r
    r@ .
    r@ bcn>key ?
    r@ bcn>cell ?
    r@ acn>balance ?
    cr ." Left="
    
    r@ bcn-left@ recurse             \   walk the left branch
    
    cr ." Right=" 
    
    r> bcn-right@ recurse            \   walk the right branch
  ELSE
    ." Nil"
    drop
  THEN
;

: act-test         ( w:act - = Test the tree )
  cr ." Root="
  bct>root @ act-test-node
;

[THEN]

\ ==============================================================================
