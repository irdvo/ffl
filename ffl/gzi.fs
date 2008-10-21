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
\  $Date: 2008-10-21 17:27:50 $ $Revision: 1.3 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] gzi.version [IF]

include ffl/bis.fs
include ffl/lbf.fs


( gzi = GZip Input Base Module )
( The gzi module implements the base words for using the GZip inflate        )
( algoritme. The module is used for reading from a gzip file [zif] and       )
( stream [zis].                                                              )


1 constant gzi.version


( gzi constants )

0 constant gzi.ok            ( -- n = Decompression is finished okee )
1 constant gzi.done          ( -- n = Decompression is done )
2 constant gzi.more          ( -- n = Decompression step needs more data )

32768 constant gzi.out-size  ( -- n = Output buffer size )


( gzi structure )

begin-structure gzi%  ( -- n = Get the required space for a gzi variable )
  bis%
  +field  gzi>bis            \ the inflator extends the input buffer
  field:  gzi>state          \ the current state (as xt)
  lbf%
  +field  gzi>lbf            \ the output buffer
  field:  gzi>last           \ is this the last block ?
  \ field:  gzi>result         \ the result of the conversion
  \ crc?
end-structure


( GZip inflation variable creation, initialisation and destruction )

: gzi-init         ( gzi -- = Initialise the GZip inflation variable )
  >r
  r@  gzi>bis    bis-init
  r@  gzi>state  nil!
  r@  gzi>last   off

  1 chars gzi.out-size 
  r@  gzi>lbf    lbf-init

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

: gzi-do-stored    ( gzi -- ior = Process uncompressed data )
  drop gzi.done
;


: gzi-do-type      ( gzi -- ior = Check last block and inflation type )
  >r
  r@ gzi>last @ IF
    gzi.done                 \ Return to caller
  ELSE
    3 r@ bis-need-bits IF
      1 r@ bis-fetch-bits  \ Fetch last indicator and save it 
      0<> r@ gzi>last !
      1 r@ bis-next-bits   \ Last indicator processed

      2 r@ bis-fetch-bits  \ Fetch block type
      CASE
        0 OF ['] gzi-do-stored r@ gzi-state!  gzi.ok ENDOF
        1 OF ['] gzi-do-stored r@ gzi-state!  gzi.ok ENDOF
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
