\ ==============================================================================
\
\                  config - the config in the ffl
\
\            Copyright (C) 2005-2008  Dick van Oudheusden
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
\  $Date: 2009-05-20 13:27:22 $ $Revision: 1.5 $
\
\ ==============================================================================
\
\ This file is for flk.
\
\ ==============================================================================


s" ffl.version" forth-wordlist search-wordlist 0= [IF]


( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)


000900 constant ffl.version


( Private words )
  
variable ffl.endian   1 ffl.endian !


( System Settings )

create end-of-line     ( -- c-addr = Counted string for the end of line for the current system )
  1 c, 10 c,         \ unix: lf
\ 2 c, 13 c, 10 c,   \ dos:  cr lf


s" ADDRESS-UNIT-BITS" environment? 0= [IF] 8 [THEN] 
  constant #bits/byte   ( -- +n = Number of bits in a byte )
  
#bits/byte 1 chars *
  constant #bits/char   ( -- +n = Number of bits in a char )

1 cells
  constant cell         ( -- +n = Number of bytes in a cell )
  
#bits/byte cell *
  constant #bits/cell   ( -- +n = Number of bits in a cell )  

ffl.endian c@ 0=             
  constant bigendian?   ( -- flag = Check for bigendian hardware )


( Extension words )

: [DEFINED] 
  bl word find   nip 
; immediate


: [UNDEFINED] 
  bl word find   nip 0= 
; immediate


: ms@                                                   ( -- u = Fetch milliseconds timer )
  time_of_day
  swap 1000 um*
  rot  1000 /
  m+ drop
;


s" MAX-U" environment? drop constant max-ms@            ( -- u = Maximum value of the milliseconds timer )


: icompare         ( c-addr1 u1 c-addr2 u2 -- n = Compare case insensitive )
  caps-compare
;


: is
  postpone to
; immediate


[DEFINED] floats [IF]

( Float extension constants )

1 floats constant float

0E+0 fconstant 0e+0  ( F: -- r = Float constant 0.0 )
1E+0 fconstant 1e+0  ( F: -- r = Float constant 1.0 )
2E+0 fconstant 2e+0  ( F: -- r = Float constant 2.0 )


( Float extension words )

[THEN]


( Toolbelt )

include ffl/tlb.fs


( Exceptions )

s" Index out of range" exception constant exp-index-out-of-range ( -- n = Index out of range exception number )
s" Invalid state"      exception constant exp-invalid-state      ( -- n = Invalid state exception number )
s" No data available"  exception constant exp-no-data            ( -- n = No data available exception number )
s" Invalid parameters" exception constant exp-invalid-parameters ( -- n = Invalid parameters on stack )
s" Wrong file type"    exception constant exp-wrong-file-type    ( -- n = Wrong file type )
s" Wrong file version" exception constant exp-wrong-file-version ( -- n = Wrong file version )
s" Wrong file data"    exception constant exp-wrong-file-data    ( -- n = Wrong file data )
s" Wrong checksum"     exception constant exp-wrong-checksum     ( -- n = Wrong checksum )
s" Wrong length"       exception constant exp-wrong-length       ( -- n = Wrong length )
s" Invalid data"       exception constant exp-invalid-data       ( -- n = Invalid data exception number )

[ELSE]
  drop
[THEN]

\ ==============================================================================

