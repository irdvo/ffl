\ ==============================================================================
\
\                  sh1 - the SHA-1 module in the ffl
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
\  $Date: 2006-12-04 19:55:43 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] sh1.version [IF]


include ffl/stc.fs


( sh1 = SHA-1 module )
( The sh1 module implements words for using the SHA-1 algorithm.                )


1 constant sh1.version


( Private constants )

80 constant sh1.w-size
16 constant sh1.b-size
hex
5A827999 constant sh1.k0
6ED9EBA1 constant sh1.k1
8F1BBCDC constant sh1.k2
CA62C1D6 constant sh1.k3
decimal


( SHA-1 structure )

struct: sh1%       ( - n = Get the required space for the sha1 data structure )
  cell:  sh1>h0
  cell:  sh1>h1
  cell:  sh1>h2
  cell:  sh1>h3
  cell:  sh1>h4
 sh1.w-size
  cells: sh1>w
 sh1.b-size
  cells: sh1>b               \ buffer with data
  cell:  sh1>length          \ total length of processed data
;struct


( Structure creation, initialisation and destruction )

: sh1-init         ( w:sh1 - = Initialise the sh1 structure )
  [ hex ]
  67452301 over sh1>h0 !
  EFCDAB89 over sh1>h1 !
  98BADCFE over sh1>h2 !
  10325476 over sh1>h3 !
  C3D2E1F0 over sh1>h4 !
  [ decimal ]
  
  sh1>length 0!
;


: sh1-create       ( C: "name" -  R: - w:sh1 = Create a named sha-1 structure in the dictionary )
  create  here sh1% allot  sh1-init
;


: sh1-new          ( - w:sh1 = Create a new sha-1 structure on the heap )
  sh1% allocate  throw   dup sh1-init
;


: sh1-free         ( w:sh1 - = Free the tree node from the heap )
  free throw
;


( Private words )

: sh1+rotate       ( e d c b a f k w - d c b a t = Rotate the sha-1 state )
  + +                        \ t = f + k + w
  over 5 lroll +             \ t += a lroll 5
  >r swap 30 lroll swap      \ b lroll 30
  4 roll r> +                \ rotate the state t += e
;


: sh1-transform    ( c-addr w:sh1 - = Transform 64 bytes of data )
  >r
  
  \ ToDo ..
  
  r@ sh1>h4 @   
  r@ sh1>h3 @  
  r@ sh1>h2 @ 
  r@ sh1>h1 @ 
  r@ sh1>h0 @                \ S: e d c b a
  
  \ ToDo ...
  
  \ 0..19
  DO
    >r >r
    over r@ invert and         \ S: e d c d&~b
    over r@ and or             \ S: e d c f = d & ~b | c & b
    r> swap r> swap            \ S: e d c b a f
    sh1.k0 I sh1+rotate 
  LOOP
  
  \ ToDo ...
  
  \ 20..39
  DO
    >r >r
    2dup xor                   \ S: e d c f = c ^ d
    r> tuck xor                \ S: e d c b f = b ^ c ^ d
    r> swap                    \ S: e d c b a f
    sh1.k1 I sh1+rotate 
  LOOP

  \ ToDo ...
  
  \ 40..59
  DO
    >r >r
    2dup over r@ and >r        \ S: e d c d    R : a b f = b & d
    and r> or                  \ S: e d c f = b & d | c & d
    over r@ and or             \ S: e d c f = b & d | c & d | b & c  
    r> swap r> swap            \ S: e d c b a f
    sh1.k2 I sh1+rotate
  LOOP
  
  \ ToDo ...
  
  \ 60..79
  DO
    >r >r
    2dup xor
    r> tuck xor
    r> swap 
    sh1.k3 I sh1+rotate
  LOOP
  
  \ ToDo ...
  
  r@ sh1>h0 +!
  r@ sh1>h1 +!
  r@ sh1>h2 +!
  r@ sh1>h3 +!
  r> sh1>h4 +!
;


( SHA-1 words )

: sh1-reset        ( w:sh1 - = Reset the SHA-1 state )
  sh1-init
;


: sh1-update       ( c-addr u w:sh1 - = Update the SHA-1 with more data )
  \ ToDo
;


: sh1-finish       ( w:sh1 - u1 u2 u3 u4 u5 = Finish the SHA-1 calculation )
  >r
  
  \ ToDo
  
  r@ sh1>h0 @
  r@ sh1>h1 @
  r@ sh1>h2 @
  r@ sh1>h3 @
  r> sh1>h4 @
;

: sh1+to-string    ( u1 u2 u3 u4 u5 - c-addr u = Convert SHA-1 result to string, using the pictured output area )
  \ ToDo
;

( Inspection )

: sh1-dump         ( w:sh1 - = Dump the sh1 state )
  >r
  ." sh1:" r@ . cr
  ."  result :" r@ sh1>h0 @ r@ sh1>h1 @ r@ sh1>h2 @ r@ sh1>h3 @ r@ sh1>h4 @ sh1+to-string type cr
  ."  length :" r@ sh1>length ? cr
  ."  buffer :" r@ sh1>w r> sh1>length @ 64 min dump
;

[THEN]

\ ==============================================================================
