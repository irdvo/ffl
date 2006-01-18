\ ==============================================================================
\
\              tos - the text output stream module in the ffl
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
\  $Date: 2006-01-18 19:01:44 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] tos.version [IF]


include ffl/str.fs


( tos = Text output stream )
( The tos module implements a text output stream. It extends the str module, )
( so all words from the str module, can be used on the tos data structure.   )


1 constant tos.version


( Public structure )

struct: tos%       ( - n = Get the required space for the tos data structure )
  str% field: tos>output
       cell:  tos>offset
;struct


( Private database )



( Private words )



( Public words )

: tos-init         ( w:tos - = Initialise the empty output stream )
  dup tos>output  str-init   \ Initialise the base string data structure
      tos>offset  0!
;


: tos-create       ( C: "name" - R: - w:tos = Create a named output stream in the dictionary )
  create   here   tos% allot   tos-init
;


: tos-new          ( - w:tos = Create a new output stream on the heap )
  tos% allocate  throw  dup tos-init
;


: tos-free         ( w:tos - = Free the output stream from the heap )
  free  throw
;


: tos-rewrite      ( w:tos - = Rewrite the output stream )
  dup tos>output str-clear
      tos>offset 0!
;


( Write to text output stream )

: tos-write-char    ( c w:tos - = Write character to the stream )
;


: tos-write-string  ( c-addr n w:tos - = Write string to the stream )
;


: tos-write-line    ( c-addr n w:tos - = Write string and cr/lf to the stream )
;


: tos-write-cell    ( w w:tos - = Write binary a cell to the stream )
;


: tos-write-double  ( d w:tos - = Write binary a double to the stream )
;


: tos-write-float   ( f w:tos - = Write binary a float to the stream )
;


( Position in the stream )

: tos-tell         ( w:tos - u = Tell the current position )
;


: tos-seek-start   ( u w:tos - = Seek the u position from start )
;


: tos-seek-current ( u w:tos - = Seek the u position from current )
;


: tos-seek-end     ( u w:tos - = Seek the u position from end )
;

[THEN]

\ ==============================================================================
