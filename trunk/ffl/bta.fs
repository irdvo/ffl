\ ==============================================================================
\
\                 bta - the bitarry module in the ffl
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
\  $Date: 2006-03-25 07:40:02 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] bta.version [IF]


( bta = Bit array module )
( The bta module implements a bit array. )


1 constant bta.version


( Public structure )

struct: bta%       ( - n = Get the required space for the bta data structure )
  cell:  bta>min
  cell:  bta>max
  cell:  bta>bits            \ start of bits..
;struct



( Private database )


( Private words )


( Public words )

: bta-init         ( u:min u:max w:bta - = Initialise the bit array )
  \ Save min and max
  \ reset
;


: bta-create       ( C: u:min u:max "name" - R: - w:bta = Create a bit array [min..max] inclusive in the dictionary )
  \ Check min,max
  \ create   here   bta% allot   bta-init
;


: bta-new          ( u:min u:max - w:bta = Create a bit array [min..max] inclusive on the heap )
  \ Check min,max
  \ bta% allocate  throw  dup bta-init
;


: bta-free         ( w:bta - = Free the bit array from the heap )
  free
;



( Member words )

: bta-min@         ( w:bta - u = Get the minimum index of the bit array )
  bta>min @
;


: bta-max@         ( w:bta - u = Get the maximum index of the bit array )
  bta>max @
;


: bta-length@      ( w:bta - u = Get the number of bits in the bit array )
  bta-max@ bta-min@ - 1+
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
  \ Lus array -> FF  
;


: bta-set-bit      ( n w:bta - = Set the indexth bit )
  \ check index
  \ index->array & bit
  \ Or bit
;


: bta-set-bits     ( n n w:bta - = Set a range of bits )
  \ check range
  \ Lus range -> bta-set-bit
;



( Bit reset words )

: bta-reset-all    ( w:bta - = Reset all bits in the bit array )
  \ Lus array -> 0
;


: bta-reset-bit    ( n w:bta - = Reset the indexth bit )
  \ check index
  \ index->array & bit
  \ And  -bit
;


: bta-reset-bits   ( n n w:bta - = Reset a range of bits )
  \ Lus range -> bta-set-bit
;



( Bit check words )

: bta-has-bit      ( n w:bta - f = Check if the indexth bit is set )
  \ index-> array & bit (with check)
  \ And bit -> result
;


: bta-count-bits   ( n n w:bta - u = Count the number of bits set in the range )
  \ Lus range -> bta-has-bit -> count
;

: bta-count-all   ( w:bta - u = Count the number of bits set )
  \ Lus array -> bta-has-bit -> count
;



( Inspection )

: bta-dump         ( w:bta - = Dump the text output stream )
  ." bta:" dup . cr
  ."  min :" dup bta>min . cr
  ."  max :" dup bta>max . cr
  ."  bits:" drop \ ToDo - show all bits 1101110001001
;

[THEN]

\ ==============================================================================
