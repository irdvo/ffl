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
\  $Date: 2008-10-14 17:18:51 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] gzi.version [IF]

include ffl/bis.fs
\ include ffl/lbf.fs


( gzi = GZip Input Base Module )
( The gzi module implements the base words for using the GZip inflate        )
( algoritme. The module is used for reading from a gzip file [zif] and       )
( stream [zis].                                                              )


1 constant gzi.version


( gzi constants )

0 constant gzi.ok            ( -- n = Decompression is finished okee )
1 constant gzi.done          ( -- n = Decompression is done )
2 constant gzi.more          ( -- n = Decompression step needs more data )


( gzi structure )

begin-structure gzi%  ( -- n = Get the required space for a gzi variable )
  bis%
  +field  gzi>bis            \ the inflator extends the input buffer
  field:  gzi>state          \ the current state (as xt)
  \ field:  gzi>result         \ the result of the conversion
  \ crc?
  \ lbf%
  \ +field  gzi>output         \ the output buffer
end-structure


( GZip inflation variable creation, initialisation and destruction )

: gzi-init         ( gzi -- = Initialise the GZip inflation variable )
  dup  gzi>bis      bis-init
  dup  gzi>state    nil!
  \ dup  gzi>result   0!
  \ 1 chars over
  \ 32768   swap 
  \     gzi>output lbf-init   \ initialise the output buffers for chars and size 32k byte
  drop
\ ToDo
;


: gzi-(free)       ( gzi -- = Free the internal, private variables from the heap )
  \ dup gzi>output  lbf-(free)
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
  3 over gzi>bis bis-need-bits IF
    ['] gzi-do-stored swap gzi-state!
    gzi.ok
  ELSE
    drop
    gzi.more
  THEN 
;



( Inflate words )

: gzi-init-inflate ( gzi -- = Start the inflation of data )
  ['] gzi-do-type over gzi-state!
  
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
    ."  input   :" dup gzi>bis      bis-dump
    ."  state   :" dup gzi>state    ? cr
  drop
;
  
[THEN]

\ ==============================================================================
