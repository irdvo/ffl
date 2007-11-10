\ ==============================================================================
\
\                 msc - the message catalog in the ffl
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
\  $Date: 2007-11-10 07:20:08 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] msc.version [IF]

include ffl/hct.fs

( msc = Message Catalog )
( The msc module implements words for building and using a message catalog.  )
( The catalog is used to translates a message to another message. It can be  )
( used for internationalization of messages and for converting messages. The )
( module supports reading gettexts mo-files.                                 )


1 constant msc.version


( Catalog structure )

struct: msc%   ( - n = Get the required space for the msc data structure )
  tis%
  field: msc>tis        \ Todo
  cell:  msc>data       \ 
  cell:  msc>writer     \ 
;struct



( Catalog structure creation, initialisation and destruction )

: msc-init   ( w:msc - = Initialise the msc structure )
;


: msc-create   ( C: "name" -  R: - w:msc = Create a named message catalog variable in the dictionary )
  create  here msc% allot  msc-init
;


: msc-new   ( - w:msc = Create a new message catalog variable on the heap )
  msc% allocate  throw   dup msc-init
;


: msc-free   ( w:msc - = Free the message catalog variable from the heap )
  free             \ ToDo
;


( Catalog words )

: msc-add  ( c-addr u c-addr u w:msc - = Add a msgid and translation to the catalog )
;


: msg-translate ( c-addr u w:msc - c-addr u = Translate a msgid with the catalog, return msgid if not found )
;


: msc-remove  ( c-addr u w:msc - = Remove a msgid from the catalog )
;


( MO-file words )

: msc-read  ( c-addr u w:msc - ?? = Add the contents of the mo-file to the catalog )
;


( Inspection )

: msc-dump  ( w:msc - = Dump the catalog )
;


[THEN]

\ ==============================================================================
