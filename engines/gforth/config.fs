\ ==============================================================================
\
\                  config - the config in the ffl
\
\             Copyright (C) 2005-2006  Dick van Oudheusden
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
\  $Date: 2006-08-10 18:57:44 $ $Revision: 1.4 $
\
\ ==============================================================================
\
\ This file is for gforth.
\
\ ==============================================================================


s" ffl.version" forth-wordlist search-wordlist 0= [IF]


( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)


000300 constant ffl.version


( Private words )
  
variable sys.endian   1 sys.endian !


( System Settings )

create sys.eol     ( - c-addr = Counted string for the end of line for the current system )
  1 c, 10 c,         \ unix: lf
\ 2 c, 13 c, 10 c,   \ dos:  cr lf
  
  
8                           constant sys.bits-in-byte   ( - n = Number of bits in a byte )

sys.bits-in-byte 1 chars *  constant sys.bits-in-char   ( - n = Number of bits in a char )
  
sys.bits-in-byte cell *     constant sys.bits-in-cell   ( - n = Number of bits in a cell )  

sys.endian c@ 0=            constant sys.bigendian      ( - f = Check for bigendian hardware )
  
  
( Extension words )

\ : [DEFINED]   
  
: 2+               ( n - n+2 = Add two to tos)
  1+ 1+
;


: lroll            ( u1 u - u2 = Rotate u1 u bits to the left )
  2dup lshift >r
  [ cell 8 * ] literal swap - rshift r>
  or
;


: 0!               ( w - = Set zero in address )
  0 swap !
;


: nil!             ( w - = Set nil in address )
  nil swap !
;


: nil=             ( w - f = Check for nil )
  nil =
;


: nil<>            ( w - f = Check for unequal to nil )
  nil <>
;


: 1+!              ( w - = Increase contents of address by 1 )
  1 swap +!
;


: 1-!              ( w - = Decrease contents of address by 1 )
  -1 swap +!
;


: @!               ( w a - w = First fetch the contents and then store the new value )
  dup @ -rot !
;


: index2offset     ( n:index n:length - n:offset = Convert an index [-length..length> into an offset [0..length> )
  over 0< IF
    +
  ELSE
    drop
  THEN
;


( Public Exceptions )

s" Index out of range" exception constant exp-index-out-of-range ( - n = Index out of range exception number )
s" Invalid state"      exception constant exp-invalid-state      ( - n = Invalid state exception number )
s" No data available"  exception constant exp-no-data            ( - n = No data available exception number )
s" Invalid parameters" exception constant exp-invalid-parameters ( - n = Invalid parameters on stack )

[ELSE]
  drop
[THEN]

\ ==============================================================================

