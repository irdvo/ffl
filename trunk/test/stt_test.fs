\ ==============================================================================
\
\          stt_test - the test words for the stt module in the ffl
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
\  $Date: 2007-11-21 18:29:11 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/stt.fs
include ffl/tst.fs


.( Testing: stt) cr

begin-stringtable stt1
," Forth"
," Foundation"
," Library"
end-stringtable

t{ 0 stt1 s" Forth"      compare ?0 }t
t{ 1 stt1 s" Foundation" compare ?0 }t
t{ 2 stt1 s" Library"    compare ?0 }t

\ ==============================================================================
