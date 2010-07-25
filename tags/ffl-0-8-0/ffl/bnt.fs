\ ==============================================================================
\
\               bnt - the generic binary tree in the ffl
\
\               Copyright (C) 2008  Dick van Oudheusden
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
\  $Date: 2008-04-24 16:50:23 $ $Revision: 1.3 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] bnt.version [IF]


include ffl/stc.fs
include ffl/bnn.fs


( bnt = Generic binary tree )
( The bnt module implements a generic unbalanced binary tree with the key   )
( cell based. The implementation is non-recursive. Pay special attention to )
( the bnt-insert word. This word inserts new nodes in the tree. The input   )
( parameters are the tree itself, the key, the node creation word and any   )
( optional parameters for the node creation word. The bnt-insert word will  )
( only call the node creation word if the key is unique in the tree. If so  )
( the creation word is called and the resulting node is stored in the tree  )
( and returned as output parameter with the true flag. If the key is not    )
( unique the node creation word is not called and the current node with the )
( key is returned with the false flag and the optional parameters. In that  )
( case the calling word can update this node with the optional parameters.  )
( The stack notation of the node creation word is: i*x x bnn1 -- bnn2.      )
( i*x are the optional parameters, x is the key, bnn1 is the parent node    )
( and bnn2 is the returned tree node. See also [bnn] and [bcn] for examples )
( of node creation words.                                                   )


1 constant bnt.version


( Generic binary tree structure )

begin-structure bnt%       ( -- n = Get the required space for a bnt variable )
  field: bnt>root            \ the root of the tree
  field: bnt>length          \ the number of nodes in the tree
  field: bnt>compare         \ the compare word for the key
end-structure



( Tree creation, initialisation and destruction )

: bnt-init         ( bnt -- = Initialise the tree )
  dup          bnt>root   nil!
  dup          bnt>length   0!
  ['] <=> swap bnt>compare   !
;


: bnt-(free)       ( xt bnt -- = Free the nodes from the heap using xt )
  swap >r
  dup bnt>root @
  BEGIN
    dup nil<>
  WHILE
    dup bnn-left@ nil<> IF        \ Walk left tree
      dup  bnn-left@
      swap bnn>left nil!
    ELSE
      dup bnn-right@ nil<> IF     \ Walk right tree
        dup  bnn-right@
        swap bnn>right nil!
      ELSE
        dup  bnn-parent@          \ Move up and ..
        swap r@ execute           \ .. free the node
      THEN
    THEN
  REPEAT
  rdrop
  drop
  dup bnt>root nil!               \ Root = nil and length = 0
      bnt>length 0!
;


: bnt-create       ( "<spaces>name" -- ; -- bnt = Create a named binary tree in the dictionary )
  create  here bnt% allot  bnt-init
;


: bnt-new          ( -- bnt = Create a new binary tree on the heap )
  bnt% allocate  throw   dup bnt-init
;


: bnt-free         ( bnt -- = Free the tree node from the heap )
  ['] bnn-free over bnt-(free)    \ Free the nodes 
  
  free throw                      \ Free the tree
;


( Member words )

: bnt-length@      ( bnt -- u = Get the number of elements in the tree )
  bnt>length @
;


: bnt-empty?       ( bnt -- flag = Check for an empty tree )
  bnt-length@ 0=
;


: bnt-compare@     ( bnt -- xt = Get the compare execution token for comparing keys )
  bnt>compare @
;


: bnt-compare!     ( xt bnt -- = Set the compare execution token for comparing keys )
  bnt>compare !
;


( Private words )

: bnt-smallest-node  ( bnn1 -- bnn2 = Find the smallest node in the subtree, starting from node bnn1 )
  dup
  BEGIN
    dup nil<>
  WHILE
    nip
    dup bnn-left@
  REPEAT
  drop
;


: bnt-next-node    ( bnn1 -- bnn2 = Find the next node in the tree )
  dup bnn-right@ nil<> IF         \ If right subtree is present then
    bnn-right@ bnt-smallest-node  \   Go to the smallest node in this subtree
  ELSE                            \ Else
    BEGIN
      dup bnn-parent@             \   Go to a non visited parent
      dup nil<> IF
        tuck bnn-right@ <>
      ELSE
        nip true
      THEN
    UNTIL
  THEN
;


: bnt-greatest-node  ( bnn1 -- bnn2 = Find the greatest node in the subtree, starting from node bnn1 )
  dup
  BEGIN
    dup nil<>
  WHILE
    nip
    dup bnn-right@
  REPEAT
  drop
;


: bnt-prev-node    ( bnn1 -- bnn2 = Find the previous node in the tree )
  dup bnn-left@ nil<> IF          \ If left subtree is present then
    bnn-left@ bnt-greatest-node   \   Go to the greatest node in this subtree
  ELSE                            \ Else
    BEGIN
      dup bnn-parent@             \   Go to a non visited parent
      dup nil<> IF
        tuck bnn-left@ <>
      ELSE
        nip true
      THEN
    UNTIL
  THEN
;


: bnt-search-node  ( x bnt -- bnn | nil = Search the node with key x )
  dup  bnt-compare@
  swap bnt>root @
  BEGIN
    dup nil<> IF
      bnn-compare-key             \ Compare the key, if node is not nil
      ?dup 0<>
    ELSE
      false
    THEN
  WHILE
    0< IF                         \ Compare result negative, go the left ..
      bnn-left@
    ELSE
      bnn-right@                  \ .. else go the right
    THEN
  REPEAT
  nip nip
;


: bnt-swap-smallest ( bnn1 -- bnn2 = Swap the node with the smallest node in the right subtree )
  dup bnn-right@ bnt-smallest-node          \ Find the smallest node in the right subtree
  
  dup bnn-right@ nil= IF               \ If small.right = nil Then (Note: small.left is always nil)
    dup  bnn-parent@                   \   Remove the node from its parent:
    2dup bnn-left@ = IF                \   If small.parent.left = small Then
      bnn>left  nil!                   \     small.parent.left = nil
    ELSE                               \   Else 
      bnn>right nil!                   \     small.parent.right = nil
    THEN
  ELSE                                 \ Else (swap the right node of small to the parent of small)
    dup  bnn-parent@
    over bnn-right@ bnn-parent!        \   small.right.parent = small.parent
    dup  bnn-right@
    over dup bnn-parent@               \   IF small.parent.left = small Then
    tuck bnn-left@ = IF
      bnn>left !                       \     small.parent.left = small.right
    ELSE                               \   Else
      bnn>right !                      \     small.parent.right = small.right
    THEN
  THEN
  
  over bnn-right@ over bnn>right !     \ small.right = node.right
  2dup swap bnn-right@ bnn-parent!     \ node.right.parent = small
    
  over bnn-left@  over bnn>left !      \ small.left  = node.left
  tuck swap bnn-left@ bnn-parent!      \ node.left.parent = small
;


: bnt-delete-node  ( bnn1 bnt -- bnn2 = Delete node bnn from the tree, return the deleted node )
  >r
  dup bnn-left@ nil= IF
    dup bnn-right@ nil= IF
      nil                       \ Both left and right nil -> child = nil
    ELSE
      dup bnn-right@            \ Only left nil -> child = right
    THEN
  ELSE
    dup bnn-right@ nil= IF
      dup bnn-left@             \ Only right nil -> child = left
    ELSE
      dup bnt-swap-smallest     \ Both not nil -> swap with smallest to the right
    THEN
  THEN  
    
  dup nil<> IF                  \ If child <> nil Then
    over bnn-parent@ 
    over bnn-parent!            \   child.parent = node.parent
  THEN
  over bnn-parent@ nil= IF      \ If node.parent = nil Then
    r@ bnt>root !               \   root = child
  ELSE                          \ Else
    over dup bnn-parent@        \   node.parent
    tuck bnn-left@ = IF         \   If node.parent.left = node Then
      bnn>left !                \     node.parent.left = child
    ELSE                        \   Else 
      bnn>right !               \     node.parent.right = child
    THEN
  THEN
     
  dup bnn>parent nil!
  dup bnn>left   nil!
  dup bnn>right  nil!
  rdrop
;


( Tree words )

: bnt-clear        ( xt bnt -- = Delete all nodes in the tree using word xt )
  bnt-(free)
;


: bnt-insert  ( i*x xt x bct -- bnn1 true | i*x bnn2 false = Insert a new unique node in the tree with key x, creation word xt and optional parameters )
  >r
  r@ bnt>root @ nil= IF           \ first element in tree
    nil rot execute
    dup r@ bnt>root !             \ Create the root node
    true
  ELSE
    r@ bnt>compare @
    r@ bnt>root    @
    BEGIN
      bnn-compare-key ?dup 0= IF  \ Key already present, return for update
        nip nip nip               \ Compare token, key, creation token 
        false true                \ Done, no insertion
      ELSE
        0< IF
          dup bnn-left@ nil= IF   \ No left node present -> insert
            >r
            drop                  \ Compare token
            r@ rot execute        \ Create the node
            dup r> bnn>left !
            true true             \ Done, insertion
          ELSE
            bnn-left@ false       \ continue searching to the left
          THEN
        ELSE
          dup bnn-right@ nil= IF  \ No right node present -> insert
            >r
            drop                  \ Compare token
            r@ rot execute        \ Create the node
            dup r> bnn>right !
            true true             \ Done, insertion
          ELSE
            bnn-right@ false      \ continue searching to the right
          THEN
        THEN
      THEN
    UNTIL
  THEN
  
  dup IF
    r@ bnt>length 1+!
  THEN
  rdrop
;


: bnt-delete       ( x bnt -- false | bnn true = Delete key x from the tree, return the deleted node )
  >r
  r@ bnt-search-node
  nil<>? IF
    r@ bnt>length 1-!             \ Update length if deleted

    r@ bnt-delete-node
    
    true
  ELSE
    false
  THEN
  rdrop
;


: bnt-get          ( x bnt -- false | bnn true = Get the node related to key x from the tree )
  bnt-search-node nil<>?
;


: bnt-has?         ( x1 bnt -- flag = Check if the key x1 is present in the tree )
  bnt-search-node nil<>
;


: bnt-execute      ( i*x xt bnt -- j*x = Execute xt for every node in the tree )
  bnt>root @
  bnt-smallest-node               \ Find the smallest node
  BEGIN                           \ While not all nodes done Do
    dup nil<>
  WHILE
    2>r                           \   Clear the stack
    2r@
    swap execute                  \   Execute with node
    2r>
    bnt-next-node                 \   Find the next node
  REPEAT
  2drop  
;


: bnt-execute?     ( i*x xt bnt -- j*x flag = Execute xt for every node in the tree until xt returns true )
  bnt>root @
  bnt-smallest-node          \ Find the smallest node
  false
  BEGIN                      \ While not all nodes done AND xt returned false Do
    over nil<> over 0= AND
  WHILE
    drop
    2>r                      \   Clear the stack
    2r@ swap execute         \   Execute with node
    2r>
    bnt-next-node            \   Find the next node
    rot
  REPEAT
  nip nip
;


( Inspection )

: bnt-dump         ( bnt -- = Dump the tree node structure )
  ." bnt:" dup . cr
  ."   root   :" dup bnt>root    ? cr
  ."   length :" dup bnt>length  ? cr
  ."   compare:" dup bnt>compare ? cr
  ."   nodes  :" cr ['] bnn-dump swap bnt-execute cr
;

[THEN]

\ ==============================================================================
