\ ==============================================================================
\
\              act_expl - the AVL tree example in the ffl
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
\  $Date: 2008-04-24 16:50:22 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/act.fs
include ffl/bci.fs
include ffl/str.fs
include ffl/enm.fs


\ Example1: store mountain height in a AVL tree with numerical keys

\ The mountain enumeration

begin-enumeration
  enum: MountEverest
  enum: MontBlanc
  enum: MountElbrus
  enum: Vaalserberg
end-enumeration


\ Create the AVL tree on the heap and store it in the heights variable

act-new value heights


\ Add the mountain heights in the tree; the key is the mountain enum value

8300 MountEverest heights act-insert
4819 MontBlanc    heights act-insert
5642 MountElbrus  heights act-insert


\ Find a mountain height in the tree

MontBlanc heights act-get [IF]
  .( Mount:mont blanc height:) . cr
[ELSE]
  .( Mount:mont blanc not in tree.) cr
[THEN]

Vaalserberg heights act-get [IF]
  .( Mount:vaalserber height:) . cr
[ELSE]
  .( Mount:vaalserberg not in tree.) cr
[THEN]


\ Free the heights tree from the heap

heights act-free



\ Example2: store mountain height in a AVL tree with string keys


\ Create the AVL tree in the dictionary

act-create mountains


\ Setup the compare word for comparing the mountain names

: mount-compare  ( str str - n = Compare the two mountain names )
  str^ccompare
;

' mount-compare mountains act-compare!


\ Add the mountain heights to the AVL tree; the key is the mountain name in a (unique) dynamic string

8300 str-new dup s" mount everest" rot str-set  mountains act-insert
4819 str-new dup s" mont blanc"    rot str-set  mountains act-insert
5642 str-new dup s" mount elbrus"  rot str-set  mountains act-insert


\ Find a mountain height in the AVL tree

str-new value mount-name

s" mont blanc" mount-name str-set

mount-name mountains act-get [IF]
  .( Mount:)        mount-name str-get type 
  .(  height:)      . cr 
[ELSE]
  .( Mount:) mount-name str-get type .(  not in tree.) cr
[THEN]


s" vaalserberg" mount-name str-set

mount-name mountains act-get [IF]
  .( Mount:)        mount-name str-get type 
  .(  height:)      . cr 
[ELSE]
  .( Mount:) mount-name str-get type .(  not in tree.) cr
[THEN] 


\ Word for printing the mountain heights

: mount-emit ( x x -- = Print mountain )
  str-get type ."  --> " . cr
;


\ Print all mountain heights

' mount-emit mountains act-execute       \ Execute the word mount-emit for all entries in the tree


\ Example binary tree iterator

\ Create the tree iterator in the dictionary (use bci also for AVL trees)

mountains bci-create mount-iter          \ Create an iterator named mount-iter on the mountains tree


\ Using the iterator

mount-iter bci-first [IF]
  .( First mount:) mount-iter bci-key drop str-get type 
  .(  height:)     . cr 
[ELSE]
  .( No first mountain.) cr
[THEN]

mount-iter bci-last [IF]
  .( Last mount:) mount-iter bci-key drop str-get type 
  .(  height:)    . cr
[ELSE]
  .( No last mountain.) cr
[THEN]


\ Cleanup the tree

mountains act-clear
