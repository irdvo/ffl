\ ==============================================================================
\
\                   stt - the stringtable in the ffl
\
\               Copyright (C) 2007  Dick van Oudheusden
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
\  $Date: 2009-05-20 13:27:22 $ $Revision: 1.4 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] stt.version [IF]

( stt = Stringtable module )
( The stt module implements a stringtable with counted strings. The code is  )
( inspired by Wil Badens stringtable.                                        )


2 constant stt.version


( Stringtable syntax words )

: begin-stringtable  ( "<spaces>name" -- stringtable-sys ; n -- c-addr u = Start a named stringtable definition; return the nth string )
  create
    here
    cell allot
    here                \ stringtable-sys: a-addr (start) a-addr (strings)
  does>
    @ 
    swap cells + 
    @ count  
;


: +"               ( "ccc<quote>" -- = Parse ccc delimited by double quote and place the string as counted string in the stringtable )
  [char] " parse
  here 
  over char+ allot
  2dup c!
  char+ swap cmove
  align
;


: end-stringtable  ( stringtable-sys -- = End the stringtable definition )
  here rot !                      \ Set the offset
  here swap
  BEGIN
    2dup <>
  WHILE
    dup ,                         \ Store the start of the strings
    count chars + aligned         \ Move to the next string
  REPEAT
  2drop
;

[THEN]

\ ==============================================================================
