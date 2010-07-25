\ ==============================================================================
\
\              tos - the text output stream module in the ffl
\
\               Copyright (C) 2005-2010  Dick van Oudheusden
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
\  $Date: 2008-02-22 06:38:06 $ $Revision: 1.16 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] tos.version [IF]


include ffl/str.fs
include ffl/msc.fs


( tos = Text output stream )
( The tos module implements a text output stream. It extends the str module, )
( so all words from the str module, can be used on a tos variable.           )
( The data written to the stream is always appended. Alignment is normally   )
( done for the last written data. By using the start alignment pointers      )
( words the start of the alignment can be changed. The end of the alignment  )
( is always the end of the stream. The message catalog can be used for       )
( localization of strings. Note: the numerical words in this module use the  )
( the numeric output string. The writer can be used to direct the result of  )
( the formatter to a writer word by calling tos-flush:                       )
( {{{                                                                        )
(  Stack usage writer word: c-addr u x -- flag = Write c-addr u, return success )
( }}}                                                                        )

4 constant tos.version


( Output stream structure )

begin-structure tos%       ( -- n = Get the required space for a tos variable )
  str% 
  +field  tos>text
  field:  tos>pntr
  field:  tos>msc             \ Reference to a message catalog
  field:  tos>writer
  field:  tos>data
end-structure


( Private words )

: tos-sync         ( tos -- = Synchronize the string length and the alignment start pointer )
  dup  str-length@
  swap tos>pntr !
;


: tos-pntr?!       ( n tos -- flag = Set the alignment start pointer after range check )
  2dup str-length@ 
  over > swap 0>= and IF          \ Check for pointer range
    tos>pntr !
    true
  ELSE
    2drop
    false
  THEN
;


( Output stream creation, initialisation and destruction )

: tos-init         ( tos -- = Initialise the empty output stream )
  dup str-init               \ Initialise the base string data structure
  dup tos-sync
  dup tos>msc    nil!
  dup tos>writer nil!
      tos>data     0!
;


: tos-(free)       ( tos -- = Free the tos data from the heap )
  str-(free)
;


: tos-create       ( "<spaces>name" -- ; -- tos = Create a named output stream in the dictionary )
  create   here   tos% allot   tos-init
;


: tos-new          ( -- tos = Create a new output stream on the heap )
  tos% allocate  throw  dup tos-init
;


: tos-free         ( tos -- = Free the output stream from the heap )
  dup tos-(free)
  
  free throw
;


( Stream words )

: tos-rewrite      ( tos -- = Rewrite the output stream )
  dup tos>text   str-clear
      tos-sync
;


( Alignment start pointer words )

: tos-pntr@       ( tos -- u = Get the current alignment start pointer )
  tos>pntr @
;


: tos-pntr!        ( n tos -- flag = Set the alignment pointer from start [n>=0] or from end [n<0], return success )
  over 0< IF
    tuck str-length@ +            \ Determine new pointer for negative value
    swap
  THEN
  
  tos-pntr?!
;


: tos-pntr+!       ( n tos -- flag = Add the offset n to the alignment pointer, return success )
  tuck tos-pntr@ +
  swap
  
  tos-pntr?!
;


( Writer words )

: tos-set-writer  ( x xt tos -- = Use the stream for writing using the writer callback xt and its data x )
  tuck
  tos>writer !
  tos>data   !
;


: tos-flush       ( tos -- = Flush the contents of the stream to the writer )
  >r
  r@ tos>writer @ nil<>? IF       \ If writer present Then
    r@ tos>data @ swap            \   Get data
    r@ str-get dup IF             \   Get string, If length > 0 Then
      2swap execute IF            \     Do writer, If processed Then
        r@ str-clear              \       Clear string
        r@ tos>pntr 0!            \       Reset stream pointer
      THEN
    ELSE
      2drop 2drop
    THEN
  THEN
  rdrop
;


( Message catalog words )

: tos-msc!         ( msc tos -- = Set the message catalog for the output stream )
  tos>msc !
;


: tos-msc@         ( tos -- msc | nil = Get the message catalog for the output stream )
  tos>msc @
;


( Write data words )

: tos-write-char    ( char tos -- = Write character to the stream )
  dup tos-sync
  str-append-char
;


: tos-write-chars   ( char u tos -- = Write u chars to the stream )
  dup tos-sync
  str-append-chars
;


: tos-write-string  ( c-addr u tos -- = Write the string c-addr u to the stream, using the message catalog if present )
  >r
  r@ tos-sync
  r@ tos-msc@ dup nil<> IF
    msc-translate                 \ If message catalog present, than translate the string
  ELSE
    drop
  THEN
  r> str-append-string
;


: tos-write-line    ( tos -- = Write end-of-line from config to the stream, not align able )
  end-of-line
  count bounds ?DO
    I c@ over tos-write-char
  1 chars +LOOP
  tos-sync
;  


: tos-write-number  ( n tos -- = Write the number n in the current base to the stream )
  dup tos-sync swap
  s>d
  swap over dabs
  <# #s rot sign #>
  rot str-append-string
;


: tos-write-double  ( d tos -- = Write the double d in the current base to the stream )
  dup tos-sync -rot
  swap over dabs
  <# #s rot sign #>
  rot str-append-string
;


[DEFINED] represent [IF]
: tos-write-float   ( r tos -- = Write the float r to the stream in notation: [-]0.digitsE[-]digits, using PAD and PRECISION )
  >r
  r@ tos-sync
  r@ str-length@ precision + 7 + r@ str-size!
  pad precision 80 min represent IF
    IF s" -0." ELSE s" 0." THEN
    r@ str-append-string
    pad precision 80 min 
    r@ str-append-string
    s>d
    swap over dabs
    <# #s rot sign [char] E hold #>
  ELSE
    2drop
    s" !err!"
  THEN
  r> str-append-string
;

: tos-write-fixed-point  ( r tos -- = Write the float r to the stream in fixed-point notation: [-]digits.digits0, using PAD and PRECISION )
  >r
  r@ tos-sync
  precision 80 min                                       \ Limit precision
  pad over represent IF
    IF [char] - r@ str-append-char THEN                  \ Add the sign
    dup 0<= IF                                           \ If exponent is negative
      abs
      2dup + 1+ 1+ r@ str-length@ + r@ str-size!         \   Insure the size for the float
      s" 0." r@ str-append-string                        \   Start with 0.
      [char] 0 swap r@ str-append-chars                  \   Append exponent zero's
      pad swap r@ str-append-string                      \   Append the string
    ELSE 2dup < IF                                       \ Else If exponent is smaller than the precision
      over 1+ r@ str-length@ + r@ str-size!              \   Insure the size for the float
      over pad swap r@ str-append-string                 \   Append the float
      swap - [char] 0 swap r@ str-append-chars           \   Fill out with zero's
      [char] .  r@ str-append-char                       \   Add the dot
    ELSE                                                 \ Else
      dup 1+ r@ str-length@ + r@ str-size!               \   Insure the size for the float
      pad over r@ str-append-string                      \   Append the digits for the exponent
      [char] . r@ str-append-char                        \   Add the dot
      pad -rot /string r@ str-append-string              \   Append the remaining
    THEN THEN
  ELSE
    drop 2drop
    s" !err!" r@ str-append-string
  THEN
  rdrop
;
[THEN]


( Alignment words )

: tos-align        ( char u1 u2 tos -- = Align the previous written data with padding character char, u1 trailing chars and u2 leading chars )
  >r
  r@ tos>pntr @ r@ str-length@ < IF    \ Something to align ?
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


: tos-align-left   ( char u tos -- = Align the previous written data to the left, using padding character char with width u )
  >r
  r@ str-length@ r@ tos-pntr@ -        \ Determine length previous written text
  - dup 0> IF                          \ If width > length previous written text then
    0 r@ tos-align                     \   Align with trailing chars
  ELSE
    2drop
  THEN
  rdrop
;


: tos-align-right  ( char u tos -- = Align the previous written data to the right, using padding character char with width u )
  >r
  r@ str-length@ r@ tos-pntr@ -        \ Determine length previous written text
  - dup 0> IF                          \ If width > length previous written text then
    0 swap r@ tos-align                \   Align with leading chars
  ELSE
    2drop
  THEN
  rdrop
;


: tos-center       ( char u tos -- = Center the previous written data, using padding character char with width u )
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

: tos-dump         ( tos -- = Dump the text output stream )
  ." tos:" dup . cr
  dup tos>text str-dump
  ."  pntr  :" dup tos>pntr ? cr
  ."  msc   :"     tos>msc  ? cr
;

[THEN]

\ ==============================================================================
