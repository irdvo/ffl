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
\  $Date: 2008-07-03 17:21:49 $ $Revision: 1.6 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] gzf.version [IF]

include ffl/gzs.fs
include ffl/str.fs
include ffl/gzp.fs

( gzf = GZip File )
( The gzf module implements a GZip file. It compresses [deflate] or          )
( decompresses [inflage] a file.                                             )


1 constant gzf.version


( Operating Systems )

  0 constant gzf.fat       ( - n = FAT OS )
  1 constant gzf.amiga     ( - n = Amiga OS )
  2 constant gzf.vms       ( - n = VMS OS )
  3 constant gzf.unix      ( - n = UNIX OS )
  4 constant gzf.vm/cms    ( - n = VM-CMS OS )
  5 constant gzf.atari     ( - n = ATARI OS )
  6 constant gzf.hpfs      ( - n = HPFS OS )
  7 constant gzf.macintos  ( - n = MACINTOS OS )
  8 constant gzf.z-system  ( - n = Z-SYSTEM OS )
  9 constant gzf.cp/m      ( - n = CP-M OS )
 10 constant gzf.tops-20   ( - n = TOPS-20 OS )
 11 constant gzf.ntfs      ( - n = NTFS OS )
 12 constant gzf.qdos      ( - n = QDOS OS )
 13 constant gzf.acorn     ( - n = ACORN OS )
255 constant gzf.unknown   ( - n = other  )


( Compression modes )

  8 constant gzf.deflate   ( - n = Deflate Compression mode )


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
  field:  gzf>state          \ the state: 1=reading 2=writing 0=none
  field:  gzf>file           \ the current file
  field:  gzf>text           \ is the gzip file a text file ?
  field:  gzf>mode           \ the compression mode
  field:  gzf>os             \ the operating system
  field:  gzf>mtime          \ the modification time
  str%
  +field  gzf>name           \ Name string
  str%
  +field  gzf>comment        \ Comment string
  gzp%
  +field  gzf>gzp            \ GZip base words
end-structure


( GZip file variable creation, initialisation and destruction )

: gzf-init         ( gzf -- = Initialise the GZip file variable )
  dup  gzf>state  0!
  dup  gzf>file   0!
  dup  gzf>text   off
  dup  gzf>mode   0!
  dup  gzf>mtime  0!
  gzf.unknown
  over gzf>os      !
  dup  gzf>name    str-init
  dup  gzf>comment str-init
  dup  gzf>gzp     gzp-init
  drop
\ ToDo
;


: gzf-(free)       ( gzf -- = Free the internal, private variables from the heap )
  dup gzf>name    str-(free)
  dup gzf>comment str-(free)
  dup gzf>gzp     gzp-(free)
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
  

: gzf-mode@        ( gzf -- n = Get the compression mode of the current file in the gzip file )
  gzf>mode @
;


: gzf-mode!        ( n gzf -- = Set the compression mode for the next file in the gzip file )
  gzf>mode !
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


: gzf+read-zstring    ( c-addr fileid str -- = Read a z-string from the gzip file )
  >r
  r@ str-clear
  BEGIN
    2dup 1 swap read-file throw

    1 <> exp-no-data AND throw    \ Data available ?

    over c@ ?dup             \ While not eos
  WHILE
    r@ str-append-char
  REPEAT
  2drop
  rdrop
;


: gzf-do-read-header  ( hdr gzf -- = Read the gzip file header )
  >r
  dup gzf%hdr% r@ gzf-file@ read-file throw  \ Read the header

  gzf%hdr% <> exp-no-data AND throw    \ Check the length the read data
    
  dup  gzf>hdr>id1 c@ 31  <>           \ Check file type
  over gzf>hdr>id2 c@ 139 <> OR
  exp-wrong-file-type AND throw

  dup gzf>hdr>cm c@ 8 u>               \ Check compression mode
  exp-wrong-file-data AND throw

  dup gzf>hdr>flg c@ 224 AND 0<>       \ Check flags
  exp-wrong-file-data AND throw
     
  dup gzf>hdr>flg c@                   \ Process flags

  dup 1 AND 0<>                        \ File is text
  r@ gzf-text!

  dup 4 AND IF                         \ Extra fields
    over 2 r@ gzf-file@ read-file throw  \ Reuse id fields ...

    2 <> exp-no-data AND throw         \ Data present ?

    over dup c@ swap char+ c@ 256 * +  \ xlen 
    
    r@ gzf-file@ file-position throw
    rot m+ r@ gzf-file@ reposition-file throw \ Skip extra fields
  THEN

  dup 8 AND IF                \ Name field
    over  r@ gzf-file@  r@ gzf>name  gzf+read-zstring
  THEN

  dup 16 AND IF               \ Comment field
    over  r@ gzf-file@  r@ gzf>comment  gzf+read-zstring
  THEN

  2 AND IF                    \ (Skip) CRC field
    over 2 r@ gzf-file@ read-file throw  \ Reuse id fields ...
  THEN

  dup gzf>hdr>cm c@          \ Header seems okee, save the info
  r@  gzf-mode!

  dup gzf>hdr>mtime @
  r@  gzf-mtime!

  gzf>hdr>os c@
  r>  gzf-os!
;


( File words )

: gzf-open-file    ( c-addr u gzf -- ior = Open an existing gzip file for reading with name c-addr u )
  >r
  r@ gzf>state @ 0<> exp-invalid-state AND throw

  r/o bin open-file ?dup IF
    nip
  ELSE
      r@ gzf>file  !
    1 r@ gzf>state !
    0
  THEN
  rdrop
;


: gzf-read-header  ( gzf -- ior = Read the [next] header from the gzip file )
  >r
  r@ gzf>state @ 1 <> exp-invalid-state AND throw

  gzf%hdr% allocate throw       \ Allocate space for the header
    
  dup r@ ['] gzf-do-read-header catch  \ Try to read the header

  dup IF
    nip nip                     \ Exception -> drop gzf,hdr
  ELSE
    r@ gzf>gzp gzp-init-inflate \ Start inflation
  THEN
  swap free throw
  rdrop
;


: gzf-read-file    ( c-addr1 u1 gzf -- u2 ior = Read/decompress maximum u1 bytes from the file and store those at c-addr1, return the actual read bytes )
\ Buffer? -> read bytes from file -> feed gzp -> process result, keep reading till u1 bytes 
\ ToDo
;


: gzf-read-line    ( c-addr1 u1 gzf -- u2 ior = Read/decompress till end of line or maximum u1 bytes from the file and store those at c-addr1, return the actual read bytes )
\ ToDo
;


: gzf-create-file  ( c-addr u gzf -- ior = Create a gzip file for writing with name c-addr u )
  >r
  r@ gzf>state @ 0<> exp-invalid-state AND throw

  w/o bin create-file ?dup IF
    nip
  ELSE
      r@ gzf>file     !
    2 r@ gzf>state    !

    8 r@ gzf>mode     !      \ Reset the header info
      r@ gzf>mtime   0!
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
  r@ gzf>state @ 2 <> exp-invalid-state AND throw
  
  gzf%hdr% allocate throw    \ Allocate space for the header

  31  over gzf>hdr>id1 c!
  139 over gzf>hdr>id2 c!

  r@ gzf-mode@
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
  dup  gzf>state @ 0= exp-invalid-state AND throw

  dup  gzf-file@ close-file 

  swap gzf>state 0!
;

 
( Inspection )

: gzf-dump   ( gzf - = Dump the gzf )
  ." gzf:" dup . cr
  ."  state  :" dup gzf>state ? cr
  ."  file   :" dup gzf>file  ? cr
  ."  text?  :" dup gzf>text  ? cr
  ."  mode   :" dup gzf>mode  ? cr
  ."  os     :" dup gzf>os    ? cr
  ."  mtime  :" dup gzf>mtime ? cr
  ."  name   :" dup gzf>name    str-get type cr
  ."  comment:" dup gzf>comment str-get type cr
  ."  GZip   :" dup gzf>gzp     gzp-dump cr
  drop
;
  
[THEN]

\ ==============================================================================
