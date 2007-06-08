\ ==============================================================================
\
\               chs - the character set module in the ffl
\
\               Copyright (C) 2006  Dick van Oudheusden
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
\  $Date: 2007-06-08 06:28:29 $ $Revision: 1.8 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] chs.version [IF]


include ffl/stc.fs
include ffl/chr.fs


( chs = Character set module )
( The chs module implements words for using character sets. It supports the )
( POSIX classes used in regular expressions. )
  

2 constant chs.version


( Private constant )

128 #bits/char / constant chs.size  ( - n = Size of the characters set in chars: 128 / bits in chars )
  
  
( Character Set Structure )

struct: chs%       ( - n = Get the required space for the chs data structure )
 chs.size
  chars: chs>set
;struct


( Set creation, initialisation and destruction )

: chs-init         ( w:chs - = Initialise the character set )
  chs>set  chs.size chars  erase
;


: chs-create       ( "name" - = Create a named character set in the dictionary )
  create   here   chs% allot   chs-init
;


: chs-new          ( - w:chs = Create a new character set on the heap )
  chs% allocate  throw  dup chs-init
;


: chs-free         ( w:chs - = Free the character set from the heap )
  free throw 
;

( Sets words )

: chs^move         ( w:chs2 w:chs1 - = Move chs2 in chs1 )
  chs>set swap chs>set swap chs.size cmove
;


( Private words )

: chs+validate-char  ( c - = Check and throw exception if character out of range )
  127 u> exp-invalid-parameters AND throw
;


: chs+validate-chars ( c c - = Check and thow exception if characters out of range )
  127 u> swap 127 u> OR exp-invalid-parameters AND throw
;


: chs-address      ( c w:chs - u:mask w:addr = Determine mask and array address for character )
  >r
  #bits/char /mod           \ Mask and offset
  swap 1 swap lshift
  swap chars r> chs>set + 
;


: chs-set-ch       ( c w:chs - = Set the character in the set )
  chs-address
  tuck c@ OR
  swap c!
;


: chs-reset-ch     ( c w:chs - = Reset the character in the set )
  chs-address
  swap invert 
  over c@ AND 
  swap c!
;


: chs-ch?          ( c w:chs - f = Check if the character is in the set )
  chs-address
  c@ AND 0<>
;


: chs-emit-char    ( c - = Emit the character from the set )
  dup chr-print? IF
    emit
  ELSE
    [char] < emit 0 .r [char] > emit
  THEN
;


( Set words )

: chs-set          ( w:chs - = Set all characters in the set )
  chs>set chs.size -1 fill
;


: chs-reset        ( w:chs - = Reset all characters in the set )
  chs>set chs.size chars erase
;


: chs-invert       ( w:chs - = Invert all characters in the set )
  chs>set
  chs.size 0 DO
    dup  c@ invert
    over c!
    char+
  LOOP
  drop
;


( Char words )

: chs-set-char     ( c w:chs - = Set the character in the set )
  over chs+validate-char
  chs-set-ch
;


: chs-reset-char   ( c w:chs - = Reset the character in the set )
  over chs+validate-char
  chs-reset-ch
;


( Character range words )

: chs-set-chars    ( c:max c:min w:chs - = Set a range of characters in the set )
  -rot
  2dup chs+validate-chars
  swap 1+ swap DO
    I over chs-set-ch
  LOOP
  drop
;


: chs-reset-chars  ( c:max c:min w:chs - = Reset a range of characters in the set )
  -rot
  2dup chs+validate-chars
  swap 1+ swap DO
    I over chs-reset-ch
  LOOP
  drop
;


( String words )

: chs-set-string  ( c-addr u w:chs - = Set the characters in the string in the set )
  -rot
  bounds ?DO
    I c@ over chs-set-char
  1 chars +LOOP
  drop
;


: chs-reset-string  ( c-addr u w:chs - = Reset the characters in the string in the set )
  -rot
  bounds ?DO
    I c@ over chs-reset-char
  1 chars +LOOP
  drop
;


( POSIX classes )

: chs-set-upper    ( w:chs - = Set the upper class in the set )
  >r [char] Z [char] A r> chs-set-chars
;


: chs-reset-upper  ( w:chs - = Reset the upper class in the set )
  >r [char] Z [char] A r> chs-reset-chars  
;


: chs-set-lower    ( w:chs - = Set the lower class in the set )
  >r [char] z [char] a r> chs-set-chars
;


: chs-reset-lower  ( w:chs - = Reset the lower class in the set )
  >r [char] z [char] a r> chs-reset-chars
;


: chs-set-alpha    ( w:chs - = Set the alpha class in the set )
  dup chs-set-upper chs-set-lower
;


: chs-reset-alpha  ( w:chs - = Reset the alpha class in the set )
  dup chs-reset-upper chs-reset-lower
;


: chs-set-digit    ( w:chs - = Set the digit class in the set )
  >r [char] 9 [char] 0 r> chs-set-chars
;


: chs-reset-digit  ( w:chs - = Reset the digit class in the set )
  >r [char] 9 [char] 0 r> chs-reset-chars
;


: chs-set-alnum    ( w:chs - = Set the alnum class in the set )
  dup chs-set-upper 
  dup chs-set-lower 
      chs-set-digit
;


: chs-reset-alnum  ( w:chs - = Reset the alnum class in the set )
  dup chs-reset-upper 
  dup chs-reset-lower 
      chs-reset-digit
;


: chs-set-xdigit   ( w:chs - = Set the xdigit class in the set )
  >r
  r@ chs-set-digit
  [char] F [char] A r@ chs-set-chars
  [char] f [char] a r> chs-set-chars
;


: chs-reset-xdigit  ( w:chs - = Reset the xdigit class in the set )
  >r
  r@ chs-reset-digit
  [char] F [char] A r@ chs-reset-chars
  [char] f [char] a r> chs-reset-chars
;


: chs-set-punct    ( w:chs - = Set the punct class in the set )
  >r
  [char] / [char] ! r@ chs-set-chars
  [char] @ [char] : r@ chs-set-chars
  [char] ` [char] [ r@ chs-set-chars
  [char] ~ [char] { r> chs-set-chars  
;


: chs-reset-punct  ( w:chs - = Reset the punct class in the set )
  >r
  [char] / [char] ! r@ chs-reset-chars
  [char] @ [char] : r@ chs-reset-chars
  [char] ` [char] [ r@ chs-reset-chars
  [char] ~ [char] { r> chs-reset-chars  
;


: chs-set-blank    ( w:chs - = Set the blank class in the set )
  chr.sp over chs-set-char
  chr.ht swap chs-set-char  
;


: chs-reset-blank  ( w:chs - = Reset the blank class in the set )
  chr.sp over chs-reset-char
  chr.ht swap chs-reset-char  
;


: chs-set-space    ( w:chs - = Set the space class in the set )
  dup chs-set-blank
  chr.cr over chs-set-char
  chr.lf over chs-set-char
  chr.ff over chs-set-char
  chr.vt swap chs-set-char
;


: chs-reset-space  ( w:chs - = Reset the space class in the set )
  dup chs-reset-blank
  chr.cr over chs-reset-char
  chr.lf over chs-reset-char
  chr.ff over chs-reset-char
  chr.vt swap chs-reset-char
;


: chs-set-cntrl    ( w:chs - = Set the cntrl class in the set )
  >r chr.us chr.nul r@ chs-set-chars
  chr.del r> chs-set-char
;


: chs-reset-cntrl  ( w:chs - = Reset the cntrl class in the set )
  >r chr.us chr.nul r@ chs-reset-chars
  chr.del r> chs-reset-char
;


: chs-set-graph    ( w:chs - = Set the graph class in the set )
  dup chs-set-alnum chs-set-punct
;


: chs-reset-graph  ( w:chs - = Reset the graph class in the set )
  dup chs-reset-alnum chs-reset-punct
;


: chs-set-print    ( w:chs - = Set the print class in the set )
  dup chs-set-graph 
  chr.sp swap chs-set-char
;


: chs-reset-print  ( w:chs - = Reset the print class in the set )
  dup chs-reset-graph
  chr.sp swap chs-reset-char
;


: chs-set-word     ( w:chs - = Set the word class in the set )
  dup chs-set-alnum
  [char] _ swap chs-set-char
;


: chs-reset-word   ( w:chs - = Reset the word class in the set )
  dup chs-reset-alnum
  [char] _ swap chs-reset-char
;


( Char check word )

: chs-char?        ( c w:chs - f = Check if character is in the set )
  over chs+validate-char
  chs-ch?
;


( Special words )

: chs-execute      ( .. xt w:chs - .. = Execute xt for every character in the set )
  128 0 DO
    I over chs-ch? IF        \ If character in the set
      I 
      swap >r
      swap >r                \   Clear the stack ..
      r@ execute             \   .. and execute the token
      r> r>                  \   Restor the stack
    THEN
  LOOP
  2drop
;


( Inspection )

: chs-dump         ( w:chs - = Dump the chs state )
  ." chs:" dup . cr
  ."   set:" ['] chs-emit-char swap chs-execute
;

[THEN]

\ ==============================================================================
