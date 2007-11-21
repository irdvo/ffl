\ ==============================================================================
\
\        msc_test - the test words for the msc module in the ffl
\
\               Copyright (C) 2007  Dick van Oudheusden
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
\  $Date: 2007-11-21 18:29:11 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/dos.fs
include ffl/tst.fs


.( Testing: dos) cr

t{ dtm-create dtm4 }t

t{ tos-create tos4 }t

t{ 0 45 15 18 21 dtm.november 2007 dtm4 dtm-set }t

\ t{ dtm4 s" %A" tos4 dos-write-format }t

t{ dtm4 tos4 dos-write-date-time }t

t{ tos4 str-get type cr }t

t{ tos4 tos-rewrite }t

t{ dtm4 tos4 dos-write-ampm-time }t

t{ tos4 str-get type cr }t


\ ==============================================================================
