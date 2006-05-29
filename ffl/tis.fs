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
\  $Date: 2006-04-05 17:39:42 $ $Revision: 1.11 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] tis.version [IF]


include ffl/str.fs


( tis = Text input stream )
( The tis module implements a text input stream. It extends the str module, )
( so all words from the str module, can be used on the tis data structure.  )
( There are seven basic methods: fetch = fetch the data, the stream pointer )
( is not updated; next = after a fetch, the stream pointer is updated; seek )
( = move the stream pointer; match = try to match data, if there is a match,)
( the stream pointer is updated, read = read data, if data is returned then )
( the stream pointer is updated; scan = scan for data, if the data is found )
( then the leading text is returned and the stream pointer is moved after   )
( the scanned data; skip = move the stream pointer after the skipped data.  )  

1 constant tis.version


( Public structure )

struct: tis%       ( - n = Get the required space for the tis data structure )
  str% field: tis>text
       cell:  tis>pntr
;struct


( Private database )


( Private words )

: tis-pntr?!       (  n w:tos - = Update the stream pointer after range check )
  2dup str-length@ 
  over > swap 0>= and IF          \ Check for pointer range
    tis>pntr !
    true
  ELSE
    2drop
    false
  THEN
;


( Public words )

: tis-init         ( w:tis - = Initialise the empty input stream )
  dup str-init               \ Initialise the base string data structure
      tis>pntr  0!
;


: tis-create       ( C: "name" - R: - w:tis = Create a named input stream in the dictionary )
  create   here   tis% allot   tis-init
;


: tis-new          ( - w:tis = Create a new input stream on the heap )
  tis% allocate  throw  dup tis-init
;


: tis-free         ( w:tis - = Free the input stream from the heap )
  str-free
;


: tis-reset        ( w:tis - = Reset the input stream )
  tis>pntr 0!
;


: tis-eof?         ( w:tis - f = Check if the end of the stream is reached )
  dup tis>pntr @ swap str-length@ >=
;


( Fetch and next words )

: tis-fetch-char   ( w:tis - false | c true = Fetch the next character from the stream )
  dup tis>pntr @ over str-length@ over >= IF
    chars swap str-data@ + c@
    true
  ELSE
    false
  THEN
;


: tis-next-char    ( w:tis - = Move the stream pointer one character after fetch-char )
  tis>pntr 1+!
;


: tis-fetch-chars  ( n w:tis - 0 | addr u = Fetch maximum of n next characters from the stream )
  >r
  r@ str-length@ r@ tis>pntr @ -       \ Determine remaining length, limit between 0 and requested chars
  min 0 max
  
  dup 0> IF
    r@ tis>pntr @ chars
    r@ str>data   @ +                  \ Determine start of remaining chars in stream
    swap
  THEN
  rdrop
;


: tis-next-chars   ( n w:tis - = Move the stream pointer n characters after fetch-chars )
  tis>pntr +!
;


( Set and get words )

: tis-set          ( c-addr u w:tis - = Set the string in the stream, reset the stream pointer )
  dup tis-reset
  str-set
;


: tis-get          ( w:tis - 0 | addr u = Get the remaining characters from the stream, stream pointer is not changed )
  >r
  r@ str-length@ r@ tis>pntr @ -       \ Determine remaining length
  0 max
  
  dup 0> IF
    r@ tis>pntr @ chars
    r@ str>data @ +                    \ Determine start of remaining chars in stream
    swap
  THEN
  rdrop
;


( Seek and tell words: position in the stream )

: tis-pntr@        ( w:tis - u = Get the stream pointer )
  tis>pntr @
;


: tis-pntr!        ( n w:tis - f = Set the stream pointer from start {>=0} or from end {<0} )
  over 0< IF
    tuck str-length@ +                 \ Determine new pointer for negative value
    swap
  THEN
  
  tis-pntr?!
;


: tis-pntr+!       ( n w:tis - f = Add an offset to the stream pointer )
  tuck tis-pntr@ +
  swap 
  
  tis-pntr?!
;


( Match words: check for starting data)

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


( Read data words )

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


: tis-read-line    ( w:tis - 0 | c-addr n = Read characters till cr and/or lf )
  >r
  1 r@ tis-fetch-chars dup 0> IF
    drop 0
    
    BEGIN
      r@ tis-fetch-char IF             \ Read the string till cr/lf
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
    
    chr.cr r@ tis-cmatch-char drop     \ Process cr
    chr.lf r@ tis-cmatch-char drop     \ Process lf
  THEN
  
  rdrop
;
    

: tis-read-number  ( w:tis - false | n true = Read a cell number in the current base )
  >r
  r@ tis-pntr@
  false                                \ Process leading +/-
  [char] - r@ tis-cmatch-char IF
    0=
  ELSE
    [char] + r@ tis-cmatch-char drop
  THEN
  
  r@ tis-fetch-char IF
    chr-base IF
      r@ tis-next-char
      
      BEGIN                            \ Process digits
        r@ tis-fetch-char IF
          chr-base
        ELSE
          false
        THEN
      WHILE
        swap base @ * +                \ Calculate the number
        r@ tis-next-char
      REPEAT
      
      swap IF
        negate
      THEN
      nip
      true
    ELSE
      drop r@ tis-pntr! drop
      false
    THEN
  ELSE
    drop r@ tis-pntr! drop
    false
  THEN
  rdrop
;


: tis-read-double  ( w:tis - false | d true = Read a double value in the current base )
  >r
  r@ tis-pntr@
  false                                \ Process leading +/-
  [char] - r@ tis-cmatch-char IF
    0=
  ELSE
    [char] + r@ tis-cmatch-char drop
  THEN
  
  r@ tis-fetch-char IF
    chr-base IF
      s>d
      r@ tis-next-char
      
      BEGIN
        r@ tis-fetch-char IF
          chr-base                     \ Process the digits
        ELSE
          false
        THEN
      WHILE
        >r base @ 1 m*/ r> m+          \ Calculate the number
        r@ tis-next-char
      REPEAT
      
      2swap IF
        >r dnegate r>
      THEN
      drop
      true
    ELSE
      drop r@ tis-pntr! drop
      false
    THEN
  ELSE
    drop r@ tis-pntr! drop
    false
  THEN
  rdrop
;


( Scan words: look for data in the stream )

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
    r@ tis-fetch-char IF
      chr-blank?                       \ Process the spaces
    ELSE
      false
    THEN
  WHILE
    1+
    r@ tis-next-char
  REPEAT
  rdrop
;


( Inspection )

: tis-dump         ( w:tis - = Dump the text input stream )
  ." tis:" dup . cr
  dup tis>text str-dump
  ."  pntr  :" tis>pntr ? cr
;


[THEN]

\ ==============================================================================