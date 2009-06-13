\ ==============================================================================
\
\              a32_expl - the Adler32 example in the ffl
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
\  $Date: 2008-04-05 08:05:28 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/a32.fs


\ Create an Adler32 variable ad1 in the dictionary

a32-create ad1

\ Update the variable with data

s" The quick brown fox jumps over the lazy dog" ad1 a32-update

\ Finish the Adler32 calculation resulting in unsigned 32 bit word
\ on the stack representing the value

ad1 a32-finish

\ Convert the value to a hex string and print

a32+to-string type cr



\ Create an Adler32 variable on the heap

a32-new value ad2

\ Update the variable with multiple data

s" The quick brown fox " ad2 a32-update
s" jumps over the lazy dog" ad2 a32-update

\ Finish the calculation

ad2 a32-finish

\ Convert the value to a hex string and print

a32+to-string type cr

\ Free the variable from the heap

ad2 a32-free
