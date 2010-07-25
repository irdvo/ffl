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
\  $Date: 2008-03-18 19:09:48 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/tos.fs


.( Testing: tos ) cr 

t{ tos-create t3 }t

t{ t3 tos-rewrite }t
t{ s" Hello" t3 tos-write-string }t
t{ char *  5 3 t3 tos-align }t
t{ t3 str-get s" ***Hello*****" ?str }t

t{ t3 tos-rewrite }t
t{ char # 4 t3 tos-write-chars }t
t{ char + 0 3 t3 tos-align }t
t{ t3 str-get s" +++####" ?str }t

t{ s" --" t3 tos-write-string }t
t{ chr.sp 1 1 t3 tos-align }t
t{ t3 str-get s" +++#### -- " ?str }t

t{ t3 tos-rewrite }t
t{ s" Hello" t3 tos-write-string }t
t{ chr.sp 8 t3 tos-align-left }t
t{ t3 str-get s" Hello   " ?str }t

t{ t3 tos-rewrite }t
t{ s" Hello" t3 tos-write-string }t
t{ chr.sp 8 t3 tos-align-right }t
t{ t3 str-get s"    Hello" ?str }t

t{ t3 tos-rewrite }t
t{ s" Hello" t3 tos-write-string }t
t{ chr.sp 8 t3 tos-center }t
t{ t3 str-get s"   Hello " ?str }t

t{ tos-new constant t4 }t

t{ -712 t4 tos-write-number }t
t{ chr.sp 3 t4 tos-align-left }t
t{ t4 str-get s" -712" ?str }t

t{ -100. t4 tos-write-double }t
t{ chr.sp 5 t4 tos-align-right }t
t{ t4 str-get s" -712 -100" ?str }t

precision value save-precision

3 set-precision
t{ -56E+2 t4 tos-write-float }t
t{ bl 10 t4 tos-align-right  }t
t{ t4 str-get s" -712 -100  -0.560E4" ?str }t

t{ -14E-3 t4 tos-write-float }t
t{ bl 10 t4 tos-align-right  }t
t{ t4 str-get s" -712 -100  -0.560E4 -0.140E-1" ?str }t

4 set-precision

t{ t4 tos-rewrite }t
t{ 57.21E-3 t4 tos-write-fixed-point }t
t{ bl t4 tos-write-char }t
t{ 57.21E-2 t4 tos-write-fixed-point }t
t{ bl t4 tos-write-char }t
t{ 57.21E-1 t4 tos-write-fixed-point }t
t{ bl t4 tos-write-char }t
t{ 57.21E+0 t4 tos-write-fixed-point }t
t{ bl t4 tos-write-char }t
t{ 57.21E+1 t4 tos-write-fixed-point }t
t{ bl t4 tos-write-char }t
t{ 57.21E+2 t4 tos-write-fixed-point }t
t{ bl t4 tos-write-char }t
t{ 57.21E+3 t4 tos-write-fixed-point }t
t{ t4 str-get s" 0.05721 0.5721 5.721 57.21 572.1 5721. 57210." ?str }t

t{ t4 tos-rewrite }t
t{ -57.21E-3 t4 tos-write-fixed-point }t
t{ bl t4 tos-write-char }t
t{ -57.21E-2 t4 tos-write-fixed-point }t
t{ bl t4 tos-write-char }t
t{ -57.21E-1 t4 tos-write-fixed-point }t
t{ bl t4 tos-write-char }t
t{ -57.21E+0 t4 tos-write-fixed-point }t
t{ bl t4 tos-write-char }t
t{ -57.21E+1 t4 tos-write-fixed-point }t
t{ bl t4 tos-write-char }t
t{ -57.21E+2 t4 tos-write-fixed-point }t
t{ bl t4 tos-write-char }t
t{ -57.21E+3 t4 tos-write-fixed-point }t
t{ t4 str-get s" -0.05721 -0.5721 -5.721 -57.21 -572.1 -5721. -57210." ?str }t

save-precision set-precision

variable p1

t{ t4 tos-rewrite }t
t{ 2000 t4 tos-write-number }t
t{ s" Hello" t4 tos-write-string }t
t{ t4 tos-pntr@ p1 ! }t
t{ 2000 t4 tos-write-number }t

\ Align last two items

t{ p1 @ t4 tos-pntr! ?true }t
t{ chr.sp 12 t4 tos-center }t
t{ t4 str-get s" 2000  Hello2000 " ?str }t

t{ t4 tos-free }t

\ ==============================================================================
