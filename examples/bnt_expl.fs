\ ==============================================================================
\
\          bnt_expl - the binary tree example in the ffl
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
\  $Date: 2008-04-10 16:12:01 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/bnt.fs
include ffl/bni.fs
include ffl/str.fs


\ Example: store mountain positions in a binary tree


\ Create the generic binary tree in the dictionary

bnt-create mountains


\ Setup the compare word for comparing the mountain names

: mount-compare  ( str str - n = Compare the two mountain names )
  str^ccompare
;

' mount-compare mountains bnt-compare!


\ Extend the generic binary tree node with mountain positions fields

begin-structure mount%
  bnn%
  +field   mount>node        \ Mountain structure extends the bnn structure
  ffield:  mount>lng         \ Longitude position
  ffield:  mount>lat         \ Latitude position
end-structure


\ Create the allocation word for the extended structure

: mount-new    ( F: r1 r2 -- ; x bnn1 -- bnn2 = Create a new mountain position variable with position r1 r2, name c-addr u and parent bnn1 )
  mount% allocate throw             \ Allocate the mountain variable
  >r
  r@ mount>node bnn-init            \ Initialise the binary tree node
  r@ mount>lng f!                   \ Save the longitude position
  r@ mount>lat f!                   \ Save the latitude position
  r>
;

 
  
\ Add the mountain positions to the binary tree; the key is the mountain name in a (unique) dynamic string

27.98E0 86.92E0  ' mount-new  str-new dup s" mount everest" rot str-set  mountains bnt-insert [IF]
  .( Mountain:) bnn-key@ str-get type .(  added to the tree.) cr
[ELSE]
  .( Mountain was not unique in tree) cr drop fdrop fdrop 
[THEN]

45.92E0  6.92E0  ' mount-new  str-new dup s" mont blanc" rot str-set   mountains bnt-insert [IF]
  .( Mountain:) bnn-key@ str-get type .(  added to the tree.) cr
[ELSE]
  .( Mountain was not unique in tree) cr drop fdrop fdrop
[THEN]

43.35E0 42.43E0 ' mount-new   str-new dup s" mount elbrus" rot str-set  mountains bnt-insert [IF]
  .( Mountain:) bnn-key@ str-get type .(  added to the tree.) cr
[ELSE]
  .( Mountain was not unique in tree) cr drop fdrop fdrop
[THEN]


\ Find a mountain in the binary tree

str-new value mount-name

s" mont blanc" mount-name str-set

mount-name mountains bnt-get [IF]
  .( Mount:)      dup bnn-key@ str-get type 
  .(  latitude:)  dup mount>lat f@ f. 
  .(  longitude:)     mount>lng f@ f. cr
[ELSE]
  .( Mount:) mount-name str-get type .(  not in tree.) cr
[THEN]


s" vaalserberg" mount-name str-set

mount-name mountains bnt-get [IF]
  .( Mount:)      dup bnn-key@ str-get type 
  .(  latitude:)  dup mount>lat f@ f. 
  .(  longitude:)     mount>lng f@ f. cr
[ELSE]
  .( Mount:) mount-name str-get type .(  not in tree.) cr
[THEN] 


\ Word for printing the mountain positions

: mount-emit ( mount -- = Print mountain )
  dup bnn-key@ str-get type ."  --> "
  dup mount>lat f@ f. ." - "
      mount>lng f@ f. cr
;


\ Print all mountain positions

' mount-emit mountains bnt-execute       \ Execute the word mount-emit for all entries in the tree


\ Example mountains iterator

\ Create the tree iterator in the dictionary

mountains bni-create mount-iter          \ Create an iterator named mount-iter on the mountains tree

\ Moving the iterator

mount-iter bni-first nil<>? [IF]
  .( First mount:) dup bnn-key@ str-get type 
  .(  latitude:)   dup mount>lat f@ f. 
  .(  longitude:)      mount>lng f@ f. cr
[ELSE]
  .( No first mountain.) cr
[THEN]

mount-iter bni-last nil<>? [IF]
  .( Last mount:) dup bnn-key@ str-get type 
  .(  latitude:)  dup mount>lat f@ f. 
  .(  longitude:)     mount>lng f@ f. cr
[ELSE]
  .( No last mountain.) cr
[THEN]


\ Word for freeing the tree node 

: mount-free     ( mount -- = Free the node in the tree )
  dup bnn-key@ str-free
  
  free throw
;

\ Cleanup the tree

' mount-free mountains bnt-clear

