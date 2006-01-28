\ ==============================================================================
\
\              tis - the text input stream module in the ffl
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
\  $Date: 2006-01-28 08:11:57 $ $Revision: 1.3 $
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

: tis-next         ( w:tis - = Move the offset in the input stream )
  tis>offset 1+!
;


: tis-get          ( w:tis - addr = Get address of offset in text stream )
  dup str>data @
  swap tis>offset @ 
  chars +
;


: tis-fetch-char   ( w:tis - false | c true = Fetch the next character from the stream )
  dup tis>offset @ over str-length@ over >= IF
    chars swap str>data @ + c@
    true
  ELSE
    false
  THEN
;


: tis-next-char    ( w:tis - = Move the offset one character after fetch-char )
  tis>offset 1+!
;


: tis-fetch-chars  ( n w:tis - 0 | addr u = Fetch maximum of n next characters from the stream )
  >r
  r@ str-length@ r@ tis>offset @ -     \ Determine remaining length, limit between 0 and requested chars
  min 0 max
  
  dup 0> IF
    r@ tis>offset @ chars
    r@ str>data   @ +                  \ Determine start of remaining chars in stream
    swap
  THEN
  rdrop
;


: tis-next-chars   ( n w:tis - = Move the offset n characters after fetch-chars )
  tis>offset +!
;


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
  dup tis>offset @ swap tis>input str-length@ >=
;


( Read from text input stream )

: tis-read-char    ( w:tis - false | c true = Read character from the stream )
  dup tis-fetch-char IF
    swap tis-next-char
    true
  ELSE
    drop 
    false
  THEN
;


: tis-read-string  ( n w:tis - 0 | c-addr n = Read u characters from the stream )
  >r
  r@ tis-fetch-chars
  dup 0> IF
    dup r@ tis-next-chars
  THEN
  rdrop
;


: tis-read-line    ( w:tis - 0 | c-addr n = Read characters till cr/lf )
  >r
  r@ tis-fetch-chars dup 0> IF   \ ToDo: comment
    BEGIN
      r@ tis-fetch-char IF
        dup  chr.cr <>
        swap chr.lf <> AND
      ELSE
        false
      THEN
    WHILE
      1+
      r@ tis-next-char
    REPEAT  
  THEN
  
  r@ tis-fetch-char IF              \ ToDo: tis-cmatch-char
    chr.cr = IF
      r@ tis-next-char
    THEN
  THEN
  
  r@ tis-fetch-char IF
    chr.lf = IF
      r@ tis-next-char
    THEN
  THEN
  
  rdrop
;
    

: tis-read-cell    ( w:tis - false | n true = Read a cell value in the current base )
  
;


: tis-read-double  ( w:tis - false | d true = Read a double value in the current base )
;


( Scan the text input stream )

: tis-scan-char    ( c w:tis - false | c-addr u true = Read characters till c )
;


: tis-scan-chars   ( c-addr n w:tis - false | c-addr u c true = Read characters till one of characters )
;


: tis-scan-string  ( c-addr n w:tis - false | c-addr u true = Read characters till the string )
;


: tis-imatch-char  ( c w:tis - f = Match case-insensitive a character )
;


: tis-cmatch-char  ( c w:tis - f = Match case-sensitive a character )
  >r
  r@ tis-fetch-char IF
    = dup IF
      r@ tis-next-char
    THEN
  ELSE
    drop false
  THEN
  rdrop
;


: tis-imatch-chars ( c-addr n w:tis - false | c true = Match one of the characters case-insensitive )
  >r
  chr-lower
  r@ tis-fetch-char IF
    chr-lower = dup IF
      r@ tis-next-char
    THEN
  ELSE
    drop false
  THEN
  rdrop
;


: tis-match-chars ( c-addr n w:tis - false | c true = Match one of the characters case-sensitive )
;


: tis-cmatch-string  ( c-addr n w:tis - f = Match case-sensitive a string )
;


: tis-skip-spaces  ( w:tis - n = Skip whitespace in the stream )
  >r 0
  BEGIN
    r@ tis-fetch-char IF        \ ToDo: comment
      chr-blank?
    ELSE
      false
    THEN
  WHILE
    1+
    r@ tis-next-char
  REPEAT
  rdrop
;


( Position in the stream )

: tis-tell         ( w:tis - u = Tell the current position )
  tis>offset @
;


: tis-seek-start   ( u w:tis - f = Seek the u position from start )
  2dup str-length@ u< IF
    tis>offset !
    true
  ELSE
    2drop
    false
  THEN
;


: tis-seek-current ( u w:tis - f = Seek the u position from current )
  tuck tis>offset @ +
  swap tis-seek-start
;


: tis-seek-end     ( u w:tis - f = Seek the u position from end )
  tuck str-length@ 
  swap - dup 0>= IF
    swap tis-seek-start
  ELSE
    2drop
    false
  THEN
;

[THEN]

\ ==============================================================================
