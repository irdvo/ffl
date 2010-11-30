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
\  $Date: 2009-05-20 13:27:22 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/stt.fs
include ffl/tst.fs


.( Testing: stt) cr

begin-stringtable stt1
+" Forth"
+" Foundation"
+" Library"
+\" \x21"
end-stringtable

t{ 0 stt1 s" Forth"      ?str }t
t{ 1 stt1 s" Foundation" ?str }t
t{ 2 stt1 s" Library"    ?str }t
t{ 3 stt1 s" !"          ?str }t

\ ==============================================================================
