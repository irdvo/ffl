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
\  $Date: 2006-12-11 18:00:41 $ $Revision: 1.6 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] sh1.version [IF]


cell 4 =  1 chars 1 =  AND [IF]

\ Based on the algoritms published in FIPS 180-1 and Wikipedia

include ffl/stc.fs


( sh1 = SHA-1 module )
( The sh1 module implements words for using the SHA-1 algorithm. )


1 constant sh1.version


( Private constants )

80       constant sh1.w-size       \ Size of work buffer in cells


16       constant sh1.b-size       \ Size of input buffer in cells
sh1.b-size cells 1 chars /
         constant sh1.b-csize      \ Size of input buffer in chars
         
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
  cells: sh1>b               \ input buffer with data
  cell:  sh1>length          \ total length of processed data
;struct


( SHA-1 structure creation, initialisation and destruction )

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


: sh1-free         ( w:sh1 - = Free the sha-1 structure from the heap )
  free throw
;


( Private words )

sys.bigendian [IF]
: sha!             ( w addr - = Store word on address, SHA1 order )
  !
;
: sha@             ( addr - w = Fetch word on address, SHA1 order )
  @
;
[ELSE]
: sha!
        over 24 rshift 255 and over c!
  char+ over 16 rshift 255 and over c!
  char+ over 8  rshift 255 and over c!
  char+ swap           255 and swap c!
;
: sha@
  dup                  c@ 24 lshift 
  swap char+ swap over c@ 16 lshift or
  swap char+ swap over c@  8 lshift or
  swap char+           c@           or
;
[THEN]


: sh1-w-bounds     ( u:end u:start w:sh1 - u:addr-end u:addr-start = Bounds work buffer from start to end )
  sh1>w >r
  swap cells r@ + 
  swap cells r> +
;


: sh1+rotate       ( e d c b a f k w - d c b a t = Rotate the sha-1 state )
  + +                        \ t = f + k + w
  over 5 lroll +             \ t += a lroll 5
  >r swap 30 lroll swap      \ b lroll 30
  4 roll r> +                \ rotate the state t += e
;


: sh1-cmove        ( c-addr u w:sh1 - n:len f:full = Move characters from the string to the input buffer, update the length )
  2dup sh1>length @ sh1.b-csize mod    \ index = sh1>length mod buf-size
  tuck + sh1.b-csize >= >r >r          \ full  = (index + str-len >= buf-size )
  swap sh1.b-csize r@ - min            \ copy-len = min(buf-size - index, str-len)
  2dup swap sh1>length +!              \ sh1>length += copy-len
  r> swap >r
  chars swap sh1>b + r@ cmove          \ copy(str->buf,copy-len)
  r> r>
;


: sh1-transform    ( w:sh1 - = Transform 64 bytes of data )
  >r
  
  r@ sh1>b
  16 0 r@ sh1-w-bounds DO       \ Move chunk in work buffer
    dup sha@ I !
    cell+
  cell +LOOP
  drop
    
  80 16 r@ sh1-w-bounds DO      \ Extend 16  words in work buffer to 80 words in work buffer
    I 3  cells - @
    I 8  cells - @ xor
    I 14 cells - @ xor
    I 16 cells - @ xor
      1 lroll
    I ! 
  cell +LOOP
  
  r@ sh1>h4 @                  \ Initialise hash values
  r@ sh1>h3 @  
  r@ sh1>h2 @ 
  r@ sh1>h1 @ 
  r@ sh1>h0 @                  \ S: e d c b a
  
  20 0 r@ sh1-w-bounds DO      \ Transform 0..19
    >r >r
    over r@ invert and         \ S: e d c d&~b
    over r@ and or             \ S: e d c f = d & ~b | c & b
    r> swap r> swap            \ S: e d c b a f
    sh1.k0 I @ sh1+rotate 
  cell +LOOP
  
  40 20 r@ sh1-w-bounds DO     \ Transform 20..39
    >r >r
    2dup xor                   \ S: e d c f = c ^ d
    r> tuck xor                \ S: e d c b f = b ^ c ^ d
    r> swap                    \ S: e d c b a f
    sh1.k1 I @ sh1+rotate 
  cell +LOOP

  60 40 r@ sh1-w-bounds DO     \ Transform 40..59
    >r >r
    2dup over r@ and >r        \ S: e d c d    R : a b f = b & d
    and r> or                  \ S: e d c f = b & d | c & d
    over r@ and or             \ S: e d c f = b & d | c & d | b & c  
    r> swap r> swap            \ S: e d c b a f
    sh1.k2 I @ sh1+rotate
  cell +LOOP
  
  80 60 r@ sh1-w-bounds DO     \ Transform 60..79
    >r >r
    2dup xor
    r> tuck xor
    r> swap 
    sh1.k3 I @ sh1+rotate
  cell +LOOP
  
  r@ sh1>h0 +!                 \ Add hash values to current results
  r@ sh1>h1 +!
  r@ sh1>h2 +!
  r@ sh1>h3 +!
  r> sh1>h4 +!
;


: sh1+pad      ( w:index w:buffer - = Pad the buffer )
  over chars +
  128 over c!                       \ Add 80h to buffer
  char+ 
  swap 1+ sh1.b-csize swap - chars  \ Pad remaining with zero's
  erase
;


: sh1+#s       ( u - Put a single SHA-1 result in the hold area )
  0 # # # # # # # # 2drop
;


( SHA-1 words )

: sh1-reset        ( w:sh1 - = Reset the SHA-1 state )
  sh1-init
;


: sh1-update       ( c-addr u w:sh1 - = Update the SHA-1 with more data )
  >r
  BEGIN
    2dup r@ sh1-cmove
  WHILE
    r@ sh1-transform
    /string
  REPEAT
  r> 2drop 2drop
;


: sh1-finish       ( w:sh1 - u1 u2 u3 u4 u5 = Finish the SHA-1 calculation )
  >r
  
  r@ sh1>length @ sh1.b-csize mod           \ index = sh1>length mod buf-size
  
  dup [ sh1.b-csize 2 cells - 1 chars - ] literal > IF
    r@ sh1>b sh1+pad                        \ If buffer is too full Then
    r@ sh1-transform                        \   Pad buffer and tranform
    r@ sh1>b sh1.b-csize chars erase        \   Pad next buffer
  ELSE                                      \ Else
    r@ sh1>b sh1+pad                        \   Pad buffer
  THEN
  
  r@ sh1>length @ sys.bits-in-char m*       \ Calculate bit length
  
  [ sh1.b-csize 2 cells - ] literal chars   \ Index for bit length
  r@ sh1>b +                                \ Buffer location for bit length
  
  tuck sha! cell+ sha!                      \ Store the length
  
  r@ sh1-transform                          \ Transform last buffer
  
  r@ sh1>h0 @
  r@ sh1>h1 @
  r@ sh1>h2 @
  r@ sh1>h3 @
  r> sh1>h4 @
;


: sh1+to-string    ( u1 u2 u3 u4 u5 - c-addr u = Convert SHA-1 result to string, using the pictured output area )
  base @ >r hex
  <#  sh1+#s sh1+#s sh1+#s sh1+#s sh1+#s 0. #>
  r> base !
;


( Inspection )

: sh1-dump         ( w:sh1 - = Dump the sh1 state )
  >r
  ." sh1:" r@ . cr
  ."  result :" r@ sh1>h0 @ r@ sh1>h1 @ r@ sh1>h2 @ r@ sh1>h3 @ r@ sh1>h4 @ sh1+to-string type cr
  ."  length :" r@ sh1>length ? cr
  ."  buffer :" r@ sh1>b sh1.b-csize chars dump
  ."  work   :" r> sh1>w sh1.w-size  cells dump
;

[ELSE]
.( Warning: sh1 requires 4 byte cells and 1 byte chars ) cr
[THEN]

[THEN]

\ ==============================================================================