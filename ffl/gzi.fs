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
\  $Date: 2009-05-03 19:33:15 $ $Revision: 1.8 $
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


( private huffman structure )

begin-structure gzi%hfm%  ( -- n = Get the required space for a huffman structure )
  field:  gzi>hfm>number       \ number of symbols in table
  field:  gzi>hfm>lengths      \ the bit length per symbol
  gzi.max-bits 1+
  fields: gzi>hfm>counts       \ the number of symbols per bit length
  gzi.max-bits 1+
  fields: gzi>hfm>offsets      \ the symbol offset per bit length
  field:  gzi>hfm>symbols      \ the symbol index, ordered by its bit length
end-structure

: gzi-hfm-init  ( n hfm -- = Initialise the huffman structure for n symbols )
  >r
  dup r@ gzi>hfm>number !      \ save the number of symbols
  cells
  dup allocate throw
  r@ gzi>hfm>lengths !         \ allocate the lengths array
  allocate throw
  r@ gzi>hfm>symbols !         \ allocate the symbols array
  r> gzi>hfm>counts
  gzi.max-bits 1+ cells erase  \ erase the counts array
;

: gzi-hfm-new  ( n -- hfm = Create a new huffman structure on the heap with n symbols )
  gzi%hfm% allocate throw  
  tuck gzi-hfm-init
;

: gzi-hfm-(free)  ( hfm -- = Free the internal, private variables from the heap )
  dup gzi>hfm>lengths @ 
  free throw
  gzi>hfm>symbols @
  free throw
;

: gzi-hfm-free  ( hfm -- = Free the huffman structure from the heap )
  dup gzi-hfm-(free)
  free throw
;

: gzi-hfm-set  ( u1 u2 hfm -- = Set symbol u2 with bit length u1 in the huffman structure )
  >r
  cells r@ gzi>hfm>lengths @ +   \ lengths[symbol] = length
  over swap !
  cells r> gzi>hfm>counts + 1+!  \ counts[length]++
;

: gzi-hfm-construct  ( hfm -- = Construct the huffman structure )
  trace" >construct"
  \ Check for no codes
  dup gzi>hfm>counts @ over gzi>hfm>number @ = IF \ counts[0] == n ?
    drop 0 EXIT
  THEN
  >r
    
  \ Check for over-subscribed or incomplete set
  1                            \ left = 1
  r@ gzi>hfm>counts cell+ gzi.max-bits cells bounds DO \ For counts[1]..counts[maxbits]
    1 lshift                   \ left <<= 1
    I @ -                      \ left -= counts[i], if < 0, then over subscribed
    dup 0< IF                  
      LEAVE
    THEN
  cell +LOOP                   \ S: n:left
  
  dup 0< 0= IF                 \ if left >= 0
    \ Generate offsets for each bit length
    r@ gzi>hfm>offsets cell+   \ offsets[1]
    dup 0!
    r@ gzi>hfm>counts cell+ gzi.max-bits 1- cells bounds DO \ For counts[1]..counts[maxbits-1]
      dup cell+
      swap @
      I @ +
      over !
    cell +LOOP
    drop
    
    \ Generate indices for each symbol, sorted within each bit length
    r@ gzi>hfm>number @  0
    BEGIN
      2dup >                   \ For symbol = 0 .. n-1
    WHILE
      r@ gzi>hfm>lengths @ over cells + @ \ lengths[s]
      ?dup IF
       cells r@ gzi>hfm>offsets + \ offsets[lengths[s]]
       dup @                  \ o = offsets[length[s]]
       swap 1+!               \ offsets[length[s]]++
       cells r@ gzi>hfm>symbols @ + \ symbols[o]
       over swap !            \ symbols[o] = s
      THEN
      1+
    REPEAT
    2drop
  THEN
  trace" <construct"
  rdrop
;


( gzi structure )

begin-structure gzi%  ( -- n = Get the required space for a gzi variable )
  bis%
  +field  gzi>bis              \ the inflator extends the input buffer
  field:  gzi>state            \ the current state (as xt)
  lbf%
  +field  gzi>lbf              \ the output buffer
  
  field:  gzi>last             \ is this the last block ?
  field:  gzi>length           \ the length of a block
  
  field:  gzi>fixed-symbols    \ the fixed symbols huffman table
  field:  gzi>fixed-distances  \ the fixed distance huffman table
  
  field:  gzi>symbols          \ the symbols huffman table
  field:  gzi>distances        \ the distance huffman table
  
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

  r@ gzi>fixed-symbols   nil!
  r@ gzi>fixed-distances nil!
  
  r@ gzi>symbols   nil!
  r@ gzi>distances nil!
  
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
  r@ gzi>fixed-symbols @ nil= IF
    \ allocate the fixed symbols
    288 gzi-hfm-new
    dup r@ gzi>fixed-symbols !
    \ fill the fixed symbols with symbols
    144 0   DO 8 over I swap gzi-hfm-set LOOP
    256 144 DO 9 over I swap gzi-hfm-set LOOP
    280 256 DO 7 over I swap gzi-hfm-set LOOP
    288 280 DO 8 over I swap gzi-hfm-set LOOP
    \ construct the fixed symbols
    gzi-hfm-construct drop
    
    \ allocate the fixed distances
    30 gzi-hfm-new
    dup r@ gzi>fixed-distances !
    \ fill the fixed distances with symbols
    30 0 DO 5 over I swap gzi-hfm-set LOOP
    \ construct the distances
    gzi-hfm-construct drop
  THEN
  r@ gzi>fixed-symbols   @ r@ gzi>symbols   !  \ Use the fixed huffman tables
  r@ gzi>fixed-distances @ r@ gzi>distances !
  rdrop
  \ XXX setup state for decode
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
