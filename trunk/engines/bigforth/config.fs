\ ==============================================================================
\
\                  config - the config in the ffl
\
\            Copyright (C) 2005-2007  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU Lesser General Public
\ License as published by the Free Software Foundation; either
\ version 3 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ Lesser General Public License for more details.
\
\ You should have received a copy of the GNU Lesser General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2009-05-20 13:27:22 $ $Revision: 1.22 $
\
\ ==============================================================================
\
\ This file is for bigforth.
\
\ ==============================================================================


s" ffl.version" forth-wordlist search-wordlist 0= [IF]


( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)


000900 constant ffl.version



( Extra includes )

import float

float also


( Private words )
  
variable ffl.endian   1 ffl.endian !


( System Settings )

create end-of-line     ( -- c-addr = Counted string for the end of line for the current system )
  1 c, 10 c,         \ unix: lf
\ 2 c, 13 c, 10 c,   \ dos:  cr lf


s" ADDRESS-UNIT-BITS" environment? 0= [IF] 8 [THEN] 
  constant #bits/byte   ( -- +n = Number of bits in a byte )
  
#bits/byte 1 chars *
  constant #bits/char   ( -- +n = Number of bits in a char )
  
#bits/byte cell *
  constant #bits/cell   ( -- +n = Number of bits in a cell )  

ffl.endian c@ 0=             
  constant bigendian?   ( -- flag = Check for bigendian hardware )


( Extension words )

: [DEFINED] 
  bl word find   nip 
; immediate


: [UNDEFINED] 
  bl word find   nip 0= 
; immediate


: ms@                                                   ( -- u = Fetch milliseconds timer )
  timer@ >us 1 1000 m*/ drop 
;


s" MAX-U" environment? drop constant max-ms@            ( -- u = Maximum value of the milliseconds timer )


1 chars 1 = [IF]
: char/            ( n1 -- n2 = Convert n1 address units to n2 chars )
; immediate
[ELSE]
: char/
  1 chars /
;
[THEN]


: lroll            ( u1 u2 -- u3 = Rotate u1 u2 bits to the left )
  2dup lshift >r
  #bits/cell swap - rshift r>
  or
;


: rroll            ( u1 u2 -- u3 = Rotate u1 u2 bits to the right )
  2dup rshift >r
  #bits/cell swap - lshift r>
  or
;


: d<>              ( d1 d2 -- flag = Check if two two double are unequal )
  d= 0=
;


: sgn              ( n1 -- n2 = Determine the sign of the number, return [-1,0,1] )
  -1 max 1 min
;


0 constant nil     ( -- addr = Nil address )


: 0!               ( a-addr -- = Set zero in address )
  0 swap !
;


: nil!             ( a-addr -- = Set nil in address )
  nil swap !
;


: nil=             ( addr -- flag = Check for nil )
  nil =
;


: nil<>            ( addr -- flag = Check for unequal to nil )
  nil <>
;


0 nil= [IF]
: nil<>?           ( addr -- false | addr true = If addr is nil, then return false, else return address with true )
  state @ IF
    postpone ?dup
  ELSE
    ?dup
  THEN
; immediate
[ELSE]
: nil<>?
  dup nil<> IF
    true
  ELSE
    drop
    false
  THEN
;
[THEN]  


: ?free            ( addr -- wior = Free the address if not nil )
  dup nil<> IF
    free 
  ELSE
    drop 0
  THEN
;


: 1+!              ( a-addr -- = Increase contents of address by 1 )
  1 swap +!
;


: 1-!              ( a-addr -- = Decrease contents of address by 1 )
  -1 swap +!
;


: u<>
  <>
;


: @!               ( x1 a-addr -- x2 = First fetch the contents x2 and then store the new value x1 )
  dup @ -rot !
;


: icompare         ( c-addr1 u1 c-addr2 u2 -- n = Compare case-insensitive two strings )
  caps @ >r
  caps on
  compare
  r> caps !
;


: <=>              ( n1 n2 -- n3 = Compare the two numbers n1,n2 and return the compare result [-1,0,1] )
  2dup = IF 
    2drop 0 EXIT 
  THEN
  < 2* 1+
;


: index2offset     ( n1 n2 -- n3 = Convert the index n1 [-length..length> with length n2 into the offset n3 [0..length> )
  over 0< IF
    +
  ELSE
    drop
  THEN
;


: r'@              ( R: x1 x2 -- x1 x2; -- x1 = Fetch the second cell on the return stack )
  postpone 2r@ postpone drop
; immediate


[DEFINED] floats [IF]

( Float extension constants )

0E+0 fconstant 0e+0  ( F: -- r = Float constant 0.0 )
1E+0 fconstant 1e+0  ( F: -- r = Float constant 1.0 )
2E+0 fconstant 2e+0  ( F: -- r = Float constant 2.0 )


( Float extension words )

: f2dup            ( F: r1 r2 -- r1 r2 r1 r2 = Duplicate two floats )
  fover fover
;


: ftuck            ( F: r1 r2 -- r2 r1 r2 = Swap and over )
  fswap fover
;

[THEN]


( Exceptions )

variable exp-next  -2050 exp-next !

: exception      ( c-addr u -- n = Create an exception, return its number )
  2drop
  exp-next @ 
  exp-next 1-!
;


s" Index out of range" exception constant exp-index-out-of-range ( -- n = Index out of range exception number )
s" Invalid state"      exception constant exp-invalid-state      ( -- n = Invalid state exception number )
s" No data available"  exception constant exp-no-data            ( -- n = No data available exception number )
s" Invalid parameters" exception constant exp-invalid-parameters ( -- n = Invalid parameters on stack )
s" Wrong file type"    exception constant exp-wrong-file-type    ( -- n = Wrong file type )
s" Wrong file version" exception constant exp-wrong-file-version ( -- n = Wrong file version )
s" Wrong file data"    exception constant exp-wrong-file-data    ( -- n = Wrong file data )
s" Wrong checksum"     exception constant exp-wrong-checksum     ( -- n = Wrong checksum )
s" Wrong length"       exception constant exp-wrong-length       ( -- n = Wrong length )
s" Invalid data"       exception constant exp-invalid-data       ( -- n = Invalid data exception number )

[ELSE]
  drop
[THEN]

\ ==============================================================================

