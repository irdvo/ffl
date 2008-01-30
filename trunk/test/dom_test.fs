\ ==============================================================================
\
\        xos_test - the test words for the xos module in the ffl
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
\  $Date: 2008-01-30 06:54:00 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/dom.fs
include ffl/tst.fs


.( Testing: dom) cr

t{ dom-create dom1 }t


: dom-test-reader   ( file-id -- c-addr u | 0 )
  pad 64 rot read-file throw
  dup IF
    pad swap 
    2dup type cr
  THEN
;

t{ s" test.xml" r/o open-file throw value dom.file }t

t{ dom.file ' dom-test-reader dom1 dom-read-reader ?true }t

\ dom1 dom-dump

t{ dom1 dom-write-string ?true type }t
 

\ ==============================================================================
