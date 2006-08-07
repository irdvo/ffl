\ ==============================================================================
\
\         chs_test - the test words for the chs module in the ffl
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
\  $Date: 2006-08-07 17:27:02 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/chs.fs
include ffl/tst.fs

.( Testing: chs) cr 
  
chs-create c5

t{ c5 chs-set-alpha }t
t{ char a c5 chs-char? ?true  }t
t{ char 0 c5 chs-char? ?false }t

t{ c5 chs-invert }t

t{ char a c5 chs-char? ?false }t
t{ char 0 c5 chs-char? ?true  }t

\ ==============================================================================

