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
\  $Date: 2008-11-23 06:48:53 $ $Revision: 1.3 $
\
\ ==============================================================================
\
\ This file is for gforth.
\
\ ==============================================================================


c" ffl.version" find nip 0= [IF]


( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)


000700 constant ffl.version


( Private words )
  
variable sys.endian   1 sys.endian !


( System Settings )

create sys.eol     ( - c-addr = Counted string for the end of line for the current system )
  1 c, 10 c,         \ unix: lf
\ 2 c, 13 c, 10 c,   \ dos:  cr lf
  
  
8                            constant sys.bits-in-byte   ( - n = Number of bits in a byte )

sys.bits-in-byte 1 chars *   constant sys.bits-in-char   ( - n = Number of bits in a char )
  
sys.bits-in-byte cell *      constant sys.bits-in-cell   ( - n = Number of bits in a cell )  

sys.endian c@ 0=             constant sys.bigendian      ( - f = Check for bigendian hardware )


( Extension words )

-1 constant max-ms@   ( - u = Maximum value of the milliseconds timer )


: lroll            ( u1 u - u2 = Rotate u1 u bits to the left )
  2dup lshift >r
  sys.bits-in-cell swap - rshift r>
  or
;


: 0!               ( w - = Set zero in address )
  0 swap !
;


: u<=              ( u u - f = Check for smaller and equal )
  u> 0=
;


: 0>=              ( n - f = Check for equal and greater zero )
  0< 0=
;


: 0<=
  0> 0=
;


: >=               ( n n - f = Check for greater equal )
  < 0=
;


: <=               ( n n - f = Check for smaller equal )
  > 0=
;


: d<>              ( d d - f = Check if two two double are unequal )
  d= 0=
;


0 constant nil     ( - w = Nil )


: nil!             ( w - = Set nil in address )
  nil swap !
;


: nil=             ( w - f = Check for nil )
  nil =
;


: nil<>            ( w - f = Check for unequal to nil )
  nil <>
;


: nil<>?           ( addr -- false | addr true = If addr is nil, then return false, else return address with true )
  state @ IF
    postpone ?dup
  ELSE
    ?dup
  THEN
; immediate


: 1+!              ( w - = Increase contents of address by 1 )
  1 swap +!
;


: 1-!              ( w - = Decrease contents of address by 1 )
  -1 swap +!
;


: @!               ( w a - w = First fetch the contents and then store the new value )
  dup @ -rot !
;


: ud.
  <# #s #> type
;


: sgn              ( n - n = Determine the sign of the number )
  -1 max 1 min
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


[DEFINED] float [IF]

( Float extension constants )

0e0 fconstant 0e0  ( - r:0e0 = Float constant 0.0 )

1e0 fconstant 1e0  ( - r:1e0 = Float constant 1.0 )

2e0 fconstant 2e0  ( - r:2e0 = Float constant 2.0 )


( Float extension words )

: f-rot            ( r1 r2 r3 - r3 r1 r2 = Rotate counter clockwise three floats )
  frot frot
;


: f2dup            ( r1 r2 - r1 r2 r1 r2 = Duplicate two floats )
  fover fover
;


: f>r              ( r - = Push float on the return stack )
  r> rp@ float - rp! rp@ f! >r 
;

: fr>              ( - r = Pop float from the return stack )
  r> rp@ f@ float rp@ + rp! >r
;

: fr@              ( - r = Get float from top of return stack )
  r> rp@ f@ >r
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
s" Wrong file type"    exception constant exp-wrong-file-type    ( -- n = Wrong file type )
s" Wrong file version" exception constant exp-wrong-file-version ( -- n = Wrong file version )
s" Wrong file data"    exception constant exp-wrong-file-data    ( -- n = Wrong file data )
s" Wrong checksum"     exception constant exp-wrong-checksum     ( -- n = Wrong checksum )
s" Wrong length"       exception constant exp-wrong-length       ( -- n = Wrong length )

[THEN]

\ ==============================================================================

