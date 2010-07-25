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
\  $Date: 2007-12-24 19:32:12 $ $Revision: 1.7 $
\
\ ==============================================================================

include ffl/cpx.fs
include ffl/tst.fs


[DEFINED] cpx.version [IF]


.( Testing: cpx) cr 
  
\ Module words

\ Calculations

t{ 3.0E+0 4.0E+0  -4.0E+0 5.0E+0  cpx+add  9E+0 ?r  -1E+0  ?r }t
t{ 3.0E+0 4.0E+0  -4.0E+0 5.0E+0  cpx+sub -1E+0 ?r   7E+0  ?r }t
t{ 3.0E+0 4.0E+0  -4.0E+0 5.0E+0  cpx+mul -1E+0 ?r  -32E+0 ?r }t
t{ 3.0E+0 4.0E+0  -4.0E+0 5.0E+0  cpx+div -0.7560975609E+0 ?r 0.19512195121E+0 ?r }t

t{  0.0E+0  0.0E+0 cpx+sqrt  0.0E+0 ?r 0.0E+0 ?r }t 
t{  1.0E+0  1.0E+0 cpx+sqrt  0.4550898E+0 ?r 1.0986841E+0 ?r }t 
t{  1.0E+0 -1.0E+0 cpx+sqrt -0.4550898E+0 ?r 1.0986841E+0 ?r }t
t{ -2.0E+0  2.0E+0 cpx+sqrt  1.5537739E+0 ?r 0.6435942E+0 ?r }t   
t{ -3.0E+0 -3.0E+0 cpx+sqrt -1.9029767E+0 ?r 0.7882387E+0 ?r }t

t{  0.0E+0  0.0E+0 cpx+exp  0.0E+0 ?r       1.0E+0 ?r }t
t{  1.0E+0 -1.0E+0 cpx+exp -2.2873552E+0 ?r 1.4686939E+0 ?r }t
t{ -2.0E+0  2.0E+0 cpx+exp  0.1230600E+0 ?r -0.0563193E+0 ?r }t
t{ -3.0E+0 -3.0E+0 cpx+exp -0.0070259E+0 ?r -0.0492888E+0 ?r }t 

t{  1.0E+0 -1.0E+0 cpx+ln  -0.7853981E+0 ?r  0.3465735E+0 ?r }t
t{ -1.0E+0  3.0E+0 cpx+ln   1.8925468E+0 ?r  1.1512925E+0 ?r }t 
t{ -2.0E+0 -3.0E+0 cpx+ln  -2.1587989E+0 ?r  1.2824746E+0 ?r }t 

t{ 1.8E+0 -2.7E+0 cpx+sin  1.68271849E+0 ?r  7.27801970E+0 ?r }t
t{ 1.8E+0 -2.7E+0 cpx+cos  7.21257177E+0 ?r -1.69798772E+0 ?r }t
t{ 1.8E+0 -2.7E+0 cpx+tan -1.00812541E+0 ?r -0.00402992E+0 ?r }t

t{ 1.8E+0 -2.7E+0 cpx+asin -1.8799469E+0 ?r  0.5667016E+0 ?r }t
t{ 1.8E+0 -2.7E+0 cpx+acos  1.8799469E+0 ?r  1.0040946E+0 ?r }t
t{ 1.8E+0 -2.7E+0 cpx+atan -0.2539731E+0 ?r  1.3902046E+0 ?r }t

t{ 1.8E+0 -2.7E+0 cpx+sinh  -1.3280715E+0 ?r -2.6599378E+0 ?r }t
t{ 1.8E+0 -2.7E+0 cpx+cosh  -1.2574260E+0 ?r -2.8093799E+0 ?r }t
t{ 1.8E+0 -2.7E+0 cpx+tanh   0.0407845E+0 ?r  0.9650604E+0 ?r }t

t{  1.8E+0 -2.7E+0 cpx+asinh -0.9602976E+0 ?r  1.8617664E+0 ?r }t
t{  1.8E+0  2.7E+0 cpx+acosh  1.0040946E+0 ?r  1.8799469E+0 ?r }t
t{  1.8E+0 -2.7E+0 cpx+acosh -1.0040946E+0 ?r  1.8799469E+0 ?r }t
t{ -2.0E+0  0.0E+0 cpx+acosh  3.1415926E+0 ?r  1.3169578E+0 ?r }t
t{ -1.8E+0  2.7E+0 cpx+acosh  2.1374979E+0 ?r  1.8799469E+0 ?r }t
t{  1.8E+0 -2.7E+0 cpx+atanh -1.3130350E+0 ?r  0.1615066E+0 ?r }t

\ Conversion

t{  1E+0  1E+0 cpx+to-polar  0.7853981E+0 ?r 1.41421356E+0 ?r }t
t{ -1E+0  1E+0 cpx+to-polar  2.3561944E+0 ?r 1.41421356E+0 ?r }t
t{  1E+0 -1E+0 cpx+to-polar -0.7853981E+0 ?r 1.41421356E+0 ?r }t
t{ -1E+0 -1E+0 cpx+to-polar -2.3561944E+0 ?r 1.41421356E+0 ?r }t

t{ 1.41421356E+0  0.7853981E+0 cpx+from-polar  1E+0 ?r  1E+0 ?r }t
t{ 1.41421356E+0  2.3561944E+0 cpx+from-polar  1E+0 ?r -1E+0 ?r }t
t{ 1.41421356E+0 -0.7853981E+0 cpx+from-polar -1E+0 ?r  1E+0 ?r }t
t{ 1.41421356E+0 -2.3561944E+0 cpx+from-polar -1E+0 ?r -1E+0 ?r }t

: cpxs1 s" -0.180e1-0.270e1j" ;
: cpxs2 s" -0.180e1+0.270e1j" ;
: cpxs3 s" 0.100e1-0.270e1j" ;    


3 set-precision

t{ -1.8E+0 -2.7E+0 cpx+to-string cpxs1 ?str }t
t{ -1.8E+0  2.7E+0 cpx+to-string cpxs2 ?str }t
t{  1.0E+0 -2.7E+0 cpx+to-string cpxs3 ?str }t


\ Structure

cpx-create cpx1

t{ cpx-new value cpx2 }t

t{ cpx1 cpx-get 0E+0 ?r 0E+0 ?r }t

t{ 5.1E+0 10.9E+0 cpx1 cpx-set   }t

t{ cpx1 cpx-get 10.9E+0 ?r 5.1E+0 ?r }t

t{ cpx1 cpx2 cpx^move }t

t{ cpx2 cpx-re@  5.1E+0 ?r }t
t{ cpx2 cpx-im@ 10.9E+0 ?r }t

t{ cpx1 cpx2 cpx^equal? ?true }t

t{ cpx2 cpx-get cpx+conj cpx2 cpx-set }t

t{ cpx1 cpx2 cpx^equal? ?false }t

t{ cpx2 cpx-free }t

[THEN]

\ ==============================================================================
