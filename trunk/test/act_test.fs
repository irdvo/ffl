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
\  $Date: 2006-10-23 17:38:52 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/act.fs
include ffl/bct.fs
include ffl/bci.fs

include ffl/car.fs
include ffl/rng.fs

.( Testing: act and acn) cr 
  

\ Simple test

t{ bct-create act1  }t

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

t{ act1 bct-length@   9 ?s   }t
t{ act1 bct-empty?   ?false  }t

t{ 1 act1 bct-has?  ?true      }t
t{ 1 act1 bct-get   ?true 1 ?s }t
t{ 0 act1 bct-has?  ?false     }t
t{ 0 act1 bct-get   ?false     }t
t{ 4 act1 bct-get   ?true 5 ?s }t

: act-sum ( n:sum w:data w:key - n:sum = Sum data and key )
  rot + +
;

: act-count ( n:count w:data w:key - n:count = Count the nodes )
  2drop 1+
;


t{ 0 ' act-sum act1 bct-execute 101 ?s }t


0 [IF]

\ Delete test 

t{  1 bct1 bct-delete ?true 1 ?s }t
t{  4 bct1 bct-delete ?true 4 ?s }t
t{  1 bct1 bct-delete ?false     }t
t{  9 bct1 bct-delete ?true 9 ?s }t
t{ 12 bct1 bct-delete ?false     }t

t{ bct1 bct-length@ 6 ?s }t

t{ 0 ' bct-sum bct1 bct-execute 72 ?s }t

[THEN]


\ Insert and delete a lot more nodes ..

5000 car-new value act-car   \ Array with 10000 random numbers
5189 rng-new value act-rng   \ Random generator

t{ bct-new value act2 }t

: act-compare  ( n n - r = Compare n and n )
  2dup < IF
    2drop -1
  ELSE
    > IF
      1
    ELSE
      0
    THEN
  THEN
;

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
    act2 bct-has? IF
      1+
    THEN
  LOOP
;

: act-repeat-delete ( - = Delete all values from the tree )
  5000 0 DO
    I act-car car-get
    act2 act-delete IF
      drop
    ELSE
      ." Value not found:" I . I act-car car-get . 
    THEN
  LOOP
;

t{ ' act-compare act2 bct-compare! }t

act-repeat-insert

t{ act2 bct-length@ 5000 ?s }t

t{ 0 ' act-count act2 bct-execute 5000 ?s }t

t{ act-count-present 5000 ?s }t

0 [IF]

t{ bct-repeat-delete }t

t{ bct2 bct-length@ ?0 }t

t{ 0 ' bct-count bct2 bct-execute ?0 }t

bct-repeat-insert

t{ bct2 bct-length@ 5000 ?s }t

t{ 0 ' bct-count bct2 bct-execute 5000 ?s }t
[THEN]

t{ act2 bct-free }t

act-rng rng-free
act-car car-free



[THEN]

\ ==============================================================================

