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
\  $Date: 2006-10-07 06:09:27 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] bci.version [IF]


include ffl/stc.fs
include ffl/bct.fs
include ffl/bcn.fs


( bci = Binary cell tree iterator )
( The bci module implements an iterator on the binary tree: bct )


1 constant bci.version


( Public structure )

struct: bci%       ( - n = Get the required space for a bci data structure )
  cell: bci>tree        \ Refernce to the binary tree
  cell: bci>walk        \ Current node in the tree
;struct 


( Structure creation, initialisation and destruction )

: bci-init     ( w:bct w:bci - = Initialise the iterator with a binary tree )
  tuck bci>tree    !
       bci>walk nil!
;


: bci-create   ( C: w:bct "name" - R: - w:bci = Create a named iterator in the dictionary )
  create 
    here  bci% allot  bci-init
;


: bci-new      ( w:bct - w:bci = Create an iterator on the heap )
  bci% allocate  throw  tuck bci-init
;


: bci-free     ( w:bci - = Free the iterator from the heap )
  free throw
;


( Iterator words )

: bci-get      ( w:bci - false | w true = Get the cell data from the current node )
  bci>walk @
  dup nil<> IF
    bcn>cell @ true
  ELSE
    drop false
  THEN
;


: bci-key      ( w:bci - false | w true = Get the key from the current node )
  bci>walk @
  dup nil<> IF
    bcn>key @ true
  ELSE
    drop false
  THEN    
;

: bci-set      ( w w:bci - = Set the cell data for the current node )
  bci>walk @
  dup nil<> IF
    bcn>cell !
  ELSE
    exp-invalid-state throw
  THEN    
;


: bci-first    ( w:bci - w true | false = Move the iterator to the first node )
  >r
  r@ bci>tree @ bct>root @
  bct-smallest-node
  r@ bci>walk !  
  
  r> bci-get
;


: bci-next     ( w:bci - w true | false = Move the iterator to the next node )
  >r
  r@ bci>walk @              \ check if current node has a next node
  dup nil<> IF
    bct-next-node
    r@ bci>walk !
    r@ bci-get
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
;


: bci-move     ( w w:bci - f = Move the iterator to the next node with the cell data )
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


: bci-prev     ( w:bci - w true | false = Move the iterator to the previous node )
  >r
  r@ bci>walk @              \ check if current node has a next node
  dup nil<> IF
    bct-prev-node
    r@ bci>walk !
    r@ bci-get
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
;


: bci-last     ( w:bci - w true | false = Move the iterator to the last node )
  >r
  r@ bci>tree @ bct>root @
  bct-greatest-node
  r@ bci>walk !  
  
  r> bci-get
;


: bci-first?   ( w:bci - f = Check if the iterator is on the first node )
  >r
  bci>walk @
  dup nil<> IF
    r@ bci>tree @ bct>root @ 
    bct-smallest-node 
    =
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
;


: bci-last?    ( w:bci - f = Check if the iterator is on the last node )
  >r
  bci>walk @
  dup nil<> IF
    r@ bci>tree @ bct>root @ 
    bct-greatest-node 
    =
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
;


: bci-dump     ( w:bci - = Dump the iterator )
  ." bci:" dup . cr
  ."  tree :" dup bci>tree ?  cr
  ."  walk :"     bci>walk ?  cr
;

[THEN]

\ ==============================================================================
