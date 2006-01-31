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
\  $Date: 2006-01-31 20:26:35 $ $Revision: 1.3 $
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


( Public words )

: tos-init         ( w:tos - = Initialise the empty output stream )
  dup tos>text   str-init   \ Initialise the base string data structure
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


( Write to text output stream )

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
  \ ToDo
  tos-sync
;


: tos-write-number  ( n w:tos - = Write a number to the stream )
  dup tos-sync
;


: tos-write-double  ( d w:tos - = Write a double to the stream )
  dup tos-sync
;


( Align the previous written text )

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
  r@ str-length@ r@ tos>pntr @ -       \ Determine length previous written text
  - dup 0> IF                          \ If width > length previous written text then
    0 r@ tos-align                     \   Align with trailing chars
  ELSE
    2drop
  THEN
  rdrop
;


: tos-align-right  ( c:pad u:width w:tos - = Align right the previous written text )
  >r
  r@ str-length@ r@ tos>pntr @ -       \ Determine length previous written text
  - dup 0> IF                          \ If width > length previous written text then
    0 swap r@ tos-align                \   Align with leading chars
  ELSE
    2drop
  THEN
  rdrop
;


: tos-center       ( c:pad u:width w:tos - = Center the previous written text )
  >r
  r@ str-length@ r@ tos>pntr @ -       \ Determine length previous written text
  - dup 0> IF                          \ If width > length previous written text then
    dup 2/ swap over - r@ tos-align    \   Align with leading and trailing chars
  ELSE
    2drop
  THEN
  rdrop
;


\ Alignment start pointer words

: tos-tell         ( w:tos - u = Tell the current alignment start pointer )
  tos>pntr @
;


: tos-seek-start  ( u w:tos - f = Set the alignment start pointer )
;


\ Inspection

: tos-dump         ( w:tos - = Dump the text output stream )
;

[THEN]

\ ==============================================================================
