\ ==============================================================================
\
\        bis_test - the test words for the bis module in the ffl
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
\  $Date: 2009-05-05 05:56:30 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/bis.fs


.( Testing: bis) cr 

t{ bis-create bis1 }t

\ Testing byte reading

: bis-abc ( -- c-addr u ) s" abc" ;

t{ bis-abc bis1 bis-set }t

t{ 4 bis1 bis-read-bytes ?false }t

t{ 1 bis1 bis-read-bytes ?true char a ?s }t

t{ 2 bis1 bis-read-bytes ?true char c #bits/byte lshift char b + ?s }t  \ (c << 8) + b

t{ 1 bis1 bis-read-bytes ?false }t

: bis-def ( -- c-addr u ) s" def" ;

t{ bis-def bis1 bis-set }t

t{ 1 bis1 bis-read-bytes ?true char d ?s }t

t{ 1 bis1 bis-read-bytes ?true char e ?s }t

t{ 2 bis1 bis-read-bytes ?false }t

: bis-ghi ( -- c-addr u ) s" ghi" ;

t{ bis-ghi bis1 bis-set }t

t{ 2 bis1 bis-read-bytes ?true char g #bits/byte lshift char f + ?s }t \ (g << 8) + f

\ Testing bit reading

t{ bis1 bis-bytes>bits }t

t{ 1 bis1 bis-need-bits ?true }t \ h = 0110 1000

t{ 1 bis1 bis-fetch-bits ?0 }t 

t{ 1 bis1 bis-next-bits }t

t{ 3 bis1 bis-need-bits ?true }t

t{ 3 bis1 bis-fetch-bits 4 ?s }t

t{ 3 bis1 bis-next-bits }t

t{ 5 bis1 bis-need-bits ?true }t \ i = 0110 1001

t{ 5 bis1 bis-fetch-bits 22 ?s }t

t{ 5 bis1 bis-next-bits }t

t{ 10 bis1 bis-need-bits ?false }t

: bis-j ( -- c-addr u ) s" j" ;

t{ bis-j bis1 bis-set }t

t{ 10 bis1 bis-need-bits ?true }t \ j = 0110 1010

t{ 10 bis1 bis-fetch-bits 308 ?s }t

t{ 10 bis1 bis-next-bits }t       \ remaining in buffer: 01101

\ Testing bis-get-bit

t{ bis1 bis-get-bit ?true 1 ?s }t

t{ bis1 bis-get-bit ?true 0 ?s }t

t{ bis1 bis-get-bit ?true 1 ?s }t

t{ bis1 bis-get-bit ?true 1 ?s }t

t{ bis1 bis-get-bit ?true 0 ?s }t

t{ bis1 bis-get-bit ?false }t

: bis-jkl  ( -- c-addr u ) s" jkl" ;

t{ bis-jkl bis1 bis-set }t

t{ bis1 bis-get-bit ?true 0 ?s }t

t{ bis1 bis-get-bit ?true 1 ?s }t

\ Testing bits > byte

t{ bis1 bis-bits>bytes }t

t{ 1 bis1 bis-read-bytes ?true char k ?s }t



cell 3 > [IF]

t{ bis-new value bis2 }t

t{ bis-jkl bis2 bis-set }t 

t{ 22 bis2 bis-need-bits ?true }t 

t{ 22 bis2 bis-fetch-bits 2911082 ?s }t

t{ 22 bis2 bis-next-bits }t

t{ bis2 bis-free }t

[THEN]

\ ==============================================================================
