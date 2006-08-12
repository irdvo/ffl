\ ==============================================================================
\
\                 bar - the bit array module in the ffl
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
\  $Date: 2006-08-09 17:03:07 $ $Revision: 1.6 $
\
\ ==============================================================================

\ Layout: 
\   Index: +7  +6 +5 +4 +3 +2 +1 +0 +15 +14 +13 +12 +11 +10 +9 +8 +23 +22 +21 +20 +19 +18 +17 +16 
\   Byte:  0                        1                             2
\   Mask:  128 64 32 16 8  4  2  1  128 64  32  16  8   4   2  1  128 64  32  16  8   4   2   1 
  
include ffl/config.fs


[UNDEFINED] bar.version [IF]


include ffl/stc.fs


( bar = Bit array module )
( The bar module implements a bit array. )


1 constant bar.version


( Public structure )

struct: bar%       ( - n = Get the required space for the bar data structure )
  cell:  bar>length
  cell:  bar>size            \ the size of bits
  cell:  bar>bits
;struct



( Private database )


( Public words )

: bar-init         ( n:length w:bar - = Initialise the bit array )
  >r
  1 max                      \ at least one bit in the array
  
  dup sys.bits-in-char
  /mod swap 0<> IF
    1+
  THEN
  
  dup r@ bar>size !
  
  dup chars allocate throw         \ allocate the array
  
  tuck swap 0 fill           \ reset all bits
  
  r@ bar>bits   !
  r> bar>length !
;


: bar-create       ( C: n:length "name" - R: - w:bar = Create a bit array in the dictionary )
  create  here  bar% allot  bar-init
;


: bar-new          ( n:length - w:bar = Create a bit array on the heap )
  bar% allocate throw  dup >r bar-init r> 
;


: bar-free         ( w:bar - = Free the bit array from the heap )
  dup bar>bits @ free throw
  free throw
;



( Private words )

: bar-offset?      ( n w:bar - f = Check if the offset is valid in the bit array )
  0 swap bar>length @ within
;



( Member words )

: bar-length@      ( w:bar - u = Get the number of bits in the bit array )
  bar>length @
;


: bar-index?       ( n w:bar - f = Check if an index is valid in the bit array )
  tuck bar-length@ index2offset 
  swap bar-offset?
;



( Private words )

: bar-address      ( n w:bar - u:mask w:addr = Determine address and bit mask for an index )
  tuck bar-length@ index2offset
  
  2dup swap bar-offset?
  
  0= exp-index-out-of-range AND throw
  
  sys.bits-in-byte /mod      \ mask and offset
  
  swap 1 swap lshift         \ Convert remainder to bit mask
  
  -rot chars swap bar>bits @ +
;


: bar-next-bit     ( u:mask w:addr - u:mask w:addr = Move mask and address to the next bit )
  swap dup 
  [ 1 sys.bits-in-char 1- lshift ] literal = IF   \ 128
    drop 1
    swap char+
  ELSE
    1 lshift swap
  THEN
;


: bar-end-address?   ( u:mask w:addr u:mask w:addr - f = Has the first address reached the second, end address ? )
  rot
  2dup < IF                  \ first byte address is smaller than the second address
    2drop 2drop false
  ELSE
    = IF                     \ first byte address is equal to the second address -> check the mask
      u<=
    ELSE
      2drop true
    THEN
  THEN
;


: bar-emit-bit     ( f - = Emit the state of the flag )
  1 AND [char] 0 + emit
;



( Bit set words )

: bar-set-bit      ( n w:bar - = Set the indexth bit )
  bar-address
  tuck c@ OR
  swap c!
;


: bar-set-bits     ( u:number n:start w:bar - = Set a range of bits )
  >r
  over 0<> IF
    tuck + 1- r@ bar-address      \ Determine end and start address and mask
    rot r@ bar-address
    
    BEGIN                         \ Loop throught the addresses
      2dup 
      tuck c@ OR
      swap c!
      
      2over 2over bar-end-address? 0=
    WHILE
      bar-next-bit
    REPEAT
    2drop 2drop
  ELSE
    2drop
  THEN
  rdrop
;


: bar-set          ( w:bar - = Set all bits in the bit array )
  dup bar>bits @ swap bar>size @ -1 fill
;



( Bit reset words )

: bar-reset-bit    ( n w:bar - = Reset the indexth bit )
  bar-address
  swap invert over c@ AND
  swap c!
;


: bar-reset-bits   ( u:number n:start w:bar - = Reset a range of bits )
  >r
  over 0<> IF
    tuck + 1- r@ bar-address      \ Determine end and start address and mask
    rot r@ bar-address
    
    BEGIN                         \ Loop throught the addresses
      2dup 
      swap invert over c@ AND
      swap c!
      
      2over 2over bar-end-address? 0=
    WHILE
      bar-next-bit
    REPEAT
    2drop 2drop
  ELSE
    2drop
  THEN
  rdrop
;


: bar-reset        ( w:bar - = Reset all bits in the bit array )
  dup bar>bits @ swap bar>size @ 0 fill
;



( Bit invert words )

: bar-invert-bit   ( n w:bar - = Invert the indexth bit )
  bar-address
  tuck c@ XOR
  swap c!
;


: bar-invert-bits  ( u:number n:start w:bar - = Invert a range of bits )
  >r
  over 0<> IF
    tuck + 1- r@ bar-address      \ Determine end and start address and mask
    rot r@ bar-address
    
    BEGIN                         \ Loop throught the addresses
      2dup 
      tuck c@ XOR
      swap c!
      
      2over 2over bar-end-address? 0=
    WHILE
      bar-next-bit
    REPEAT
    2drop 2drop
  ELSE
    2drop
  THEN
  rdrop
;


: bar-invert       ( w:bar - = Invert all bits in the bit array )
  dup bar-length@
  swap 0
  swap bar-invert-bits
;



( Bit check words )

: bar-get-bit      ( n w:bar - f = Check if the indexth bit is set )
  bar-address
  c@ AND 0<>
;


: bar-count-bits   ( n:number n:start w:bar - u = Count the number of bits set in a range )
  0 >r                       \ count = 0
  -rot over 0<> IF           \ number > 0
    tuck + 1-                \ end index
    rot tuck bar-address     \ end address
    2swap bar-address        \ start address
    BEGIN
      2dup c@ AND IF
        r> 1+ >r             \ increase counter if bit set
      THEN
      
      2over 2over bar-end-address? 0=
    WHILE
      bar-next-bit
    REPEAT
    2drop 2drop
  ELSE
    2drop drop
  THEN
  
  r>
;


: bar-count        ( w:bar - u = Count the number of bits set in the bit array )
  dup bar-length@
  swap 0
  swap bar-count-bits
;


: bar-execute      ( ... xt w:bar - ... = Execute xt for every bit in the bit array )
  -1 over bar-address               \ end address
  rot 0 swap bar-address            \ start address
  
  BEGIN
    2swap
    2>r 2>r                         \ clear the stack for execute
    
    2r@ c@ AND 0<>                  \ fetch the bit
    
    swap dup >r execute r>          \ execute the token with the flag
    
    2r> 2r>                         \ restore the stack
    2swap
    
    2over 2over bar-end-address? 0=
  WHILE
    bar-next-bit
  REPEAT
  drop 2drop 2drop
;


( Inspection )

: bar-dump         ( w:bar - = Dump the bit array )
  ." bar:" dup . cr
  ."  length:" dup bar>length ? cr
  ."  size  :" dup bar>size   ? cr
  ."  bits  :" ['] bar-emit-bit swap bar-execute cr
;

[THEN]

\ ==============================================================================
