\ ==============================================================================
\
\                  b64 - the Base64 module in the ffl
\
\               Copyright (C) 2009  Dick van Oudheusden
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
\  $Date: 2007-12-09 07:23:17 $ $Revision: 1.8 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] b64.version [IF]


1 chars 1 = [IF]

\ Based on the algorithms published in FIPS 180-2 and Wikipedia

include ffl/str.fs


( b64 = Base64 module )
( The b64 module implements the encoding to and decoding from base64.        )


1 constant b64.version


( Private words )

: b64.codes        ( -- c-addr u = Get the base64 codes )
  \  01234567890123456789012345678901234567890123456789012345678901234
  s" ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/="
;


: b64+encode-char  ( char str -- = Encode char and append it to str )
  >r chars b64.codes drop + c@ r> str-append-char
;

nil value b64.decodes


: b64+decode-char  ( c-addr1 u1 u -- c-addr2 u2 ch = Read and decode char, throw exp-invalid-data if out of range )
  >r
  dup IF
    over c@
    >r 1 /string r>
  ELSE
    [char] =
  THEN
  chars b64.decodes + c@
  dup r> u> exp-invalid-data AND throw
;


( Base64 conversion words )

: b64-encode       ( c-addr1 u1 str -- c-addr2 u2 = Encode the string c-addr u with str, resulting in c-addr2 u2 in str )
  >r
  r@ str-clear
  tuck 3 /                   \ the number of full sets
  dup  1+ 4 * r@ str-size!   \ resize the string only once
  BEGIN
    dup
  WHILE
    swap                     \ decode three at a time
    dup c@ dup 2 rshift r@ b64+encode-char
    4 lshift [ hex ] 30 [ decimal ] AND swap char+ tuck c@ tuck 4 rshift OR r@ b64+encode-char
    2 lshift [ hex ] 3C [ decimal ] AND swap char+ tuck c@ tuck 6 rshift OR r@ b64+encode-char
    [ hex ] 3F [ decimal ] AND r@ b64+encode-char
    char+
    swap
    1-
  REPEAT
  drop
  
  swap 3 mod
  ?dup IF                    \ decode the remaining
    over c@ dup 2 rshift r@ b64+encode-char
    swap 1 = IF
      4 lshift [ hex ] 30 [ decimal ] AND r@ b64+encode-char
      [char] = r@ str-append-char
    ELSE
      4 lshift [ hex ] 30 [ decimal ] AND over char+ c@ tuck 4 rshift OR r@ b64+encode-char
      2 lshift [ hex ] 3C [ decimal ] AND r@ b64+encode-char
    THEN
    [char] = r@ str-append-char
  THEN
  drop
  r> str-get
;


: b64-decode       ( c-addr1 u1 str -- c-addr2 u2 = Decode the string c-addr1 u1 using str, resulting in c-addr2 u2 in str, throws exp-invalid-data for characters out of range )
  >r
  r@ str-clear
  dup 4 / 3 * r@ str-size!

  b64.decodes nil= IF             \ Initialise the b64.decodes array once
    256 chars allocate throw to b64.decodes
    b64.decodes 256 255 fill      \ Fill initial with error codes
    b64.codes 
    0 DO
      I over c@ chars b64.decodes + c!
      char+
    LOOP
    drop
  THEN
  
  BEGIN                           \ Iterate the string
    dup
  WHILE
    63 b64+decode-char            \ '=' not allowed
    2 lshift >r
    63 b64+decode-char            \ '=' not allowed
    dup 4 rshift r> OR r@ str-append-char  \ byte1 = ch1 << 2 | ch2 >> 4
    
    4 lshift >r
    64 b64+decode-char            \ '=' allowed
    r> over 64 u< IF
      over 2 rshift OR r@ str-append-char  \ byte2 = ch2 << 4 | ch3 >> 2
      
      6 lshift >r
      64 b64+decode-char          \ '=' allowed
      r> over 64 u< IF
        OR r@ str-append-char     \ byte3 = ch3 << 6 | ch4
      ELSE
        2drop
      THEN
    
    ELSE
      2drop
      64 b64+decode-char drop     \ '=' allowed
    THEN
  REPEAT
  2drop
  r> str-get
;

[ELSE]
.( Warning: b64 requires 1 byte chars ) cr
[THEN]

[THEN]

\ ==============================================================================
