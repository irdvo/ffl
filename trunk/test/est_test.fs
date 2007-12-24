\ ==============================================================================
\
\        est_test - the test words for the est module in the ffl
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

include ffl/est.fs
include ffl/tst.fs


.( Testing: est) cr

: est_test1
  s\" a\a\b\e\f\q\r\t\v\"\xE0\\z"
;

t{ est_test1 13 ?s value est1 }t

: est-fetch ( n - c )
  chars est1 + c@
;

t{  0 est-fetch char a ?s }t
t{  1 est-fetch      7 ?s }t
t{  2 est-fetch      8 ?s }t
t{  3 est-fetch     27 ?s }t
t{  4 est-fetch     12 ?s }t
t{  5 est-fetch char " ?s }t
t{  6 est-fetch     13 ?s }t
t{  7 est-fetch      9 ?s }t
t{  8 est-fetch     11 ?s }t
t{  9 est-fetch char " ?s }t
t{ 10 est-fetch    224 ?s }t
t{ 11 est-fetch char \ ?s }t
t{ 12 est-fetch char z ?s }t

here to est1

,\" a\a\b\e\f\l\m\q\r\t\v\z\"\xE0\\z"

t{  0 est-fetch     17 ?s }t
t{  1 est-fetch char a ?s }t
t{  2 est-fetch      7 ?s }t
t{  3 est-fetch      8 ?s }t
t{  4 est-fetch     27 ?s }t
t{  5 est-fetch     12 ?s }t
t{  6 est-fetch     10 ?s }t
t{  7 est-fetch     13 ?s }t
t{  8 est-fetch     10 ?s }t
t{  9 est-fetch char " ?s }t
t{ 10 est-fetch     13 ?s }t
t{ 11 est-fetch      9 ?s }t
t{ 12 est-fetch     11 ?s }t
t{ 13 est-fetch      0 ?s }t
t{ 14 est-fetch char " ?s }t
t{ 15 est-fetch    224 ?s }t
t{ 16 est-fetch char \ ?s }t
t{ 17 est-fetch char z ?s }t

here to est1

,\" \n"

t{ est1 count end-of-line count ?str }t

\ ==============================================================================
