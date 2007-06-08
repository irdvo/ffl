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
\  $Date: 2007-06-08 06:28:29 $ $Revision: 1.2 $
\
\ ==============================================================================
\
\ This file is the config file for minForth
\
\ ==============================================================================


[DEFINED] ffl.version 0= [IF]


( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)


000500 constant ffl.version


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

: [UNDEFINED]
  get-word "find IF drop false ELSE true THEN  ( " )
; immediate


: ms@                                                    ( - u = Fetch milliseconds timer )
  msecs 
;


s" MAX-U" environment? drop constant max-ms@            ( - u = Maximum value of the milliseconds timer )


: rdrop            ( - ) 
  r> r> drop >r
;


: lroll            ( u1 u - u2 = Rotate u1 u bits to the left )
  2dup lshift >r
  #bits/cell swap - rshift r>
  or
;


: lroll            ( u1 u - u2 = Rotate u1 u bits to the left )
  2dup lshift >r
  #bits/cell swap - rshift r>
  or
;


: d<>              ( d d - f = Check if two two double are unequal )
  d= 0=
;


: du<>             ( ud ud - f = Check if two unsigned doubles are unequal )
  d<>
;


: u<=              ( u u - f = Compare for smaller equal )
  u> 0=
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


: @!               ( w a - w = First fetch the contents and then store the new value )
  dup @ -rot !
;


: index2offset     ( n:index n:length - n:offset = Convert an index [-length..length> into an offset [0..length> )
  over 0< IF
    +
  ELSE
    drop
  THEN
;


: <=>              ( n n - n = Compare two numbers and return the compare result [-1,0,1] )
  2dup = IF 
    2drop 0 EXIT 
  THEN
  < 2* 1+
;


: toupper          ( c - c = Convert the character to upper case )
  dup [char] a >= over [char] z <= AND IF
    [ char a char A - ] literal -
  THEN
;


: icompare         ( c-addr u c-addr u - n = Compare case-insensitive two strings )
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


1 floats constant float ( - n = Size of one float )

( Float extension constants )

0e0 fconstant 0e0  ( - r:0e0 = Float constant 0.0 )

1e0 fconstant 1e0  ( - r:1e0 = Float constant 1.0 )

2e0 fconstant 2e0  ( - r:2e0 = Float constant 2.0 )


( Private float variable )

fvariable float<>cells

( Float extension words )

: f-rot            ( r1 r2 r3 - r3 r1 r2 = Rotate counter clockwise three floats )
  frot frot
;


: f2dup            ( r1 r2 - r1 r2 r1 r2 = Duplicate two floats )
  fover fover
;

: ftuck            ( r1 r2 - r2 r1 r2 = Tuck two floats )
  fswap fover
;

: f=               ( r1 r2 - f = Compare two floats )
  f- fabs 1e-99 f<
;

cell 4 = float 8 = AND [IF]

: f>r              ( r - = Put float to return stack )
  r>
  float<>cells f!
  float<>cells @ >r
  float<>cells cell+ @ >r
  >r
;

: fr>              ( - r = Get float from return stack )
  r>
  r> float<>cells cell+ !
  r> float<>cells !
     float<>cells f@
  >r
;

: fr@              ( - r = Fetch float of return stack )
  1 rpick float<>cells cell+ !
  2 rpick float<>cells !
          float<>cells f@
;



( Public Exceptions )

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

[THEN]

\ ==============================================================================

