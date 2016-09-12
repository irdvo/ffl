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
\  $Date: 2009-05-20 13:27:22 $ $Revision: 1.3 $
\
\ ==============================================================================
\
\ This file is for ntf/lxf.
\
\ ==============================================================================


[UNDEFINED] ffl.version [IF]


( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)


000900 constant ffl.version


( Private words )
  
variable ffl.endian   1 ffl.endian !


( System Settings )

create end-of-line   ( -- c-addr = Counted string for the end of line for the current system )
linux: 1 c, 10 c,         \ unix: lf
win:   2 c, 13 c, 10 c,   \ dos:  cr lf
  
  
s" ADDRESS-UNIT-BITS" environment? 0= [IF] 8 [THEN] 
  constant #bits/byte   ( -- +n = Number of bits in a byte )
  
#bits/byte 1 chars *
  constant #bits/char   ( -- +n = Number of bits in a char )
  
#bits/byte cell *
  constant #bits/cell   ( -- +n = Number of bits in a cell )  

ffl.endian c@ 0=             
  constant bigendian?   ( -- flag = Check for bigendian hardware )


( Extension words )

s" MAX-U" environment? drop constant max-ms@   ( -- u = Maximum value of the milliseconds timer )


:m 0!               ( a-addr -- = Set zero in address )
  0 swap !
;


0 constant nil     ( -- nil = Nil constant )


:m nil!             ( a-addr -- = Set nil in address )
  nil swap !
;


:m nil=             ( addr -- flag = Check for nil )
  nil =
;


:m nil<>            ( addr -- flag = Check for unequal to nil )
  nil <>
;


:m nil<>?           ( addr -- false | addr true = If addr is nil, then return false, else return address with true )
  ?dup
;


:m 1+!              ( a-addr -- = Increase contents of address by 1 )
  1 swap +!
;


:m 1-!              ( a-addr -- = Decrease contents of address by 1 )
  -1 swap +!
;


:m 0>=
  0< 0=
;


:m 0<=
  0> 0=
;


:m rdrop           ( R: x -- )
  r>drop
;


:m r'@             ( R: x1 x2 -- x1 x2, -- x1 = Fetch the second cell on the return stack )
  2r@ drop
;


:m d<>  d= 0= ;

( Float extension words )

:m float 1 floats ;

synonym 0e+0 f0.0 \ 0E+0 fconstant 0e+0  ( -- r = Float constant 0.0 )
synonym 1e+0 f1.0 \ 1E+0 fconstant 1e+0  ( -- r = Float constant 1.0 )
2E+0 fconstant 2e+0  ( -- r = Float constant 2.0 )


:m f2dup            ( r1 r2 -- r1 r2 r1 r2 = Duplicate two floats )
  fover fover
;


:m ftuck            ( r1 r2 -- r2 r1 r2 = Swap and over )
  fswap fover
;


variable exp-next  -2050 exp-next !

: exception      ( c-addr u -- n = Create an exception )
  exp-next @ dup >r
  -rot >ior
  exp-next 1-!
  r>
;


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
