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
\  $Date: 2007-12-13 20:09:03 $ $Revision: 1.5 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] xis.version [IF]

include ffl/enm.fs
include ffl/tis.fs
include ffl/msc.fs
include ffl/est.fs
include ffl/chs.fs


( xis = XML/HTML reader )
( The xis module implements a non-validating XML/HTML parser.                )
( Some notes: the default entity references, &lt; &gt; &amp;, &quot; and '   )
( are automatically translated, but all others are simply returned in the    )
( text.                                                                      )
( <pre>                                                                      )
(   Stack usage reader word: xis-reader ( w:data - false | c-addr u true     )
( </pre>                                                                     )


1 constant xis.version


( private constants )

msc-create xis.entities  ( the default entity reference catalog )

s" lt"   s" <"   xis.entities msc-add
s" gt"   s" >"   xis.entities msc-add
s" amp"  s" &"   xis.entities msc-add
s" quot" s\" \"" xis.entities msc-add
s" apos" s" '"   xis.entities msc-add


chs-create xis.start-name-set   ( the character set for the first letter of a name )

       xis.start-name-set chs-set-alpha
s" _:" xis.start-name-set chs-set-string


chs-create xis.next-name-set   ( the character set for the following letters of a name )

         xis.next-name-set chs-set-alnum
s" .-_:" xis.next-name-set chs-set-string


chs-create xis.space   ( the character set for xml space )

chr.sp xis.space chs-set-char
chr.ht xis.space chs-set-char
chr.cr xis.space chs-set-char
chr.lf xis.space chs-set-char


( xis reader constants )

begin-enumeration
  -1
  >enum: xis.error          ( -- n = Error          --                                                        )
  enum:  xis.done           ( -- n = Done reading   --                                                        )
  enum:  xis.start-xml      ( -- n = Start Document -- c-addr:standalone u c-addr:encoding u c-addr:version u )
  enum:  xis.comment        ( -- n = Comment        -- c-addr u                                               )
  enum:  xis.text           ( -- n = Normal text    -- c-addr u                                               )
  enum:  xis.start-tag      ( -- n = Start tag      -- c-addr:value u c-addr:attribute u .. n c-addr:tag u    )
  enum:  xis.end-tag        ( -- n = End tag        -- c-addr u                                               )
  enum:  xis.empty-element  ( -- n = Empty element  -- c-addr:value u c-addr:attribute u .. n c-addr:tag u    )
  enum:  xis.cdata          ( -- n = CDATA section  -- c-addr u                                               )
  enum:  xis.proc-instr     ( -- n = Proc. instr.   -- c-addr:value u c-addr:attribute u .. n c-addr:target u )
  enum:  xis.internal-dtd   ( -- n = Internal DTD   -- c-addr:markup u c-addr:name u                          )
  enum:  xis.public-dtd     ( -- n = Public DTD     -- c-addr:system u c-addr:publicid u c-addr:name u        )
  enum:  xis.system-dtd     ( -- n = System DTD     -- c-addr:system u c-addr:name u                          )
end-enumeration


( xml reader structure )

begin-structure xis%   ( -- n = Get the required space for a xis reader variable )
  tis%
  +field  xis>tis       \ the xis reader (extends the text input stream)
  field:  xis>msc       \ translation of entity references
  field:  xis>strip     \ strip leading and trailing spaces in normal text ?
end-structure



( xml reader variable creation, initialisation and destruction )

: xis-init   ( xis -- = Initialise the xml reader variable )
  
               dup  xis>tis   tis-init
  xis.entities over xis>msc   !
                    xis>strip off             
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


( Member words )

: xis-msc@   ( xis -- msc = Get the current entity reference catalog )
  xis>msc @
;


: xis-msc!   ( msc xis -- = Set the entity reference catalog for the reader )
  over nil= exp-invalid-parameters AND throw
  
  xis>msc !
;


: xis-strip@   ( xis -- flag = Return flag indicating the stripping of leading and trailing spaces in normal text )
  xis>strip @
;


: xis-strip!   ( flag xis -- = Set the flag indicating the stripping of leaading and trailing spaces in normal text )
  xis>strip !
;


( Private reader words )

: xis-read-reference   ( xis -- ... = Read and translate the reference )
  \ ToDo
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


: xis-skip-spaces   ( xis -- = Skip xml spaces in the stream ) 
  BEGIN
    xis.space over tis-match-set
  WHILE
    drop
  REPEAT
  drop
;


: xis-read-attributes ( xis -- c-addr u c-addr u n true | false = Read tag attributes )
  \ Lus: read attribute
  \ Error: remove from stack
;


: xis-read-name   ( xis -- c-addr u | 0 = Read a xml name )
  >r
  r@ tis-pntr@
  xis.start-name-set r@ tis-match-set IF
    drop
    BEGIN
      xis.next-name-set r@ tis-match-set
    WHILE
      drop
    REPEAT
    r@ tis-substring
  ELSE
    drop 0
  THEN
  rdrop
;


: xis-read-start-tag ( xis -- c-addr u c-addr u .. n c-addr u xis.start-tag = Read a start tag )
  dup xis-read-name ?dup IF
    2>r
    xis-read-attributes IF
      2r>
      \ ToDo: match / and/or >
      xis.start-tag
    ELSE
      2r> 2drop
      xis.error
    THEN
  ELSE
    drop xis.error
  THEN
;


: xis-read-end-tag ( xis -- c-addr u xis.end-tag = Read an end tag )
  xis-read-name ?dup IF
    \ ToDo: match >
    xis.end-tag
  ELSE
    xis.error
  THEN
;


: xis-read-comment ( xis -- c-addr u n = Read comment, return xis.comment )
  >r
  s" -->" r@ tis-scan-string IF        \ Search end of comment
    xis.comment
  ELSE
    [char] > r@ tis-scan-char IF       \ Not found, try on >
      xis.comment
    ELSE
      r@ tis-read-all ?dup IF          \ All is comment
        xis.comment
      ELSE
        xis.done
      THEN
    THEN
  THEN
  rdrop
;


: xis-read-proc-instr ( xis --  .. = Read a processing instruction )
  dup xis-read-name ?dup IF
    2>r
    xis-read-attributes IF
      2r>
      \ ToDo: match ? and >
      \ Check name for xml
      xis.start-tag
    ELSE
      2r> 2drop
      xis.error
    THEN
  ELSE
    drop xis.error
  THEN
;


: xis-read-cdata   ( xis -- c-addr u n = Read CDATA section, return xis.cdata )
  >r
  s" ]]>" r@ tis-scan-string IF        \ Search end of CDATA section
    xis.cdata
  ELSE
    [char] > r@ tis-scan-char IF       \ Not found, try on >
      xis.cdata
    ELSE
      r@ tis-read-all ?dup IF          \ All is CDATA section
        xis.cdata
      ELSE
        xis.done
      THEN
    THEN
  THEN
  rdrop
;


: xis-read-entity   ( xis -- i*x n = Read the entity definition )
;


: xis-read-doctype   ( xis -- i*x n = Read the document type definition )
;


: xis-read-text   ( xis -- c-addr u flag | 0 flag = Read the text, process the entity references )
  \ ToDo
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


: xis-read-texts   ( xis -- c-addr u n = Read normal xml text, return xis.text )
  \ ToDo
  >r
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
  rdrop
;

: xis-read-token   ( xis -- i*x n = Read the next xml token )
  >r
  
  r@ tis-eof? IF
    xis.done                                \ Done if no more data
  ELSE
    [char] < r@ tis-cmatch-char IF
      [char] / r@ tis-cmatch-char IF        \ Parse end tag for </
        r@ xis-read-end-tag
      ELSE 
        [char] ? r@ tis-cmatch-char IF
          r@ xis-read-proc-instr            \ Parse processing instruction for /?
        ELSE
          [char] ! r@ tis-cmatch-char IF
            s" --" r@ tis-cmatch-string IF
              r@ xis-read-comment           \ Parse comment
            ELSE
              s" [CDATA[" r@ tis-cmatch-string IF
                r@ xis-read-cdata           \ Parse CDATA section
              ELSE
                s" ENTITY" r@ tis-cmatch-string IF
                  r@ xis-read-entity        \ Parse entity definition
                ELSE
                  s" DOCTYPE" r@ tis-cmatch-string IF
                    r@ xis-read-doctype     \ Parse doctype
                  ELSE
                    xis.error
                  THEN
                THEN
              THEN
            THEN
          ELSE
            r@ xis-read-start-tag           \ Parse start tag
          THEN
        THEN
      THEN
    ELSE
      r@ xis-read-texts
    THEN
  THEN
  rdrop
;


( xml reader word )

: xis-read ( xis -- i*x n = Read the next xml token n with various parameters from the source [see xml reader constants] )
  >r
  r@ tis-reduce                   \ Keep the stream compact
  BEGIN
    r@ xis-read-token
    
    dup xis.text = IF             \ Keep looking after empty text
      over 0=
    ELSE
      false
    THEN
  WHILE
    drop 2drop
  REPEAT
  rdrop
;


( Inspection )

: xis-dump ( xis -- = Dump the xml reader variable )
;

[THEN]

\ ==============================================================================
