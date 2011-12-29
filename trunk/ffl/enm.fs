\ ==============================================================================
\
\                 enm - the enumeration module in the ffl
\
\             Copyright (C) 2007  Dick van Oudheusden
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
\  $Date: 2007-12-12 19:36:38 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] enm.version [IF]


( enm = Enumerations )
( The enm module implements simple enumerations. )


1 constant enm.version


( Enumerations syntax words )

: begin-enumeration  ( -- enum-sys = Start an enumeration definition, the first enumeration will start from 0 )
  0
;


: end-enumeration   ( enum-sys -- = End an enumeration definition )
  drop
;


( Enumeration definition words )

: enum:   ( enum-sys "<spaces>name" -- enum-sys ;  -- n = Define an enumeration, increment for the next enumeration, return the enum value )
  dup constant
  1+
;


: >enum:   ( enum-sys n "<spaces>name" -- enum-sys ; -- n = Define an enumeration starting with n, return the enum value )
  nip
  enum:
;

[THEN]

\ ==============================================================================
