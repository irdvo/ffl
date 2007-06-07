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
\  $Date: 2007-06-07 08:56:28 $ $Revision: 1.3 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] sh2.version [IF]


cell 4 =  1 chars 1 =  AND [IF]

\ Based on the algoritms published in FIPS 180-1 and Wikipedia

include ffl/stc.fs


( sh2 = SHA-256 module )
( The sh2 module implements words for using the SHA-256 algorithm. )


1 constant sh2.version


( Private constants )

64       constant sh2.work%        \ Size of work buffer in cells

16 cells char/
         constant sh2.input%       \ Size of input buffer in chars
         
create sh2.k
hex
   428A2F98 , 71374491 , B5C0FBCF , E9B5DBA5 , 3956C25B , 59F111F1 , 923F82A4 , AB1C5ED5 ,
   D807AA98 , 12835B01 , 243185BE , 550C7DC3 , 72BE5D74 , 80DEB1FE , 9BDC06A7 , C19BF174 ,
   E49B69C1 , EFBE4786 , 0FC19DC6 , 240CA1CC , 2DE92C6F , 4A7484AA , 5CB0A9DC , 76F988DA ,
   983E5152 , A831C66D , B00327C8 , BF597FC7 , C6E00BF3 , D5A79147 , 06CA6351 , 14292967 ,
   27B70A85 , 2E1B2138 , 4D2C6DFC , 53380D13 , 650A7354 , 766A0ABB , 81C2C92E , 92722C85 ,
   A2BFE8A1 , A81A664B , C24B8B70 , C76C51A3 , D192E819 , D6990624 , F40E3585 , 106AA070 ,
   19A4C116 , 1E376C08 , 2748774C , 34B0BCB5 , 391C0CB3 , 4ED8AA4A , 5B9CCA4F , 682E6FF3 ,
   748F82EE , 78A5636F , 84C87814 , 8CC70208 , 90BEFFFA , A4506CEB , BEF9A3F7 , C67178F2 ,
decimal


( SHA-256 structure )

struct: sh2%   ( - n = Get the required space for the sha1 data structure )
  cell:  sh2>h0
  cell:  sh2>h1
  cell:  sh2>h2
  cell:  sh2>h3
  cell:  sh2>h4
  cell:  sh2>h5
  cell:  sh2>h6
  cell:  sh2>h7
  cell:  sh2>a               \ 
  cell:  sh2>b
  cell:  sh2>c
  cell:  sh2>d
  cell:  sh2>e
  cell:  sh2>f
  cell:  sh2>g
  cell:  sh2>h
 sh2.work%
  cells: sh2>work
 sh2.input%
  chars: sh2>input           \ input buffer with data
  cell:  sh2>length          \ total length of processed data
;struct


( SHA-1 structure creation, initialisation and destruction )

: sh2-init   ( w:sh2 - = Initialise the sh2 structure )
  [ hex ]
  6A09E667 over sh2>h0 !
  BB67AE85 over sh2>h1 !
  3C6EF372 over sh2>h2 !
  A54FF53A over sh2>h3 !
  510E527F over sh2>h4 !
  9B05688C over sh2>h5 !
  1F83D9AB over sh2>h6 !
  5BE0CD19 over sh2>h7 !
  [ decimal ]
  
  sh2>length 0!
;


: sh2-create   ( C: "name" -  R: - w:sh2 = Create a named sha-256 structure in the dictionary )
  create  here sh2% allot  sh2-init
;


: sh2-new   ( - w:sh2 = Create a new sha-256 structure on the heap )
  sh2% allocate  throw   dup sh2-init
;


: sh2-free   ( w:sh2 - = Free the sha-256 structure from the heap )
  free throw
;


( Private words )

[UNDEFINED] sha! [IF]
  bigendian? [IF]
: sha!             ( w addr - = Store word on address, SHA order )
  postpone !
; immediate
: sha@             ( addr - w = Fetch word on address, SHA order )
  postpone @
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


: sh2-cmove   ( c-addr u w:sh2 - n:len f:full = Move characters from the string to the input buffer, update the length )
  2dup sh2>length @ sh2.input% mod     \ index = sh2>length mod buf-size
  tuck + sh2.input% >= >r >r           \ full  = (index + str-len >= buf-size )
  swap sh2.input% r@ - min             \ copy-len = min(buf-size - index, str-len)
  2dup swap sh2>length +!              \ sh2>length += copy-len
  r> swap >r
  chars swap sh2>input + r@ cmove      \ copy(str->buf,copy-len)
  r> r>
;


: sh2-transform   ( w:sh2 - = Transform 64 bytes of data )
  >r
  
  r@ sh2>work
  r@ sh2>input 16 cell bounds DO       \ Move input (bigendian) in work buffer
    I sha@ over !
    cell+
  cell +LOOP                           \ S: sh2>work + 16 cells
     
    
  48 cells bounds DO                   \ Extend 16 words in work buffer to 64 words in work buffer
    I 16 cells - @                     \ w[i] = w[i-16] + ..
    I 15 cells - @
     dup   7 rroll
     over 18 rroll  xor
     swap 3  rshift xor +              \ .. + (w[i-15] rotr 7) xor (w[i-15] rotr 18) xor (w[i-15] rshift 3) + ..
    I 7  cells - @ +                   \ .. + w[i-7] + ..
    I 2  cells - @
    dup  17 rroll
    over 19 rroll  xor
    swap 10 rshift xor +               \ .. + (w[i-2] rotr 17) xor (w[i-2] rotr 19) xor (w[i-2] rshift 10)
    I !
  cell +LOOP
    
  r@ sh2>h0 r@ sh2>a 
  8 cells char/ move                   \ Initialise hash values: h0..h7 -> a..h
  
  r@
  sh2.work% cells 0 DO
    I swap >r
    
    r@ sh2>a @                         \ s0 = (a rotr 2) xor (a rotr 12) xor (a rotr 22)
    dup   2 rroll
    over 12 rroll xor
    over 22 rroll xor
    swap
    
    r@ sh2>b @                         \ maj = (a and b) xor (a and c) xor (b and c)
    2dup
    r@ sh2>c @
    tuck
    and >r and >r and r> xor r> xor
    +                                  \ t1 = s0 + maj
    
    swap dup
    sh2.k + @                          \ k[i]
    swap r@ sh2>work + @               \ w[i]
    +                                  \ w[i] + k[i]
    r@ sh2>h @ +                       \ w[i] + k[i] + h
    
    r@ sh2>e @                         \ s1 = (e rotr 6) xor (e rotr 11) xor (e rotr 25)
    dup   6 rroll
    over 11 rroll xor
    over 25 rroll xor
    
    swap                               \ ch = (e and f) xor ((not e) and g)
    dup r@ sh2>f @ and
    swap invert r@ sh2>g @ and xor
    
    + +                                \ t2 = w[i] + k[i] + h + s1 + ch
    
    over + r@ sh2>a
    tuck @! swap cell+                 \ a = t1 + t2
    tuck @! swap cell+                 \ b = a
    tuck @! swap cell+                 \ c = b
    tuck @! rot + swap cell+           \ d = c
    tuck @! swap cell+                 \ e = d + t1
    tuck @! swap cell+                 \ f = e
    tuck @! swap cell+                 \ g = f
    !                                  \ h = g
    
    r>
  cell +LOOP
    
  r> sh2>a 8 cells bounds DO           \ Add hash values to current results
    I @ over +!
    cell+
  cell +LOOP
    
;


: sh2+pad   ( w:index w:buffer - = Pad the buffer )
  over chars +
  128 over c!                       \ Add 80h to buffer
  char+ 
  swap 1+ sh2.input% swap - chars   \ Pad remaining with zero's
  erase
;


: sh2+#s   ( u - Put a single SHA-256 result in the hold area )
  0 # # # # # # # # 2drop
;


( SHA-256 words )

: sh2-reset   ( w:sh2 - = Reset the SHA-256 state )
  sh2-init
;


: sh2-update   ( c-addr u w:sh2 - = Update the SHA-256 with more data )
  >r
  BEGIN
    2dup r@ sh2-cmove
  WHILE
    r@ sh2-transform
    /string
  REPEAT
  r> 2drop 2drop
;


: sh2-finish   ( w:sh2 - u1 u2 u3 u4 u5 u6 u7 u8 = Finish the SHA-256 calculation )
  >r
  
  r@ sh2>length @ sh2.input% mod            \ index = sh2>length mod buf-size
  
  dup [ sh2.input% 2 cells - 1 chars - ] literal > IF
    r@ sh2>input sh2+pad                    \ If buffer is too full Then
    r@ sh2-transform                        \   Pad buffer and tranform
    r@ sh2>input sh2.input% chars erase     \   Pad next buffer
  ELSE                                      \ Else
    r@ sh2>input sh2+pad                    \   Pad buffer
  THEN
  
  r@ sh2>length @ sys.bits-in-char m*       \ Calculate bit length
  
  [ sh2.input% 2 cells - ] literal chars    \ Index for bit length
  r@ sh2>input +                            \ Buffer location for bit length
  
  tuck sha! cell+ sha!                      \ Store the length
  
  r@ sh2-transform                          \ Transform last buffer
  
  r@ sh2>h0 @
  r@ sh2>h1 @
  r@ sh2>h2 @
  r@ sh2>h3 @
  r@ sh2>h4 @
  r@ sh2>h5 @
  r@ sh2>h6 @
  r> sh2>h7 @
;


: sh2+to-string   ( u1 u2 u3 u4 u5 u6 u7 u8 - c-addr u = Convert SHA-256 result to string, using the pictured output area )
  base @ >r hex
  <#  sh2+#s sh2+#s sh2+#s sh2+#s sh2+#s sh2+#s sh2+#s sh2+#s 0. #>
  r> base !
;


( Inspection )

: sh2-dump   ( w:sh2 - = Dump the sh2 state )
  >r
  ." sh2:" r@ . cr
  ."  result :" r@ sh2>h0 @ r@ sh2>h1 @ r@ sh2>h2 @ r@ sh2>h3 @ r@ sh2>h4 @ sh2>h5 @ sh2>h6 @ sh2>h7 @ sh2+to-string type cr
  ."  length :" r@ sh2>length ? cr
  ."  buffer :" r@ sh2>input sh2.input% chars dump
  ."  work   :" r> sh2>work sh2.work% cells dump
;

[ELSE]
.( Warning: sh2 requires 4 byte cells and 1 byte chars ) cr
[THEN]

[THEN]

\ ==============================================================================
