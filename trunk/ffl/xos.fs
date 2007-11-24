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
\  $Date: 2007-11-24 18:43:21 $ $Revision: 1.2 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] xos.version [IF]

include ffl/tos.fs

( xos = XML/HTML writer )
( The xos module implements words for writing xml and html to an output      )
( stream. The xos module extends the tos module, so the xos words words on   )
( tos variables.                                                             )


1 constant xos.version


( Private words )

: xos-write-string   ( c-addr u w:tos - = Write normal xml text )
  -rot
  bounds ?DO
    I c@ 
    CASE
      [char] < OF dup s" &lt;"   rot tos-write-string ENDOF
      [char] > OF dup s" &gt;"   rot tos-write-string ENDOF
      [char] " OF dup s" &quot;" rot tos-write-string ENDOF
      [char] & OF dup s" &amp;"  rot tos-write-string ENDOF
      [char] ' OF dup s" &apos;" rot tos-write-string ENDOF
      2dup swap tos-write-char
    ENDCASE
  1 chars +LOOP
  drop
;

: xos-write-starting-tag  ( c-addr u c-addr u ... n c-addr u w:tos - = Write a xml starting tag with n attributes )
  >r
  [char] <  r@ tos-write-char
            r@ tos-write-string 
  BEGIN
    ?dup
  WHILE                           \ while nr attributes > 0
    bl r@ tos-write-char
    
    -rot
       r@ tos-write-string
    
    -rot
    ?dup IF
      [char] = r@ tos-write-char
      [char] " r@ tos-write-char
               r@ xos-write-string
      [char] " r@ tos-write-char
    ELSE
      drop
    THEN
    1-
  REPEAT
  rdrop
;

( xml writer words )

: xos-write-start-document   ( c-addr:standalone u c-addr u c-addr u w:tos - = Write the start of a xml document )
  >r
  s" <?xml version=" r@ tos-write-string
  r@ tos-write-string
  [char] " r@ tos-write-char
  
  ?dup IF                         \ Optional encoding
    s" encoding=" r@ tos-write-string
    [char] " r@ tos-write-char
    r@ tos-write-string
    [char] " r@ tos-write-char
  ELSE
    drop
  THEN
  
  ?dup IF                         \ Optional standalone
    s" standalone=" r@ tos-write-string
    [char] " r@ tos-write-char
    r@ tos-write-string
    [char] " r@ tos-write-char
  ELSE
    drop
  THEN
  s" ?>" r> tos-write-string
;


: xos-write-text   ( c-addr u w:tos - = Write normal xml text )
  xos-write-string
;


: xos-write-start-tag ( c-addr u c-addr u ... n c-addr u w:tos - = Write a xml start tag with n attributes )
  >r
  r@ xos-write-starting-tag
  [char] > r> tos-write-char
;


: xos-write-end-tag ( c-addr u w:tos - = Write a xml end tag )
  >r
  [char] < r@ tos-write-char
  [char] / r@ tos-write-char
           r@ tos-write-string
  [char] > r> tos-write-char
;


: xos-write-start-end-tag   ( c-addr u c-addr u ... n c-addr u w:tos - = Write a xml start and end tag with n attributes )
  >r
  r@ xos-write-starting-tag
  [char] / r@ tos-write-char
  [char] > r> tos-write-char
;


: xos-write-raw-text ( c-addr u w:tos - = Write unprocessed xml text )
  tos-write-string
;


: xos-write-comment ( c-addr u w:tos - = Write xml comment )
  >r
  s" <--" r@ tos-write-string
          r@ tos-write-string
  s" -->" r> tos-write-string
;


: xos-write-markup ( c-addr u ... n c-addr u w:tos - = Write a xml markup with n parameters )
;


: xos-write-proc-instr ( c-addr u ... n c-addr u w:tos - = Write a xml processing instruction )
;


: xos-write-dtd   ( c-addr u c-addr u w:tos - = Write a document type definition )
;

[THEN]

\ ==============================================================================
