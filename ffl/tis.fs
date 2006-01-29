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
\  $Date: 2006-01-29 08:51:56 $ $Revision: 1.5 $
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


: tis-get          ( w:tis - 0 | addr u = Get the remaining characters from the stream )
  >r
  r@ str-length@ r@ tis>offset @ -     \ Determine remaining length
  0 max
  
  dup 0> IF
    r@ tis>offset @ chars
    r@ str>data   @ +                  \ Determine start of remaining chars in stream
    swap
  THEN
  rdrop
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
  dup tis>offset @ swap str-length@ >=
;


( Match characters in the stream)

: tis-imatch-char  ( c w:tis - f = Match case-insensitive a character )
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


: tis-cmatch-chars ( c-addr n w:tis - false | c true = Match one of the characters case-sensitive )
  >r
  r@ tis-fetch-char IF
    dup >r chr-string? r> over IF
      r@ tis-next-char
      swap
    ELSE
      drop
    THEN
  ELSE
    2drop false
  THEN
  rdrop
;


: tis-cmatch-string  ( c-addr n w:tis - f = Match case-sensitive a string )
  >r
  dup r@ tis-fetch-chars ?dup IF
    dup >r compare 0= r> over IF
      r@ tis-next-chars
    ELSE
      drop
    THEN
  ELSE
    2drop false
  THEN
  rdrop
;


: tis-imatch-string  ( c-addr n w:tis - f = Match case-insensitive a string )
  >r
  dup r@ tis-fetch-chars ?dup IF
    dup >r icompare 0= r> over IF
      r@ tis-next-chars
    ELSE
      drop
    THEN
  ELSE
    2drop false
  THEN
  rdrop
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
  1 r@ tis-fetch-chars dup 0> IF   \ ToDo: comment
    drop 0
    
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
    
    dup 0= IF
      nip
    THEN
    
    chr.cr r@ tis-cmatch-char drop
    chr.lf r@ tis-cmatch-char drop
  THEN
  
  rdrop
;
    

: tis-read-cell    ( w:tis - false | n true = Read a cell value in the current base )
  
;


: tis-read-double  ( w:tis - false | d true = Read a double value in the current base )
;


( Scan the text input stream )

: tis-scan-char    ( c w:tis - false | c-addr u true = Read characters till c )
  >r
  0 swap
  r@ tis-get ?dup IF                   \ If there are still characters in the stream
    bounds ?DO                         \ Do for all remaining characters
      dup I c@ = IF                    \  If scan character found then
        drop
        unloop
        r@ tis-get drop swap           \   Return start of character
        dup 1+ r> tis-next-chars       \   Set characters processed
        true exit
      THEN
      swap 1+ swap
      1 chars
    +LOOP
  THEN
  2drop rdrop
  false
;


: tis-scan-chars   ( c-addr n w:tis - false | c-addr u c true = Read characters till one of characters )
  >r
  0 -rot
  r@ tis-get ?dup IF                   \ If there are remaing characters in the stream
    bounds ?DO                         \ Do for the remaing characters
      2dup I c@ chr-string? IF         \  If character in the string then
        2drop
        I c@
        unloop
        swap
        r@ tis-get drop swap           \   Return the start of the remaining characters
        dup 1+ r> tis-next-chars       \   Set characters processed
        rot
        true exit
      THEN
      rot 1+ -rot
      1 chars
    +LOOP
  THEN
  2drop drop rdrop
  false
;


: tis-scan-string  ( c-addr n w:tis - false | c-addr u true = Read characters till the string )
  >r
  0 -rot
  r@ tis-get ?dup IF                   \ Check if there are remaing characters
    swap >r over - r> swap             \ Calculate the number of remaining characters that can fit the string
    dup 0> IF
      bounds ?DO                       \ Do for the remaing character that can fit the string
        2dup I over compare 0= IF      \  If the remaining characters has the string Then
          nip
          unloop
          over +
          r@ tis-get drop              \   Return the start of the remaing characters
          swap r> tis-next-chars       \   Set characters processed
          swap 
          true exit
        THEN
        rot 1+ -rot
        1 chars
      +LOOP
    ELSE
      2drop
    THEN
  THEN
  2drop drop rdrop
  false
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
