\ ==============================================================================
\
\        bct_test - the test words for the bct module in the ffl
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

include ffl/tst.fs
include ffl/bct.fs
include ffl/bci.fs

include ffl/car.fs
include ffl/rng.fs

.( Testing: bct, bcn and bci) cr 
  

\ Simple test

t{ bct-create bct1  }t

t{ bct1 bct-length@  ?0      }t
t{ bct1 bct-empty?   ?true   }t

t{  1  1 bct1 bct-insert }t
t{  8  8 bct1 bct-insert }t
t{  5  5 bct1 bct-insert }t
t{  6  6 bct1 bct-insert }t
t{  7  7 bct1 bct-insert }t
t{  4  4 bct1 bct-insert }t
t{ -1 -1 bct1 bct-insert }t
t{  9  9 bct1 bct-insert }t
t{ 11 11 bct1 bct-insert }t

t{  5  4 bct1 bct-insert }t   \ actually data change of node 4

t{ bct1 bct-length@   9 ?s   }t
t{ bct1 bct-empty?   ?false  }t

t{ 1 bct1 bct-has?  ?true      }t
t{ 1 bct1 bct-get   ?true 1 ?s }t
t{ 0 bct1 bct-has?  ?false     }t
t{ 0 bct1 bct-get   ?false     }t
t{ 4 bct1 bct-get   ?true 5 ?s }t

: bct-sum ( n:sum w:data w:key - n:sum = Sum data and key )
  rot + +
;

: bct-count ( n:count w:data w:key - n:count = Count the nodes )
  2drop 1+
;


t{ 0 ' bct-sum bct1 bct-execute 101 ?s }t

t{  1 bct1 bct-delete ?true 1 ?s }t
t{  4 bct1 bct-delete ?true 5 ?s }t
t{  1 bct1 bct-delete ?false     }t
t{  9 bct1 bct-delete ?true 9 ?s }t
t{ 12 bct1 bct-delete ?false     }t

t{ bct1 bct-length@ 6 ?s }t

t{ 0 ' bct-sum bct1 bct-execute 72 ?s }t


\ Insert and delete a lot more nodes ..

5000 car-new value bct-car   \ Array with 10000 random numbers
5189 rng-new value bct-rng   \ Random generator

t{ bct-new value bct2 }t

: bct-compare  ( n n - r = Compare n and n )
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

: bct-repeat-insert ( - = Insert 10000 random numbers in an array and the tree )
  5000 0 DO
    bct-rng rng-next-number dup
    I bct-car car-set
    
    dup bct2 bct-insert
  LOOP
;

: bct-count-present ( - n:present = Test if all values from the random array are present in the tree)
  0
  5000 0 DO
    I bct-car car-get
    bct2 bct-has? IF
      1+
    THEN
  LOOP
;

: bct-repeat-delete ( - = Delete all values from the tree )
  5000 0 DO
    I bct-car car-get
    bct2 bct-delete IF
      drop
    ELSE
      ." Value not found:" I . I bct-car car-get . 
    THEN
  LOOP
;

t{ ' bct-compare bct2 bct-compare! }t

bct-repeat-insert

t{ bct2 bct-length@ 5000 ?s }t

t{ 0 ' bct-count bct2 bct-execute 5000 ?s }t

t{ bct-count-present 5000 ?s }t

t{ bct-repeat-delete }t

t{ bct2 bct-length@ ?0 }t

t{ 0 ' bct-count bct2 bct-execute ?0 }t

bct-repeat-insert

t{ bct2 bct-length@ 5000 ?s }t

t{ 0 ' bct-count bct2 bct-execute 5000 ?s }t

t{ bct2 bct-free }t

bct-rng rng-free
bct-car car-free

0 [IF]
\ Iterator test

t{ bct1 hci-new value hci1 }t

t{ hci1 hci-first ?true 2 ?s }t
t{ hci1 hci-get   ?true 2 ?s }t
t{ hci1 hci-key   s" two" compare ?0 }t

  t{ hci1 hci-first? ?true  }t
  t{ hci1 hci-last?  ?false }t

t{ hci1 hci-next  ?true 1 ?s }t

  t{ hci1 hci-first? ?false }t
  t{ hci1 hci-last?  ?false }t

t{ hci1 hci-next  ?true 3 ?s }t
  
  t{ hci1 hci-first? ?false }t
  t{ hci1 hci-last?  ?false }t

t{ hci1 hci-next  ?true 1 ?s }t

  t{ hci1 hci-first? ?false  }t
  t{ hci1 hci-last?  ?true }t

t{ hci1 hci-next  ?false  }t


t{ hci1 hci-first ?true drop }t

t{ 1 hci1 hci-move ?true  }t
t{ 1 hci1 hci-move ?true  }t
t{ 1 hci1 hci-move ?false }t

t{ hci1 hci-free }t

[THEN]

\ ==============================================================================

