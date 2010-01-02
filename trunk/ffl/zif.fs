\ ==============================================================================
\
\            zif - the gzip file inflate module in the ffl
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
\  $Date: 2009-05-28 17:35:58 $ $Revision: 1.11 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] zif.version [IF]

include ffl/crc.fs

[DEFINED] crc.version [IF]

include ffl/gzf.fs
include ffl/gzi.fs

( zif = gzip file reader )
( The zif module implements a gzip file reader. See [gzf] for the gzip       )
( file header information.                                                    )


1 constant zif.version


( Private default buffer size )

 2048 value zif.input-size  ( -- u = Default input buffer size )
69632 value zif.output-size ( -- u = Default output buffer size )


( Private zif state )

gzi.states constant zif.done ( -- n = gzip file reader decompression ready )


( zif structure )

begin-structure zif%       ( -- n = Get the required space for a zif variable )
  gzi%
  +field  zif>gzi            \ the gzip file reader extends the gzip inflator
  gzf%
  +field  zif>gzf            \ the gzip file header
  field:  zif>file           \ the current file
  field:  zif>eof            \ is the end of file reached for the current file ?
  field:  zif>buffer-size    \ the size of the input buffer
  field:  zif>buffer         \ the input buffer
  field:  zif>result         \ the current result
  field:  zif>length         \ the total calculated length
  field:  zif>file-len       \ the length read from the file
  crc%
  +field  zif>crc            \ the calculated crc checksum
  field:  zif>file-crc       \ the crc checksum read from the file
end-structure


( gzip file reader variable creation, initialisation and destruction )

: zif-init         ( zif -- = Initialise the gzip file reader variable )
  zif.output-size
  over zif>gzi      gzi-init
  dup  zif>gzf      gzf-init
  dup  zif>file     0!
  dup  zif>eof      off
  zif.input-size
  over zif>buffer-size !
  zif.input-size allocate throw
  over zif>buffer   !
  dup  zif>result   0!
  dup  zif>length   0!
  dup  zif>file-len 0!
  dup  zif>crc      crc-init
       zif>file-crc 0!
;


: zif-(free)       ( zif -- = Free the internal, private variables from the heap )
  dup zif>buffer @ free throw
  dup zif>gzi      gzi-(free)
  dup zif>gzf      gzf-(free)
      zif>crc      crc-(free)
;


: zif-create       ( "<spaces>name" -- ; -- zif = Create a named gzip file reader variable in the dictionary )
  create   here   zif% allot   zif-init
;


: zif-new          ( -- zif = Create a new gzip file reader variable on the heap )
  zif% allocate  throw  dup zif-init
;


: zif-free         ( zif -- = Free the variable from the heap )
  dup zif-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the zif
;


( Module words )

: zif+input-size! ( u -- = Set the default input buffer size &lb;default 2kb, min. 1kb&rb; )
  1024 max to zif.input-size
;


: zif+input-size@ ( -- u = Get the default input buffer size )
  zif.input-size
;


: zif+output-size! ( u -- = Set the default output buffer size &lb;default 64kb+4kb, min. 64kb+4kb&rb; )
  69632 max to zif.output-size
;


: zif+output-size@ ( -- u = Get the default output buffer size )
  zif.output-size
;


( Member words )

: zif-gzf@         ( zif -- gzf = Get the reference to the gzip file header info after zif-read-header )
  zif>gzf
;


( Private file words )

: zif-file@           ( zif -- fileid = Get the file id of the gzip file )
  zif>file @
;


: zif-read            ( zif -- ior = Read another block of data from the file )
  >r
  r@ zif>eof @ IF
    exp-no-data
  ELSE
    r@ zif>buffer @  r@ zif>buffer-size @  r@ zif-file@  read-file ?dup IF
      nip
    ELSE
      ?dup IF                                    \ If data available Then
        dup r@ zif>buffer-size @ < r@ zif>eof !  \   Not all available -> eof
        r@ zif>buffer @ swap r@ bis-set          \   Setup buffer in gzp module
        gzi.ok
      ELSE                                       \ Else end of file
        r@ zif>eof on
        exp-no-data
      THEN
    THEN
  THEN
  rdrop
;


( Private header words )

: zif-do-crc       ( zif -- ior = Skip CRC )
  dup zif>gzf gzf-flags@ 2 AND IF
    2 swap bis-read-bytes IF 
      drop
      gzi.done                         \ Header is successful processed
    ELSE
      gzi.more
    THEN
  ELSE
    drop gzi.done                      \ No CRC -> done
  THEN
;


: zif-do-comment   ( zif -- ior = Read the comment )
  dup zif>gzf gzf-flags@ 16 AND IF
    BEGIN
      1 over bis-read-bytes IF
        ?dup IF                             \ If not eos Then
          over zif>gzf gzf>comment str-append-char  \   Append char
          false                             \   Continue
        ELSE                                \ Else
          ['] zif-do-crc swap gzi-state!    \   Next skip crc
          gzi.ok true                       \   Done & okee
        THEN
      ELSE
        drop gzi.more true                  \ No data -> more
      THEN
    UNTIL
  ELSE
    ['] zif-do-crc swap gzi-state!          \ Next skip crc
    gzi.ok
  THEN
;

 
: zif-do-name      ( zif -- ior = Read the filename )
  dup zif>gzf gzf-flags@ 8 AND IF
    BEGIN
      1 over bis-read-bytes IF
        ?dup IF                          \ If not eos Then
          over zif>gzf gzf>name str-append-char  \   Append char  
          false                          \   Continue
        ELSE                             \ Else
          ['] zif-do-comment swap gzi-state! \  Next: comment
          gzi.ok true                    \   Done & okee
        THEN
      ELSE
        drop gzi.more true               \ No data -> more
      THEN
    UNTIL
  ELSE
    ['] zif-do-comment swap gzi-state!  \ Next: comment
    gzi.ok
  THEN
;

      
: zif-do-extra     ( zif -- ior = Skip the extra bytes )
  >r
  dup zif>gzf gzf>xlen @          \ Skip xlen byte
  BEGIN
    dup IF
      1 r@ bis-read-bytes
    ELSE
      false
    THEN
  WHILE
    1-
  REPEAT

  ?dup IF                         \ All bytes done ?
    r@ zif>gzf gzf>xlen !
    gzi.more                      \  No, need more data
  ELSE
    ['] zif-do-name r@ gzi-state! \  Yes, continue with next field
    gzi.ok
  THEN
  rdrop
;


: zif-do-xlen      ( zif -- ior = Read the extra length )
  dup zif>gzf gzf-flags@ 4 AND IF
    2 over bis-read-bytes IF
      over zif>gzf gzf>xlen !
      ['] zif-do-extra swap gzi-state!  \ Next: extra bytes
      gzi.ok
    ELSE
      drop gzi.more
    THEN
  ELSE
    ['] zif-do-name swap gzi-state!  \ Next: name
    gzi.ok
  THEN
;


: zif-do-os        ( zif -- ior = Save the Operating System )
  1 over bis-read-bytes IF
    over zif>gzf gzf-os!
    ['] zif-do-xlen swap gzi-state!  \ Next: extra fields
    gzi.ok
  ELSE
    drop gzi.more
  THEN
;


: zif-do-xflags    ( zif -- ior = Check and save the extra flags )
  1 over bis-read-bytes IF
    over zif>gzf gzf>xflags !      \ XXX Check
    ['] zif-do-os swap gzi-state!  \ Next: os
    gzi.ok
  ELSE
    drop gzi.more
  THEN
;


: zif-do-mtime     ( zif -- ior = Check and process the modification time )
  4 over bis-read-bytes IF
    over zif>gzf gzf-mtime!
    ['] zif-do-xflags swap gzi-state!  \ Next: extra flags
    gzi.ok
  ELSE
    drop gzi.more
  THEN
;


: zif-do-flags     ( zif -- ior = Check and process the flags )
  1 over bis-read-bytes IF
    2dup 1 AND 0<> swap zif>gzf gzf-text!
    over zif>gzf gzf-flags!
    ['] zif-do-mtime swap gzi-state!   \ Next: mtime
    gzi.ok
  ELSE
    drop gzi.more
  THEN
;

  
: zif-do-cm        ( zif -- ior = Check the Compression Mode )
  1 over bis-read-bytes IF
    gzf.deflate = IF         \ Only support deflate
      ['] zif-do-flags swap gzi-state!  \ Next: flags
      gzi.ok
    ELSE
      drop exp-wrong-file-data
    THEN
  ELSE
    drop gzi.more
  THEN
;


: zif-do-id        ( zif -- ior = Check the IDs from the gzip file )
  2 over bis-read-bytes IF
    35615 = IF
      ['] zif-do-cm swap gzi-state!   \ Next: check CM
      gzi.ok
    ELSE
      drop exp-wrong-file-type
    THEN
  ELSE
    drop gzi.more
  THEN
;


: zif-do-length    ( zif -- ior = Check the data length )
  4 over bis-read-bytes IF
    swap zif>file-len !       \ Save the length for checking
    zif.done                  \ All done
  ELSE
    drop gzi.more
  THEN
;


: zif-do-check     ( zif -- ior = Check the data checksum )
  4 over bis-read-bytes IF
    over zif>file-crc !                \ Save the crc for checking
    ['] zif-do-length swap gzi-state!  \ Next: check data length
    gzi.ok
  ELSE
    drop gzi.more
  THEN
;


( File words )

: zif-open-file    ( c-addr u zif -- ior = Open an existing gzip file for reading with name c-addr u )
  >r
  r/o bin open-file ?dup IF
    nip
  ELSE
    r@ zif>file    !         \ Save file id
    r@ zif>eof     off      
    ['] zif-do-id            \ Header: check ids
    r@ gzi-state!            \ zif extends gzi (and bis)
    r@ bis-reset
    0
  THEN
  rdrop
;


: zif-read-header  ( zif -- ior = Read the &lb;next&rb; header from the gzip file )
  >r

  r@ zif>gzf gzf-reset            \ Reset the header info

  gzi.ok r@ zif>result !
  BEGIN
    r@ gzi-inflate                \ Do the next step in inflation: header reading
    dup gzi.more = IF             \ If more file data is needed Then
      drop
      r@ zif-read                 \   Read another buffer of data
    THEN
    ?dup
  UNTIL                           \ Continue until done or error
  
  dup gzi.done = IF
    drop
    r@ gzi-init-inflate           \ If done Then Start inflating and ..
    r@ zif>length 0!
    r@ zif>crc crc-reset      
    0                             \ .. return okee
  THEN
  rdrop
;


: zif-read-file    ( c-addr1 u1 zif -- u2 ior = Read/decompress maximum u1 bytes from the file and store those at c-addr1, return the actual read bytes )
  >r
  r@ zif>result @                 \ Inflate until u1 bytes and okee
  BEGIN
    dup gzi.ok = IF
      over r@ gzi-lbf@ lbf-length'@ >
    ELSE
      false
    THEN
  WHILE
    drop
    r@ gzi-inflate
    
    dup gzi.done = IF             \ Inflator is ready, read the last two fields in the file
      r@ gzi-end-inflate
      ['] zif-do-check r@ gzi-state!
      drop gzi.ok
    ELSE dup gzi.more = IF        \ Read more data
      drop
      r@ gzi-reduce-output
      r@ zif-read
    THEN THEN
  REPEAT
  dup r@ zif>result !

  dup gzi.ok = over zif.done = OR IF  \ Copy the requested data
    drop
    dup r@ gzi-lbf@ lbf-get' ?dup IF
       dup r@ zif>length +!           \ Increase the calculated length
      2dup r@ zif>crc crc-update      \ Update the calculated crc
      2swap rot
      2dup - >r                       \ Calculate difference between requested and available bytes
      min >r                          \ Calculate lowest of requested and available bytes
      r@ move                         \ Move the data to the destination buffer
      r> r>
    ELSE
      nip 0 swap
    THEN                              \ S: available requested-available
    
    0> IF                             \ If last data block Then
      r@ zif>crc crc-finish r@ zif>file-crc @ <> IF
        exp-wrong-checksum
      ELSE                            \   Check CRC and file length
        r@ zif>length @ r@ zif>file-len @ <> IF
          exp-wrong-length
        ELSE
          0
        THEN
      THEN
    ELSE
      0
    THEN
  ELSE
    nip nip
    0 swap                             \ Error during inflation
  THEN
  rdrop
;


: zif-close-file   ( zif -- ior = Close the file )
  zif-file@ close-file 
;


( Inspection )

: zif-dump         ( zif -- = Dump the variable )
  ." zif:" dup . cr
    ."  gzi         : " dup zif>gzi gzi-dump
    ."  gzf         : " dup zif>gzf gzf-dump
    ."  file        : " dup zif>file ? cr
    ."  eof?        : " dup zif>eof  ? cr
    ."  buffer-size : " dup zif>buffer-size ? cr
    ."  buffer      : " dup zif>buffer @ over zif>buffer-size @ dump
    ."  result      : " dup zif>result ? cr
    ."  length      : " dup zif>length ? cr
    ."  file-length : " dup zif>file-len ? cr
    ."  file-crc    : "     zif>file-crc ? cr
;

[ELSE]
.( Warning: zif requires crc ) cr
[THEN]

[THEN]

\ ==============================================================================
