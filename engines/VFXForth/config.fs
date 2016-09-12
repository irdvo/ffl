\ ==============================================================================
\
\                  config - the config in the ffl
\
\             Copyright (C) 2005-2007  Dick van Oudheusden
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
\  ...
\
\ ==============================================================================
\
\ This file is for VFX Forth for Linux or Windows
\
\ ==============================================================================

[undefined] f@ [if]
include %vfxpath%/Lib/Ndp387.fth
[then]

[undefined] ms@ [if]
  [defined] Target_386_Windows [if]
  : ms@ ( -- ms )  GetTickCount  ;
  [then]
  [defined] Target_386_Linux [if]
  : ms@ ( -- ms )  ticks  ;
  [then]
[then]

[UNDEFINED] ffl.version [IF]

( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)

000900 constant ffl.version

( Private words )

variable ffl.endian   1 ffl.endian !


( System Settings )

: end-of-line   \ -- c-addr
\ Counted string for the end of line for the current system
  eol$  ;

s" ADDRESS-UNIT-BITS" environment? 0= [IF] 8 [THEN]
  constant #bits/byte   ( -- +n = Number of bits in a byte )

#bits/byte 1 chars *
  constant #bits/char   ( -- +n = Number of bits in a char )

#bits/byte cell *
  constant #bits/cell   ( -- +n = Number of bits in a cell )

ffl.endian c@ 0=
  constant bigendian?   ( -- flag = Check for bigendian hardware )

create overrule:parse\" ( -- = VFX Forth ships with an incompatible parse\" )


( Extension words )

: lroll		\ u1 u2 -- u3 = Rotate u1 u2 bits to the left
  rol  ;
: rroll		\ u1 u2 -- u3 = Rotate u1 u2 bits to the right
  ror  ;

s" MAX-U" environment? drop constant max-ms@	\ -- u
\ Maximum value of the milliseconds timer

: 1+!		\ a-addr -- = Increase contents of address by 1
  incr  ;

: 1-!		\ a-addr -- = Decrease contents of address by 1
  decr  ;


[UNDEFINED] rdrop [IF]
: rdrop		\ -- ; R: x -- ; same as R> DROP
  ?comp  postpone r>  postpone drop  ;  immediate
[THEN]

: r'@              ( R: x1 x2 -- x1 x2; -- x1 = Fetch the second cell on the return stack )
  ?comp postpone 2r@ postpone drop
; immediate

: d<>		\ d1 d2 -- flag
  d- or 0<>  ;

: ud.		\ ud --
  0 ud.r space  ;


( Float extension words )

1 floats constant float  ( -- n = The size of one float )

0E+0 fconstant 0e+0  ( F: -- r = Float constant 0.0 )
1E+0 fconstant 1e+0  ( F: -- r = Float constant 1.0 )
2E+0 fconstant 2e+0  ( F: -- r = Float constant 2.0 )

Code f>r	\ F: f -- ; R: -- f
\ *G Transfer a float to the return stack.
  pop edx
  add esp, # #-12
  fstp fword 0 [esp]
  push edx
  fnext,
end-code

Code fr>	\ R: f -- ; F: -- f
\ *G Transfer a float from the return stack.
  pop edx
  fld fword 0 [esp]
  add esp, # #12
  push edx
  fnext,
end-code

code fr@	\ R: f -- f ; F: -- f
\ *G Copy a float from the return stack.
  fld   fword 4 [esp]
  fnext,
end-code

( Toolbelt )

include ffl/tlb.fs


( Exceptions )

#-2050 #ErrDef exp-index-out-of-range	"Index out of range"
#-2051 #ErrDef exp-invalid-state	"Invalid state"
#-2052 #ErrDef exp-no-data		"No data available"
#-2053 #ErrDef exp-invalid-parameters	"Invalid parameters"
#-2054 #ErrDef exp-wrong-file-type	"Wrong file type"
#-2055 #ErrDef exp-wrong-file-version	"Wrong file version"
#-2056 #ErrDef exp-wrong-file-data	"Wrong file data"
#-2057 #ErrDef exp-wrong-checksum	"Wrong checksum"
#-2058 #ErrDef exp-wrong-length		"Wrong length"
#-2059 #ErrDef exp-invalid-data   "Invalid data"

[ELSE]
  drop
[THEN]

\ ==============================================================================
