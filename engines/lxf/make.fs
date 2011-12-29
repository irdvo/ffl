\ ==============================================================================
\
\               make - the 'make' source file for lxf
\
\               Copyright (C) 2008  Dick van Oudheusden
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
\  $Date: 2008-05-24 11:01:18 $ $Revision: 1.1 $
\
\ ==============================================================================


.( Forth Foundation Library: ) cr
ms@

.mem

include ffl/ffl.fs

.mem
  
ms@ swap -

.( Compilation Time: ) . .( msec) cr

\ ==============================================================================

