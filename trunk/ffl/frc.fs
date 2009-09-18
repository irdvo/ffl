\ ==============================================================================
\
\                 frc - the fraction module in the ffl
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
\  $Date: 2007-12-09 07:23:15 $ $Revision: 1.6 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] frc.version [IF]


include ffl/stc.fs


( frc = Fraction module )
( The frc module implements a fraction data type. Note: to reduce normalizing  )
( of fractions, the input is validated and normalized, but the resulting       )
( output is NOT normalized. This is left to the next fraction word.            )


1 constant frc.version

  
( Fraction structure )

begin-structure frc%       ( -- n = Get the required space for a frc variable )
  field: frc>num         \ numerator
  field: frc>denom       \ denominator
end-structure



( Fraction creation, initialisation and destruction )

: frc-init         ( frc -- = Initialise the fraction to zero )
  dup    frc>num 0!
  1 swap frc>denom !
;


: frc-create       ( "<spaces>name" -- ; -- frc = Create a named fraction in the dictionary )
  create   here   frc% allot   frc-init
;


: frc-new          ( -- frc = Create a new fraction on the heap )
  frc% allocate  throw  dup frc-init
;


: frc-free         ( frc -- = Free the fraction from the heap )
  free throw 
;


( General module words )

: frc+calc-gcd     ( n1 n2 -- +n = Calculate the Greatest Common Divider )
  BEGIN                 \ BEGIN
    over mod            \   r = a % b
    ?dup
  WHILE                 \ WHILE r>0
    swap                \  a=b b=r
  REPEAT                \ REPEAT
  abs                   \ return b
;


: frc+calc-lcm     ( n1 n2 -- +n = Calculate the Least Common Multiplier )
  2dup frc+calc-gcd
  */                    \ a * b / gcd(a,b)
  abs
;


: frc+norm         ( n1 n2 -- n3 n4 = Normalize a fraction n1/n2, return n3/n4 )
  dup 0> 0= exp-invalid-parameters AND throw
  over 0= IF
    drop 1
  ELSE
    2dup frc+calc-gcd
    tuck /
    >r / r>
  THEN
;


( Calculation module words )

: frc+add          ( n1 n2 n3 n4 -- n5 n6 = Add two fractions n1/n2 and n3/n4, return n5/n6 )
  frc+norm 2swap 
  frc+norm                  \ Normalize both fractions
  rot
  2dup = IF                 \ denom1 = denom2 ?
    drop 
    >r + r>                 \   num = num1 + num2
  ELSE                      \ ELSE
    2dup * >r               \   denom = denom1 * denom2
    rot *
    >r * r> +               \   num = num1 * denom2 + num2 * denom1
    r>
  THEN
;


: frc+subtract     ( n1 n2 n3 n4 -- n5 n6 = Subtract fraction n3/n4 from fraction n1/n2, return n5/n6 )
  frc+norm 2swap 
  frc+norm 2swap            \ Normalize both fractions
  rot
  2dup = IF                 \ denom1 = denom2 ?
    drop 
    >r - r>                 \   num = num1 + num2
  ELSE                      \ ELSE
    2dup * >r               \   denom = denom1 * denom2
    rot *
    >r * r> -
    r>                      \   num = num2 * denom1 -- num1 * denom2
  THEN
;


: frc+multiply     ( n1 n2 n3 n4 -- n5 n6 = Multiply fraction n1/n2 by fraction n3/n4, return n5/n6 )
  frc+norm 2swap 
  frc+norm                   \ Normalize both fractions
  rot *                      \ denom = denom1 * denom2
  >r * r>                    \ num = num1 * num2
;


: frc+divide       ( n1 n2 n3 n4 -- n5 n6 = Divide fraction n1/n2 by fraction n3/n4, return n5/n6 )
  frc+norm 2swap 
  frc+norm 2swap             \ Normalize both fractions
  over 0= -10 AND throw      \ Divide by zero check
  >r * swap                  \ num   = num2 * denom1
  r> * swap                  \ denom = denom2 * num1 
  
  dup 0< IF                  \ numerator has the sign
    negate swap
    negate swap
  THEN
;


: frc+invert       ( n1 n2 -- n3 n4 = Invert the fraction n1/n2, return n3/n4 )
  frc+norm                   \ Normalize fraction 
  over 0= -10 AND throw      \ Divide by zero check
  swap                       \ invert
  
  dup 0< IF                  \ numerator has the sign
    negate swap
    negate swap
  THEN
;


: frc+negate       ( n1 n2 -- n3 n4 = Negate the fraction n1/n2, return n3/n4)
  frc+norm
  swap negate swap
;


: frc+abs          ( n1 n2 -- n3 n4 = Absolute the fraction n1/n2, return n3/n4 )
  frc+norm
  swap abs swap
;


( Conversion module words )

[DEFINED] d>f [DEFINED] f/ AND [IF]
: frc+to-float     ( n1 n2 -- r = Convert fraction n1/n2 to float value )
  >r s>d d>f r> s>d d>f f/
;
[THEN]


: frc+to-string    ( n1 n2 -- c-addr u = Convert fraction n1/n2 to a string using the pictured output area)
  frc+norm
  <#
  dup 1 <> IF
    s>d #s 2drop
    [char] / hold
  ELSE
    drop
  THEN
  dup abs s>d #s 
  rot sign
  #>
;


( Compare module words )

: frc+compare      ( n1 n2 n3 n4 -- n = Compare the fractions n1/n2 and n3/n4, return the result [-1,0,1] )
  frc+subtract drop sgn
;


( Structure words )

: frc-num@         ( frc -- n = Get the numerator )
  frc>num @
;


: frc-denom@       ( frc -- n = Get the denominator )
  frc>denom @
;


: frc-get          ( frc -- n1 n2 = Get the fraction n1/n2 )
  dup  frc-num@ 
  swap frc-denom@
;

  
: frc-set          ( n1 n2 frc = Normalize and set the fraction n1/n2 )
  >r
  frc+norm
  
  r@ frc>denom !
  r> frc>num   !
;


: frc^move         ( frc2 frc1 -- = Move frc2 in frc1 )
  over frc-num@   over frc>num !
  swap frc-denom@ swap frc>denom !
;


: frc^compare      ( frc2 frc1 -- n = Compare fraction2 with fraction1, return the result [-1,0,1] )
  >r frc-get 
  r> frc-get
  frc+compare
;


( Inspection )

: frc-dump         ( frc -- = Dump the fraction )
  ." frc:" dup . cr
  ."   num  :" dup frc>num   ? cr
  ."   denom:"     frc>denom ? cr
;

[THEN]

\ ==============================================================================
