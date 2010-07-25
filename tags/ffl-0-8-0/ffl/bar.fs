\ ==============================================================================
\
\                 bar - the bit array module in the ffl
\
\              Copyright (C) 2006-2008  Dick van Oudheusden
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
\  $Date: 2008-10-04 14:56:56 $ $Revision: 1.12 $
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


3 constant bar.version


( Bit array structure )

begin-structure bar%       ( -- n = Get the required space for a bar variable )
  field: bar>length
  field: bar>size            \ the size of bits
  field: bar>bits
  field: bar>mask            \ the unused bits mask
end-structure


( Private database )
                             \ the mask for unused bits
create bar.mask  255 c, 127 c, 63 c, 31 c, 15 c, 7 c, 3 c, 1 c,


( Array creation, initialisation and destruction )

: bar-init         ( +n bar -- = Initialise the array with length n)
  >r
  1 max                              \ at least one bit in the array
  
  dup #bits/char 1- + #bits/char /   \ calculate the size of the array in bytes
  
  dup r@ bar>size !

  2dup #bits/char * swap -           \ calculate number of unused bits
  chars bar.mask + c@                \ get mask for unused bits
  r@ bar>mask !
  
  dup chars allocate throw           \ allocate the array

  tuck swap 0 fill                   \ reset all bits
  
  r@ bar>bits   !
  r> bar>length !
;


: bar-(free)       ( bar -- = Free the internal data from the heap )
  bar>bits @ free throw
;


: bar-create       ( +n "<spaces>name" -- ; -- bar = Create a bit array in the dictionary with length n )
  create  here  bar% allot  bar-init
;


: bar-new          ( n -- bar = Create a bit array on the heap with length n )
  bar% allocate throw  dup >r bar-init r> 
;


: bar-free         ( bar -- = Free the array from the heap )
  dup bar-(free)
  
  free throw
;


( Private words )

: bar-offset?      ( +n bar -- flag = Check if the offset n is valid in the array )
  0 swap bar>length @ within
;


( Member words )

: bar-length@      ( bar -- +n = Get the number of bits in the array )
  bar>length @
;


: bar-index?       ( n bar -- flag = Check if the index n is valid in the array )
  tuck bar-length@ index2offset 
  swap bar-offset?
;



( Private words )

: bar-address      ( n bar -- u addr = Determine address addr and bit mask u for index n )
  tuck bar-length@ index2offset
  
  2dup swap bar-offset?
  
  0= exp-index-out-of-range AND throw
  
  #bits/byte /mod            \ mask and offset
  
  swap 1 swap lshift         \ Convert remainder to bit mask
  
  -rot chars swap bar>bits @ +
;


: bar-next-bit     ( u1 addr1 -- u2 addr2 = Move mask and address to the next bit )
  swap dup 
  [ 1 #bits/char 1- lshift ] literal = IF   \ 128
    drop 1
    swap char+
  ELSE
    1 lshift swap
  THEN
;


: bar-end-address?   ( u1 addr1 u2 addr2 -- flag = Has the first address reached the second, end address ? )
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


: bar-emit-bit     ( flag -- = Emit the state of the flag )
  1 AND [char] 0 + emit
;


: bar-clear-unused  ( bar -- = Clear the unused bits in the bit array )
  dup  bar>size @ 1- chars   \ Move to the last byte in the array
  over bar>bits @ +
  swap bar>mask @
  over c@ AND                \ Fetch the byte, clear the unused bits ..
  swap 2drop \ c!                    \ .. and store
;


( Private bit array word )

: bar^execute      ( xt bar1 bar2 -- = Execute xt for byte in both sets: [ char1 char2 -- char3 ] and store the result in bar2 )
  swap >r
  tuck dup bar>bits @ swap bar>size @  \ S: bar2 xt bits2 size2
  r@ bar>size @ min                    \ Calculate size smallest array

  r@ bar>bits @ swap bounds ?DO        \ Loop over array
    2dup c@ I c@
    rot execute                        \   Execute xt with bytes from both arrays
    over c!
    char+
  1 chars +LOOP

  >r over bar>size @ r> 
  swap r> bar>size @ - 0 max           \ Calculate the extra size in bar2

  bounds ?DO                           \ Loop over the remaining of array2
    dup 0 I c@ 
    rot execute                        \   Execute xt with zero
    I c!
  1 chars +LOOP
  drop

  bar-clear-unused                     \ Clear the unused bits
;


( Bit array words )

: bar^move         ( bar1 bar2 -- = Move bar1 into bar2 )
  >r
  dup bar>size @  r@ bar>size @ <> IF
    r@  bar>bits @  free throw         \ Free the bit array
    dup bar>size @ dup
    r@  bar>size !                     \ Set the size from bar1
    chars allocate throw               \ Allocate the new bit array and ..
    r@  bar>bits !                     \ .. store it
  THEN
  dup bar-length@ r@ bar>length !
  bar>bits @  r@ bar>bits @  r> bar>size @ cmove  \ Copy the array from bar1 into bar2
;


: bar^or           ( bar1 bar2 -- = OR the bit arrays bar1 and bar2 and store the result in bar2 )
  ['] OR -rot bar^execute
;


: bar^and          ( bar1 bar2 -- = AND the bit arrays bar1 and bar2 and store the result in bar2 )
  ['] AND -rot bar^execute
;


: bar^xor          ( bar1 bar2 -- = XOR the bit arrays bar1 and bar2 and store the result in bar2 )
  ['] XOR -rot bar^execute
;


( Bit set words )

: bar-set-bit      ( n bar -- = Set the nth bit in the array )
  bar-address
  tuck c@ OR
  swap c!
;


: bar-set-bits     ( u n bar -- = Set a range of bits in the array, starting from the nth bit, u bits long )
  >r
  over 0<> IF
    tuck + 1- r@ bar-address      \ Determine end and start address and mask
    rot r@ bar-address
    
    BEGIN                         \ Loop through the addresses
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


: bar-set-list     ( nu .. n1 u bar -- = Set n1 till nuth bits in the array )
  swap
  0 ?DO
    tuck bar-set-bit
  LOOP
  drop
;


: bar-set          ( bar -- = Set all bits in the array )
  dup bar>bits @  over bar>size @  -1 fill  \ Set all the bits in the array

  bar-clear-unused                \ Clear unused bits
;


( Bit reset words )

: bar-reset-bit    ( n bar -- = Reset the nth bit )
  bar-address
  swap invert over c@ AND
  swap c!
;


: bar-reset-bits   ( u n bar -- = Reset a range of bits in the array, starting from the nth bit, u bits long )
  >r
  over 0<> IF
    tuck + 1- r@ bar-address      \ Determine end and start address and mask
    rot r@ bar-address
    
    BEGIN                         \ Loop through the addresses
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


: bar-reset-list   ( nu .. n1 u bar -- = Reset n1 till nuth bits in the array )
  swap
  0 ?DO
    tuck bar-reset-bit
  LOOP
  drop
;


: bar-reset        ( bar -- = Reset all bits in the array )
  dup bar>bits @ swap bar>size @ 0 fill
;



( Bit invert words )

: bar-invert-bit   ( n bar -- = Invert the nth bit )
  bar-address
  tuck c@ XOR
  swap c!
;


: bar-invert-bits  ( u n bar -- = Invert a range of bits in the array, starting from the nth bit, u bits long )
  >r
  over 0<> IF
    tuck + 1- r@ bar-address      \ Determine end and start address and mask
    rot r@ bar-address
    
    BEGIN                         \ Loop through the addresses
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


: bar-invert       ( bar -- = Invert all bits in the array )
  dup bar-length@
  swap 0
  swap bar-invert-bits
;



( Bit check words )

: bar-get-bit      ( n bar -- flag = Check if the nth bit is set )
  bar-address
  c@ AND 0<>
;


: bar-count-bits   ( +n1 n2 bar -- u = Count the number of bits set in a range in the array, starting from the n2th bit, n1 bits long )
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


: bar-count        ( bar -- u = Count the number of bits set in the array )
  dup bar-length@
  swap 0
  swap bar-count-bits
;


: bar-execute      ( i*x xt bar -- j*x = Execute xt for every bit in the array )
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

: bar-dump         ( bar -- = Dump the bit array )
  ." bar:" dup . cr
  ."  length:" dup bar>length ? cr
  ."  size  :" dup bar>size   ? cr
  ."  mask  :" dup bar>mask   ? cr
  ."  bits  :" ['] bar-emit-bit swap bar-execute cr
;

[THEN]

\ ==============================================================================
