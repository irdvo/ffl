\ ==============================================================================
\
\          b64_test - the test words for the b64 module in the ffl
\
\               Copyright (C) 2009  Dick van Oudheusden
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
\  $Date: 2007-12-24 19:32:12 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/b64.fs
include ffl/tst.fs


[DEFINED] b64.version [IF]

.( Testing: b64) cr 

str-create b64-var

t{ s" "       b64-var b64-encode s" " ?str }t

t{ s" f"      b64-var b64-encode s" Zg==" ?str }t 

t{ s" fo"     b64-var b64-encode s" Zm8=" ?str }t

t{ s" foo"    b64-var b64-encode s" Zm9v" ?str }t

t{ s" foob"   b64-var b64-encode s" Zm9vYg==" ?str }t

t{ s" fooba"  b64-var b64-encode s" Zm9vYmE=" ?str }t

t{ s" foobar" b64-var b64-encode s" Zm9vYmFy" ?str }t

hex
create b64-test
 92 c, fa c, 56 c, 45 c, 10 c, aa c, 2c c, d5 c, 00 c, 1f c, 6a c, 
decimal 

t{ b64-test 11 b64-var b64-encode s" kvpWRRCqLNUAH2o=" ?str }t

t{ b64-var str-get s" kvpWRRCqLNUAH2o=" ?str }t

\ decodes

t{ s" "         b64-var b64-decode s" " ?str }t

t{ s" Zg=="     b64-var b64-decode s" f" ?str }t 

t{ s" Zm8="     b64-var b64-decode s" fo" ?str }t

t{ s" Zm9v"     b64-var b64-decode s" foo" ?str }t

t{ s" Zm9vYg==" b64-var b64-decode s" foob" ?str }t

t{ s" Zm9vYmE=" b64-var b64-decode s" fooba" ?str }t

t{ s" Zm9vYmFy" b64-var b64-decode s" foobar" ?str }t

t{ s" kvpWRRCqLNUAH2o=" b64-var b64-decode b64-test 11 ?str }t

t{ b64-var str-get b64-test 11 ?str }t

[THEN]

\ ==============================================================================

