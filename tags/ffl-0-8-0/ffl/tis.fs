\ ==============================================================================
\
\              tis - the text input stream module in the ffl
\
\               Copyright (C) 2006-2007  Dick van Oudheusden
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
\  $Date: 2008-02-21 20:31:19 $ $Revision: 1.21 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] tis.version [IF]


include ffl/str.fs
include ffl/chs.fs


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
( {{{                                                                       )
(   Stack usage reader word: x -- c-addr u | 0 = Return read data c-addr u or 0 for no more )
( }}}                                                                       )


2 constant tis.version


( Input stream structure )

begin-structure tis%       ( -- n = Get the required space for a tis variable )
  str% 
  +field  tis>text
  field:  tis>pntr
  field:  tis>reader
  field:  tis>data
end-structure


( Private words )

: tis-pntr?!       (  n tis -- = Set the stream pointer u, with range check )
  2dup str-length@ 
  over > swap 0>= and IF          \ Check for pointer range
    tis>pntr !
    true
  ELSE
    2drop
    false
  THEN
;


( Input stream creation, initialisation and destruction )

: tis-init         ( tis -- = Initialise the empty input stream )
  dup str-init               \ Initialise the base string data structure
  dup tis>pntr     0!
  dup tis>reader nil!
      tis>data     0!
;


: tis-create       ( "<spaces>name" -- ; -- tis = Create a named input stream in the dictionary )
  create   here   tis% allot   tis-init
;


: tis-new          ( -- tis = Create a new input stream on the heap )
  tis% allocate  throw  dup tis-init
;


: tis-(free)       ( tis -- = Free the internal, private variables from the heap )
  str-(free)
;

  
: tis-free         ( tis -- = Free the input stream from the heap )
  dup tis-(free)
  
  free throw
;


( Seek and tell words: position in the stream )

: tis-pntr@        ( tis -- u = Get the stream pointer )
  tis>pntr @
;


: tis-pntr!        ( n tis -- flag = Set the stream pointer from start {>=0} or from end {<0} )
  over 0< IF
    tuck str-length@ +                 \ Determine new pointer for negative value
    swap
  THEN
  
  tis-pntr?!
;


: tis-pntr+!       ( n tis -- flag = Add the offset u to the stream pointer )
  tuck tis-pntr@ +
  swap 
  
  tis-pntr?!
;


( Reader words )

: tis-set-reader  ( x xt tis -- = Initialise the stream for reading using the reader callback xt and its data x )
  >r
  r@ tis>reader !
  r@ tis>data   !
  r@            str-clear
  r> tis>pntr   0!
;


: tis-read-more   ( tis -- flag = Read more data from the reader )
  >r
  false
  r@ tis>reader @ nil<> IF
    r@ tis>data @  r@ tis>reader @  execute ?dup IF
      r@ str-append-string
      0=
    THEN
  THEN
  rdrop
;


( String words )

: tis-reset        ( tis -- = Reset the input stream for reading from string)
  tis>pntr 0!
;


: tis-set          ( c-addr u tis -- = Initialise the stream for reading from a string )
  dup tis-reset
  str-set
;


: tis-get          ( tis -- 0 | addr u = Get the remaining characters from the stream, stream pointer is not changed )
  >r
  r@ str-length@ r@ tis-pntr@ -        \ Determine remaining length
  0 max
  
  dup 0> IF
    r@ tis-pntr@ chars
    r@ str-data@ +                     \ Determine start of remaining chars in stream
    swap
  THEN
  rdrop
;


( Stream words )

: tis-eof?         ( tis -- flag = Check if the end of the stream is reached )
  >r
  r@ tis-pntr@  r@ str-length@ >= dup IF
    drop
    r@ tis-read-more 0=
  THEN
  rdrop
;


: tis-reduce   ( tis -- = Reduce the stream size )
  >r 
  r@ tis-pntr@ 256 > IF
    0 r@ tis>pntr @!  0 r@ str-delete  \ Remove leading string and reset pntr
  THEN
  rdrop
;


( Fetch and next words )

: tis-fetch-char   ( tis -- false | char true = Fetch the next character from the stream )
  dup tis-eof? 0= dup IF
    >r
    dup  tis-pntr@ chars
    swap str-data@ + c@
    r>
  ELSE
    nip
  THEN
;


: tis-next-char    ( tis -- = Move the stream pointer one character after fetch-char )
  tis>pntr 1+!
;


: tis-fetch-chars  ( n tis -- 0 | addr u = Fetch maximum of n next characters from the stream )
  >r
  BEGIN
    dup r@ str-length@ r@ tis-pntr@ - > IF
      r@ tis-read-more 0=
    ELSE
      true
    THEN
  UNTIL

  r@ str-length@ r@ tis>pntr @ -       \ Determine remaining length, limit between 0 and requested chars
  min 0 max
  
  dup 0> IF
    r@ tis>pntr @ chars
    r@ str-data@ +                     \ Determine start of remaining chars in stream
    swap
  THEN
  rdrop
;


: tis-next-chars   ( n tis -- = Move the stream pointer n characters after fetch-chars )
  tis>pntr +!
;




( Match words: check for starting data)

: tis-imatch-char  ( char tis -- flag = Match case-insensitive a character )
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


: tis-cmatch-char  ( char tis -- flag = Match case-sensitive a character )
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


: tis-cmatch-chars ( c-addr u tis -- false | char true = Match one of the characters in the string case-sensitive )
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


: tis-cmatch-string  ( c-addr u tis -- flag = Match case-sensitive a string )
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


: tis-imatch-string  ( c-addr u tis -- flag = Match case-insensitive a string )
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


: tis-match-set   ( chs tis - false | char true = Match one of the characters in the set  )
  >r
  r@ tis-fetch-char IF
    dup rot chs-char? IF
      r@ tis-next-char
      true
    ELSE
      drop false
    THEN
  ELSE
    drop false
  THEN
  rdrop
;


( Read data words )

: tis-read-char    ( tis -- false | char true = Read character from the stream )
  dup tis-fetch-char IF
    swap tis-next-char
    true
  ELSE
    drop 
    false
  THEN
;


: tis-read-all   ( tis -- 0 | c-addr u = Read all remaining characters from the stream )
  >r
  BEGIN
    r@ tis-read-more 0=                \ Read all data from the reader
  UNTIL
  
  r@ str-length@
  r@ tis-pntr@ -
  0 max                                \ Remaining characters
  dup 0> IF
    r@ tis-pntr@ chars
    r@ str-data@ +                     \ c-addr
    swap
    r@ str-length@                     \ pntr = length
    r@ tis>pntr !
  THEN
  rdrop
;


: tis-read-string  ( n tis -- 0 | c-addr u = Read n characters from the stream )
  >r
  r@ tis-fetch-chars
  dup 0> IF
    dup r@ tis-next-chars
  THEN
  rdrop
;


: tis-read-line    ( tis -- 0 | c-addr u = Read characters till cr and/or lf )
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
    

: tis-read-number  ( tis -- false | n true = Read a cell number in the current base from the stream )
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


: tis-read-double  ( tis -- false | d true = Read a double value in the current base from the stream )
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


( Private scan words )

: tis-substring   ( n tis -- c-addr u = Get a substring from the string, starting from n till tis-pntr )
  2dup str-data@ swap chars +          \ Start of string
  -rot
  tis-pntr@ swap -                     \ Length of string
;


( Scan words: look for data in the stream )

: tis-scan-char    ( char tis -- false | c-addr u true = Read characters till the char )
  >r
  r@ tis-pntr@ swap                    \ Save current pointer
  
  BEGIN
    r@ tis-fetch-char IF               \ Fetch the current character
      over = IF
        true false                     \   If match found then found & ready
      ELSE
        true                           \   Else continue searching
      THEN
    ELSE
      false false                      \ If no more characters then not found & ready
    THEN
  WHILE
    r@ tis-next-char                   \ Move to the next character in the stream
  REPEAT
  
  nip                                  \ Drop scan character
  IF
    r@ tis-substring                   \ Leading string
    
    r@ tis-next-char                   \ Skip scan character
    
    true
  ELSE
    r@ tis>pntr !                      \ Restore current pointer
    false
  THEN
  rdrop
;


: tis-scan-chars   ( c-addr1 n1 tis -- false | c-addr2 u2 char true = Read characters till one of characters in c-addr1 u1 )
  >r
  r@ tis-pntr@ -rot                    \ Save the current pointer
  
  BEGIN
    2dup
    r@ tis-fetch-char IF               \ Fetch the current character
      chr-string? IF
        true false                     \   If current character in string of chars then found & ready
      ELSE
        true                           \   Else continue searching
      THEN
    ELSE
      2drop
      false false                      \ If no more characters then not found & ready
    THEN
  WHILE
    r@ tis-next-char
  REPEAT
  
  nip nip                              \ Drop string of chars
  IF
    r@ tis-substring                   \ Leading string
    
    r@ tis-fetch-char drop             \ Fetch the scanned character
    
    r@ tis-next-char                   \ Skip scan character
    
    true
  ELSE
    r@ tis>pntr !                      \ Restore current pointer
    false
  THEN
  rdrop
;


: tis-scan-string  ( c-addr1 n2 tis -- false | c-addr1 u2 true = Read characters till the string c-addr1 n1 )
  >r
  r@ tis-pntr@ -rot                    \ Save the current pointer
  
  BEGIN
    2dup
    dup r@ tis-fetch-chars ?dup IF     \ Fetch the same string from the stream
      compare 0= IF
        true false                     \   If strings equal then found & ready
      ELSE 
        true                           \   Else continue searching
      THEN
    ELSE
      2drop
      false false                      \ If no more characters then not found & ready
    THEN
  WHILE
    r@ tis-next-char                   \ Move to next character in stream
  REPEAT
  
  IF
    nip
    swap 
    r@ tis-substring                   \ Leading string
    
    rot r@ tis-next-chars              \ Skip scanned string
    
    true
  ELSE
    2drop
    r@ tis>pntr !                      \ Restore current pointer
    false
  THEN
  rdrop
;


: tis-iscan-string   ( c-addr1 n1 tis -- false | c-addr2 u2 true = Read characters till the string c-addr1 n1 [case insensitive] )
  >r
  r@ tis-pntr@ -rot                    \ Save the current pointer
  
  BEGIN
    2dup
    dup r@ tis-fetch-chars ?dup IF     \ Fetch the same string from the stream
      icompare 0= IF
        true false                     \   If strings equal then found & ready
      ELSE 
        true                           \   Else continue searching
      THEN
    ELSE
      2drop
      false false                      \ If no more characters then not found & ready
    THEN
  WHILE
    r@ tis-next-char                   \ Move to next character in stream
  REPEAT
  
  IF
    nip
    swap 
    r@ tis-substring                   \ Leading string
    
    rot r@ tis-next-chars              \ Skip scanned string
    
    true
  ELSE
    2drop
    r@ tis>pntr !                      \ Restore current pointer
    false
  THEN
  rdrop
;


: tis-scan-set   ( chs tis - false | c-addr u char true = Read characters till one of the characters in the set chs )
  >r
  r@ tis-pntr@ swap                    \ Save the current pointer
  
  BEGIN
    r@ tis-fetch-char IF               \ Fetch the current character
      over chs-char? IF
        true false                     \   If current character in string of chars then found & ready
      ELSE
        true                           \   Else continue searching
      THEN
    ELSE
      false false                      \ If no more characters then not found & ready
    THEN
  WHILE
    r@ tis-next-char
  REPEAT
  
  nip                                  \ Drop set
  IF
    r@ tis-substring                   \ Leading string
    
    r@ tis-fetch-char drop             \ Fetch the scanned character
    
    r@ tis-next-char                   \ Skip scan character
    
    true
  ELSE
    r@ tis>pntr !                      \ Restore current pointer
    false
  THEN
  rdrop
;


( Skip words: skip data in the stream )

: tis-skip-spaces  ( tis -- n = Skip whitespace in the stream, return the number of skipped whitespace characters )
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

: tis-dump         ( tis -- = Dump the text input stream )
  ." tis:" dup . cr
  dup tis>text str-dump
  ."  pntr  :" tis>pntr ? cr
;


[THEN]

\ ==============================================================================
