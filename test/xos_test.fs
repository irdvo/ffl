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
\  $Date: 2007-12-02 07:54:12 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/xos.fs
include ffl/tst.fs


.( Testing: xos) cr

t{ tos-create tos5 }t

t{ s" yes" s" ISO-8559-1" s" 1.0"   tos5 xos-write-start-xml }t
t{ s" this is comment"              tos5 xos-write-comment }t
t{ s" on" s" mode" 1 s" pi"         tos5 xos-write-proc-instr }t
t{ s" yes" s" important" 1 s" mail" tos5 xos-write-start-tag }t
t{ 0 s" to"                         tos5 xos-write-start-tag }t
t{ s" Bill & Sara"                  tos5 xos-write-text }t
t{ s" to"                           tos5 xos-write-end-tag }t
t{ 0 s" from"                       tos5 xos-write-start-tag }t
t{ s" $#70;orth"                    tos5 xos-write-raw-text }t
t{ s" from"                         tos5 xos-write-end-tag }t
t{ 0 s" contents"                   tos5 xos-write-start-tag }t
t{ s" : 2dup dup dup ;"             tos5 xos-write-cdata }t
t{ s" contents"                     tos5 xos-write-end-tag }t
t{ 0 s" others"                     tos5 xos-write-empty-element }t
t{ s" mail"                         tos5 xos-write-end-tag }t

t{ tos5 str-get type cr }t
\ Todo: compare, lastig door " in string ..


\ test DTDs

t{ tos5 tos-rewrite }t

t{ s" order.dtd" s" order" tos5 xos-write-system-dtd }t
t{ s" <!ELEMENT mail (to+,from,contents,others+)>" s" mail" tos5 xos-write-internal-dtd }t
t{ s" something" s" -//W3C//DTD HTML 4.0 Transitional//EN" s" HTML" tos5 xos-write-public-dtd }t

t{ tos5 str-get type cr }t



 
\ ==============================================================================
