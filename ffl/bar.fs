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
\  $Date: 2006-03-26 17:33:37 $ $Revision: 1.1 $
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
  cell:  bar>start
  cell:  bar>length
  cell:  bar>size            \ the size of bits
  cell:  bar>bits
;struct



( Private database )


( Public words )

: bar-init         ( u:number n:start w:bar - = Initialise the bit array )
  >r
  swap dup 0= IF             \ at least one bit in the array
    1+
  THEN
  
  dup 8 /mod swap 0<> IF     \ 8 bits in a byte
    1+
  THEN
  
  dup r@ bar>size !
  
  dup allocate throw         \ allocate the array
  
  tuck swap 0 fill           \ erase the array
  
  r@ bar>bits   !
  r@ bar>length !
  r> bar>start  !
;


: bar-create       ( C: u:number n:start "name" - R: - w:bar = Create a bit array in the dictionary )
  create  here  bar% allot  bar-init
;


: bar-new          ( u:max u:min - w:bar = Create a bit array [min..max] inclusive on the heap )
  bar% allocate throw  dup >r bar-init r> 
;


: bar-free         ( w:bar - = Free the bit array from the heap )
  dup bar>bits free throw
  free throw
;



( Member words )

: bar-start@       ( w:bar - u = Get the start index of the bit array )
  bar>start @
;


: bar-length@      ( w:bar - u = Get the number of bits in the bit array )
  bar>length @
;


: bar-end@         ( w:bar - u = Get the end index of the bit array )
  dup bar-start@ swap bar-length@ + 1-
;


: bar-index?       ( n w:bar - f == Check if the index is valid )
  dup bar-start@ swap bar-length@ over + within
;


( Private words )

: bar-address      ( n w:bar - u:mask w:addr = Determine address and bit mask for index )
  2dup bar-index?   
  
  0= exp-index-out-of-range AND throw
    
  tuck bar-start@ - 8 /mod
  
  swap 1 swap lshift         \ Convert remainder to bit mask
  
  -rot swap bar>bits @ +
;


: bar-next-bit     ( u:mask w:addr - u:mask w:addr = Move mask and address to the next bit )
  swap dup 128 = IF
    drop 1
    swap 1+
  ELSE
    1 lshift swap
  THEN
;


: bar-equal-address?   ( u:mask w:addr u:mask w:addr - f = Are the first address equal to the second address ? )
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

    
( Arithmetic words )

: bar^and          ( w:bar w:bar - = And two bit arrays )
  \ Bitwise or byte wise -> bitwise ??
;


: bar^or           ( w:bar w:bar - = Or two bit arrays )
;


: bar^xor          ( w:bar w:bar - = Xor two bit arrays )
;


: bar-not          ( w:bar - = Invert all bits in the bit array )
;



( Bit set words )

: bar-set-all      ( w:bar - = Set all bits in the bit array )
  dup bar>bits @ swap bar>size @ 255 fill
;


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
      
      2over 2over bar-equal-address? 0=
    WHILE
      bar-next-bit
    REPEAT
    2drop 2drop
    
  THEN
  r>
;



( Bit reset words )

: bar-reset-all    ( w:bar - = Reset all bits in the bit array )
  dup bar>bits @ swap bar>size @ 0 fill
;


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
      
      2over 2over bar-equal-address? 0=
    WHILE
      bar-next-bit
    REPEAT
    2drop 2drop
    
  THEN
  r>
;



( Bit check words )

: bar-has-bit      ( n w:bar - f = Check if the indexth bit is set )
  bar-address
  c@ AND 0<>
;


: bar-count-bits   ( n:number n:start w:bar - u = Count the number of bits set in the range )
;

: bar-count-all   ( w:bar - u = Count the number of bits set )
  0 >r
  >r r@ bar-end@   r@ bar-address
     r@ bar-start@ r> bar-address
  BEGIN
    2dup c@ AND IF
      r> 1+ >r
    THEN
    
    2over 2over bar-equal-address? 0=
  WHILE
    bar-next-bit
  REPEAT
  2drop 2drop
  
  r>
;



( Inspection )

: bar-dump         ( w:bar - = Dump the text output stream )
  ." bar:" dup . cr
  ."  start :" dup bar>start  ? cr
  ."  length:" dup bar>length ? cr
  ."  size  :" dup bar>size   ? cr
  ."  bits  :" 
  >r r@ bar-end@   r@ bar-address
     r@ bar-start@ r> bar-address
  BEGIN
    2dup c@ AND IF
      [char] 1
    ELSE
      [char] 0
    THEN
    emit
    
    2over 2over bar-equal-address? 0=
  WHILE
    bar-next-bit
  REPEAT
  2drop 2drop
  cr
;

[THEN]

\ ==============================================================================
