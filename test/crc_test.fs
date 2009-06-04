\ ==============================================================================
\
\        crc_test - the cyclic redundancy check test in the ffl
\
\               Copyright (C) 2005  Dick van Oudheusden
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
\  $Date: 2006-12-05 18:32:48 $ $Revision: 1.7 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/crc.fs

[DEFINED] crc.version [IF]

.( Testing: crc) cr 
  
hex
t{ crc-create c1 }t

t{ c1 crc-poly@ EDB88320 ?u }t

t{                s" An Arbitrary String"        c1 crc-update c1 crc-finish  6FBEAAE7 ?u }t
t{ c1 crc-reset   s" ZYXWVUTSRQPONMLKJIHGFEDBCA" c1 crc-update c1 crc-finish  99CDFDB2 ?u }t
t{ c1 crc-reset   s" 123456789"                  c1 crc-update c1 crc-finish  CBF43926 ?u }t
decimal

t{ 32 26 23 22 16 12 11 10 8 7 5 4 2 1 0 15 crc+calc-poly hex EDB88320 decimal ?u }t

hex
t{ EDB88320 c1 crc-poly! }t

t{ c1 crc-reset   s" ZYXWVUTSRQPONMLKJIHGFEDBCA" c1 crc-update c1 crc-finish  99CDFDB2 ?u }t

t{ s" An Arbitrary String" crc+calc-crc32 6FBEAAE7 ?u }t
decimal

[THEN]

\ ==============================================================================
