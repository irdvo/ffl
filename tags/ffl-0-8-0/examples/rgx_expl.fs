\ ==============================================================================
\
\          rgx_expl - the regular expression example in the ffl
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
\  $Date: 2008-10-06 18:22:09 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/rgx.fs

\ Create a regular expression variable rgx1 

rgx-create rgx1

\ Compile a regular expression and check the result

s" ((a*)b)*" rgx1 rgx-compile [IF] 
  .( Expression successful compiled) cr
[ELSE]
  .( Compilation failed on position:) . cr
[THEN]

\ Match case sensitive a test string
 
s" abb" rgx1 rgx-cmatch? [IF]
  .( Test string matched) cr
[ELSE]
  .( No match) cr  
[THEN]



\ Create a regular expression variable on the heap
 
rgx-new value rgx2

\ Compile a regular expression for matching a float number

s" [-+\s]?\d+(\.\d+)?" rgx2 rgx-compile [IF]
  .( Expression successful compiled) cr
[ELSE]
  .( Compilation failed on position:) . cr
[THEN]

\ Match a float number

s" -12.47" rgx2 rgx-cmatch? [IF]
  .( Float number matched) cr
[ELSE]
  .( No match) cr
[THEN]

\ Free the variable from the heap

rgx2 rgx-free

