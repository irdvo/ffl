\ ==============================================================================
\
\                  sh2 - the SHA-256 module in the ffl
\
\               Copyright (C) 2007  Dick van Oudheusden
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


[UNDEFINED] sh2.version [IF]


cell 4 >=  1 chars 1 =  AND [IF]

\ Based on the algorithms published in FIPS 180-2 and Wikipedia

include ffl/stc.fs
include ffl/fwt.fs

( sh2 = SHA-256 module )
( The sh2 module implements the SHA-256 algorithm.                           )


1 constant sh2.version


( Private constants )

64 constant sh2.work%        \ Size of work buffer in longs

16 #bytes/long * char/
   constant sh2.input%       \ Size of input buffer in chars
         
create sh2.k
hex
   428A2F98 l, 71374491 l, B5C0FBCF l, E9B5DBA5 l, 3956C25B l, 59F111F1 l, 923F82A4 l, AB1C5ED5 l,
   D807AA98 l, 12835B01 l, 243185BE l, 550C7DC3 l, 72BE5D74 l, 80DEB1FE l, 9BDC06A7 l, C19BF174 l,
   E49B69C1 l, EFBE4786 l, 0FC19DC6 l, 240CA1CC l, 2DE92C6F l, 4A7484AA l, 5CB0A9DC l, 76F988DA l,
   983E5152 l, A831C66D l, B00327C8 l, BF597FC7 l, C6E00BF3 l, D5A79147 l, 06CA6351 l, 14292967 l,
   27B70A85 l, 2E1B2138 l, 4D2C6DFC l, 53380D13 l, 650A7354 l, 766A0ABB l, 81C2C92E l, 92722C85 l,
   A2BFE8A1 l, A81A664B l, C24B8B70 l, C76C51A3 l, D192E819 l, D6990624 l, F40E3585 l, 106AA070 l,
   19A4C116 l, 1E376C08 l, 2748774C l, 34B0BCB5 l, 391C0CB3 l, 4ED8AA4A l, 5B9CCA4F l, 682E6FF3 l,
   748F82EE l, 78A5636F l, 84C87814 l, 8CC70208 l, 90BEFFFA l, A4506CEB l, BEF9A3F7 l, C67178F2 l,
decimal


( SHA-256 structure )

begin-structure sh2%   ( -- n = Get the required space for a sha2 variable )
  lfield:   sh2>h0
  lfield:   sh2>h1
  lfield:   sh2>h2
  lfield:   sh2>h3
  lfield:   sh2>h4
  lfield:   sh2>h5
  lfield:   sh2>h6
  lfield:   sh2>h7
  lfield:   sh2>a
  lfield:   sh2>b
  lfield:   sh2>c
  lfield:   sh2>d
  lfield:   sh2>e
  lfield:   sh2>f
  lfield:   sh2>g
  lfield:   sh2>h
  sh2.work%
  lfields:  sh2>work          \ work buffer
  sh2.input%
  cfields: sh2>input         \ input buffer with data
  field:   sh2>length        \ total length of processed data
end-structure


( SHA-256 variable creation, initialisation and cleanup )

: sh2-init   ( sh2 -- = Initialise the sh2 variable )
  [ hex ]
  6A09E667 over sh2>h0 l!
  BB67AE85 over sh2>h1 l!
  3C6EF372 over sh2>h2 l!
  A54FF53A over sh2>h3 l!
  510E527F over sh2>h4 l!
  9B05688C over sh2>h5 l!
  1F83D9AB over sh2>h6 l!
  5BE0CD19 over sh2>h7 l!
  [ decimal ]
  
  sh2>length 0!
;


: sh2-create   ( "<spaces>name" -- ; -- sh2 = Create a named SHA-256 variable in the dictionary )
  create  here sh2% allot  sh2-init
;


: sh2-new   ( -- sh2 = Create a new SHA-256 variable on the heap )
  sh2% allocate  throw   dup sh2-init
;


: sh2-free   ( sh2 -- = Free the SHA-256 variable from the heap )
  free throw
;


( Private words )

[UNDEFINED] sha! [IF]
  bigendian? [IF]
: sha!             ( x addr -- = Store long on address, SHA order )
  postpone l!
; immediate
: sha@             ( addr -- x = Fetch long on address, SHA order )
  postpone l@
; immediate
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
[THEN]


: sh2-cmove   ( c-addr u sh2 -- n flag = Move characters from the string to the input buffer, update the length, return length and full indication )
  2dup sh2>length @ sh2.input% mod     \ index = sh2>length mod buf-size
  tuck + sh2.input% >= >r >r           \ full  = (index + str-len >= buf-size )
  swap sh2.input% r@ - min             \ copy-len = min(buf-size - index, str-len)
  2dup swap sh2>length +!              \ sh2>length += copy-len
  r> swap >r
  chars swap sh2>input + r@ cmove      \ copy(str->buf,copy-len)
  r> r>
;


: sh2-transform   ( sh2 -- = Transform 64 bytes of data )
  >r
  
  r@ sh2>work
  r@ sh2>input 16 #bytes/long * bounds DO   \ Move input (bigendian) in work buffer
    I sha@ over l!
    #bytes/long +
  #bytes/long +LOOP                         \ S: sh2>work + 16 longs
     
  48 #bytes/long * bounds DO                \ Extend 16 words in work buffer to 64 words in work buffer
    I 16 #bytes/long * - l@                 \ w[i] = w[i-16] + ..
    I 15 #bytes/long * - l@
    dup   7 lrroll
    over 18 lrroll xor
    swap 3  rshift xor +                    \ .. + (w[i-15] rotr 7) xor (w[i-15] rotr 18) xor (w[i-15] rshift 3) + ..
    I 7  #bytes/long * - l@ +               \ .. + w[i-7] + ..
    I 2  #bytes/long * - l@
    dup  17 lrroll
    over 19 lrroll xor
    swap 10 rshift xor +                    \ .. + (w[i-2] rotr 17) xor (w[i-2] rotr 19) xor (w[i-2] rshift 10)
    I l!
  #bytes/long +LOOP
    
  r@ sh2>h0 r@ sh2>a 
  8 #bytes/long * char/ move                \ Initialise hash values: h0..h7 -> a..h
  
  r>
  sh2.work% #bytes/long * 0 DO
    sh2.k I + l@                            \ k[i] + ..
    over sh2>work I + l@ +                  \ .. + w[i] + ..
    over sh2>h l@ +                         \ w[i] + k[i] + h
    
    swap >r                                 \ done with I, save sh2
  
    r@ sh2>e l@                             \ s1 = (e rotr 6) xor (e rotr 11) xor (e rotr 25)
    dup   6 lrroll
    over 11 lrroll xor
    over 25 lrroll xor
    
    swap                                    \ ch = (e and f) xor ((not e) and g)
    dup r@ sh2>f l@ and
    swap invert r@ sh2>g l@ and xor
    + +                                     \ t1 = w[i] + k[i] + h + s1 + ch

    r@ sh2>a l@                             \ s0 = (a rotr 2) xor (a rotr 12) xor (a rotr 22)
    dup   2 lrroll
    over 13 lrroll xor
    over 22 lrroll xor
    swap
    
    r@ sh2>b l@                             \ maj = (a and b) xor (a and c) xor (b and c)
    2dup
    r@ sh2>c l@
    tuck
    and >r and >r and r> xor r> xor
    +                                       \ t2 = s0 + maj
    
    over + r@ sh2>a
    tuck l@! swap #bytes/long +             \ a = t1 + t2
    tuck l@! swap #bytes/long +             \ b = a
    tuck l@! swap #bytes/long +             \ c = b
    tuck l@! rot + swap #bytes/long +       \ d = c
    tuck l@! swap #bytes/long +             \ e = d + t1
    tuck l@! swap #bytes/long +             \ f = e
    tuck l@! swap #bytes/long +             \ g = f
    l!                                      \ h = g
    
    r>
  #bytes/long +LOOP
  
  dup sh2>h0
  swap sh2>a 8 #bytes/long * bounds DO     \ Add hash values to current results
    I l@ over l+!
    #bytes/long +
  #bytes/long +LOOP
  drop
;


: sh2+pad   ( n c-addr -- = Pad the buffer c-addr, starting from index n )
  over chars +
  128 over c!                       \ Add 80h to buffer
  char+ 
  swap 1+ sh2.input% swap - chars   \ Pad remaining with zero's
  erase
;

cell 4 = [IF]
: sh2-length!  ( sh2 -- = Store the length as bit length in sha order )
  >r
  r@ sh2>length @ #bits/char m*                     \ Calculate bit length
  
  [ sh2.input% 2 #bytes/long * - ] literal chars    \ Index for bit length
  r> sh2>input +                                    \ Buffer location for bit length
  
  tuck sha! #bytes/long + sha!                      \ Store the length
;
[ELSE]
: sh2-length!  ( sh2 -- = Store the length as bit length in sha order )
  >r
  r@ sh2>length @ #bits/char *                      \ Calculate bit length
  
  dup u>l swap [ #bytes/long #bits/byte * ] literal rshift   \ Split in lsl msl

  [ sh2.input% 2 #bytes/long * - ] literal chars    \ Index for bit length
  r> sh2>input +                                    \ Buffer location for bit length
  
  tuck sha! #bytes/long + sha!                      \ Store the length
;
[THEN]

[UNDEFINED] sha+#s [IF]
: sha+#s   ( u -- Put a single SHA result in the hold area )
  0 # # # # # # # # 2drop
;
[THEN]


( SHA-256 words )

: sh2-reset   ( sh2 -- = Reset the SHA-256 state )
  sh2-init
;


: sh2-update   ( c-addr u sh2 -- = Update the SHA-256 with more data c-addr u )
  >r
  BEGIN
    2dup r@ sh2-cmove
  WHILE
    r@ sh2-transform
    /string
  REPEAT
  r> 2drop 2drop
;


: sh2-finish   ( sh2 -- u1 u2 u3 u4 u5 u6 u7 u8 = Finish the SHA-256 calculation, return the result )
  >r
  
  r@ sh2>length @ sh2.input% mod            \ index = sh2>length mod buf-size
  
  dup [ sh2.input% 2 #bytes/long * - 1 chars - ] literal > IF
    r@ sh2>input sh2+pad                    \ If buffer is too full Then
    r@ sh2-transform                        \   Pad buffer and transform
    r@ sh2>input sh2.input% chars erase     \   Pad next buffer
  ELSE                                      \ Else
    r@ sh2>input sh2+pad                    \   Pad buffer
  THEN
  
  r@ sh2-length!
  
  r@ sh2-transform                          \ Transform last buffer
  
  r@ sh2>h0 l@
  r@ sh2>h1 l@
  r@ sh2>h2 l@
  r@ sh2>h3 l@
  r@ sh2>h4 l@
  r@ sh2>h5 l@
  r@ sh2>h6 l@
  r> sh2>h7 l@
;


: sh2+to-string   ( u1 u2 u3 u4 u5 u6 u7 u8 -- c-addr u = Convert SHA-256 result to the string c-addr u, using the pictured output area )
  base @ >r hex
  <#  sha+#s sha+#s sha+#s sha+#s sha+#s sha+#s sha+#s sha+#s 0. #>
  r> base !
;


( Inspection )

: sh2-dump   ( sh2 -- = Dump the sh2 variable )
  >r
  ." sh2:" r@ . cr
  ."  result :" r@ sh2>h0 l@ r@ sh2>h1 l@ r@ sh2>h2 l@ r@ sh2>h3 l@ r@ sh2>h4 l@ r@ sh2>h5 l@ r@ sh2>h6 l@ r@ sh2>h7 l@ sh2+to-string type cr
  ."  length :" r@ sh2>length ? cr
  ."  buffer :" r@ sh2>input sh2.input% chars dump
  ."  work   :" r> sh2>work sh2.work% #bytes/long * dump
;

[ELSE]
.( Warning: sh2 requires at least 4 byte cells and 1 byte chars ) cr
[THEN]

[THEN]

\ ==============================================================================
