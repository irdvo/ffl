\ ==============================================================================
\
\                 arg - the argument parser in the ffl
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
\  $Date: 2007-07-12 18:29:38 $ $Revision: 1.3 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] arg.version [IF]

[DEFINED] #args [DEFINED] arg AND [IF]


include ffl/snl.fs
include ffl/sni.fs
include ffl/str.fs

( arg = Arguments parser )
( The arg parser module implements words for parsing command line arguments. )
( Due to the fact that the ANS standard does not specify words for arguments )
( this module has a environmental dependency.                                )
( <pre>                                                                      )
(                                                                            )
(     Supported option formats:                                              )
(        -v            short switch option                                   )
(        -f a.txt      short option with parameter                           )
(        -vq           multiple short swith options                          )
(        --file=a.txt  long option with parameter                            )
(        --verbose     long switch option                                    )
(        --            stop parsing arguments                                )
( </pre>                                                                     )


1 constant arg.version



( Global setting )

[DEFINED] cols [IF]
 0 VALUE arg.cols    ( - n = Value with the number of columns for parser output [0=use cols word] )
[ELSE]
80 VALUE arg.cols
[THEN]



( Private structure )

struct: arg%opt%   ( - n = Get the required size for the argument option structure )
  snn%
  field: arg>opt>snn         \ this structure is extending the single list node
  cell:  arg>opt>data        \ the optional data for the callback
  cell:  arg>opt>xt          \ the callback xt
  cell:  arg>opt>switch      \ is the option a switch (no parameter) ?
  char:  arg>opt>short       \ the short option or 0
  cell:  arg>opt>long        \ the long option or ""
  cell:  arg>opt>descr       \ the description of option
;struct


: arg-opt-init   ( c:short c-addr:long u c-addr:descr u f:switch w:data xt:callback w:opt - = Initialise the arg>opt structure )
  >r
  r@ arg>opt>snn      snn-init
  r@ arg>opt>xt     !
  r@ arg>opt>data   !
  r@ arg>opt>switch !
  str-new dup 
  r@ arg>opt>descr  ! str-set
  str-new dup 
  r@ arg>opt>long   ! str-set
  r> arg>opt>short c!
;


: arg-opt-new   ( c:short c-addr:long u c-addr:descr u f:switch w:data xt:callback - w:opt = Create a new arg-opt structure on the heap )
  arg%opt% allocate  throw   >r r@ arg-opt-init r>
;


: arg-opt-free   ( w:opt - = Free the arg.opt structure from the heap )
  dup arg>opt>long  @ str-free
  dup arg>opt>descr @ str-free
    
  free throw
;


: arg-opt-print   ( n:length w:opt - n:length = Print one argument option )
  >r
  2 spaces                             \ ToDo: compacter
  r@ arg>opt>short c@ ?dup IF          \ Print the optional short option
    [char] - emit emit
  ELSE
    2 spaces
  THEN
                                       \ Print the comma if both are present
  r@ arg>opt>short c@ 0<>  r@ arg>opt>long @ str-empty? 0= AND IF
    [char] ,
  ELSE
    bl
  THEN
  emit space
  
  r@ arg>opt>long @ dup str-empty? 0= IF  \ Print the optional long option
    [char] - dup emit emit
    dup str-get type
    over swap str-length@ -            \ Calculate number filling spaces
  ELSE
    drop dup
  THEN
  1+ spaces
  r> arg>opt>descr @ str-get type cr   \ Print description: ToDo column
;



( Argument parser structure )

struct: arg%   ( - n = Get the required space for the arg data structure )
  snl%
  field: arg>options    \ the option list
  sni%
  field: arg>iter       \ the iterator on the option list
  cell:  arg>name       \ the program name
  cell:  arg>usage      \ the program usage
  cell:  arg>version    \ the program version
  cell:  arg>tail       \ the program tail
  cell:  arg>data       \ the optional data
  cell:  arg>xt         \ the non-option callback xt
  cell:  arg>length     \ the length of the longest long option
;struct



( Argument parser structure creation, initialisation and destruction )

: arg-init   ( c-addr:name u c-addr:usage u c-addr:version u c-addr:tail u w:arg - = Initialise the argument parser structure )
  >r
  r@ arg>options             snl-init
  r@ arg>options r@ arg>iter sni-init
  str-new dup
  r@ arg>tail    ! str-set
  str-new dup
  r@ arg>version ! str-set
  str-new dup
  r@ arg>usage   ! str-set
  str-new dup
  r@ arg>name    ! str-set
  r@ arg>data nil!
  r> arg>xt   nil!
;


: arg-create   ( C: "name"  c-addr:name u c-addr:usage u c-addr:version u c-addr:tail u -  R: - w:arg = Create a named argument parser structure in the dictionary )
  create  here arg% allot  arg-init
;


: arg-new   (  c-addr:name u  c-addr:usage u c-addr:version u c-addr:tail u - w:arg = Create a new argument parser structure on the heap )
  arg% allocate  throw   >r r@ arg-init r>
;


: arg-free   ( w:arg - = Free the argument parser structure from the heap )
  dup arg>name    @ str-free
  dup arg>usage   @ str-free
  dup arg>version @ str-free
  dup arg>tail    @ str-free
  
  dup arg>options                 \ Free all argument options
  BEGIN
    dup snl-remove-first dup nil<>
  WHILE
    arg-opt-free
  REPEAT
  2drop  

  free throw
;


( Default print words )

: arg-print-version   ( w:arg - false = Print the version info )
  dup arg>name    @ str-get type space
      arg>version @ str-get type cr
  
  \ ToDo: column
  ." This is free software; see the source for copying conditions. There is NO" cr
  ." warranty; not even for MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE." cr
  false
;


: arg-print-help   ( w:arg - false = Print the help info )
  >r
  ." Usage: " 
  r@ arg>name   @ str-get type space
  r@ arg>usage  @ str-get type cr cr
  r@ arg>length @ ['] arg-opt-print r@ arg>options snl-execute drop cr
  r> arg>tail   @ str-get type cr
  false
;


( Option words )

: arg-add-option   ( c:short c-addr:long u c-addr:descr u f:switch w:data xt:callback w:arg - = Add an option to the argument parser )
  >r
  arg-opt-new                          \ Create the option
  dup arg>opt>long @ str-length@       \ Determine length of long option
  r@  arg>length @ max r@ arg>length ! \ Save the length of the longest long option
  r>  arg>options snl-append           \ Append in the options list
;


: arg-add-default-options   ( w:arg - = Add the default help and version options )
  >r
  [char] ? s" help"    s" show this help"    true r@ ['] arg-print-help    r@ arg-add-option
         0 s" version" s" show version info" true r@ ['] arg-print-version r> arg-add-option
;


( Private parser words )

: arg-find-short   ( c:option w:arg - w:opt | nil = Find a short option in the option list )
  arg>iter 
  tuck sni-first                  \ Iterate the option list
  BEGIN                           \ S: iter c opt
    dup nil<> IF
      2dup arg>opt>short c@ <>    \   Check for short option in list
    ELSE
      false
    THEN
  WHILE
    drop over sni-next
  REPEAT
  nip nip
;


: arg-parse-short   ( n:index c-addr:option u w:arg - n:index f = Parse a short option )
;


: arg-find-long   ( c-addr:option u w:arg - w:opt | nil = Find a long option the option list )
  arg>iter 
  dup sni-first                   \ Iterate the option list
  BEGIN                           \ S: c-addr u iter opt
    dup nil<> IF
      2over 2over nip
      arg>opt>long @ str-ccompare 0<>  \ Check for long option in list
    ELSE
      false
    THEN
  WHILE
    drop dup sni-next
  REPEAT
  nip nip nip
;


: arg+split-option   ( c-addr u - u = Split a option in option and parameter )
  0 -rot
  bounds ?DO                 \ S: u
    I c@ [char] = = IF
      LEAVE
    THEN
    1+
  LOOP
;


: arg-parse-long   ( n:index c-addr:option u w:arg - n:index f = Parse a long option )
  >r
  2dup arg+split-option
  >r over r> tuck                 \ S: n c-addr u u' c-addr u'
  
  r> arg-find-long
  
  dup nil<> IF                    \ If long option found Then
    >r
    r@ arg>opt>switch @ IF        \   If switch option Then
      over = IF                   \     If no '=' in option Then
        2drop
        r@ arg>opt>data @ r@ arg>opt>xt @ execute
      ELSE
        ." Unexpected parameter for switch option: --" type cr
        false
      THEN
    ELSE                          \ Else
      >r 2dup r> 1+ /string       \   Remove option= from option
      dup 0> IF
        r@ arg>opt>data @ r@ arg>opt>xt @ execute 
        nip nip
      ELSE
        2drop
        ." Expecting parameter for option: --" type cr
        false
      THEN
    THEN
    rdrop
  ELSE
    2drop 
    ." Unknown option: --" type cr
    false
  THEN
;


( Parser words )

: arg-parse   ( w:data xt w:arg - f:ok = Parse the command line arguments, xt is called for every argument that is not an option )
  >r
  true 1
  BEGIN
    2dup #args < AND
  WHILE                                \ Do for arg1 to #args
    nip
    dup arg
    over c@ [char] - = IF              \   If argument start with - Then
      1 /string
      dup 0> IF
        over c@ [char] - = IF          \     If next in argument is - Then
          1 /string
          dup 0> IF                    \       If more in argument Then
            r@ arg-parse-long          \         Parse long option
          ELSE                         \       Else
            drop #args true            \         Done ('--')
          THEN
        ELSE                           \     Else
          r@ arg-parse-short           \       Parse short option
        THEN
      ELSE                             \    Else
        ." Invalid option: -" cr       \      Invalid option
        false
      THEN
    ELSE                               \   Else
      r@ arg>data @ r@ arg>xt @ execute \    Not an option..
    THEN
    swap 1+
  REPEAT
  drop
;


( Inspection )

: arg-dump   ( w:arg - = Dump the arg state )
  \ ToDo
;

[ELSE]
.( Module arg requires #args and arg.)
[THEN]

[THEN]

\ ==============================================================================
