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
\  $Date: 2006-04-02 06:45:37 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/car.fs


.( Testing: car ) cr 

t{ 26 car-create c4 }t

t{ 40 car-new value c2 }t

t{  0 car-new value c3 }t

\ index check

t{  0  c4 car-index? ?true }t
t{ -26 c4 car-index? ?true }t
t{ -27 c4 car-index? ?false }t
t{  25 c4 car-index? ?true }t
t{  26 c4 car-index? ?false }t
t{  0  c3 car-index? ?false }t
t{ -1  c3 car-index? ?false }t
t{  1  c3 car-index? ?false }t

\ length

t{  c4 car-length@ 26 ?s }t
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


\ free

\ t{ c2 car-free }t
\ t{ c3 car-free }t

\ ==============================================================================
