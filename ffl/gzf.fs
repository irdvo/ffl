\ ==============================================================================
\
\               gzf - the gzip file module in the ffl
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
\  $Date: 2008-08-14 17:57:44 $ $Revision: 1.8 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] gzf.version [IF]

include ffl/gzs.fs
include ffl/str.fs
include ffl/gzp.fs

( gzf = GZip File )
( The gzf module implements a GZip file. It compresses [deflate] or          )
( decompresses [inflate] a file.                                             )


1 constant gzf.version


( Operating Systems )

  0 constant gzf.fat       ( -- n = FAT OS )
  1 constant gzf.amiga     ( -- n = Amiga OS )
  2 constant gzf.vms       ( -- n = VMS OS )
  3 constant gzf.unix      ( -- n = UNIX OS )
  4 constant gzf.vm/cms    ( -- n = VM-CMS OS )
  5 constant gzf.atari     ( -- n = ATARI OS )
  6 constant gzf.hpfs      ( -- n = HPFS OS )
  7 constant gzf.macintos  ( -- n = MACINTOS OS )
  8 constant gzf.z-system  ( -- n = Z-SYSTEM OS )
  9 constant gzf.cp/m      ( -- n = CP-M OS )
 10 constant gzf.tops-20   ( -- n = TOPS-20 OS )
 11 constant gzf.ntfs      ( -- n = NTFS OS )
 12 constant gzf.qdos      ( -- n = QDOS OS )
 13 constant gzf.acorn     ( -- n = ACORN OS )
255 constant gzf.unknown   ( -- n = other  )


( Default buffer size )

2048 value gzf.size        ( -- n = Default input buffer size )

( Compression modes )

  8 constant gzf.deflate   ( -- n = Deflate Compression mode )


( Private structure )

begin-structure gzf%hdr%
  cfield: gzf>hdr>id1        \ ID1
  cfield: gzf>hdr>id2        \ ID2
  cfield: gzf>hdr>cm         \ Compression method
  cfield: gzf>hdr>flg        \ Flags
  field:  gzf>hdr>mtime      \ Modification time
  cfield: gzf>hdr>xfl        \ Extra flags
  cfield: gzf>hdr>os         \ Operating System
end-structure


( gzf structure )

begin-structure gzf%       ( -- n = Get the required space for a gzf variable )
  field:  gzf>access         \ the access: 1=reading 2=writing 0=none
  field:  gzf>file           \ the current file
  field:  gzf>eof            \ is the end of file reached for the current file ?
  field:  gzf>state          \ the execution state
  field:  gzf>buffer         \ the input buffer
  field:  gzf>text           \ is the gzip file a text file ?
  field:  gzf>flags          \ the flags
  field:  gzf>os             \ the operating system
  field:  gzf>mtime          \ the modification time
  field:  gzf>xflags         \ the extra flags
  field:  gzf>xlen           \ the extra length
  str%
  +field  gzf>name           \ Name string
  str%
  +field  gzf>comment        \ Comment string
  gzp%
  +field  gzf>gzp            \ GZip base words
  field:  gzf>result         \ Result
end-structure


( GZip file variable creation, initialisation and destruction )

: gzf-init         ( gzf -- = Initialise the GZip file variable )
  dup  gzf>access 0!
  dup  gzf>file   0!
  dup  gzf>eof    off
  dup  gzf>state  nil!
  gzf.size allocate throw
  over gzf>buffer !
  dup  gzf>text   off
  dup  gzf>mtime  0!
  gzf.unknown
  over gzf>os      !
  dup  gzf>xflags 0!
  dup  gzf>xlen   0!
  dup  gzf>name    str-init
  dup  gzf>comment str-init
  dup  gzf>gzp     gzp-init
  dup  gzf>result 0!
  drop
\ ToDo
;


: gzf-(free)       ( gzf -- = Free the internal, private variables from the heap )
  dup gzf>buffer @ free throw
  dup gzf>name     str-(free)
  dup gzf>comment  str-(free)
  dup gzf>gzp      gzp-(free)
  drop
  \ ToDo
;


: gzf-create       ( "<spaces>name" -- ; -- gzf = Create a named GZip file variable in the dictionary )
  create   here   gzf% allot   gzf-init
;


: gzf-new          ( -- gzf = Create a new GZip file variable on the heap )
  gzf% allocate  throw  dup gzf-init
;


: gzf-free         ( gzf -- = Free the variable from the heap )
  dup gzf-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the gzf
;


( Member words )

: gzf-text@        ( gzf -- flag = Get if the source of the current file in the gzip file is text )
  gzf>text @
;


: gzf-text!        ( flag gzf -- = Set if the source for the next file in the gzip file is text )
  gzf>text !
;
  

: gzf-os@          ( gzf -- n = Get the operting system of the current file in the gzip file )
  gzf>os @
;


: gzf-os!          ( n gzf -- = Set the operating system for the next file in the gzip file )
  gzf>os !
;


: gzf-mtime@       ( gzf -- u = Get the modification time of the current file in the gzip file in seconds since unix epoch )
  gzf>mtime @
;


: gzf-mtime!       ( u gzf -- = Set the modification time for the next file in the gzip file in seconds since unix epoch )
  gzf>mtime !
;


: gzf-name@        ( gzf -- c-addr u = Get the name of the current file in the gzip file )
  gzf>name str-get
;


: gzf-name!        ( c-addr u gzf -- = Set the name of the next file in the gzip file )
  gzf>name str-set
;


: gzf-comment@     ( gzf -- c-addr u = Get the comment of the current file in the gzip file )
  gzf>comment str-get
;


: gzf-comment!     ( c-addr u gzf -- = Set the comment for the next file in the gzip file )
  gzf>comment str-set
;


( Private words )

: gzf-file@           ( gzf -- fileid = Get the file id of the gzip file )
  gzf>file @
;


: gzf-read            ( gzf -- n = Read another block of data from the file )
  ." gzf-read"
  >r
  r@ gzf>eof @ IF
    exp-no-data
  ELSE
    r@ gzf>buffer @  gzf.size  r@ gzf-file@  read-file ?dup IF \ ToDo gzf.size
      nip
    ELSE
      ?dup IF                                    \ If data available Then
        dup gzf.size < r@ gzf>eof !              \   Not all available -> eof
        r@ gzf>buffer @ swap  r@ gzf>gzp gzp-set \   Setup buffer in gzp module
        gzp.ok
      ELSE                                       \ Else end of file
        r@ gzf>eof on
        exp-no-data
      THEN
    THEN
  THEN
  rdrop
;


( Private header words )

: gzf-do-crc       ( gzf -- n = Skip CRC )
  dup gzf>flags @ 2 AND IF
    2 swap gzf>gzp gzp-skip-bytes IF 
      gzp.done                         \ Header is succesfull processed
    ELSE
      gzp.more
    THEN
  ELSE
    drop gzp.done                      \ No CRC -> done
  THEN
;

  
: gzf-do-comment   ( gzf -- n = Read the comment )
  dup gzf>flags @ 16 AND IF
    BEGIN
      dup gzf>gzp gzp-byte IF
        ?dup IF                             \ If not eos Then
          over gzf>comment str-append-char  \   Append char
          false                             \   Continue
        ELSE                                \ Else
          ['] gzf-do-crc swap gzf>state !   \   Next skip crc
          gzp.ok true                       \   Done & okee
        THEN
      ELSE
        drop gzp.more true                  \ No data -> more
      THEN
    UNTIL
  ELSE
    ['] gzf-do-crc swap gzf>state !         \ Next skip crc
    gzp.ok
  THEN
;

 
: gzf-do-name      ( gzf -- n = Read the filename )
  dup gzf>flags @ 8 AND IF
    BEGIN
      dup gzf>gzp gzp-byte IF
        ?dup IF                          \ If not eos Then
          over gzf>name str-append-char  \   Append char  
          false                          \   Continue
        ELSE                             \ Else
          ['] gzf-do-comment swap gzf>state ! \  Next: comment
          gzp.ok true                    \   Done & okee
        THEN
      ELSE
        drop gzp.more true               \ No data -> more
      THEN
    UNTIL
  ELSE
    ['] gzf-do-comment swap gzf>state ! \ Next: comment
    gzp.ok
  THEN
;

      
: gzf-do-extra     ( gzf -- n = Skip the extra bytes )
  dup gzf>xlen @ ?dup IF
    over gzp-skip-bytes IF
      ['] gzf-do-name swap gzf>state !  \ Next ..
      gzp.ok
    ELSE
      drop gzp.more
    THEN    
  ELSE
    ['] gzf-do-name swap gzf>state !  \ Next ..
    gzp.ok
  THEN
;


: gzf-do-xlen      ( gzf -- n = Read the extra length )
  dup gzf>flags @ 4 AND IF
    dup gzf>gzp gzp-byte2 IF
      over gzf>xlen !
      ['] gzf-do-extra swap gzf>state ! \ Next extra bytes
      gzp.ok
    ELSE
      drop gzp.more
    THEN
  ELSE
    ['] gzf-do-name swap gzf>state ! \ Next name
    gzp.ok
  THEN
;


: gzf-do-os        ( gzf -- n = Save the Operating System )
  dup gzf>gzp gzp-byte IF
    over gzf-os!
    ['] gzf-do-xlen swap gzf>state ! \ Next extra fields
    gzp.ok
  ELSE
    drop gzp.more
  THEN
;


: gzf-do-xflags    ( gzf -- n = Check and save the extra flags )
  dup gzf>gzp gzp-byte IF
    over gzf>xflags ! \ ToDo Check
    ['] gzf-do-os swap gzf>state ! \ Next: os
    gzp.ok
  ELSE
    drop gzp.more
  THEN
;


: gzf-do-mtime     ( gzf -- n = Check and process the modification time )
  dup gzf>gzp gzp-byte4 IF
    over gzf-mtime!
    ['] gzf-do-xflags swap gzf>state ! \ Next: extra flags
    gzp.ok
  ELSE
    drop gzp.more
  THEN
;


: gzf-do-flags     ( gzf -- n = Check and process the flags )
  dup gzf>gzp gzp-byte IF
    2dup 1 AND 0<> swap gzf-text!
    over gzf>flags !
    ['] gzf-do-mtime swap gzf>state ! \ Next: mtime
    gzp.ok
  ELSE
    drop gzp.more
  THEN
;

  
: gzf-do-cm        ( gzf -- n = Check the Compression Mode )
  dup gzf>gzp gzp-byte IF
    gzf.deflate = IF         \ Only support deflate
      ['] gzf-do-flags swap gzf>state ! \ Next: flags
      gzp.ok
    ELSE
      drop exp-wrong-file-data
    THEN
  ELSE
    drop gzp.more
  THEN
;


: gzf-do-id        ( gzf -- n = Check the IDs from the gzip file )
  dup gzf>gzp gzp-byte2 IF
    35615 = IF
      ['] gzf-do-cm swap gzf>state !   \ Next: check CM
      gzp.ok
    ELSE
      drop exp-wrong-file-type
    THEN
  ELSE
    drop gzp.more
  THEN
;


( File words )

: gzf-open-file    ( c-addr u gzf -- ior = Open an existing gzip file for reading with name c-addr u )
  >r
  r@ gzf>access @ 0<> exp-invalid-state AND throw

  r/o bin open-file ?dup IF
    nip
  ELSE
      r@ gzf>file    !        \ Save file id
      r@ gzf>eof     off      
    1 r@ gzf>access  !       \ Set reading
    ['] gzf-do-id            \ Header: check ids
      r@ gzf>state   !
      r@ gzf>gzp     gzp-clear
    0
  THEN
  rdrop
;


: gzf-read-header  ( gzf -- ior = Read the [next] header from the gzip file )
  >r
  r@ gzf>access @ 1 <> exp-invalid-state AND throw  \ reading ?

  r@ gzf>text   0!                \ Reset the header fields
  r@ gzf>flags  0!
  gzf.unknown
  r@ gzf>os      !
  r@ gzf>mtime  0!
  r@ gzf>xflags 0!
  r@ gzf>xlen   0!
  r@ gzf>name    str-clear
  r@ gzf>comment str-clear
  gzp.ok
  r@ gzf>result  !
  BEGIN
    r@ dup gzf>state @ execute    \ Execute next step in header reading
    dup gzp.more = IF             \ If more file data is needed Then
      drop
      r@ gzf-read                 \   Read another buffer of data
    THEN
    ?dup
  UNTIL                           \ Continue until done or error
  dup gzp.done = IF
    r@ gzf>gzp gzp-init-inflate   \ If done Then Start inflating and ..
    drop 0                        \ .. return okee
  THEN
  rdrop
;


: gzf-read-file    ( c-addr1 u1 gzf -- u2 ior = Read/decompress maximum u1 bytes from the file and store those at c-addr1, return the actual read bytes )
  >r
  r@ gzf>result @                 \ Inflate until u1 bytes and okee
  BEGIN
    gzp.ok = IF
      r@ gzf>gzp gzp-get-length over <
    ELSE
      false
    THEN
  WHILE
    r@ gzf>gzp gzp-inflate 
    dup gzp.more = IF             \ Read more data
      drop
      r@ gzf-read
    THEN
  REPEAT
  dup r@ gzf>result !

  dup gzp.done = IF               \ Inflate done
    drop
    r@ gzf>gzp gzp-end-inflate
    gzp.ok
  THEN

  dup gzp.ok = IF
    drop
    dup r@ gzf>gzp gzp-get ?dup IF
      rot min >r                  \ Min of requested and present length
      swap r@ move                \ Switch source and dest and move
      r>                          \ Return length
    ELSE
      2drop
      0
    THEN
    gzp.ok
  ELSE
    nip nip
    0 swap
  THEN
  rdrop
;


: gzf-read-line    ( c-addr1 u1 gzf -- u2 ior = Read/decompress till end of line or maximum u1 bytes from the file and store those at c-addr1, return the actual read bytes )
\ ToDo
;


: gzf-create-file  ( c-addr u gzf -- ior = Create a gzip file for writing with name c-addr u )
  >r
  r@ gzf>access @ 0<> exp-invalid-state AND throw

  w/o bin create-file ?dup IF
    nip
  ELSE
      r@ gzf>file     !
    2 r@ gzf>access   !

      r@ gzf>mtime   0!      \ Reset the header info
      r@ gzf>name    str-clear
      r@ gzf>comment str-clear
    gzf.unknown
      r@ gzf>os       !
    0
  THEN
  rdrop
;


: gzf-write-header ( gzf -- ior = Write the [next] header in the gzip file )
  >r
  r@ gzf>state @ 2 <> exp-invalid-state AND throw  \ writing ?
  
  gzf%hdr% allocate throw    \ Allocate space for the header

  31  over gzf>hdr>id1 c!
  139 over gzf>hdr>id2 c!

  gzf.deflate
      over gzf>hdr>cm  c!
  0
  r@ gzf-text@ IF
    1 OR
  THEN
  r@ gzf>name str-empty? 0= IF
    8 OR
  THEN
  r@ gzf>comment str-empty? 0= IF
    16 OR
  THEN
      over gzf>hdr>flg c!
  r@ gzf-mtime@
      over gzf>hdr>mtime !
  0   over gzf>hdr>xfl  c!
  r@ gzf-os@
      over gzf>hdr>os   c!

  dup gzf%hdr%  r@ gzf-file@  write-file 
  
  ?dup 0= IF
    r@ gzf>name str-length@ ?dup IF
      r@ gzf>name str-get-zstring
      swap char+  r@ gzf-file@  write-file
    ELSE
      0
    THEN
  THEN

  ?dup 0= IF
    r@ gzf>comment str-length@ ?dup IF
      r@ gzf>comment str-get-zstring
      swap char+  r@ gzf-file@  write-file
    ELSE
      0
    THEN
  THEN

  swap free throw

  rdrop
;


: gzf-write-file   ( c-addr u gzf -- ior = Write/compress u2 bytes to the file, starting from c-addr )
\ ToDo
;


: gzf-write-line   ( c-addr u gzf -- ior = Write/compress u2 bytes to the file, starting from c-addr, followed by and end of line )
\ ToDo
;


: gzf-flush-file   ( gzf -- ior = Flush the file )
\ ToDo
;


: gzf-close-file   ( gzf -- ior = Close the file )
  dup  gzf>access @ 0= exp-invalid-state AND throw

  dup  gzf-file@ close-file 

  swap gzf>access 0!
;

 
( Inspection )

: gzf-dump   ( gzf - = Dump the gzf )
  ." gzf:" dup . cr
  ."  access  :" dup gzf>access ? cr
  ."  file   :" dup gzf>file  ? cr
  ."  text?  :" dup gzf>text  ? cr
  ."  os     :" dup gzf>os    ? cr
  ."  mtime  :" dup gzf>mtime ? cr
  ."  name   :" dup gzf>name    str-get type cr
  ."  comment:" dup gzf>comment str-get type cr
  ."  GZip   :" dup gzf>gzp     gzp-dump cr
  drop
;
  
[THEN]

\ ==============================================================================
