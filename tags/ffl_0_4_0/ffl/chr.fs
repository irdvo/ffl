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
\  $Date: 2006-12-10 07:47:29 $ $Revision: 1.7 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] chr.version [IF]


( chr = Char Data Type )
( The chr module implements words for checking ranges of characters and for )
( converting characters.                                                    )
  

2 constant chr.version


( ASCII constants )

0   constant chr.nul         ( - c = the null character )
1   constant chr.soh         ( - c = the soh character )
2   constant chr.stx         ( - c = the stx character )
3   constant chr.etx         ( - c = the etx character )
4   constant chr.eot         ( - c = the eot character )
5   constant chr.enq         ( - c = the enq character )
6   constant chr.ack         ( - c = the ack character )
7   constant chr.bel         ( - c = the bel character )
8   constant chr.bs          ( - c = the backspace character )
9   constant chr.ht          ( - c = the horz. tab character )
10  constant chr.lf          ( - c = the line feed character )
11  constant chr.vt          ( - c = the vert. tab character )
12  constant chr.ff          ( - c = the formfeed character )
13  constant chr.cr          ( - c = the carriage return character )
14  constant chr.sm          ( - c = the sm character )
15  constant chr.si          ( - c = the si character )
16  constant chr.dle         ( - c = the dle character )
17  constant chr.dc1         ( - c = the dc1 character )
18  constant chr.dc2         ( - c = the dc2 character )
19  constant chr.dc3         ( - c = the dc3 character )
20  constant chr.dc4         ( - c = the dc4 character )
21  constant chr.nak         ( - c = the nak character )
22  constant chr.syn         ( - c = the syn character )
23  constant chr.etb         ( - c = the etc character )
24  constant chr.can         ( - c = the cancel character )
25  constant chr.em          ( - c = the em character )
26  constant chr.sub         ( - c = the sub character )
27  constant chr.esc         ( - c = the escape character )
28  constant chr.fs          ( - c = the fs character )
29  constant chr.gs          ( - c = the gs character )
30  constant chr.rs          ( - c = the rs character )
31  constant chr.us          ( - c = the us character )
32  constant chr.sp          ( - c = the space character )
127 constant chr.del         ( - c = the delete character )


( Character class checking words )

: chr-range?   ( c c:min c:max - f = Check if character is in an including range )
  1+ within                  \ !or!   -rot over <= -rot >= AND   !or!   >r over <= swap r> <= AND
;

  
: chr-lower?   ( c - f = Check for a lowercase alphabetic character )
  [char] a [char] z chr-range?
;


: chr-upper?   ( c - f = Check for an uppercase alphabetic character )
  [char] A [char] Z chr-range?
;


: chr-alpha?   ( c - f = Check for an alphabetic character )
  dup chr-upper? swap chr-lower? OR
;


: chr-digit?   ( c - f = Check for a decimal digit )
  [char] 0 [char] 9 chr-range?
;


: chr-alnum?   ( c - f = Check for an alphanumeric character )
  dup chr-digit? swap chr-alpha? OR
;


: chr-ascii?   ( c - f = Check for an ascii character )
  chr.nul chr.del chr-range?
;


: chr-blank?   ( c - f = Check for a blank character, space or tab )
  dup chr.sp = swap chr.ht = OR
;


: chr-cntrl?   ( c - f = Check for a control character, 0 till 31 )
  chr.nul chr.us chr-range?
;


: chr-graph?   ( c - f = Check for a printable character except space )
  [char] ! [char] ~ chr-range?
;


: chr-print?   ( c - f = Check for a printable character including space )
  bl [char] ~ chr-range?
;


: chr-punct?   ( c - f = Check for a printable character, but not a space or alphanumeric character )
  dup chr-graph? swap chr-alnum? 0= AND
;


: chr-space?   ( c - f = Check for a white-space: space, lf, vt, ff, cr )
  dup chr.sp = swap chr.ht chr.cr chr-range? OR
;


: chr-hexdigit? ( c - f = Check for a hexadecimal character )
  dup chr-digit? swap 
  dup [char] a [char] f chr-range? swap
  [char] A [char] F chr-range?
  OR OR
;


: chr-octdigit? ( c - f = Check for an octal character )
  [char] 0 [char] 7 chr-range?
;


: chr-string?  ( c-addr u c - f = Check if the characters is in the string )
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

: chr-upper    ( c - c = Convert character to uppercase )
  dup chr-lower? IF
    [ char a char A - ] literal -
  THEN
;


: chr-lower    ( c - c = Convert character to lowercase )
  dup chr-upper? IF
    [ char a char A - ] literal +
  THEN
;


: chr-base     ( c - false | u true = Convert a character within the current base )
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
