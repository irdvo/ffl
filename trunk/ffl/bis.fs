\ ==============================================================================
\
\              bis - the bit input stream module in the ffl
\
\               Copyright (C) 2008  Dick van Oudheusden
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
\  $Date: 2008-08-20 16:20:18 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] bis.version [IF]

include ffl/str.fs

( bis = Bit Input Stream Module )
( The bis module implements words for reading bits and bytes from a stream   )
( of bytes.                                                                  )

1 constant bis.version

( bis structure )

begin-structure bis%  ( -- n = Get the required space for a bis variable )
  dfield: bis>input          \ the input buffer and length
  field:  bis>hold           \ the temporary hold buffer
  field:  bis>bits           \ the number of bits in the hold buffer (1..24)
  field:  bis>bytes          \ the number of bytes in the hold buffer (1..4)
end-structure


( Bit input stream variable creation, initialisation and destruction )

: bis-init         ( bis -- = Initialise the bit input stream variable )
;


: bis-create       ( "<spaces>name" -- ; -- bis = Create a named bit input stream variable in the dictionary )
  create  here  bis% allot  bis-init
;


: bis-new          ( -- bis = Create a new bit input stream variable on the heap )
  bis% allocate  throw  dup bis-init
;


: bis-(free)       ( bis -- = Free the internal, private variables from the heap )
;


: bis-free         ( bis -- = Free the bit input stream variable from the heap )
  dup bis-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the bis
;


( Input buffer words )

: bis-set          ( c-addr u bis -- = Set the buffer for the input buffer )
  \ ToDo
;


: bis-reset        ( bis -- = Reset the input buffer )
  \ ToDo
;


( Read byte words )

: bis-bits>bytes   ( bis -- = Start reading bytes, dropping remaining bits )
  \ ToDo
;


: bis-read-bytes   ( n1 bis -- false | n2 true = Try reading n1 bytes, return the read number )
  \ ToDo
;


: bis-skip-bytes   ( n bis -- flag = Skip n bytes )
  \ Todo
;


( Read bits words )

: bis-bytes>bits   ( bis -- = Start reading bits )
  \ ToDo
;


: bis-need-bits    ( n bis -- flag = Check if there are n bits available )
  \ ToDo
;


: bis-fetch-bits    ( n1 bis -- n2 = Fetch n1 bits and return the value )
  \ ToDo
;


: bis-next-bits     ( n bis -- = Set n bits processed in the buffer )
  \ ToDo
;


( Inspection )

: bis-dump         ( bis -- = Dump the bit input stream )
  \ ToDo
;

[THEN]

\ ==============================================================================

