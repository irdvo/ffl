\ ==============================================================================
\
\                  config - the config in the ffl
\
\             Copyright (C) 2005-2007  Dick van Oudheusden
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
\  $Date: 2008-03-23 15:27:01 $ $Revision: 1.51 $
\
\ ==============================================================================
\
\ This file is for gforth.
\
\ ==============================================================================


s" ffl.version" forth-wordlist search-wordlist 0= [IF]


( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)


000700 constant ffl.version


( Private words )
  
variable ffl.endian   1 ffl.endian !


( System Settings )

create end-of-line    ( -- c-addr = Counted string for the end of line for the current system )
  newline string,    \ All hosts except dos  (gforth 0.6.2)
\ 2 c, 13 c, 10 c,   \ dos:  cr lf


s" ADDRESS-UNIT-BITS" environment? 0= [IF] 8 [THEN] 
  constant #bits/byte   ( -- n = Number of bits in a byte )
  
#bits/byte 1 chars *
  constant #bits/char   ( -- n = Number of bits in a char )
  
#bits/byte cell *
  constant #bits/cell   ( -- n = Number of bits in a cell )  

ffl.endian c@ 0=             
  constant bigendian?   ( -- flag = Check for bigendian hardware )


( Extension words )

: ms@  ( -- u = Fetch milliseconds timer )
  utime 1 1000 m*/ drop 
;


s" MAX-U" environment? drop constant max-ms@   ( -- u = Maximum value of the milliseconds timer )


\ No usable command line arguments in gforth


: lroll   ( u1 u2 -- u3 = Rotate u1 u2 bits to the left )
  2dup lshift >r
  #bits/cell swap - rshift r>
  or
;


: rroll   ( u1 u2 -- u3 = Rotate u1 u2 bits to the right )
  2dup rshift >r
  #bits/cell swap - lshift r>
  or
;


: 0!   ( a-addr -- = Set address to zero )
  0 swap !
;


: nil!   ( a-addr -- = Set address to nil )
  nil swap !
;


: nil=   ( addr -- flag = Check for nil )
  nil =
;


: nil<>   ( addr -- flag = Check for unequal to nil )
  nil <>
;


0 nil= [IF]
: nil<>?    ( addr -- false | addr true = If addr is nil, then return false, else return address with true )
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


: ?free   ( addr -- ior = Free the address if not nil )
  dup nil<> IF
    free
  ELSE
    drop 0
  THEN
;


: 1+!   ( a-addr -- = Increase contents of address by 1 )
  1 swap +!
;


: 1-!   ( a-addr -- = Decrease contents of address by 1 )
  -1 swap +!
;


: @!   ( x1 a-addr -- x2 = First fetch the contents x2 and then store value x1 )
  dup @ -rot !
;


: icompare   ( c-addr1 u1 c-addr2 u2 -- n = Compare case-insensitive two strings and return the result [-1,0,1] )
  rot swap 2swap 2over
  min 0 ?DO
    over c@ toupper over c@ toupper - sgn ?dup IF
      >r 2drop 2drop r>
      unloop 
      exit
    THEN
    1 chars + swap 1 chars + swap
  LOOP
  2drop
  - sgn
;


: <=>   ( n1 n2 -- n = Compare the two numbers and return the compare result [-1,0,1] )
  2dup = IF 
    2drop 0 EXIT 
  THEN
  < 2* 1+
;

      
: index2offset   ( n1 n2 -- n3 = Convert the index n1 range [-n2..n2> into offset n3 range [0..n2>, negative values of n1 downward length n2 )
  over 0< IF
    +
  ELSE
    drop
  THEN
;


: >,"              ( c-addr1 -- c-addr2 = Move to the next string, stored by ," )
  count chars + aligned
;

[DEFINED] float [IF]

( Float extension constants )

0E+0 fconstant 0e+0  ( F: -- r = Float constant 0.0 )
1E+0 fconstant 1e+0  ( F: -- r = Float constant 1.0 )
2E+0 fconstant 2e+0  ( F: -- r = Float constant 2.0 )


( Float extension words )

: f-rot            ( F: r1 r2 r3 -- r3 r1 r2 = Rotate counter clockwise three floats )
  frot frot
;


: f2dup            ( F: r1 r2 -- r1 r2 r1 r2 = Duplicate two floats )
  fover fover
;


: f>r              ( F: r -- ; R: -- r = Push float on the return stack )
  r> rp@ float - rp! rp@ f! >r 
;

: fr>              ( F: -- r ; R: r -- = Pop float from the return stack )
  r> rp@ f@ float rp@ + rp! >r
;

: fr@              ( F: -- r ; R: r -- r = Get float from top of return stack )
  r> rp@ f@ >r
;

[THEN]


( Exceptions )

s" Index out of range" exception constant exp-index-out-of-range ( -- n = Index out of range exception number )
s" Invalid state"      exception constant exp-invalid-state      ( -- n = Invalid state exception number )
s" No data available"  exception constant exp-no-data            ( -- n = No data available exception number )
s" Invalid parameters" exception constant exp-invalid-parameters ( -- n = Invalid parameters on stack )
s" Wrong file type"    exception constant exp-wrong-file-type    ( -- n = Wrong file type )
s" Wrong file version" exception constant exp-wrong-file-version ( -- n = Wrong file version )

[ELSE]
  drop
[THEN]

\ ==============================================================================

