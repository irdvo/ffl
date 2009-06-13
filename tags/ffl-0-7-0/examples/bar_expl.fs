\ ==============================================================================
\
\             bar_expl - the bit array example in the ffl
\
\               Copyright (C) 2008  Dick van Oudheusden
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
\  $Date: 2008-10-05 06:34:20 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/bar.fs


\ Create a bit array with 15 bits [0..14] in the dictionary

15 bar-create bar1


\ Set bit 1, 4..8, 12 and 14 in the array

1       bar1 bar-set-bit
5 4     bar1 bar-set-bits

12 14 2 bar1 bar-set-list   \ new in version 3

\ Count the number of bits set 

.( There are ) bar1 bar-count . .( bits set in the array.) cr

\ Check for bits

6 bar1 bar-get-bit [IF]
  .( Bit 6 is set in the array.) cr
[ELSE]
  .( Bit 6 is not set in the array.) cr
[THEN]

13 bar1 bar-get-bit [IF]
  .( Bit 13 is set in the array.) cr
[ELSE]
  .( Bit 13 is not set in the array.) cr
[THEN]


\ Create a bit array with 8 bits on the heap

8 bar-new value bar2

\ Set all bits in the array

bar2 bar-set

\ Reset bits 5..7 in the array

3 5 bar2 bar-reset-bits

\ Print the bit array by executing bar-emit for every bit in the array

: bar-emit ( flag -- )
  1 AND [char] 0 + emit
;

.( Bit array: ) ' bar-emit bar2 bar-execute cr

\ Hamming distance

.( Hamming distance: ) 
bar2 bar1 bar^xor          \ new in version 3
bar1 bar-count . cr

\ Free the array from the heap

bar2 bar-free

