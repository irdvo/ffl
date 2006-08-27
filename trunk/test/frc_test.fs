\ ==============================================================================
\
\         frc_test - the test words for the frc module in the ffl
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
\  $Date: 2006-08-27 07:06:44 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/frc.fs
include ffl/tst.fs

.( Testing: frc) cr 
  
\ Module words

t{ 10  2 frc+calc-gcd 2 ?s }t
t{ 2 -10 frc+calc-gcd 2 ?s }t

t{ 5  15 frc+calc-lcm 15 ?s }t
t{ 10 15 frc+calc-lcm 30 ?s }t

\ Creation / allocation

frc-create frc1

t{ frc-new value frc2 }t

t{ frc1 frc-get ?0 1 ?s  }t

t{ 10 5 frc1 frc-set     }t

t{ frc1 frc-get 1 ?s 2 ?s }t    \ normalized 5/10 -> 1/2

\ Members

t{ frc2 frc-get ?0 1 ?s  }t

t{ 8 12 frc2 frc-set }t

t{ frc2 frc-num@ 3 ?s   }t
t{ frc2 frc-denom@ 2 ?s }t     \ normalized 12/8 -> 3/2

\ Calculation

t{ 6 4 frc1 frc-set }t
t{ 5 2 frc2 frc-set }t

\ t{ frc2 frc1 frc^add }t

\ t{ frc1 frc-get 16 ?s 15 ?s }t

\ t{ 5 3 frc1 frc-add }t

\ t{ frc1 frc-get 5 ?s 3 ?s }t

t{ frc2 frc-free }t

\ ==============================================================================
