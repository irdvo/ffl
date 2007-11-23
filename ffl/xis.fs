\ ==============================================================================
\
\                  xis - the xml reader in the ffl
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


[UNDEFINED] xis.version [IF]

include ffl/tis.fs

( xis = XML reader )
( The xis module implements words for non-validating parsing of XML sources. )
( Due to the non-validating nature of the reader it can also parse HTML.     )
( Some notes: the default entity references, &lt; &gt; &amp; and &quot;, are )
( automatically translated, but all others are simply returned in the text.  )
( <pre>                                                                      )
(   Stack usage reader word: xis-reader ( w:data - false | c-addr u true     )
( </pre>                                                                     )


1 constant xis.version


( xisl reader constants )

-1 constant xis.error                   ( - n = Error         -                                 )
 0 constant xis.done                    ( - n = Done reading  -                                 )
 1 constant xis.comment                 ( - n = Comment       - c-addr u                        )
 2 constant xis.text                    ( - n = Normal text   - c-addr u                        )
 3 constant xis.start-tag               ( - n = Start tag     - c-addr u c-addr u .. n c-addr u )
 4 constant xis.end-tag                 ( - n = End tag       - c-addr u                        )
 5 constant xis.markup                  ( - n = Markup        - c-addr u .. n c-addr u          )
 6 constant xis.processing-instruction  ( - n = Proc. instr.  - c-addr u .. n c-addr u          )

( xisl reader and writer structure )

struct: xis%   ( - n = Get the required space for the xisl reader data structure )
  tis%
  field: xis>tis        \ the xis reader (extends the text input stream)
;struct



( XML parser structure creation, initialisation and destruction )

: xis-init   ( w:xis - = Initialise the xml parser structure )
  
  dup  xis>tis     tis-init
  dup  xis>writer  nil!
       xis>data    nil!
;


: xis-create   ( C: "name" -  R: - w:xis = Create a named xml parser variable in the dictionary )
  create  here xis% allot  xis-init
;


: xis-new   ( - w:xis = Create a new xml parser variable on the heap )
  xis% allocate  throw   dup xis-init
;


: xis-free   ( w:xis - = Free the xis variable from the heap )
  str-free
;


( xml reader init words )

: xis-set-reader  ( w:data xt w:xis - = Init the xml reader for reading using the reader callback )
  xis>tis tis-set-reader
;


: xis-set-string  ( c-addr u w:xis - = Init the xml reader for for reading from string )
  xis>tis tis-set
;


( Private reader words )

: xis-read-reference   ( w:tis - n = Read and translate the reference )
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


: xis-read-comment ( .. )
;


: xis-read-markup ( .. )
;


: xis-read-markup-parameter ( .. )
;


: xis-read-tag-attribute ( w:tis - c-addr u c-addr u = Read a tag attribute )
;


: xis-read-start-tag ( w:tis - c-addr u c-addr u .. n c-addr u xis.start-tag = Read a start tag )
;


: xis-read-end-tag ( w:tis - c-addr u xis.end-tag = Read an end tag )
;


: xis-read-processing-instruction ( w:tis -  .. = Read a processing instruction )
;


: xis-read-tag   ( w:tis - .. = Read a tag )
  >r
  r@ tis-fetch-char IF
    dup chr-alpha? IF
      drop 
      r@ xis-read-start-tag
    ELSE
      dup [char] / = IF
        drop 
        r@ tis-next-char
        r@ tis-fetch-char IF
          chr-alpha? IF
            r@ xis-read-end-tag
          ELSE
            \ What to do ?
          THEN
        ELSE
          xis.done
        THEN    
      ELSE
        dup [char] ! = IF
          drop 
          r@ xis-read-markup
        ELSE
          [char] ? = IF
            r@ xis-read-processing-instruction
          ELSE
            \ What to do ?
          THEN
        THEN
      THEN
    THEN
  ELSE
    xis.done
  THEN
  rdrop
;


: xis-read-text   ( w:tis - c-addr u f | 0 f = Read the text, process the entity references )
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


( xml reader word )

: xis-read ( w:xis - ... xis.xxx = Read the next xml token with data from the source [see xml reader constants] )
  xis>tis >r
  
  r@ tis-reduce                             \ Keep the stream compact
  
  r@ tis-eof? IF
    xis.done                                \ Done if no more data
  ELSE
    [char] < r@ tis-cmatch-char IF
      r@ xis-read-tag                       \ Read tag if first character is '<'
    ELSE
      r@ xis-read-text 0= IF                \ Read text
        BEGIN
          r@ xis-read-text                  \ If not done, continu reading text
          >r 
          ?dup IF
            nip +
          THEN
          r>
        UNTIL
      THEN
      
      ?dup IF                               \ If text read Then
        xis.text                            \   Text processed
      ELSE                                  \ Else
        drop                                \   Done
        xis.done
      THEN
    THEN
  THEN
  rdrop
;


( Inspection )

: xis-dump ( w:xis - = Dump the xml reader variable )
;

[THEN]

\ ==============================================================================
