\ ==============================================================================
\
\             htm - the HTML reader and writer in the ffl
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
\  $Date: 2007-09-15 06:24:31 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] htm.version [IF]

include ffl/str.fs

( htm = HTML reader and writer )
( The htm module implements words for non-validating parsing of HTML sources )
( and for writing HTML. ..more..                                             )
( <pre>                                                                      )
(   Stack usage reader word: htm-reader ( w:data - false | c-addr u true     )
(   Stack usage writer word: htm-writer ( c-addr u w:data - f                )
( </pre>                                                                     )


1 constant htm.version


( Html reader constants )

1 constant htm.done                    ( Html reader is ready with reading )
2 constant htm.comment                 ( Html comment                      )
3 constant htm.text                    ( Html normal text                  )
4 constant htm.start-tag               ( Html start tag                    )
5 constant htm.end-tag                 ( Html end tag                      )
6 constant htm.markup                  ( Html markup                       )
7 constant htm.processing-instruction  ( Html processing instruction       )


( Html reader and writer structure )

struct: htm%   ( - n = Get the required space for the html reader data structure )
  cell:  htm>string     \ the html string
  cell:  htm>index      \ the index in the html string
  cell:  htm>start      \ the start of the current parsed item
  cell:  htm>length     \ the length of the current parsed item
  cell:  htm>reader     \ the xt of the reader (pull)
  cell:  htm>writer     \ the xt of the writer
  cell:  htm>data       \ the optional data for the reader and writer
;struct



( Html parser structure creation, initialisation and destruction )

: htm-init   ( w:htm - = Initialise the html parser structure )
  str-new
  over htm>string  !
  dup  htm>index   0!
  dup  htm>start   nil!
  dup  htm>length  0!
  dup  htm>reader  nil!
  dup  htm>writer  nil!
       htm>data    nil!
;


: htm-create   ( C: "name" -  R: - w:htm = Create a named html parser structure in the dictionary )
  create  here htm% allot  htm-init
;


: htm-new   ( - w:htm = Create a new html parser structure on the heap )
  htm% allocate  throw   dup htm-init
;


: htm-free   ( w:htm - = Free the htmument parser structure from the heap )
  dup htm>string @ str-free
  free throw
;


( Html reader init words )

: htm-set-reader  ( w:data xt w:htm - = Init the html reader for reading using the reader callback )
  >r
  r@ htm>reader !
  r@ htm>data   !
  r@ htm>string @ str-clear
  r> htm>index  0!
;


: htm-set-string  ( c-addr u w:htm - = Init the html reader for for reading from string )
  >r
  r@ htm>string str-set
  r@ htm>index  0!
  r@ htm>reader nil!
  r> htm>data   nil!
;


( Private reader words )

: htm-read-markup ( .. )
;


: htm-read-start-tag ( .. )
;


: htm-read-end-tag ( .. )
;


: htm-read-processing-instruction ( .. )
;

: htm-read-text ( .. )
;


( Html reader word )

: htm-read ( - HTM-DONE | .. )
;



( Html writer init words )

: htm-set-writer ( w:data xt w:htm - = Init the html writer for writing using the writer callback )
;


: htm-clear-string ( w:htm - = Init the html writer for writing to string )
;


( Html writer words )

: htm-write-markup ( c-addr u .. n c-addr u w:htm - = Write the markup with n parameters )
;


: htm-write-start-tag ( c-addr u c-addr u .. n c-addr u w:htm - = Write the HTML start tag with n attributes )
;


: htm-write-end-tag ( c-addr u w:htm - = Write the HTML end tag )
;


: htm-write-processing-instruction ( .. )
;

: htm-write-text ( c-addr u w:htm - = Write normal HTML text )
;


: htm-write-comment ( c-addr u w:htm - = Write HTML comment )
;


( Html writer finish words )

: htm-get-string ( w:htm - c-addr u = Get the html string when writing to string )
;

( Inspection )

: htm-dump ( w:htm - = Dump the html structure )
;

[THEN]

\ ==============================================================================
