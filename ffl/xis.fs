\ ==============================================================================
\
\                  xis - the xml reader in the ffl
\
\               Copyright (C) 2007  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU Lesser General Public
\ License as published by the Free Software Foundation; either
\ version 3 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ Lesser General Public License for more details.
\
\ You should have received a copy of the GNU Lesser General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2008-02-21 20:31:19 $ $Revision: 1.16 $
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
( The default entity references, &lt; &gt; &amp;, &quot; and ' are           )
( translated, all others are simply returned in the text. By using the       )
( xis-msc! word a message catalog can be set, that will overrule the default )
( translations of entity references. The xis-set-reader word expects an      )
( execution token with the following stack behavior:                         )
( <pre>                                                                      )
(    x -- c-addr u | 0                                                       )
( </pre>                                                                     )
( Data x is the same as the first parameter during calling of the word       )
( xis-set-reader. For reading from files this is normally the file           )
( descriptor. The word returns, if successful, the read data in c-addr u.    )
( The xis-read word returns the parsed xml token with the following varying  )
( stack parameters:                                                          )
( <pre>                                                                      )
( xis.error          --                                                      )
( xis.done           --                                                      )
( xis.start-xml      -- c-addr1 u1 .. c-addrn un n          = Return n attribute names with their value                )
( xis.comment        -- c-addr u                            = Return the comment                                       )
( xis.text           -- c-addr u                            = Return the normal text                                   )
( xis.start-tag      -- c-addr1 u1 .. c-addrn un n c-addr u = Return the tag name and n attributes with their value    )
( xis.end-tag        -- c-addr u                            = Return the tag name                                      )
( xis.empty-element  -- c-addr1 u1 .. c-addrn un n c-addr u = Return the tag name and n attributes with their value    )
( xis.cdata          -- c-addr u                            = Return the CDATA section text                            )
( xis.proc-instr     -- c-addr1 u1 .. c-addrn un n c-addr u = Return the target name and n attributes with their value )
( xis.internal-dtd   -- c-addr1 u1 c-addr2 u2               = Return the DTD name c-addr2 u2 and markup c-addr1 u1     )
( xis.public-dtd     -- c-addr1 u1 c-addr2 u2 c-addr3 u3 c-addr4 u4 = Return the DTD name, the markup, the system-id and public-id )
( xis.system-dtd     -- c-addr1 u1 c-addr2 u2 c-addr3 u3    = Return the DTD name, the markup and the system-id        )
( </pre>                                                                     )

2 constant xis.version


( private constants )

msc-create xis.entities  ( the default entity reference catalog )

  s" lt"   s" <"   xis.entities msc-add
  s" gt"   s" >"   xis.entities msc-add
  s" amp"  s" &"   xis.entities msc-add
  s" quot" s\" \"" xis.entities msc-add
  s" apos" s" '"   xis.entities msc-add


chs-create xis.start-name   ( the character set for the first letter of a name )

         xis.start-name chs-set-alpha
  char _ xis.start-name chs-set-char
  char : xis.start-name chs-set-char


chs-create xis.next-name   ( the character set for the following letters of a name )

  xis.start-name xis.next-name chs^move
                 xis.next-name chs-set-digit
          char . xis.next-name chs-set-char
          char - xis.next-name chs-set-char


chs-create xis.space   ( the character set for xml space )

  chr.sp xis.space chs-set-char
  chr.ht xis.space chs-set-char
  chr.cr xis.space chs-set-char
  chr.lf xis.space chs-set-char

  
chs-create xis.end-normal-text   ( the ending character set for normal text )
  
  char < xis.end-normal-text chs-set-char
  char & xis.end-normal-text chs-set-char
  

chs-create xis.end-quoted-value   ( the ending character set for a quoted attribute value )
  
  char " xis.end-quoted-value chs-set-char
  char & xis.end-quoted-value chs-set-char
  
  
chs-create xis.end-apos-value   ( the ending character set for a single quoted attribute value )
  
  char ' xis.end-apos-value chs-set-char
  char & xis.end-apos-value chs-set-char
  
  
chs-create xis.end-non-quoted-value   ( the ending character set for a non-quoted attribute value )

  xis.space xis.end-non-quoted-value chs^move
  char &    xis.end-non-quoted-value chs-set-char
  char >    xis.end-non-quoted-value chs-set-char
  
  
( xis reader constants )

begin-enumeration
  -1
  >enum: xis.error          ( -- n = Error          )
  enum:  xis.done           ( -- n = Done reading   )
  enum:  xis.start-xml      ( -- n = Start Document )
  
  enum:  xis.comment        ( -- n = Comment        )
  enum:  xis.text           ( -- n = Normal text    )
  enum:  xis.start-tag      ( -- n = Start tag      )
  enum:  xis.end-tag        ( -- n = End tag        )
  enum:  xis.empty-element  ( -- n = Empty element  )
  enum:  xis.cdata          ( -- n = CDATA section  )
  enum:  xis.proc-instr     ( -- n = Proc. instr.   )
  enum:  xis.internal-dtd   ( -- n = Internal DTD   )
  enum:  xis.public-dtd     ( -- n = Public DTD     )
  enum:  xis.system-dtd     ( -- n = System DTD     )
end-enumeration


( xml reader structure )

begin-structure xis%   ( -- n = Get the required space for a xis reader variable )
  tis%
  +field  xis>tis       \ the xis reader (extends the text input stream)
  field:  xis>msc       \ the reference to entity references translation catalog
  field:  xis>strip     \ strip leading and trailing spaces in normal text ?
end-structure


( xml reader variable creation, initialisation and destruction )

: xis-init   ( xis -- = Initialise the xml reader variable )
  
               dup  xis>tis   tis-init
  xis.entities over xis>msc   !
                    xis>strip off             
;


: xis-(free)   ( xis -- = Free the internal, private variables from the heap )
  tis-(free)
;

    
: xis-create   ( "<spaces>name" -- ; -- xis = Create a named xml reader variable in the dictionary )
  create  here xis% allot  xis-init
;


: xis-new   ( -- xis = Create a new xml reader variable on the heap )
  xis% allocate  throw   dup xis-init
;


: xis-free   ( xis -- = Free the xis reader variable from the heap )
  dup xis-(free)
  
  free throw
;


( xml reader init words )

: xis-set-reader  ( x xt xis -- = Init the xml parser for reading using the reader callback xt with its data x )
  tis-set-reader
;


: xis-set-string  ( c-addr u xis -- = Init the xml parser for for reading from the string c-addr u )
  tis-set
;


( Member words )

: xis-msc@   ( xis -- msc = Get the current entity reference catalog )
  xis>msc @
;


: xis-msc!   ( msc xis -- = Set the entity reference catalog for the reader )
  over nil= exp-invalid-parameters AND throw
  
  xis>msc !
;


: xis-strip@   ( xis -- flag = Return flag indicating the stripping of leading and trailing white space in normal text )
  xis>strip @
;


: xis-strip!   ( flag xis -- = Set the flag indicating the stripping of leading and trailing white space in normal text )
  xis>strip !
;


( Private reader words )

: xis-read-reference   ( xis -- = Read and translate an entity reference )
  >r
  [char] # r@ tis-cmatch-char IF              \ Numerical reference
    base @ decimal
    r@ tis-pntr@
    r@ tis-read-number IF
      [char] ; r@ tis-cmatch-char IF
        swap 1- 1-                            \   Calculate delete and insert index
        r@ tis-pntr@ over -                   \   Calculate delete length
        2dup
        swap r@ str-delete                    \   Delete the reference
        -rot r@ str-insert-char               \   Insert the numerical reference
        negate 1+ r@ tis-pntr+! drop          \   Update the stream pointer
      ELSE
        2drop                                 \   No closing ;
      THEN
    ELSE
      drop                                    \   Not a number
    THEN
    base !  
  ELSE                                        \ Name reference
    [char] ; r@ tis-scan-char IF              \   Look for the end of the reference
      dup 1+ 1+                               \   Calculate length of reference
      -rot                                    \   And save it
      r@ xis-msc@ msc-translate? IF           \   Try to translate the reference
        tuck                                  \   Save translated length
        2over drop r@ tis-pntr@ over -        \   Calculate the delete and insert index
        tuck                                  \   Save index for insert
        r@ str-delete                         \   Delete the old reference (note: old>=translated)
        r@ str-insert-string                  \   Insert the translated reference in the stream
        swap - r@ tis-pntr+! drop             \   Update the stream pointer
      ELSE
        drop
      THEN
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
    4drop
  LOOP
;


: xis-read-name   ( xis -- c-addr u | 0 = Read a xml name )
  >r
  r@ tis-pntr@
  xis.start-name r@ tis-match-set IF
    drop
    BEGIN
      xis.next-name r@ tis-match-set
    WHILE
      drop
    REPEAT
    r@ tis-substring
  ELSE
    drop 0
  THEN
  rdrop
;


: xis-read-ref-text   ( chs xis -- c-addr u = Read text till set with translation of references )
  >r
  r@ tis-pntr@ swap
  BEGIN
    dup r@ tis-scan-set IF                  \ Scan text till set
      nip nip
      [char] & = IF
        true
      ELSE 
        -1 r@ tis-pntr+! drop               \ Set trailing char unprocessed
        false
      THEN
    ELSE
      r@ tis-read-all IF                    \ Nothing from set found, all remaining text is normal text
        drop
      THEN
      false
    THEN
  WHILE
    r@ xis-read-reference                   \ Repeat for entity reference
  REPEAT
  drop
  
  r> tis-substring                          \ Get the full text string
;


: xis-read-value   ( xis -- c-addr u = Read attribute value )
  >r
  [char] " r@ tis-cmatch-char IF
    xis.end-quoted-value r@ xis-read-ref-text
    [char] " r@ tis-cmatch-char drop       \ Remove trailing double quote
  ELSE
    [char] ' r@ tis-cmatch-char IF
      xis.end-apos-value r@ xis-read-ref-text
      [char] ' r@ tis-cmatch-char drop     \ Remove trailing single quote
    ELSE
      xis.end-non-quoted-value r@ xis-read-ref-text
    THEN
  THEN
  rdrop
;


: xis-read-attributes ( c-addr1 u1 xis -- c-addrn un c-addrn un .. n c-addr1 u1 = Read tag attributes )
  0
  >r >r
  BEGIN
    r@ xis-skip-spaces
    r@ xis-read-name ?dup
    
    IF
      r@ xis-skip-spaces
      [char] = r@ tis-cmatch-char IF
        r@ xis-skip-spaces
        r@ xis-read-value
      ELSE
        nil 0                          \ Attribute without value
      THEN
      2rot                             \ Rotate name and value after tag
      r> r> 1+ >r >r                   \ Increment the attribute counter
      false
    ELSE
      true
    THEN
  UNTIL
  rdrop
  r> -rot
;


: xis-read-start-tag ( xis -- c-addrn un c-addrn un .. n c-addr u xis.start-tag = Read a start tag with n attribute names and values )
  >r
  r@ xis-read-name ?dup IF
    r@ xis-read-attributes
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
    xis.error
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


: xis-read-proc-instr ( xis -- c-addrn un c-addrn un .. n c-addr u = Read a processing instruction with n attribute names and values )
  >r
  r@ xis-read-name ?dup IF
    r@ xis-read-attributes
    r@ xis-skip-spaces
    s" ?>" r@ tis-cmatch-string IF
      2dup s" xml" icompare 0= IF
        2drop
        xis.start-xml
      ELSE
        xis.proc-instr
      THEN
    ELSE
      xis+error-tag
      xis.error
    THEN
  ELSE
    drop xis.error
  THEN
  rdrop
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


: xis-read-literal   ( xis -- c-addr u | nil 0 = Read literal )
  >r
  r@ xis-skip-spaces
  [char] " r@ tis-cmatch-char IF
    [char] " r@ tis-scan-char 0= IF
      nil 0
    THEN
  ELSE
    [char] ' r@ tis-cmatch-char IF
      [char] ' r@ tis-scan-char 0= IF
        nil 0
      THEN
    ELSE
      nil 0
    THEN
  THEN
  rdrop
;

: xis+error-dtd      ( i*x n -- = Cleanup stack if DTD is not correct )
  >r
  4drop                                     \ Drop name and markup
  r@ xis.internal-dtd <> IF
    2drop                                   \ Drop system id
    r@ xis.public-dtd = IF
      2drop                                 \ Drop public id
    THEN
  THEN
  rdrop    
;


: xis-read-doctype   ( xis -- i*x n = Read the document type definition )
  >r
  r@ xis-skip-spaces
  r@ xis-read-name ?dup IF
    r@ xis-skip-spaces
    
    s" SYSTEM" r@ tis-cmatch-string IF      \ Check external id
      r@ xis-read-literal 2swap
      xis.system-dtd
    ELSE
      s" PUBLIC" r@ tis-cmatch-string IF
        r@ xis-read-literal 2swap
        r@ xis-read-literal 2swap
        xis.public-dtd
      ELSE
        xis.internal-dtd
      THEN
    THEN
    
    r@ xis-skip-spaces
    [char] [ r@ tis-cmatch-char IF
      [char] ] r@ tis-scan-char 0= IF
        nil 0
      THEN
    ELSE
      nil 0
    THEN
    rot >r 2swap r>                         \ move markup after name and xml token        

    r@ xis-skip-spaces
    [char] > r@ tis-cmatch-char 0= IF       \ Check for ending >, else error
      xis+error-dtd
      xis.error
    THEN
  ELSE
    xis.error
  THEN
  rdrop
;


: xis-read-text   ( xis -- c-addr u n | n = Read normal xml text, return xis.text or xis.done )
  >r
  
  xis.end-normal-text r@ xis-read-ref-text  \ Read till <, translate & references
  
  ?dup IF                                   \ If text read Then
    r@ xis-strip@ IF                        \   Strip it if requested
      str+strip
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
                s" DOCTYPE" r@ tis-cmatch-string IF
                  r@ xis-read-doctype       \ Parse doctype
                ELSE
                  xis.error
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


: xis+remove-attribute-parameters  ( c-addr1 u1 .. c-addrn un n -- = Remove the attributes parameters )
  0 ?DO
    4drop
  LOOP
;


: xis+?type ( c-addr u - = Print the string with zero length check )
  dup IF
    type
  ELSE
    2drop
    ." <empty>"
  THEN
;


: xis+print-attributes ( c-addr1 u1 .. c-addrn un n  -- Print all attributes )
  0 ?DO                                 \ Do for all attributes
    2swap
    ."  Attribute: " type               \   Print attribute name
    ."  Value: " xis+?type cr           \   Print attribute value
  LOOP
;


( xml reader word )

: xis-read ( xis -- i*x n = Read the next xml token n with various parameters from the source &lb;see xml reader constants&rb; )
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
    3drop
  REPEAT
  rdrop
;


: xis+remove-read-parameters  ( i*x n -- = Remove the various parameters of a xml token after calling xis-read &lb;see xml reader constants&rb; )
  CASE
    xis.error         OF                                        ENDOF
    xis.done          OF                                        ENDOF
    xis.start-xml     OF        xis+remove-attribute-parameters ENDOF
    xis.comment       OF  2drop                                 ENDOF
    xis.text          OF  2drop                                 ENDOF
    xis.start-tag     OF  2drop xis+remove-attribute-parameters ENDOF
    xis.end-tag       OF  2drop                                 ENDOF
    xis.empty-element OF  2drop xis+remove-attribute-parameters ENDOF
    xis.cdata         OF  2drop                                 ENDOF
    xis.proc-instr    OF  2drop xis+remove-attribute-parameters ENDOF
    xis.internal-dtd  OF  4drop                                 ENDOF
    xis.public-dtd    OF  4drop 4drop                           ENDOF
    xis.system-dtd    OF  4drop 2drop                           ENDOF
  ENDCASE
;


: xis+dump-read-parameters  ( i*x n -- = Dump the various parameters of a xml token after calling xis-read &lb;see xml reader constants&rb; )
  CASE
    xis.error         OF ." XML syntax error" cr                                                                              ENDOF
    xis.done          OF ." XML processing done" cr                                                                           ENDOF
    xis.start-xml     OF ." Start XML document:" cr xis+print-attributes                                                      ENDOF
    xis.comment       OF ." Comment: " type cr                                                                                ENDOF
    xis.text          OF ." Text: " type cr                                                                                   ENDOF
    xis.start-tag     OF ." Start tag: " type cr xis+print-attributes                                                         ENDOF
    xis.end-tag       OF ." End tag: " type cr                                                                                ENDOF
    xis.empty-element OF ." Empty element: " type cr xis+print-attributes                                                     ENDOF
    xis.cdata         OF ." CDATA section: " type cr                                                                          ENDOF
    xis.proc-instr    OF ." Proc. Instr.: " type cr xis+print-attributes                                                      ENDOF
    xis.internal-dtd  OF ." Internal DTD: " type ."  Markup: " type cr                                                        ENDOF
    xis.public-dtd    OF ." Public DTD: " type ."  Markup: " xis+?type ."  SystemID: " xis+?type ."  PublicID: " xis+?type cr ENDOF
    xis.system-dtd    OF ." System DTD: " type ."  Markup: " xis+?type ."  SystemID: " xis+?type cr                           ENDOF
  ENDCASE
;


[THEN]

\ ==============================================================================
