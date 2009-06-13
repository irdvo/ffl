\ ==============================================================================
\
\                  config - the config in the ffl
\
\             Copyright (C) 2005-2006  Dick van Oudheusden
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
\  $Date: 2009-05-20 13:27:22 $ $Revision: 1.12 $
\
\ ==============================================================================
\
\ This file is for fina.
\
\ ==============================================================================


( Extension words )


char " parse ffl.version" forth-wordlist search-wordlist 0= [IF]

require timer.fs

: s" [char] " parse state @ if postpone sliteral else here >r s, r> count then ; immediate

: [DEFINED]
 bl word find   nip
; immediate


: [UNDEFINED]
 bl word find   nip 0=
; immediate

1 cells constant cell

: u<=  u> 0= ;
: 0>=  0< 0= ;
: >= < 0= ;
: d<> d= 0= ;
: ud. <# #s #> type space ;

: sgn              ( n1 -- n2 = Determine the sign of the number, return [-1,0,1] )
  -1 max 1 min
;

( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)


000700 constant ffl.version


( Private words )

variable ffl.endian   1 ffl.endian !


( System Settings )

create end-of-line    ( -- c-addr = Counted string for the end of line for the current system )
 newline s,


s" ADDRESS-UNIT-BITS" environment? 0= [IF] 8 [THEN] 
  constant #bits/byte   ( -- +n = Number of bits in a byte )
  
#bits/byte 1 chars *
  constant #bits/char   ( -- +n = Number of bits in a char )
  
#bits/byte cell *
  constant #bits/cell   ( -- +n = Number of bits in a cell )  

ffl.endian c@ 0=             
  constant bigendian?   ( -- flag = Check for bigendian hardware )


( Extension words )

: ms@                   ( -- u = Fetch milliseconds timer )
  nstime 1 1000000 m*/ drop
;


s" MAX-U" environment? drop constant max-ms@  ( -- u = Max val of the milliseconds timer )


1 chars 1 = [IF]
: char/            ( n1 -- n2 = Convert address units to chars )
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


: 0!               ( a-addr -- = Set zero in address )
 0 swap !
;


0 constant nil     ( -- addr = Nil constant )

: nil!             ( a-addr -- = Set nil in address )
 nil swap !
;


: nil=             ( addr -- flag = Check for nil )
 nil =
;


: nil<>            ( addr -- flag = Check for unequal to nil )
 nil <>
;


: nil<>?           ( addr -- false | addr true = If addr is nil, then return false, else return address with true )
  state @ IF
    postpone ?dup
  ELSE
    ?dup
  THEN
; immediate


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


: @!               ( x1 a-addr -- x2 = First fetch the contents x2 and then store the new value x1 )
 dup @ -rot !
;


: <=>              ( n1 n2 -- n3 = Compare two numbers and return the compare result [-1,0,1] )
 2dup = IF
   2drop 0 EXIT
 THEN
 < 2* 1+
;

\ ( n1 n2 -- n3 = Convert the index n1 [-length..length> with length n2 into the offset n3 [0..length> )
: index2offset
 over 0< IF
   +
 ELSE
   drop
 THEN
;


( Exceptions )

variable exp-next  -2050 exp-next !

: exception      ( c-addr u -- n = Create an exception )
 2drop
 exp-next @
 -1 exp-next +!
;


\ -- n = Index out of range exception number
s" Index out of range" exception constant exp-index-out-of-range
\ -- n = Invalid state exception number
s" Invalid state"      exception constant exp-invalid-state
\ -- n = No data available exception number
s" No data available"  exception constant exp-no-data
\ -- n = Invalid parameters on stack
s" Invalid parameters" exception constant exp-invalid-parameters
\ -- n = Wrong file type
s" Wrong file type"    exception constant exp-wrong-file-type    
\ -- n = Wrong file version
s" Wrong file version" exception constant exp-wrong-file-version 
\ -- n = Wrong file data
s" Wrong file data"    exception constant exp-wrong-file-data
\ -- n = Wrong checksum
s" Wrong checksum"     exception constant exp-wrong-checksum
\ -- n = Wrong length
s" Wrong length"       exception constant exp-wrong-length

[ELSE]
 drop
[THEN]

\ ==============================================================================
