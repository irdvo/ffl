\ ==============================================================================
\
\                 gmo - the message catalog in the ffl
\
\               Copyright (C) 2007  Dick van Oudheusden
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
\  $Date: 2007-11-12 07:12:40 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] gmo.version [IF]

include ffl/msc.fs

( gmo = Gettexts mo-file )
( The gmo module implements words for importing the contents of gettexts mo- )
( file into a message catalog.                                               )


1 constant gmo.version


( File structure )

struct: gmo%   ( - n = Get the required space for the gmo data structure )
  cell:  gmo>file            \ File id
  cell:  gmo>open            \ Is the file open ?
  cell:  gmo>msc             \ Message catalog
  2
  cells: gmo>buffer          \ Input buffer
  cell:  gmo>message         \ Message
  cell:  gmo>translation     \ Translation
  hnt%
  field: gmo>table           \ Hash table
  cell:  gmo>mo1
  cell:  gmo>mo2
;struct


( File structure creation, initialisation and destruction )

: gmo-init   ( w:gmo - = Initialise the mo-file structure )
  \ ToDo
;


: gmo-create   ( C: "name" -  R: - w:gmo = Create a named mo-file variable in the dictionary )
  create  here gmo% allot  gmo-init
;


: gmo-new   ( - w:gmo = Create a mo-file variable on the heap )
  gmo% allocate  throw   dup gmo-init
;


: gmo-free   ( w:gmo - = Free the mo-file variable from the heap )
  \ ToDo
;


( File words )

: gmo-open  ( c-addr u w:gmo - = Open the mo-file )
  \ ToDo
;


: gmo-read  ( w:msc w:gmo - ?? = Read the mo-file, add the contents to the catalog )
  >r
  r@ gmo>mo1 2 cells char/ 2swap
  
  r/o bin open-file throw
  >r
  2dup r@ read-file throw
  
  over <> exp-no-data AND throw
  
  over dup @ swap cell+ @
  
  hex u. u. decimal
  
  2drop
  r>
  close-file throw
  rdrop
;


: gmo-close   ( w:gmo - = Close the mo-file )
  \ ToDo
;


( Inspection )

: gmo-dump  ( w:gmo - = Dump mo-file variable )
  ." gmo:" . cr 
  \ ToDo
;

[THEN]

\ ==============================================================================
