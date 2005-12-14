\ ==============================================================================
\
\                 stc - the struct module in the ffl
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
\  $Date: 2005-12-14 19:27:43 $ $Revision: 1.1.1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] stc.version [IF]


1 constant stc.version


( Public words )

: struct:      ( "name" - w 0 -- Start a struct definition )
  create 
    here  1 cells allot  dup 0! 0
  does>        ( - n -- Return size of struct )
    @
;


: ;struct      ( w n - -- End a struct definition )
  swap !
;


: field:       ( w n "name" - w -- Create a sized structure field )
  create 
    over , + 
  does>        ( w - w -- Determine address with offset )
    @ +
;


: char:        ( w "name" - w -- Create a char structure field )
  1 chars field:
;


: chars:       ( w n "name" - w -- Create char array structure field )
  chars field:
;


: cell:        ( w "name" - w -- Create a cell structure field )
  aligned  1 cells field:
;


: cells:       ( w n "name" - w -- Create cell array structure field )
  swap aligned swap  cells field:
;


: double:      ( w "name" - w -- Create double structure field )
  aligned  2 cells field:
;


: doubles:     ( w n "name" - w -- Create double array structure field )
  swap aligned swap 2* cells field:
;


: float:       ( w "name" - w -- Create a float structure field )
  faligned  1 floats field:
;


: floats:      ( w n "name" - w -- Create float array structure field )
  swap faligned swap  floats field:
;
 
[THEN]

\ ==============================================================================
