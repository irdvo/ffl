\ ==============================================================================
\
\          sh2_test - the test words for the sh2 module in the ffl
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
\  $Date: 2007-12-24 19:32:12 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/sh2.fs
include ffl/tst.fs


[DEFINED] sh2.version [IF]

.( Testing: sh2) cr 
  
sh2-create sh21

hex

t{ s" "  sh21 sh2-update }t    \ see test vectors 1-0

t{ sh21 sh2-finish 7852B855 ?u A495991B ?u 649B934C ?u 27AE41E4 ?u
                   996FB924 ?u 9AFBF4C8 ?u 98FC1C14 ?u E3B0C442 ?u }t
                   

t{ sh21 sh2-reset }t

t{ s" abc" sh21 sh2-update }t    \ see test vectors 1-2

t{ sh21 sh2-finish F20015AD ?u B410FF61 ?u 96177A9C ?u B00361A3 ?u 
                   5DAE2223 ?u 414140DE ?u 8F01CFEA ?u BA7816BF ?u }t
    
                   
t{ sh2-new value sh22 }t   \ see test vector 1-5

t{ s" abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq" sh22 sh2-update }t

t{ sh22 sh2-finish 19DB06C1 ?u F6ECEDD4 ?u 64FF2167 ?u A33CE459 ?u
                   0C3E6039 ?u E5C02693 ?u D20638B8 ?u 248D6A61 ?u }t
                   
                   
decimal

: sh2-test
  31250 0 DO
    s" aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" sh22 sh2-update
  LOOP
;

t{ sh22 sh2-reset }t

sh2-test                 \ million times "a", see test vectors 1-8

t{ sh22 sh2-finish sh2+to-string s" CDC76E5C9914FB9281A1C7E284D73E67F1809A48A497200E046D39CCC7112CD0" ?str }t

t{ sh22 sh2-free }t

[THEN]

\ ==============================================================================

