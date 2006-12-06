\ ==============================================================================
\
\          sh1_test - the test words for the sh1 module in the ffl
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
\  $Date: 2006-12-06 20:06:21 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/sh1.fs
include ffl/tst.fs


.( Testing: sh1) cr 
  
sh1-create sh11

hex

t{ s" abc" sh11 sh1-update }t

t{ sh11 sh1-finish 9CD0D89D ?u 7850C26C ?u BA3E2571 ?u 4706816A ?u A9993E36 ?u }t

t{ sh11 sh1-reset }t

t{ s" abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq" sh11 sh1-update }t

t{ sh11 sh1-finish E54670F1 ?u F95129E5 ?u BAAE4AA1 ?u 1C3BD26E ?u 84983E44 ?u }t

t{ sh1-new value sh12 }t

decimal

: sh1-test
  100000 0 DO
    s" aaaaaaaaaa" sh12 sh1-update
  LOOP
;

sh1-test

hex

t{ sh12 sh1-finish 6534016F ?u DBAD2731 ?u F61EEB2B ?u D4C4DAA4 ?u 34AA973C ?u }t

t{ sh12 sh1-free }t

[THEN]

decimal

\ ==============================================================================

