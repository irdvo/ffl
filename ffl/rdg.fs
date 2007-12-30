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
\  $Date: 2007-12-30 08:16:08 $ $Revision: 1.2 $
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

: rdg-gen-[0,1>   ( rdg -- r = Generate a random number [0,1> )
  dup  rdg>data      @
  swap rdg>generator @ 
  execute
;


: rdg-gen-<0,1>   ( rdg -- r = Generate a positive random number <0,1> )
  BEGIN
    dup rdg-gen-[0,1>
    fdup f0=
  WHILE
    fdrop
  REPEAT
  drop
;

( Private gamma words )

: rdg-gamma-large   ( r1 rdg -- r2 = Generate a random number with a gamma distribution with alpha r1 [alpha>1] )
  f>r
  fr@ 2E+0 f* 1E+0 f- fsqrt            \ s = sqrt(r1 * 2 - 1)
  BEGIN
    BEGIN
      PI dup rdg-gen-[0,1> f* ftan     \ y = tan(PI * rng)
      fover fover f* fr@ f+ 1E+0 f-    \ x = s * y + r1 - 1
      fdup f0= fdup 0E+0 f< OR
    WHILE                              \ repeat while x <= 0
      fdrop fdrop
    REPEAT
    
    fdup fr@ 1E+0 f- f/ fln            \ t = ln(x / (r1 - 1))
    fr@ 1E+0 f- f*                     \ t = t * (r1 - 1)
    f>r f>r fover fover f* fr> fswap fr> f- fexp   \ t = exp(t - (s * y))
    frot fdup f* 1E+0 f+               \ t = t + (y * y + 1)
    dup rdg-gen-[0,1> f<               \ repeat while rng < t
  WHILE
    fdrop
  REPEAT
  drop
  fr> fdrop
;


: rdg-gamma-int   ( r1 rdg -- r2 = Generate a random number with a gamma distribution with alpha r1 [alpha>0] and alpha is non-fractional )
  fdup 12E+0 f< IF           \ ToDo: Environmental dependency ?
    1E+0 fswap               \ p = 1
    f>d d>s 0 DO             \ Do alpha times
      dup rdg-gen-<0,1> f*   \   p = p * rng
    LOOP
    drop
    fln fnegate              \ p = -ln p
  ELSE
    rdg-gamma-large
  THEN
;


( Random generator words )

: rdg-uniform   ( r1 r2 rdg -- r3 = Generate a random number with a uniform distribution in the range of [r1,r2> )
  fover f-                   \ r2 - r1
  rdg-gen-[0,1> f*           \ (r2 - r1) * rng
  f+                         \ r1 + (r2 - r1) * rng
;


\ Ratio method (Kinderman-Monahan); see Knuth v2, 3rd ed, p130.
\ K+M, ACM Trans Math Software 3 (1977) 257-260.
\ With Leva's modifications to the original K+M method; see:
\ J. L. Leva, ACM Trans Math Software 18 (1992) 449-453 and 454-455. */

: rdg-normal   ( r1 r2 rdg -- r3 = Generate a random number with a normal or gaussian distribution with mu or mean r1 and sigma or standard deviation r2 )
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
      0.27846E+0 f< IF                      \ Done if Q > 0.27846
        fover fdup fdup f* fswap fln f* -4E+0 f*   \ -4 * u^2 * ln(u)
        fover fdup f*                               \ v^2
        fswap f<                            \ Done if v^2 < -4 * u^2 * ln(u) 
      ELSE
        true
      THEN
    THEN
   WHILE
    fdrop fdrop
  REPEAT
  drop
  fswap f/ f* f+                            \ r1 + r2 * v / u
;


: rdg-exponential   ( r1 rdg -- r2 = Generate a random number with an exponential distribution with mu or mean r1 )
  rdg-gen-<0,1>              \ look for positive random number
  fln fnegate f*             \ -ln rng * r1
;

1 [IF]
: .f
  [char] < emit fdepth 0 .r [char] > emit space 
  fdepth dup 0 ?DO
    dup I - 1- fpick f.
  LOOP
  drop cr
;
[THEN]
  
\ Based on Marsaglia and Tsang, "A Simple Method for
\ generating gamma variables", ACM Transactions on Mathematical
\ Software, Vol 26, No 3 (2000), p363-372.

: rdg-gamma   ( r1 r2 rdg -- r3 = Generate a random number with a gamma distribution with alpha r1 [alpha>0] and beta r2 [beta>0], alpha*beta = mean, alpha*beta^2 = variance )
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

0 [IF]
double
gsl_ran_gamma (const gsl_rng * r, const double a, const double b)
{
  /* assume a > 0 */

  if (a < 1)
    {
      double u = gsl_rng_uniform_pos (r);
      return gsl_ran_gamma (r, 1.0 + a, b) * pow (u, 1.0 / a);
    }

  {
    double x, v, u;
    double d = a - 1.0 / 3.0;
    double c = (1.0 / 3.0) / sqrt (d);

    while (1)
      {
        do
          {
            x = gsl_ran_gaussian_ziggurat (r, 1.0);
            v = 1.0 + c * x;
          }
        while (v <= 0);

        v = v * v * v;
        u = gsl_rng_uniform_pos (r);

        if (u < 1 - 0.0331 * x * x * x * x) 
          break;

        if (log (u) < 0.5 * x * x + d * (1 - v + log (v)))
          break;
      }
    
    return b * d * v;
  }
}
[THEN]

: rdg-beta   ( r1 r2 rdg -- r3 = Generate a random number with a beta distribution with alpha r1 [alpha>0] and beta r2 [beta>0], alpha*beta = mean, alpha*beta^2 = variance )
\ ToDo
;


: rdg-binomial ( .. )
;


: rdg-poisson ( .. )
;


: rdg-pareto   ( r1 rdg -- r2 = Generate a random number with a pareto distribution with alpha r1 the shape parameter )
  \ ToDo
;


: rdg-weibull   ( r1 r2 rdg -- r3 = Generate a random number with a Weibull distribution with alpha r1 the scale parameter and beta r2 the shape parameter )
  \ Todo
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
