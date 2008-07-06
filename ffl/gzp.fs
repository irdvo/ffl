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
\  $Date: 2008-07-06 14:44:49 $ $Revision: 1.2 $
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
1 constant gzp.done          ( -- n = [De]compression is done )
2 constant gzp.more          ( -- n = [De]compression step needs more data )
3 constant gzp.eof           ( -- n = End of input file )


( gzp structure )

begin-structure gzp%  ( -- n = Get the required space for a gzp variable )
  dfield: gzp>input          \ the current input buffer and length
  field:  gzp>hold           \ the current data from the input buffer
  field:  gzp>bits           \ the number of bits in the hold
  field:  gzp>state          \ the current state (as xt)
  field:  gzp>result         \ the result of the conversion
  \ crc?
  lbf%
  +field  gzp>output         \ the output buffer
end-structure


( GZip file variable creation, initialisation and destruction )

: gzp-init         ( gzp -- = Initialise the GZip file variable )
  nil over 0 
  swap gzp>input    2!
  dup  gzp>hold     0!
  dup  gzp>bits     0!
  dup  gzp>state    0!
  dup  gzp>result   0!
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


( Input buffer words )

: gzp-clear        ( gzp -- Clear the input buffer )
  nil over 0 
  swap gzp>input 2!
  dup  gzp>hold  0!
       gzp>bits  0!
;


: gzp-set          ( c-addr u gzp -- Set the input buffer )
  dup gzp>input 2@ nip exp-invalid-state AND throw

  gzp>input 2!
;


: gzp-align        ( gzp -- Remove bits for going to byte boundary )
  dup gzp>hold 0!
  dup gzp>bits @ #bits/byte >= IF
    ." Warning: more than a byte in the hold buffer "
  THEN
  gzp>bits 0!
;


: gzp-byte          ( gzp -- n true | false = Get a byte )
  >r
  r@ gzp>input 2@ dup IF
    over c@ -rot
    1 /string r@ gzp>input 2!
    true
  ELSE
    2drop false
  THEN
  rdrop
;


: gzp-byte2        ( gzp -- n true | false = Get two bytes )
  >r
  r@ gzp>input 2@ dup 1 > IF
    over dup c@ swap char+ c@ 8 lshift + -rot
    2 /string r@ gzp>input 2!
    true
  ELSE
    2drop false
  THEN
  rdrop
;


: gzp-byte4        ( gzp -- n true | false = Get four bytes )
  >r
  r@ gzp>input 2@ dup 3 > IF
    over
    dup c@ swap char+
    dup c@ 8  lshift swap char+
    dup c@ 16 lshift swap char+
        c@ 24 lshift + + + -rot
    4 /string r@ gzp>input 2!
    true
  ELSE
    2drop false
  THEN
  rdrop
;


: gzp-skip-bytes   ( n gzp -- flag = Skip n bytes )
  >r
  r@ gzp>input 2@ rot 2dup >= IF  \ If input length >= n
    /string r@ gzp>input 2!       \   Remove from input
    true
  ELSE
    drop 2drop
    false
  THEN
  rdrop
;


: gzp-need-bits    ( n gzp -- flag = Reserve n bits )
  >r
  r@ gzp>input 2@ rot
  BEGIN
    2dup r@ gzp>bits @ > AND \ while bits < n and length > 0 Do
  WHILE
    -rot
    over c@
    r@ gzp>bits @ lshift
    r@ gzp>hold +!           \  hold = hold + ([input] << bits)

    1 /string                \  input++
    #bits/byte @ gzp>bits +! \  bits += 8
    rot
  REPEAT
  r@ gzp>input 2!
  r> gzp>bits @ <=           \ result = (n <= bits)
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


( Output buffer words )

: gzp-get          ( u1 gzp -- c-addr u2 | 0 = Get n bytes from the output buffer )
  gzp>output lbf-get'
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
  
  drop
  \ ToDo
;


: gzp-inflate      ( n1 gzp -- n2 = Inflate data till n bytes in output buffer with result code n )
  gzp.ok
  BEGIN
    \ ToDo stack
    dup gzp.ok = IF
      2dup gzp>output lbf-length'@ >
    ELSE
      false
    THEN
  WHILE
    dup dup gzp>state @ execute
  REPEAT
;


: gzp-end-inflate  ( gzp -- = Finish the inflation of data )
;


( Private deflate words )


( Deflate words )

: gzp-init-deflate ( gzp -- = Start the deflation of data )
;


: gzp-deflate      ( gzp -- n = Deflate data with result code n )
;


: gzp-end-deflate  ( gzp -- = Finish the deflation of data )
;


( Inspection )

: gzp-dump   ( gzp - = Dump the gzp )
  ." gzp:" dup . cr
    ."  input   :" dup gzp>input 2@ . . cr
    ."  hold    :" dup gzp>hold     ? cr
    ."  bits    :" dup gzp>bits     ? cr
    ."  state   :" dup gzp>state    ? cr
    ."  result  :" dup gzp>result   ? cr
    ."  output  :" dup gzp>output   lbf-dump cr
  drop
;
  
[THEN]

\ ==============================================================================
