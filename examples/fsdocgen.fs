\ ==============================================================================
\
\                  fsdocgen - ffl documentation generator
\
\              Copyright (C) 2015..2016  Dick van Oudheusden
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
\  $Date: 2009-05-28 17:35:58 $ $Revision: 1.11 $
\
\ ==============================================================================

include ffl/stc.fs
include ffl/enm.fs
include ffl/str.fs
include ffl/car.fs
include ffl/tis.fs

[DEFINED] #args [DEFINED] arg@ AND [IF]

\ ==============================================================================
\ Settings
\ ==============================================================================

char / constant directory-separator        \ Directory separator
char . constant extension-separator        \ File extension separator

1022   constant source-line-size           \ Maximum size of the source line

\ ==============================================================================
\ Constants
\ ==============================================================================

str-create fsdocgen-version  s" 0.1.0" fsdocgen-version str-set

\ ==============================================================================
\ The documented item
\ ==============================================================================

\ ------------------------------------------------------------------------------
\ Data structure
\ ------------------------------------------------------------------------------

begin-enumeration
  enum: docu.module
  enum: docu.description
  enum: docu.header
  enum: docu.word
end-enumeration

begin-structure docu%
        field:  docu>type
  str% +field   docu>module
  str% +field   docu>filename
  str% +field   docu>word
  str% +field   docu>stack
  str% +field   docu>description
end-structure

\ ------------------------------------------------------------------------------
\ allocator
\ ------------------------------------------------------------------------------

: docu-new ( -- docu = Allocate a new documented item )
  docu% allocate throw
  >r
  r@ docu>type        0!
  r@ docu>module      str-init
  r@ docu>filename    str-init
  r@ docu>word        str-init
  r@ docu>stack       str-init
  r@ docu>description str-init
  r>
;

\ ------------------------------------------------------------------------------
\ Deallocator
\ ------------------------------------------------------------------------------

: docu-free  ( docu -- = Free the documented item )
  >r
  r@ docu>module      str-(free)
  r@ docu>filename    str-(free)
  r@ docu>word        str-(free)
  r@ docu>stack       str-(free)
  r@ docu>description str-(free)
  r>
  free throw
;

\ ------------------------------------------------------------------------------
\ Word compare
\ ------------------------------------------------------------------------------

: docu^word-compare  ( doc1 doc2 -- n = Compare two words )
  swap docu>word swap docu>word str^icompare
;

\ ------------------------------------------------------------------------------
\ Module compare
\ ------------------------------------------------------------------------------

: docu^module-compare  ( doc1 doc2 -- n = Compare two modules )
  swap docu>module swap docu>module str^icompare
;

\ ------------------------------------------------------------------------------
\ Get
\ ------------------------------------------------------------------------------

: docu-get  ( docu -- str1 str2 = Get the generator 1:word and 2:stack )
  dup docu>word swap docu>stack
;

\ ==============================================================================
\ The forth source document generator
\ ==============================================================================

\ ------------------------------------------------------------------------------
\ File and line Parser state
\ ------------------------------------------------------------------------------

begin-enumeration
  enum:  state-module-short
  enum:  state-description
  enum:  state-public-words
  enum:  state-private-words
end-enumeration

state-module-short value file-parser-state
state-module-short value file-writer-state

0                  value line-parser-state

\ ------------------------------------------------------------------------------
\ Data fields
\ ------------------------------------------------------------------------------

  str-create output-directory
  str-create example-directory
0 car-create input-files                      \ Array with the input filenames
0 car-create module-index                     \ Array with the modules
0 car-create word-index                       \ Array with the words

\ ------------------------------------------------------------------------------
\ Data initialisation
\ ------------------------------------------------------------------------------

256  input-files car-size!
32   input-files car-extra!

256  module-index       car-size!
32   module-index       car-extra!
' docu^module-compare
     module-index       car-compare!

1024 word-index         car-size!
512  word-index         car-extra!
' docu^word-compare
     word-index         car-compare!

\ ------------------------------------------------------------------------------
\ Read the program arguments
\ ------------------------------------------------------------------------------

: read-arguments ( -- flag = Read the command line arguments )
  #args 3 < IF
    ." usage: fsdocgen  OUTPUT DIRECTORY  EXAMPLE DIRECTORY  FILE .." cr
    ." The html documentation generator for the ffl-library." cr cr
  ELSE
    0 arg@ output-directory  str-set        \ Save the output directory
    1 arg@ example-directory str-set        \ Save the example directory

    #args 2 DO
      str-new                               \ Create a new dyn. string, ..
      dup input-files car-append            \ .. append it in the array ..
      i arg@ rot str-set                    \ .. and fill it with the arugment
    LOOP
  THEN
  input-files car-length@ 0>
;


\ ------------------------------------------------------------------------------
\ Short module description line parser
\ ------------------------------------------------------------------------------

: parse-header-end ( docu c-addr -- docu = Parse till the end of the header )
  drop
;

: parse-short-module-description ( docu -- docu = Parse the descripion )
  c@ dup [char] ) = IF
    drop
    ['] parse-header-end to line-parser-state
  ELSE
    over docu>description str-append-char
  THEN
;

: is-comment-separator ( c-addr -- flag = Is the comment separator at c-addr )
  false
  over c@ [char] = = IF
    over char+ c@ chr-blank? IF
      over char- c@ chr-blank? IF
        0=
      THEN
    THEN
  THEN
  nip
;


: parse-short-module-name ( docu c-addr -- docu = Parse the module name )
  dup is-comment-separator IF
    drop
    ['] parse-short-module-description to line-parser-state
  ELSE
    c@ dup [char] ) = IF
      ['] parse-header-end to line-parser-state
    ELSE
      over docu>module str-append-char
    THEN
  THEN
;

: parse-short-module-comment ( docu c-addr -- docu = Parse the comment )
  c@ chr-blank? IF
    ['] parse-short-module-name to line-parser-state
  ELSE
    ['] parse-header-end to line-parser-state
  THEN
;

: parse-short-module-start ( docu c-addr -- docu = Parse the start of a short module )
  c@ dup [char] ( = IF
    drop
    ['] parse-short-module-comment to line-parser-state
  ELSE
    chr-blank? 0= IF
      ['] parse-header-end to line-parser-state
    THEN
  THEN
;

\ ------------------------------------------------------------------------------
\ Header line parser
\ ------------------------------------------------------------------------------

: parse-header-description ( docu c-addr -- docu = Parse the header description )
  c@ dup [char] ) = IF
    drop
    ['] parse-header-end to line-parser-state
  ELSE
    over docu>description str-append-char
  THEN
;

: parse-header-comment ( docu c-addr -- docu = Parse the comment )
  c@ chr-blank? IF
    ['] parse-header-description to line-parser-state
  ELSE
    ['] parse-header-end to line-parser-state
  THEN
;

: parse-header-start ( docu c-addr -- docu = Parse the start of a header )
  c@ dup [char] ( = IF
    drop
    ['] parse-header-comment to line-parser-state
  ELSE
    chr-blank? 0= IF
      ['] parse-header-end to line-parser-state
    THEN
  THEN
;

: parse-header-line ( str docu xt -- docu  = Parse a line with the line-parser-state )
  to line-parser-state

  swap str-bounds ?DO
    I line-parser-state execute
  LOOP
;


\ ------------------------------------------------------------------------------
\ Word line parser
\ ------------------------------------------------------------------------------

: parse-word-end  ( docu c-addr -- docu = Skip parsing )
  drop
;

: parse-word-name ( docu c-addr -- docu = Parse a word name )
  c@ dup chr-blank? IF
    drop
    ['] parse-word-end to line-parser-state
  ELSE
    over docu>word str-prepend-char
  THEN
;

: parse-to-word ( docu c-addr -- docu = Skip spaces before word )
  c@ dup chr-blank? 0= IF
    over docu>word str-prepend-char

    ['] parse-word-name to line-parser-state
  ELSE
    drop
  THEN
;

: is-comment-start ( c-addr -- flag = Is the start of a comment at c-addr )
  false
  over c@ [char] ( = IF
    over char+ c@ chr-blank? IF
      over char- c@ chr-blank? IF
        0=
      THEN
    THEN
  THEN
  nip
;

: parse-word-stack ( docu c-addr -- docu = Parse the word stack description )
  dup is-comment-start IF
    drop
    ['] parse-to-word to line-parser-state
  ELSE
    c@ over docu>stack str-prepend-char
  THEN
;

: parse-word-description ( docu c-addr -- docu = Parse the word stack description )
  dup is-comment-separator IF
    drop
    ['] parse-word-stack to line-parser-state
  ELSE
    dup is-comment-start IF
      drop
      >r
      r@ docu>description str-get r@ docu>stack str-set \ No description, only stack info
      r@ docu>description str-clear
      r>
      ['] parse-to-word to line-parser-state
    ELSE
      c@ over docu>description str-prepend-char
    THEN
  THEN
;

: parse-word-start ( docu c-addr -- docu = Parse the start of a word comment )
  dup c@ [char] ) = IF
    drop
    ['] parse-word-description to line-parser-state
  ELSE
    c@ chr-blank? 0= IF
      ['] parse-word-end to line-parser-state
    THEN
  THEN
;

: parse-word-line ( str docu xt -- docu = Parse a word line with the line-parser-state )
  to line-parser-state

  swap str-get swap char- tuck + DO
    I line-parser-state execute
  1 chars -LOOP
;

\ ------------------------------------------------------------------------------
\ Start reading a source file
\ ------------------------------------------------------------------------------

: strip-filename ( str1 -- str2 = Strip the input filename from directory and extension )
  tis-new >r
  str-get r@ tis-set

  BEGIN                                          \ Remove leading directories
    directory-separator r@ tis-scan-char
  WHILE
    2drop
  REPEAT

  str-new dup
  extension-separator r@ tis-scan-char 0= IF     \ Look for the extension
    r@ tis-get                                   \ If not there, module name
  THEN
  rot str-set

  r> tis-free
;


: add-filename-extesion ( str -- = Append the html extension )
  s" .html" rot str-append-string
;

: add-output-directory ( str -- = Prepend output directory )
  -1 output-directory str-get-char directory-separator <> IF
    directory-separator    over str-prepend-char   \ Only prepend a directory separator if missing
  THEN
  output-directory str-get rot  str-prepend-string
;


: add-output-filename-in-index ( str -- = Add a new module to the index with output filename )
  docu-new >r
  docu.module r@ docu>type !
              r@ docu>filename str^move
  r> module-index car-append
;


: create-output-file  ( str --  fileid = Create the output file, throws any errors )
  str-get r/w create-file throw
;


: open-input-file  ( str -- fileid = Open the input file, throws any errors )
  str-get r/o open-file throw
;


: start-reading-source  ( str -- fileid fileid = Start reading a source file resulting in an input fileid and output fileid )
  >r
  r@ open-input-file
  r> strip-filename 

  >r
  r@ add-filename-extesion
  r@ add-output-filename-in-index
  r@ add-output-directory
  r@ create-output-file
  r> str-free

  state-module-short to file-parser-state
  state-module-short to file-writer-state
;


\ ------------------------------------------------------------------------------
\ Read a line from the source file
\ ------------------------------------------------------------------------------


: read-source-line ( fileid -- str true | false = Read a line from the source )
  str-new
  source-line-size 2 + over str-size!            \ Insure the size of the line
  tuck str-data@ source-line-size rot read-line throw IF
    over str-length!
    true
  ELSE
    drop
    str-free
    false
  THEN
;

\ ------------------------------------------------------------------------------
\ Parse a source line
\ ------------------------------------------------------------------------------

: create-module  ( -- docu = Create module documentation with name1 and description2, throws exp-invalid-state )
  module-index car-tos
  dup docu>type @ docu.module <> exp-invalid-state AND throw
;


: create-description ( -- docu = Create module description documentation )
  docu-new
  docu.description over docu>type !
;


: create-word  ( -- docu = Create word documentation with description1, stack2 and word3 )
  docu-new
  docu.word over docu>type !

  module-index car-tos
  docu>filename over docu>filename str^move      \ Use the filename of current module for filename
;


: create-header  ( -- docu = Create header documentation )
  docu-new 
  docu.header over docu>type !
;

: parse-module-short  ( str -- docu true | false = Parse a short module description )
  create-module  ['] parse-short-module-start  parse-header-line

  dup docu>module str-empty? IF
    drop
    false
  ELSE
    state-description to file-parser-state
    true
  THEN
;

: parse-module-long  ( str -- docu true | false = Parse a long module description )
  create-description  ['] parse-header-start   parse-header-line

  dup docu>description str-empty? IF
    docu-free
    false
  ELSE
    true
  THEN
;

: is-private-header  ( c-addr u -- flag = Check case-insensitive if the word 'private' is in the string )
  str-new >r
  r@ str-set
  r@ str-lower
  s" private " 0 r@ str-find -1 <>
  r>  str-free
;


: parse-public-header  ( str -- docu true | false = Parse a header in the public words )
  create-header  ['] parse-header-start  parse-header-line

  dup docu>description
  dup str-empty? IF
    drop docu-free
    false
  ELSE
    str-get is-private-header IF
      state-private-words to file-parser-state
      docu-free
      false
    ELSE
      true
    THEN
  THEN
;

: parse-private-header  ( str -- docu true | false = Parse a header in the private words )
  create-header  ['] parse-header-start  parse-header-line

  dup docu>description 
  dup str-empty? swap str-get is-private-header OR IF
    docu-free
    false
  ELSE
    state-public-words to file-parser-state
    true
  THEN
;

: parse-header  ( str -- docu true | false = Parse a header comment )
  file-parser-state CASE
    state-module-short  OF parse-module-short   ENDOF
    state-description   OF parse-module-long    ENDOF
    state-public-words  OF parse-public-header  ENDOF
    state-private-words OF parse-private-header ENDOF
  ENDCASE
;


: parse-word-comment  ( str -- docu true | false = Parse a word comment )
  file-parser-state state-public-words = IF
    create-word  ['] parse-word-start  parse-word-line

    dup docu>word str-empty? IF
      docu-free
      false
    ELSE
      dup word-index car-insert-sorted
      true
    THEN
  ELSE
    drop
    false
  THEN
;

: parse-source-line ( str -- docu true | false = Parse the source line )
  >r
  r@ str-strip
  r@ str-length@ 3 > IF
    0 r@ str-get-char [char] ( =  1 r@ str-get-char chr-blank? AND IF  \ Start of comment
      r@ parse-header
    ELSE
      -1 r@ str-get-char [char] ) = IF           \ End of comment
        r@ parse-word-comment
      ELSE
        false
      THEN
    THEN
  ELSE
    r@ str-empty?  file-parser-state state-description = AND IF \ Empty line ends module description
      state-public-words to file-parser-state
    THEN

    false
  THEN
  r> str-free
;


\ ------------------------------------------------------------------------------
\ Entity conversions
\ ------------------------------------------------------------------------------

: do-with-entities ( c-addr u fileid xt -- = Use xt to write the string entities )
  2swap
  str-new -rot
  bounds ?DO
    I c@ CASE
      [char] < OF dup s" &lt;"   rot str-append-string ENDOF
      [char] > OF dup s" &gt;"   rot str-append-string ENDOF
      [char] & OF dup s" &amp;"  rot str-append-string ENDOF
      [char] ' OF dup s" &apos;" rot str-append-string ENDOF
      2dup swap str-append-char
    ENDCASE
  1 chars +LOOP
  >r
  r@ str-get 2swap execute throw
  r> str-free
;

: write-file-with-entities ( c-addr u fileid -- = Write the string with entities )
  ['] write-file do-with-entities
;

: write-line-with-entities ( c-addr u fileid -- ior = Write the string with entities en end of line )
  ['] write-line do-with-entities
;

\ ------------------------------------------------------------------------------
\ Write the documentation
\ ------------------------------------------------------------------------------

: write-module-short  ( docu fileid -- = Write the module short info )
  >r
  dup docu>type @ docu.module = IF
    s" <!DOCTYPE html>"                            r@ write-line throw
    s" <html>"                                     r@ write-line throw
    s" <head>"                                     r@ write-line throw
    s" <title>"                                    r@ write-file throw
    dup docu>module str-get                        r@ write-file throw
    s"  -- "                                       r@ write-file throw
    dup docu>description str-get                   r@ write-file throw
    s" </title>"                                   r@ write-line throw
    s" <meta name='generator' content='fsdocgen'>" r@ write-line throw
    s" <link rel='stylesheet' href='style.css'>"   r@ write-line throw
    s" </head>"                                    r@ write-line throw
    s" <body>"                                     r@ write-line throw
    s" <h2>Module description</h2>"                r@ write-line throw
    s" <dl>"                                       r@ write-file throw
    s" <dt>"                                       r@ write-file throw
    dup docu>module str-get                        r@ write-file throw
    s"  -- "                                       r@ write-file throw
    dup docu>description str-get                   r@ write-file throw
    s" </dt>"                                      r@ write-line throw
    s" <dd>"                                       r@ write-line throw
    state-description to file-writer-state
  ELSE
    exp-invalid-state throw
  THEN
  drop                                           \ No free: in module-index
  rdrop
;


: write-public  ( docu fileid -- = Write public words and headers )
  >r
  dup docu>type @ docu.header = IF
    s" <h2>"                                       r@ write-file throw
    dup docu>description str-get                   r@ write-file throw
    s" </h2>"                                      r@ write-line throw
    docu-free
  ELSE
    dup docu>type @ docu.word = IF
      s" <dl>"                                     r@ write-line throw
      s" <dt id='"                                 r@ write-file throw
      dup docu>word str-get                        r@ write-file-with-entities
      s" '>"                                       r@ write-file throw
      dup docu>word str-get                        r@ write-file-with-entities
      s"  ( "                                      r@ write-file throw
      dup docu>stack str-get                       r@ write-file-with-entities
      s"  )</dt>"                                  r@ write-line throw
      s" <dd>"                                     r@ write-file throw
      dup docu>description str-get                 r@ write-file throw
      s" </dd>"                                    r@ write-line throw
      s" </dl>"                                    r@ write-line throw
      drop                                       \ No free: in word-index
    ELSE
      exp-invalid-state throw
    THEN
  THEN
  rdrop
;


: write-module-long  ( docu fileid -- = Write the module description )
  >r
  dup docu>type @ docu.description = IF
    dup docu>description str-get                   r@ write-line throw
    docu-free
  ELSE
    s" </dd>"                                      r@ write-line throw
    s" </dl>"                                      r@ write-line throw
    s" <hr>"                                       r@ write-line throw
    r@ write-public

    state-public-words to file-writer-state
  THEN
  rdrop
;


: write-footer ( fileid -- = Write the footer )
  >r
  s" <hr>"                                          r@ write-line throw
  s" <p>Generated by fsdocgen "                     r@ write-file throw
  fsdocgen-version str-get                          r@ write-file throw
  s" </p>"                                          r@ write-line throw
  s" </body>"                                       r@ write-line throw
  s" </html>"                                       r@ write-line throw
  rdrop
;


: write-documentation ( docu fileid -- = Write documentation )
  file-writer-state CASE
    state-module-short  OF write-module-short       ENDOF
    state-description   OF write-module-long        ENDOF
    state-public-words  OF write-public             ENDOF
    state-private-words OF exp-invalid-state throw  ENDOF
  ENDCASE
;

\ ------------------------------------------------------------------------------
\ Write the example in the output file
\ ------------------------------------------------------------------------------

: open-example-file ( -- fileid true | false = Open the example file )
  str-new >r

  module-index car-tos docu>module r@ str^move \ Fetch the module

  r@ str-strip                                 \ Trim spaces

  s" _expl.fs" r@ str-append-string            \ Append the example name

  -1 example-directory str-get-char directory-separator <> IF
    directory-separator r@ str-prepend-char    \ Prepend missing directory separator
  THEN

  example-directory str-get r@ str-prepend-string \ Prepend example directory

  r@ str-get r/o open-file dup IF              \ Check for present file
    nip
  THEN
  0=
  r> str-free
;

: write-example-file ( fileid1 fileid2 - = Write the example file )
  swap >r
  s" <h2>Examples</h2>" r@ write-line throw
  s" <pre>"             r@ write-line throw

  str-new
  source-line-size 2 + over str-size!          \ Insure the size of the line

  BEGIN
    2dup str-data@ source-line-size rot read-line throw
  WHILE
    over str-length!                           \ Set the number of chars read

    dup str-get         r@ write-line-with-entities  \ Write the example line
  REPEAT
  drop
  str-free
  drop                                         \ Fileid

  s" </pre>"            r@ write-line throw
  rdrop
;

: write-example ( fileid -- = Write the optional example in the output file )
  open-example-file IF
    2dup write-example-file
    close-file drop
  THEN
  drop
;
\ ------------------------------------------------------------------------------
\ Stop reading from the source file
\ ------------------------------------------------------------------------------

: stop-reading-source ( fileid fileid -- = Close the input and output file )
  dup write-example
  dup write-footer

  close-file throw
  close-file throw
;

\ ------------------------------------------------------------------------------
\ Convert the source files to documentation
\ ------------------------------------------------------------------------------

: convert-to-documentation  ( str -- = Convert a source file to html )
  start-reading-source
  BEGIN
    over read-source-line
  WHILE
    parse-source-line IF
      over write-documentation
    THEN
  REPEAT
  stop-reading-source
;

: try-convert-to-documentation ( str -- = Try to convert a source module to html )
  ['] convert-to-documentation catch ?dup IF
   ." Failed to generate documentation for " swap str-get type ."  reason: " . cr
  THEN
;

: convert-source-files  ( -- = Convert the source files to html )
  ['] try-convert-to-documentation  input-files  car-execute
;

\ ------------------------------------------------------------------------------
\ Write the module index file
\ ------------------------------------------------------------------------------

: create-module-file ( -- fileid = Create the module index file )
  str-new >r
  s" index.html"
  r@ str-set
  r@ add-output-directory
  r@ str-get r/w create-file throw
  r> str-free
;

: write-header-in-module-file ( fileid -- = Write the header in the module index file )
  >r
  s" <!DOCTYPE html>"                                     r@ write-line throw
  s" <html>"                                              r@ write-line throw
  s" <head>"                                              r@ write-line throw
  s" <title>Forth Foundation Library -- Modules</title>"  r@ write-line throw
  s" <meta name='generator' content='fsdocgen'>"          r@ write-line throw
  s" <link rel='stylesheet' href='style.css'>"            r@ write-line throw
  s" </head>"                                             r@ write-line throw
  s" <body>"                                              r@ write-line throw
  s" <h2>Modules</h2>"                                    r@ write-line throw
  s" <table>"                                             r@ write-line throw
  rdrop
;

: write-module-in-module-file ( fileid docu -- fileid = Write the module in the module index file )
  swap >r
  dup docu>module str-empty? 0= IF
    s" <tr>"                         r@ write-line throw
    s" <td><a href='"                r@ write-file throw
    dup docu>filename str-get        r@ write-file throw
    s" '>"                           r@ write-file throw
    dup docu>module str-get          r@ write-file throw
    s" </a></td>"                    r@ write-line throw
    s" <td>"                         r@ write-file throw
    dup docu>description str-get     r@ write-file throw
    s" </td>"                        r@ write-line throw
    s" </tr>"                        r@ write-line throw
  THEN
  docu-free
  r>
;

: write-footer-in-module-file ( file-id - = Write the footer in the module index file )
  >r
  s" </table>"                       r@ write-line throw
  s" <hr>"                           r@ write-line throw
  s" <p>Generated by fsdocgen "      r@ write-file throw
  fsdocgen-version str-get           r@ write-file throw
  s" </p>"                           r@ write-line throw
  s" </body>"                        r@ write-line throw
  s" </html>"                        r@ write-line throw
  rdrop
;

: write-module-index  ( -- = Write the module index file )
  module-index car-sort                          \ Sort the module names

  create-module-file
  dup write-header-in-module-file
  ['] write-module-in-module-file module-index car-execute  \ Write all modules in the index file
  dup write-footer-in-module-file
  close-file throw
;

\ ------------------------------------------------------------------------------
\ Write the word index file
\ ------------------------------------------------------------------------------

: create-words-file ( -- fileid = Create the words index file )
  str-new >r
  s" words.html"
  r@ str-set
  r@ add-output-directory
  r@ str-get r/w create-file throw
  r> str-free
;

: write-header-in-words-file ( fileid -- = Write the header in the words index file )
  >r
  s" <!DOCTYPE html>"                                     r@ write-line throw
  s" <html>"                                              r@ write-line throw
  s" <head>"                                              r@ write-line throw
  s" <title>Forth Foundation Library -- Words</title>"    r@ write-line throw
  s" <meta name='generator' content='fsdocgen'>"          r@ write-line throw
  s" <link rel='stylesheet' href='style.css'>"            r@ write-line throw
  s" </head>"                                             r@ write-line throw
  s" <body>"                                              r@ write-line throw
  s" <h2>Words</h2>"                                      r@ write-line throw
  s" <dl>"                                                r@ write-line throw
  rdrop
;

: write-word-in-words-file ( fileid docu -- fileid = Write the word in the words index file )
  swap >r
  dup docu>word str-empty? 0= IF
    s" <dt><a href='"                r@ write-file throw
    dup docu>filename str-get        r@ write-file throw
    s" #"                            r@ write-file throw
    dup docu>word str-get            r@ write-file-with-entities
    s" '>"                           r@ write-file throw
    dup docu>word str-get            r@ write-file-with-entities
    s" </a> ( "                      r@ write-file throw
    dup docu>stack str-get           r@ write-file-with-entities
    s" )</dt>"                       r@ write-line throw
  THEN
  docu-free
  r>
;

: write-footer-in-words-file ( fileid -- = Write the footer in the words index file )
  >r
  s" </dl>"                          r@ write-line throw
  s" <hr>"                           r@ write-line throw
  s" <p>Generated by fsdocgen "      r@ write-file throw
  fsdocgen-version str-get           r@ write-file throw
  s" </p>"                           r@ write-line throw
  s" </body>"                        r@ write-line throw
  s" </html>"                        r@ write-line throw
  rdrop
;

: write-word-index ( -- = Write the word index file )
  create-words-file
  dup write-header-in-words-file
  ['] write-word-in-words-file word-index car-execute \ Write all words in the index file
  dup write-footer-in-words-file
  close-file throw
;

\ ==============================================================================
\ Main program
\ ==============================================================================

: fsdocgen  ( -- = Main program )
  read-arguments IF
    convert-source-files
    write-module-index
    write-word-index
  THEN
;

fsdocgen

[ELSE]
  .( fsdocgen: the forth engine does not support command line arguments. )
[THEN]

bye

