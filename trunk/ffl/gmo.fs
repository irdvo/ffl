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
\  $Date: 2008-03-02 15:03:03 $ $Revision: 1.6 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] gmo.version [IF]

cell 4 >=  1 chars 1 =  AND [IF]

include ffl/msc.fs
include ffl/str.fs
include ffl/fwt.fs


( gmo = Gettexts mo-file import )
( The gmo module implements the import of the contents of a gettexts mo-file )
( into a message catalog. The mo-file uses 4 byte pointers and 1 byte        )
( characters, so this module has an environmental dependency.                )


1 constant gmo.version


( Private structures )

begin-structure gmo%hdr%
  lfield:   gmo>hdr>magic         \ Magic number 
  lfield:   gmo>hdr>version       \ File version
  lfield:   gmo>hdr>number        \ Number of entries
  lfield:   gmo>hdr>msg-off       \ Offset for first message
  lfield:   gmo>hdr>trn-off       \ Offset for first translation
end-structure
 
begin-structure gmo%pair%
  lfield:   gmo>pair>len          \ Message length
  lfield:   gmo>pair>off          \ Message offset
end-structure

begin-structure gmo%
  gmo%hdr%
  +field    gmo>hdr               \ mo-file header
  field:    gmo>msgs              \ Messages
  field:    gmo>trns              \ Translations
  field:    gmo>file              \ File
  field:    gmo>msc               \ Message catalog
  field:    gmo>msg               \ message string
  field:    gmo>trn               \ Translation string
end-structure


( Private words )

: gmo-new  ( msc fileid -- gmo = Build the gmo variable with the message catalog and the file )
  
  gmo% allocate throw
  
  tuck gmo>file !
  tuck gmo>msc  !
  
  dup  gmo>msgs nil!
  dup  gmo>trns nil!
  
  0 over gmo>hdr>magic   l!
  0 over gmo>hdr>version l!
  0 over gmo>hdr>number  l!
  0 over gmo>hdr>msg-off l!
  0 over gmo>hdr>trn-off l!
  
  str-new 
  256 over str-size!              \ Start with reasonable size
  over gmo>msg !
  
  str-new
  256 over str-size!
  over gmo>trn !
;


: gmo-free  ( gmo -- = Free the gmo variable )
  
  dup gmo>file @ close-file drop
  
  dup gmo>msg  @ str-free
  dup gmo>trn  @ str-free
  
  dup gmo>msgs @ ?free throw
  dup gmo>trns @ ?free throw
  
  dup gmo>msc nil!
  
  free throw
;

  
: gmo-read-header  ( gmo -- 0 | ior = Read the header from the mo-file )
  >r
  
  r@ gmo>hdr gmo%hdr% r@ gmo>file @ read-file ?dup 0= IF
    gmo%hdr% <> exp-no-data AND
  THEN
  
  ?dup 0= IF
    r@ gmo>hdr>magic l@ [ hex ] 950412DE [ decimal ] <> exp-wrong-file-type AND
  THEN
  
  ?dup 0= IF
    r@ gmo>hdr>version l@ 0 <> exp-wrong-file-version AND
  THEN
  
  ?dup 0= IF
    r@ gmo>hdr>msg-off l@ 0  r@ gmo>file @  reposition-file
  THEN
  
  ?dup 0= IF
    r@ gmo>hdr>number l@ gmo%pair% *
    
    dup allocate throw
    dup r@ gmo>msgs !
    
    over r@ gmo>file @ read-file ?dup 0= IF
      <> exp-no-data AND
    THEN
  THEN
  
  ?dup 0= IF
    r@ gmo>hdr>trn-off l@ 0  r@ gmo>file @  reposition-file
  THEN
  
  ?dup 0= IF
    r@ gmo>hdr>number l@ gmo%pair% *
    
    dup allocate throw
    dup r@ gmo>trns !
    
    over r@ gmo>file @ read-file ?dup 0= IF
      <> exp-no-data AND
    THEN
  THEN
  rdrop
;  


: gmo-read-msg    ( pair str fileid -- 0 | ior = Read one message from the mo-file into the string )
  rot 
  dup gmo>pair>len l@ >r                    \ Save length of message
  
  r@ 0= IF                                  \ If length = 0 Then
    2drop
    str-clear                               \   Clear string and okee
    0
  ELSE                                      \ Else move the file pointer to the offset
    gmo>pair>off l@  over  0 swap  reposition-file ?dup IF
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


: gmo-read-msgs   ( gmo -- 0 | ior = Read the messages from the mo-file and stores them in the message catalog )
  >r
  
  r@ gmo>trns @                   \ Translation pointer
  r@ gmo>msgs @                   \ Message pointer
  r@ gmo>hdr>number l@            \ count
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

: gmo-read  ( c-addr u msc -- 0 | ior = Read a mo-file named c-addr u and store the contents in the message catalog msc )
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
.( Warning: gmo requires at least 4 byte cells and 1 byte chars ) cr
[THEN]

[THEN]

\ ==============================================================================
