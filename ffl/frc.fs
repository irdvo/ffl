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
\  $Date: 2006-08-27 07:06:44 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] frc.version [IF]


include ffl/stc.fs


( frc = Fraction module )
( The frc module implements words for using fractions.)
  

1 constant frc.version

  
( Public structure )

struct: frc%       ( - n = Get the required space for the frc data structure )
  cell: frc>num         \ numerator
  cell: frc>denom       \ denominator
;struct


( Private words )


( Public words )

: frc-init         ( w:frc - = Initialise the fraction to zero )
  dup    frc>num 0!
  1 swap frc>denom !
;


: frc-create       ( "name" - = Create a named fraction in the dictionary )
  create   here   frc% allot   frc-init
;


: frc-new          ( - w:frc = Create a new fraction on the heap )
  frc% allocate  throw  dup frc-init
;


: frc-free         ( w:frc - = Free the fraction from the heap )
  free throw 
;


: frc+calc-gcd     ( n1 n2 - n:gcd = Calculate the Greatest Common Divider )
  abs swap abs          \ both positive
  
  2dup < IF             \ smallest on top
    swap
  THEN
  
  BEGIN                 \ BEGIN
    2dup mod            \   r = a % b
    ?dup
  WHILE                 \ WHILE r>0
    rot drop            \  a=b b=r
  REPEAT                \ REPEAT
  nip                   \ return b
;


: frc+calc-lcm     ( n1 n2 - n:lcm = Calculate the Least Common Multiplier )
  abs swap abs          \ both positive
  2dup frc+calc-gcd
  >r * r> /             \ a * b / gcd(a,b)
;


: frc+norm         ( n:denom n:num - n:denom n:num = Normalize a fraction on stack )
  over 0> 0= exp-invalid-parameters AND throw
  
  2dup frc+calc-gcd
  tuck /
  >r / r>
;


( Member words )

: frc-num@         ( w:frc - n:numerator = Get the numerator )
  frc>num @
;


: frc-denom@       ( w:frc - n:denominator = Get the denominator )
  frc>denom @
;


: frc-get          ( w:frc - n:denom n:num = Get the fraction )
  dup  frc-denom@ 
  swap frc-num@
;


: frc-norm         ( w:frc - = Normalize the fraction )
  >r
  
  r@ frc-denom@ r@ frc-num@ frc+calc-gcd
  
  r@ frc-denom@ over / r@ frc>denom !
  r@ frc-num@   swap / r> frc>num   !
;

  
: frc-set          ( n:denom n:num w:frc = Set the fraction )
  rot
  dup 0> 0= exp-invalid-parameters AND throw
  
  over frc>denom !
  tuck frc>num   !
  frc-norm
;


: frc^move         ( w:frc2 w:frc1 - = Move frc2 in frc1 )
  over frc-num@   over frc>num !
  swap frc-denom@ swap frc>denom !
;


( Private words )

: frc-add-norm     ( n:denom n:num w:frc - = Add normalized num/denom to fraction )
  >r
  over r@ frc-denom@ = IF
    nip r@ frc>num +!
  ELSE
    r@ frc-denom@ * over r@ frc-num@ * + r@ frc>num !
    r@ frc-denom@ * r@ frc>denom !
  THEN
  
  r> frc-norm
;


: frc-sub-norm     ( n:denom n:num w:frc - = Subtract normalized num/denom from fraction )
  >r
  over r@ frc-denom@ = IF
    nip negate r@ frc>num +!
  ELSE
    r@ frc-denom@ * over r@ frc-num@ * swap  - r@ frc>num !
    r@ frc-denom@ * r@ frc>denom !
  THEN
  
  r> frc-norm
;


( Calculation words )

: frc+add          ( n:denom2 n:num2 n:denom1 n:num1 - n:denom n:num = Add two fractions on stack )
  frc+norm 2swap frc+norm   \ Normalize both fractions
  >r rot
  2dup = IF                 \ denom1 = denom2 ?
    drop swap
    r> +                    \   num = num1 + num2
  ELSE                      \ ELSE
    2dup *                  \   denom = denom1 * denom2
    r> swap >r
    * >r * r> +             \   num = num1 * denom2 + num2 * denom1
    r> swap
  THEN
  frc+norm                  \ Normalize the result
;


: frc-add          ( n:denom n:num w:frc - = Add num/denom to fraction )
  >r frc+norm r> frc-add-norm
;


: frc^add          ( w:frc2 w:frc1 - = Add fraction 2 to fraction 1)
  >r frc-get r> frc-add-norm
;


: frc+subtract     ( n:denon2 n:num2 n:denom1 n:num1 - n:denom n:num = Subtract fraction2 from fraction1 on stack )
  frc+norm 2swap frc+norm   \ Normalize both fractions
  >r rot
  2dup = IF                 \ denom1 = denom2 ?
    drop swap
    r> -                    \   num = num1 + num2
  ELSE                      \ ELSE
    2dup *                  \   denom = denom1 * denom2
    r> swap >r
    * >r * r> -             \   num = num1 * denom2 - num2 * denom1
    r> swap
  THEN
  frc+norm                  \ Normalize the result
;


: frc-subtract     ( n:denom n:num w:frc - = Subtract num/denom from fraction )
  >r frc+norm r> frc-sub-norm
;


: frc^subtract     ( w:frc2 w:frc1 - = Subtract fraction2 from fraction 1)
  >r frc-get r> frc-sub-norm
;


: frc-multiply     ( n:denom n:num w:frc - = Multiply fraction by num/denom )
  >r
  frc+norm
  r@ frc-num@   * r@ frc>num   !
  r@ frc-denom@ * r@ frc>denom !
  
  r> frc-norm
;


: frc^multiply     ( w:frc2 w:frc1 - = Multiply fraction1 by fraction2 )
  >r
  frc-get
  r@ frc-num@   * r@ frc>num   !
  r@ frc-denom@ * r@ frc>denom !
  
  r> frc-norm
;


: frc-divide       ( n:denom n:num w:frc - = Divide fraction by num/denom )
;


: frc^divide       ( w:frc2 w:frc1 - = Divide fraction1 by fraction 2 )
  >r frc-get r> frc-divide
;


: frc-invert       ( w:frc - = Invert the fraction )
;


: frc-negate       ( w:frc - = Negate the fraction )
  
;


: frc-abs          ( w:frc - = Absolute the fraction )
;


( Conversion words )

\ conditional
: frc-to-float     ( w:frc - r = Convert to float value )
;


: frc-to-string    ( w:frc - c-addr u = Convert to a string )
;


( Compare words )

: frc-compare      ( n:denom n:num w:frc - n = Compare fraction with num/denom )
;


: frc^compare      ( w:frc2 w:frc1 - n = Compare fraction1 with fraction 2 )
  >r frc-get r> frc-compare
;


( Inspection )

: frc-dump         ( w:frc - = Dump the fraction )
  ." frc:" dup . cr
  ."   num  :" dup frc>num   ? cr
  ."   denom:"     frc>denom ? cr
;

[THEN]

\ ==============================================================================
