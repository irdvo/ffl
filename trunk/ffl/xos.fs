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
\  $Date: 2007-12-02 18:53:28 $ $Revision: 1.4 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] xos.version [IF]

include ffl/tos.fs

( xos = XML/HTML writer )
( The xos module implements words for writing xml and html to an output      )
( stream. The xos module extends the tos module, so the xos words works on   )
( tos variables. The module translates the normal entity references: &lt;,   )
( &gt;, &quot;, &amp; and '. All other entity references should be written   )
( with the xos-write-raw-text word. Note: balancing of start and end tags    )
( is not checked, so the module can also be used to write html output.       )


1 constant xos.version


( Private words )

: xos-write-string   ( c-addr u w:tos - = Write normal xml text with entity reference translation )
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

: xos-write-name-attr  ( c-addr u c-addr u ... n c-addr u w:tos - = Write a xml name with n attributes )
  >r
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

: xos-write-start-xml   ( c-addr:standalone u c-addr:encoding u c-addr:version u w:tos - = Write the start of a xml document )
  >r
  s" <?xml version=" r@ tos-write-string
  [char] " r@ tos-write-char
           r@ tos-write-string
  [char] " r@ tos-write-char
  
  ?dup IF                         \ Optional encoding
    s"  encoding=" r@ tos-write-string
    [char] " r@ tos-write-char
    r@ tos-write-string
    [char] " r@ tos-write-char
  ELSE
    drop
  THEN
  
  ?dup IF                         \ Optional standalone
    s"  standalone=" r@ tos-write-string
    [char] " r@ tos-write-char
    r@ tos-write-string
    [char] " r@ tos-write-char
  ELSE
    drop
  THEN
  s" ?>" r> tos-write-string
;


: xos-write-text   ( c-addr u w:tos - = Write normal xml text with translation to the default entity references )
  xos-write-string
;


: xos-write-start-tag ( c-addr u c-addr u ... n c-addr u w:tos - = Write a xml start tag with n attributes )
  >r
  [char] < r@ tos-write-char
           r@ xos-write-name-attr
  [char] > r> tos-write-char
;


: xos-write-end-tag ( c-addr u w:tos - = Write a xml end tag )
  >r
  [char] < r@ tos-write-char
  [char] / r@ tos-write-char
           r@ tos-write-string
  [char] > r> tos-write-char
;


: xos-write-empty-element   ( c-addr u c-addr u ... n c-addr u w:tos - = Write a xml start and end tag with n attributes )
  >r
  [char] < r@ tos-write-char
           r@ xos-write-name-attr
  [char] / r@ tos-write-char
  [char] > r> tos-write-char
;


: xos-write-raw-text ( c-addr u w:tos - = Write unprocessed xml text )
  tos-write-string
;


: xos-write-comment ( c-addr u w:tos - = Write xml comment )
  >r
  s" <!--" r@ tos-write-string
          r@ tos-write-string
  s" -->" r> tos-write-string
;


: xos-write-cdata   ( c-addr u w:tos - = Write a xml CDATA section )
  >r
  s" <![CDATA[" r@ tos-write-string
                r@ tos-write-string
  s" ]]>"       r> tos-write-string
;


: xos-write-proc-instr ( ...  c-addr:value u c-addr:name u n c-addr:target u w:tos - = Write a xml processing instruction )
  >r
  s" <?" r@ tos-write-string
         r@ xos-write-name-attr
  s" ?>" r> tos-write-string
;


: xos-write-internal-dtd   ( c-addr:markup u c-addr:name u w:tos - = Write an internal document type definition )
  >r
  s" <!DOCTYPE " r@ tos-write-string
                 r@ tos-write-string
  s"  ["         r@ tos-write-string
                 r@ tos-write-string   \ Write markup
  s" ]>"         r> tos-write-string
;


: xos-write-system-dtd   ( c-addr:system u c-addr:name u w:tos - = Write a system document type definition )
  >r
  s" <!DOCTYPE " r@ tos-write-string
                 r@ tos-write-string
  s"  SYSTEM "   r@ tos-write-string
  [char] "       r@ tos-write-char
                 r@ tos-write-string   \ Write system
  [char] "       r@ tos-write-char
  [char] >       r> tos-write-char               
;


: xos-write-public-dtd   ( c-addr:system u c-addr:publicid u c-addr:name u w:tos - = Write a public document type definition )
  >r
  s" <!DOCTYPE " r@ tos-write-string
                 r@ tos-write-string
  s"  PUBLIC "   r@ tos-write-string
  [char] "       r@ tos-write-char
                 r@ tos-write-string   \ Write public id
  [char] "       r@ tos-write-char
  ?dup IF               
    bl           r@ tos-write-char
    [char] "     r@ tos-write-char
                 r@ tos-write-string   \ Write system
    [char] "     r@ tos-write-char
  ELSE
    drop
  THEN
  [char] >       r> tos-write-char               
;

[THEN]

\ ==============================================================================
