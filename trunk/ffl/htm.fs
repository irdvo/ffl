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
\  $Date: 2007-09-18 05:27:26 $ $Revision: 1.3 $
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

0 constant htm.error                   ( Error         - ..                  )
1 constant htm.done                    ( Done reading  -                     )
2 constant htm.comment                 ( Comment       - c-addr u            )
3 constant htm.text                    ( Normal text   - c-addr u            )
4 constant htm.start-tag               ( Start tag     - c-addr u            )
5 constant htm.tag-attribute           ( Tag attribute - c-addr u c-addr u   )
6 constant htm.end-tag                 ( End tag       - c-addr u            )
7 constant htm.markup                  ( Markup        - c-addr u            )
8 constant htm.markup-parameter        ( Markup param. - c-addr u            )
9 constant htm.processing-instruction  ( Proc. instr.  - c-addr u c-addr u   )


( Html reader and writer structure )

struct: htm%   ( - n = Get the required space for the html reader data structure )
  str%
  field: htm>string     \ the html string (extends the normal string)
  cell:  htm>offset     \ the offset in the html string
  cell:  htm>state      \ the current state
  cell:  htm>reader     \ the xt of the reader (pull)
  cell:  htm>writer     \ the xt of the writer
  cell:  htm>data       \ the optional data for the reader and writer
;struct



( Html parser structure creation, initialisation and destruction )

: htm-init   ( w:htm - = Initialise the html parser structure )
  dup  htm>string  str-init
  dup  htm>offset  0!
  htm.done
  over htm>state  !
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
  str-free
;


( Html reader init words )

: htm-set-reader  ( w:data xt w:htm - = Init the html reader for reading using the reader callback )
  >r
  r@ htm>reader !
  r@ htm>data   !
  r@ htm>string str-clear
  r@ htm>offset 0!
  htm.text
  r> htm>state  !
;


: htm-set-string  ( c-addr u w:htm - = Init the html reader for for reading from string )
  >r
  r@ htm>string str-set
  r@ htm>offset 0!
  r@ htm>reader nil!
  r@ htm>data   nil!
  htm.text
  r> htm>state  !
;


( Private reader words )

: htm-fetch-char?  ( w:htm - c true | false = Fetch the next character )
  >r
  r@ htm>offset @  r@ htm>string
  2dup str-offset? 0= IF
    r@ htm>reader @ dup nil<> IF
      r@ htm>data @ swap execute IF
        r@ htm>string str-append-string
      THEN
    ELSE
      drop
    THEN
  THEN
  
  2dup str-offset? IF
    str-data@ swap chars + c@
    true
  ELSE
    2drop
    false
  THEN
  rdrop
;


: htm-next-char  ( w:htm - = Move to the next character )
  htm>offset 1+!
;


: htm-prev-char  ( w:htm - = Move to the previous character )
  htm>offset 1-!
;


: htm-read-name ( w:htm - c-addr u w:htm = Read a name from the source )
  dup  htm>string str-data@       \ Save current string position
  over htm>offset @ chars +
  swap 1
  BEGIN                           \ Read the tag name
    over htm-next-char
    over htm-fetch-char? IF
      dup chr-space? over [char] > = OR swap [char] = = OR 0=
    ELSE
      false
    THEN
  WHILE
    1+
  REPEAT
  swap
;


: htm-skip-spaces  ( w:htm - w:htm = Skip spaces in the source )
  BEGIN                           \ Skip trailing spaces
    dup htm-fetch-char? IF
      chr-space?
    ELSE
      false
    THEN
  WHILE
    dup htm-next-char
  REPEAT
;


: htm-read-comment ( .. )
;


: htm-read-markup ( .. )
;


: htm-read-markup-parameter ( .. )
;


: htm-read-start-tag ( w:htm - c-addr u htm.start-tag = Read a start tag )
  htm-read-name
  htm-skip-spaces  
  
  dup htm-fetch-char? IF          \ Check for > or attributes
    dup [char] > = IF
      drop
      dup htm-next-char
    ELSE
      chr-alpha? IF
        htm.tag-attribute over htm>state !
      ELSE
        \ error
      THEN
    THEN
  THEN
  drop  
  htm.start-tag
;


: htm-read-tag-attribute ( w:htm - c-addr u c-addr u htm.tag-attribute = Read a tag attribute )
  htm-read-name
  htm-skip-spaces
  
  dup htm-fetch-char? IF
    dup [char] = = IF
      drop
      dup htm-skip-spaces
      \ to be cont.
    ELSE
      dup chr-alpha? IF
        2drop
        nil 0
      ELSE
        [char] > = IF
          drop
          nil 0
        ELSE
          \ error
        THEN
      THEN
    THEN
  THEN
  htm.tag-attribute
;


: htm-read-end-tag ( w:htm - c-addr u htm.end-tag = Read an end tag )
  htm-read-name
  htm-skip-spaces
  
  dup htm-fetch-char? IF          \ Check for > 
    [char] > = IF
      dup htm-next-char
    ELSE
       \ error !!
    THEN
  THEN
  drop  
  htm.end-tag
;


: htm-read-processing-instruction ( .. )
;


: htm-read-text ( w:htm - c-addr u htm.text = Read normal html text )
  dup  htm>string str-data@ 
  over htm>offset @ chars +
  swap 1                           \ S: c-addr htm count
  BEGIN
    over htm-next-char
    over htm-fetch-char? IF
      [char] < <>
    ELSE
      false
    THEN
  WHILE
    1+
  REPEAT
  nip
  htm.text
;


: htm-do-text ( w:htm - .. = Scan in text )
  .s
  >r
  r@ htm-fetch-char? IF
    [char] < = IF
      r@ htm-next-char
      
      r@ htm-fetch-char? IF
        dup chr-alpha? IF
          drop 
          r@ htm-read-start-tag
        ELSE
          dup [char] / = IF
            drop 
            r@ htm-next-char
            r@ htm-fetch-char? IF
              chr-alpha? IF
                r@ htm-read-end-tag
              ELSE
                r@ htm-prev-char
                r@ htm-prev-char
                r@ htm-read-text
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
                r@ htm-prev-char
                r@ htm-read-text
              THEN
            THEN
          THEN
        THEN
      ELSE
        htm.done
      THEN
    ELSE
      r@ htm-read-text
    THEN
  ELSE
    htm.done
  THEN
  rdrop
;


( Html reader word )

: htm-read ( w:htm - htm.done | c-addr u htm.text | c-addr u htm.start-tag | c-addr u htm.end-tag | ..  = Read the next html token from the source )
  dup htm>state @
  CASE
    htm.done OF
      drop htm.done
      ENDOF
    htm.tag-attribute OF
      htm-read-tag-attribute
      ENDOF
    htm.markup-parameter OF
      htm-read-markup-parameter
      ENDOF
    htm.comment OF
      htm-read-comment
      ENDOF
      
    >r htm-do-text r>             \ default: check text
  ENDCASE
;



( Html writer init words )

: htm-set-writer ( w:data xt w:htm - = Init the html writer for writing using the writer callback )
;


: htm-clear-string ( w:htm - = Init the html writer for writing to string )
;


( Html writer words )

: htm-write-markup ( c-addr u w:htm - = Write a HTML markup )
;


: htm-write-markup-parameter ( c-addr u w:htm - = Write a markup parameter )
;

: htm-write-start-tag ( c-addr u w:htm - = Write the HTML start tag )
;


: htm-write-tag-attribute ( c-addr u c-addr u w:htm - = Write a HTML tag attribute )
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
