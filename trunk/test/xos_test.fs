\ ==============================================================================
\
\        xos_test - the test words for the xos module in the ffl
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
\  $Date: 2007-11-24 18:43:21 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/xos.fs
include ffl/tst.fs


.( Testing: xos) cr

t{ tos-create tos5 }t

t{ nil 0 nil 0 s" 1.0" tos5 xos-write-start-document }t
t{ s" this is comment" tos5 xos-write-comment }t
t{ s" yes" s" important" 1 s" mail" tos5 xos-write-start-tag }t
t{ 0 s" to" tos5 xos-write-start-tag }t
t{ s" Bill & Sara" tos5 xos-write-text }t
t{ s" to"   tos5 xos-write-end-tag }t
t{ s" mail" tos5 xos-write-end-tag }t

t{ tos5 str-get type cr }t

\ ==============================================================================
