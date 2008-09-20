\ ==============================================================================
\
\                spf - the sprintf formatter in the ffl
\
\               Copyright (C) 2008  Dick van Oudheusden
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
\  $Date: 2008-09-20 05:31:18 $ $Revision: 1.3 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] spf.version [IF]

include ffl/str.fs


( spf = Sprintf string formatter )
( The spf module implements a simplified sprintf function in forth.          )
( <pre>                                                                      )
( Format: %[flags][width][length]specifier                                   )
(     Flags: 0      = Left-pads the number with zeros instead of spaces      )
(            -      = Left justify the number                                )
(            +      = A positive number is preceded with a '+'               )
(            space  = A positive number is preceded with a space             )
(     Width: number = the minimum number of characters written               )
(    Double: l      = the argument is interpreted as a double                )
( Specifier: c      = format a character [char]                              )
(            d      = format a signed number [n or d]                        )
(            i      = format a signed number [n or d]                        )
(            o      = format a signed octal [n or d]                         )
(            s      = format a string [c-addr u]                             )
(            u      = format a unsigned number [u or ud]                     )
(            x      = format a unsigned hexadecimal number [u or ud]         )
(            X      = format a unsigned hexadecimal number, capital letters  )
(            p      = format a unsigned hexadecimal number [u or ud]         )
(            n      = store the length of the string in [addr]               )
(            %      = write a '%' []                                         )
( </pre>                                                                     )


1 constant spf.version


( Private flags )

1  constant spf.zero-padding   \ 0
2  constant spf.left-justify   \ -
4  constant spf.space-sign     \ ' '
8  constant spf.plus-sign      \ +
16 constant spf.double         \ l


( Private format words )

: spf-left-pad     ( n1 n2 str -- n1 n2 = Pad n2 spaces to the left, if indicated by n1 )
  >r
  over spf.left-justify AND 0= IF
    bl over r@ str-append-chars
  THEN
  rdrop
;


: spf-zero-left-pad  ( n1 n2 str -- n1 n2 = Pad n2 spaces of zeros to the left, if indicated by n1 )
  >r
  over spf.left-justify AND 0= IF
    over spf.zero-padding AND IF
      [char] 0
    ELSE
      bl
    THEN
    over r@ str-append-chars
  THEN
  rdrop
;


: spf-right-pad    ( n1 n2 str -- = Pad n2 spaces to the right, if indicated by n1 )
  >r
  swap spf.left-justify AND IF
    bl swap r@ str-append-chars
  ELSE
    drop
  THEN
  rdrop
;


: spf+convert-char ( char n1 n2 -- char n1 n3 = Convert a char and determine the pad width n3 )
  1- 0 max
;


: spf+convert-string ( c-addr u n1 n2 -- c-addr u n1 n3 = Convert a string and determine the pad width n3 )
  >r over r> swap - 0 max
;


: spf+convert-signed ( n | d n1 n2 -- c-addr u n1 n3 = Convert a signed number and determine the pad width n3 )
  >r >r
  r@ spf.double AND 0= IF
    s>d                           \ Convert single to double
  THEN

  dup >r dabs <# #s r> 0< IF      \ Convert double to a string
    [char] - hold
  ELSE
    r@ spf.plus-sign AND IF
      [char] + hold
    ELSE r@ spf.space-sign AND IF
      bl hold
    THEN THEN 
  THEN #>
  r>                              \ Flags

  over r> swap - 0 max            \ Pad Width
;


: spf+convert-unsigned  ( u | ud n1 n2 -- c-addr u n1 n3 = Convert an unsigned number and determine the pad width n3 )
  over spf.double AND IF
    2swap
  ELSE
    rot 0                    \ Convert single to a double
  THEN

  <# #s #> 2swap             \ Convert double to string

  >r over r> swap - 0 max    \ Pad width
;


: spf-store-lower  ( c-addr u str -- = Store the string lower case in str )
  -rot
  bounds ?DO
    I c@ chr-lower over str-append-char
  LOOP
  drop
;


: spf-store-upper  ( c-addr u str -- = Store the string upper case in str )
  -rot
  bounds ?DO
    I c@ chr-upper over str-append-char
  LOOP
  drop
;


( Private state words )

0 value spf.check-format
0 value spf.check-flags
0 value spf.check-width
0 value spf.check-double
0 value spf.check-specifier


: spf-check-specifier ( i*x n1 n2 char str -- j*x str xt = Check for specifier, next state = check-format )
  >r
  CASE
    [char] d OF spf+convert-signed  r@ spf-zero-left-pad  2swap r@ str-append-string  r@ spf-right-pad ENDOF

    [char] i OF spf+convert-signed  r@ spf-zero-left-pad  2swap r@ str-append-string  r@ spf-right-pad ENDOF

    [char] u OF spf+convert-unsigned  r@ spf-zero-left-pad  2swap r@ str-append-string  r@ spf-right-pad ENDOF

    [char] x OF base @ >r  hex  spf+convert-unsigned  r> base !  r@ spf-zero-left-pad  2swap r@ spf-store-lower  r@ spf-right-pad ENDOF

    [char] X OF base @ >r  hex  spf+convert-unsigned  r> base !  r@ spf-zero-left-pad  2swap r@ spf-store-upper  r@ spf-right-pad ENDOF

    [char] c OF spf+convert-char  r@ spf-left-pad rot  r@ str-append-char  r@ spf-right-pad ENDOF

    [char] s OF spf+convert-string  r@ spf-left-pad 2swap  r@ str-append-string  r@ spf-right-pad ENDOF

    [char] n OF 2drop  r@ str-length@ swap ! ENDOF

    [char] o OF base @ >r  8 base !  spf+convert-signed  r> base !  r@ spf-zero-left-pad  2swap r@ str-append-string  r@ spf-right-pad ENDOF

    [char] p OF base @ >r  hex  spf+convert-unsigned  r> base !  r@ spf-zero-left-pad  2swap r@ spf-store-lower  r@ spf-right-pad ENDOF

    [char] % OF 2drop [char] % r@ str-append-char ENDOF

    [char] ? r@ str-append-char >r 2drop r>
  ENDCASE
  r>
  spf.check-format
;
' spf-check-specifier to spf.check-specifier


: spf-check-double ( n1 n2 char str -- n3 n2 str xt1 | str xt2 = Check for double, next states xt1 = check-specifier, xt2 = check-format )
  over [char] l = IF
    nip 2>r spf.double OR 2r>
    spf.check-specifier
  ELSE
    spf-check-specifier
  THEN
;
' spf-check-double to spf.check-double


: spf-check-width  ( n1 n2 char str -- n1 n3 str xt1 | .. | str xt3 = Check for width, next states xt1 = check-width, xt2 = ..., xt3 = check-format )
  over chr-digit? IF
    >r swap 10 * swap [char] 0 - + r>
    spf.check-width
  ELSE
    spf-check-double
  THEN
;
' spf-check-width to spf.check-width


: spf-check-flags  ( n1 char str -- n2 str xt1 | n1 n2 str xt2 | str xt3 = Check for flags, next states xt1 = check-flags, xt2 = .., xt3 = check-format )
  over [char] 0 = IF
    nip >r spf.zero-padding OR r>
    spf.check-flags
  ELSE over [char] - = IF
    nip >r spf.left-justify OR r>
    spf.check-flags
  ELSE over bl = IF
    nip >r spf.space-sign OR r>
    spf.check-flags
  ELSE over [char] + = IF
    nip >r spf.plus-sign OR r>
    spf.check-flags
  ELSE
    0 -rot spf-check-width
  THEN THEN THEN THEN
;
' spf-check-flags to spf.check-flags


: spf-check-format  ( char str -- str xt1 | 0 str xt2 = Check for format character, next states xt1 = check-format, xt2 = check-flags )
  over [char] % = IF
    nip 0 swap spf.check-flags
  ELSE
    tuck str-append-char
    spf.check-format
  THEN
;
' spf-check-format to spf.check-format


( Sprintf words )


: spf-append       ( i*x c-addr u str -- = Convert the arguments i*x with the format string c-addr u and append the result to str )
  spf.check-format 2swap
  bounds ?DO
    I c@ -rot execute
  LOOP
  2drop
;


: spf-set          ( i*x c-addr u str -- = Convert the arguments i*x with the format string c-addr u and set the result in str)
  dup str-clear spf-append
;


: spf"             ( "ccc<quote>" i*x str -- = Convert the arguments i*x with the format string and set the result in str )
  [char] " parse
  state @ IF
    postpone    sliteral
    ['] rot     compile,
    ['] spf-set compile,
  ELSE
    rot spf-set
  THEN
; immediate

[THEN]

\ ==============================================================================
