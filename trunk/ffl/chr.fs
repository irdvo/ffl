\ ==============================================================================
\
\                   chr - the char module in the ffl
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
\  $Date: 2007-12-09 07:23:15 $ $Revision: 1.8 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] chr.version [IF]


( chr = Char Data Type )
( The chr module implements words for checking ranges of characters and for )
( converting characters.                                                    )
  

2 constant chr.version


( ASCII constants )

0   constant chr.nul         ( -- char = the null character )
1   constant chr.soh         ( -- char = the soh character )
2   constant chr.stx         ( -- char = the stx character )
3   constant chr.etx         ( -- char = the etx character )
4   constant chr.eot         ( -- char = the eot character )
5   constant chr.enq         ( -- char = the enq character )
6   constant chr.ack         ( -- char = the ack character )
7   constant chr.bel         ( -- char = the bel character )
8   constant chr.bs          ( -- char = the backspace character )
9   constant chr.ht          ( -- char = the horz. tab character )
10  constant chr.lf          ( -- char = the line feed character )
11  constant chr.vt          ( -- char = the vert. tab character )
12  constant chr.ff          ( -- char = the formfeed character )
13  constant chr.cr          ( -- char = the carriage return character )
14  constant chr.sm          ( -- char = the sm character )
15  constant chr.si          ( -- char = the si character )
16  constant chr.dle         ( -- char = the dle character )
17  constant chr.dc1         ( -- char = the dc1 character )
18  constant chr.dc2         ( -- char = the dc2 character )
19  constant chr.dc3         ( -- char = the dc3 character )
20  constant chr.dc4         ( -- char = the dc4 character )
21  constant chr.nak         ( -- char = the nak character )
22  constant chr.syn         ( -- char = the syn character )
23  constant chr.etb         ( -- char = the etc character )
24  constant chr.can         ( -- char = the cancel character )
25  constant chr.em          ( -- char = the em character )
26  constant chr.sub         ( -- char = the sub character )
27  constant chr.esc         ( -- char = the escape character )
28  constant chr.fs          ( -- char = the fs character )
29  constant chr.gs          ( -- char = the gs character )
30  constant chr.rs          ( -- char = the rs character )
31  constant chr.us          ( -- char = the us character )
32  constant chr.sp          ( -- char = the space character )
127 constant chr.del         ( -- char = the delete character )


( Character class checking words )

: chr-range?   ( char1 char2 char3 -- flag = Check if char1 is in the range [char2..char3] )
  1+ within                  \ !or!   -rot over <= -rot >= AND   !or!   >r over <= swap r> <= AND
;

  
: chr-lower?   ( char -- flag = Check for a lowercase alphabetic character )
  [char] a [char] z chr-range?
;


: chr-upper?   ( char -- flag = Check for an uppercase alphabetic character )
  [char] A [char] Z chr-range?
;


: chr-alpha?   ( char -- flag = Check for an alphabetic character )
  dup chr-upper? swap chr-lower? OR
;


: chr-digit?   ( char -- flag = Check for a decimal digit character )
  [char] 0 [char] 9 chr-range?
;


: chr-alnum?   ( char -- flag = Check for an alphanumeric character )
  dup chr-digit? swap chr-alpha? OR
;


: chr-ascii?   ( char -- flag = Check for an ascii character )
  chr.nul chr.del chr-range?
;


: chr-blank?   ( char -- flag = Check for a blank character, space or tab )
  dup chr.sp = swap chr.ht = OR
;


: chr-cntrl?   ( char -- flag = Check for a control character, 0 till 31 )
  chr.nul chr.us chr-range?
;


: chr-graph?   ( char -- flag = Check for a printable character except space )
  [char] ! [char] ~ chr-range?
;


: chr-print?   ( char -- flag = Check for a printable character including space )
  bl [char] ~ chr-range?
;


: chr-punct?   ( char -- flag = Check for a printable character, but not a space or alphanumeric character )
  dup chr-graph? swap chr-alnum? 0= AND
;


: chr-space?   ( char -- flag = Check for a white-space: space, lf, vt, ff, cr )
  dup chr.sp = swap chr.ht chr.cr chr-range? OR
;


: chr-hexdigit? ( char -- flag = Check for a hexadecimal character )
  dup chr-digit? swap 
  dup [char] a [char] f chr-range? swap
  [char] A [char] F chr-range?
  OR OR
;


: chr-octdigit? ( char -- flag = Check for an octal character )
  [char] 0 [char] 7 chr-range?
;


: chr-string?  ( c-addr u char -- flag = Check if the character is in the string )
  -rot bounds ?DO
    dup I c@ = IF
      drop true
      UNLOOP EXIT
    THEN
    1 chars 
  +LOOP
  drop false
;


( Character conversion words )

: chr-upper    ( char1 -- char2 = Convert char1 to uppercase )
  dup chr-lower? IF
    [ char a char A - ] literal -
  THEN
;


: chr-lower    ( char1 -- char2 = Convert char1 to lowercase )
  dup chr-upper? IF
    [ char a char A - ] literal +
  THEN
;


: chr-base     ( char -- false | u true = Convert the character to a digit according the current base )
  dup chr-alnum? IF
    dup chr-alpha? IF
      chr-upper
      [ char A 10 - ] literal -
    ELSE
      [char] 0 -
    THEN
    
    dup base @ u< dup 0= IF
      nip
    THEN
  ELSE
    drop false
  THEN
;


[THEN]

\ ==============================================================================
