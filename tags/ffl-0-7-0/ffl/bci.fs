\ ==============================================================================
\
\             bci - the binary cell tree iterator in the ffl
\
\              Copyright (C) 2006-2008  Dick van Oudheusden
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
\  $Date: 2008-04-15 17:13:54 $ $Revision: 1.7 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] bci.version [IF]


include ffl/bni.fs
include ffl/bct.fs
include ffl/bcn.fs


( bci = Binary cell tree iterator )
( The bci module implements an iterator on the [bct] binary tree.   )


2 constant bci.version


( Iterator Structure )

bni% constant bci%  ( -- n = Get the required space for a bci variable )


( Iterator creation, initialisation and destruction )

: bci-init     ( bct bci -- = Initialise the iterator with a binary tree )
  bni-init
;


: bci-create   ( bct "<spaces>name" -- ; -- bci = Create a named iterator in the dictionary with a binary tree )
  create  here  bci% allot  bci-init
;


: bci-new      ( bct -- bci = Create an iterator on the heap with a binary tree )
  bci% allocate  throw  tuck bci-init
;


: bci-free     ( bci -- = Free the iterator from the heap )
  bni-free
;


( Private words )

: bci+get      ( bcn -- false | x true = Get the cell data x from the node )
  nil<>? IF
    bcn-cell@ true
  ELSE
    false
  THEN
;

  
( Iterator words )

: bci-get      ( bci -- false | x true = Get the cell data x from the current node )
  bni-get bci+get
;


: bci-key      ( bci -- false | x true = Get the key x from the current node )
  bni-key
;

: bci-set      ( x bci -- = Set the cell data x for the current node )
  bni-get
  nil<>? IF
    bcn-cell!
  ELSE
    exp-invalid-state throw
  THEN    
;


: bci-first    ( bci -- x true | false = Move the iterator to the first node, return the cell data x )
  bni-first bci+get
;


: bci-next     ( bci -- x true | false = Move the iterator to the next node, return the cell data x )
  bni-next bci+get
;


: bci-move     ( x bci -- flag = Move the iterator to the next node with the cell data x )
  swap
  BEGIN
    over bci-next IF
      over = 
    ELSE
      true
    THEN
  UNTIL
  drop
  bni>walk @ nil<>
;


: bci-prev     ( bci -- x true | false = Move the iterator to the previous node, return the cell data x )
  bni-prev bci+get
;


: bci-last     ( bci -- x true | false = Move the iterator to the last node, return the cell data x )
  bni-last bci+get
;


: bci-first?   ( bci -- flag = Check if the iterator is on the first node )
  bni-first?
;


: bci-last?    ( bci -- flag = Check if the iterator is on the last node )
  bni-last?
;


( Inspection )

: bci-dump     ( bci -- = Dump the iterator variable )
  bni-dump
;

[THEN]

\ ==============================================================================
