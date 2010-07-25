\ ==============================================================================
\
\                a32 - the Adler32 algorithm in the ffl
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
\  $Date: 2008-04-05 08:05:28 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] a32.version [IF]


cell 4 >=  1 chars 1 =  AND [IF]

include ffl/stc.fs


( a32 = Adler32 Algorithm )
( The a32 module implements the Adler32 algorithm.)
  

1 constant a32.version


( Private constants )

65521 constant a32.base     ( -- u = Adler32 base )
2776  constant a32.nmax     ( -- n = Adler32 nmax )
hex
FFFF  constant a32.mask     ( -- u = 16 bit mask )
decimal


( Adler32 Structure )

begin-structure a32%        ( -- n = Get the required space for an Adler32 variable )
  field:  a32>sum
  field:  a32>length
end-structure


( Adler32 variable creation, initialisation and destruction )

: a32-init     ( a32 -- = Initialise the Adler32 variable )
  1 over a32>sum     !
         a32>length 0!
;


: a32-create   ( "<spaces>name" -- ; -- a32 = Create a named Adler32 variable in the dictionary )
  create   here   a32% allot   a32-init
;


: a32-new      ( -- a32 = Create a new Adler32 variable on the heap )
  a32% allocate  throw  dup a32-init
;


: a32-free     ( a32 -- = Free the Adler32 variable from the heap )
  free throw 
;


( Member word )

: a32-length@  ( a32 -- u = Get the length of the sum )
  a32>length @
;


( Adler32 words )

: a32-reset    ( a32 -- = Reset the Adler32 state )
  a32-init
;


: a32-update   ( c-addr u a32 -- = Update the Adler32 with more data c-addr u )
  >r
  dup r@ a32>length +!            \ track length
  r@ a32>sum @
  dup 16 rshift a32.mask AND      \ s2 = sum >> 16 in 16 bit
  swap a32.mask AND               \ s1 = sum in 16 bit
  2swap
  
  BEGIN
    ?dup
  WHILE                           \ while length > 0
    2>r
    2r@ a32.nmax min bounds DO    \ Update in blocks of nmax characters
      I c@ +
      tuck +
      swap
    1 chars +LOOP
    
    swap a32.base MOD             \ s2 %= a32.base
    swap a32.base MOD             \ s1 %= a32.base
    
    2r>
    dup a32.nmax min /string      \ max nmax processed
  REPEAT
  drop
  
  swap 16 lshift OR
  r> a32>sum !
;


: a32-finish   ( a32 -- u = Finish the Adler32 calculation, return the result u )
  a32>sum @
;


: a32^combine  ( a32 a32 -- u = Combine the twee Addler32 sums and return the combined sum u )
  over a32>sum @ >r
       a32>sum @ >r
  a32-length@
  a32.base mod                    \ rem  = a32-2.length mod base
  r@ a32.mask AND                 \ sum1 = a32-1 AND mask
  2dup * a32.base mod             \ sum2 = rem * sum1 mod base
  r> 16 rshift a32.mask AND +     \ sum2 += a32-1 >> 16 AND mask
  r@ 16 rshift a32.mask AND +     \ sum2 += a32-2 >> 16 AND mask
  a32.base + rot -                \ sum2 += base - rem
  swap r> a32.mask AND + 
  a32.base + 1-                   \ sum1 += a32-2 AND mask + BASE - 1
  dup a32.base u> IF              \ if sum1 > base Then sum1 -= base
    a32.base -
  THEN
  dup a32.base u> IF              \ if sum1 > base Then sum1 -= base
    a32.base -
  THEN
  swap 
  dup a32.base 1 lshift u> IF     \ if sum2 > (base << 1) Then sum2 -= (base << 1)
    a32.base 1 lshift -
  THEN
  dup a32.base u> IF              \ if sum2 > base Then sum2 -= base
    a32.base -
  THEN
  16 lshift OR                    \ result = )sum2 << 16) or sum1
;


: a32+to-string  ( u -- c-addr u = Convert the Adler32 result to a string, using the pictured output area )
  base @ >r hex
  0 <# #s #>
  r> base !
;


( Inspection )

: a32-dump       ( a32 -- = Dump the Adler32 variable )
  ." a32:" dup . cr
  ."  sum   :" dup a32>sum    ? cr
  ."  length:"     a32>length ? cr
;

[ELSE]
.( Warning: a32 requires at least 4 byte cells and 1 byte chars ) cr
[THEN]

[THEN]

\ ==============================================================================

