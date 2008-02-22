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
\  $Date: 2008-02-22 06:38:06 $ $Revision: 1.8 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] msc.version [IF]

include ffl/hnt.fs

( msc = Message Catalog )
( The msc module implements a message catalog.                               )
( The catalog is used to translates a message to another message. It can be  )
( used for internationalization of messages and for converting messages. Use )
( the [gmo] module for importing gettexts mo-files in a message catalog.     )


1 constant msc.version


( Private structure )

begin-structure msc%msg%   ( -- n = Get the required space a message node )
  hnn%
  +field  msc>msg>node       \ Hash table node
  field:  msc>msg>length     \ Translation length
  field:  msc>msg>text       \ Translation text
end-structure


: msc-msg-free   ( msg -- = Free the text in the node )
  msc>msg>text @ free throw
;


( Catalog structure )

hnt% constant msc%    ( -- n = Get the required space for a message catalog )


( Catalog creation, initialisation and destruction )

: msc-init   ( msc -- = Initialise the catalog )
  10 swap hnt-init
;


: msc-(free)   ( msc -- = Free the catalogs nodes from the heap )
  ['] msc-msg-free swap hnt-(free)
;


: msc-create   ( "<spaces>name" --  ; -- msc = Create a named message catalog in the dictionary )
  create  here msc% allot  msc-init
;


: msc-new   ( -- msc = Create a message catalog on the heap )
  msc% allocate  throw   dup msc-init
;


: msc-free   ( msc -- = Free the message catalog from the heap )
  dup msc-(free)
  
  free throw
;


( Catalog words )

: msc-add  ( c-addr1 u1 c-addr2 u2 msc -- = Add the message c-addr1 u1 and translation c-addr2 u2 to the catalog )
  >r
  2swap
  2dup r@ hnt-search              \ Search for the key in the table
  nil<>? IF                       \ If already in hash table Then
    >r
    drop 2drop                    \   Drop hash and key
    dup r@ msc>msg>length @ > IF  \   If new translation is longer previous Then
      r@ msc>msg>text @ 
      over resize throw           \     Resize the text
      r@ msc>msg>text !
    THEN
    dup r@ msc>msg>length !       \   Save the new length
    r> msc>msg>text @
    swap cmove                    \   Save the translation text
  ELSE                            \ Else
    msc%msg% allocate throw       \   Create new message node
    >r
    r@ hnn-init                   \   Initialise the message node
    dup r@ msc>msg>length !       \   Save the translation length in the node
    dup allocate throw            \   Allocate space for the translation
    dup r@ msc>msg>text !         \   Save the pointer in the node
    swap cmove                    \   Save the translation text
    r> r@ hnt-insert              \   Store in the hash table
  THEN
  rdrop
;


: msc-translate  ( c-addr1 u1 msc -- c-addr2 u2 = Translate the message c-addr1 u1 with the catalog, return message if not found )
  >r 2dup r>                      \ Save the message
  hnt-get nil<>? IF               \ Search for the message, if found
    nip nip                       \   Drop old message and 
    dup  msc>msg>text   @         \   .. fetch the translation
    swap msc>msg>length @
  THEN
;


: msc-translate?   ( c-addr1 u2 msc -- c-addr2 u2 true | false = Translate the message c-addr1 u1 with the catalog, return success )
  hnt-get nil<>? IF              \ Search for the message, if found
    dup  msc>msg>text   @        \  Fetch the translation
    swap msc>msg>length @
    true
  ELSE
    false
  THEN
;


: msc-remove  ( c-addr u msc -- flag = Remove the message c-addr u from the catalog, return success )
  hnt-delete nil<>? IF            \ If node is removed Then
    dup msc-msg-free              \   Free text and node
    hnn-free
    true
  ELSE
    false
  THEN
;


( Private words )

: msc-emit-msg  ( msg -- = Emit the message and the translation )
  dup hnn-key@ type 
  ."  -> " 
  dup msc>msg>text @ swap msc>msg>length @ type
  cr
;


( Inspection )

: msc-dump  ( msc -- = Dump the message catalog )
  ." msc:" cr 
  ['] msc-emit-msg swap hnt-execute
;

[THEN]

\ ==============================================================================
