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
\  $Date: 2005-12-15 19:43:09 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/crc.fs


." Testing: crc" cr 
  
hex
t{ crc-create c1 }t

t{                s" An Arbitrary String"        c1 crc-update c1 crc-finish  6FBEAAE7 ?u }t
t{ c1 crc-start   s" ZYXWVUTSRQPONMLKJIHGFEDBCA" c1 crc-update c1 crc-finish  99CDFDB2 ?u }t
t{ c1 crc-start   s" 123456789"                  c1 crc-update c1 crc-finish  CBF43926 ?u }t
decimal


\ t{ crc-init s" Hello" rot  .s crc-update32 .s crc-get32 .s 4157704578 ?u }t
\ t{ crc-init s"  "     rot  .s crc-update32 .s crc-get32 .s 3928882368 ?u }t 

\ ==============================================================================
