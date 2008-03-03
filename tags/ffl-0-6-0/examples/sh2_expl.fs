\ ==============================================================================
\
\              sh2_expl - the SHA-256 example in the ffl
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
\  $Date: 2007-06-08 06:49:35 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/sh2.fs

\ Create a SHA-256 variable sh1 in the dictionary

sh2-create sh1

\ Update the variable with data

s" The quick brown fox jumps over the lazy dog" sh1 sh2-update

\ Finish the SHA-256 calculation resulting in 8 unsigned 32 bit words
\ on the stack representing the hash value

sh1 sh2-finish

\ Convert the hash value to a hex string and print

sh2+to-string type cr



\ Create a SHA-256 variable on the heap

sh2-new value sh2

\ Update the variable with multiple data

s" The quick brown fox " sh2 sh2-update
s" jumps over the lazy dog" sh2 sh2-update

\ Finish the calculation

sh2 sh2-finish

\ Convert the hash value to a hex string and print

sh2+to-string type cr

\ Free the variable from the heap

sh2 sh2-free
