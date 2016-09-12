\ ==============================================================================
\
\                  config - the config in the ffl
\
\             Copyright (C) 2005-2006  Dick van Oudheusden
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
\  $Date: 2009-05-20 13:27:22 $ $Revision: 1.12 $
\
\ ==============================================================================
\
\ This file is for fina.
\
\ ==============================================================================


char " parse ffl.version" forth-wordlist search-wordlist 0= [IF]

( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)

000900 constant ffl.version

require timer.fs

: s" 
  [char] " parse state @ if 
    postpone sliteral 
  else 
    here >r s, r> count 
  then 
; immediate


: [DEFINED]
  bl word find   nip
; immediate


: [UNDEFINED]
  bl word find   nip 0=
; immediate

1 cells constant cell


( Private words )

variable ffl.endian   1 ffl.endian !


( System Settings )

create end-of-line    ( -- c-addr = Counted string for the end of line for the current system )
 newline s,


s" ADDRESS-UNIT-BITS" environment? 0= [IF] 8 [THEN] 
  constant #bits/byte   ( -- +n = Number of bits in a byte )
  
#bits/byte 1 chars *
  constant #bits/char   ( -- +n = Number of bits in a char )
  
#bits/byte cell *
  constant #bits/cell   ( -- +n = Number of bits in a cell )  

ffl.endian c@ 0=             
  constant bigendian?   ( -- flag = Check for bigendian hardware )


( Extension words )

: ms@                   ( -- u = Fetch milliseconds timer )
  nstime 1 1000000 m*/ drop
;


s" MAX-U" environment? drop constant max-ms@  ( -- u = Max val of the milliseconds timer )


1 chars 1 = [IF]
: char/            ( n1 -- n2 = Convert address units to chars )
; immediate
[ELSE]
: char/
  1 chars /
;
[THEN]


( Toolbelt )

include ffl/tlb.fs


( Exceptions )

variable exp-next  -2050 exp-next !

: exception      ( c-addr u -- n = Create an exception )
 2drop
 exp-next @
 -1 exp-next +!
;


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
