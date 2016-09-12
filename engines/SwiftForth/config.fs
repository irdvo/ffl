\ ==============================================================================
\
\                  config - the config in the ffl
\
\             Copyright (C) 2012  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU Lesser General Public
\ License as published by the Free Software Foundation; either
\ version 2 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ General Public License for more details.
\
\ You should have received a copy of the GNU Lesser General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\ This file is for SwiftForth.
\
\ ==============================================================================

[UNDEFINED] f@ [IF]
  requires fpmath.f
[THEN]

s" ffl.version" forth-wordlist search-wordlist 0= [IF]


( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)


000800 constant ffl.version


( Private words )
  
variable ffl.endian   1 ffl.endian !


( System Settings )

create end-of-line    ( -- c-addr = Counted string for the end of line for the current system )
  1 c, 10 c,         \ All hosts except dos  (gforth 0.6.2)
\ 2 c, 13 c, 10 c,   \ dos:  cr lf


s" ADDRESS-UNIT-BITS" environment? 0= [IF] 8 [THEN] 
  constant #bits/byte   ( -- n = Number of bits in a byte )
  
#bits/byte 1 chars *
  constant #bits/char   ( -- n = Number of bits in a char )
  
#bits/byte cell *
  constant #bits/cell   ( -- n = Number of bits in a cell )  

ffl.endian c@ 0=             
  constant bigendian?   ( -- flag = Check for bigendian hardware )


( gforth words )

aka r>drop rdrop

0 constant nil

: 0>=  0< INVERT ;
: 0<=  0> INVERT ;
: u<=  u> INVERT ;
: d<>  d= INVERT ;

aka du. ud.

: sgn ( n -- -1|0|1 )
  dup 0> 1 and swap 0< or ;

1 chars 1 = [IF]
  : char/ ;
[ELSE]
  : char/ 1 chars / ;
[THEN]

( Extension words )

aka counter ms@ ( -- u = Fetch milliseconds timer )

s" MAX-U" environment? drop constant max-ms@   ( -- u = Maximum value of the milliseconds timer )


\ Command line arguments in SwiftForth i386-Linux

\ FFL assumes gforth's behavior, in which system-processed args are
\ stripped from ARGV .  SwiftForth does _not_ do this, cannot be made to
\ do this without a recompile.

\ The following assumes that SwiftForth has been started as "<cmd>
\ <filename> ...args" (or "<cmd>", with interactive loading of FFL.)
\ If it was started as "<cmd> <a bunch of code that happens to load
\ FFL>", FFL's arg handling will misbehave.
 
version zcount s" Linux" search nip nip 0= [IF]
  : argc  cmdline argc ;
  : argv  cmdline rot argv ;
[THEN]

argc 2 - 1 max constant #args  ( -- n = Get the number of command line arguments )

argc 1 = [IF]
  aka argv arg@ 
[ELSE]
: arg@  ( n -- c-addr u = Get the nth command line argument )
  2 + argv ;
[THEN]

aka off 0!   ( a-addr -- = Set address to zero )

aka ++ 1+!   ( a-addr -- = Increase contents of address by 1 )

aka compare(nc) icompare   ( c-addr1 u1 c-addr2 u2 -- n = Compare case-insensitive two strings and return the result [-1,0,1] )

cell 4 = [IF] 
: <w@   ( w-addr -- n = Fetch a word, 16 bit, sign extend )
  w@ dup [ hex ] 0FFF > FFFF0000 [ decimal ] and or ; 
[THEN]

[DEFINED] sl@ [IF]
' sl@ alias <l@   ( l-addr -- n = Fetch a long word, 32 bit, sign extend )
[THEN]


[DEFINED] d>f [IF]
1 floats constant float

( Float extension constants )

0E+0 fconstant 0e+0  ( F: -- r = Float constant 0.0 )
1E+0 fconstant 1e+0  ( F: -- r = Float constant 1.0 )
2E+0 fconstant 2e+0  ( F: -- r = Float constant 2.0 )


( Float extension words )

\ FFL's expected fatan2 definitely isn't SwiftForth's.
: fatan2           ( F: r r -- r )
  f2dup f0< f0< and if fswap fabs fswap [fatan2] fnegate exit then [fatan2] 
;

[THEN]


( Toolbelt )

include ffl/tlb.fs


( Exceptions )

THROW#
s" Index out of range" >THROW ENUM exp-index-out-of-range ( -- n = Index out of range exception number )
s" Invalid state"      >THROW ENUM exp-invalid-state      ( -- n = Invalid state exception number )
s" No data available"  >THROW ENUM exp-no-data            ( -- n = No data available exception number )
s" Invalid parameters" >THROW ENUM exp-invalid-parameters ( -- n = Invalid parameters on stack )
s" Wrong file type"    >THROW ENUM exp-wrong-file-type    ( -- n = Wrong file type )
s" Wrong file version" >THROW ENUM exp-wrong-file-version ( -- n = Wrong file version )
s" Wrong file data"    >THROW ENUM exp-wrong-file-data    ( -- n = Wrong file data )
s" Wrong checksum"     >THROW ENUM exp-wrong-checksum     ( -- n = Wrong checksum )
s" Wrong length"       >THROW ENUM exp-wrong-length       ( -- n = Wrong length )
s" Invalid data"       >THROW ENUM exp-invalid-data       ( -- n = Invalid data exception number )
TO THROW#

[ELSE]
  drop
[THEN]

\ ==============================================================================

