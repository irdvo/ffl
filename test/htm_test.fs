\ ==============================================================================
\
\        htm_test - the test words for the htm module in the ffl
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
\  $Date: 2007-09-29 05:00:58 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/htm.fs
include ffl/tst.fs


.( Testing: htm) cr

\ Reading from a string

htm-create htm1

t{ s" <body>hello</body>" htm1 htm-set-string }t

t{ htm1 htm-read . type cr }t
t{ htm1 htm-read . type cr }t
t{ htm1 htm-read . type cr }t

0 [IF]
\ Reading via a reader

htm-new value htm2

: htm-reader ( w:fileid - c-addr u true | false = Read html info )
  ." reader.." cr
  pad 80 rot read-file throw
  ?dup IF
    pad swap true
  ELSE
    false
  THEN
;

s" test.htm" r/o open-file throw value htm-file

t{ htm-file  ' htm-reader htm2 htm-set-reader }t

t{ htm2 htm-read . }t

t{ htm-file close-file throw }t

t{ htm2 htm-free }t

[THEN]

\ ==============================================================================
