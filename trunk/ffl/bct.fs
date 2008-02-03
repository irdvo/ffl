\ ==============================================================================
\
\            bct - the binary cell tree module in the ffl
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
\  $Date: 2008-02-03 07:09:33 $ $Revision: 1.11 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] bct.version [IF]


include ffl/stc.fs
include ffl/bcn.fs


( bct = binary cell tree module )
( The bct module implements an unbalanced binary tree with the key and data )
( cell based. The implementation is non-recursive.                          )


1 constant bct.version


( Binary tree structure )

begin-structure bct%       ( -- n = Get the required space for a bct variable )
  field: bct>root            \ the root of the tree
  field: bct>length          \ the number of nodes in the tree
  field: bct>compare         \ the compare word for the key
end-structure



( Tree creation, initialisation and destruction )

: bct-init         ( bct -- = Initialise the tree )
  dup          bct>root   nil!
  dup          bct>length   0!
  ['] <=> swap bct>compare   !
;


: bct-create       ( "<spaces>name" -- ; -- bct = Create a named binary tree in the dictionary )
  create  here bct% allot  bct-init
;


: bct-new          ( -- bct = Create a new binary tree on the heap )
  bct% allocate  throw   dup bct-init
;


: bct-free         ( bct -- = Free the tree node from the heap )
  dup bct>root @
  BEGIN                           \ Free the nodes in the tree
    dup nil<>
  WHILE
    dup bcn-left@ nil<> IF
      dup  bcn-left@
      swap bcn>left nil!
    ELSE
      dup bcn-right@ nil<> IF
        dup  bcn-right@
        swap bcn>right nil!
      ELSE
        dup  bcn-parent@
        swap bcn-free
      THEN
    THEN
  REPEAT
  over bct>root !
  
  free throw                      \ Free the tree
;


( Member words )

: bct-length@      ( bct -- u = Get the number of elements in the tree )
  bct>length @
;


: bct-empty?       ( bct -- flag = Check for an empty tree )
  bct-length@ 0=
;


: bct-compare@     ( bct -- xt = Get the compare execution token for comparing keys )
  bct>compare @
;


: bct-compare!     ( xt bct -- = Set the compare execution token for comparing keys )
  bct>compare !
;


( Private words )

: bct-smallest-node  ( bcn1 -- bcn2 = Find the smallest node in the subtree, starting from node bcn1 )
  dup
  BEGIN
    dup nil<>
  WHILE
    nip
    dup bcn-left@
  REPEAT
  drop
;


: bct-next-node    ( bcn1 -- bcn2 = Find the next node in the tree )
  dup bcn-right@ nil<> IF         \ If right subtree is present then
    bcn-right@ bct-smallest-node  \   Go to the smallest node in this subtree
  ELSE                            \ Else
    BEGIN
      dup bcn-parent@             \   Go to a non visited parent
      dup nil<> IF
        tuck bcn-right@ <>
      ELSE
        nip true
      THEN
    UNTIL
  THEN
;


: bct-greatest-node  ( bcn1 -- bcn2 = Find the greatest node in the subtree, starting from node bcn1 )
  dup
  BEGIN
    dup nil<>
  WHILE
    nip
    dup bcn-right@
  REPEAT
  drop
;


: bct-prev-node    ( bcn1 -- bcn2 = Find the previous node in the tree )
  dup bcn-left@ nil<> IF          \ If left subtree is present then
    bcn-left@ bct-greatest-node   \   Go to the greatest node in this subtree
  ELSE                            \ Else
    BEGIN
      dup bcn-parent@             \   Go to a non visited parent
      dup nil<> IF
        tuck bcn-left@ <>
      ELSE
        nip true
      THEN
    UNTIL
  THEN
;


: bct-insert-node  ( xt x1 x2 bct -- bcn flag = Create a node with xt with key x2 and data x1 and insert it in the tree )
  >r
  r@ bct>root @ nil= IF           \ first element in tree
    rot nil swap 
    execute dup                   \ create the node
    r@ bct>root !
    true
  ELSE
    r@ bct>compare @
    r@ bct>root    @
    BEGIN
      bcn-compare-key
      ?dup 0= IF                  \ key already present, update the data
        nip nip                   \ key, compare token 
        tuck bcn>cell !
        nip false true            \ done, no insertion
      ELSE
        0< IF
          dup bcn-left@ nil= IF   \ no left node present -> insert
            >r
            drop                  \ compare token
            rot r@ swap
            execute dup           \ create the node
            r> bcn>left !
            true true             \ done, insertion
          ELSE
            bcn-left@ false       \ continu searching to the left
          THEN
        ELSE
          dup bcn-right@ nil= IF  \ no right node present -> insert
            >r
            drop                  \ compare token
            rot r@ swap
            execute dup           \ create the node
            r> bcn>right !
            true true             \ done, insertion
          ELSE
            bcn-right@ false      \ continu searching to the right
          THEN
        THEN
      THEN
    UNTIL
  THEN
  
  dup IF
    r@ bct>length 1+!
  THEN
  rdrop
;


: bct-search-node  ( x bct -- bcn | nil = Search a node with key x )
  dup  bct-compare@
  swap bct>root @
  BEGIN
    dup nil<> IF
      bcn-compare-key             \ Compare the key, if node is not nil
      ?dup 0<>
    ELSE
      false
    THEN
  WHILE
    0< IF                         \ Compare result negative, go the left ..
      bcn-left@
    ELSE
      bcn-right@                  \ .. else go the right
    THEN
  REPEAT
  nip nip
;


: bct-replace-node ( bcn1 -- bcn2 = Replace the node with another node )
  dup bcn-left@ nil<> over bcn-right@ nil<> AND IF
    dup bcn-right@ bct-smallest-node        \ Both branches not nil Then
    2dup bcn>key  @ swap bcn>key  !         \   Find the smallest in the right subtree
    tuck bcn>cell @ swap bcn>cell !         \   Copy the contents 
  THEN
;


: bct-delete-node  ( bcn bct -- = Delete the node from the tree )
  >r
  dup bcn-left@ nil= IF                     \ Find the child node
    dup bcn-right@
  ELSE
    dup bcn-left@
  THEN
  
  over dup bcn-parent@ dup nil= IF          \ If root node Then
    2drop
    dup r@ bct>root !                       \    Child is root node
    nil swap bcn-parent!
  ELSE                                      \ Else
    tuck bcn-left@ = IF                     \   If node is left child of parent Then
      2dup bcn>left !                       \     Set child for left branch
    ELSE                                    \   Else
      2dup bcn>right !                      \     Set child for right branch
    THEN
    swap bcn-parent!                        \   Set parent
  THEN
  bcn-free                                  \ Free the node
  rdrop
;


( Tree words )

: bct-insert       ( x1 x2 bct -- = Insert data x2 with key x1 in the tree )
  >r ['] bcn-new -rot r>
  bct-insert-node
  2drop                           \ no balancing: drop flag and node  
;


: bct-delete       ( x1 bct -- false | x2 true = Delete key x1 from the tree, return the cell data x2 )
  >r
  r@ bct-search-node
  nil<>? IF
    r@ bct>length 1-!             \ Update length if deleted
    >r r@ bcn>cell @ true r>      \ Setup return info
      
    bct-replace-node              \ Try to replace the node
    
    r@ bct-delete-node            \ Delete the node
  ELSE
    false
  THEN
  rdrop
;


: bct-get          ( x1 bct -- false | x2 true = Get the data x2 related to key x1 from the tree )
  bct-search-node
  nil<>? IF
    bcn>cell @
    true
  ELSE
    false
  THEN
;


: bct-has?         ( x1 bct -- flag = Check if the key x1 is present in the tree )
  bct-search-node
  nil<>
;


: bct-execute      ( i*x xt bct -- j*x = Execute xt for every key and data in the tree )
  bct>root @
  bct-smallest-node               \ find the smallest node
  BEGIN                           \ while not nil do
    dup nil<>
  WHILE
    2>r                           \   clear the stack
    2r@
    dup  bcn>cell @
    swap bcn>key  @
    rot  execute                  \   execute with key and data
    2r>
    
    bct-next-node                 \   find the next node
  REPEAT
  2drop  
;


( Private words )

: bct-emit-element ( x1 x2 -- = Emit the key x2 and cell data x1 )
  0 .r [char] = emit 0 .r [char] ; emit
;


( Inspection )

: bct-dump         ( bct -- = Dump the tree node structure )
  ." bct:" dup . cr
  ."   root   :" dup bct>root    ? cr
  ."   length :" dup bct>length  ? cr
  ."   compare:" dup bct>compare ? cr
  ."   nodes  :" ['] bct-emit-element swap bct-execute cr
;

[THEN]

\ ==============================================================================
