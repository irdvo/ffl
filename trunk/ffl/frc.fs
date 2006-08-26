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
\  $Date: 2006-08-26 19:14:19 $ $Revision: 1.1 $
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


: frc+gcd          ( n:denom n:num - n:gcd = Calculate the Greatest Common Divider )
  2drop                 \ Todo
  1
;


: frc-norm         ( w:frc - = Normalize the fraction )
  >r
  
  r@ frc-denom@ r@ frc-num@ frc+gcd
  
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


( Calculation words )

: frc-add          ( n:denom n:num w:frc - = Add num/denom to fraction )
  >r
  swap
  dup 0> 0= exp-invalid-parameters AND throw
  dup r@ frc-denom@ = IF
    drop r@ frc>num +!
  ELSE
    tuck r@ frc-num@ * swap r@ frc-denom@ * + r@ frc>num !
    r@ frc-denom@ * r@ frc>denom !
  THEN
  
  r> frc-norm
;


: frc^add          ( w:frc2 w:frc1 - = Add fraction 2 to fraction 1)
  >r frc-get r> frc-add
;


: frc-subtract     ( n:denom n:num w:frc - = Subtract num/denom from fraction )
  >r
  swap
  dup 0> 0= exp-invalid-parameters AND throw
  dup r@ frc-denom@ = IF
    drop negate r@ frc>num +!
  ELSE
    tuck r@ frc-num@ * swap r@ frc-denom@ * - r@ frc>num !
    r@ frc-denom@ * r@ frc>denom !
  THEN
  
  r> frc-norm
;


: frc^subtract     ( w:frc2 w:frc1 - = Subtract fraction2 from fraction 1)
  >r frc-get r> frc-subtract
;


: frc-multiply     ( n:denom n:num w:frc - = Multiply fraction by num/denom )
;


: frc^multiply     ( w:frc2 w:frc1 - = Multiply fraction1 by fraction2 )
  >r frc-get r> frc-multiply
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
