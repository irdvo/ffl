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
\  $Date: 2006-09-02 14:56:22 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/cpx.fs
include ffl/tst.fs

.( Testing: cpx) cr 
  
\ Module words

\ Calculations

t{ 3.0e0 4.0e0  -4.0e0 5.0e0  cpx+add  9e0 ?r  -1e0  ?r }t
t{ 3.0e0 4.0e0  -4.0e0 5.0e0  cpx+sub -1e0 ?r   7e0  ?r }t
t{ 3.0e0 4.0e0  -4.0e0 5.0e0  cpx+mul -1e0 ?r  -32e0 ?r }t
t{ 3.0e0 4.0e0  -4.0e0 5.0e0  cpx+div -0.7560975609e0 ?r 0.19512195121e0 ?r }t

t{ 1.8e0 -2.7e0 cpx+sin  1.68271849e0 ?r  7.27801970e0 ?r }t
t{ 1.8e0 -2.7e0 cpx+cos  7.21257177e0 ?r -1.69798772e0 ?r }t
t{ 1.8e0 -2.7e0 cpx+tan -1.00812541e0 ?r -0.00402992e0 ?r }t

t{  1e0  1e0 cpx+to-polar  0.7853981e0 ?r 1.41421356e0 ?r }t
t{ -1e0  1e0 cpx+to-polar  2.3561944e0 ?r 1.41421356e0 ?r }t
t{  1e0 -1e0 cpx+to-polar -0.7853981e0 ?r 1.41421356e0 ?r }t
t{ -1e0 -1e0 cpx+to-polar -2.3561944e0 ?r 1.41421356e0 ?r }t

t{ 1.41421356e0  0.7853981e0 cpx+from-polar  1e0 ?r  1e0 ?r }t
t{ 1.41421356e0  2.3561944e0 cpx+from-polar  1e0 ?r -1e0 ?r }t
t{ 1.41421356e0 -0.7853981e0 cpx+from-polar -1e0 ?r  1e0 ?r }t
t{ 1.41421356e0 -2.3561944e0 cpx+from-polar -1e0 ?r -1e0 ?r }t

\ t{  1 2 1 3 cpx+add 6  ?s 5  ?s }t

\ Compare

\ t{ 3 6 1 2 cpx+compare    ?0 }t

\ Conversion

\ : cpxs1 s" -2/7" ;
    
\ t{ -6 21 cpx+to-string cpxs1 compare ?0 }t  


\ Creation and allocation structure

cpx-create cpx1

t{ cpx-new value cpx2 }t

\ t{ cpx1 cpx-get 1 ?s ?0  }t

\ t{ 5 10 cpx1 cpx-set     }t

\ t{ cpx1 cpx-get 2 ?s 1 ?s }t    \ normalized 5/10 -> 1/2


\ t{ cpx2 cpx-get 1 ?s ?0  }t

\ t{ 12 8 cpx2 cpx-set }t

\ t{ cpx2 cpx-num@ 3 ?s   }t
\ t{ cpx2 cpx-denom@ 2 ?s }t     \ normalized 12/8 -> 3/2

\ t{ cpx2 cpx1 cpx^move }t

\ t{ cpx1 cpx-get 2 ?s 3 ?s }t

\ t{ cpx2 cpx1 cpx^compare ?0 }t

\ t{ 1 2 cpx1 cpx-set }t
\ t{ 3 4 cpx2 cpx-set }t

\ t{ cpx2 cpx1 cpx^compare 1 ?s }t

\ t{ 1 3 cpx2 cpx-set }t

\ t{ cpx2 cpx1 cpx^compare -1 ?s }t

t{ cpx2 cpx-free }t

\ ==============================================================================
