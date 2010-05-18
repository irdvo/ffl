\ ==============================================================================
\
\          a32_test - the test words for the a32 module in the ffl
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
\  $Date: 2008-04-05 08:05:28 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/a32.fs
include ffl/tst.fs


[DEFINED] a32.version [IF]

.( Testing: a32) cr 
  
a32-create ad1

hex

t{ s" abc" ad1 a32-update }t

t{ ad1 a32-finish 24D0127 ?u }t

t{ ad1 a32-length@ 3 ?s }t

t{ ad1 a32-reset }t

t{ s" abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq" ad1 a32-update }t

t{ ad1 a32-finish 807416F9 ?u }t


t{ ad1 a32-reset }t

t{ s" abcd" ad1 a32-update }t
t{ s" bcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq" ad1 a32-update }t

t{ ad1 a32-finish 807416F9 ?u }t


t{ a32-new value ad2 }t

decimal

: a32-test
  50000 0 DO
    s" aaaaaaaaaaaaaaaaaaaa" ad2 a32-update
  LOOP
;

a32-test  \ a million times "a"

t{ ad2 a32-finish a32+to-string s" 15D870F9" ?str }t

t{ ad2 a32-length@ 1000000 ?s }t

t{ ad2 ad1 a32^combine hex CE9E87F1 ?u decimal }t

t{ ad2 a32-reset }t

10000 chars allocate throw value a32-buf    \ 10000 times highest byte 
a32-buf 10000 255 fill

t{ a32-buf 10000 ad2 a32-update }t
t{ ad2 a32-finish hex B623EB2B ?u decimal }t

a32-buf free throw

t{ ad2 a32-free }t

[THEN]

\ ==============================================================================
