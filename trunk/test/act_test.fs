\ ==============================================================================
\
\        act_test - the test words for the act module in the ffl
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
\  $Date: 2008-04-24 16:50:23 $ $Revision: 1.5 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/act.fs
include ffl/bci.fs

include ffl/car.fs
include ffl/rng.fs

.( Testing: act and acn) cr 
  

: act-verify-node  ( w:node - w:height = Verify the node by checking the balance )
  dup nil= IF
    drop 0
  ELSE
    >r
    r@ bnn-right@  recurse
    r@ bnn-left@   recurse
    2dup -
    dup r@ acn-balance@ <> IF
      ." Invalid balance for node:" r@ bnn>key ? cr
    THEN
    dup -1 < swap 1 > OR IF
      ." Unbalanced at node:" r@ bnn>key ? cr
    THEN
    rdrop
    max 1+
  THEN
;


: act-verify       ( w:act - = Verify the act tree )
  bnt>root @
  act-verify-node
  drop
;


\ Simple test

t{ act-create act1  }t

t{  1  1 act1 act-insert }t
t{  8  8 act1 act-insert }t
t{  5  5 act1 act-insert }t
t{  6  6 act1 act-insert }t
t{  7  7 act1 act-insert }t
t{  4  4 act1 act-insert }t
t{ -1 -1 act1 act-insert }t
t{  9  9 act1 act-insert }t
t{ 11 11 act1 act-insert }t

t{  5  4 act1 act-insert }t   \ actually data change of node 4

t{ act1 act-length@   9 ?s   }t
t{ act1 act-empty?   ?false  }t

act1 act-verify

t{ 1 act1 act-has?  ?true      }t
t{ 1 act1 act-get   ?true 1 ?s }t
t{ 0 act1 act-has?  ?false     }t
t{ 0 act1 act-get   ?false     }t
t{ 4 act1 act-get   ?true 5 ?s }t


: act-sum ( n:sum w:data w:key - n:sum = Sum data and key )
  rot + +
;

: act-count ( n:count w:data w:key - n:count = Count the nodes )
  2drop 1+
;


t{ 0 ' act-sum act1 act-execute 101 ?s }t


\ Delete test 

t{  1 act1 act-delete ?true 1 ?s }t
t{  4 act1 act-delete ?true 5 ?s }t
t{  1 act1 act-delete ?false     }t
t{  9 act1 act-delete ?true 9 ?s }t
t{ 12 act1 act-delete ?false     }t

t{ act1 act-length@ 6 ?s }t

t{ 0 ' act-sum act1 act-execute 72 ?s }t

act1 act-verify


\ Insert and delete a lot more nodes ..

5000 car-new value act-car   \ Array with 5000 random numbers
5189 rng-new value act-rng   \ Random generator

t{ act-new value act2 }t

: act-repeat-insert ( - = Insert 5000 random numbers in an array and the tree )
  5000 0 DO
    act-rng rng-next-number dup
    I act-car car-set
    
    dup act2 act-insert
  LOOP
;

: act-count-present ( - n:present = Test if all values from the random array are present in the tree)
  0
  5000 0 DO
    I act-car car-get
    act2 act-has? IF
      1+
    THEN
  LOOP
;

: act-repeat-delete ( - = Delete half values from the tree )
  2500 0 DO
    I act-car car-get
    act2 act-delete IF
      drop
    ELSE
      ." Value not found:" I . I act-car car-get . 
    THEN
  LOOP
;


act-repeat-insert

t{ act2 act-length@ 5000 ?s }t

t{ 0 ' act-count act2 act-execute 5000 ?s }t

t{ act-count-present 5000 ?s }t

act2 act-verify


t{ act-repeat-delete }t

t{ act2 act-length@ 2500 ?s }t

t{ 0 ' act-count act2 act-execute 2500 ?s }t

act2 act-verify


act-repeat-insert

t{ act2 act-length@ 7500 ?s }t

t{ 0 ' act-count act2 act-execute 7500 ?s }t

act2 act-verify


t{ act2 act-free }t


act-rng rng-free
act-car car-free

[THEN]

\ ==============================================================================

