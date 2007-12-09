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
\  $Date: 2007-12-09 07:23:17 $ $Revision: 1.3 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] xis.version [IF]

include ffl/tis.fs
include ffl/msc.fs
include ffl/est.fs


( xis = XML/HTML reader )
( The xis module implements a non-validating XML/HTML parser.                )
( Some notes: the default entity references, &lt; &gt; &amp; and &quot;, are )
( automatically translated, but all others are simply returned in the text.  )
( <pre>                                                                      )
(   Stack usage reader word: xis-reader ( w:data - false | c-addr u true     )
( </pre>                                                                     )


1 constant xis.version


( private variable )

msc-create xis.entities  ( the default entity reference catalog )

s" lt"   s" <"   xis.entities msc-add
s" gt"   s" >"   xis.entities msc-add
s" amp"  s" &"   xis.entities msc-add
s" quot" s\" \"" xis.entities msc-add
s" apos" s" '"   xis.entities msc-add


( xis reader constants )

-1 constant xis.error          ( -- n = Error          --                                                        )
 0 constant xis.done           ( -- n = Done reading   --                                                        )
 1 constant xis.start-xml      ( -- n = Start Document -- c-addr:standalone u c-addr:encoding u c-addr:version u )
 1 constant xis.comment        ( -- n = Comment        -- c-addr u                                               )
 2 constant xis.text           ( -- n = Normal text    -- c-addr u                                               )
 3 constant xis.start-tag      ( -- n = Start tag      -- c-addr:value u c-addr:attribute u .. n c-addr:tag u    )
 4 constant xis.end-tag        ( -- n = End tag        -- c-addr u                                               )
 5 constant xis.empty-element  ( -- n = Empty element  -- c-addr:value u c-addr:attribute u .. n c-addr:tag u    )
 6 constant xis.cdata          ( -- n = CDATA section  -- c-addr u                                               )
 6 constant xis.proc-instr     ( -- n = Proc. instr.   -- c-addr:value u c-addr:attribute u .. n c-addr:target u )
 7 constant xis.internal-dtd   ( -- n = Internal DTD   -- c-addr:markup u c-addr:name u                          )
 8 constant xis.public-dtd     ( -- n = Public DTD     -- c-addr:system u c-addr:publicid u c-addr:name u        )
 9 constant xis.system-dtd     ( -- n = System DTD     -- c-addr:system u c-addr:name u                          )
 
 
( xml reader structure )

begin-structure xis%   ( -- n = Get the required space for a xis reader variable )
  tis%
  +field  xis>tis       \ the xis reader (extends the text input stream)
  field:  xis>msc       \ translation of entity references
end-structure



( xml reader variable creation, initialisation and destruction )

: xis-init   ( xis -- = Initialise the xml reader variable )
  
               dup  xis>tis  tis-init
  xis.entities swap xis>msc  !
;


: xis-create   ( "<spaces>name" -- ; -- xis = Create a named xml reader variable in the dictionary )
  create  here xis% allot  xis-init
;


: xis-new   ( -- xis = Create a new xml reader variable on the heap )
  xis% allocate  throw   dup xis-init
;


: xis-free   ( xis -- = Free the xis reader variable from the heap )
  str-free
;


( xml reader init words )

: xis-set-reader  ( x xt xis -- = Init the xml reader for reading using the reader callback xt with its data x )
  xis>tis tis-set-reader
;


: xis-set-string  ( c-addr u xis -- = Init the xml reader for for reading from the string c-addr u )
  xis>tis tis-set
;


( Entity reference catalog words )

: xis-msc@   ( xis -- msc = Get the current entity reference catalog )
  xis>msc @
;


: xis-msc!   ( msc xis -- = Set the entity reference catalog for the reader )
  over nil= exp-invalid-parameters AND throw
  
  xis>msc !
;


( Private reader words )

: xis-read-reference   ( tis -- ... = Read and translate the reference )
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


: xis-read-tag-attribute ( tis -- c-addr u c-addr u = Read a tag attribute )
;


: xis-read-start-tag ( tis -- c-addr u c-addr u .. n c-addr u xis.start-tag = Read a start tag )
;


: xis-read-end-tag ( tis -- c-addr u xis.end-tag = Read an end tag )
;


: xis-read-proc-instr ( tis --  .. = Read a processing instruction )
;


: xis-read-markup   ( tis -- .. = Read markup text )
;


: xis-read-tag   ( tis -- .. = Read a tag )
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
            r@ xis-read-proc-instr
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


: xis-read-text   ( tis -- c-addr u flag | 0 flag = Read the text, process the entity references )
  >r
  s" <&" r@ tis-scan-chars IF               \ Scan for < or &
    [char] < = IF
      -1 r@ tis-pntr+! drop
      true
    ELSE
      r@ xis-read-reference +
      false
    THEN
  ELSE
    r@ tis-read-all
    true
  THEN
  rdrop
;


( xml reader word )

: xis-read ( xis -- i*x n = Read the next xml token n with various parameters from the source [see xml reader constants] )
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

: xis-dump ( xis -- = Dump the xml reader variable )
;

[THEN]

\ ==============================================================================
