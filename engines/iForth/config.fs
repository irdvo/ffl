\ ==============================================================================
\
\                  config - the config in the ffl
\
\             Copyright (C) 2005-2010  Dick van Oudheusden
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
\  $Date: 2009-05-20 13:27:22 $ $Revision: 1.5 $
\
\ ==============================================================================
\
\ This file is for iForth.
\
\ ==============================================================================

s" ffl.version" forth-wordlist search-wordlist 0= [IF]

ANEW -ffl


( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)


000900 constant ffl.version


( Private words )
  
variable ffl.endian   1 ffl.endian !


( System Settings )

create end-of-line    ( -- c-addr = Counted string for the end of line for the current system )
  $CR count end-of-line pack drop	\ All hosts 

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
  ms? ;


s" MAX-U" environment? drop constant max-ms@   ( -- u = Maximum value of the milliseconds timer )


: rdrop            ( R: x -- )
  -R 
; \ relying on the fact that this word is inlined


: lroll   ( u1 u2 -- u3 = Rotate u1 u2 bits to the left )
  ROL 
;


: rroll   ( u1 u2 -- u3 = Rotate u1 u2 bits to the right )
  ROR 
;


: ?free   ( addr -- ior = Free the address if not nil )
    free ; \ freeing 0 is not an error


: icompare   ( c-addr1 u1 c-addr2 u2 -- n = Compare case-insensitive two strings and return the result [-1,0,1] )
  rot swap 2swap 2over ( -- u1 u2 c-addr1 c-addr2 u1 u2 )
  min 0 ?DO
    over c@ >UPC over c@ >UPC - sgn ?dup IF
      >r 4drop r>
      unloop 
      exit
    THEN
    char+ swap char+ swap
  LOOP
  2drop
  - sgn
;


: IS  
  STATE @ IF  
    POSTPONE [IS]  
  ELSE  
    ['] IS EXECUTE  
  THEN
; IMMEDIATE


: w@         ( w-addr -- n = Fetch a word, 16 bit, zero extend )
  16B@
;

: <w@        ( w-addr -- n = Fetch a word, 16 bit, sign extend )
  S16B@
;

: w!         ( n w-addr -- = Store a word, 16 bit )
  16B!
;

: l@         ( l-addr -- n = Fetch a long word, 32 bit, zero extend )
  32B@
;

: <l@        ( l-addr -- n = Fetch a long word, 32 bit, sign extend )
  S32B@
;

: l!         ( n l-addr -- = Store a long word, 32 bit )
  32B!
;


[DEFINED] floats [IF]

( Float extension constants )

1 floats constant float ( -- n = Size of one float )


( Float extension words )

: f-rot            ( r1 r2 r3 -- r3 r1 r2 = Rotate counter clockwise three floats )
  -frot ;

CREATE xp 0 C,
CREATE xstack #256 FLOATS ALLOT

: f>r              ( r -- ; R: -- r = Push float on the return stack )
  -1 xp C+!  xstack xp C@ FLOAT[] F! ;

: fr>              ( -- r ; R: r -- = Pop float from the return stack )
  xstack xp C@ FLOAT[] F@   1 xp C+! ;

: fr@              ( -- r ; R: r -- r = Get float from top of return stack )
  xstack xp C@ FLOAT[] F@ ; 

[THEN]


( Toolbelt )

include ffl/tlb.fs


( Exceptions )

variable exp-next  #-2050 exp-next !

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
s" Wrong file data"    exception constant exp-wrong-file-data    ( -- n = Wrong file data )
s" Wrong checksum"     exception constant exp-wrong-checksum     ( -- n = Wrong checksum )
s" Wrong length"       exception constant exp-wrong-length       ( -- n = Wrong length )
s" Invalid data"       exception constant exp-invalid-data       ( -- n = Invalid data exception number )

[ELSE]
  drop
[THEN]

\ ==============================================================================

