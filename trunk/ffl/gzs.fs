\ ==============================================================================
\
\               gzs - the gzip stream module in the ffl
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
\  $Date: 2008-06-03 15:08:54 $ $Revision: 1.2 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] gzs.version [IF]

include ffl/stc.fs


( gzs = GZip Stream )
( The gzs module implements a gzip stream. It compresses [deflate] or        )
( decompresses [inflate] a stream of data.                                   )


1 constant gzs.version


( gzs structure )

begin-structure gzs%       ( -- n = Get the required space for a gzs variable )
  field:  gzs>events          \ the number of events in the machine
end-structure


( Stream creation, initialisation and destruction )

: gzs-init         ( gzs -- = Initialise the GZip stream )
\ ToDo
;


: gzs-(free)       ( gzs -- = Free the internal, private variables from the heap )
\ ToDo
;


: gzs-create       ( "<spaces>name" +n -- ; -- gzs = Create a named GZip stream in the dictionary )
  create   here   gzs% allot   gzs-init
;


: gzs-new          ( +n -- gzs = Create a new GZip stream on the heap )
  gzs% allocate  throw  tuck gzs-init
;


: gzs-free         ( gzs -- = Free the stream from the heap )
  dup gzs-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the gzs
;


( Member words )



( Private words )


( Stream words )

: gzs-reset  ( gzs -- = Reset the stream )
\ ToDo
;


: gzs-compress  ( a-addr1 u1 gzs -- a-addr2 u2 | 0 = Compress/deflate u bytes starting at a-addr and return the result a-addr2 u2 )
\ ToDo
;


: gzs-decompress   ( a-addr1 u1 gzs -- a-addr2 u2 | 0 = Decompress/inflate u1 compressed bytes starting at a-addr1 and return the result a-addr2 u2 )
\ ToDo
;


: gzs-finish       ( gzs -- a-addr u | 0 = Finish the compression/decompression )
\ ToDo
;

 
( Inspection )

: gzs-dump   ( gzs - = Dump the gzs )
  ." gzs:" . cr
;
  
[THEN]

\ ==============================================================================
