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
\  $Date: 2008-04-24 16:50:23 $ $Revision: 1.8 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] act.version [IF]


include ffl/stc.fs
include ffl/acn.fs
include ffl/bct.fs


( act = AVL binary tree cell module )
( The act module implements an AVL binary tree with the key and data cell      )
( based. The act module is a specialization of the bct module. As a result the )
( bci iterator can be used as iterator on the act tree. The implementation is  )
( non-recursive.                                                               )


3 constant act.version


bct% constant act%      ( -- n = Get the required space for an act variable )


( Tree creation, initialisation and destruction )

: act-init         ( act -- = Initialise the act tree )
  bct-init
;


: act-(free)       ( act -- = Free the nodes from the heap )
  bct-(free)
;


: act-create       ( "<spaces>name" -- ; -- act = Create a named tree in the dictionary )
  bct-create
;


: act-new          ( -- act = Create a new tree on the heap )
  bct-new
;


: act-free         ( act -- = Free the tree from the heap )\
  bct-free
;


( Member words )

: act-length@      ( act -- u = Get the number of elements in the tree )
  bct-length@
;


: act-empty?       ( act -- flag = Check for an empty tree )
  bct-empty?
;


: act-compare@     ( act -- xt = Get the compare execution token for comparing keys )
  bct-compare@
;


: act-compare!     ( xt act -- = Set the compare execution token for comparing keys )
  bct-compare!
;


( Private words )

: act-rotate-left  ( acn1 act -- acn2 = Rotate nodes to the left for balance, return the root node of the subtree )
  >r
  dup bnn-right@                       \ root' = root.right
  2dup bnn-left@ swap bnn>right !      \ root.right = root'.left
  2dup bnn-left@ bnn-parent!           \ root'.left.parent = root
  2dup bnn>left !                      \ root'.left = root
  swap
  2dup bnn>parent @!                   \ root.parent = root'
  dup nil= IF                          \ IF root.parent = nil Then
    2drop
    dup r@ bnt>root !                  \   tree.root = root
    nil
  ELSE                                 \ Else
    tuck bnn-left@ = IF                \  IF root = parent.left Then
      2dup bnn>left !                  \    parent.left = root'
    ELSE                               \  Else
      2dup bnn>right !                 \    parent.right = root'
    THEN
  THEN
  over bnn-parent!                     \ root'.parent = parent or nil
  rdrop
;


: act-rotate-right ( acn1 act -- acn2 = Rotate nodes to the right for balance, return the new root node of the subtree )
  >r
  dup bnn-left@                        \ root' = root.left
  2dup bnn-right@ swap bnn>left !      \ root.left = root'.right
  2dup bnn-right@ bnn-parent!          \ root'.right.parent = root
  2dup bnn>right !                     \ root'.right = root
  swap
  2dup bnn>parent @!                   \ root.parent = root'
  dup nil= IF                          \ IF root.parent = nil Then
    2drop
    dup r@ bnt>root !                  \   tree.root = root
    nil
  ELSE                                 \ Else
    tuck bnn-left@ = IF                \  IF root = parent.left Then
      2dup bnn>left !                  \    parent.left = root'
    ELSE                               \  Else
      2dup bnn>right !                 \    parent.right = root'
    THEN
  THEN
  over bnn-parent!                     \ root'.parent = parent or nil
  rdrop
;


: act-heavy-left   ( acn1 act -- acn2 = Change the tree if tree is left heavy, return the new root node of the subtree)
  >r
  dup bnn-left@ acn-balance@ 0< IF     \ If node.left.balance = LEFT Then
    dup acn>balance 0!                 \   node.balance = equal
    dup bnn-left@ acn>balance 0!       \   node.left.balance = equal
    r@ act-rotate-right                \   rotate subtree right
  ELSE                                 \ Else
    >r                                 \   If node.left.right.balance = equal Then
    0 r@ bnn-left@ bnn-right@ acn>balance @! ?dup 0= IF
      0 0                              \     equal, equal
    ELSE                               \   Else
      0< IF                            \     If ...balance = LEFT Then
        1 0                            \       right, equal
      ELSE                             \     Else ...balance = RIGHT 
        0 -1                           \       equal, left
      THEN
    THEN
    r@ bnn-left@ acn>balance !         \   Set node.left.balance
    r@ acn>balance !                   \   Set node.balance
    r>
    dup bnn-left@ r@ act-rotate-left drop \  rotate left subtree to the left
    r@ act-rotate-right                \  rotate subtree to the right
  THEN
  rdrop
;


: act-heavy-right  ( acn1 act -- acn2 = Change the tree if tree is right heavy, return the new root node of the subtree )
  >r
  dup bnn-right@ acn-balance@ 0> IF    \ If node.right.balance = RIGHT Then
    dup acn>balance 0!                 \   node.balance = equal
    dup bnn-right@ acn>balance 0!      \   node.right.balance = equal
    r@ act-rotate-left                 \   rotate subtree left
  ELSE                                 \ Else
    >r                                 \   If node.right.left.balance = equal Then
    0 r@ bnn-right@ bnn-left@ acn>balance @! ?dup 0= IF
      0 0                              \     equal, equal
    ELSE                               \   Else
      0< IF                            \     If ...balance = LEFT Then
        0 1                            \       equal, right
      ELSE                             \     Else ...balance = RIGHT 
        -1 0                           \       left, equal
      THEN
    THEN
    r@ bnn-right@ acn>balance !        \   Set node.right.balance
    r@ acn>balance !                   \   Set node.balance
    r>
    dup bnn-right@ r@ act-rotate-right drop \  rotate right subtree to the right
    r@ act-rotate-left                 \  rotate subtree to the left
  THEN
  rdrop
;


: act-grown-left   ( acn1 act -- acn2 flag = Update balance for the node after grown to the left, return new root subtree and rebalance flag )
  >r
  dup acn-balance@ ?dup 0= IF               \ If this node was balanced Then
    -1 over acn>balance !                   \   Heavy to the left
    true
  ELSE
    0< IF                                   \ Else If already heavy to the left Then
      r@ act-heavy-left                     \   Rotate to make it balanced
    ELSE                                    \ Else if heavy to the right Then
      dup acn>balance 0!                    \   Balanced.
    THEN
    false
  THEN
  rdrop
;


: act-grown-right  ( acn1 act -- acn2 flag = Update balance for the node after grown to the right, return new root subtree and rebalance flag )
  >r
  dup acn-balance@ ?dup 0= IF               \ If this node was balanced Then
    1 over acn>balance !                    \   Heavy to the right
    true
  ELSE
    0> IF                                   \ Else If already heavy to the right Then
      r@ act-heavy-right                    \   Rotate to make it balanced
    ELSE                                    \ Else if heavy to the left Then
      dup acn>balance 0!                    \   Balanced.
    THEN
    false
  THEN
  rdrop
;

 
: act-replace-node ( acn1 -- acn2 = Replace the node with another node )
  dup bnn-left@ nil<> over bnn-right@ nil<> AND IF \ Both branches not nil Then
    dup acn-balance@ 0< IF                  \ If subtree is left heavy Then
      dup bnn-left@ bnt-greatest-node       \   Find the replacement in the left subtree
    ELSE                                    \ Else
      dup bnn-right@ bnt-smallest-node      \   In the right subtree
    THEN
  
    2dup bnn-key@  swap bnn>key !           \   Copy the contents
    tuck bcn-cell@ swap bcn-cell! 
  THEN
;


: act-light-right  ( acn1 act -- acn2 flag = Change the tree if tree is right light, return new root subtree and rebalance flag )
  >r
  dup bnn-left@ acn-balance@ 0< IF     \ If node.left.balance = LEFT Then
    dup acn>balance 0!                 \   node.balance = equal
    dup bnn-left@ acn>balance 0!       \   node.left.balance = equal
    r@ act-rotate-right                \   rotate subtree right
    true
  ELSE                                 \ Else
    dup bnn-left@ acn-balance@ 0= IF
      -1 over acn>balance !
       1 over bnn-left@ acn>balance !
      r@ act-rotate-right
      false
    ELSE
      >r                                 \   If node.left.right.balance = equal Then
      0 r@ bnn-left@ bnn-right@ acn>balance @! ?dup 0= IF
        0 0                              \     equal, equal
      ELSE                               \   Else
        0< IF                            \     If ...balance = LEFT Then
          1 0                            \       right, equal
        ELSE                             \     Else ...balance = RIGHT 
          0 -1                           \       equal, left
        THEN
      THEN
      r@ bnn-left@ acn>balance !         \   Set node.left.balance
      r@ acn>balance !                   \   Set node.balance
      r>
      dup bnn-left@ r@ act-rotate-left drop \  rotate left subtree to the left
      r@ act-rotate-right                \  rotate subtree to the right
      true
    THEN
  THEN
  rdrop
;


: act-light-left   ( acn1 act -- acn2 flag = Change the tree if tree is left light, return new root subtree and rebalance flag )
  >r
  dup bnn-right@ acn-balance@ 0> IF    \ If node.right.balance = RIGHT Then
    dup acn>balance 0!                 \   node.balance = equal
    dup bnn-right@ acn>balance 0!      \   node.right.balance = equal
    r@ act-rotate-left                 \   rotate subtree left
    true
  ELSE                                 \ Else
    dup bnn-right@ acn-balance@ 0= IF
       1 over acn>balance !            \   node.balance = right
      -1 over bnn-right@ acn>balance ! \   node.right.balance = left
      r@ act-rotate-left               \   rotate subtree left
      false
    ELSE
      >r                                 \   If node.right.left.balance = equal Then
      0 r@ bnn-right@ bnn-left@ acn>balance @! ?dup 0= IF
        0 0                              \     equal, equal
      ELSE                               \   Else
        0< IF                            \     If ...balance = LEFT Then
          0 1                            \       equal, right
        ELSE                             \     Else ...balance = RIGHT 
          -1 0                           \       left, equal
        THEN
      THEN
      r@ bnn-right@ acn>balance !        \   Set node.right.balance
      r@ acn>balance !                   \   Set node.balance
      r>
      dup bnn-right@ r@ act-rotate-right drop \  rotate right subtree to the right
      r@ act-rotate-left                 \  rotate subtree to the left
      true
    THEN
  THEN
  rdrop
;


: act-shrunk-left   ( acn1 act -- acn2 flag = Update balance for the node after shrunk to the left, return new root subtree and rebalance flag )
  >r
  dup acn-balance@ ?dup 0= IF               \ If this node was balanced Then
    1 over acn>balance !                    \   Heavy to the right
    false
  ELSE
    0> IF                                   \ Else If already heavy to the right Then
      r@ act-light-left                     \   Rotate to make it balanced
    ELSE                                    \ Else if heavy to the right Then
      dup acn>balance 0!                    \   Balanced.
      true
    THEN
  THEN
  rdrop
;


: act-shrunk-right  ( acn1 act -- acn2 flag = Update balance for the node after shrunk to the right, return new root subtree and rebalance flag )
  >r
  dup acn-balance@ ?dup 0= IF               \ If this node was balanced Then
    -1 over acn>balance !                   \   Heavy to the left
    false
  ELSE
    0< IF                                   \ Else If already heavy to the left Then
      r@ act-light-right                    \   Rotate to make it balanced
    ELSE                                    \ Else if heavy to the right Then
      dup acn>balance 0!                    \   Balanced.
      true
    THEN
  THEN
  rdrop
;


: act-shrunk       ( acn act -- = Rebalance after the tree is shrunk )
  >r
  dup bnn-parent@ true               \   Balance the tree
  BEGIN
    over nil<> AND                   \   While nodes and not balanced Do
  WHILE
    tuck bnn-left@ = IF              \   If walked left Then
      r@ act-shrunk-left              \     Grown left
    ELSE                             \   Else
      r@ act-shrunk-right             \     Grown right
    THEN
    >r dup bnn-parent@ r>            \   Move to the parent
  REPEAT
  2drop
  rdrop
;

   
( Tree words )

: act-clear        ( act -- = Delete all nodes in the tree )
  bct-(free)
;


: act-insert       ( x1 x2 act -- = Insert data x1 with key x2 in the tree )
  >r 
  ['] acn-new swap
  r@ bnt-insert
  
  IF                                   \ If inserted Then
    dup bnn-parent@ true               \   Balance the tree
    BEGIN
      over nil<> AND                   \   While nodes and not balanced Do
    WHILE
      tuck bnn-left@ = IF              \   If walked left Then
        r@ act-grown-left              \     Grown left
      ELSE                             \   Else
        r@ act-grown-right             \     Grown right
      THEN
      >r dup bnn-parent@ r>            \   Move to the parent
    REPEAT
    2drop
  ELSE
    bcn-cell!
  THEN
  rdrop
;


: act-delete       ( x1 act -- false | x2 true = Delete key x1 from the tree, return the data x2 if found )
  >r
  r@ bnt-search-node
  nil<>? IF
    r@ bnt>length 1-!             \ Update length if deleted
    >r r@ bcn-cell@ true r>       \ Save return info
      
    act-replace-node              \ Try to replace the node
    
    dup r@ act-shrunk             \ Rebalance, the tree is shrunk
    
    r@ bnt-delete-node            \ Delete the node
    
    acn-free
  ELSE
    false
  THEN
  rdrop
;

: act-get          ( x1 act -- false | x2 true = Get the data x2 related to key x1 from the tree )
  bnt-search-node
  nil<>? IF
    bcn-cell@
    true
  ELSE
    false
  THEN
;


: act-has?         ( x1 act -- flag = Check if the key x1 is present in the tree )
  bnt-has?
;


: act-execute      ( i*x xt act -- i*x = Execute xt for every key and data in the tree )
  bct-execute
;


: act-execute?     ( i*x xt bct -- j*x flag = Execute xt for every key and data in the tree until xt returns true )
  bct-execute?
;

( Inspection )

: act-dump         ( act -- = Dump the tree variable )
  bct-dump
;

[THEN]

\ ==============================================================================
