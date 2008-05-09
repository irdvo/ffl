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
\  $Date: 2008-05-09 12:52:25 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] gzf.version [IF]

include ffl/gzs.fs


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
  field:  gzf>file            \ the current file
  field:  gzf>text            \ is the gzip file a text file ?
  field:  gzf>mode            \ the compression mode
  field:  gzf>os              \ the operating system
  field:  gzf>mtime           \ the modification time
  field:  gzf>reading         \ is gzf in reading mode ?
end-structure


( GZip file variable creation, initialisation and destruction )

: gzf-init         ( gzf -- = Initialise the GZip file variable )
\ ToDo
;


: gzf-(free)       ( gzf -- = Free the internal, private variables from the heap )
\ ToDo
;


: gzf-create       ( "<spaces>name" +n -- ; -- gzf = Create a named GZip file variable in the dictionary )
  create   here   gzf% allot   gzf-init
;


: gzf-new          ( +n -- gzf = Create a new GZip file variable on the heap )
  gzf% allocate  throw  tuck gzf-init
;


: gzf-free         ( gzf -- = Free the variable from the heap )
  dup gzf-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the gzf
;


( Member words )

: gzf-text@        ( gzf -- flag = Get if the source of the gzip file is text )
  gzf>text @
;


: gzf-text!        ( flag gzf -- = Set if the source of the gzip file is text )
  gzf>text !
;
  

: gzf-mode@        ( gzf -- n = Get the compression mode )
  gzf>mode @
;


: gzf-mode!        ( n gzf -- = Set the compression mode )
  gzf>mode !
;


: gzf-os@          ( gzf -- n = Get the operting system from the gzip file )
  gzf>os @
;


: gzf-os!          ( n gzf -- = Set the operating system for the gzip file )
  gzf>os !
;


: gzf-mtime@       ( gzf -- u = Get the modification time from the gzip file in seconds since unix epoch )
  gzf>mtime @
;


: gzf-mtime!       ( u gzf -- = Set the modification time for the gzip file in seconds since unix epoch )
  gzf>mtime !
;


( Private words )

( File words )

: gzf-open-file    ( c-addr u gzf -- ior = Open an existing gzip file for reading with name c-addr u )
  >r
  r/o bin open-file ?dup IF
    nip
  ELSE
    r@ gzf>file !
    gzf%hdr% allocate throw      \ Allocate space for the header

    dup gzf%hdr% r@ gzf>file @ read-file  \ Read the header

    ?dup 0= IF                    \ Check the length the read data
      gzf%hdr% <> exp-no-data AND
    THEN
    
    ?dup 0= IF                    \ Check file type
      dup  gzf>hdr>id1 c@ 31  <>
      over gzf>hdr>id2 c@ 139 <> OR
      exp-wrong-file-type AND
    THEN

    ?dup 0= IF
      dup gzf>hdr>cm c@ . 0       \ Todo check range
    THEN
 
    ?dup 0= IF
      dup gzf>hdr>xfl c@ . 0      \ Todo extra flags
    THEN

    ?dup 0= IF
      dup gzf>hdr>cm c@
      r@  gzf-mode!

      dup gzf>hdr>mtime @
      r@  gzf-mtime!

      dup gzf>hdr>os c@
      r@  gzf-os!
      0
    THEN

    swap free throw

    dup IF
      0 r@ gzf>file @!
      close-file throw
    ELSE
      r@ gzf>reading on
    THEN
  THEN
  rdrop
;


: gzf-create-file  ( c-addr u gzf -- ior = Create a gzip file for writing with name c-addr u )
\ ToDo
;


: gzf-read-file    ( c-addr1 u1 gzf -- u2 ior = Read/decompress maximum u1 bytes from the file and store those at c-addr1, return the actual read bytes )
\ ToDo
;


: gzf-read-line    ( c-addr1 u1 gzf -- u2 ior = Read/decompress till end of line or maximum u1 bytes from the file and store those at c-addr1, return the actual read bytes )
\ ToDo
;


: gzf-write-file   ( c-addr u gzf -- ior = Write/compress u2 bytes to the file, starting from c-addr )
\ ToDo;
;


: gzf-write-line   ( c-addr u gzf -- ior = Write/compress u2 bytes to the file, starting from c-addr, followed by and end of line )
\ ToDo
;


: gzf-flush-file   ( gzf -- ior = Flush the file )
\ ToDo
;


: gzf-close-file   ( gzf -- ior = Close the file )
\ ToDo
;

 
( Inspection )

: gzf-dump   ( gzf - = Dump the gzf )
  ." gzf:" . cr
;
  
[THEN]

\ ==============================================================================
