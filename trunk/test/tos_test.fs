\ ==============================================================================
\
\          tos_test - the test words for the tos module in ffl
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
\  $Date: 2006-01-31 20:26:35 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/tos.fs


.( Testing: tos ) cr 

t{ tos-create t3 }t

t{ t3 tos-rewrite }t
t{ s" Hello" t3 tos-write-string }t
t{ char *  5 3 t3 tos-align }t
t{ t3 str-get s" ***Hello*****" compare ?0 }t

t{ t3 tos-rewrite }t
t{ char # 4 t3 tos-write-chars }t
t{ char + 0 3 t3 tos-align }t
t{ t3 str-get s" +++####" compare ?0 }t

t{ s" --" t3 tos-write-string }t
t{ chr.sp 1 1 t3 tos-align }t
t{ t3 str-get s" +++#### -- " compare ?0 }t

t{ t3 tos-rewrite }t
t{ s" Hello" t3 tos-write-string }t
t{ chr.sp 8 t3 tos-align-left }t
t{ t3 str-get s" Hello   " compare ?0 }t

t{ t3 tos-rewrite }t
t{ s" Hello" t3 tos-write-string }t
t{ chr.sp 8 t3 tos-align-right }t
t{ t3 str-get s"    Hello" compare ?0 }t

t{ t3 tos-rewrite }t
t{ s" Hello" t3 tos-write-string }t
t{ chr.sp 8 t3 tos-center }t
t{ t3 str-get s"   Hello " compare ?0 }t

\ ==============================================================================
