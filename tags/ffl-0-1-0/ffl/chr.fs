\ ==============================================================================
\
\                   chr - the char module in the ffl
\
\               Copyright (C) 2005  Dick van Oudheusden
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
\  $Date: 2005-12-19 19:51:26 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] chr.version [IF]


( chr = Char Data Type )
( The chr module implements words for checking ranges of characters and for    )
( converting upper to lower case and v.v.                                      )
  

1 constant chr.version


( Public words )

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
  0 127 chr-range?
;


: chr-blank?   ( c - f = Check for a blank character, space or tab )
  dup bl = swap 9 = OR
;


: chr-cntrl?   ( c - f = Check for a control character, 0 till 31 )
  0 31 chr-range?
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
  dup bl = swap 9 13 chr-range? OR
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


[THEN]

\ ==============================================================================
