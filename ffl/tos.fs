\ ==============================================================================
\
\              tos - the text output stream module in the ffl
\
\               Copyright (C) 2005-2006  Dick van Oudheusden
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
\  $Date: 2006-02-01 20:05:00 $ $Revision: 1.4 $
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
  str% field: tos>text
       cell:  tos>pntr
;struct


( Private database )



( Private words )

: tos-sync         ( w:tos - = Synchronize the string length and the alignment start pointer )
  dup  str-length@
  swap tos>pntr !
;


0 [IF]
: tos-number-info ( n - f:neg n:width = Determine the sign and width of a number )
  dup 0< tuck IF
    negate 2 
  ELSE
    1 
  THEN
  swap
  
  BEGIN
    base @ / ?dup
  WHILE
    swap 1+ swap
  REPEAT
;


: tos-double-info ( d - f:neg n:width = Determine the sign and width of a double )
  2dup d0< IF
    true -rot
    dnegate 
    2 
  ELSE
    false -rot
    1
  THEN
  -rot
  
  BEGIN
    base @ m/ 2dup d0<>
  WHILE
    rot 1+ -rot
  REPEAT
  2drop
;
[THEN]


( Public words )

: tos-init         ( w:tos - = Initialise the empty output stream )
  dup str-init               \ Initialise the base string data structure
      tos-sync
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
  dup tos>text   str-clear
      tos-sync
;


( Alignment start pointer words )

: tos-pntr@       ( w:tos - u = Get the current alignment start pointer )
  tos>pntr @
;


: tos-pntr!       ( u w:tos - f = Set the alignment start pointer )
  2dup str-length@ u<= IF
    tos>pntr !
    true
  ELSE
    2drop
    false
  THEN
;

( Write to the text output stream )

: tos-write-char    ( c w:tos - = Write character to the stream )
  dup tos-sync
  str-append-char
;


: tos-write-chars   ( c u w:tos - = Write u characters to the stream )
  dup tos-sync
  str-append-chars
;


: tos-write-string  ( c-addr u w:tos - = Write string to the stream )
  dup tos-sync
  str-append
;


: tos-write-line    ( w:tos - = Write cr/lf to the stream, not alignable )
  [DEFINED] sys-win32 [IF]
  chr.cr over tos-write-char
  chr.lf over tos-write-char
  [THEN]
  [DEFINED] sys-unix [IF]
  chr.lf over tos-write-char
  [THEN]
  tos-sync
;


: tos-write-number  ( n w:tos - = Write a number in the current base to the stream )
  dup tos-sync swap
  s>d
  swap over dabs
  <# #s rot sign #>
  rot str-append
;


0 [IF]
  dup tos-number-info                  \ Determine sign and width of number
  chr.sp swap r@ tos-write-chars       \ Write width spaces in the stream
  swap
  r> str-bounds drop 1 chars -         \ Determine the start address
  swap
  
  BEGIN
    base @ /mod tos-convert-digit      \ Determine a digit from the number
    -rot over c!                       \ Store the digit
    1 chars -
    swap
    ?dup 0=                            \ Until the whole number is converted
  UNTIL
  
  swap IF                              \ If negative Then
    char - swap c!                     \   Prepend a sign
  ELSE
    drop
  THEN
[THEN]


: tos-write-double  ( d w:tos - = Write a double in the current base to the stream )
  dup tos-sync -rot
  swap over dabs
  <# #s rot sign #>
  rot str-append
;


( Align the text )

: tos-align        ( c:pad u:trailing u:leading w:tos - = Align the previous written text )
  >r
  r@ tos>pntr @ r@ str-length@ < IF   \ ToDo: exception ??
    >r over r>
    
    ?dup IF                            \ Insert the leading spaces
      r@ tos>pntr @ r@ str-insert-chars
    ELSE
      drop
    THEN
    
    ?dup IF                            \ Insert the trailing spaces
      r@ str-append-chars
    ELSE
      drop
    THEN
    
  ELSE
    drop 2drop
  THEN
  
  rdrop
;


: tos-align-left   ( c:pad u:width w:tos - = Align left the previous written text )
  >r
  r@ str-length@ r@ tos-pntr@ -        \ Determine length previous written text
  - dup 0> IF                          \ If width > length previous written text then
    0 r@ tos-align                     \   Align with trailing chars
  ELSE
    2drop
  THEN
  rdrop
;


: tos-align-right  ( c:pad u:width w:tos - = Align right the previous written text )
  >r
  r@ str-length@ r@ tos-pntr@ -        \ Determine length previous written text
  - dup 0> IF                          \ If width > length previous written text then
    0 swap r@ tos-align                \   Align with leading chars
  ELSE
    2drop
  THEN
  rdrop
;


: tos-center       ( c:pad u:width w:tos - = Center the previous written text )
  >r
  r@ str-length@ r@ tos-pntr@ -        \ Determine length previous written text
  - dup 0> IF                          \ If width > length previous written text then
    dup 2/ swap over - r@ tos-align    \   Align with leading and trailing chars
  ELSE
    2drop
  THEN
  rdrop
;


( Inspection )

: tos-dump         ( w:tos - = Dump the text output stream )
;

[THEN]

\ ==============================================================================
