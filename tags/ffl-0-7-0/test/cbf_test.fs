\ ==============================================================================
\
\          cbf_test - the test words for the cbf module in ffl
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
\  $Date: 2008-07-03 17:21:49 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/cbf.fs


.( Testing: cbf ) cr 

t{ 1 chars 10 cbf-create cbf1 }t

\ Access words check

t{ cbf1 cbf-access@ ?nil ?nil }t

t{ ' c! ' c@ cbf1 cbf-access! }t

\ Lifo check

t{ cbf1 cbf-length@ ?0 }t

t{ char a cbf1 cbf-enqueue }t

t{ cbf1 cbf-length@ 1 ?s }t

t{ cbf1 cbf-dequeue ?true char a ?s }t

t{ cbf1 cbf-length@ ?0 }t

t{ cbf1 cbf-dequeue ?false }t

t{ s" Hello" cbf1 cbf-set }t

t{ pad 3 cbf1 cbf-get pad swap s" Hel" compare ?0 }t

t{ pad 3 cbf1 cbf-get pad swap s" lo" compare ?0 }t

t{ pad 5 cbf1 cbf-get ?0 }t

t{ s" Morning" cbf1 cbf-set }t

t{ cbf1 cbf-length@ 7 ?s }t

t{ pad 4 cbf1 cbf-fetch pad swap s" Morn" compare ?0 }t

t{ s" HaveANiceDay" cbf1 cbf-set }t

t{ cbf1 cbf-length@ 19 ?s }t

t{ pad 4 cbf1 cbf-fetch pad swap s" Morn" compare ?0 }t

t{ pad 15 cbf1 cbf-fetch pad swap s" MorningHaveANic" compare ?0 }t

t{ pad 15 cbf1 cbf-get pad swap s" MorningHaveANic" compare ?0 }t

t{ cbf1 cbf-length@ 4 ?s }t

t{ cbf1 cbf-clear }t

t{ cbf1 cbf-length@ ?0 }t

\ Fifo check

t{ cbf1 cbf-tos ?false }t

t{ char b cbf1 cbf-push }t

t{ char c cbf1 cbf-push }t

t{ cbf1 cbf-length@ 2 ?s }t

t{ cbf1 cbf-tos ?true char c ?s }t

t{ cbf1 cbf-pop ?true char c ?s }t

t{ cbf1 cbf-pop ?true char b ?s }t

t{ cbf1 cbf-pop ?false }t

: cbf-test1  ( -- = Repeat test )
  [char] a cbf1 cbf-enqueue
  [char] b cbf1 cbf-enqueue
  [char] c cbf1 cbf-enqueue
  33333 0 DO
    cbf1 cbf-tos    2drop
    cbf1 cbf-pop     drop
    cbf1 cbf-push
    cbf1 cbf-dequeue drop
    cbf1 cbf-enqueue
  LOOP
;

t{ cbf-test1 }t

t{ cbf1 cbf-length@ 3 ?s }t

t{ cbf1 cbf-pop ?true char c ?s }t
t{ cbf1 cbf-pop ?true char b ?s }t
t{ cbf1 cbf-pop ?true char a ?s }t

t{ cbf1 cbf-(free) }t


\ Address check

t{ 2 cells 2 cbf-new value cbf2 }t

2variable cbf.var

t{ cbf2 cbf-extra@ cbf+extra@ ?s }t

t{ 5 cbf2 cbf-extra! }t

t{ cbf2 cbf-extra@ 5 ?s }t


10 20 cbf.var 2!

t{ cbf.var cbf2 cbf-enqueue }t

30 40 cbf.var 2!

t{ cbf.var cbf2 cbf-enqueue }t

50 60 cbf.var 2!

t{ cbf.var cbf2 cbf-enqueue }t

t{ cbf2 cbf-length@ 3 ?s }t

t{ cbf2 cbf-dequeue dup ?true [IF] dup @ 20 ?s cell+ @ 10 ?s [THEN] }t

t{ 1 cbf2 cbf-skip 1 ?s }t

t{ cbf2 cbf-dequeue dup ?true [IF] dup @ 60 ?s cell+ @ 50 ?s [THEN] }t

t{ cbf2 cbf-free }t

\ ==============================================================================
