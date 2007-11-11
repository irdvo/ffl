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
\  $Date: 2007-11-11 19:09:45 $ $Revision: 1.2 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] msc.version [IF]

include ffl/hnt.fs

( msc = Message Catalog )
( The msc module implements words for building and using a message catalog.  )
( The catalog is used to translates a message to another message. It can be  )
( used for internationalization of messages and for converting messages. The )
( module supports reading gettexts mo-files.                                 )


1 constant msc.version


( Private structure )

struct: msc%msg%   ( - n = Get the required space the message node structure )
  hnn%
  field: msc>msg>node        \ Hash table node
  cell:  msc>msg>length      \ Translation length
  cell:  msc>msg>text        \ Translation text
;struct


: msc-msg-free   ( w:msg - = Free the text in the node )
  msc>msg>text @ free throw
;


( Catalog structure )

struct: msc%   ( - n = Get the required space for the msc data structure )
  hnt%
  field: msc>table           \ Hash table
  cell:  msc>mo1
  cell:  msc>mo2
;struct


( Catalog structure creation, initialisation and destruction )

: msc-init   ( w:msc - = Initialise the msc structure )
  10 over hnt-init
  dup msc>mo1 0!
  msc>mo2 0!
;


: msc-create   ( C: "name" -  R: - w:msc = Create a named message catalog variable in the dictionary )
  create  here msc% allot  msc-init
;


: msc-new   ( - w:msc = Create a new message catalog variable on the heap )
  msc% allocate  throw   dup msc-init
;


: msc-free   ( w:msc - = Free the message catalog variable from the heap )
  ['] msc-msg-free over hnt-execute    \ Free the texts in the nodes
  
  hnt-free                             \ Free the nodes and the tree
;


( Catalog words )

: msc-add  ( c-addr:msg u c-addr:translation u w:msc - = Add a msg and translation to the catalog )
  >r
  2swap
  2dup r@ hnt-search              \ Search for the key in the table
  dup nil<> IF                    \ If already in hash table Then
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
    drop
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


: msc-translate  ( c-addr u w:msc - c-addr u = Translate a message with the catalog, return message if not found )
  >r 2dup r>                      \ Save the message
  hnt-get dup nil<> IF            \ Search for the message, if found
    nip nip                       \   Drop old message and 
    dup  msc>msg>text   @         \   .. fetch the translation
    swap msc>msg>length @
  ELSE                            \ Else 
    drop                          \   Use the old message
  THEN
;


: msc-remove  ( c-addr u w:msc - f = Remove a message from the catalog )
  hnt-delete dup nil<> IF         \ If node is removed Then
    dup msc-msg-free              \   Free text and node
    hnn-free
    true
  ELSE
    drop
    false
  THEN
;


( MO-file words )

: msc-read  ( c-addr u w:msc - ?? = Add the contents of the mo-file to the catalog )
;


( Private words )

: msc-emit-msg  ( w:msg - = Emit the message and the translation )
  dup hnn-key@ type 
  ."  -> " 
  dup msc>msg>text @ swap msc>msg>length @ type
  cr
;


( Inspection )

: msc-dump  ( w:msc - = Dump the catalog )
  ." msc:" cr 
  ['] msc-emit-msg swap hnt-execute
;

[THEN]

\ ==============================================================================
