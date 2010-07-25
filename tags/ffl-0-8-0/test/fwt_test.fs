\ ==============================================================================
\
\          fwt_test - the test words for the fwt module in ffl
\
\             Copyright (C) 2010  Dick van Oudheusden
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
\  $Date: 2008-03-18 19:09:48 $ $Revision: 1.12 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/fwt.fs


.( Testing: fwt ) cr 

\ structure testing

begin-structure fwt1%
  wfield:    fwt1>1
end-structure

t{   fwt1%  2 ?s }t
t{ 0 fwt1>1 0 ?s }t

begin-structure fwt2%
  4 wfields: fwt2>1
end-structure

t{   fwt2%  8 ?s }t
t{ 0 fwt2>1 0 ?s }t

begin-structure fwt3%
  lfield:    fwt3>1
end-structure

t{   fwt3%  4 ?s }t
t{ 0 fwt3>1 0 ?s }t

begin-structure fwt4%
  3 lfields: fwt4>1
end-structure

t{   fwt4%  12 ?s }t
t{ 0 fwt4>1  0 ?s }t

begin-structure fwt5%
  wfield:    fwt5>1
  field:     fwt5>2  \ align
  wfield:    fwt5>3
  wfield:    fwt5>4
  lfield:    fwt5>5
  field:     fwt5>6
  lfield:    fwt5>7
  lfield:    fwt5>8
end-structure

cell 4 = [IF]
t{   fwt5%  28 ?s }t
t{ 0 fwt5>1  0 ?s }t
t{ 0 fwt5>2  4 ?s }t
t{ 0 fwt5>3  8 ?s }t
t{ 0 fwt5>4 10 ?s }t
t{ 0 fwt5>5 12 ?s }t
t{ 0 fwt5>6 16 ?s }t
t{ 0 fwt5>7 20 ?s }t
t{ 0 fwt5>8 24 ?s }t
[THEN]

cell 8 = [IF]
t{   fwt5%  40 ?s }t
t{ 0 fwt5>1  0 ?s }t
t{ 0 fwt5>2  8 ?s }t
t{ 0 fwt5>3 16 ?s }t
t{ 0 fwt5>4 18 ?s }t
t{ 0 fwt5>5 20 ?s }t
t{ 0 fwt5>6 24 ?s }t
t{ 0 fwt5>7 32 ?s }t
t{ 0 fwt5>8 36 ?s }t
[THEN]


\ memory access testing

variable fwt1

1 fwt1 !
t{ fwt1  w@      1 ?s }t
t{ fwt1 <w@      1 ?s }t

-2 fwt1 !
t{ fwt1  w@  65534 ?s }t
t{ fwt1 <w@     -2 ?s }t

196607 fwt1 !
t{ -2 fwt1 w!         }t
t{ fwt1   @ 196606 ?s }t
t{ fwt1  w@  65534 ?s }t
t{ fwt1 <w@     -2 ?s }t

cell 4 = [IF]
1 fwt1 !
t{ fwt1  l@      1 ?s }t
t{ fwt1 <l@      1 ?s }t

-2 fwt1 !
t{ fwt1  l@     -2 ?s }t
t{ fwt1 <l@     -2 ?s }t

t{ 196607 fwt1 l!     }t
t{ fwt1   @ 196607 ?s }t
t{ fwt1  l@ 196607 ?s }t
t{ fwt1 <l@ 196607 ?s }t
[THEN]


cell 8 = [IF]
1 fwt1 !
t{ fwt1  l@           1 ?s }t
t{ fwt1 <l@           1 ?s }t

-2 fwt1 !
t{ fwt1  l@  4294967294 ?s }t
t{ fwt1 <l@          -2 ?s }t

12884901886 fwt1 !
t{ fwt1   @ 12884901886 ?s }t
t{ fwt1  l@  4294967294 ?s }t
t{ fwt1 <l@          -2 ?s }t
[THEN]

\ ==============================================================================
