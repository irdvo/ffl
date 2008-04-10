\ ==============================================================================
\
\             bni - the generic binary tree iterator in the ffl
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
\  $Date: 2008-04-10 16:12:01 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] bni.version [IF]


include ffl/stc.fs
include ffl/bnt.fs


( bni = Generic binary tree iterator )
( The bni module implements an iterator on the generic binary tree [bnt]. )


1 constant bni.version


( Iterator Structure )

begin-structure bni%       ( -- n = Get the required space for a bni variable )
  field: bni>tree       \ Reference to the binary tree
  field: bni>walk       \ Current node in the tree
end-structure


( Iterator creation, initialisation and destruction )

: bni-init     ( bnt bni -- = Initialise the iterator with a binary tree )
  tuck bni>tree    !
       bni>walk nil!
;


: bni-create   ( bnt "<spaces>name" -- ; -- bni = Create a named iterator in the dictionary with a binary tree )
  create 
    here  bni% allot  bni-init
;


: bni-new      ( bnt -- bni = Create an iterator on the heap with a binary tree )
  bni% allocate  throw  tuck bni-init
;


: bni-free     ( bni -- = Free the iterator from the heap )
  free throw
;


( Iterator words )

: bni-get      ( bni -- bnn | nil = Get the current node from the iterator )
  bni>walk @
;


: bni-key      ( bni -- false | x true = Get the key x from the current node )
  bni-get
  nil<>? IF
    bnn-key@ true
  ELSE
    false
  THEN    
;


: bni-first    ( bni -- bnn | nil = Move the iterator to the first node, return this node )
  >r
  r@ bni>tree @ bnt>root @
  bnt-smallest-node
  r@ bni>walk !  
  r> bni-get
;


: bni-next     ( bni -- bnn | nil = Move the iterator to the next node, return this node )
  >r
  r@ bni-get            \ check if current node has a next node
  nil<>? IF
    bnt-next-node
    r@ bni>walk !
    r@ bni-get
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
;


: bni-prev     ( bni -- bnn | nil = Move the iterator to the previous node, return this node )
  >r
  r@ bni-get            \ check if current node has a next node
  nil<>? IF
    bnt-prev-node
    r@ bni>walk !
    r@ bni-get
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
;


: bni-last     ( bni -- bnn | nil = Move the iterator to the last node, return this node )
  >r
  r@ bni>tree @ bnt>root @
  bnt-greatest-node
  r@ bni>walk !  
  r> bni-get
;


: bni-first?   ( bni -- flag = Check if the iterator is on the first node )
  >r
  r@ bni-get
  nil<>? IF
    r@ bni>tree @ bnt>root @ 
    bnt-smallest-node 
    =
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
;


: bni-last?    ( bni -- flag = Check if the iterator is on the last node )
  >r
  r@ bni-get
  nil<>? IF
    r@ bni>tree @ bnt>root @ 
    bnt-greatest-node 
    =
  ELSE
    exp-invalid-state throw
  THEN
  rdrop
;


( Inspection )

: bni-dump     ( bni -- = Dump the iterator variable )
  ." bni:" dup . cr
  ."  tree :" dup bni>tree ?  cr
  ."  walk :"     bni>walk ?  cr
;

[THEN]

\ ==============================================================================
