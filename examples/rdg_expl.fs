\ ==============================================================================
\
\     rdg_expl - the pseudo random distribution generator example in the ffl
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
\  $Date: 2008-01-06 06:37:25 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/rng.fs
include ffl/rdg.fs


\ Create a random generator variable in the dictionary with seed 5489

5489 rng-create rng1

\ Create a distributed random generator in the dictionary with the rng1 random generator
\ The distributed random generator expects the state and the word that generates random
\ numbers in the range [0..1>.

rng1 ' rng-next-float rdg-create rdg1

\ Generate a normal or gaussian random number with mean 1.0 and stddev 0.5

1E+0 0.5E+0 rdg1 rdg-normal f. cr

\ Generate an exponential random number with mean 2.0

2E+0 rdg1 rdg-exponential f. cr

\ Generate a gamma random number with alpha 2.0 and beta 0.5

2E+0 0.5E+0 rdg1 rdg-gamma f. cr

\ Generate a beta random number with alpha 2.0 and beta 0.5

2E+0 0.5E+0 rdg1 rdg-beta f. cr

\ Generate a binomial random number with probability 0.4 and trails 15

0.4E+0 15 rdg1 rdg-binomial u. cr

\ Generate a poisson random number with mean 17.0

17E+0 rdg1 rdg-poisson u. cr

\ Generate a pareto random number with alpha 3.5 and beta 2.0

3.5E+0 2E+0 rdg1 rdg-pareto f. cr

\ Generate a weibull random number with alpha 3.5 and beta 2.0

3.5E+0 2E+0 rdg1 rdg-weibull f. cr



\ Create a distributed random generator variable on the heap

rng1 ' rng-next-float rdg-new value rdg2

\ Generate an uniform random number in the range of [34.5,34.6>

34.5E+0 34.6E+0 rdg2 rdg-uniform f. cr

\ Free the variable from the heap

rdg2 rdg-free

