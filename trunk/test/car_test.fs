\ ==============================================================================
\
\          car_test - the test words for the car module in ffl
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
\  $Date: 2006-04-05 17:10:27 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/car.fs


.( Testing: car ) cr 

t{ 10 car-create c4 }t

t{ 40 car-new value c2 }t

t{  0 car-new value c3 }t

\ index check

t{  0  c4 car-index? ?true }t
t{ -10 c4 car-index? ?true }t
t{ -11 c4 car-index? ?false }t
t{  9  c4 car-index? ?true }t
t{  10 c4 car-index? ?false }t
t{  0  c3 car-index? ?false }t
t{ -1  c3 car-index? ?false }t
t{  1  c3 car-index? ?false }t

\ length

t{  c4 car-length@ 10 ?s }t
t{  c2 car-length@ 40 ?s }t
t{  c3 car-length@ ?0 }t

\ extra

t{ car+extra@ 8 ?s }t
t{ 16 car+extra! }t
t{ car+extra@ 16 ?s }t

t{    c2 car-extra@ 8 ?s }t
t{ 24 c2 car-extra! }t
t{    c2 car-extra@ 24 ?s }t

\ array words

t{ 0 c2 car-get ?0 }t
t{ 1 c2 car-get ?0 }t
t{ 2 c2 car-get ?0 }t

t{ 15 0 c2 car-set }t
t{ 20 1 c2 car-set }t
t{ 25 2 c2 car-set }t

t{ 0 c2 car-get 15 ?s }t
t{ 1 c2 car-get 20 ?s }t
t{ 2 c2 car-get 25 ?s }t

t{ 10 c2 car-prepend }t
t{ 1  c2 car-get 15 ?s }t
t{ 0  c2 car-get 10 ?s }t

t{ 90 c2 car-append }t
t{ -1 c2 car-get 90 ?s }t

t{ 17 2 c2 car-insert }t
t{ 1 c2 car-get 15 ?s }t
t{ 2 c2 car-get 17 ?s }t
t{ 3 c2 car-get 20 ?s }t

t{ 5 0 c2 car-insert }t
t{ 100 -1 c2 car-insert }t

t{ 0  c2 car-get 5 ?s }t
t{ -2 c2 car-get 100 ?s }t

t{ c2 car-length@ 45 ?s }t

t{  0 c2 car-delete 5 ?s }t
t{ -1 c2 car-delete 90 ?s }t
t{  4 c2 car-delete 25 ?s }t

t{ c2 car-length@ 42 ?s }t

t{ c2 car-clear }t

t{  0 c2 car-get ?0 }t
t{  1 c2 car-get ?0 }t
t{  2 c2 car-get ?0 }t
t{ -1 c2 car-get ?0 }t

t{ 4 c3 car-append }t
t{ 0 c3 car-delete 4 ?s }t

\ lifo / fifo

t{ 1 c3 car-push }t
t{ 2 c3 car-push }t
t{ 3 c3 car-push }t

t{ c3 car-tos 3 ?s }t

t{ c3 car-length@ 3 ?s }t

t{ c3 car-pop 3 ?s }t
t{ c3 car-pop 2 ?s }t
t{ c3 car-pop 1 ?s }t

t{ 1 c3 car-enqueue }t
t{ 2 c3 car-enqueue }t
t{ 3 c3 car-enqueue }t

t{ c3 car-tos 1 ?s }t

t{ c3 car-length@ 3 ?s }t

t{ c3 car-dequeue 1 ?s }t
t{ c3 car-dequeue 2 ?s }t
t{ c3 car-dequeue 3 ?s }t

\ Specials

t{ c4 car-clear }t
t{ 2 0 c4 car-set }t
t{ 4 2 c4 car-set }t
t{ 2 4 c4 car-set }t
t{ 5 6 c4 car-set }t
t{ 2 8 c4 car-set }t

t{ 3 c4 car-has? ?false }t
t{ 5 c4 car-has? ?true  }t

t{ 2 c4 car-count 3 ?s }t
t{ 4 c4 car-count 1 ?s }t
t{ 3 c4 car-count ?0 }t

t{ 4 c4 car-find 2 ?s }t
t{ 7 c4 car-find -1 ?s }t

: car-test-add ( n n - n )
  +
;

t{ 0 ' car-test-add c4 car-execute 15 ?s }t


: car-test-add2 ( n n - n flag )
  dup 5 = IF + true ELSE + false THEN
;

t{ 0 ' car-test-add2 c4 car-execute? ?true 13 ?s }t

\ Sorting 

t{ c4 car-clear }t
t{ 50 0 c4 car-set }t
t{ 30 1 c4 car-set }t
t{ 10 2 c4 car-set }t
t{ 90 3 c4 car-set }t
t{ 70 4 c4 car-set }t
t{ 80 5 c4 car-set }t
t{ 40 6 c4 car-set }t
t{ 60 7 c4 car-set }t
t{ 20 8 c4 car-set }t
t{ 00 9 c4 car-set }t

t{ c4 car-sort }t

t{ 0 c4 car-get 00 ?s }t
t{ 1 c4 car-get 10 ?s }t
t{ 2 c4 car-get 20 ?s }t
t{ 3 c4 car-get 30 ?s }t
t{ 4 c4 car-get 40 ?s }t
t{ 5 c4 car-get 50 ?s }t
t{ 6 c4 car-get 60 ?s }t
t{ 7 c4 car-get 70 ?s }t
t{ 8 c4 car-get 80 ?s }t
t{ 9 c4 car-get 90 ?s }t

t{  10 c4 car-find-sorted ?true   1 ?s }t
t{  15 c4 car-find-sorted ?false  2 ?s }t
t{ -10 c4 car-find-sorted ?false  0 ?s }t
t{  50 c4 car-find-sorted ?true   5 ?s }t
t{  95 c4 car-find-sorted ?false 10 ?s }t

t{  60 c4 car-has-sorted? ?true }t
t{  91 c4 car-has-sorted? ?false }t

: car-test-compare ( n n - n = Compare word for reverse sorting )
  - negate
;

t{ ' car-test-compare c4 car-compare! }t

t{ c4 car-sort }t

t{ 0 c4 car-get 90 ?s }t
t{ 1 c4 car-get 80 ?s }t
t{ 2 c4 car-get 70 ?s }t
t{ 3 c4 car-get 60 ?s }t
t{ 4 c4 car-get 50 ?s }t
t{ 5 c4 car-get 40 ?s }t
t{ 6 c4 car-get 30 ?s }t
t{ 7 c4 car-get 20 ?s }t
t{ 8 c4 car-get 10 ?s }t
t{ 9 c4 car-get 00 ?s }t

t{  10 c4 car-find-sorted ?true   8 ?s }t
t{  15 c4 car-find-sorted ?false  8 ?s }t
t{ -10 c4 car-find-sorted ?false 10 ?s }t
t{  50 c4 car-find-sorted ?true   4 ?s }t
t{  95 c4 car-find-sorted ?false  0 ?s }t

t{  95 c4 car-insert-sorted }t
t{  70 c4 car-insert-sorted }t
t{  75 c4 car-insert-sorted }t
t{  00 c4 car-insert-sorted }t
t{  -5 c4 car-insert-sorted }t

t{ c4 car-length@ 15 ?s }t

t{  0 c4 car-get 95 ?s }t
t{  1 c4 car-get 90 ?s }t
t{  2 c4 car-get 80 ?s }t
t{  3 c4 car-get 75 ?s }t
t{  4 c4 car-get 70 ?s }t
t{  5 c4 car-get 70 ?s }t
t{  6 c4 car-get 60 ?s }t
t{  7 c4 car-get 50 ?s }t
t{  8 c4 car-get 40 ?s }t
t{  9 c4 car-get 30 ?s }t
t{ 10 c4 car-get 20 ?s }t
t{ 11 c4 car-get 10 ?s }t
t{ 12 c4 car-get 00 ?s }t
t{ 13 c4 car-get 00 ?s }t
t{ 14 c4 car-get -5 ?s }t

\ free

t{ c2 car-free }t
t{ c3 car-free }t

\ ==============================================================================
