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
\  $Date: 2008-05-29 05:31:05 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/lbf.fs


.( Testing: lbf ) cr 

t{ 1 chars 50 lbf-create lbf1 }t

\ Access words check

t{ lbf1 lbf-access@ ?nil ?nil }t

t{ ' ! ' @ lbf1 lbf-access! }t

\ Fifo check

t{ lbf1 lbf-length@ ?0 }t

t{ char a lbf1 lbf-enqueue }t

t{ lbf1 lbf-length@ 1 ?s }t

t{ lbf1 lbf-dequeue ?true char a ?s }t

t{ lbf1 lbf-length@ ?0 }t

t{ lbf1 lbf-dequeue ?false }t


\ ==============================================================================
