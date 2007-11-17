\ ==============================================================================
\
\              gmo - the import mo-file module in the ffl
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
\  $Date: 2007-11-17 07:47:22 $ $Revision: 1.4 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] gmo.version [IF]

cell 4 =  1 chars 1 =  AND [IF]

include ffl/msc.fs
include ffl/str.fs


( gmo = Gettexts mo-file import )
( The gmo module implements words for importing the contents of a gettexts   )
( mo-file into a message catalog. The mo-file uses 4 byte pointers and 1     )
( byte characters, so this module has an environmental dependency.           )


1 constant gmo.version


( Private structures )

struct: gmo%hdr%
  cell:     gmo>hdr>magic         \ Magic number 
  cell:     gmo>hdr>version       \ File version
  cell:     gmo>hdr>number        \ Number of entries
  cell:     gmo>hdr>msg-off       \ Offset for first message
  cell:     gmo>hdr>trn-off       \ Offset for first translation
;struct
 
struct: gmo%pair%
  cell:     gmo>pair>len          \ Message length
  cell:     gmo>pair>off          \ Message offset
;struct

struct: gmo%
  gmo%hdr%
  field:    gmo>hdr               \ mo-file header
  cell:     gmo>msgs              \ Messages
  cell:     gmo>trns              \ Translations
  cell:     gmo>file              \ File
  cell:     gmo>msc               \ Message catalog
  cell:     gmo>msg               \ message string
  cell:     gmo>trn               \ Translation string
;struct


( Private words )

: gmo-new  ( w:msc w:fileid - w:gmo = Build the gmo structure )
  
  gmo% allocate throw
  
  tuck gmo>file !
  tuck gmo>msc  !
  
  dup  gmo>msgs nil!
  dup  gmo>trns nil!
  
  dup  gmo>hdr>magic   0!
  dup  gmo>hdr>version 0!
  dup  gmo>hdr>number  0!
  dup  gmo>hdr>msg-off 0!
  dup  gmo>hdr>trn-off 0!
  
  str-new 
  256 over str-size!              \ Start with reasonable size
  over gmo>msg !
  
  str-new
  256 over str-size!
  over gmo>trn !
;


: gmo-free  ( w:gmo - = Free the gmo structure)
  
  dup gmo>file @ close-file drop
  
  dup gmo>msg  @ str-free
  dup gmo>trn  @ str-free
  
  dup gmo>msgs @ ?free throw
  dup gmo>trns @ ?free throw
  
  dup gmo>msc nil!
  
  free throw
;

  
: gmo-read-header  ( w:gmo - 0 | ior = Read the header from the mo-file)
  >r
  
  r@ gmo>hdr gmo%hdr% r@ gmo>file @ read-file ?dup 0= IF
    gmo%hdr% <> exp-no-data AND
  THEN
  
  ?dup 0= IF
    r@ gmo>hdr>magic @ [ hex ] 950412de [ decimal ] <> exp-wrong-file-type AND
  THEN
  
  ?dup 0= IF
    r@ gmo>hdr>version @ 0 <> exp-wrong-file-version AND
  THEN
  
  ?dup 0= IF
    r@ gmo>hdr>msg-off @ 0  r@ gmo>file @  reposition-file
  THEN
  
  ?dup 0= IF
    r@ gmo>hdr>number @ gmo%pair% *
    
    dup allocate throw
    dup r@ gmo>msgs !
    
    over r@ gmo>file @ read-file ?dup 0= IF
      <> exp-no-data AND
    THEN
  THEN
  
  ?dup 0= IF
    r@ gmo>hdr>trn-off @ 0  r@ gmo>file @  reposition-file
  THEN
  
  ?dup 0= IF
    r@ gmo>hdr>number @ gmo%pair% *
    
    dup allocate throw
    dup r@ gmo>trns !
    
    over r@ gmo>file @ read-file ?dup 0= IF
      <> exp-no-data AND
    THEN
  THEN
  rdrop
;  


: gmo-read-msg    ( w:pair w:str w:file - 0 | ior = Read one message from the mo-file into the string )
  rot 
  dup gmo>pair>len @ >r                     \ Save length of message
  
  r@ 0= IF                                  \ If length = 0 Then
    2drop
    str-clear                               \   Clear string and okee
    0
  ELSE                                      \ Else move the file pointer to the offset
    gmo>pair>off @  over  0 swap  reposition-file ?dup IF
      nip nip                               \   No success, return ior
    ELSE
      over  r@  swap  str-size!             \ Else insure size of string and read the message
      
      over str-data@  r@  rot  read-file  ?dup IF
        nip                                 \   No success, return ior
      ELSE
        dup r@ <> IF                        \   Success, check the return string length
          2drop
          exp-no-data                       \   Different string length, return exp-no-data as ior
        ELSE
          swap str-length!                  \   Else set the string length
          0
        THEN
      THEN
    THEN
  THEN
  rdrop
;


: gmo-read-msgs   ( w:gmo - 0 | ior = Read the messages from the mo-file and stores them in the message catalog )
  >r
  
  r@ gmo>trns @                   \ Translation pointer
  r@ gmo>msgs @                   \ Message pointer
  r@ gmo>hdr>number @             \ count
  0                               \ ior
  BEGIN
    2dup 0= swap 0> AND           \ While ior=0 AND count > 0 Do
  WHILE
    2over
                                  \   Read the message
    r@ gmo>msg @  r@ gmo>file @  gmo-read-msg ?dup IF
      nip
    ELSE                          \   Read the translation
      r@ gmo>trn @  r@ gmo>file @  gmo-read-msg
    THEN
    nip
    
                                  \ Add the messages to the catalog
    r@ gmo>msg @ str-get  r@ gmo>trn @ str-get  r@ gmo>msc @  msc-add
    
    >r 1- >r gmo%pair% + >r gmo%pair% + r> r> r>   \ Update translation pointer, message pointer and counter
  REPEAT
  rdrop
  nip nip nip
;


( Import )

: gmo-read  ( c-addr u m:msc - 0 | ior = Read a mo-file and store the contents in the message catalog )
  -rot
  
  r/o bin open-file ?dup 0= IF
    
    gmo-new
    
    dup gmo-read-header ?dup 0= IF
      
      dup gmo-read-msgs
    THEN
    
    swap gmo-free
  ELSE
    nip
  THEN
;

[ELSE]
.( Warning: gmo requires 4 byte cells and 1 byte chars ) cr
[THEN]

[THEN]

\ ==============================================================================
