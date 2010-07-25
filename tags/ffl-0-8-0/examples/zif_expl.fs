\ ==============================================================================
\
\          zif_expl - the gzip file reader example in the ffl
\
\               Copyright (C) 2009  Dick van Oudheusden
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
\  $Date: 2008-01-13 08:09:33 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/zif.fs

\ Convert gzips modification time to a string

include ffl/dos.fs

dtm-create gzf-dtm
tos-create gzf-tos

: mtime-to-str ( u1 -- c-addr u2 = Convert gzips mtime u1 to the string c-addr u2 )
  0 dtm.unix-epoch gzf-dtm dtm-set-with-seconds  \ Calculate the date/time
  gzf-dtm s" %c" gzf-tos dos-write-format        \ Format the date/time string
  gzf-tos str-get                                \ Get the string
;


\ Copy contents gzip file to another file

: copy-to-file     ( c-addr u zif -- ior = Copy contents )
  >r
  r/w create-file throw           \ Create the destination file
  BEGIN                           \ Zolang data in gzip file Do
    pad 80 r@ zif-read-file
    dup 0= IF
      over 0>
    ELSE
      false
    THEN
  WHILE
    drop
    over pad -rot write-file throw  \  Write data in file
  REPEAT
  nip
  swap close-file throw
  rdrop
;


\ Example: Read a gzip file and save the result in a file

\ Create the gzip file reader variable

zif-new value zif1

s" gzipped.gz"  zif1 zif-open-file ?dup 0= [IF]
  .( gzipped.gz succesfully opened) cr

  \ Read the header info
  zif1 zif-read-header ?dup 0= [IF]
    
    \ Get the header info
    zif1 zif-gzf@
    .( Text file         : ) dup gzf-text@ . cr
    .( Operating system  : ) dup gzf-os@ . cr
    .( Modification time : ) dup gzf-mtime@ mtime-to-str type cr
    .( Name              : ) dup gzf-name@ type cr
    .( Comment           : ) dup gzf-comment@ type cr
    
    gzf-name@ zif1 copy-to-file ?dup 0= [IF]
      .( Gzip file is succesfully inflated.) cr
    [ELSE]
      .( Error during inflation: ) . cr
    [THEN]
  [THEN]
  
  zif1 zif-close-file drop
[ELSE]
  .( Error opening gzipped.gz:) . cr
[THEN]

\ Free the zif variable from the heap

zif1 zif-free

