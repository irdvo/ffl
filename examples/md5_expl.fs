\ ==============================================================================
\
\                 md5_expl - the MD5 example in the ffl
\
\               Copyright (C) 2009  Dick van Oudheusden
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
\  $Date: 2009-03-24 18:24:03 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/md5.fs

\ Create a MD5 variable md1 in the dictionary

md5-create md1

\ Update the variable with data

s" The quick brown fox jumps over the lazy dog" md1 md5-update

\ Finish the MD5 calculation resulting in four unsigned 32 bit words
\ on the stack representing the hash value

md1 md5-finish

\ Convert the hash value to a hex string and print it

md5+to-string type cr



\ Create a MD5 variable on the heap

md5-new value md2

\ Update the variable with multiple data

s" The quick brown fox "    md2 md5-update
s" jumps over the lazy dog" md2 md5-update

\ Finish the calculation

md2 md5-finish

\ Convert the hash value to a hex string and print

md5+to-string type cr

\ Free the variable from the heap

md2 md5-free
