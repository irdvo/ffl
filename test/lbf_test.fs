\ ==============================================================================
\
\          lbf_test - the test words for the lbf module in ffl
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
\  $Date: 2009-05-10 14:36:26 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/lbf.fs


.( Testing: lbf ) cr 

t{ 1 chars 10 lbf-create lbf1 }t

\ Access words check

t{ lbf1 lbf-access@ ?nil ?nil }t

t{ ' c! ' c@ lbf1 lbf-access! }t

\ Lifo check

t{ lbf1 lbf-length@ ?0 }t

t{ char a lbf1 lbf-enqueue }t

t{ lbf1 lbf-length@ 1 ?s }t

t{ lbf1 lbf-dequeue ?true char a ?s }t

t{ lbf1 lbf-length@ ?0 }t

t{ lbf1 lbf-dequeue ?false }t

t{ s" Hello" lbf1 lbf-set }t

t{ 2 lbf1 lbf-get' s" He" compare ?0 }t

t{ lbf1 lbf-length'@ 3 ?s }t
t{ lbf1 lbf-length@  5 ?s }t

t{ 3 lbf1 lbf-get s" Hel" compare ?0 }t

t{ lbf1 lbf-length'@ 2 ?s }t
t{ lbf1 lbf-length@  2 ?s }t

t{ 3 lbf1 lbf-get' s" lo" compare ?0 }t

t{ 3 lbf1 lbf-get s" lo" compare ?0 }t

t{ 5 lbf1 lbf-get ?0 }t

t{ s" GoodMorning" lbf1 lbf-set }t

t{ 4 lbf1 lbf-fetch s" Good" compare ?0 }t

t{ s" HaveANiceDay" lbf1 lbf-set }t

t{ 4 lbf1 lbf-fetch s" Good" compare ?0 }t

t{ 15 lbf1 lbf-fetch s" GoodMorningHave" compare ?0 }t

t{ 15 lbf1 lbf-get s" GoodMorningHave" compare ?0 }t

t{ lbf1 lbf-length@ 8 ?s }t

\ Copy check

t{ 4 7 lbf1 lbf-copy }t

t{ 8 4 lbf1 lbf-copy }t

t{ lbf1 lbf-length@ 20 ?s }t

t{ 20 lbf1 lbf-get s" ANiceDayNiceNiceNice" compare ?0 }t

t{ lbf1 lbf-clear }t

t{ lbf1 lbf-length@ ?0 }t

\ Fifo check

t{ lbf1 lbf-tos ?false }t

t{ char b lbf1 lbf-push }t

t{ char c lbf1 lbf-push }t

t{ lbf1 lbf-length@ 2 ?s }t

t{ lbf1 lbf-tos ?true char c ?s }t

t{ lbf1 lbf-pop ?true char c ?s }t

t{ lbf1 lbf-pop ?true char b ?s }t

t{ lbf1 lbf-pop ?false }t

t{ lbf1 lbf-(free) }t

\ Address check

t{ 2 cells 2 lbf-new value lbf2 }t

2variable lbf.var

t{ lbf2 lbf-extra@ lbf+extra@ ?s }t

t{ 5 lbf2 lbf-extra! }t

t{ lbf2 lbf-extra@ 5 ?s }t


10 20 lbf.var 2!

t{ lbf.var lbf2 lbf-enqueue }t

30 40 lbf.var 2!

t{ lbf.var lbf2 lbf-enqueue }t

50 60 lbf.var 2!

t{ lbf.var lbf2 lbf-enqueue }t

t{ lbf2 lbf-length@ 3 ?s }t

t{ lbf2 lbf-dequeue ?true dup @ 20 ?s cell+ @ 10 ?s }t

t{ 1 lbf2 lbf-skip 1 ?s }t

t{ 0 lbf2 lbf-reduce }t

t{ lbf2 lbf-dequeue ?true dup @ 60 ?s cell+ @ 50 ?s }t

t{ lbf2 lbf-free }t

\ ==============================================================================
