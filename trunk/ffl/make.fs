\ ==============================================================================
\
\               make - the 'make' source file for gforth
\
\               Copyright (C) 2005  Dick van Oudheusden
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
\  $Date: 2008-03-02 15:03:02 $ $Revision: 1.6 $
\
\ ==============================================================================

include tags.fs

.( Forth Foundation Library: ) cr
utime

unused

include ffl/ffl.fs

unused -


.( Compilation Size: ) . .( bytes) cr
  
utime 2swap d- 1 1000 m*/

.( Compilation Time: ) d. .( msec) cr

\ ==============================================================================

