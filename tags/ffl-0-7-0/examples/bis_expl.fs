\ ==============================================================================
\
\           bis_expl - the bit input stream example in the ffl
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
\  $Date: 2008-09-07 06:52:17 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/bis.fs


\ Create an bit input stream variable bis1 in the dictionary

bis-create bis1

\ Give the stream some input data (note: the stream does not make a copy of the data)

: bis-ab ( -- c-addr u ) s" ab" ;

bis-ab bis1 bis-set

\ Try reading 10 bits from the stream

10 bis1 bis-need-bits [IF]
  .( There are 10 bits available in the buffer. ) cr
[ELSE]
  .( There are not enough bits availabe in the buffer, refill.) cr
[THEN]

\ Read the bits from the stream

.( The 10 bits from the stream:) 10 bis1 bis-fetch-bits . cr

\ Indicate the the 10 bits are processed

10 bis1 bis-next-bits

\ Try reading another 10 bits from the stream

10 bis1 bis-need-bits [IF]
  .( There are another 10 bits available in the buffer.) cr
[ELSE]
  .( There are not enough bits available in the buffer, refill.) cr
[THEN]

\ Refill the input stream

: bis-cd ( -- c-addr u) s" cd" ;

bis-cd bis1 bis-set

\ Again try reading another 10 bits from the stream

10 bis1 bis-need-bits [IF]
  .( There are another 10 bits available in the buffer.) cr
[ELSE]
  .( There are not enough bits available in the buffer, refill.) cr
[THEN]

\ Read the bits from the stream and indicate that the bits are processed

.( The 10 bits from the stream:) 10 bis1 bis-fetch-bits . cr

\ Indicate the the 10 bits are processed

10 bis1 bis-next-bits

