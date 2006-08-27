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
\  $Date: 2006-08-27 18:02:15 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] frc.version [IF]


include ffl/stc.fs


( frc = Fraction module )
( The frc module implements words for using fractions. Note: to reduce normalizing of fractions,)
( the input is validated and normalized, but the resulting output is NOT normalized. This is left)
( to the next fraction word.)


1 constant frc.version

  
( Fraction structure )

struct: frc%       ( - n = Get the required space for the frc data structure )
  cell: frc>num         \ numerator
  cell: frc>denom       \ denominator
;struct



( Structure creation, initialisation and destruction )

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


( General module words )

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


: frc+norm         ( n:num n:denom - n:num n:denom = Normalize a fraction on stack )
  dup 0> 0= exp-invalid-parameters AND throw
  2dup frc+calc-gcd
  tuck /
  >r / r>
;


( Calculation module words )

: frc+add          ( n:num2 n:denom2 n:num1 n:denom1 - n:num n:denom = Add two fractions on stack )
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


: frc+subtract     ( n:num2 n:denom2 n:num1 n:denom1 - n:num n:denom = Subtract fraction1 from fraction2 on stack )
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
    r>                      \   num = num2 * denom1 - num1 * denom2
  THEN
;


: frc+multiply     ( n:num2 n:denom2 n:num1 n:denom1 - n:num n:denom = Multiply fraction1 by fraction2 on stack )
  frc+norm 2swap 
  frc+norm                   \ Normalize both fractions
  rot *                      \ denom = denom1 * denom2
  >r * r>                    \ num = num1 * num2
;


: frc+divide       ( n:num2 n:denom2 n:num1 n:denom1 - n:num n:denom = Divide fraction1 by fraction2 on stack )
  frc+norm 2swap 
  frc+norm 2swap             \ Normalize both fractions
  over 0= -10 AND throw      \ Divide by zero check
  >r * swap                  \ num   = num2 * denom1
  r> * swap                  \ denom = denom2 * num1 
;


: frc+invert       ( n:num n:denom - n:num n:denom = Invert the fraction on stack)
  frc+norm                   \ Normalize fraction 
  over 0= -10 AND throw      \ Divide by zero check
  swap                       \ invert
;


: frc+negate       ( n:num n:denom - n:num n:denom = Negate the fraction on stack)
  frc+norm
  swap negate swap
;


: frc+abs          ( n:denom n:num - n:denom n:num = Absolute the fraction on stack )
  frc+norm
  swap abs swap
;


( Conversion module words )

[DEFINED] d>f [DEFINED] f/ AND [IF]
: frc+to-float     ( n:num n:denom - r = Convert fraction to float value )
  >r s>d d>f r> s>d d>f f/
;
[THEN]


: frc+to-string    ( n:num n:denom - c-addr u = Convert fraction to a string )
  frc+norm
  <#
  dup 1 <> IF
    s>d #s 2drop
    [char] / hold
  ELSE
    drop
  THEN
  dup abs s>d # 
  rot sign
  #>
;


( Compare module words )

: frc+compare      ( n:num2 n:denom2 n:num1 n:denom1 - n = Compare two fractions on stack )
  frc+subtract drop sgn
;


( Structure words )

: frc-num@         ( w:frc - n:numerator = Get the numerator )
  frc>num @
;


: frc-denom@       ( w:frc - n:denominator = Get the denominator )
  frc>denom @
;


: frc-get          ( w:frc - n:num n:denom = Get the fraction )
  dup  frc-num@ 
  swap frc-denom@
;

  
: frc-set          ( n:num n:denom w:frc = Normalize and set the fraction )
  >r
  frc+norm
  
  r@ frc>denom !
  r> frc>num   !
;


: frc^move         ( w:frc2 w:frc1 - = Move frc2 in frc1 )
  over frc-num@   over frc>num !
  swap frc-denom@ swap frc>denom !
;


: frc^compare      ( w:frc2 w:frc1 - n = Compare fraction2 with fraction1 )
  >r frc-get 
  r> frc-get
  frc+compare
;


: frc-dump         ( w:frc - = Dump the fraction )
  ." frc:" dup . cr
  ."   num  :" dup frc>num   ? cr
  ."   denom:"     frc>denom ? cr
;

[THEN]

\ ==============================================================================
