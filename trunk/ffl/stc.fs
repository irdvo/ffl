\ ==============================================================================
\
\                 stc - the struct module in the ffl
\
\             Copyright (C) 2005-2007  Dick van Oudheusden
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
\  $Date: 2008-05-24 11:01:18 $ $Revision: 1.8 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] stc.version [IF]


( stc = ANS Structure module )
( The stc module implements ANS structures. )


2 constant stc.version


( ANS Structure syntax words )

[UNDEFINED] begin-structure [IF]
: begin-structure   ( "<spaces>name" -- structure-sys ; -- n = Start definition of a named structure, return the structure size )
  create
    here 0 , 0
  does>
    @
;
[THEN]


[UNDEFINED] end-structure [IF]
: end-structure   ( structure-sys -- = End a structure definition )
  swap !
;
[THEN]


( ANS field definition words )

[UNDEFINED] +field [DEFINED] overrule:+field OR [IF]
: +field   ( structure-sys n "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field of size n bytes, return the field address )
  create
    over , +
  does>
    @ +
;
[THEN]


[UNDEFINED] cfield: [IF]
: cfield:   ( structure-sys "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field of 1 char, return the field address )
  1 chars +field
;
[THEN]


[UNDEFINED] field: [IF]
: field:   ( structure-sys "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field of 1 cell, return the field address )
  aligned cell +field
;
[THEN]


[UNDEFINED] dfield: [IF]
: dfield:   ( structure-sys "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field of 1 double, return the field address )
  aligned 2 cells +field
;
[THEN]


[UNDEFINED] ffield: [DEFINED] faligned AND [IF]
: ffield:   ( structure-sys "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field of 1 float, return the field address )
  faligned 1 floats +field
;
[THEN]


[UNDEFINED] sffield: [DEFINED] sfaligned AND [IF]
: sffield:   ( structure-sys "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field of 1 single float, return the field address )
  sfaligned 1 sfloats +field
;
[THEN]


[UNDEFINED] dffield: [DEFINED] dfaligned AND [IF]
: dffield:   ( structure-sys "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field of 1 double float, return the field address )
  dfaligned 1 dfloats +field
;
[THEN]


( Array field definition words )

[UNDEFINED] cfields: [IF]
: cfields:   ( structure-sys n "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field of n chars, return the field address )
  chars +field
;
[THEN]


[UNDEFINED] fields: [IF]
: fields:   ( structure-sys n "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field of n cells, return the field address )
  swap aligned swap cells +field
;
[THEN]


[UNDEFINED] dfields: [IF]
: dfields:   ( structure-sys n "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field of n doubles, return the field address )
  swap aligned swap 2* cells +field
;
[THEN]


[UNDEFINED] ffields: [DEFINED] faligned AND [IF]
: ffields:   ( structure-sys n "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field of n floats, return the field address )
  swap faligned swap floats +field
;
[THEN]


[THEN]

\ ==============================================================================
