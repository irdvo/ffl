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
\  $Date: 2008-05-19 05:43:45 $ $Revision: 1.13 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] arg.version [IF]

[DEFINED] #args [DEFINED] arg@ AND [IF]

include ffl/snl.fs
include ffl/sni.fs
include ffl/tis.fs


( arg = Arguments parser )
( The arg parser module implements a command line arguments parser.          )
( Due to the fact that the ANS standard does not specify words for arguments )
( this module has a environmental dependency.                                )
( {{{                                                                        )
(   Supported option formats:                                                )
(      -v            short switch option                                     )
(      -f a.txt      short option with parameter                             )
(      -vq           multiple short switch options                           )
(      --file=a.txt  long option with parameter                              )
(      --verbose     long switch option                                      )
(      --            stop parsing arguments                                  )
( }}}                                                                        )


1 constant arg.version



( Global setting )

79 value arg.cols    ( -- n = Value with the number of columns for parser output [def. 79] )


( Constants )

 3 constant arg.version-option   ( -- n = Version option  )
 2 constant arg.help-option      ( -- n = Help option     )
 1 constant arg.non-option       ( -- n = Non option      )
 0 constant arg.done             ( -- n = Done parsing    )
-1 constant arg.error            ( -- n = Error in option )


( Private structure )

begin-structure arg%opt%   ( -- n = Get the required size for an argument option )
  snn%
  +field  arg>opt>snn        \ this structure is extending the single list node
  field:  arg>opt>id         \ the option id (4..)
  field:  arg>opt>switch     \ is the option a switch (no parameter) ?
  field:  arg>opt>short      \ the short option or 0
  field:  arg>opt>long       \ the long option or ""
  field:  arg>opt>descr      \ the description of option
end-structure


: arg-opt-init ( char c-addr1 u1 c-addr2 u2 flag n opt -- = Initialise a option with the short option char, the long option c-addr1 u1, the description c-addr2 u2, the switch flag and the identifier n [id=4..])
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


: arg-opt-new   ( char c-addr1 u1 c-addr2 u2 flag n -- opt = Create a new option on the heap with the short option char, the long option c-addr1 u1, the description c-addr2 u2, the switch flag and the identifier n [id=4..])
  arg%opt% allocate  throw   >r r@ arg-opt-init r>
;


: arg-opt-free   ( opt -- = Free the option from the heap )
  dup arg>opt>long  @ str-free
  dup arg>opt>descr @ str-free
    
  free throw
;


: arg-opt-print   ( n opt -- n = Print one option with maximum length n )
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

begin-structure arg%   ( -- n = Get the required space for an argument parser variable )
  tis%
  +field arg>arg        \ the current argument
  snl%
  +field arg>options    \ the option list
  sni%
  +field arg>iter       \ the iterator on the option list
  field: arg>name       \ the program name
  field: arg>usage      \ the program usage
  field: arg>version    \ the program version
  field: arg>tail       \ the program extra info
  field: arg>index      \ the argument index
  field: arg>length     \ the length of the longest long option
end-structure



( Argument parser structure creation, initialisation and destruction )

: arg-init ( c-addr1 u1 c-addr2 u2 c-addr3 u3 c-addr4 u4 arg -- = Initialise the parser with the program name c-addr1 u1, the usage c-addr2 u2, the version c-addr3 u3 and general info c-addr4 u4 )
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


: arg-(free)   ( arg -- = Free the internal, private variables from the heap )
  dup arg>arg       tis-(free)
  
  dup arg>name    @ str-free
  dup arg>usage   @ str-free
  dup arg>version @ str-free
  dup arg>tail    @ str-free
  
  arg>options                     \ Free all argument options
  BEGIN
    dup snl-remove-first dup nil<>
  WHILE
    arg-opt-free
  REPEAT
  2drop  
;


: arg-create ( c-addr1 u1 c-addr2 u2 c-addr3 u3 c-addr4 u4 "<spaces>name" -- ; -- arg = Create a named parser in the dictionary with the program name c-addr1 u1, the usage c-addr2 u2, the version c-addr3 u3 and general info c-addr4 u4 )
  create  here arg% allot  arg-init
;


: arg-new (  c-addr1 u1  c-addr2 u2 c-addr3 u3 c-addr4 u4 -- arg = Create a new parser on the heap with the program name c-addr1 u1, the usage c-addr2 u2, the version c-addr3 u3 and general info c-addr4 u4 )
  arg% allocate  throw   >r r@ arg-init r>
;


: arg-free   ( arg -- = Free the parser from the heap )
  dup arg-(free)             \ Free the private variables

  free throw                 \ Free the parser
;


( Default print words )

: arg-print-version   ( arg -- = Print the version info )
  dup arg>name    @ str-get type space
      arg>version @ str-get type cr
      
  s" This is free software; see the source for copying conditions. There is NO warranty; not even for MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE."
      
  arg.cols 20 max str+columns     \ Split the string in columns
  
  0 DO                            \ Type the columns
    type cr
  LOOP
;


: arg-print-help   ( arg -- = Print the help info )
  >r
  ." Usage: " 
  r@ arg>name   @ str-get type space
  r@ arg>usage  @ str-get type cr cr
  r@ arg>length @ ['] arg-opt-print r@ arg>options snl-execute drop cr
  r> arg>tail   @ str-get type cr
;


( Option words )

: arg-add-option ( char c-addr1 u1 c-addr2 u2 flag n arg -- = Add an option to the parser with the short option char, the long option c-addr1 u1, the description c-addr2 u2, the switch flag and the identifier n [id=4..])
  >r
  arg-opt-new                          \ Create the option
  dup arg>opt>long @ str-length@       \ Determine length of long option
  r@  arg>length @ max r@ arg>length ! \ Save the length of the longest long option
  r>  arg>options snl-append           \ Append in the options list
;


: arg-add-help-option   ( arg -- = Add the default help option )
  >r [char] ? s" help" s" show this help" true arg.help-option r> arg-add-option
;


: arg-add-version-option   ( arg -- = Add the default version option )
  >r 0 s" version" s" show version info" true arg.version-option r> arg-add-option
;


( Private parser errors )

: arg+exp-param-str   ( -- c-addr u = Warning: Expecting parameter for option string)
  s" Expecting parameter for option: -"
;

: arg+unk-opt-str   ( -- c-addr u = Warning: Unknown option string)
  s" Unknown option: -"  
;

: arg+inv-opt-str   ( -- c c-addr u = Warning: Invalid option string)
  [char] - s" Invalid option: " 
;

: arg+do-short-error   ( char c-addr u -- n = Generate an error for short option char, return the error code )
  type emit cr
  arg.error
;

: arg+do-long-error   ( c-addr1 u1 c-addr2 u2 -- id = Generate an error c-addr2 u2 for a long option c-addr1 u1, return the error code )
  type [char] - emit type cr
  arg.error
;


( Private parser words )

: arg-find-short   ( char arg -- opt | nil = Find a short option char in the option list )
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


: arg-parse-short   ( arg -- c-addr u n | n = Parse a short option, return the option identifier and optional the parameter )
  >r
  r@ arg>arg tis-read-char IF               \ Read the option character
    dup r@ arg-find-short                   \ Find it in the list
    nil<>? IF                               \ If found Then
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
      arg+unk-opt-str arg+do-short-error
    THEN
  ELSE
    [char] - 
    arg+inv-opt-str arg+do-short-error
  THEN
  rdrop
;


: arg-compare-long   ( c-addr1 u1 c-addr2 u2 -- flag = Compare two long options )
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


: arg-find-long   ( c-addr u arg -- opt | nil = Find a long option c-addr u in the option list, return the option )
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


: arg-parse-long   ( arg -- c-addr u n | n = Parse a long option, return the option identifier and optional the parameter )
  >r
  r@ arg>arg tis-eof? IF
    arg.done                                \ Done ('--')
  ELSE
    [char] = r@ arg>arg tis-scan-char IF
      2dup r@ arg-find-long
      nil<>? IF
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
        arg+unk-opt-str arg+do-long-error
      THEN
    ELSE
      r@ arg>arg tis-read-all
      2dup r@ arg-find-long
      nil<>? IF
        dup arg>opt>switch @ IF
          nip nip
          arg>opt>id @
        ELSE
          drop
          arg+exp-param-str arg+do-long-error
        THEN
      ELSE
        arg+unk-opt-str arg+do-long-error
      THEN
    THEN
  THEN
  rdrop
;


: arg-parse-opt   ( arg -- c-addr u id | id = Parse the next option, return the option identifier and optional the parameter )
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

: arg-parse   ( arg -- c-addr u n | n = Parse the next command line argument, return the option identifier and optional the parameter )
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
.( Warning: arg requires #args and arg@.) cr
[THEN]

[THEN]

\ ==============================================================================
