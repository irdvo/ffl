\ ==============================================================================
\
\             gsv - the gtk-server interface module in the ffl
\
\               Copyright (C) 2009  Dick van Oudheusden
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


include ffl/config.fs

[UNDEFINED] gsv.version [IF]

include ffl/tis.fs
include ffl/tos.fs
include ffl/est.fs
include ffl/spf.fs
include ffl/scf.fs

( gsv = gtk-server interface )
( The gsv module implements an interface to the gtk-server. This server       )
( makes it possible for an ANSish forth engine to show a gtk GUI. Due to the  )
( fact that using this module requires an external tool, this module has an   )
( environmental dependency. See [GTKserver] for how to use this module.       )

1 constant gsv.version


( private module variables )

str-create gsv.in
str-create gsv.out

str-create gsv.cmd

260 gsv.cmd str-size!


( Low level, overrulable gtk-server connection words )

[UNDEFINED] gsv+connect [IF]
: gsv+connect      ( c-addr u -- ior = Connect to the gtk-server via a fifo c-addr u )
  2dup
  gsv.in  str-set            \ No special connection necessary for named pipes
  gsv.out str-set
  0
;
[THEN]


[UNDEFINED] gsv+call [IF]
: gsv+call         ( c-addr1 u1 -- c-addr2 u2 = Call the gtk-server with command c-addr1 u1, resulting in response c-addr2 u2 )
  \ ." Sending:" 2dup type cr
  over swap
  gsv.out str-get w/o open-file throw 
  >r  
  r@ write-file throw        ( Call the server, write-line results in a SIGPIPE )
  r> close-file throw
  
  gsv.in  str-get r/o open-file throw 
  >r
  dup 256  r@ read-line throw drop
  r> close-file throw        ( Receive the response )
  \ ." Receiving:" 2dup type cr
;
[THEN]


[UNDEFINED] gsv+disconnect [IF]
: gsv+disconnect   ( -- ior = Disconnect from the gtk-server, the gtk-server is *NOT* closed )
  0
;
[THEN]


( private gtk-server connection words )

: gsv+invoke       ( i*x j*r c-addr1 u1 c-addr2 u2 -- k*x l*r  = Call the gtk-server with parameters i*x j*r and format string c-addr1 u1 and return specifier string c-addr2 u2 )
  2>r 
  gsv.cmd spf-set                \ Format the call
  gsv.cmd str-get gsv+call       \ Send the call the gtk-server and receive the response
  2r> 
  scf+scan drop                  \ Scan the response
;


( private gtk-server configuration file parser words )

: gsv+parse-arg    ( tos tis -- = Parse and compile an argument )
  2>r
  r@ tis-skip-spaces drop
  [char] , r@ tis-scan-char 0= IF
    r@ tis-read-all 
  THEN                                 \ S: c-addr u | 0
  ?dup IF
    -trailing
    2dup s" NULL" icompare 0= IF
      s"  NULL"
    ELSE 2dup s" WIDGET" icompare 0= IF
      s"  %d"
    ELSE 2dup s" PTR_WIDGET" icompare 0= IF
      s"  %d"
    ELSE 2dup s" POINTER" icompare 0= IF
      s"  %d"
    ELSE 2dup s" ADDRESS" icompare 0= IF
      s"  %d"
    ELSE 2dup s" BOOL" icompare 0= IF
      s"  %d"
    ELSE 2dup s" PTR_BOOL" icompare 0= IF
      s"  %d"
    ELSE 2dup s" STRING" icompare 0= IF
      s"  %q"
    ELSE 2dup s" PTR_STRING" icompare 0= IF
      s"  %q"
    ELSE 2dup s" INT" icompare 0= IF
      s"  %d"
    ELSE 2dup s" PTR_INT" icompare 0= IF
      s"  %d"
    ELSE 2dup s" LONG" icompare 0= IF
      s"  %ld"
    ELSE 2dup s" PTR_LONG" icompare 0= IF
      s"  %ld"
    ELSE 2dup s" DOUBLE" icompare 0= IF
      s"  %E"
    ELSE 2dup s" PTR_DOUBLE" icompare 0= IF
      s"  %E"
    ELSE 2dup s" FLOAT" icompare 0= IF
      s"  %E"
    ELSE 2dup s" PTR_FLOAT" icompare 0= IF
      s"  %E"
    ELSE 2dup s" NONE" icompare 0= IF
      s" "
    ELSE 2dup s" VOID" icompare 0= IF
      s" "
    ELSE 2dup s" MACRO" icompare 0= IF
      s"  %s"
    ELSE 2dup s" BASE64" icompare 0= IF
      s"  %s"
    ELSE
      ." Unexpected gtk-server type:" 2dup type cr s" "
    THEN THEN THEN THEN THEN THEN THEN THEN THEN THEN THEN THEN THEN THEN THEN THEN THEN THEN THEN THEN THEN
    r'@ tos-write-string
    2drop
  THEN
  rdrop rdrop
;
  

: gsv+parse-return  ( tos tis -- = Parse and compile the return argument )
  2>r
  r@ tis-skip-spaces drop
  [char] , r@ tis-scan-char 0= IF
    r@ tis-read-all 
  THEN                                 \ S: c-addr u | 0
  ?dup IF
    -trailing
    2dup s" PTR_WIDGET" icompare 0= IF
      s"  %d" true
    ELSE 2dup s" PTR_BOOL" icompare 0= IF
      s"  %d" true
    ELSE 2dup s" PTR_STRING" icompare 0= IF
      s"  %q" true
    ELSE 2dup s" PTR_INT" icompare 0= IF
      s"  %d" true
    ELSE 2dup s" PTR_LONG" icompare 0= IF
      s"  %ld" true
    ELSE 2dup s" PTR_DOUBLE" icompare 0= IF
      s"  %E" true
    ELSE 2dup s" PTR_FLOAT" icompare 0= IF
      s"  %E" true
    ELSE
      false
    THEN THEN THEN THEN THEN THEN THEN
    
    IF 
      r'@ tos-write-string
    THEN
    2drop
  THEN
  rdrop rdrop
;


: gsv+parse-function  ( tos tis -- = Parse and compile a function from the configuration line )
  2>r
  r@ tis-skip-spaces drop
  [char] = r@ tis-cmatch-char IF
    r@ tis-skip-spaces drop
    [char] , r@ tis-scan-char IF       \ Scan the name of the function

      r'@ tos-rewrite                  \ Compile the name of the function
      s" : "       r'@ tos-write-string      \   Start definition
      2dup         r'@ tos-write-string      \   Write name of function
      s\"  s\\\q " r'@ tos-write-string
                   r'@ tos-write-string       \  Write function name in formatter

      r@ tis-skip-spaces drop
      [char] , r@ tis-scan-char IF     \ Scan the callback signal, ignore
        2drop
        r@ tis-skip-spaces drop
        r@ tis-pntr@                   \ Save position for rescanning returns
        [char] , r@ tis-scan-char IF   \ Scan for return type and ignore
          2drop
          
          r@ tis-skip-spaces drop
          r@ tis-read-number IF        \ Read number of args: S: n
            [char] , r@ tis-scan-char IF
              2drop                    \ Ignore optional comma
            THEN

            BEGIN
              ?dup
            WHILE
              2r@ gsv+parse-arg         \ Parse and compile the arguments
              1-
            REPEAT
          ELSE
            2drop
          THEN
        THEN
        
        s\" \q s\\\q " r'@ tos-write-string
        
        r@ tis-pntr! drop              \ Start scanning again for returns
        
        2r@ gsv+parse-arg              \ Parse and compile return type
        
        r@ tis-skip-spaces drop
        r@ tis-read-number IF          \ Read number of args: S: n
          [char] , r@ tis-scan-char IF
            2drop                      \ Ignore optional comma
          THEN

          BEGIN
            ?dup
          WHILE
            2r@ gsv+parse-return       \ Parse and compile return arguments
            1-
          REPEAT

          s\" \q gsv+invoke ;" r'@ tos-write-string \ Call gtk-server 

          r'@ str-get  evaluate       \ compile it
        THEN
      THEN
    THEN
  THEN
  rdrop rdrop
;


: gsv+parse-enum   ( tos tis -- = Parse and compile an enum from the configuration file )
  2>r
  base @
  r@ tis-skip-spaces drop              \ Parse the enum
  [char] = r@ tis-cmatch-char IF
    r@ tis-skip-spaces drop
    [char] , r@ tis-scan-char IF       \ S: c-addr u (name)
      r@ tis-skip-spaces drop
      s" 0x" r@ tis-imatch-string IF
        hex 
      ELSE
        decimal
      THEN
      r@ tis-read-number IF            \ S: c-addr u n
        r'@ tos-rewrite                \ Compile the enum
        r'@ tos-write-number
        s"  constant " r'@ tos-write-string
        r'@ tos-write-string
        r'@ str-get evaluate
      ELSE
        2drop
      THEN
    THEN
  THEN
  base !
  rdrop rdrop
;


: gsv+parse-line   ( tos tis -- = Parse the configuration file line )
  2>r
  r@ tis-reset
  r@ tis-skip-spaces drop
  r@ tis-eof? 0= IF
    s" FUNCTION_NAME" r@ tis-imatch-string IF
      2r@ gsv+parse-function
    ELSE s" ENUM_NAME" r@ tis-imatch-string IF
      2r@ gsv+parse-enum
    THEN THEN
  THEN
  rdrop rdrop
;


: gsv+included     ( c-addr u -- ior = Include the gtk-server configuration file c-addr u and create words based the contents of this file )
  r/o open-file ?dup 0= IF   \ S: fileid
    >r
    tos-new
    256 over str-size!       \ Prevent resizing, at least 256 chars
    tis-new
    1024 over str-size!      \ Insure at least 1024 chars
    BEGIN                    \ S: tos tis
      dup str-data@ 1024 r@ read-line
      tuck 0= AND
    WHILE                    \ S: tos tis u2 ior
      drop
      ?dup IF
        over str-length!
        2dup gsv+parse-line
      THEN
    REPEAT                   \ S: tos tis u2 ior
    nip
    swap tis-free
    swap tos-free
    r> close-file drop
  ELSE                       \ S: fileid ior
    nip
  THEN
;


( gtk-server connection words )

: gsv+open         ( c-addr1 u1 c-addr2 u2 -- ior = Open the connection to the gtk-server by reading the configuration file c-addr1 u1 and connecting to c-addr2 u2 )
  gsv+connect ?dup IF
    nip nip
  ELSE
    c" gtk_server_enable_c_string_escaping" >r
    r@ find nip 0= IF
      gsv+included           \ If function not found, then include file
    ELSE
      2drop 0
    THEN

    ?dup 0= IF
      r> find IF
        catch dup 0= IF      \ If successful, remove response
          nip nip
        THEN
      ELSE
        drop exp-no-data
      THEN
    ELSE
      rdrop
    THEN 
  THEN
;


: gsv+close        ( -- ior = Close the connection to the gtk-server, the gtk-server is *NOT* closed )
  gsv+disconnect
;


( gtk-server event words )

: gsv+server-connect  ( n1 c-addr1 u1 n2 -- c-addr2 u2 = Call gtk_server_connect with widgetid n2, signal c-addr1 u1 and description n2, returning the result c-addr2 u2)
  s" gtk_server_connect %d %q %d" s" %q" gsv+invoke
;


: gsv+server-connect-after  (  n1 c-addr1 u1 n2 -- c-addr2 u2 = Call gtk_server_connect_after with widgetid n2, signal c-addr1 u1 and description n2, returning the result c-addr2 u2)
  s" gtk_server_connect_after %d %q %d" s" %q" gsv+invoke
;

[THEN]

\ ==============================================================================
