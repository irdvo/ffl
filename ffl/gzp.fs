\ ==============================================================================
\
\               gzp - the gzip base module in the ffl
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
\  $Date: 2008-06-28 06:12:29 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] gzp.version [IF]

include ffl/lbf.fs

( gzp = GZip Base Module )
( The gzp module implements the base words for using the GZip compression    )
( algoritme. The module is used by the GZip file [gzf] and GZip stream [gzs] )
( modules.                                                                   )


1 constant gzp.version

( gzp constants )

0 constant gzp.ok            ( -- n = [De]compression is finished okee )
1 constant gzp.more          ( -- n = [De]compression needs more data )

( gzp structure )

begin-structure gzp%  ( -- n = Get the required space for a gzp variable )
  field:  gzp>input          \ the current input buffer
  field:  gzp>in-len         \ the input buffer length
  field:  gzp>hold           \ the current data from the input buffer
  field:  gzp>bits           \ the number of bits in the hold
  field:  gzp>state          \ the current state (as xt)
  field:  gzp>result         \ the result of the conversion
  field:  gzp>out-len        \ the length of the conversion
  \ crc?
  lbf%
  +field  gzp>output         \ the output buffer
end-structure


( GZip file variable creation, initialisation and destruction )

: gzp-init         ( gzp -- = Initialise the GZip file variable )
  dup  gzp>input    0!
  dup  gzp>in-len   0!
  dup  gzp>hold     0!
  dup  gzp>bits     0!
  dup  gzp>state    0!
  dup  gzp>result   0!
  dup  gzp>out-len  0!
  1 chars over
  32768   swap 
       gzp>output lbf-init   \ initialise the output buffers for chars and size 32k byte
  drop
\ ToDo
;


: gzp-(free)       ( gzp -- = Free the internal, private variables from the heap )
  dup gzp>output  lbf-(free)
  drop
  \ ToDo
;


: gzp-create       ( "<spaces>name" -- ; -- gzp = Create a named GZip base variable in the dictionary )
  create   here   gzp% allot   gzp-init
;


: gzp-new          ( -- gzp = Create a new GZip base variable on the heap )
  gzp% allocate  throw  dup gzp-init
;


: gzp-free         ( gzp -- = Free the variable from the heap )
  dup gzp-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the gzp
;


( Private bit words )

: gzp-need-bits    ( n gzp -- flag = Reserve n bits )
  BEGIN                      \ while bits < n and index < length Do
    2dup gzp>bits @ >
    over gzp>in-len @ AND
  WHILE
    dup  gzp>input @ c@      \   hold = hold + ([input] << bits)
    dup  gzp>bits  @ lshift
    over gzp>hold +!
    
    #bits/byte over gzp>bits +!    \ bits = bits + 8
    1 chars    over gzp>input +!   \ input++
               dup  gzp>in-len 1-! \ in-len--
  REPEAT
  gzp>bits @ <=              \ result = (n <= bits)
;



: gzp-bits         ( n1 gzp -- n2 = Get n1 bits )
  gzp>hold @
  swap 1 swap lshift 1 -  \ hold & ((1 << n1) - 1)
  AND
;
  

: gzp-drop-bits    ( n gzip -- = Drop n bits )
  2dup
  tuck gzp>hold @ swap rshift  \ hold = hold >> n
  swap gzp>hold !
  swap negate swap gzp>bits +! \ bits -= n
;


( Private inflate words )

: gzp-do-stored    ( gzp -- n = Process uncompressed data )
;


: gzp-do-type      ( gzp -- n = Check last block and inflation type )
  3 over gzp-need-bits IF
    1 over gzp-bits ." Last:" . cr
    2 over gzp-bits ." Type:" . cr
    ['] gzp-do-stored swap gzp>state !
    gzp.ok
  ELSE
    drop
    gzp.more
  THEN 
;



( Inflate words )

: gzp-init-inflate ( gzp -- = Start the inflation of data )
  ['] gzp-do-type over gzp>state !
  dup gzp>bits 0!
  dup gzp>hold 0!
  
  drop
  \ ToDo
;


: gzp-inflate      ( c-addr1 u1 gzp -- c-addr2 u2 | 0 n = Inflate data with result code n )
  tuck gzp>in-len    !       \ Save buffer
  tuck gzp>input     !
  dup  gzp>result   0!
  BEGIN
    dup dup gzp>state @ execute
    ?dup 
  UNTIL
  \ ToDo
  tuck gzp>result !
;


: gzp-end-inflate  ( gzp -- = Finish the inflation of data )
;


( Private deflate words )


( Deflate words )

: gzp-init-deflate ( gzp -- = Start the deflation of data )
;


: gzp-deflate      ( c-addr1 u1 gzp -- c-addr2 u2 | 0 n = Deflate data with result code n )
;


: gzp-end-deflate  ( gzp -- = Finish the deflation of data )
;


( Inspection )

: gzp-dump   ( gzp - = Dump the gzp )
  ." gzp:" dup . cr
    ."  input   :" dup gzp>input    ? cr
    ."  in-len  :" dup gzp>in-len   ? cr
    ."  hold    :" dup gzp>hold     ? cr
    ."  bits    :" dup gzp>bits     ? cr
    ."  state   :" dup gzp>state    ? cr
    ."  result  :" dup gzp>result   ? cr
    ."  out-len :" dup gzp>out-len  ? cr
    ."  output  :" dup gzp>output   lbf-dump cr
  drop
;
  
[THEN]

\ ==============================================================================
