\ ==============================================================================
\
\    rdg - the distributed pseudo random number generator module in the ffl
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
\  $Date: 2007-12-27 06:19:48 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] rdg.version [IF]


[DEFINED] float [IF]
s" FLOATING-EXT"   environment? [IF] drop
s" FLOATING-STACK" environment? [IF] drop


include ffl/stc.fs


( rdg = Distributed pseudo random number generators )
( The rdg module implements pseudo random number generators with a        )
( distribution. The module requires a pseudo random generator with an     )
( uniform distribution which returns floating point random numbers with   )
( a range of [0,1>.                                                       )

1 constant rdg.version


( Private words )

( Distributed Random generator structure )

begin-structure rdg%       ( -- n = Get the required space for a rdg variable )
  field:  rdg>data            \ the state of the random generator
  field:  rdg>generator       \ the xt of the random generator
end-structure


( Distributed random generator creation, initialisation and destruction )

: rdg-init         ( x xt rdg -- = Initialise the generator with the random generator xt and its data x )
  tuck
  rdg>generator !
  rdg>data      !
;


: rdg-create       ( x xt "<spaces>name" -- ; -- rdg = Create a named random generator in the dictionary with the random generator xt and its data x )
  create   here   rdg% allot   rdg-init
;


: rdg-new          ( x xt -- rdg = Create a new random generator on the heap with the random generator xt and its data x )
  rdg% allocate  throw  >r r@ rdg-init r>
;


: rdg-free         ( rdg -- = Free the random generator from the heap )
  free throw 
;


( Private words )

: rdg-generate   ( rdg -- r = Generate a random number )
  dup  rdg>data      @
  swap rdg>generator @ 
  execute
;
  

( Random generator words )

: rdg-uniform   ( r1 r2 rdg -- r3 = Generate a random number with a uniform distribution in the range of [r1,r2> )
  fover f-                   \ r2 - r1
  rdg-generate f*            \ (r2 - r1) * rng
  f+                         \ r1 + (r2 - r1) * rng
;


: rdg-normal   ( r1 r2 rdg -- r3 = Generate a random number with a normal or gaussian distribution with mu or mean r1 and sigma or standard deviation r2 )
\ ToDo
;


: rdg-lognormal   ( r1 r2 rdg -- r3 = Generate a random number with a log normal distribution with mu or mean r1 and sigma or standard deviation r2 )
  rdg-normal
  fexp
;


: rdg-exponential   ( r1 rdg -- r2 = Generate a random number with an exponential distribution with lambda or 1/mean r1 )
  BEGIN
    dup rdg-generate         \ look for random value > 1e-7
    fdup 1E-7 f<
  WHILE
    fdrop
  REPEAT
  drop
  flog fnegate               \ -log rng
  fswap f/                   \ -log rng / r1
;


: rdg-von-misses   ( r1 r2 rdg -- r3 = Generate a random number with a Von Misses distibution with mu or mean angle [0,2pi> r1 and kappa or concentration parameter [kappa >= 0] r2 )
\ ToDo
;


: rdg-gamma   ( r1 r2 rdg -- r3 = Generate a random number with a gamma distribution with alpha r1 [alpha>0] and beta r2 [beta>0], alpha*beta = mean, alpha*beta^2 = variance )
\ ToDo
;


: rdg-beta   ( r1 r2 rdg -- r3 = Generate a random number with a beta distribution with alpha r1 [alpha>0] and beta r2 [beta>0], alpha*beta = mean, alpha*beta^2 = variance )
\ ToDo
;


: rdg-pareto   ( r1 rdg -- r2 = Generate a random number with a pareto distribution with alpha r1 the shape parameter )
  1e+0 rdg-generate f-       \ 1.0 - rng
  fswap
  1e+0 fswap f/              \ 1.0 / r1
  f**                        \ (1.0 - rng) ^ (1.0 / r1)
  1e+0 fswap f/              \ 1.0 / (1.0 - rng) ^ (1.0 / r1)
;


: rdg-weibull   ( r1 r2 rdg -- r3 = Generate a random number with a Weibull distribution with alpha r1 the scale parameter and beta r2 the shape parameter )
  1e+0 rdg-generate f-       \ 1.0 - rng
  flog fnegate               \ -log(1.0 - rng)
  1e+0 frot f/               \ 1.0 / r2
  f**                        \ (-log(1.0 - rng)) ^ (1.0 / r2)
  f*                         \ (-log(1.0 - rng)) ^ (1.0 / r2) * r1
;

[ELSE]
.( Warning: cpx requires a separate floating point stack ) cr
[THEN]
[ELSE]
.( Warning: cpx requires floating point words ) cr
[THEN]
[ELSE]
.( Warning: cpx requires floating point words ) cr
[THEN]
[THEN]

\ ==============================================================================
