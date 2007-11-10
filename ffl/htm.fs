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
\  $Date: 2007-11-10 07:20:08 $ $Revision: 1.5 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] htm.version [IF]

include ffl/tis.fs

( htm = HTML reader and writer )
( The htm module implements words for non-validating parsing of HTML sources )
( and for writing HTML.                                                      )
( Some notes: the default entity references, &lt; &gt; &amp; and &quot;, are )
( automatically translated, but all others are simply returned in the text.  )
( For the writer counts: htm-write-text will translate the default entity    )
( references. For all others use the word htm-write-entity-reference.        )
( <pre>                                                                      )
(   Stack usage reader word: htm-reader ( w:data - false | c-addr u true     )
(   Stack usage writer word: htm-writer ( c-addr u w:data - f                )
( </pre>                                                                     )


1 constant htm.version


( Html reader constants )

-1 constant htm.error                   ( - n = Error         -                                 )
 0 constant htm.done                    ( - n = Done reading  -                                 )
 1 constant htm.comment                 ( - n = Comment       - c-addr u                        )
 2 constant htm.text                    ( - n = Normal text   - c-addr u                        )
 3 constant htm.start-tag               ( - n = Start tag     - c-addr u c-addr u .. n c-addr u )
 4 constant htm.end-tag                 ( - n = End tag       - c-addr u                        )
 5 constant htm.markup                  ( - n = Markup        - c-addr u .. n c-addr u          )
 6 constant htm.processing-instruction  ( - n = Proc. instr.  - c-addr u .. n c-addr u          )

( Html reader and writer structure )

struct: htm%   ( - n = Get the required space for the html reader data structure )
  tis%
  field: htm>tis        \ the html string (extends the text input stream)
  cell:  htm>data       \ the data for the writer
  cell:  htm>writer     \ the xt of the writer
;struct



( Html parser structure creation, initialisation and destruction )

: htm-init   ( w:htm - = Initialise the html parser structure )
  
  dup  htm>tis     tis-init
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
  str-free
;


( Html reader init words )

: htm-set-reader  ( w:data xt w:htm - = Init the html reader for reading using the reader callback )
  htm>tis tis-set-reader
;


: htm-set-string  ( c-addr u w:htm - = Init the html reader for for reading from string )
  htm>tis tis-set
;


( Private reader words )

: htm-read-reference   ( w:tis - n = Read and translate the reference )
  >r
  s" lt;" r@ tis-imatch-string IF
    \ ToDo
    ." <"
  ELSE  s" gt;" r@ tis-imatch-string IF
    \ ToDo
    ." >"
  ELSE s" amp;" r@ tis-imatch-string IF
    \ ToDo
    ." &"
  THEN THEN THEN
  rdrop
  0
;


: htm-read-comment ( .. )
;


: htm-read-markup ( .. )
;


: htm-read-markup-parameter ( .. )
;


: htm-read-tag-attribute ( w:tis - c-addr u c-addr u = Read a tag attribute )
;


: htm-read-start-tag ( w:tis - c-addr u c-addr u .. n c-addr u htm.start-tag = Read a start tag )
;


: htm-read-end-tag ( w:tis - c-addr u htm.end-tag = Read an end tag )
;


: htm-read-processing-instruction ( w:tis -  .. = Read a processing instruction )
;


: htm-read-tag   ( w:tis - .. = Read a tag )
  >r
  r@ tis-fetch-char IF
    dup chr-alpha? IF
      drop 
      r@ htm-read-start-tag
    ELSE
      dup [char] / = IF
        drop 
        r@ tis-next-char
        r@ tis-fetch-char IF
          chr-alpha? IF
            r@ htm-read-end-tag
          ELSE
            \ What to do ?
          THEN
        ELSE
          htm.done
        THEN    
      ELSE
        dup [char] ! = IF
          drop 
          r@ htm-read-markup
        ELSE
          [char] ? = IF
            r@ htm-read-processing-instruction
          ELSE
            \ What to do ?
          THEN
        THEN
      THEN
    THEN
  ELSE
    htm.done
  THEN
  rdrop
;


: htm-read-text   ( w:tis - c-addr u f | 0 f = Read the text, process the entity references )
  >r
  s" <&" r@ tis-scan-chars IF               \ Scan for <&
    [char] < = IF
      -1 r@ tis-pntr+! drop
      true
    ELSE
      r@ tis-do-entity +
      false
    THEN
  ELSE
    r@ tis-read-all
    true
  THEN
  rdrop
;


( Html reader word )

: htm-read ( w:htm - ... htm.xxx = Read the next html token with data from the source [see html reader constants] )
  htm>tis >r
  
  r@ tis-reduce                             \ Keep the stream compact
  
  r@ tis-eof? IF
    htm.done                                \ Done if no more data
  ELSE
    [char] < r@ tis-cmatch-char IF
      r@ htm-read-tag                       \ Read tag if first character is '<'
    ELSE
      r@ htm-read-text 0= IF                \ Read text
        BEGIN
          r@ htm-read-text                  \ If not done, continu reading text
          >r 
          ?dup IF
            nip +
          THEN
          r>
        UNTIL
      THEN
      
      ?dup IF                               \ If text read Then
        htm.text                            \   Text processed
      ELSE                                  \ Else
        drop                                \   Done
        htm.done
      THEN
    THEN
  THEN
  rdrop
;


( Html writer init words )

: htm-set-writer ( w:data xt w:htm - = Initialise the html writer for writing using the writer callback )
;


: htm-clear-string ( w:htm - = Initialise the html writer for writing to string )
;


( Html writer words )

: htm-write-markup ( c-addr u ... n c-addr u w:htm - = Write a HTML markup with n parameters )
;


: htm-write-start-tag ( c-addr u c-addr u ... n c-addr u w:htm - = Write a HTML start tag with n attributes )
;


: htm-write-end-tag ( c-addr u w:htm - = Write a HTML end tag )
;


: htm-write-processing-instruction ( c-addr u ... n c-addr u w:htm - = Write a HTML processing instruction )
;


: htm-write-text ( c-addr u w:htm - = Write normal HTML text )
;


: htm-write-comment ( c-addr u w:htm - = Write HTML comment )
;


: htm-write-entity-reference ( c-addr u u:htm - = Write a HTML entity reference )
;


( Html writer finish words )

: htm-get-string ( w:htm - c-addr u = Get the html string when writing to string )
;

( Inspection )

: htm-dump ( w:htm - = Dump the html structure )
;

[THEN]

\ ==============================================================================
