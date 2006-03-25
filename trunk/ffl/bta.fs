\ ==============================================================================
\
\                 bta - the bit array module in the ffl
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
\  $Date: 2006-03-25 21:12:48 $ $Revision: 1.2 $
\
\ ==============================================================================

\ Layout: 
\   Index: +7  +6 +5 +4 +3 +2 +1 +0 +15 +14 +13 +12 +11 +10 +9 +8 +23 +22 +21 +20 +19 +18 +17 +16 
\   Byte:  0                        1                             2
\   Mask:  128 64 32 16 8  4  2  1  128 64  32  16  8   4   2  1  128 64  32  16  8   4   2   1 
  
include ffl/config.fs


[UNDEFINED] bta.version [IF]


include ffl/str.fs


( bta = Bit array module )
( The bta module implements a bit array. )


1 constant bta.version


( Public structure )

struct: bta%       ( - n = Get the required space for the bta data structure )
  cell:  bta>start
  cell:  bta>length
  cell:  bta>size            \ the size of bits
  cell:  bta>bits
;struct



( Private database )


( Public words )

: bta-init         ( u:number n:start w:bta - = Initialise the bit array )
  >r
  swap dup 0= IF             \ at least one bit in the array
    1+
  THEN
  
  dup 8 /mod swap 0<> IF     \ 8 bits in a byte
    1+
  THEN
  
  dup r@ bta>size !
  
  dup allocate throw         \ allocate the array
  
  tuck swap 0 fill           \ erase the array
  
  r@ bta>bits   !
  r@ bta>length !
  r> bta>start  !
;


: bta-create       ( C: u:number n:start "name" - R: - w:bta = Create a bit array in the dictionary )
  create  here  bta% allot  bta-init
;


: bta-new          ( u:max u:min - w:bta = Create a bit array [min..max] inclusive on the heap )
  bta% allocate throw  dup >r bta-init r> 
;


: bta-free         ( w:bta - = Free the bit array from the heap )
  dup bta>bits free throw
  free throw
;



( Member words )

: bta-start@       ( w:bta - u = Get the start index of the bit array )
  bta>start @
;


: bta-length@      ( w:bta - u = Get the number of bits in the bit array )
  bta>length @
;


: bta-end@         ( w:bta - u = Get the end index of the bit array )
  dup bta-start@ swap bta-length@ + 1-
;


: bta-index?       ( n w:bta - f == Check if the index is valid )
  dup bta-start@ swap bta-length@ over + within
;


( Private words )

: bta-address      ( n w:bta - u:mask w:addr = Determine address and bit mask for index )
  2dup bta-index?   
  
  0= exp-index-out-of-range AND throw
    
  tuck bta-start@ - 8 /mod
  
  swap 1 swap lshift         \ Convert remainder to bit mask
  
  -rot swap bta>bits @ +
;


: bta-next-bit     ( u:mask w:addr - u:mask w:addr = Move mask and address to the next bit )
  swap dup 128 = IF
    drop 1
    swap 1+
  ELSE
    1 lshift swap
  THEN
;


: bta-equal-address?   ( u:mask w:addr u:mask w:addr - f = Are the first address equal to the second address ? )
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

: bta^and          ( w:bta w:bta - = And two bit arrays )
  \ Bitwise or byte wise -> bitwise ??
;


: bta^or           ( w:bta w:bta - = Or two bit arrays )
;


: bta^xor          ( w:bta w:bta - = Xor two bit arrays )
;


: bta-not          ( w:bta - = Invert all bits in the bit array )
;



( Bit set words )

: bta-set-all      ( w:bta - = Set all bits in the bit array )
  dup bta>bits @ swap bta>size @ 255 fill
;


: bta-set-bit      ( n w:bta - = Set the indexth bit )
  bta-address
  tuck c@ OR
  swap c!
;


: bta-set-bits     ( u:number n:start w:bta - = Set a range of bits )
  >r
  over 0<> IF
    tuck + 1- r@ bta-address      \ Determine end and start address and mask
    rot r@ bta-address
    
    BEGIN                         \ Loop throught the addresses
      2dup 
      tuck c@ OR
      swap c!
      
      2over 2over bta-equal-address? 0=
    WHILE
      bta-next-bit
    REPEAT
    2drop 2drop
    
  THEN
  r>
;



( Bit reset words )

: bta-reset-all    ( w:bta - = Reset all bits in the bit array )
  dup bta>bits @ swap bta>size @ 0 fill
;


: bta-reset-bit    ( n w:bta - = Reset the indexth bit )
  bta-address
  swap invert over c@ AND
  swap c!
;


: bta-reset-bits   ( u:number n:start w:bta - = Reset a range of bits )
  >r
  over 0<> IF
    tuck + 1- r@ bta-address      \ Determine end and start address and mask
    rot r@ bta-address
    
    BEGIN                         \ Loop throught the addresses
      2dup 
      swap invert over c@ AND
      swap c!
      
      2over 2over bta-equal-address? 0=
    WHILE
      bta-next-bit
    REPEAT
    2drop 2drop
    
  THEN
  r>
;



( Bit check words )

: bta-has-bit      ( n w:bta - f = Check if the indexth bit is set )
  bta-address
  c@ AND 0<>
;


: bta-count-bits   ( n:number n:start w:bta - u = Count the number of bits set in the range )
;

: bta-count-all   ( w:bta - u = Count the number of bits set )
  0 >r
  >r r@ bta-end@   r@ bta-address
     r@ bta-start@ r> bta-address
  BEGIN
    2dup c@ AND IF
      r> 1+ >r
    THEN
    
    2over 2over bta-equal-address? 0=
  WHILE
    bta-next-bit
  REPEAT
  2drop 2drop
  
  r>
;



( Inspection )

: bta-dump         ( w:bta - = Dump the text output stream )
  ." bta:" dup . cr
  ."  start :" dup bta>start  ? cr
  ."  length:" dup bta>length ? cr
  ."  size  :" dup bta>size   ? cr
  ."  bits  :" 
  >r r@ bta-end@   r@ bta-address
     r@ bta-start@ r> bta-address
  BEGIN
    2dup c@ AND IF
      [char] 1
    ELSE
      [char] 0
    THEN
    emit
    
    2over 2over bta-equal-address? 0=
  WHILE
    bta-next-bit
  REPEAT
  2drop 2drop
  cr
;

[THEN]

\ ==============================================================================
