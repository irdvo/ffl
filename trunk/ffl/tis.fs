\ ==============================================================================
\
\              tis - the text input stream module in the ffl
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
\  $Date: 2005-12-30 20:36:27 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] tis.version [IF]


include ffl/str.fs


( tis = Text input stream )
( The tis module implements a text input stream. It extends the str module, )
( so all words from the str module, can be used on the tis data structure.  )


1 constant tis.version


( Public structure )

struct: tis%       ( - n = Get the required space for the tis data structure )
  str% field: tis>input
       cell:  tis>offset
;struct


( Private database )



( Private words )



( Public words )

: tis-init         ( w:tis - = Initialise the empty input stream )
  dup str-init               \ Initialise the base string data structure
      tis>offset  0!
;


: tis-create       ( C: "name" - R: - w:tis = Create a named input stream in the dictionary )
  create   here   tis% allot   tis-init
;


: tis-new          ( - w:tis = Create a new input stream on the heap )
  tis% allocate  throw  dup tis-init
;


: tis-free         ( w:tis - = Free the input stream from the heap )
  free  throw
;


: tis-reset        ( w:tis - = Reset the input stream )
  tis>offset 0!
;


: tis-eof?         ( w:tis - f = Check if the end of the stream is reached )
;


( Read from text input stream )

: tis-read-char    ( w:tis - c = Read character from the stream)
;


: tis-read-string  ( n w:tis - c-addr n = Read n characters from the stream )
;


: tis-read-line    ( w:tis - c-addr n = Read characters till cr/lf )
;


: tis-read-cell    ( w:tis - n = Read binary a cell )
;


: tis-read-double  ( w:tis - d = Read binary a double )
;


: tis-read-float   ( w:tis - f = Read binary a float )
;


( Scan the text input stream )

: tis-scan-char    ( c w:tis - false | c-addr u true = Read characters till c )
;


: tis-scan-chars   ( c-addr n w:tis - false | c-addr u c true = Read characters till one of characters )
;


: tis-scan-string  ( c-addr n w:tis - false | c-addr u true = Read characters till the string )
;


: tis-imatch-string  ( c-addr n w:tis - f = Match case-insensitive a string )
;


: tis-cmatch-string  ( c-addr n w:tis - f = Match case-sensitive a string )
;


: tis-skip-spaces  ( w:tis - n = Skip whitespace in the stream )
;


( Position in the stream )

: tis-tell         ( w:tis - u = Tell the current position )
;


: tis-seek-start   ( u w:tis - = Seek the u position from start )
;


: tis-seek-current ( u w:tis - = Seek the u position from current )
;


: tis-seek-end     ( u w:tis - = Seek the u position from end )
;

[THEN]

\ ==============================================================================
