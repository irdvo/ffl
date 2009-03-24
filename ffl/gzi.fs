\ ==============================================================================
\
\            gzi - the gzip inflate base module in the ffl
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
\  $Date: 2009-03-24 18:24:03 $ $Revision: 1.7 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] gzi.version [IF]

\ -- ToDo Remove
include ffl/log.fs

log-to-console
7 log-stack
false log-time&date
\ ----

include ffl/bis.fs
include ffl/lbf.fs


( gzi = GZip Input Base Module )
( The gzi module implements the base words for using the GZip inflate        )
( algoritme. The module is used for reading from a gzip file [zif] and       )
( stream [zis].                                                              )


1 constant gzi.version


( gzi constants )

begin-enumeration
  enum: gzi.ok       ( -- n = Decompression step is okee )
  enum: gzi.done     ( -- n = Decompression is done )
  enum: gzi.more     ( -- n = Decompression step needs more data )
  enum: gzi.states   ( -- n = Decompression step states )
end-enumeration


32768 constant gzi.out-size  ( -- n = Output buffer size )
320   constant gzi.max-codes ( -- n = Maximum number of codes )
15    constant gzi.max-bits  ( -- n = Maximum number of bits  )


( private structure )

begin-structure gzi%hfm%  ( -- n = Get the required space for a huffman structure )
  field:  gzi>hfm>syms         \ number of symbols per bit length
  field:  gzi>hfm>offs         \ the offsets per symbol
end-structure


( gzi structure )

begin-structure gzi%  ( -- n = Get the required space for a gzi variable )
  bis%
  +field  gzi>bis              \ the inflator extends the input buffer
  field:  gzi>state            \ the current state (as xt)
  lbf%
  +field  gzi>lbf              \ the output buffer
  
  field:  gzi>last             \ is this the last block ?
  field:  gzi>length           \ the length of a block
  
  gzi%hfm%
  +field  gzi>fix-syms         \ the fixed symbols huffman table
  gzi%hfm%
  +field  gzi>fix-dsts         \ the fixed distance huffman table

  gzi.max-codes
  fields: gzi>lengths          \ the temporary array for the lengths for the symbols, distances
  gzi.max-bits 1+
  fields: gzi>offsets          \ the temporary array for the offsets in the symbol table for each length
  
  field:  gzi>syms             \ the symbols huffman table
  field:  gzi>dsts             \ the distance huffman table
  
  \ field:  gzi>result         \ the result of the conversion
  \ crc?
end-structure


( GZip inflation variable creation, initialisation and destruction )

: gzi-init         ( gzi -- = Initialise the GZip inflation variable )
  >r
  r@  gzi>bis    bis-init
  r@  gzi>state  nil!
  r@  gzi>last   off
  r@  gzi>length 0!

  1 chars gzi.out-size 
  r@  gzi>lbf    lbf-init

  r@ gzi>fix-syms gzi>hfm>syms nil!
  r@ gzi>fix-syms gzi>hfm>offs nil!
  r@ gzi>fix-dsts gzi>hfm>syms nil!
  r@ gzi>fix-dsts gzi>hfm>offs nil!
  
  rdrop
\ ToDo
;


: gzi-(free)       ( gzi -- = Free the internal, private variables from the heap )
  dup gzi>lbf    lbf-(free)
  drop
  \ ToDo
;


: gzi-create       ( "<spaces>name" -- ; -- gzi = Create a named GZip inflation variable in the dictionary )
  create   here   gzi% allot   gzi-init
;


: gzi-new          ( -- gzi = Create a new GZip inflation variable on the heap )
  gzi% allocate  throw  dup gzi-init
;


: gzi-free         ( gzi -- = Free the variable from the heap )
  dup gzi-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the gzi
;


( Member words )

: gzi-state!       ( xt gzi -- = Set the current state )
  gzi>state !
;


( Private inflate words )

: gzi-construct ( gzi%hfm% n gzi -- n = Construct the huffman table with n length starting at gzi>lengths, return left in length set )
  >r
  over gzi.max-bits 1+ cells allocate throw swap gzi>hfm>syms !  \ Allocate the huffman arrays
  2dup                 cells allocate throw swap gzi>hfm>offs !
  
  over gzi>hfm>syms @   gzi.max-bits 1+ cells  erase   \ Clear symbols per length array

  over gzi>hfm>syms @
  over r@ gzi>lengths swap
  bounds DO                   \ For n symbols Do syms[length[symbol]]++
    dup I @ cells + 1+!
  cell +LOOP
                               \ S: hfm n syms
  2dup @ = IF rdrop drop 2drop 0 EXIT THEN  \ Check for no codes
  
  cell+
  1                            \ S: hfm n syms+1 left
  over gzi.max-bits cells      \ Check for over-subscribed or incomplete set of lengths
  bounds DO
    1 lshift                   \ left = (left << 1) - syms[length]
    I @ -
    dup 0< IF unloop rdrop >r drop 2drop r> EXIT THEN
  cell +LOOP
  
  -rot
  r@ gzi>offsets cell+
  dup 0!                       \ offsets[1] = 0
  swap gzi.max-bits 1- cells 
  bounds DO                   \ Do for all lengths
    I @ over @ +
    swap cell+ tuck !          \   offsets[length+1] = offsets[length] + syms[length]
  cell +LOOP
  drop
  
  >r swap gzi>hfm>offs @ r>
  r@ gzi>offsets swap
  r> gzi>lengths swap
  0 DO                       \ Do for all symbols
    dup 2over rot             \ 3dup
    I cells + @
    cells +
    dup @ swap 1+!
    cells + I swap !          \  offs[offsets[lengths[symbol]]++] = symbol
  LOOP
  2drop drop
;


( Private inflate state words )

0 value gzi.do-type  ( -- xt = Xt of gzi-do-type )


: gzi-do-copy      ( gzi -- ior = Copy uncompressed data )
  trace" do-copy"
  >r
  r@ gzi>length @ ?dup IF
    r@ bis-get                         \ Get byte data from input stream
    rot min                            \ Limit with requested length and stream length
    tuck r@ gzi>lbf lbf-set            \ Copy data to the output stream
    dup  negate r@ gzi>length +!       \ Update the requested length
    r@ bis-get rot /string r@ bis-set  \ Update the input stream
  THEN
  r@ gzi>length @ IF
    gzi.more
  ELSE
    gzi.do-type r@ gzi-state!
    gzi.ok
  THEN
  rdrop
;


: gzi-do-stored    ( gzi -- ior = Process uncompressed data )
  trace" do-stored"
  dup bis-bits>bytes
  4 over bis-read-bytes IF
    dup            [ hex ] FFFF [ decimal ] AND
    swap 16 rshift [ hex ] FFFF [ decimal ] XOR
    over = IF
      trace" Length:"
      over gzi>length !
      ['] gzi-do-copy swap gzi-state!
      gzi.ok
    ELSE
      2drop exp-wrong-file-data
    THEN
  ELSE
    drop gzi.more
  THEN
;


: gzi-do-fixed     ( gzi -- ior = Process data with a fixed table )
  trace" do-fixed"
  >r
  r@ gzi>fix-syms gzi>hfm>syms @ nil= IF
    \ the fixed symbols
    r@ gzi>lengths 0
    BEGIN dup 144 < WHILE swap dup 8 ! cell+ swap 1+ REPEAT
    BEGIN dup 256 < WHILE swap dup 9 ! cell+ swap 1+ REPEAT
    BEGIN dup 280 < WHILE swap dup 7 ! cell+ swap 1+ REPEAT
    BEGIN dup 288 < WHILE swap dup 8 ! cell+ swap 1+ REPEAT
    nip
    r@ gzi>fix-syms swap r@ gzi-construct
    \ the fixed distances
    r@ gzi>lengths 0
    BEGIN dup  30 < WHILE swap dup 5 ! cell+ swap 1+ REPEAT
    nip
    r@ gzi>fix-dsts swap r@ gzi-construct
  THEN
  r@ gzi>fix-syms r@ gzi>syms !    \ Use the fixed huffman tables
  r@ gzi>fix-dsts r@ gzi>dsts !
  rdrop
  \ ToDo: setup state for decode
;


: gzi-do-type      ( gzi -- ior = Check last block and inflation type )
  trace" do-type"
  >r
  r@ gzi>last @ IF
    gzi.done                 \ Return to caller
  ELSE
    3 r@ bis-need-bits IF
      1 r@ bis-fetch-bits  \ Fetch last indicator and save it 
      trace" LastBlock"
      0<> r@ gzi>last !
      1 r@ bis-next-bits   \ Last indicator processed

      2 r@ bis-fetch-bits  \ Fetch block type
      trace" BlockType"
      CASE
        0 OF ['] gzi-do-stored r@ gzi-state!  gzi.ok ENDOF
        1 OF ['] gzi-do-fixed  r@ gzi-state!  gzi.ok ENDOF
        2 OF ['] gzi-do-stored r@ gzi-state!  gzi.ok ENDOF
        exp-wrong-file-data swap
      ENDCASE
      2 r@ bis-next-bits
    ELSE
      gzi.more
    THEN
  THEN
  rdrop
;
' gzi-do-type to gzi.do-type


( Inflate words )

: gzi-init-inflate ( gzi -- = Start the inflation of data )
  ['] gzi-do-type over gzi-state!
 
  dup bis-bytes>bits         \ Start reading bits

  drop
  \ ToDo
;


: gzi-inflate      ( gzi -- ior = Do the next step in inflating data, return the result code )
  dup gzi>state @ execute
;


: gzi-end-inflate  ( gzi -- = Finish the inflation of data )
  drop \ ToDo
;


( Inspection )

: gzi-dump   ( gzi - = Dump the gzi )
  ." gzi:" dup . cr
    ."  bis   :" dup gzi>bis      bis-dump
    ."  state :" dup gzi>state    ? cr
    ."  lbf   :" dup gzi>lbf      lbf-dump
  drop
;
  
[THEN]

\ ==============================================================================
