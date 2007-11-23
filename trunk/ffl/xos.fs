\ ==============================================================================
\
\                xos - the xml/html writer in the ffl
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
\  $Date: 2007-11-23 06:21:52 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] xos.version [IF]

include ffl/tis.fs

( xos = XML/HTML writer )
( The xos module implements words for writing xml and html to an output      )
( stream.                                                                    )


1 constant xos.version


( xml writer structure )

struct: xos%   ( - n = Get the required space for the xml writer data structure )
  tos%
  field: xos>tos        \ the xml string (extends the text output stream)
;struct



( xml writer structure creation, initialisation and destruction )

: xos-init   ( w:xos - = Initialise the xml writer variable )
  
  xos>tos  tos-init
;


: xos-create   ( C: "name" -  R: - w:xos = Create a named xml writer variable in the dictionary )
  create  here xos% allot  xos-init
;


: xos-new   ( - w:xos = Create a new xml writer variable on the heap )
  xos% allocate  throw   dup xos-init
;


: xos-free   ( w:xos - = Free the xml writer variable from the heap )
  tos-free
;


( xml writer init words )

: xos-set-writer ( w:data xt w:xos - = Initialise the xml writer for writing using the writer callback )
;


: xos-clear-string ( w:xos - = Initialise the xml writer for writing to string )
;


( xml writer words )

: xos-write-markup ( c-addr u ... n c-addr u w:xos - = Write a xml markup with n parameters )
;


: xos-write-start-tag ( c-addr u c-addr u ... n c-addr u w:xos - = Write a xml start tag with n attributes )
;


: xos-write-end-tag ( c-addr u w:xos - = Write a xml end tag )
;


: xos-write-processing-instruction ( c-addr u ... n c-addr u w:xos - = Write a xml processing instruction )
;


: xos-write-text ( c-addr u w:xos - = Write normal xml text )
;


: xos-write-comment ( c-addr u w:xos - = Write xml comment )
;


: xos-write-entity-reference ( c-addr u u:xos - = Write a xml entity reference )
;


( xml writer finish words )

: xos-get-string ( w:xos - c-addr u = Get the xml string when writing to string )
;

( Inspection )

: xos-dump ( w:xos - = Dump the xml writer variable )
;

[THEN]

\ ==============================================================================
