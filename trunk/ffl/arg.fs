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
\  $Date: 2007-10-09 17:31:07 $ $Revision: 1.7 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] arg.version [IF]

[DEFINED] #args [DEFINED] arg@ AND [IF]


include ffl/snl.fs
include ffl/sni.fs
include ffl/tis.fs

( arg = Arguments parser )
( The arg parser module implements words for parsing command line arguments. )
( Due to the fact that the ANS standard does not specify words for arguments )
( this module has a environmental dependency.                                )
( <pre>                                                                      )
(   Supported option formats:                                                )
(      -v            short switch option                                     )
(      -f a.txt      short option with parameter                             )
(      -vq           multiple short switch options                           )
(      --file=a.txt  long option with parameter                              )
(      --verbose     long switch option                                      )
(      --            stop parsing arguments                                  )
( </pre>                                                                     )


1 constant arg.version



( Global setting )

79 value arg.cols    ( - n = Value with the number of columns for parser output [def. 79] )


( Constants )

 3 constant arg.version-option
 2 constant arg.help-option
 1 constant arg.non-option
 0 constant arg.done
-1 constant arg.error


( Private structure )

struct: arg%opt%   ( - n = Get the required size for the argument option structure )
  snn%
  field: arg>opt>snn         \ this structure is extending the single list node
  cell:  arg>opt>id          \ the option id (4..)
  cell:  arg>opt>switch      \ is the option a switch (no parameter) ?
  char:  arg>opt>short       \ the short option or 0
  cell:  arg>opt>long        \ the long option or ""
  cell:  arg>opt>descr       \ the description of option
;struct


: arg-opt-init   ( c:short c-addr:long u c-addr:descr u f:switch n:id w:opt - = Initialise the arg>opt structure [id=4..])
  >r
  r@ arg>opt>snn      snn-init
  
  r@ arg>opt>id     !
  
  r@ arg>opt>switch !
  
  str-new dup 
  r@ arg>opt>descr  ! str-set
  
  str-new dup 
  r@ arg>opt>long   ! str-set
  
  r> arg>opt>short c!
;


: arg-opt-new   ( c:short c-addr:long u c-addr:descr u f:switch n:id - w:opt = Create a new arg-opt structure on the heap [id=4..])
  arg%opt% allocate  throw   >r r@ arg-opt-init r>
;


: arg-opt-free   ( w:opt - = Free the arg.opt structure from the heap )
  dup arg>opt>long  @ str-free
  dup arg>opt>descr @ str-free
    
  free throw
;


: arg-opt-print   ( n:length w:opt - n:length = Print one argument option )
  swap >r
  dup  arg>opt>short c@
  over arg>opt>long   @
  
  2 spaces
  over ?dup IF                         \ Print the optional short option
    [char] - emit emit
  ELSE
    2 spaces
  THEN
                                       \ Print the comma if both are present
  tuck str-empty? 0= AND IF
    [char] ,
  ELSE
    bl
  THEN
  emit space
  
  dup str-empty? 0= IF                 \ Print the optional long option
    [char] - dup emit emit
    dup str-get type
    r@ swap str-length@ -              \ Calculate number filling spaces
  ELSE
    drop r@ 1+ 1+
  THEN
  1+ spaces
  
  arg>opt>descr @ str-get
  arg.cols 40 max r@ 9 + - str+columns \ Format the description in columns
  
  dup 0> IF                            \ print the first line after the options
    >r type cr r>
    1-
  THEN
  
  BEGIN                               \ Type any remaining info on empty lines
    dup 0>
  WHILE
    r@ 9 + spaces
    >r type cr r>
    1-
  REPEAT
  drop
  r>
;


( Argument parser structure )

struct: arg%   ( - n = Get the required space for the arg data structure )
  tis%
  field: arg>arg        \ the current argument
  snl%
  field: arg>options    \ the option list
  sni%
  field: arg>iter       \ the iterator on the option list
  cell:  arg>name       \ the program name
  cell:  arg>usage      \ the program usage
  cell:  arg>version    \ the program version
  cell:  arg>tail       \ the program extra info
  cell:  arg>index      \ the argument index
  cell:  arg>length     \ the length of the longest long option
;struct



( Argument parser structure creation, initialisation and destruction )

: arg-init   ( c-addr:name u c-addr:usage u c-addr:version u c-addr:info u w:arg - = Initialise the argument parser structure )
  >r
  r@ arg>options             snl-init
  r@ arg>options r@ arg>iter sni-init
  r@ arg>arg                 tis-init
  
  str-new dup
  r@ arg>tail    ! str-set
  
  str-new dup
  r@ arg>version ! str-set
  
  str-new dup
  r@ arg>usage   ! str-set
  
  str-new dup
  r@ arg>name    ! str-set
  
  r@ arg>index  0!
  r> arg>length 0!
;


: arg-create   ( C: "name"  c-addr:name u c-addr:usage u c-addr:version u c-addr:info u -  R: - w:arg = Create a named argument parser structure in the dictionary )
  create  here arg% allot  arg-init
;


: arg-new   (  c-addr:name u  c-addr:usage u c-addr:version u c-addr:info u - w:arg = Create a new argument parser structure on the heap )
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

  tis-free
;


( Default print words )

: arg-print-version   ( w:arg - = Print the version info )
  dup arg>name    @ str-get type space
      arg>version @ str-get type cr
      
  s" This is free software; see the source for copying conditions. There is NO warranty; not even for MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE."
      
  arg.cols 20 max str+columns     \ Split the string in columns
  
  0 DO                            \ Type the columns
    type cr
  LOOP
;


: arg-print-help   ( w:arg - = Print the help info )
  >r
  ." Usage: " 
  r@ arg>name   @ str-get type space
  r@ arg>usage  @ str-get type cr cr
  r@ arg>length @ ['] arg-opt-print r@ arg>options snl-execute drop cr
  r> arg>tail   @ str-get type cr
;


( Option words )

: arg-add-option   ( c:short c-addr:long u c-addr:descr u f:switch n:id w:arg - = Add an option to the argument parser [id=4..])
  >r
  arg-opt-new                          \ Create the option
  dup arg>opt>long @ str-length@       \ Determine length of long option
  r@  arg>length @ max r@ arg>length ! \ Save the length of the longest long option
  r>  arg>options snl-append           \ Append in the options list
;


: arg-add-help-option   ( w:arg - = Add the default help option )
  >r [char] ? s" help" s" show this help" true arg.help-option r> arg-add-option
;


: arg-add-version-option   ( w:arg - = Add the default version option )
  >r 0 s" version" s" show version info" true arg.version-option r> arg-add-option
;


( Private parser errors )

: arg+exp-param-str   ( - c-addr u =  Expecting parameter for option string)
  s" Expecting parameter for option: -"
;

: arg+unk-opt-str   ( - c-addr u =  Unknown option string)
  s" Unknown option: -"  
;

: arg+inv-opt-str   ( - c c-addr u = Invalid option string)
  [char] - s" Invalid option: " 
;

: arg+do-short-error   ( c c-addr u - id = Generate an error for a short option )
  type emit cr
  arg.error
;

: arg+do-long-error   ( c-addr u c-addr u - id = Generate an error for a long option )
  type [char] - emit type cr
  arg.error
;


( Private parser words )

: arg-find-short   ( c w:arg - w:opt | nil = Find a short option in the option list )
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


: arg-parse-short   ( w:arg - c-addr u n:id | n:id = Parse a short option )
  >r
  r@ arg>arg tis-read-char IF               \ Read the option character
    dup r@ arg-find-short                   \ Find it in the list
    dup nil<> IF                            \ If found Then
      dup arg>opt>switch @ IF               \   If switch Then return id
        nip
        arg>opt>id @
      ELSE                                  \ Else If last option Then
        r@ arg>arg tis-eof? IF
          r@ arg>index @ #args < IF
            nip
            r@ arg>index dup @
            swap 1+!
            arg@                            \   Read next argument and return id
            rot arg>opt>id @
          ELSE
            drop
            arg+exp-param-str arg+do-short-error
          THEN
        ELSE
          drop
          arg+exp-param-str arg+do-short-error
        THEN
      THEN
    ELSE
      drop
      arg+unk-opt-str arg+do-short-error
    THEN
  ELSE
    [char] - 
    arg+inv-opt-str arg+do-short-error
  THEN
  rdrop
;


: arg-compare-long   ( c-addr u c-addr u - f = Compare two long options )
  rot
  2dup < IF                       \ If option 2 is shorter than option 1 -> not equal
    2drop 2drop
    false exit
  THEN
  
  tuck > IF                       \ If option 2 is longer than option 1 Then
    2dup chars +
    c@ [char] = <> IF             \   The longer char must be '=' else not equal
      2drop drop
      false exit
    THEN
  THEN
  
  tuck compare 0=                 \ Compare
;


: arg-find-long   ( c-addr u w:arg - w:opt | nil = Find a long option in the option list )
  arg>iter 
  dup sni-first                   \ Iterate the option list
  BEGIN                           \ S: c-addr u iter opt
    dup nil<> IF
      2over 2over nip
      arg>opt>long @ str-get arg-compare-long 0=  \ Check for long option in list
    ELSE
      false
    THEN
  WHILE
    drop dup sni-next
  REPEAT
  nip nip nip
;


: arg-parse-long   ( w:arg - c-addr u n:id | n:id = Parse a long option )
  >r
  r@ arg>arg tis-eof? IF
    arg.done                                \ Done ('--')
  ELSE
    [char] = r@ arg>arg tis-scan-char IF
      2dup r@ arg-find-long
      dup nil<> IF
        dup arg>opt>switch @ IF
          drop
          ." Unexpected parameter for switch option: --" type cr
          arg.error
        ELSE
          r@ arg>arg tis-eof? IF
            drop
            arg+exp-param-str arg+do-long-error
          ELSE
            nip nip
            r@  arg>arg tis-read-all          \ read the parameter
            rot arg>opt>id @
          THEN
        THEN
      ELSE
        drop
        arg+unk-opt-str arg+do-long-error
      THEN
    ELSE
      r@ arg>arg tis-read-all
      2dup r@ arg-find-long
      dup nil<> IF
        dup arg>opt>switch @ IF
          nip nip
          arg>opt>id @
        ELSE
          drop
          arg+exp-param-str arg+do-long-error
        THEN
      ELSE
        drop
        arg+unk-opt-str arg+do-long-error
      THEN
    THEN
  THEN
  rdrop
;


: arg-parse-opt   ( w:arg - c-addr u n:id | n:id = Parse the option )
  >r
  r@ arg>arg tis-eof? IF
    arg+inv-opt-str arg+do-short-error
  ELSE
    [char] - r@ arg>arg tis-cmatch-char IF
      r@ arg-parse-long
    ELSE
      r@ arg-parse-short
    THEN
  THEN
  rdrop
;


( Parser words )

: arg-parse   ( w:arg - c-addr u n:id | n:id = Parse the next command line argument )
  >r
  r@ arg>arg tis-eof? IF
    r@ arg>index dup @ dup #args < IF
      arg@ r@ arg>arg tis-set
      1+!
      
      [char] - r@ arg>arg tis-cmatch-char IF
        r@ arg-parse-opt
      ELSE
        r@ arg>arg tis-read-all
        arg.non-option
      THEN
    ELSE
      2drop
      arg.done
    THEN
  ELSE
    r@ arg-parse-short
  THEN
  rdrop
;

[ELSE]
.( Module arg requires #args and arg@.) cr
[THEN]

[THEN]

\ ==============================================================================
