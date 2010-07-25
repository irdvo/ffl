\ ==============================================================================
\
\               chs - the character set module in the ffl
\
\              Copyright (C) 2006-2008  Dick van Oudheusden
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
\  $Date: 2008-10-06 18:22:09 $ $Revision: 1.14 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] chs.version [IF]


include ffl/stc.fs
include ffl/chr.fs


( chs = Character set module )
( The chs module implements a character set. It supports the POSIX classes   )
( used in regular expressions.                                               )
  

3 constant chs.version


( Private constant )

128 #bits/char / constant chs.size  ( -- n = Size of the characters set in chars: 128 / bits in chars )
  
  
( Character Set Structure )

begin-structure chs%       ( -- n = Get the required space for a chs variable )
  chs.size
  cfields: chs>set
end-structure


( Set creation, initialisation and destruction )

: chs-init         ( chs -- = Initialise the set )
  chs>set  chs.size chars  erase
;


: chs-create       ( "<spaces>name" -- ; -- chs = Create a named character set in the dictionary )
  create   here   chs% allot   chs-init
;


: chs-new          ( -- chs = Create a new character set on the heap )
  chs% allocate  throw  dup chs-init
;


: chs-free         ( chs -- = Free the set from the heap )
  free throw 
;


( Private set word )

: chs^execute      ( xt chs2 chs1 -- = Execute xt for every byte of chs2 and chs1 and store the result in chs1, xt: [ char1 char2 -- char3] )
  >r chs>set r> chs>set
  chs.size bounds DO
    2dup c@ 
    I c@ 
    rot execute
    I c!
    char+
  1 chars +LOOP
  2drop
;


( Sets words )

: chs^move         ( chs1 chs2 -- = Move chs1 in chs2 )
  chs>set swap chs>set swap chs.size cmove
;


: chs^or           ( chs1 chs2 -- = OR the sets chs1 with chs2 and store the result in chs2 )
  ['] OR -rot chs^execute
;


: chs^and          ( chs1 chs2 -- = AND the sets chs1 with chs2 and store the result in chs2 )
  ['] AND -rot chs^execute
;


: chs^xor          ( chs1 chs2 -- = XOR the sets chs1 with chs2 and store the result in chs2 )
  ['] XOR -rot chs^execute
;


( Private words )

: chs+validate-char  ( char -- = Check and throw exception if character is out of range )
  127 u> exp-invalid-parameters AND throw
;


: chs+validate-chars ( char1 char2 -- = Check and throw exception if one of the characters out of range )
  127 u> swap 127 u> OR exp-invalid-parameters AND throw
;


: chs-address      ( char chs -- u a-addr = Determine mask u and array address for the character )
  >r
  #bits/char /mod           \ Mask and offset
  swap 1 swap lshift
  swap chars r> chs>set + 
;


: chs-set-ch       ( char chs -- = Set the character in the set )
  chs-address
  tuck c@ OR
  swap c!
;


: chs-reset-ch     ( char chs -- = Reset the character in the set )
  chs-address
  swap invert 
  over c@ AND 
  swap c!
;


: chs-ch?          ( char chs -- flag = Check if the character is in the set )
  chs-address
  c@ AND 0<>
;


: chs-emit-char    ( char -- = Emit the character from the set )
  dup chr-print? IF
    emit
  ELSE
    [char] < emit 0 .r [char] > emit
  THEN
;


( Set words )

: chs-set          ( chs -- = Set all characters in the set )
  chs>set chs.size -1 fill
;


: chs-reset        ( chs -- = Reset all characters in the set )
  chs>set chs.size chars erase
;


: chs-invert       ( chs -- = Invert all characters in the set )
  chs>set
  chs.size 0 DO
    dup  c@ invert
    over c!
    char+
  LOOP
  drop
;


( Char words )

: chs-set-char     ( char chs -- = Set the character in the set )
  over chs+validate-char
  chs-set-ch
;


: chs-reset-char   ( char chs -- = Reset the character in the set )
  over chs+validate-char
  chs-reset-ch
;


( Character range words )

: chs-set-chars    ( char1 char2 chs -- = Set the character range [char2..char1] in the set )
  -rot
  2dup chs+validate-chars
  2dup max 1+ -rot min DO
    I over chs-set-ch
  LOOP
  drop
;


: chs-reset-chars  ( char1 char2 chs -- = Reset the character range [char2..char1] in the set )
  -rot
  2dup chs+validate-chars
  2dup max 1+ -rot min DO
    I over chs-reset-ch
  LOOP
  drop
;


( String words )

: chs-set-string  ( c-addr u chs -- = Set the characters in the string in the set )
  -rot
  bounds ?DO
    I c@ over chs-set-char
  1 chars +LOOP
  drop
;


: chs-reset-string  ( c-addr u chs -- = Reset the characters in the string in the set )
  -rot
  bounds ?DO
    I c@ over chs-reset-char
  1 chars +LOOP
  drop
;


( List words )

: chs-set-list     ( charu .. char1 u chs -- = Set the characters char1 till charu in the set )
  swap
  0 ?DO
    tuck chs-set-char
  LOOP
  drop
;


: chs-reset-list   ( charu .. char1 u chs -- = Reset the characters char1 till charu in the set )
  swap
  0 ?DO
    tuck chs-reset-char
  LOOP
  drop
;


( POSIX classes )

: chs-set-upper    ( chs -- = Set the upper class in the set )
  >r [char] Z [char] A r> chs-set-chars
;


: chs-reset-upper  ( chs -- = Reset the upper class in the set )
  >r [char] Z [char] A r> chs-reset-chars  
;


: chs-set-lower    ( chs -- = Set the lower  class in the set )
  >r [char] z [char] a r> chs-set-chars
;


: chs-reset-lower  ( chs -- = Reset the lower class in the set )
  >r [char] z [char] a r> chs-reset-chars
;


: chs-set-alpha    ( chs -- = Set the alpha class in the set )
  dup chs-set-upper chs-set-lower
;


: chs-reset-alpha  ( chs -- = Reset the alpha class in the set )
  dup chs-reset-upper chs-reset-lower
;


: chs-set-digit    ( chs -- = Set the digit class in the set )
  >r [char] 9 [char] 0 r> chs-set-chars
;


: chs-reset-digit  ( chs -- = Reset the digit class in the set )
  >r [char] 9 [char] 0 r> chs-reset-chars
;


: chs-set-alnum    ( chs -- = Set the alnum class in the set )
  dup chs-set-upper 
  dup chs-set-lower 
      chs-set-digit
;


: chs-reset-alnum  ( chs -- = Reset the alnum class in the set )
  dup chs-reset-upper 
  dup chs-reset-lower 
      chs-reset-digit
;


: chs-set-xdigit   ( chs -- = Set the xdigit class in the set )
  >r
  r@ chs-set-digit
  [char] F [char] A r@ chs-set-chars
  [char] f [char] a r> chs-set-chars
;


: chs-reset-xdigit  ( chs -- = Reset the xdigit class in the set )
  >r
  r@ chs-reset-digit
  [char] F [char] A r@ chs-reset-chars
  [char] f [char] a r> chs-reset-chars
;


: chs-set-punct    ( chs -- = Set the punct class in the set )
  >r
  [char] / [char] ! r@ chs-set-chars
  [char] @ [char] : r@ chs-set-chars
  [char] ` [char] [ r@ chs-set-chars
  [char] ~ [char] { r> chs-set-chars  
;


: chs-reset-punct  ( chs -- = Reset the punct class in the set )
  >r
  [char] / [char] ! r@ chs-reset-chars
  [char] @ [char] : r@ chs-reset-chars
  [char] ` [char] [ r@ chs-reset-chars
  [char] ~ [char] { r> chs-reset-chars  
;


: chs-set-blank    ( chs -- = Set the blank class in the set )
  chr.sp over chs-set-char
  chr.ht swap chs-set-char  
;


: chs-reset-blank  ( chs -- = Reset the blank class in the set )
  chr.sp over chs-reset-char
  chr.ht swap chs-reset-char  
;


: chs-set-space    ( chs -- = Set the space class in the set )
  dup chs-set-blank
  chr.cr over chs-set-char
  chr.lf over chs-set-char
  chr.ff over chs-set-char
  chr.vt swap chs-set-char
;


: chs-reset-space  ( chs -- = Reset the space class in the set )
  dup chs-reset-blank
  chr.cr over chs-reset-char
  chr.lf over chs-reset-char
  chr.ff over chs-reset-char
  chr.vt swap chs-reset-char
;


: chs-set-cntrl    ( chs -- = Set the cntrl class in the set )
  >r chr.us chr.nul r@ chs-set-chars
  chr.del r> chs-set-char
;


: chs-reset-cntrl  ( chs -- = Reset the cntrl class in the set )
  >r chr.us chr.nul r@ chs-reset-chars
  chr.del r> chs-reset-char
;


: chs-set-graph    ( chs -- = Set the graph class in the set )
  dup chs-set-alnum chs-set-punct
;


: chs-reset-graph  ( chs -- = Reset the graph class in the set )
  dup chs-reset-alnum chs-reset-punct
;


: chs-set-print    ( chs -- = Set the print class in the set )
  dup chs-set-graph 
  chr.sp swap chs-set-char
;


: chs-reset-print  ( chs -- = Reset the print class in the set )
  dup chs-reset-graph
  chr.sp swap chs-reset-char
;


: chs-set-word     ( chs -- = Set the word class in the set )
  dup chs-set-alnum
  [char] _ swap chs-set-char
;


: chs-reset-word   ( chs -- = Reset the word class in the set )
  dup chs-reset-alnum
  [char] _ swap chs-reset-char
;


( Char check word )

: chs-char?        ( char chs -- flag = Check if the character is in the set )
  over 127 u> IF
    2drop
    false
  ELSE
    chs-ch?
  THEN
;


( Special words )

: chs-execute      ( i*x xt chs -- j*x = Execute xt for every character in the set )
  128 0 DO
    I over chs-ch? IF        \ If character in the set
      I 
      swap >r
      swap >r                \   Clear the stack ..
      r@ execute             \   .. and execute the token
      r> r>                  \   Restore the stack
    THEN
  LOOP
  2drop
;


( Inspection )

: chs-dump         ( chs -- = Dump the chs state )
  ." chs:" dup . cr
  ."   set:" ['] chs-emit-char swap chs-execute
;

[THEN]

\ ==============================================================================
