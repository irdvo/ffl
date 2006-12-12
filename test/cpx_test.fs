\ ==============================================================================
\
\         cpx_test - the test words for the cpx module in the ffl
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
\  $Date: 2006-12-12 19:45:52 $ $Revision: 1.5 $
\
\ ==============================================================================

include ffl/cpx.fs
include ffl/tst.fs


[DEFINED] cpx.version [IF]


.( Testing: cpx) cr 
  
\ Module words

\ Calculations

t{ 3.0e0 4.0e0  -4.0e0 5.0e0  cpx+add  9e0 ?r  -1e0  ?r }t
t{ 3.0e0 4.0e0  -4.0e0 5.0e0  cpx+sub -1e0 ?r   7e0  ?r }t
t{ 3.0e0 4.0e0  -4.0e0 5.0e0  cpx+mul -1e0 ?r  -32e0 ?r }t
t{ 3.0e0 4.0e0  -4.0e0 5.0e0  cpx+div -0.7560975609e0 ?r 0.19512195121e0 ?r }t

t{  0.0e0  0.0e0 cpx+sqrt 0.0e ?r 0.0e0 ?r }t 
t{  1.0e0  1.0e0 cpx+sqrt  0.4550898e0 ?r 1.0986841e0 ?r }t 
t{  1.0e0 -1.0e0 cpx+sqrt -0.4550898e0 ?r 1.0986841e0 ?r }t
t{ -2.0e0  2.0e0 cpx+sqrt  1.5537739e0 ?r 0.6435942e0 ?r }t   
t{ -3.0e0 -3.0e0 cpx+sqrt -1.9029767e0 ?r 0.7882387e0 ?r }t

t{  0.0e0  0.0e0 cpx+exp  0.0e0 ?r       1.0e0 ?r }t
t{  1.0e0 -1.0e0 cpx+exp -2.2873552e0 ?r 1.4686939e0 ?r }t
t{ -2.0e0  2.0e0 cpx+exp  0.1230600e0 ?r -0.0563193e0 ?r }t
t{ -3.0e0 -3.0e0 cpx+exp -0.0070259e0 ?r -0.0492888e0 ?r }t 

t{  1.0e0 -1.0e0 cpx+ln  -0.7853981e0 ?r  0.3465735e0 ?r }t
t{ -1.0e0  3.0e0 cpx+ln   1.8925468e0 ?r  1.1512925e0 ?r }t 
t{ -2.0e0 -3.0e0 cpx+ln  -2.1587989e0 ?r  1.2824746e0 ?r }t 

t{ 1.8e0 -2.7e0 cpx+sin  1.68271849e0 ?r  7.27801970e0 ?r }t
t{ 1.8e0 -2.7e0 cpx+cos  7.21257177e0 ?r -1.69798772e0 ?r }t
t{ 1.8e0 -2.7e0 cpx+tan -1.00812541e0 ?r -0.00402992e0 ?r }t

t{ 1.8e0 -2.7e0 cpx+asin -1.8799469e0 ?r  0.5667016e0 ?r }t
t{ 1.8e0 -2.7e0 cpx+acos  1.8799469e0 ?r  1.0040946e0 ?r }t
t{ 1.8e0 -2.7e0 cpx+atan -0.2539731e0 ?r  1.3902046e0 ?r }t

t{ 1.8e0 -2.7e0 cpx+sinh  -1.3280715e0 ?r -2.6599378e0 ?r }t
t{ 1.8e0 -2.7e0 cpx+cosh  -1.2574260e0 ?r -2.8093799e0 ?r }t
t{ 1.8e0 -2.7e0 cpx+tanh   0.0407845e0 ?r  0.9650604e0 ?r }t

t{  1.8e0 -2.7e0 cpx+asinh -0.9602976e0 ?r  1.8617664e0 ?r }t
t{  1.8e0  2.7e0 cpx+acosh  1.0040946e0 ?r  1.8799469e0 ?r }t
t{  1.8e0 -2.7e0 cpx+acosh -1.0040946e0 ?r  1.8799469e0 ?r }t
t{ -2.0e0  0.0e0 cpx+acosh  3.1415926e0 ?r  1.3169578e0 ?r }t
t{ -1.8e0  2.7e0 cpx+acosh  2.1374979e0 ?r  1.8799469e0 ?r }t
t{  1.8e0 -2.7e0 cpx+atanh -1.3130350e0 ?r  0.1615066e0 ?r }t

\ Conversion

t{  1e0  1e0 cpx+to-polar  0.7853981e0 ?r 1.41421356e0 ?r }t
t{ -1e0  1e0 cpx+to-polar  2.3561944e0 ?r 1.41421356e0 ?r }t
t{  1e0 -1e0 cpx+to-polar -0.7853981e0 ?r 1.41421356e0 ?r }t
t{ -1e0 -1e0 cpx+to-polar -2.3561944e0 ?r 1.41421356e0 ?r }t

t{ 1.41421356e0  0.7853981e0 cpx+from-polar  1e0 ?r  1e0 ?r }t
t{ 1.41421356e0  2.3561944e0 cpx+from-polar  1e0 ?r -1e0 ?r }t
t{ 1.41421356e0 -0.7853981e0 cpx+from-polar -1e0 ?r  1e0 ?r }t
t{ 1.41421356e0 -2.3561944e0 cpx+from-polar -1e0 ?r -1e0 ?r }t

: cpxs1 s" -0.180e1-0.270e1j" ;
: cpxs2 s" -0.180e1+0.270e1j" ;
: cpxs3 s" 0.100e1-0.270e1j" ;    
  
  
3 set-precision

t{ -1.8e0 -2.7e0 cpx+to-string cpxs1 compare ?0 }t
t{ -1.8e0  2.7e0 cpx+to-string cpxs2 compare ?0 }t
t{  1.0e0 -2.7e0 cpx+to-string cpxs3 compare ?0 }t


\ Structure

cpx-create cpx1

t{ cpx-new value cpx2 }t

t{ cpx1 cpx-get 0e0 ?r 0e0 ?r }t

t{ 5.1e0 10.9e0 cpx1 cpx-set   }t

t{ cpx1 cpx-get 10.9e0 ?r 5.1e0 ?r }t

t{ cpx1 cpx2 cpx^move }t

t{ cpx2 cpx-re@  5.1e0 ?r }t
t{ cpx2 cpx-im@ 10.9e0 ?r }t

t{ cpx1 cpx2 cpx^equal? ?true }t

t{ cpx2 cpx-get cpx+conj cpx2 cpx-set }t

t{ cpx1 cpx2 cpx^equal? ?false }t

t{ cpx2 cpx-free }t

[THEN]

\ ==============================================================================
