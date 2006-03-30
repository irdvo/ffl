\ ==============================================================================
\
\          bar_test - the test words for the bar module in ffl
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
\  $Date: 2006-03-30 17:25:40 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/bar.fs


.( Testing: bar ) cr 

t{ 9 bar-create b1 }t

t{ 7 bar-new value b2 }t

\ index check

t{  0 b2 bar-index? ?true  }t
t{  4 b2 bar-index? ?true  }t
t{  6 b2 bar-index? ?true  }t
t{  7 b2 bar-index? ?false }t
t{ -1 b2 bar-index? ?true  }t
t{ -7 b2 bar-index? ?true  }t
t{ -8 b2 bar-index? ?false }t

\ length

t{ b1 bar-length@ 9 ?s }t
t{ b2 bar-length@ 7 ?s }t

\ set, reset bit array

t{ b1 bar-set   b1 bar-count 9 ?s }t
t{ b1 bar-reset b1 bar-count 0 ?s }t

\ bit set,get

t{ b2 bar-count ?0 }t
t{  0 b2 bar-set-bit }t
t{  0 b2 bar-get-bit ?true  }t
t{  1 b2 bar-get-bit ?false }t
t{ -1 b2 bar-get-bit ?false }t
t{ -7 b2 bar-get-bit ?true  }t
t{ b2 bar-count 1 ?s }t

t{  2 b2 bar-set-bit }t
t{  4 b2 bar-set-bit }t
t{ b2 bar-count 3 ?s }t

t{  1 b2 bar-set-bit }t
t{  3 b2 bar-set-bit }t
t{ b2 bar-count 5 ?s }t

t{  6 b2 bar-set-bit }t
t{ -1 b2 bar-get-bit ?true }t
t{ -2 b2 bar-get-bit ?false }t

\ bit reset,get

t{ b2 bar-set }t
t{ 6  b2 bar-reset-bit }t
t{ 6  b2 bar-get-bit ?false }t
t{ 0  b2 bar-get-bit ?true  }t
t{ 5  b2 bar-get-bit ?true  }t
t{ b2 bar-count 6 ?s }t
t{ 6  b2 bar-reset-bit }t
t{ b2 bar-count 6 ?s }t
t{ 0  b2 bar-reset-bit }t
t{ -2 b2 bar-reset-bit }t
t{ b2 bar-count 4 ?s }t

\ bit invert,get

t{ b2 bar-set }t
t{  4 b2 bar-invert-bit }t
t{  4 b2 bar-get-bit ?false }t
t{  3 b2 bar-get-bit ?true  }t
t{  5 b2 bar-get-bit ?true  }t
t{ b2 bar-count 6 ?s }t
t{  4 b2 bar-invert-bit }t
t{  4 b2 bar-get-bit ?true  }t
t{  3 b2 bar-get-bit ?true  }t
t{  5 b2 bar-get-bit ?true  }t
t{ b2 bar-count 7 ?s }t

\ bit ranges
t{ b2 bar-reset }t
t{ 5  1 b2 bar-set-bits }t
t{ b2 bar-count 5 ?s }t
t{ 0  1 b2 bar-reset-bits }t
t{ b2 bar-count 5 ?s }t
t{ 3 -5 b2 bar-reset-bits }t
t{ b2 bar-count 2 ?s }t
t{ 0 -5 b2 bar-set-bits }t
t{ b2 bar-count 2 ?s }t
t{ 7  0 b2 bar-invert-bits }t
t{ b2 bar-count 5 ?s }t
t{ 0  0 b2 bar-invert-bits }t
t{ b2 bar-count 5 ?s }t
t{ 0 b2 bar-get-bit ?true  }t
t{ 1 b2 bar-get-bit ?false }t
t{ 2 b2 bar-get-bit ?true  }t
t{ 3 b2 bar-get-bit ?true  }t
t{ 4 b2 bar-get-bit ?true  }t
t{ 5 b2 bar-get-bit ?false }t
t{ 6 b2 bar-get-bit ?true  }t
t{ 4 0 b2 bar-count-bits 3 ?s }t


\ execute

: bar-test ( n f - n )
  2 AND 1+ +
;

t{ 0 ' bar-test b2 bar-execute 17 ?s }t

t{ b2 bar-free }t

\ ==============================================================================
