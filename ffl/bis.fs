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
\  $Date: 2008-10-26 06:50:20 $ $Revision: 1.5 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] bis.version [IF]


include ffl/stc.fs

( bis = Bit Input Stream Module )
( The bis module implements words for reading bits and bytes from a stream   )
( of bytes. The module uses a cell wide internal buffer to make it possible  )
( to keep reading from succeeding input streams. The maximum number of bits  )
( that can be read per call, is limited to the size of a cell minus one      )
( byte. So if a cell is 4 bytes wide, this maximum is 24. Keep in mind that  )
( the module does not copy the data for the stream, it only stores a         )
( reference to the data.                                                     )


1 constant bis.version

( bis structure )

begin-structure bis%  ( -- n = Get the required space for a bis variable )
  dfield: bis>input          \ the input stream and length
  field:  bis>hold           \ the temporary hold buffer
  field:  bis>bits           \ the number of bits in the hold buffer (1..24)
  field:  bis>bytes          \ the number of bytes in the hold buffer (1..4)
end-structure


( Bit input stream variable creation, initialisation and destruction )

: bis-init         ( bis -- = Initialise the bit input stream variable )
  nil over 0 swap bis>input 2!
  dup bis>hold  0!
  dup bis>bits  0!
      bis>bytes 0!
;


: bis-create       ( "<spaces>name" -- ; -- bis = Create a named bit input stream variable in the dictionary )
  create  here  bis% allot  bis-init
;


: bis-new          ( -- bis = Create a new bit input stream variable on the heap )
  bis% allocate  throw  dup bis-init
;


: bis-free         ( bis -- = Free the bit input stream variable from the heap )
  free throw                 \ Free the bis
;


( Private words )

: bis-bounds       ( n bis -- c-addr1 c-addr2 = Get start and end address of stream when n bytes are needed )
  >r
  r@ bis>input 2@
  2>r
  2r@ rot min                \ Validate needed bytes with stream length
  dup 2r> rot /string        \ Update the stream
  r> bis>input 2!
  bounds                     \ Get the start and end address
;


( Input stream words )

: bis-set          ( c-addr u bis -- = Set the data string for the input stream )
  bis>input 2!
;


: bis-get          ( bis -- c-addr u = Get the data string from the input stream )
  bis>input 2@
;


: bis-reset        ( bis -- = Reset the input buffer, not the input stream )
  dup bis>hold  0!
  dup bis>bits  0!
      bis>bytes 0!
;


( Read byte words )

: bis-bits>bytes   ( bis -- = Start reading bytes, dropping the not byte aligned bits )
  >r
  r@ bis>hold @
  0 r@ bis>bits @!           \ bits = 0
  dup #bits/byte / r@ bis>bytes ! \ bytes = bits / 8
  #bits/byte 1- mod rshift
  r> bis>hold !              \ hold = hold >> bits mod 7
;


: bis-read-bytes   ( n1 bis -- false | n2 true = Try reading n1 bytes via the buffer from the stream, return the read number, n1 <= #bytes/cell )
  >r
  r@ bis>hold  @ over
  r@ bis>bytes @
  tuck - 0 max               \ need: n1 - bytes, max 0

  r@ bis-bounds ?DO
    tuck
    I c@
    swap #bits/byte * lshift \ hold += [buffer] << bytes * 8
    +
    swap 1+                  \ bytes++
  LOOP

  rot                        \ s:hold bytes n1
  2dup = IF
    2drop
    r@ bis>hold  0!          \ Exact match, reset buffer and counter
    r@ bis>bytes 0!
    true
  ELSE
    2dup > IF                \ Buffer is larger
      tuck - r@ bis>bytes !  \ Reduce bytes
      #bits/byte *
      2dup rshift r@ bis>hold ! \ Reduce buffer
      1 swap lshift 1- AND   \ return: hold AND (1 << n1*8)-1
      true
    ELSE                     \ Not enough bytes in the input
      drop
      r@ bis>bytes !         \ Save state in buffer and bytes
      r@ bis>hold  !
      false
    THEN
  THEN
  rdrop 
;


( Read bits words )

: bis-bytes>bits   ( bis -- = Start reading bits from the stream )
  0 over bis>bytes @!
  #bits/byte * swap bis>bits ! \ Move bytes to bits
;


: bis-need-bits    ( n bis -- flag = Make sure there are n bits from the stream available in the buffer )
  >r
  r@ bis>hold @ over
  r@ bis>bits @
  tuck - 0 max                  \ need: max(n - bits, 0)
  #bits/byte 1- + #bits/byte /  \ need in bytes

  r@ bis-bounds ?DO             \ From buffer
    tuck
    I c@
    swap lshift                 \ hold += [buffer] << bits
    +
    swap #bits/byte +           \ bits += 8
  LOOP
  tuck
  r@ bis>bits !                 \ Save current state
  r> bis>hold !
  <=
;


: bis-fetch-bits    ( n1 bis -- n2 = Fetch n1 bits from the buffer and return the value )
  swap 1 swap lshift 1-         \ Mask: (1 << n1)-1 for hold
  swap bis>hold @ AND
;


: bis-next-bits     ( n bis -- = Set n bits processed in the buffer )
  >r
  r@ bis>hold @ over rshift r@ bis>hold !      \ hold >>= n
  negate r> bis>bits +!                        \ bits -= n
;


( Inspection )

: bis-dump         ( bis -- = Dump the bit input stream )
  ." bis:" dup . cr
    ."  input   :" dup bis>input 2@ . . cr
    ."  hold    :" dup bis>hold     ? cr
    ."  bits    :" dup bis>bits     ? cr
    ."  bytes   :"     bis>bytes    ? cr
;

[THEN]

\ ==============================================================================

