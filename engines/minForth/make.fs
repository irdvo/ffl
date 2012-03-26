\ ==============================================================================
\
\               make - the 'make' source file for minForth
\
\               Copyright (C) 2005  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU Lesser General Public
\ License as published by the Free Software Foundation; either
\ version 3 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ Lesser General Public License for more details.
\
\ You should have received a copy of the GNU Lesser General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2007-04-01 04:40:12 $ $Revision: 1.1 $
\
\ ==============================================================================

.( Forth Foundation Library: ) cr
msecs

unused unused-n

include ffl/ffl.fs

unused-n - swap unused - +

.( Compilation Size: ) . .( bytes) cr

msecs swap -

.( Compilation Time: ) . .( msec) cr

\ ==============================================================================

