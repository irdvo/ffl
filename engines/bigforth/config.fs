\ ==============================================================================
\
\                  config - the config in the ffl
\
\            Copyright (C) 2005-2007  Dick van Oudheusden
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
\  $Date: 2007-06-08 06:28:29 $ $Revision: 1.12 $
\
\ ==============================================================================
\
\ This file is for bigforth.
\
\ ==============================================================================


s" ffl.version" forth-wordlist search-wordlist 0= [IF]


( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)


000500 constant ffl.version



( Extra includes )

import float

float also


( Private words )
  
variable ffl.endian   1 ffl.endian !


( System Settings )

create end-of-line     ( - c-addr = Counted string for the end of line for the current system )
  1 c, 10 c,         \ unix: lf
\ 2 c, 13 c, 10 c,   \ dos:  cr lf


s" ADDRESS-UNIT-BITS" environment? 0= [IF] 8 [THEN] 
  constant #bits/byte   ( - n = Number of bits in a byte )
  
#bits/byte 1 chars *
  constant #bits/char   ( - n = Number of bits in a char )
  
#bits/byte cell *
  constant #bits/cell   ( - n = Number of bits in a cell )  

ffl.endian c@ 0=             
  constant bigendian?   ( - f = Check for bigendian hardware )


( Extension words )

: [DEFINED] 
  bl word find   nip 
; immediate


: [UNDEFINED] 
  bl word find   nip 0= 
; immediate


: ms@                                                    ( - u = Fetch milliseconds timer )
  utime 1 1000 m*/ drop 
;


s" MAX-U" environment? drop constant max-ms@            ( - u = Maximum value of the milliseconds timer )

: 2+               ( n - n+2 = Add two to tos)
  1+ 1+
;


: ms@              ( - u = Fetch milliseconds timer )
  msecs
;


s" MAX-U" environment? drop constant max-ms@  ( - u = Maximum value of the milliseconds timer )


1 chars 1 = [IF]
: char/            ( n:aus - n:chars = Convert address units to chars )
; immediate
[ELSE]
: char/
  1 chars /
;
[THEN]


: lroll            ( u1 u - u2 = Rotate u1 u bits to the left )
  2dup lshift >r
  #bits/cell swap - rshift r>
  or
;


: rroll            ( u1 u - u2 = Rotate u1 u bits to the right )
  2dup rshift >r
  #bits/cell swap - lshift r>
  or
;


: d<>              ( d d - f = Check if two two double are unequal )
  d= 0=
;


: du<>             ( ud ud - f = Check if two unsigned doubles are unequal )
  d<>
;


: sgn              ( n - n = Determine the sign of the number )
  dup 0= IF 
    EXIT 
  THEN
  0< 2* 1+
;


0 constant nil     ( - w = Nil address )


: 0!               ( w - = Set zero in address )
  0 swap !
;


: nil!             ( w - = Set nil in address )
  nil swap !
;


: nil=             ( w - f = Check for nil )
  nil =
;


: nil<>            ( w - f = Check for unequal to nil )
  nil <>
;


: ?free            ( addr - = Free the address [and throw] if not nil )
  dup nil<> IF
    free throw
  ELSE
    drop
  THEN
;


: 1+!              ( w - = Increase contents of address by 1 )
  1 swap +!
;


: 1-!              ( w - = Decrease contents of address by 1 )
  -1 swap +!
;


: u<>
  <>
;


: @!               ( w a - w = First fetch the contents and then store the new value )
  dup @ -rot !
;


: <=>              ( n n - n = Compare two numbers and return the compare result [-1,0,1] )
  2dup = IF 
    2drop 0 EXIT 
  THEN
  < 2* 1+
;


: index2offset     ( n:index n:length - n:offset = Convert an index [-length..length> into an offset [0..length> )
  over 0< IF
    +
  ELSE
    drop
  THEN
;


[DEFINED] floats [IF]

( Float extension constants )

0e0 fconstant 0e0  ( - r:0e0 = Float constant 0.0 )

1e0 fconstant 1e0  ( - r:1e0 = Float constant 1.0 )

2e0 fconstant 2e0  ( - r:2e0 = Float constant 2.0 )


( Float extension words )

: f2dup            ( r1 r2 - r1 r2 r1 r2 = Duplicate two floats )
  fover fover
;


: ftuck            ( r1 r2 - r2 r1 r2 = Swap and over )
  fswap fover
;

[THEN]


( Exceptions )

variable exp-next  -2050 exp-next !

: exception      ( w:addr u - n = add an exception )
  2drop
  exp-next @ 
  exp-next 1-!
;


s" Index out of range" exception constant exp-index-out-of-range ( - n = Index out of range exception number )
s" Invalid state"      exception constant exp-invalid-state      ( - n = Invalid state exception number )
s" No data available"  exception constant exp-no-data            ( - n = No data available exception number )
s" Invalid parameters" exception constant exp-invalid-parameters ( - n = Invalid parameters on stack )

[ELSE]
  drop
[THEN]

\ ==============================================================================

