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
\  $Date: 2007-12-24 19:32:12 $ $Revision: 1.3 $
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

cell 4 = [IF]
t{ 2000000000 dup dup frc+calc-lcm ?s }t
[THEN]


\ Calculations

t{  1 2 1 3 frc+add 6  ?s 5  ?s }t
t{  4 6 2 5 frc+add 15 ?s 16 ?s }t
t{ -1 3 1 6 frc+add 2dup 18 ?s -3 ?s frc+norm  6 ?s -1 ?s }t

t{  2 3  1 6 frc+subtract 2dup 18 ?s  9 ?s frc+norm  2 ?s  1 ?s }t
t{  2 3 -1 6 frc+subtract 2dup 18 ?s 15 ?s frc+norm  6 ?s  5 ?s }t

t{  4 6 2 5  frc+multiply 15 ?s  4 ?s }t
t{ -2 3 3 7  frc+multiply 2dup 21 ?s -6 ?s frc+norm  7 ?s -2 ?s }t

t{  2 3 1 6  frc+divide 2dup  3 ?s 12 ?s frc+norm  1 ?s  4 ?s }t
t{ 1 3 -1 4  frc+divide 2dup  3 ?s -4 ?s frc+norm  3 ?s -4 ?s }t

t{ -1 3 frc+invert 1 ?s -3 ?s }t

t{ -1 3 frc+negate  3 ?s  1 ?s }t
t{  1 3 frc+negate  3 ?s -1 ?s }t
t{ -1 3 frc+abs     3 ?s  1 ?s }t
t{  1 3 frc+abs     3 ?s  1 ?s }t

\ Compare

t{ 3 6 1 2 frc+compare    ?0 }t
t{ 4 6 1 2 frc+compare  1 ?s }t
t{ 2 6 1 2 frc+compare -1 ?s }t

\ Conversion

: frcs1 s" -2/7" ;
: frcs2 s" 0"    ;
: frcs3 s" 3"    ;
: frcs4 s" 12/5" ;
    
t{ -6 21 frc+to-string frcs1 ?str }t  
t{  0 3  frc+to-string frcs2 ?str }t
t{  3 1  frc+to-string frcs3 ?str }t
t{ 12 5  frc+to-string frcs4 ?str }t

[DEFINED] frc+to-float [IF]
t{  1 3 frc+to-float  0.33333e0 ?r }t
t{ -1 3 frc+to-float -0.33333e0 ?r }t
[THEN]


\ Creation and allocation structure

frc-create frc1

t{ frc-new value frc2 }t

t{ frc1 frc-get 1 ?s ?0  }t

t{ 5 10 frc1 frc-set     }t

t{ frc1 frc-get 2 ?s 1 ?s }t    \ normalized 5/10 -> 1/2


t{ frc2 frc-get 1 ?s ?0  }t

t{ 12 8 frc2 frc-set }t

t{ frc2 frc-num@ 3 ?s   }t
t{ frc2 frc-denom@ 2 ?s }t     \ normalized 12/8 -> 3/2

t{ frc2 frc1 frc^move }t

t{ frc1 frc-get 2 ?s 3 ?s }t

t{ frc2 frc1 frc^compare ?0 }t

t{ 1 2 frc1 frc-set }t
t{ 3 4 frc2 frc-set }t

t{ frc2 frc1 frc^compare 1 ?s }t

t{ 1 3 frc2 frc-set }t

t{ frc2 frc1 frc^compare -1 ?s }t

t{ frc2 frc-free }t

\ ==============================================================================
