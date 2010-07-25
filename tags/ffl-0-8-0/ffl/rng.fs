\ ==============================================================================
\
\       rng - the pseudo random number generator module in the ffl
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
\  $Date: 2007-12-09 07:23:16 $ $Revision: 1.3 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] rng.version [IF]


cell 4 >= [IF]

include ffl/stc.fs


( rng = Pseudo random number generator module )
( The rng module implements a pseudo random number generator;             )
( it uses the Mersenne Twister as generator with a period of 2^19937 - 1. )

\
\ The module is inspired by a C-program for MT19937:
\
\ (C) 1997 - 2002 Takuji Nishimura and Makoto Matsumoto. 
\ All rights reserved
\

1 constant rng.version


( Private words )

hex
FFFFFFFF constant rng.mask  ( -- u = 32 bit mask )
decimal

624      constant rng.n     ( -- n = MT n value )
397      constant rng.m     ( -- n = MT m value )


( Random generator structure )

begin-structure rng%       ( -- n = Get the required space for a rng variable )
  field:  rng>mti             \ mt index 
  rng.n
  fields: rng>mt              \ mt state
end-structure


( Private words )

: rng-init-mt      ( u rng -- = Initialise the mt with the seed u )
  swap rng.mask AND          \ only 32 bits in seed
  over rng>mt
  2dup !                     \ mt[0] = seed
  swap
  rng.n 1 DO                 \ S: rng mt-1 mt[-1]
    dup 30 rshift XOR        \ mt[-1] ^ mt[-1] >> 30
    1812433253 *             \  * 1812433253 + index
    I +
    rng.mask AND             \ only 32 bits
    swap
    cell+ 2dup !             \ save in mt
    swap
  LOOP
  2drop
  rng.n swap rng>mti !
;

    
hex
: rng-twist        ( u1 u2 u3 -- u4 = Twist values )
       80000000 AND          
  swap 7FFFFFFF AND OR       
  tuck 1 rshift XOR         
  swap 1 AND 0<> 
  9908B0DF AND XOR
;
decimal


: rng-refill-mt    ( rng -- = Refill the mt array )
  dup rng>mti 0!
  rng>mt 
  [ rng.n rng.m - cells ] literal over + over DO
    I rng.m cells + @
    I cell+ @
    I @
    rng-twist
    I !
    cell
  +LOOP
  
  [ rng.n 1- cells ] literal over + over [ rng.n rng.m - cells ] literal + DO
    I [ rng.m rng.n - cells ] literal + @
    I cell+ @
    I @
    rng-twist
    I !
    cell
  +LOOP
  
  [ rng.n 1- cells ] literal over + dup @   \ mt[n-1]
  rot dup [ rng.m 1- cells ] literal + @    \ mt[m-1]
  -rot @ swap                               \ mt[0]
  rng-twist
  swap !                                    \ mt[n-1]
;


( Random generator creation, initialisation and destruction )

: rng-init         ( u rng -- = Initialise the generator with the seed u )
  rng-init-mt
;


: rng-create       ( u "<spaces>name" -- ; -- rng = Create a named random generator in the dictionary with seed u )
  create   here   rng% allot   rng-init
;


: rng-new          ( u -- rng = Create a new random generator on the heap with seed u )
  rng% allocate  throw  tuck rng-init
;


: rng-free         ( rng -- = Free the random generator from the heap )
  free throw 
;


( Random generator words )

: rng-seed          ( u rng -- = Initialise the generator with the seed u )
  rng-init-mt
;


: rng-next-number  ( rng -- n = Calculate the next pseudo random number, 32 bit )
  >r
  r@ rng>mti @ rng.n >= IF  \ Check for refill of mt 
    r@ rng-refill-mt
  THEN
  r@ rng>mt  
  r@ rng>mti @ cells +      \ Get the rnd value
  @
  r> rng>mti 1+!            \ increment the state index
                            \ Start tempering
  dup 11 rshift XOR
  dup  7 lshift 2636928640 AND XOR
  dup 15 lshift 4022730752 AND XOR
  dup 18 rshift XOR
;


[DEFINED] f/ [IF]
: rng-next-float   ( rng -- r = Calculate the next pseudo random float number, range [0,1> )
  rng-next-number 0 d>f 4294967296e0 f/
;
[THEN]


( Inspection )

: rng-dump         ( rng -- = Dump the random generator )
  ." rng:" dup . cr
  ."   mti:" dup rng>mti ? cr
  ."   mt:"      rng>mt  rng.n cells dump cr
;

[ELSE]
.( Warning: rng requires at least 4 byte cells ) cr
[THEN]
[THEN]

\ ==============================================================================
