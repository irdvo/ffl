\ ==============================================================================
\
\             chs_expl - the character set example in the ffl
\
\               Copyright (C) 2008  Dick van Oudheusden
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
\  $Date: 2008-10-05 06:34:20 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/chs.fs


\ Create a character set variable chs1 in the dictionary

chs-create chs1

\ Set 'a', '*', 'q', 'w' and '0'..'9' in the set

char a chs1 chs-set-char
char * chs1 chs-set-char

char 9 char 0 chs1 chs-set-chars

s" qw" chs1 chs-set-string

\ Check for characters in the set

char 7 chs1 chs-char? [IF]
  .( Character '7' is in the set ) cr
[ELSE]
  .( Character '7' is not in the set ) cr
[THEN]

char ; chs1 chs-char? [IF]
  .( Character ';' is in the set ) cr
[ELSE]
  .( Character ';' is not in the set ) cr
[THEN]

\ Create a character set on the heap

chs-new value chs2

\ Use the space and xdigit classes to fill the set 

chs1 chs-set-space
chs1 chs-set-xdigit

\ Invert the set so that the set contains no spaces and xdigits

chs1 chs-invert

\ Print the set contents by excecuting chs-emit for every character in the set

: chs-emit  ( char -- )
  dup chr-print? IF
    emit
  ELSE
    [char] < emit 0 .r [char] > emit
  THEN
;

.( Set:) ' chs-emit chs1 chs-execute cr

\ Free the set from the heap

chs2 chs-free

