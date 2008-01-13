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
\  $Date: 2008-01-13 08:09:33 $ $Revision: 1.7 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] xos.version [IF]

include ffl/tos.fs

( xos = XML/HTML writer )
( The xos module implements an xml and html writer. The xos module extends   )
( the tos module with extra words, so the xos words work on tos variables.   )
( The module translates the normal entity references: &lt;, &gt;, &quot;,    )
( &amp; and '. All other entity references should be written with the word   )
( xos-write-raw-text. Note: balancing of start and end tags is not checked,  )
( so the module can also be used to write html output.                       )


1 constant xos.version


( Private words )

: xos-write-string   ( c-addr u tos -- = Write the normal xml text c-addr u with entity reference translation )
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

: xos-write-name-attr  ( c-addr1 u1 ... c-addr2n u2n n c-addr u tos -- = Write a xml tag c-addr u with the n attribute names and values )
  >r
  r@ tos-write-string 
  BEGIN
    ?dup
  WHILE                           \ while nr attributes > 0
    >r 2swap r>                   \  swap name and value
    
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


: xos-write-literal   ( c-addr u tos - = Write an optional system or public id literal )
  >r
  ?dup IF               
    bl           r@ tos-write-char
    [char] "     r@ tos-write-char
                 r@ tos-write-string 
    [char] "     r@ tos-write-char
  ELSE
    drop
  THEN
  rdrop
;


: xos-write-markup   ( c-addr u tos - = Write an optional markup )
  >r
  ?dup IF
    bl           r@ tos-write-char
    [char] [     r@ tos-write-char
                 r@ tos-write-string
    [char] ]     r@ tos-write-char
  ELSE
    drop
  THEN
  rdrop
;


( xml writer words )

: xos-write-start-xml   ( c-addr1 u1 ... c-addr2n u2n n tos -- = Write the start of a xml document with n attributes and values )
  >r
  s" <?"  r@ tos-write-string
  s" xml" r@ xos-write-name-attr
  s" ?>"  r> tos-write-string
;


: xos-write-text   ( c-addr u tos -- = Write normal xml text c-addr u with translation to the default entity references )
  xos-write-string
;


: xos-write-start-tag ( c-addr1 u1 ... c-addr2n u2n n c-addr u tos -- = Write the xml start tag c-addr u with n attributes and values c-addr* u* )
  >r
  [char] < r@ tos-write-char
           r@ xos-write-name-attr
  [char] > r> tos-write-char
;


: xos-write-end-tag ( c-addr u tos -- = Write the xml end tag c-addr u )
  >r
  [char] < r@ tos-write-char
  [char] / r@ tos-write-char
           r@ tos-write-string
  [char] > r> tos-write-char
;


: xos-write-empty-element   ( c-addr1 u1 ... c-addr2n u2n n c-addr u tos -- = Write the xml start and end tag c-addr u with n attributes and values c-addr* u*)
  >r
  [char] < r@ tos-write-char
           r@ xos-write-name-attr
  [char] / r@ tos-write-char
  [char] > r> tos-write-char
;


: xos-write-raw-text ( c-addr u tos -- = Write unprocessed xml text )
  tos-write-string
;


: xos-write-comment ( c-addr u tos -- = Write a xml comment )
  >r
  s" <!--" r@ tos-write-string
           r@ tos-write-string
  s" -->"  r> tos-write-string
;


: xos-write-cdata   ( c-addr u tos -- = Write a xml CDATA section )
  >r
  s" <![CDATA[" r@ tos-write-string
                r@ tos-write-string
  s" ]]>"       r> tos-write-string
;


: xos-write-proc-instr ( c-addr1 u1 c-addr2n u2n n c-addr u tos -- = Write a xml processing instruction with target c-addr u and n attributes and values c-addr* u* )
  >r
  s" <?" r@ tos-write-string
         r@ xos-write-name-attr
  s" ?>" r> tos-write-string
;


: xos-write-internal-dtd   ( c-addr1 u1 c-addr2 u2 tos -- = Write an internal document type definition with name c-addr2 u2 and markup c-addr1 u1 )
  >r
  s" <!DOCTYPE " r@ tos-write-string
                 r@ tos-write-string
                 r@ xos-write-markup
  [char] >       r> tos-write-char
;


: xos-write-system-dtd   ( c-addr1 u1 c-addr2 u2 c-addr3 u3 tos -- = Write a system document type definition with name c-addr3 u3, markup c-addr2 u2 and system c-addr1 u1 )
  >r
  s" <!DOCTYPE " r@ tos-write-string
                 r@ tos-write-string
  s"  SYSTEM"    r@ tos-write-string
  2swap  
                 r@ xos-write-literal   \ Write system id
                 r@ xos-write-markup
  [char] >       r> tos-write-char               
;


: xos-write-public-dtd   ( c-addr1 u1 c-addr2 u2 c-addr3 u3 c-addr4 u4 tos -- = Write a public document type definition with name c-addr4 u4, markup c-addr3 u3, public-id c-addr2 u2 and system c-addr1 u1 )
  >r
  s" <!DOCTYPE " r@ tos-write-string
                 r@ tos-write-string
  s"  PUBLIC"    r@ tos-write-string
  2swap
                 r@ xos-write-literal    \ Write public id
  2swap
                 r@ xos-write-literal    \ Write system id
                 r@ xos-write-markup
  [char] >       r> tos-write-char               
;

[THEN]

\ ==============================================================================
