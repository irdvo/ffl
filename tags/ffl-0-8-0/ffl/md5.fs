\ ==============================================================================
\
\                  md5 - the MD5 algorithm in the ffl
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
\  $Date: 2007-12-09 07:23:16 $ $Revision: 1.14 $
\
\ ==============================================================================

\
\ This module uses ideas and code from Fedrick Warren, Marcel Hendrix and Ulrich Hoffmann
\

include ffl/config.fs


[UNDEFINED] md5.version [IF]


cell 4 >=  1 chars 1 =  AND [IF]

include ffl/stc.fs
include ffl/fwt.fs


( md5 = MD5 Message Digest Algorithm )
( The md5 module implements the MD5 algorithm.)
  

1 constant md5.version


( Private constants )

16 constant md5.long-size   ( -- n = Size of buffer in longs )
64 constant md5.byte-size   ( -- n = Size of buffer in bytes )


( MD5 Structure )

begin-structure md5%       ( -- n = Get the required space for a md5 variable )
  lfield:  md5>a
  lfield:  md5>b
  lfield:  md5>c
  lfield:  md5>d
 md5.long-size 
  lfields: md5>buffer
  field:   md5>length       
end-structure


( Private words )

\ MD5 Transform constants
7  constant md5.s11  
12 constant md5.s12  
17 constant md5.s13  
22 constant md5.s14
5  constant md5.s21
9  constant md5.s22  
14 constant md5.s23  
20 constant md5.s24
4  constant md5.s31
11 constant md5.s32  
16 constant md5.s33  
23 constant md5.s34
6  constant md5.s41
10 constant md5.s42  
15 constant md5.s43  
21 constant md5.s44


: md5+f            ( x y z -- r = basic MD5 F function )
  invert and or + +
;


: md5+g            ( x y z  -- r = basic MD5 G function )
  invert and or + +
;


: md5+h            ( x y z -- r = basic MD5 H function )
  xor xor + +
;


: md5+i            ( x y z  -- r = basic MD5 I function )
  invert or xor + +
;


0 value md5.a        ( -- a = MD5 variable local a )
0 value md5.b        ( -- b = MD5 variable local b )
0 value md5.c        ( -- c = MD5 variable local c )
0 value md5.d        ( -- d = MD5 variable local d )
0 value md5.buf      ( -- addr = Current MD5 buffer )


create md5.length    ( -- addr = MD5 length in bits )
  #bytes/long 2* allot

create md5.pad       ( -- addr = MD5 padding )
  md5.long-size #bytes/long * allot   
  md5.pad md5.long-size #bytes/long * erase   
  128 md5.pad c!  


bigendian? [IF]
: md5+buf@+        ( u n -- u+buf[n] = Fetch and add with MD5 buffer )
  #bytes/long * md5.buf + 
  dup c@
  swap char+ swap over c@ 8  lshift or
  swap char+ swap over c@ 16 lshift or
  swap char+           c@ 24 lshift or
  +
;
[ELSE]
: md5+buf@+        ( u n -- u+buf[n] = Fetch and add with MD5 buffer )
  #bytes/long * md5.buf + l@ +
;
[THEN]


bigendian? [IF]
hex
: md5!             ( x addr -- = Store word on address, MD5 order )
  over                 FF and over c!
  char+ over 8  rshift FF and over c!
  char+ over 10 rshift FF and over c!
  char+ swap 18 rshift FF and swap c!
;
decimal
[ELSE]
: md5!             ( x addr -- = Store word on address, MD5 order )
  postpone l!
; immediate
[THEN]


hex
: md5+round1       
  D76AA478  0 md5+buf@+ md5.a  md5.b md5.c AND md5.d md5.b md5+f u>l md5.s11 llroll  md5.b + TO md5.a   
  E8C7B756  1 md5+buf@+ md5.d  md5.a md5.b AND md5.c md5.a md5+f u>l md5.s12 llroll  md5.a + TO md5.d
  242070DB  2 md5+buf@+ md5.c  md5.d md5.a AND md5.b md5.d md5+f u>l md5.s13 llroll  md5.d + TO md5.c
  C1BDCEEE  3 md5+buf@+ md5.b  md5.c md5.d AND md5.a md5.c md5+f u>l md5.s14 llroll  md5.c + TO md5.b
  F57C0FAF  4 md5+buf@+ md5.a  md5.b md5.c AND md5.d md5.b md5+f u>l md5.s11 llroll  md5.b + TO md5.a 
  4787C62A  5 md5+buf@+ md5.d  md5.a md5.b AND md5.c md5.a md5+f u>l md5.s12 llroll  md5.a + TO md5.d
  A8304613  6 md5+buf@+ md5.c  md5.d md5.a AND md5.b md5.d md5+f u>l md5.s13 llroll  md5.d + TO md5.c
  FD469501  7 md5+buf@+ md5.b  md5.c md5.d AND md5.a md5.c md5+f u>l md5.s14 llroll  md5.c + TO md5.b
  698098D8  8 md5+buf@+ md5.a  md5.b md5.c AND md5.d md5.b md5+f u>l md5.s11 llroll  md5.b + TO md5.a 
  8B44F7AF  9 md5+buf@+ md5.d  md5.a md5.b AND md5.c md5.a md5+f u>l md5.s12 llroll  md5.a + TO md5.d 
  FFFF5BB1  A md5+buf@+ md5.c  md5.d md5.a AND md5.b md5.d md5+f u>l md5.s13 llroll  md5.d + TO md5.c  
  895CD7BE  B md5+buf@+ md5.b  md5.c md5.d AND md5.a md5.c md5+f u>l md5.s14 llroll  md5.c + TO md5.b
  6B901122  C md5+buf@+ md5.a  md5.b md5.c AND md5.d md5.b md5+f u>l md5.s11 llroll  md5.b + TO md5.a 
  FD987193  D md5+buf@+ md5.d  md5.a md5.b AND md5.c md5.a md5+f u>l md5.s12 llroll  md5.a + TO md5.d
  A679438E  E md5+buf@+ md5.c  md5.d md5.a AND md5.b md5.d md5+f u>l md5.s13 llroll  md5.d + TO md5.c
  49B40821  F md5+buf@+ md5.b  md5.c md5.d AND md5.a md5.c md5+f u>l md5.s14 llroll  md5.c + TO md5.b 
;


: md5+round2
  F61E2562  1 md5+buf@+ md5.a  md5.b md5.d AND md5.c md5.d md5+g u>l md5.s21 llroll  md5.b + TO md5.a
  C040B340  6 md5+buf@+ md5.d  md5.a md5.c AND md5.b md5.c md5+g u>l md5.s22 llroll  md5.a + TO md5.d
  265E5A51  B md5+buf@+ md5.c  md5.d md5.b AND md5.a md5.b md5+g u>l md5.s23 llroll  md5.d + TO md5.c
  E9B6C7AA  0 md5+buf@+ md5.b  md5.c md5.a AND md5.d md5.a md5+g u>l md5.s24 llroll  md5.c + TO md5.b
  D62F105D  5 md5+buf@+ md5.a  md5.b md5.d AND md5.c md5.d md5+g u>l md5.s21 llroll  md5.b + TO md5.a
  02441453  A md5+buf@+ md5.d  md5.a md5.c AND md5.b md5.c md5+g u>l md5.s22 llroll  md5.a + TO md5.d
  D8A1E681  F md5+buf@+ md5.c  md5.d md5.b AND md5.a md5.b md5+g u>l md5.s23 llroll  md5.d + TO md5.c
  E7D3FBC8  4 md5+buf@+ md5.b  md5.c md5.a AND md5.d md5.a md5+g u>l md5.s24 llroll  md5.c + TO md5.b
  21E1CDE6  9 md5+buf@+ md5.a  md5.b md5.d AND md5.c md5.d md5+g u>l md5.s21 llroll  md5.b + TO md5.a
  C33707D6  E md5+buf@+ md5.d  md5.a md5.c AND md5.b md5.c md5+g u>l md5.s22 llroll  md5.a + TO md5.d
  F4D50D87  3 md5+buf@+ md5.c  md5.d md5.b AND md5.a md5.b md5+g u>l md5.s23 llroll  md5.d + TO md5.c
  455A14ED  8 md5+buf@+ md5.b  md5.c md5.a AND md5.d md5.a md5+g u>l md5.s24 llroll  md5.c + TO md5.b
  A9E3E905  D md5+buf@+ md5.a  md5.b md5.d AND md5.c md5.d md5+g u>l md5.s21 llroll  md5.b + TO md5.a
  FCEFA3F8  2 md5+buf@+ md5.d  md5.a md5.c AND md5.b md5.c md5+g u>l md5.s22 llroll  md5.a + TO md5.d
  676F02D9  7 md5+buf@+ md5.c  md5.d md5.b AND md5.a md5.b md5+g u>l md5.s23 llroll  md5.d + TO md5.c
  8D2A4C8A  C md5+buf@+ md5.b  md5.c md5.a AND md5.d md5.a md5+g u>l md5.s24 llroll  md5.c + TO md5.b 
;


: md5+round3
  FFFA3942  5 md5+buf@+ md5.a  md5.b md5.c md5.d md5+h u>l md5.s31 llroll  md5.b + TO md5.a
  8771F681  8 md5+buf@+ md5.d  md5.a md5.b md5.c md5+h u>l md5.s32 llroll  md5.a + TO md5.d
  6D9D6122  B md5+buf@+ md5.c  md5.d md5.a md5.b md5+h u>l md5.s33 llroll  md5.d + TO md5.c
  FDE5380C  E md5+buf@+ md5.b  md5.c md5.d md5.a md5+h u>l md5.s34 llroll  md5.c + TO md5.b
  A4BEEA44  1 md5+buf@+ md5.a  md5.b md5.c md5.d md5+h u>l md5.s31 llroll  md5.b + TO md5.a
  4BDECFA9  4 md5+buf@+ md5.d  md5.a md5.b md5.c md5+h u>l md5.s32 llroll  md5.a + TO md5.d
  F6BB4B60  7 md5+buf@+ md5.c  md5.d md5.a md5.b md5+h u>l md5.s33 llroll  md5.d + TO md5.c
  BEBFBC70  A md5+buf@+ md5.b  md5.c md5.d md5.a md5+h u>l md5.s34 llroll  md5.c + TO md5.b
  289B7EC6  D md5+buf@+ md5.a  md5.b md5.c md5.d md5+h u>l md5.s31 llroll  md5.b + TO md5.a
  EAA127FA  0 md5+buf@+ md5.d  md5.a md5.b md5.c md5+h u>l md5.s32 llroll  md5.a + TO md5.d
  D4EF3085  3 md5+buf@+ md5.c  md5.d md5.a md5.b md5+h u>l md5.s33 llroll  md5.d + TO md5.c
  04881D05  6 md5+buf@+ md5.b  md5.c md5.d md5.a md5+h u>l md5.s34 llroll  md5.c + TO md5.b
  D9D4D039  9 md5+buf@+ md5.a  md5.b md5.c md5.d md5+h u>l md5.s31 llroll  md5.b + TO md5.a
  E6DB99E5  C md5+buf@+ md5.d  md5.a md5.b md5.c md5+h u>l md5.s32 llroll  md5.a + TO md5.d
  1FA27CF8  F md5+buf@+ md5.c  md5.d md5.a md5.b md5+h u>l md5.s33 llroll  md5.d + TO md5.c
  C4AC5665  2 md5+buf@+ md5.b  md5.c md5.d md5.a md5+h u>l md5.s34 llroll  md5.c + TO md5.b 
;


: md5+round4
  F4292244  0 md5+buf@+ md5.a  md5.c md5.b md5.d md5+i u>l md5.s41 llroll  md5.b + TO md5.a
  432AFF97  7 md5+buf@+ md5.d  md5.b md5.a md5.c md5+i u>l md5.s42 llroll  md5.a + TO md5.d
  AB9423A7  E md5+buf@+ md5.c  md5.a md5.d md5.b md5+i u>l md5.s43 llroll  md5.d + TO md5.c
  FC93A039  5 md5+buf@+ md5.b  md5.d md5.c md5.a md5+i u>l md5.s44 llroll  md5.c + TO md5.b
  655B59C3  C md5+buf@+ md5.a  md5.c md5.b md5.d md5+i u>l md5.s41 llroll  md5.b + TO md5.a
  8F0CCC92  3 md5+buf@+ md5.d  md5.b md5.a md5.c md5+i u>l md5.s42 llroll  md5.a + TO md5.d
  FFEFF47D  A md5+buf@+ md5.c  md5.a md5.d md5.b md5+i u>l md5.s43 llroll  md5.d + TO md5.c
  85845DD1  1 md5+buf@+ md5.b  md5.d md5.c md5.a md5+i u>l md5.s44 llroll  md5.c + TO md5.b
  6FA87E4F  8 md5+buf@+ md5.a  md5.c md5.b md5.d md5+i u>l md5.s41 llroll  md5.b + TO md5.a
  FE2CE6E0  F md5+buf@+ md5.d  md5.b md5.a md5.c md5+i u>l md5.s42 llroll  md5.a + TO md5.d
  A3014314  6 md5+buf@+ md5.c  md5.a md5.d md5.b md5+i u>l md5.s43 llroll  md5.d + TO md5.c
  4E0811A1  D md5+buf@+ md5.b  md5.d md5.c md5.a md5+i u>l md5.s44 llroll  md5.c + TO md5.b
  F7537E82  4 md5+buf@+ md5.a  md5.c md5.b md5.d md5+i u>l md5.s41 llroll  md5.b + TO md5.a
  BD3AF235  B md5+buf@+ md5.d  md5.b md5.a md5.c md5+i u>l md5.s42 llroll  md5.a + TO md5.d
  2AD7D2BB  2 md5+buf@+ md5.c  md5.a md5.d md5.b md5+i u>l md5.s43 llroll  md5.d + TO md5.c
  EB86D391  9 md5+buf@+ md5.b  md5.d md5.c md5.a md5+i u>l md5.s44 llroll  md5.c + TO md5.b 
;
decimal


: md5-transform    ( md5 -- = MD5 Basic transformation )
  >r
  
  r@ md5>a l@ to md5.a     \ copy to values for easy access
  r@ md5>b l@ to md5.b
  r@ md5>c l@ to md5.c 
  r@ md5>d l@ to md5.d
  
  md5+round1
  md5+round2
  md5+round3
  md5+round4
  
  md5.d r@ md5>d l+!
  md5.c r@ md5>c l+!
  md5.b r@ md5>b l+!
  md5.a r> md5>a l+!
;


: md5+cmove        ( c-addr u n1 -- n2 flag = Move data from source to buffer, return number processed and full indication )
  2dup + md5.byte-size min md5.byte-size = >r   \ full buffer ?
  tuck md5.byte-size swap - min >r              \ number of chars taken from source
  chars md5.buf + r@ chars cmove                \ move source in the buffer
  r> r>
;


cell 4 = [IF]
: md5-length!      ( md5 -- = Store the bit length )
  md5>length @ #bits/byte m* swap                \ Bit length: msl lsl
  md5.length tuck md5! #bytes/long + md5!
;
[ELSE]
: md5-length!      ( md5 -- = Store the bit length )
  md5>length @ #bits/byte *                       \ Bit length
  dup [ #bytes/long #bits/byte * ] literal rshift swap u>l   \ Split in msl lsl
  md5.length tuck md5! #bytes/long + md5!
;
[THEN]


hex
: md5+#s       ( u -- = Convert one MD5 number in hold area )
  dup 18 rshift FF and 0 # # 2drop
  dup 10 rshift FF and 0 # # 2drop
  dup  8 rshift FF and 0 # # 2drop
                FF and 0 # # 2drop
;
decimal


( MD5 variable creation, initialisation and destruction )

: md5-init     ( md5 -- = Initialise the MD5 variable )
  >r
  [ hex ]
  67452301 r@ md5>a l!
  EFCDAB89 r@ md5>b l! 
  98BADCFE r@ md5>c l!
  10325476 r@ md5>d l!
  [ decimal ]
  
  r> md5>length 0!
;


: md5-create   ( "<spaces>name" -- ; -- md5 = Create a named MD5 variable in the dictionary )
  create   here   md5% allot   md5-init
;


: md5-new      ( -- md5 = Create a new MD5 variable on the heap )
  md5% allocate  throw  dup md5-init
;


: md5-free     ( md5 -- = Free the MD5 variable from the heap )
  free throw 
;


( MD5 words )

: md5-reset        ( md5 -- = Reset the MD5 state )
  md5-init
;


: md5-update       ( c-addr u md5 -- = Update the MD5 with more data c-addr u )
  >r
  r@ md5>buffer to md5.buf
  
  BEGIN
    2dup r@ md5>length @ md5.byte-size mod md5+cmove
    over r@ md5>length +!
  WHILE
    r@ md5-transform
    /string
  REPEAT
  rdrop
  drop 2drop
;


: md5-finish       ( md5 -- u1 u2 u3 u4 = Finish the MD5 calculation, return the result u1 u2 u3 u4 )
  >r
  r@ md5-length!
  
  r@ md5>length @ md5.byte-size mod 
  [ md5.byte-size #bytes/long 2* - ] literal \ reserve two longs for the length
  
  2dup < IF                        \ pad length
    swap -
  ELSE
    md5.byte-size + swap -
  THEN
  
  md5.pad swap r@ md5-update       \ pad the buffer
  
  md5.length #bytes/long 2* r@ md5-update \ add the bit length to the algorithm
  
  r@ md5>a l@                       \ Return the result
  r@ md5>b l@
  r@ md5>c l@
  r> md5>d l@
;


: md5+to-string    ( u1 u2 u3 u4 -- c-addr u = Convert MD5 result to the string, using the pictured output area )
  base @ >r hex
  <# md5+#s md5+#s md5+#s md5+#s 0. #>
  r> base !
;


( Inspection )

: md5-dump         ( md5 -- = Dump the md5 variable )
  >r
  ." md5:" r@ . cr
  ."  result :" r@ md5>a l@ r@ md5>b l@ r@ md5>c l@ r@ md5>d l@ md5+to-string type cr
  ."  length :" r@ md5>length ? cr
  ."  buffer :" r@ md5>buffer r> md5>length @ 64 min dump
;

[ELSE]
.( Warning: md5 requires at least 4 byte cells and 1 byte chars ) cr
[THEN]

[THEN]

\ ==============================================================================
