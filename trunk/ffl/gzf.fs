\ ==============================================================================
\
\              gzf - the gzip file base module in the ffl
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
\  $Date: 2008-10-20 16:59:45 $ $Revision: 1.11 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] gzf.version [IF]

include ffl/str.fs


( gzf = gzip File )
( The gzf module implements the base definitions for using a gzip file. It   )
( is  used by the [zif] and, in a future version, [zof] module.              )


1 constant gzf.version


( Operating Systems constants )

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


( Compression modes constants )

  8 constant gzf.deflate   ( -- n = Deflate Compression mode )


( gzf structure )

begin-structure gzf%       ( -- n = Get the required space for a gzf variable )
  field:  gzf>text           \ is the gzip file a text file ?
  field:  gzf>flags          \ the flags
  field:  gzf>mtime          \ the modification time
  field:  gzf>os             \ the operating system
  field:  gzf>xflags         \ the extra flags
  field:  gzf>xlen           \ the extra length
  str%
  +field  gzf>name           \ Name string
  str%
  +field  gzf>comment        \ Comment string
end-structure


( gzip file variable creation, initialisation and destruction )

: gzf-init         ( gzf -- = Initialise the gzip file variable )
  dup  gzf>text   off
  dup  gzf>flags  0!
  dup  gzf>mtime  0!
  gzf.unknown
  over gzf>os      !
  dup  gzf>xflags 0!
  dup  gzf>xlen   0!
  dup  gzf>name    str-init
       gzf>comment str-init
;


: gzf-(free)       ( gzf -- = Free the internal, private variables from the heap )
  dup gzf>name     str-(free)
      gzf>comment  str-(free)
;


: gzf-create       ( "<spaces>name" -- ; -- gzf = Create a named gzip file variable in the dictionary )
  create   here   gzf% allot   gzf-init
;


: gzf-new          ( -- gzf = Create a new gzip file variable on the heap )
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
 

: gzf-flags@       ( gzf -- u = Get the flags from the gzip file header )
  gzf>flags @
;


: gzf-flags!       ( u gzf -- = Set the flags for the gzip file header )
  gzf>flags !
;


: gzf-os@          ( gzf -- n = Get the operating system of the current file in the gzip file )
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


( Header words )

: gzf-reset        ( gzf -- = Reset the gzip file header )
  dup  gzf>text   off
  dup  gzf>flags  0!
  gzf.unknown
  over gzf>os      !
  dup  gzf>mtime  0!
  dup  gzf>xflags 0!
  dup  gzf>xlen   0!
  dup  gzf>name    str-clear
       gzf>comment str-clear
;


( Inspection )

: gzf-dump   ( gzf -- = Dump the variable )
  ." gzf:" dup . cr
  ."  text?  :" dup gzf>text  ? cr
  ."  os     :" dup gzf>os    ? cr
  ."  mtime  :" dup gzf>mtime ? cr
  ."  name   :" dup gzf>name    str-get type cr
  ."  comment:" dup gzf>comment str-get type cr
  drop
;
  
[THEN]

\ ==============================================================================
