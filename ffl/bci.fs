\ ==============================================================================
\
\             bci - the binary cell tree iterator in the ffl
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
\  $Date: 2008-02-03 07:09:33 $ $Revision: 1.6 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] bci.version [IF]


include ffl/stc.fs
include ffl/bct.fs
include ffl/bcn.fs


( bci = Binary cell tree iterator )
( The bci module implements an iterator on the binary tree [bct]. )


1 constant bci.version


( Iterator Structure )

begin-structure bci%       ( -- n = Get the required space for a bci variable )
  field: bci>tree       \ Refernce to the binary tree
  field: bci>walk       \ Current node in the tree
end-structure


( Iterator creation, initialisation and destruction )

: bci-init     ( bct bci -- = Initialise the iterator with a binary tree )
  tuck bci>tree    !
       bci>walk nil!
;


: bci-create   ( bct "<spaces>name" -- ; -- bci = Create a named iterator in the dictionary with a binary tree )
  create 
    here  bci% allot  bci-init
;


: bci-new      ( bct -- bci = Create an iterator on the heap with a binary tree )
  bci% allocate  throw  tuck bci-init
;


: bci-free     ( bci -- = Free the iterator from the heap )
  free throw
;


( Iterator words )

: bci-get      ( bci -- false | x true = Get the cell data x from the current node )
  bci>walk @
  nil<>? IF
    bcn>cell @ true
  ELSE
    false
  THEN
;


: bci-key      ( bci -- false | x true = Get the key x from the current node )
  bci>walk @
  nil<>? IF
    bcn>key @ true
  ELSE
    false
  THEN    
;

: bci-set      ( x bci -- = Set the cell data x for the current node )
  bci>walk @
  nil<>? IF
    bcn>cell !
  ELSE
    exp-invalid-state throw
  THEN    
;


: bci-first    ( bci -- x true | false = Move the iterator to the first node, return the cell data x )
  >r
  r@ bci>tree @ bct>root @
  bct-smallest-node
  r@ bci>walk !  
  
  r> bci-get
;


: bci-next     ( bci -- x true | false = Move the iterator to the next node, return the cell data x )
  >r
  r@ bci>walk @              \ check if current node has a next node
  nil<>? IF
    bct-next-node
    r@ bci>walk !
    r@ bci-get
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
;


: bci-move     ( x w:bci -- flag = Move the iterator to the next node with the cell data x )
  swap
  BEGIN
    over bci-next IF
      over = 
    ELSE
      true
    THEN
  UNTIL
  drop
  bci>walk @ nil<>
;


: bci-prev     ( bci -- x true | false = Move the iterator to the previous node, return the cell data x )
  >r
  r@ bci>walk @              \ check if current node has a next node
  nil<>? IF
    bct-prev-node
    r@ bci>walk !
    r@ bci-get
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
;


: bci-last     ( bci -- x true | false = Move the iterator to the last node, return the cell data x )
  >r
  r@ bci>tree @ bct>root @
  bct-greatest-node
  r@ bci>walk !  
  
  r> bci-get
;


: bci-first?   ( bci -- flag = Check if the iterator is on the first node )
  >r
  r@ bci>walk @
  nil<>? IF
    r@ bci>tree @ bct>root @ 
    bct-smallest-node 
    =
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
;


: bci-last?    ( bci -- flag = Check if the iterator is on the last node )
  >r
  r@ bci>walk @
  nil<>? IF
    r@ bci>tree @ bct>root @ 
    bct-greatest-node 
    =
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
;


( Inspection )

: bci-dump     ( bci -- = Dump the iterator variable )
  ." bci:" dup . cr
  ."  tree :" dup bci>tree ?  cr
  ."  walk :"     bci>walk ?  cr
;

[THEN]

\ ==============================================================================
