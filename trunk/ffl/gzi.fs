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
\  $Date: 2009-05-28 17:35:58 $ $Revision: 1.23 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] gzi.version [IF]

include ffl/bis.fs
include ffl/lbf.fs
include ffl/enm.fs


( gzi = gzip Input Base Module )
( The gzi module implements the gzip inflate algorithm. The module is used   )
( for reading from a gzip file [zif] and, in a future version, stream [zis]. )


1 constant gzi.version


( gzi constants )

begin-enumeration
  enum: gzi.ok       ( -- n = Decompression step is okee )
  enum: gzi.done     ( -- n = Decompression is done )
  enum: gzi.more     ( -- n = Decompression step needs more data )
  enum: gzi.states   ( -- n = Decompression step states )
end-enumeration


( private gzi constants and tables )

320 constant gzi.max-codes  ( -- n = Maximum number of codes )
15  constant gzi.max-bits   ( -- n = Maximum number of bits  )
gzi.max-bits 1+
    constant gzi.max-bits+1 ( -- n = Maximum number of bits + 1 )


: gzi.table        ( "<spaces>name" -- ; u1 -- u2 = Create a cell based lookup table )
  create
  does>
    swap cells + @
;

gzi.table gzi.length-offsets  ( u1 -- u2 = Table with the offsets for the length codes 257..285 )
    3 ,  4 ,  5 ,  6 ,  7 ,  8 ,  9 , 10 ,  11 ,  13 ,  15 ,  17 ,  19 ,  23 , 27 , 
   31 , 35 , 43 , 51 , 59 , 67 , 83 , 99 , 115 , 131 , 163 , 195 , 227 , 258 ,

gzi.table gzi.length-extras   ( u1 -- u2 = Table with the extra bits for the length codes 257..185 )
    0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 1 , 1 , 1 , 1 , 2 , 2 , 2 , 
    2 , 3 , 3 , 3 , 3 , 4 , 4 , 4 , 4 , 5 , 5 , 5 , 5 , 0 , 

gzi.table gzi.distance-offsets (  u1 -- u2 = Table with the offsets for the distance codes 0..29 )
      1 ,   2 ,   3 ,   4 ,   5 ,    7 ,    9 ,   13 ,   17 ,   25 ,   33 ,    49 ,    65 ,    97 ,   129 , 
    193 , 257 , 385 , 513 , 769 , 1025 , 1537 , 2049 , 3073 , 4097 , 6145 ,  8193 , 12289 , 16385 , 24577 ,

gzi.table gzi.distance-extras
    0 , 0 , 0 , 0 , 1 , 1 , 2 ,  2 ,  3 ,  3 ,  4 ,  4 ,  5 ,  5 ,  6 , 
    6 , 7 , 7 , 8 , 8 , 9 , 9 , 10 , 10 , 11 , 11 , 12 , 12 , 13 , 13 ,

gzi.table gzi.code-orders
    16 , 17 , 18 , 0 , 8 , 7 , 9 , 6 , 10 , 5 , 11 , 4 , 12 , 3 , 13 , 2 , 14 , 1 , 15 ,


( private huffman structure )

begin-structure gzi%hfm%  ( -- n = Get the required space for a huffman structure )
  field:  gzi>hfm>number       \ number of symbols in table

  field:  gzi>hfm>lengths      \ pointer to array with the bit length per symbol
  gzi.max-bits+1
  fields: gzi>hfm>offsets      \ array with the offsets per bit length
  
  gzi.max-bits+1
  fields: gzi>hfm>counts       \ array with the number of symbols per bit length
  field:  gzi>hfm>symbols      \ array with the symbol index, ordered by its bit length
  
  field:  gzi>hfm>index        \ index in symbols during iterating
  field:  gzi>hfm>first        \ first code in symbols during iterating
  field:  gzi>hfm>count        \ pointer in counts during iterating
end-structure


: gzi-hfm-init  ( a-addr u hfm -- n = Initialise the huffman structure for u symbols and bit lengths a-addr, return completeness n )
  >r
  dup r@ gzi>hfm>number !      \ save the number of symbols
  cells allocate throw
  r@ gzi>hfm>symbols !         \ allocate the symbols array
  r@ gzi>hfm>lengths !
  r@ gzi>hfm>counts gzi.max-bits+1 cells erase   \ erase the counts array
  
  r>
  dup gzi>hfm>lengths @ over gzi>hfm>number @ cells bounds DO
    I @ cells
    over gzi>hfm>counts + 1+!  \ count the bit lengths
  cell +LOOP
  
  dup gzi>hfm>counts @ over gzi>hfm>number @ = IF \ Check for no codes: counts[0] == n ?
    drop 0 EXIT
  THEN
  >r
    
  1                            \ Check for over-subscribed or incomplete set, left = 1
  r@ gzi>hfm>counts cell+ gzi.max-bits cells bounds DO \ For counts[1]..counts[maxbits]
    1 lshift                   \ left <<= 1
    I @ -                      \ left -= counts[i], if < 0, then over subscribed
    dup 0< IF                  
      LEAVE
    THEN
  cell +LOOP                   \ S: n:left
  
  dup 0< 0= IF                 \ if left >= 0
    r@ gzi>hfm>offsets cell+   \ Generate offsets for each bit length, start with offsets[1]
    dup 0!
    r@ gzi>hfm>counts cell+ gzi.max-bits 1- cells bounds DO \ For counts[1]..counts[maxbits-1]
      dup cell+
      swap @
      I @ +
      over !
    cell +LOOP
    drop
    
    r@ gzi>hfm>number @  0     \ Generate indices for each symbol, sorted within each bit length
    BEGIN
      2dup >                   \ For symbol = 0 .. n-1
    WHILE
      r@ gzi>hfm>lengths @ over cells + @  \ lengths[s]
      ?dup IF
       cells r@ gzi>hfm>offsets +  \ offsets[lengths[s]]
       dup @                   \ o = offsets[length[s]]
       swap 1+!                \ offsets[length[s]]++
       cells r@ gzi>hfm>symbols @ +  \ symbols[o]
       over swap !             \ symbols[o] = s
      THEN
      1+
    REPEAT
    2drop
  THEN
  r> gzi>hfm>lengths nil!
;


: gzi-hfm-new  ( a-addr u -- n hfm = Create a new huffman structure on the heap with u symbols and bit lengths a-addr, return completeness n )
  gzi%hfm% allocate throw  >r r@ gzi-hfm-init r>
;


: gzi-hfm-(free)  ( hfm -- = Free the internal, private variables from the heap )
  gzi>hfm>symbols @ free throw
;


: gzi-hfm-free  ( hfm -- = Free the huffman structure from the heap )
  dup gzi-hfm-(free)
  free throw
;


( private huffman structure iterating words )

: gzi-hfm-start    ( hfm -- = Start iterating the huffman structure with bit length 1 )
  dup gzi>hfm>index 0!
  dup gzi>hfm>first 0!
  
  dup  gzi>hfm>counts cell+    \ start with bit length 1
  swap gzi>hfm>count !
;


: gzi-hfm-code>symbol  ( u1 hfm -- false | u2 true = Check if code u1 is valid for current bit length, if so return the symbol, else move iterator to next bit length )
  >r
  r@ gzi>hfm>count @ @
  2dup r@ gzi>hfm>first @ + < IF   \ if code < first + [count] then
    drop
    r@ gzi>hfm>first @ -
    r@ gzi>hfm>index @ + cells
    r@ gzi>hfm>symbols @ + @       \  return symbols[code - first + index]
    true
  ELSE                             \ else
    nip
    dup r@ gzi>hfm>index +!        \   index += [count]
    r@ gzi>hfm>first @             \   first  = (first + [count]) << 1
    + 1 lshift
    r@ gzi>hfm>first !
    cell r@ gzi>hfm>count +!       \   count++
    false
  THEN
  rdrop
;


( private huffman table dump )

: gzi-hfm-dump     ( hfm -- = Dump the huffman structure )
  ." gzi-hfm:" dup . cr
    ."  number  :" dup gzi>hfm>number ? cr
    ."  lengths :" dup gzi>hfm>lengths ? cr
    ."  counts  :" dup gzi>hfm>counts  gzi.max-bits+1 cells bounds DO I ? cell +LOOP cr
    ."  offsets :" dup gzi>hfm>offsets gzi.max-bits+1 cells bounds DO I ? cell +LOOP cr
    ."  symbols :" dup gzi>hfm>symbols @ over gzi>hfm>number @ cells bounds ?DO I ? cell +LOOP cr
    ."  index   :" dup gzi>hfm>index ? cr
    ."  first   :" dup gzi>hfm>first ? cr
    ."  count   :"     gzi>hfm>count ? cr
;


( gzi structure )

begin-structure gzi%  ( -- n = Get the required space for a gzi variable )
  bis%
  +field  gzi>bis              \ the inflater extends the input buffer
  field:  gzi>state            \ the current state (as xt)
  field:  gzi>lbf-size         \ the initial size of the output buffer
  lbf%
  +field  gzi>lbf              \ the output buffer
  
  field:  gzi>last-block       \ is this the last block ?
  field:  gzi>block-length     \ the length of a block

  gzi.max-codes
  fields: gzi>lengths          \ the array with the bit lengths for the symbols

  field:  gzi>hfm-fixed-symbols    \ the fixed symbols huffman table
  field:  gzi>hfm-fixed-distances  \ the fixed distance huffman table
  
  field:  gzi>hfm-symbols      \ the symbols huffman table
  field:  gzi>hfm-distances    \ the distance huffman table
  
  field:  gzi>code             \ the current code
  field:  gzi>code-length      \ the current code length
  
  field:  gzi>copy-length      \ the copy length
  field:  gzi>copy-distance    \ the copy distance
  
  field:  gzi>length-codes     \ the number of literal/length codes
  field:  gzi>distance-codes   \ the number of distance codes
  field:  gzi>code-codes       \ the number of code length codes
  field:  gzi>length+distance-codes \ the number of length and distance codes
  
  field:  gzi>hfm-code-codes   \ the code lengths for the code length huffman table
  
  field:  gzi>index            \ index during building lengths

  field:  gzi>repeat-length    \ the bit length value to be repeated
  field:  gzi>repeat-bits      \ the number of repeat bits to be read
  field:  gzi>repeat-times     \ the number of times the bit length must be repeated
end-structure


( Private free words )

: gzi-free-symbols?  ( gzi -- flag = Should the symbols huffman table be freed ? )
  dup gzi>hfm-symbols @ dup
  rot gzi>hfm-fixed-symbols @ <>   \ the symbols table should not be nil or equal to the fixed table
  swap nil<> AND
;


: gzi-free-distances?  ( gzi -- flag = Should the distances huffman table be freed ? )
  dup gzi>hfm-distances @ dup
  rot gzi>hfm-fixed-distances @ <> \ the distances table should not be nil or equal to the fixed table
  swap nil<> AND
;


( gzip inflation variable creation, initialisation and destruction )

: gzi-init         ( u gzi -- = Initialise the gzip inflation variable with an initial output buffer size u )
  >r
  r@  gzi>bis          bis-init
  r@  gzi>state        nil!
  r@  gzi>last-block   off
  r@  gzi>block-length 0!
  69632 max dup
  r@  gzi>lbf-size     !            \ output buffer size, minimal 64K + 4k

  1 chars swap
  r@ gzi>lbf           lbf-init

  ['] c! ['] c@ 
  r@ gzi>lbf           lbf-access!
  
  r@ gzi>hfm-fixed-symbols   nil!
  r@ gzi>hfm-fixed-distances nil!
  
  r@ gzi>hfm-symbols   nil!
  r@ gzi>hfm-distances nil!
  
  r@ gzi>code        0!
  r@ gzi>code-length 0!

  r@ gzi>length-codes          0!
  r@ gzi>distance-codes        0!
  r@ gzi>code-codes            0!
  r@ gzi>length+distance-codes 0!

  r@ gzi>hfm-code-codes  nil!

  r@ gzi>repeat-length 0!
  r@ gzi>repeat-bits   0!
  r@ gzi>repeat-times  0!

  rdrop
;


: gzi-(free)       ( gzi -- = Free the internal, private variables from the heap )
  dup gzi>lbf    lbf-(free)
  
  dup gzi-free-symbols?   IF dup gzi>hfm-symbols   @ gzi-hfm-free THEN
  dup gzi-free-distances? IF dup gzi>hfm-distances @ gzi-hfm-free THEN
  
  dup gzi>hfm-fixed-symbols   @ nil<>? IF gzi-hfm-free THEN
  dup gzi>hfm-fixed-distances @ nil<>? IF gzi-hfm-free THEN
      gzi>hfm-code-codes      @ nil<>? IF gzi-hfm-free THEN
;


: gzi-create       ( "<spaces>name" u -- ; -- gzi = Create a named gzip inflation variable in the dictionary with an initial output buffer size u )
  create   here   gzi% allot   gzi-init
;


: gzi-new          ( -- gzi = Create a new gzip inflation variable on the heap )
  gzi% allocate  throw  tuck gzi-init
;


: gzi-free         ( gzi -- = Free the variable from the heap )
  dup gzi-(free)        \ Free the internal, private variables from the heap

  free throw            \ Free the variable
;


( Member words )

: gzi-lbf@         ( gzi -- lbf = Get the output buffer )
  gzi>lbf
;


( Private inflate words )

: gzi-state!       ( xt gzi -- = Set the current state )
  gzi>state !
;


: gzi-decode       ( hfm gzi -- u gzi.ok | ior = Decode the current code to a symbol )
  >r
  BEGIN
    r@ gzi>code-length @ gzi.max-bits+1 < IF  \ if not all bits done then
      r@ bis-get-bit IF                       \   get next bit, if available then
        r@ gzi>code @ 1 lshift OR             \     put bit in code and ..
        2dup swap gzi-hfm-code>symbol IF            \     convert to the symbol, if success then
          nip nip gzi.ok true                 \       done
        ELSE                                  \     else
          r@ gzi>code !                       \       save, try next bit
          r@ gzi>code-length 1+!
          false
        THEN
      ELSE
        drop gzi.more true                    \   if no more bits available, return for more
      THEN
    ELSE
      drop exp-wrong-file-data true           \ all bits tried, no symbol found, error
    THEN
  UNTIL
  rdrop
;


( Private inflate state words )

0 value gzi.do-type       ( -- xt = xt of gzi-do-type )
0 value gzi.do-codes      ( -- xt = xt of gzi-do-codes )
0 value gzi.do-code-table ( -- xt = xt of gzi-do-code-table )

: gzi-do-copy      ( gzi -- ior = Copy uncompressed data )
  >r
  r@ gzi>block-length @ ?dup IF
    r@ bis-get                         \ Get byte data from input stream
    rot min                            \ Limit with requested length and stream length
    tuck r@ gzi>lbf lbf-set            \ Copy data to the output stream
    dup  negate r@ gzi>block-length +! \ Update the requested length
    r@ bis-get rot /string r@ bis-set  \ Update the input stream
  THEN
  r@ gzi>block-length @ IF
    gzi.more
  ELSE
    gzi.do-type r@ gzi-state!
    gzi.ok
  THEN
  rdrop
;


: gzi-do-stored    ( gzi -- ior = Process uncompressed data )
  dup bis-bits>bytes
  4 over bis-read-bytes IF
    dup            [ hex ] FFFF [ decimal ] AND
    swap 16 rshift [ hex ] FFFF [ decimal ] XOR
    over = IF
      over gzi>block-length !
      ['] gzi-do-copy swap gzi-state!
      gzi.ok
    ELSE
      2drop exp-wrong-file-data
    THEN
  ELSE
    drop gzi.more
  THEN
;


: gzi-start-codes  ( gzi -- = Start decoding codes )
    dup  gzi>code 0!
  1 over gzi>code-length !
    dup  gzi>hfm-symbols @ gzi-hfm-start
  gzi.do-codes 
    swap gzi-state!
;


: gzi-do-distance-extra  ( gzi -- ior = Read the extra copy distance bits )
  dup gzi>code @ gzi.distance-extras      \ get extra length bits based on symbol
  over 2dup bis-need-bits IF              \ if extra bits in the buffer then
    2dup bis-fetch-bits
    over gzi>code @ gzi.distance-offsets +  \   copy-distance = distance-offsets[symbol] + extra bits
    over gzi>copy-distance !
    bis-next-bits                         \   set bits processed

    dup gzi>copy-length @
    over dup gzi>copy-distance @
    swap gzi>lbf lbf-copy                 \   copy from output buffer

    dup gzi-start-codes                   \   continue decoding the codes
    gzi.ok
  ELSE                                    \ else input buffer empty
    2drop gzi.more
  THEN
  nip
;


: gzi-do-distance  ( gzi -- ior = Decode the distance code )
  dup gzi>hfm-distances @ over gzi-decode
  dup gzi.ok = IF
    drop
    dup 30 < IF                          \       if valid distance code then
      dup gzi.distance-extras 0= IF      \         if no extra distance bits to read
        gzi.distance-offsets
        over gzi>copy-distance !
        
        dup gzi>copy-length @
        over dup gzi>copy-distance @
        swap gzi>lbf lbf-copy            \           copy from output buffer

        dup gzi-start-codes              \           continue decoding codes
        gzi.ok
      ELSE
        over gzi>code !                  \           save distance code for distance length
        ['] gzi-do-distance-extra over gzi-state! \  and read the extra bits
        gzi.ok
      THEN
    ELSE                                 \       else invalid file data
      drop exp-wrong-file-data
    THEN
  THEN
  nip
;


: gzi-start-distance  ( gzi -- = Start decoding distance )
    dup  gzi>code 0!
  1 over gzi>code-length !
    dup  gzi>hfm-distances @ gzi-hfm-start
  ['] gzi-do-distance
    swap gzi-state!
;


: gzi-do-length-extra  ( gzi -- ior = Read the extra copy length bits )
  dup gzi>code @ gzi.length-extras        \ get extra length bits based on symbol
  over 2dup bis-need-bits IF              \ if extra bits in the buffer then
    2dup bis-fetch-bits
    over gzi>code @ gzi.length-offsets +  \   copy-length = length-offsets[symbol] + extra bits
    over gzi>copy-length !
    bis-next-bits                         \   set bits processed

    dup gzi-start-distance                \   start decoding the distance
    gzi.ok
  ELSE                                    \ else input buffer empty
    2drop gzi.more
  THEN
  nip
;


: gzi-do-codes  ( gzi -- ior = Inflate the codes )
  BEGIN
    dup gzi>hfm-symbols @ over gzi-decode
    dup gzi.ok = IF
      drop
      dup 256 < IF                         \       if normal character then
        over gzi>lbf lbf-enqueue           \         put symbol in buffer
        dup gzi-start-codes                \         setup for next code
        false
      ELSE 
        dup 256 = IF                       \       else if end-of-block then
          drop
          gzi.do-type over gzi-state!
          gzi.ok true
        ELSE                               \        else copy length code
          257 -
          dup 28 > IF
            drop exp-wrong-file-data true
          ELSE
            dup gzi.length-extras 0= IF    \          if no extra length bits to read then
              gzi.length-offsets
              over gzi>copy-length !
              dup gzi-start-distance       \            start decoding distance
            ELSE
              over gzi>code !              \          else save symbol for copy length
              ['] gzi-do-length-extra over gzi-state! \  and read the extra bits
            THEN
            gzi.ok true
          THEN
        THEN
      THEN
    ELSE
      true
    THEN
  UNTIL
  nip
;
' gzi-do-codes to gzi.do-codes


: gzi-do-fixed     ( gzi -- ior = Process data with a fixed table )
  >r
  r@ gzi>hfm-fixed-symbols @ nil= IF

    r@ gzi>lengths
    144 0   DO 8 over I cells + ! LOOP  \ fill the length for the fixed symbols
    256 144 DO 9 over I cells + ! LOOP
    280 256 DO 7 over I cells + ! LOOP
    288 280 DO 8 over I cells + ! LOOP  \ S: lengths
    
    288 gzi-hfm-new                     \ create the huffman table for the fixed symbols
    r@ gzi>hfm-fixed-symbols ! drop

    r@ gzi>lengths
    30 0 DO 5 over I cells + ! LOOP     \ fill the fixed distances with symbols
    
    30 gzi-hfm-new                      \ create the huffman table for fixed distances
    r@ gzi>hfm-fixed-distances ! drop
  THEN
  r@ gzi-free-symbols?   IF r@ gzi>hfm-symbols   @ gzi-hfm-free THEN
  r@ gzi-free-distances? IF r@ gzi>hfm-distances @ gzi-hfm-free THEN
  
  r@ gzi>hfm-fixed-symbols   @ r@ gzi>hfm-symbols   !  \ Use the fixed huffman tables
  r@ gzi>hfm-fixed-distances @ r@ gzi>hfm-distances !

  r@ gzi-start-codes                           \ Setup for decode codes

  ['] gzi-do-codes r> gzi-state!
  gzi.ok
;


: gzi-start-code-table  ( gzi -- = Start using the code lengths codes table )
    dup  gzi>code 0!
  1 over gzi>code-length !
    dup  gzi>hfm-code-codes @ gzi-hfm-start
  gzi.do-code-table
    swap gzi-state!
;


: gzi-construct-dynamic  ( gzi -- ior = Construct the dynamic huffman tables )
  dup gzi>lengths over gzi>length-codes @ gzi-hfm-new swap
  dup 0< IF
    drop gzi-hfm-free drop
    exp-wrong-file-data
    EXIT
  ELSE 0> IF
    over gzi>length-codes @ over gzi>hfm>counts @ - 1- IF
      gzi-hfm-free drop
      exp-wrong-file-data
      EXIT
      THEN
    THEN
  THEN
  
  over gzi-free-symbols? IF over gzi>hfm-symbols @ gzi-hfm-free THEN

  over gzi>hfm-symbols !
  
  dup gzi>lengths over gzi>length-codes @ cells + over gzi>distance-codes @ gzi-hfm-new swap
  dup 0< IF
    drop gzi-hfm-free drop
    exp-wrong-file-data
    EXIT
  ELSE 0> IF
    over gzi>distance-codes @ over gzi>hfm>counts @ - 1- IF
      gzi-hfm-free drop
      exp-wrong-file-data
      EXIT
      THEN
    THEN
  THEN
  
  over gzi-free-distances? IF over gzi>hfm-distances @ gzi-hfm-free THEN
  
  swap gzi>hfm-distances !
  gzi.ok
;


: gzi-do-repeat-times  ( gzi -- ior = Read the repeat times and perform the bit length copy )
  dup gzi>repeat-bits @ over
  2dup bis-need-bits IF             \ Read the extra repeat times bits
    2dup bis-fetch-bits
    over gzi>repeat-times +!
         bis-next-bits
    
    >r
    r@ gzi>repeat-length @          \ Copy the repeat length, repeat times in the lengths array
    r@ gzi>lengths
    r@ gzi>index @ cells +
    r@ gzi>repeat-times @ cells bounds ?DO
      dup I !
    cell +LOOP
    drop
    r>
    
    dup gzi>index @  over gzi>repeat-times @ +
    2dup swap gzi>length+distance-codes @ < IF  \ If not all bit lengths decoded then
      over gzi>index !
      dup  gzi-start-code-table        \   Continue decoding
      gzi.ok
    ELSE                               \  Else
      drop
      dup gzi-construct-dynamic        \    Construct the dynamic huffman tables and ..
      dup gzi.ok = IF
        over gzi-start-codes           \    .. start decoding the data
      THEN
    THEN
  ELSE
    2drop
    gzi.more
  THEN
  nip
;


: gzi-do-code-table  ( gzi -- ior = Decode the length/literal table )
  BEGIN
    dup gzi>hfm-code-codes @ over gzi-decode
    dup gzi.ok = IF
      drop
      dup 16 < IF                           \ normal bit length 0..15: store in lengths array
        2dup swap gzi>repeat-length !
        over 
        dup  gzi>lengths
        swap gzi>index @
        dup 1+ >r
        cells + !
        r>
        2dup swap gzi>length+distance-codes @ < IF  \ if not all codes read, then continue
          over gzi>index  !
          dup    gzi>code  0!
          1 over gzi>code-length !
          dup  gzi>hfm-code-codes @ gzi-hfm-start
          false
        ELSE                               \ else construct the dynamic huffman tables and ..
          drop
          dup gzi-construct-dynamic
          dup gzi.ok = IF
            over gzi-start-codes           \ .. start decoding the data
          THEN
          true
        THEN
      ELSE                                 \ repeat length coding
        dup 16 = IF
          drop
          2 over gzi>repeat-bits  !
          3 over gzi>repeat-times !
        ELSE 
          17 = IF
            3 over gzi>repeat-bits  !
            3 over gzi>repeat-times !
              dup  gzi>repeat-length 0!
          ELSE
            7  over gzi>repeat-bits  !
            11 over gzi>repeat-times !
               dup  gzi>repeat-length 0!
          THEN 
        THEN
        ['] gzi-do-repeat-times over gzi-state!
        gzi.ok true
      THEN
    ELSE
      true
    THEN
  UNTIL
  nip
;
' gzi-do-code-table to gzi.do-code-table


: gzi-do-code-codes ( gzi -- ior = Read the code length code lengths )
  BEGIN
    dup gzi>index @ over gzi>code-codes @ < IF  \ if not all lengths read then
      3 over bis-need-bits IF
        3 over bis-fetch-bits                   \   read the length and store in lengths array
        over dup gzi>index @ gzi.code-orders cells swap gzi>lengths + !
        3 over bis-next-bits
        dup gzi>index 1+!
        false
      ELSE
        gzi.more true
      THEN
    ELSE
      dup gzi>lengths
      over 19 swap gzi>index @ ?DO              \ if all lengths read than fill out with zero's and
        dup I gzi.code-orders cells + 0!
      LOOP
      19 gzi-hfm-new swap IF                    \ create the huffman table with the lengths
        gzi-hfm-free
        exp-wrong-file-data true
      ELSE
        over gzi>hfm-code-codes @! nil<>? IF gzi-hfm-free THEN \ Free previous table
        dup  gzi>index 0!                       \ Setup for next state
        dup  gzi-start-code-table
        gzi.ok true
      THEN
    THEN
  UNTIL
  nip
;


: gzi-do-table     ( gzi -- ior = Start processing the dynamic table by reading the table lengths )
  14 over bis-need-bits IF
    5 over bis-fetch-bits         \ Read number of literal/length codes
    257 +
    dup 286 > IF
      drop exp-wrong-file-data
    ELSE
      2dup swap gzi>length-codes !
      over gzi>length+distance-codes !
      5 over bis-next-bits
    
      5 over bis-fetch-bits       \ Read number of distance codes
      1+
      dup 30 > IF
        drop exp-wrong-file-data
      ELSE
        2dup swap gzi>distance-codes !
        over gzi>length+distance-codes +!
        5 over bis-next-bits
    
        4 over bis-fetch-bits     \ Read number of code length codes
        4 +
        over gzi>code-codes !
        4 over bis-next-bits
    
        dup gzi>index 0!           \ Setup next step
        ['] gzi-do-code-codes over gzi-state!
        gzi.ok
      THEN
    THEN
  ELSE
    gzi.more
  THEN
  nip
;


: gzi-do-type      ( gzi -- ior = Check last block and inflation type )
  >r
  r@ gzi>last-block @ IF
    gzi.done                 \ Return to caller
  ELSE
    3 r@ bis-need-bits IF
      1 r@ bis-fetch-bits  \ Fetch last indicator and save it 
      0<> r@ gzi>last-block !
      1 r@ bis-next-bits   \ Last indicator processed

      2 r@ bis-fetch-bits  \ Fetch block type
      CASE
        0 OF ['] gzi-do-stored r@ gzi-state!  gzi.ok ENDOF
        1 OF ['] gzi-do-fixed  r@ gzi-state!  gzi.ok ENDOF
        2 OF ['] gzi-do-table  r@ gzi-state!  gzi.ok ENDOF
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
 
  bis-bytes>bits       \ Start reading bits
;


: gzi-inflate      ( gzi -- ior = Do the next step in inflating data, return the result code )
  dup gzi>state @ execute
;


: gzi-reduce-output ( gzi -- = Check if the output buffer can be reduced )
  dup  gzi>lbf-size @ 4096 -
  swap gzi>lbf
  dup lbf-gap@ rot over < IF    \ If gap between out and out' > initial-size - 4k Then
    32768 -
    swap 2dup lbf-skip drop lbf-reduce  \   Remove processed data and reduce buffer
  ELSE
    2drop
  THEN
;


: gzi-end-inflate  ( gzi -- = Finish the inflation of data )
  drop
;


( Inspection )

: gzi-dump   ( gzi -- = Dump the gzi )
  ." gzi:" dup . cr
    ."  bis                   : " dup gzi>bis      bis-dump
    ."  state                 : " dup gzi>state    ? cr
    ."  lbf-size              : " dup gzi>lbf-size ? cr
    ."  lbf                   : " dup gzi>lbf      lbf-dump
    ."  last-block            : " dup gzi>last-block ? cr
    ."  block-length          : " dup gzi>block-length ? cr
    ."  lengths               : " dup gzi>lengths gzi.max-codes cells bounds DO I ? cell +LOOP cr
    ."  hfm-fixed-symbols     : "   dup gzi>hfm-fixed-symbols @   dup nil<> IF gzi-hfm-dump ELSE . cr THEN
    ."  hfm-fixed-distances   : " dup gzi>hfm-fixed-distances @ dup nil<> IF gzi-hfm-dump ELSE . cr THEN
    ."  hfm-symbols           : "   dup gzi>hfm-symbols @   dup nil<> IF gzi-hfm-dump ELSE . cr THEN
    ."  hfm-distances         : " dup gzi>hfm-distances @ dup nil<> IF gzi-hfm-dump ELSE . cr THEN
    ."  code                  : " dup gzi>code ? cr
    ."  code-length           : " dup gzi>code-length ? cr
    ."  copy-length           : " dup gzi>copy-length ? cr
    ."  copy-distance         : " dup gzi>copy-distance ? cr
    ."  length-codes          : " dup gzi>length-codes ? cr
    ."  distance-codes        : " dup gzi>distance-codes ? cr
    ."  code-codes            : " dup gzi>code-codes ? cr
    ."  length+distance-codes : " dup gzi>length+distance-codes ? cr
    ."  hfm-code-codes        : " dup gzi>hfm-code-codes @ dup nil<> IF gzi-hfm-dump ELSE . cr THEN
    ."  index                 : " dup gzi>index ? cr
    ."  repeat-length         : " dup gzi>repeat-length ? cr
    ."  repeat-bits           : " dup gzi>repeat-bits ? cr
    ."  repeat-times          : "     gzi>repeat-times ? cr
;

[THEN]

\ ==============================================================================
