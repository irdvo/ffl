\ ==============================================================================
\
\              b64_expl - the Base64 example in the ffl
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
\  $Date: 2008-04-05 08:05:28 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/b64.fs


\ Create a string in the dictionary for storing the encoded characters

str-create inbase64

\ Encode the string: The quick brown fox jumps over the lazy dog

s" The quick brown fox jumps over the lazy dog" inbase64 b64-encode

\ Print the result

.( Base64 encoding of 'The quick brown fox jumps over the lazy dog' is: ) type cr

\ The result is also stored in inbase64

.( inbase64 contents: ) inbase64 str-get type cr


\ Create a string on the heap for storing the decoded characters

str-new value frombase64

\ Decode the encoded string of The quick brown fox jumps over the lazy dog

s" VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IGRvZw==" frombase64 b64-decode

.( Base64 decoding of 'VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IGRvZw==' is: ) type cr

\ The result is also stored in frombase64

.( frombase64 contents: ) frombase64 str-get type cr

\ Free the string from the heap

frombase64 str-free
