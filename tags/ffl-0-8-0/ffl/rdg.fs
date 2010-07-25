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
\  $Date: 2008-03-04 18:39:16 $ $Revision: 1.7 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] rdg.version [IF]


[DEFINED] float [IF]
s" FLOATING-EXT"   environment? [IF] drop
s" FLOATING-STACK" environment? [IF]
23 > [IF]


include ffl/stc.fs


( rdg = Distributed pseudo random number generators )
( The rdg module implements pseudo random number generators with a        )
( distribution. The module requires a pseudo random generator with an     )
( uniform distribution which returns floating point random numbers with   )
( a range of [0,1>. Due to the extensive use of the floating point stack, )
( this module has an environmental dependency.                            )


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

: rdg-gen-[0,1>   ( F: -- r ; rdg -- = Generate a random number [0,1> )
  dup  rdg>data      @
  swap rdg>generator @ 
  execute
;


: rdg-gen-<0,1>   ( F: -- r ; rdg -- = Generate a positive random number <0,1> )
  BEGIN
    dup rdg-gen-[0,1>
    fdup f0=
  WHILE
    fdrop
  REPEAT
  drop
;


( Random generator words )

: rdg-uniform   ( F: r1 r2 -- r3 ; rdg -- = Generate a random number with a uniform distribution in the range of [r1,r2> )
  fover f-                   \ r2 - r1
  rdg-gen-[0,1> f*           \ (r2 - r1) * rng
  f+                         \ r1 + (r2 - r1) * rng
;


\ Ratio method (Kinderman-Monahan); see Knuth v2, 3rd ed, p130.
\ K+M, ACM Trans Math Software 3 (1977) 257-260.
\ With Leva's modifications to the original K+M method; see:
\ J. L. Leva, ACM Trans Math Software 18 (1992) 449-453 and 454-455.

: rdg-normal   ( F: r1 r2 -- r3 ; rdg -- = Generate a random number with a normal or Gaussian distribution with mu or mean r1 and sigma or standard deviation r2 )
  BEGIN
    1E+0 dup rdg-gen-[0,1> f-               \ u = 1.0 - rng
    dup rdg-gen-[0,1> 0.5E+0 f-             \ v = rng - 0.5
    1.7156E+0 f*                            \ v = v * 1.7156
    fover 0.449871E+0 f-                    \ x = u - 0.449871
    fover fabs -0.386595E+0 f-              \ y = fabs (v) - (-0.386595);
    fover 0.25472E+0 f* 
    fover 0.19600E+0 f*
    fswap f- f* 
    fswap fdup f* f+                        \ Q = x * x + y * (0.19600 * y - 0.25472 * x);
    fdup 0.27597E+0 f< IF                   \ Done if Q < 0.27597
      fdrop false
    ELSE
      0.27846E+0 fswap f< IF                \ Continue if Q > 0.27846
        true
      ELSE
        fover fdup fdup f* fswap fln f* -4E+0 f*   \ -4 * u^2 * ln(u)
        fover fdup f*                               \ v^2
        f<                                  \ Continue if v^2 > -4 * u^2 * ln(u) 
      THEN
    THEN
   WHILE
    fdrop fdrop
  REPEAT
  drop
  fswap f/ f* f+                            \ r1 + r2 * v / u
;


: rdg-exponential   ( F: r1 -- r2 ; rdg -- = Generate a random number with an exponential distribution with mu or mean r1 [>0] )
  rdg-gen-<0,1>              \ look for positive random number
  fln fnegate f*             \ -ln rng * r1
;

  
\ Based on Marsaglia and Tsang, "A Simple Method for
\ generating gamma variables", ACM Transactions on Mathematical
\ Software, Vol 26, No 3 (2000), p363-372.

: rdg-gamma   ( F: r1 r2 -- r3 ; rdg -- = Generate a random number with a gamma distribution with alpha r1 [>0] and beta r2 [>0], alpha*beta = mean, alpha*beta^2 = variance )
  fover 1E+0 f< IF
    fover 1E+0 f+ fswap dup recurse         \ rdg-gamma(1+r1, r2) * ..
    1E+0 frot f/ rdg-gen-<0,1> fswap f** f* \ .. rng^(1 / r1)
  ELSE
    f>r
    1E+0 3E+0 f/ f-                         \ d = r1 - 1/3
    1E+0 3E+0 f/ fover fsqrt f/             \ c = 1/3 / sqrt(d)
    f>r                                     \ S: d
    BEGIN
      BEGIN
        0E+0 1E+0 dup rdg-normal            \ x = normal(0,1)
        fdup fr@ f* 1E+0 f+                 \ v = 1.0 + c * x
        fdup f0< fdup f0= OR
      WHILE
        fdrop fdrop
      REPEAT
      fswap f>r                             \ Save x   S: d v
      
      fdup fdup f* f*                       \ v = v * v * v
      
      1e+0 0.0331e+0 fr@ f* fr@ f* fr@ f* fr@ f* f- \ 1 - 0.0331 * x * x * x * x
      
      dup rdg-gen-<0,1> fdup frot f< IF     \ Done when rng < 1 - 0.0331 * x * x * x * x
        fdrop false
      ELSE
        fln
        0.5E+0 fr@ f* fr@ f*                \ 0.5 * x * x
        f>r f>r
        fover fover 1E+0 fover f- fswap fln f+ f*  \ d * (1 - v + ln(v))
        fr> fswap
        fr> f+ f< 0=                        \ Done when ln(rng) < 0.5 * x * x + d * (1 - v + ln(v))
      THEN
      fr> fdrop                             \ Done with x
    WHILE
      fdrop                                 \ Done with v
    REPEAT
    fr> fdrop                               \ Done with c
    drop
    f* fr> f*                               \ r2 * d * v
  THEN
;


\ Based on Knuth

: rdg-beta   ( F: r1 r2 -- r3 ; rdg -- = Generate a random number with a beta distribution with alpha r1 [>0] and beta r2 [>0], alpha*beta = mean, alpha*beta^2 = variance )
  fswap 1E+0 dup rdg-gamma        \ x1 = gamma(r1, 1.0)
  fswap 1E+0     rdg-gamma        \ x2 = gamma(r2, 1.0)
  fover f+ f/                     \ x1 / (x1 + x2)
;


\ Based on Knuth

: rdg-binomial ( F: r -- ; u1 rdg -- u2 = Generate a random number with a binomial distribution with probability r [0,1] and trails u1 [>=0])
  >r
  0                               \ k = 0 n = u1 p = r
  BEGIN
    over 10 u>                    \ while u1 > 10
  WHILE
    swap dup
    2/ 1+                         \ a = 1 + (n / 2)
    swap
    1+ over -                     \ b = 1 + n - a;
    2dup swap
    0 d>f 0 d>f r@ rdg-beta       \ X = rdg-beta(a, b)

    fover fover f< IF             \ If p < X
      drop
      1- swap                     \   n = a-1
      f/                          \   p = p / X
    ELSE                          \ Else
      1- -rot                     \   n = b - 1
      +                           \   k = k + a
      fswap fover f-
      1E+0 frot f- f/             \   p = (p - X) / (1 - X)
    THEN
  REPEAT

  swap r> -rot                    \ S: rdg k n      F: p
  0 ?DO                           \ Loop over n 
    over rdg-gen-[0,1>
    fover f< IF                   \  If (rng < p) Then k++
      1+
    THEN
  LOOP
  nip
  fdrop
;


: rdg-poisson ( F: r -- ; rdg -- u = Generate a random number with a Poisson distribution with mean r [>=0] )
  >r
  0                               \ k = 0 mu = r
  BEGIN
    fdup 10E+0 fswap f<
  WHILE                           \ while mu > 0
    fdup 7E+0 f* 8E+0 f/
    f>d drop                      \   m = (unsigned) mu * 7/8
    dup 0 d>f 1E+0 r@ rdg-gamma   \   X = gamma(m)
    fover fover f< IF             \   If mu < X 
      f/ 1- r> rdg-binomial +     \     k+binomial(mu/X,m-1)
      EXIT
    ELSE                          \   Else
      + f-                        \     k = k + m  mu = mu - X
    THEN
  REPEAT
  
  fnegate fexp                   \  emu = exp(-mu)
  1E+0                           \  prod = 1.0  
  BEGIN                          \  BEGIN
    1+                           \    k++
    r@ rdg-gen-[0,1> f*          \    prod *= rng
    fover fover f< 0=            \  UNTIL prod <= emu
  UNTIL
  fdrop fdrop
  rdrop
  1-                             \ k--
;


: rdg-pareto   ( F: r1 r2 -- r3 ; rdg -- = Generate a random number with a Pareto distribution with alpha r1 [>0] the scale parameter and r2 [>0] the shape parameter )
  rdg-gen-<0,1>                  \ x = rng
  fswap -1E+0 fswap f/ f**       \ z = x ^ (-1/beta)
  f*                             \ r1 * z
;


: rdg-weibull   ( F: r1 r2 -- r3 ; rdg -- = Generate a random number with a Weibull distribution with alpha r1 [>0] the scale parameter and beta r2 [>0] the shape parameter )
  rdg-gen-<0,1>                  \ x = rng
  fln fnegate                    \ -ln(x)
  fswap 1E+0 fswap f/ f**        \ z = -ln(x) ^ (1/beta)
  f*                             \ r1 * z
;

[ELSE]
.( Warning: rdg expects a floating point stack depth of at least 24 floats ) cr
[THEN]
[ELSE]
.( Warning: rdg requires a separate floating point stack ) cr
[THEN]
[ELSE]
.( Warning: rdg requires floating point words ) cr
[THEN]
[ELSE]
.( Warning: rdg requires floating point words ) cr
[THEN]
[THEN]

\ ==============================================================================
