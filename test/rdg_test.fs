\ ==============================================================================
\
\         rdg_test - the test words for the rdg module in the ffl
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
\  $Date: 2007-12-30 08:16:08 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/rng.fs
include ffl/rdg.fs
include ffl/tst.fs


[DEFINED] rdg.version [IF]


.( Testing: rdg) cr 

t{ 5489 rng-create rng3 }t

t{ rng3 ' rng-next-float rdg-create rdg1 }t


\ checked with gsl-1.10: gsl_ran_gaussian_ratio_method
t{  0E+0 0.5E+0 rdg1 rdg-normal -0.44759539E+0 ?r }t 
t{  1E+0 0.5E+0 rdg1 rdg-normal  1.04488381E+0 ?r }t 
t{  0E+0 0.5E+0 rdg1 rdg-normal -0.37048549E+0 ?r }t 
t{  0E+0 0.5E+0 rdg1 rdg-normal  0.22997112E+0 ?r }t 
t{  0E+0 0.5E+0 rdg1 rdg-normal  0.49689682E+0 ?r }t 
t{ -2E+0 0.5E+0 rdg1 rdg-normal -2.87176322E+0 ?r }t 
t{  0E+0 0.5E+0 rdg1 rdg-normal -0.57489741E+0 ?r }t 
t{  0E+0 0.5E+0 rdg1 rdg-normal  1.42277046E+0 ?r }t 
t{  0E+0 0.5E+0 rdg1 rdg-normal  0.07756191E+0 ?r }t 
t{  0E+0 0.5E+0 rdg1 rdg-normal  0.74235660E+0 ?r }t 

\ checked with gsl-1.10: gsl_ran_exponential
t{  0.5E+0 rdg1 rdg-exponential 1.66613875E+0 ?r }t
t{  0.5E+0 rdg1 rdg-exponential 0.50903162E+0 ?r }t
t{  0.5E+0 rdg1 rdg-exponential 0.08177190E+0 ?r }t
t{  0.5E+0 rdg1 rdg-exponential 0.77576300E+0 ?r }t
t{  0.5E+0 rdg1 rdg-exponential 0.03414304E+0 ?r }t
t{  0.5E+0 rdg1 rdg-exponential 0.19183258E+0 ?r }t
t{  0.5E+0 rdg1 rdg-exponential 0.19376214E+0 ?r }t
t{  0.5E+0 rdg1 rdg-exponential 0.45972471E+0 ?r }t
t{  0.5E+0 rdg1 rdg-exponential 0.13870740E+0 ?r }t
t{  0.5E+0 rdg1 rdg-exponential 0.15011541E+0 ?r }t

\ checked with gsl-1.10: gsl_ran_gamma (modified)
t{  15E+0  0.5E+0 rdg1 rdg-gamma 7.01523132E+0  ?r }t
t{  15E+0  0.5E+0 rdg1 rdg-gamma 8.25312323E+0  ?r }t
t{  15E+0  0.5E+0 rdg1 rdg-gamma 6.57587412E+0  ?r }t
t{  15E+0  0.5E+0 rdg1 rdg-gamma 6.31248523E+0  ?r }t
t{  15E+0  0.5E+0 rdg1 rdg-gamma 3.99324402E+0  ?r }t
t{  15E+0  0.5E+0 rdg1 rdg-gamma 4.72211717E+0  ?r }t
t{  15E+0  0.5E+0 rdg1 rdg-gamma 5.35435538E+0  ?r }t
t{  15E+0  0.5E+0 rdg1 rdg-gamma 8.49411653E+0  ?r }t
t{  15E+0  0.5E+0 rdg1 rdg-gamma 6.97074297E+0  ?r }t
t{  15E+0  0.5E+0 rdg1 rdg-gamma 7.00845102E+0  ?r }t

t{  0.5E+0 0.5E+0 rdg1 rdg-gamma 1.47727187E+0 ?r }t
t{  0.5E+0 0.5E+0 rdg1 rdg-gamma 1.33811752E+0 ?r }t
t{  0.5E+0 0.5E+0 rdg1 rdg-gamma 0.56072758E+0 ?r }t
t{  0.5E+0 0.5E+0 rdg1 rdg-gamma 0.32241659E+0 ?r }t
t{  0.5E+0 0.5E+0 rdg1 rdg-gamma 3.42835869E+0 ?r }t
t{  0.5E+0 0.5E+0 rdg1 rdg-gamma 0.00049857E+0 ?r }t
t{  0.5E+0 0.5E+0 rdg1 rdg-gamma 1.87629944E+0 ?r }t
t{  0.5E+0 0.5E+0 rdg1 rdg-gamma 0.31658686E+0 ?r }t
t{  0.5E+0 0.5E+0 rdg1 rdg-gamma 0.04855770E+0 ?r }t
t{  0.5E+0 0.5E+0 rdg1 rdg-gamma 0.07316919E+0 ?r }t

[THEN]

[THEN]

\ ==============================================================================
