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
\  $Date: 2007-12-16 19:37:18 $ $Revision: 1.8 $
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

: xis-read-reference   ( xis -- = Read and translate the reference )
  >r
  [char] ; r@ tis-scan-char IF              \ Look for the end of the reference
    dup 1+ 1+                               \ Calculate length of reference
    -rot                                    \ And save it
    r@ xis-msc@ msc-translate? IF           \ Try to translate the reference
      tuck                                  \ Save translated length
      2over drop r@ tis-pntr@ swap -        \ Calculate the insert index
      r@ str-insert-string                  \ Insert the translated reference in the stream
      2dup over r@ tis-pntr@ swap - +       \ Calculate the delete index
      r@ str-delete                         \ Delete the old reference
      swap - r@ tis-pntr+! drop             \ Update the stream pointer
    ELSE
      drop
    THEN
  THEN
  rdrop
;


: xis-skip-spaces   ( xis -- = Skip xml spaces in the stream ) 
  BEGIN
    xis.space over tis-match-set
  WHILE
    drop
  REPEAT
  drop
;


: xis+error-tag   ( c-addrn un c-addrn un .. n c-addr1 u1 -- = Clear the stack after an error in a tag )
  2drop
  0 ?DO
    2drop
    2drop
  LOOP
;


: xis-read-attributes ( c-addr1 u1 xis -- c-addrn un c-addrn un .. n c-addr1 u1 true | false = Read tag attributes )
  drop true
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


: xis-read-start-tag ( xis -- c-addrn un c-addrn un .. n c-addr u xis.start-tag = Read a start tag )
  >r
  r@ xis-read-name ?dup IF
    r@ xis-read-attributes IF
      r@ xis-skip-spaces
      s" />" r@ tis-cmatch-string IF
        xis.empty-element
      ELSE
        [char] > r@ tis-cmatch-char IF
          xis.start-tag
        ELSE
          xis+error-tag
          xis.error
        THEN
      THEN
    ELSE
      2r> 2drop
      xis.error
    THEN
  ELSE
    drop xis.error
  THEN
  rdrop
;


: xis-read-end-tag ( xis -- c-addr u xis.end-tag = Read an end tag )
  >r
  r@ xis-read-name ?dup IF
    r@ xis-skip-spaces
    [char] > r@ tis-cmatch-char IF    
      xis.end-tag
    ELSE
      xis.error
    THEN
  ELSE
    xis.error
  THEN
  rdrop
;


: xis-read-comment ( xis -- c-addr u n = Read comment, return xis.comment )
  >r
  s" -->" r@ tis-scan-string IF        \ Search end of comment
    xis.comment
  ELSE
    r@ tis-read-all ?dup IF            \ All is comment
      xis.comment
    ELSE
      xis.done
    THEN
  THEN
  rdrop
;


: xis-read-proc-instr ( xis -- c-addrn un c-addrn un .. n c-addr u = Read a processing instruction )
  >r
  r@ xis-read-name ?dup IF
    xis-read-attributes IF
      r@ xis-skip-spaces
      s" ?>" r@ tis-cmatch-string IF
        2dup s" xml" icompare 0= IF
          xis.start-xml
        ELSE
          xis.proc-instr
        THEN
      ELSE
        xis+error-tag
        xis.error
      THEN
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
    r@ tis-read-all ?dup IF            \ All is CDATA section
      xis.cdata
    ELSE
      xis.done
    THEN
  THEN
  rdrop
;


: xis-read-entity   ( xis -- i*x n = Read the entity definition )
;


: xis-read-doctype   ( xis -- i*x n = Read the document type definition )
;


: xis-read-text   ( xis -- c-addr u n | n = Read normal xml text, return xis.text or xis.done )
  >r
  r@ tis-pntr@
  BEGIN
    s" <&" r@ tis-scan-chars IF             \ Scan text till < or &
      nip nip
      [char] & = IF
        true
      ELSE
        -1 r@ tis-pntr+! drop               \ Set < unprocessed
        false
      THEN
    ELSE
      r@ tis-read-all IF                    \ No < or & found, all remaining text is normal text
        drop
      THEN
      false
    THEN
  WHILE
    r@ xis-read-reference                   \ Repeat for entity reference
  REPEAT
  
  r@ tis-substring                          \ Get the full text string
  
  ?dup IF                                   \ If text read Then
    r@ xis-strip@ IF                        \   Strip it if requested
      \ ToDo str+strip
    THEN
    xis.text                                \   Text processed
  ELSE                                      \ Else
    drop                                    \   Done
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
      r@ xis-read-text
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
