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
\  $Date: 2008-03-02 15:03:03 $ $Revision: 1.4 $
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

\ checked with gsl-1.10: gsl_ran_beta (modified)
t{ 0.1E+0   0.1E+0 rdg1 rdg-beta 0.99993960E+0 ?r }t
t{ 0.2E+0   1.1E+0 rdg1 rdg-beta 0.32639035E+0 ?r }t
t{ 0.3E+0   3.1E+0 rdg1 rdg-beta 0.00575353E+0 ?r }t
t{ 0.4E+0   7.1E+0 rdg1 rdg-beta 0.10315773E+0 ?r }t
t{ 0.5E+0  15.9E+0 rdg1 rdg-beta 0.01867630E+0 ?r }t
t{ 0.6E+0  31.9E+0 rdg1 rdg-beta 0.00562753E+0 ?r }t
t{ 0.7E+0  63.9E+0 rdg1 rdg-beta 0.00268437E+0 ?r }t
t{ 0.8E+0 127.9E+0 rdg1 rdg-beta 0.00573342E+0 ?r }t
t{ 0.9E+0 255.9E+0 rdg1 rdg-beta 0.00201518E+0 ?r }t
t{ 1.0E+0 511.9E+0 rdg1 rdg-beta 0.00109881E+0 ?r }t

\ checked with gsl-1.10: gsl_ran_binomial_knuth (modified)
t{ 0.9E+0   5 rdg1 rdg-binomial   4 ?u }t
t{ 0.8E+0  15 rdg1 rdg-binomial  11 ?u }t
t{ 0.7E+0  25 rdg1 rdg-binomial  21 ?u }t
t{ 0.6E+0  35 rdg1 rdg-binomial  21 ?u }t
t{ 0.5E+0  45 rdg1 rdg-binomial  20 ?u }t
t{ 0.4E+0  55 rdg1 rdg-binomial  24 ?u }t
t{ 0.3E+0  65 rdg1 rdg-binomial  16 ?u }t
t{ 0.2E+0  75 rdg1 rdg-binomial  13 ?u }t
t{ 0.1E+0  85 rdg1 rdg-binomial  15 ?u }t
t{ 0.0E+0  95 rdg1 rdg-binomial   0 ?u }t

\ checked with gsl-1.10: gsl_ran_poisson (modified)
t{  0.5E+0 rdg1 rdg-poisson   0 ?u }t
t{  2.5E+0 rdg1 rdg-poisson   5 ?u }t
t{  4.5E+0 rdg1 rdg-poisson   3 ?u }t
t{  6.5E+0 rdg1 rdg-poisson   5 ?u }t
t{  8.5E+0 rdg1 rdg-poisson   8 ?u }t
t{ 10.5E+0 rdg1 rdg-poisson  13 ?u }t
t{ 12.5E+0 rdg1 rdg-poisson  10 ?u }t
t{ 14.5E+0 rdg1 rdg-poisson  10 ?u }t
t{ 16.5E+0 rdg1 rdg-poisson  16 ?u }t
t{ 18.5E+0 rdg1 rdg-poisson  17 ?u }t

\ checked with gsl-1.10: gsl_ran_pareto
t{  0.5E+0 0.5E+0 rdg1 rdg-pareto  3.19627113E+0 ?r }t
t{  0.5E+0 1.5E+0 rdg1 rdg-pareto  0.67574893E+0 ?r }t
t{  1.5E+0 2.5E+0 rdg1 rdg-pareto  2.23881531E+0 ?r }t
t{  0.5E+0 3.5E+0 rdg1 rdg-pareto  0.85719001E+0 ?r }t
t{  2.5E+0 4.5E+0 rdg1 rdg-pareto  2.50672614E+0 ?r }t
t{  0.5E+0 5.5E+0 rdg1 rdg-pareto  0.52442780E+0 ?r }t
t{  3.5E+0 6.5E+0 rdg1 rdg-pareto  5.79460199E+0 ?r }t
t{  0.5E+0 7.5E+0 rdg1 rdg-pareto  0.50829023E+0 ?r }t
t{  0.5E+0 8.5E+0 rdg1 rdg-pareto  0.50722690E+0 ?r }t
t{  4.5E+0 9.5E+0 rdg1 rdg-pareto  4.51976424E+0 ?r }t

\ checked with gsl-1.10: gsl_ran_weibull
t{  0.5E+0 0.5E+0 rdg1 rdg-weibull  0.00411372E+0 ?r }t
t{  0.5E+0 1.5E+0 rdg1 rdg-weibull  1.26279804E+0 ?r }t
t{  1.5E+0 2.5E+0 rdg1 rdg-weibull  0.83024858E+0 ?r }t
t{  0.5E+0 3.5E+0 rdg1 rdg-weibull  0.41924933E+0 ?r }t
t{  2.5E+0 4.5E+0 rdg1 rdg-weibull  3.01282633E+0 ?r }t
t{  0.5E+0 5.5E+0 rdg1 rdg-weibull  0.39286396E+0 ?r }t
t{  3.5E+0 6.5E+0 rdg1 rdg-weibull  3.66115208E+0 ?r }t
t{  0.5E+0 7.5E+0 rdg1 rdg-weibull  0.50733162E+0 ?r }t
t{  0.5E+0 8.5E+0 rdg1 rdg-weibull  0.50523448E+0 ?r }t
t{  4.5E+0 9.5E+0 rdg1 rdg-weibull  4.85256504E+0 ?r }t

[THEN]

\ ==============================================================================
