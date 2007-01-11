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
\  $Date: 2006-12-10 07:47:29 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] stc.version [IF]


( stc = Struct module )
( The stc module implements a simple struct mechanism for forth. )


1 constant stc.version


( Structure syntax words )

: struct:      ( C: "name" - w 0 R: - n = Start a named struct definition, leave the size on stack )
  create 
    here  1 cells allot  dup 0! 0
  does>
    @
;


: ;struct      ( w n - = End a struct definition )
  swap !
;


( Field definition words )

: field:       ( C: w n "name" - w R: w - w = Create a named structure field, add offset to address )
  create 
    over , + 
  does>
    @ +
;


: char:        ( C: w "name" - w R: w - w = Create a named char structure field, add offset to address )
  1 chars field:
;


: chars:       ( C: w n "name" - w R: w - w = Create named char array structure field, add offset to address )
  chars field:
;


: cell:        ( C: w "name" - w R: w - w = Create a named cell structure field, add offset to address )
  aligned  1 cells field:
;


: cells:       ( C: w n "name" - w R: w - w = Create named cell array structure field, add offset to address )
  swap aligned swap  cells field:
;


: double:      ( C: w "name" - w R: w - w = Create named double structure field, add offset to address )
  aligned  2 cells field:
;


: doubles:     ( C: w n "name" - w R: w - w = Create named double array structure field, add offset to address )
  swap aligned swap 2* cells field:
;

[DEFINED] faligned [IF]

: float:       ( C: w "name" - w R: w - w = Create a named float structure field, add offset to address )
  faligned  1 floats field:
;


: floats:      ( C: w n "name" - w R: w - w = Create named float array structure field, add offset to address )
  swap faligned swap  floats field:
;
[THEN]

[THEN]

\ ==============================================================================
