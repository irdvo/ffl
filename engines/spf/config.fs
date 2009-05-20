\ ==============================================================================
\
\                  config - the config in the ffl
\
\             Copyright (C) 2005-2007  Dick van Oudheusden
\                    Copyright (C) 2008 ygrek
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
\  $Date: 2009-05-20 13:27:22 $ $Revision: 1.8 $
\
\ ==============================================================================
\
\ This file is for SP-Forth.
\
\ ==============================================================================

REQUIRE [IF]      lib/include/tools.f

S" ffl.version" FORTH-WORDLIST SEARCH-WORDLIST 0= [IF]

REQUIRE ANSI-FILE lib/include/ansi-file.f
REQUIRE F.        lib/include/float2.f
REQUIRE .R        lib/include/core-ext.f
REQUIRE /STRING   lib/include/string.f
REQUIRE DEFER     lib/include/defer.f
REQUIRE M*/       lib/include/double.f
REQUIRE CASE      lib/ext/case.f
REQUIRE TIME&DATE lib/include/facil.f

\ REQUIRE COMPARE-U ~ac/lib/string/compare-u.f
[UNDEFINED] CHAR-UPPERCASE [IF]
: CHAR-UPPERCASE ( c -- c1 )
  DUP [CHAR] a [CHAR] z 1+ WITHIN IF 32 - EXIT THEN
  DUP [CHAR] а [CHAR] я 1+ WITHIN IF 32 - THEN
;
[THEN]

REQUIRE CASE-INS  lib/ext/caseins.f

CASE-INS ON

( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)

000600 constant ffl.version


( Private words )
  
variable ffl.endian   1 ffl.endian !


( System Settings )

create end-of-line    ( -- c-addr = Counted string for the end of line for the current system )
EOLN S",  

s" ADDRESS-UNIT-BITS" environment? 0= [IF] 8 [THEN] 
  constant #bits/byte   ( -- n = Number of bits in a byte )
  
#bits/byte 1 chars *
  constant #bits/char   ( -- n = Number of bits in a char )
  
#bits/byte cell *
  constant #bits/cell   ( -- n = Number of bits in a cell )  

ffl.endian c@ 0=             
  constant bigendian?   ( -- flag = Check for bigendian hardware )


( Extension words )

[UNDEFINED] ms@ [IF]

 \ assume windows
[UNDEFINED] GetTickCount [IF]
 WINAPI: GetTickCount KERNEL32.DLL
[THEN]

: ms@ ( -- u ) GetTickCount  ;

[THEN]


s" MAX-U" environment? 0= [IF] -1 [THEN] constant max-ms@   ( -- u = Maximum value of the milliseconds timer )


\ Command line is a single string in SPF/Win32
[DEFINED] ARGC [IF] \ available in SPF/Linux

: #args            ( -- n = Get the number of command line arguments )
  ARGC 1-
;

: arg@             ( n -- c-addr u = Get the nth command line argument )
  1+ CELLS ARGV + @ ASCIIZ>
;
[THEN]



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

: include PARSE-NAME ANSI-FILE::>ZFILENAME INCLUDED ;

: SGN -1 MAX 1 MIN ;

: MS PAUSE ;

: BIN ;

: CHAR/ 1 CHARS / ;

: 0>= 0< 0= ;

: >= < 0= ;

: <= > 0= ;

: D<> D= 0= ;

: UD. D. ;

: NIL 0 ;

: 0<= 0> 0= ;

: U<= U> 0= ;

: bounds OVER + SWAP ;


: nil!   ( a-addr -- = Set address to nil )
  0!
;


: nil=   ( addr -- flag = Check for nil )
  0=
;


: nil<>   ( addr -- flag = Check for unequal to nil )
  0 <>
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


: 1-!   ( a-addr -- = Decrease contents of address by 1 )
  -1 swap +!
;


: @!   ( x1 a-addr -- x2 = First fetch the contents x2 and then store value x1 )
  dup @ -rot !
;

MODULE: inner
[DEFINED] COMPARE-U [IF]
: COMPARE-U COMPARE-U ;
[ELSE]
: UPPERCASE ( addr1 u1 -- )
  OVER + SWAP ?DO
    I C@ CHAR-UPPERCASE I C!
  LOOP ;

: COMPARE-CHAR-U ( c1 c2 --  -1|0|1 )
  2DUP = IF 2DROP 0 EXIT THEN
  CHAR-UPPERCASE SWAP CHAR-UPPERCASE -
  DUP 0= IF EXIT THEN
  0< IF 1 EXIT THEN -1
  \ n is minus-one (-1) if the character c1 has a lesser numeric value than the character c2
  \ and one (1) otherwise.
;

: COMPARE-U ( addr1 u1 addr2 u2 -- flag )
  ROT 2DUP - >R UMIN
  0 ?DO 2DUP
  C@ SWAP C@ SWAP
  COMPARE-CHAR-U DUP IF NIP NIP UNLOOP RDROP EXIT THEN DROP
  SWAP CHAR+ SWAP CHAR+
  LOOP 2DROP R>
  DUP 0= IF EXIT THEN
  0< IF 1 EXIT THEN -1
  \ n is minus-one (-1) if u1 is less than u2 and one (1) otherwise
;
[THEN]
;MODULE


: icompare   ( c-addr1 u1 c-addr2 u2 -- n = Compare case-insensitive two strings and return the result [-1,0,1] )
  inner::COMPARE-U
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


\ size of float value in bytes
\ float2.f operates with 64-bit floats by default
8    CONSTANT float

TRUE CONSTANT FLOATING-EXT
6    CONSTANT FLOATING-STACK

[DEFINED] float [IF]

( Float extension constants )

0E+0 fconstant 0e+0  ( -- r = Float constant 0.0 )
1E+0 fconstant 1e+0  ( -- r = Float constant 1.0 )
2E+0 fconstant 2e+0  ( -- r = Float constant 2.0 )


( Float extension words )

: f-rot            ( r1 r2 r3 -- r3 r1 r2 = Rotate counter clockwise three floats )
  frot frot
;

: ftuck ( f1 f2 -- f2 f1 f2 )
  FSWAP FOVER 
;


: f2dup            ( r1 r2 -- r1 r2 r1 r2 = Duplicate two floats )
  fover fover
;


: f>r              ( r -- ; R: -- r = Push float on the return stack )
  r> rp@ float - rp! rp@ f! >r 
;

: fr>              ( -- r ; R: r -- = Pop float from the return stack )
  r> rp@ f@ float rp@ + rp! >r
;

: fr@              ( -- r ; R: r -- r = Get float from top of return stack )
  r> rp@ f@ >r
;

[THEN]


( Exceptions )

variable exp-next  -2050 exp-next !

: exception      ( c-addr u -- n = Create an exception )
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
s" Wrong checksum"     exception constant exp-wrong-checksum     ( -- n = Wrong checksum )
s" Wrong length"       exception constant exp-wrong-length       ( -- n = Wrong length )

[ELSE]
  drop
[THEN]

\ ==============================================================================

