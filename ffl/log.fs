\ ==============================================================================
\
\                  log - the log module in the ffl
\
\               Copyright (C) 2008  Dick van Oudheusden
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
\  $Date: 2008-09-10 16:13:40 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] log.version [IF]

include ffl/enm.fs
include ffl/tos.fs
include ffl/stt.fs

( log = Logging module )
( The log module implements a software logging and tracing module.           )
( Default: console appender, problem file appender then console appender     )
( Info about the different appenders                                         )
( fatal = abort )
( layout rolling file name )

1 constant log.version


( Log events )

begin-enumeration
  enum: log.trace         ( -- n = the trace event )
  enum: log.debug         ( -- n = the debug event )
  enum: log.info          ( -- n = the info event )
  enum: log.warning       ( -- n = the warning event )
  enum: log.error         ( -- n = the error event )
  enum: log.fatal         ( -- n = the fatal event )
  enum: log.none          ( -- n = disable all events )
end-enumeration


( Private log database )

     defer log.appender   ( --    = the current logging appender )

tos-create log.msg        ( -- tos = the log message )

0    value log.level      ( -- n  = the suppress level )
0    value log.stack      ( -- n  = the number of stack elements appended to the log message )

str-create log.filename   ( -- .. = the logging file name )

0    value log.entries    ( -- n  = the maximum number of entries in the rolling file )
0    value log.entry      ( -- n  = the current entry in the current file )
0    value log.files      ( -- n  = the maximum number of rolling files )
0    value log.filenr     ( -- n  = the current file number )
0    value log.file       ( -- n  = the current file )


begin-stringtable log.event>string
," TRACE "
," DEBUG "
," INFO  "
," WARN  "
," ERROR "
," FATAL "
end-stringtable


( Private appender words )

: log-console      ( c-addr u -- = Log to the console )
  type cr
;


: log-open-rolling ( c-addr u -- fileid ior = Create the rolling log file )
  log.msg tos-rewrite         \ use the message to format the log filename
  log.msg tos-write-string
  [char] .   
  log.msg tos-write-char      \ append the '.'
  log.filenr 
  log.msg tos-write-number    \ append the file nr

  log.msg str-get w/o create-file \ create the file
;


: log-rolling      ( c-addr u -- = Log in a rolling file )
  log.entry 1+                                   \ Increase the entry number
  dup log.entries > IF
    log.filenr log.files mod 1+ to log.filenr    \ Roll and increase the file nr
    log.filename str-get log-open-rolling 0= IF  \ Try to create the next file
      log.file close-file drop                   \ If success then close the previous
      to log.file
      drop 0                                     \ Reset entry
    ELSE                                         \ Else set the console as appender
      drop
      ['] log-console is log.appender
    THEN
  THEN
  to log.entry

  log.file write-line IF                          \ Write the message to the file
    log.file close-file drop
    ['] log-console is log.appender              \ Failure -> console as appender
  ELSE
    log.file flush-file drop
  THEN
;


: log-file         ( c-addr u -- = Log in a file )
  log.file write-line IF
    ['] log-console is log.appender              \ Failure -> console as appender
  ELSE
    log.file flush-file drop
  THEN
;


' log-console is log.appender  \ Default: log to console


( Appender words )

: log-by-rolling   ( c-addr u n1 n2 -- = Start logging to a rolling files, with names starting with c-addr u, maximum n1 files and n2 entries in one file )
  1 to log.filenr
  2over log-open-rolling 0= IF    \ Try creating the log file name, starting with 1
    to log.file
    to log.entries
    to log.files
       log.filename str-set

    1 to log.filenr
    0 to log.entry

    ['] log-rolling is log.appender
  ELSE                           \ File could not be opened -> use the console appender
    drop 2drop 2drop
    ['] log-console is log.appender
  THEN
;


: log-by-file      ( fileid -- = Start logging to the file )
  to log.file
  ['] log-file is log.appender

;


: log-by-callback  ( xt -- = Start logging to the xt callback )
  is log.appender
;


: log-by-console   ( -- = Start logging to the console )
  ['] log-console is log.appender
;


( Private log words )

: (do-log)         ( c-addr u n -- = Log the message c-addr u with event n to the current appender )
  dup log.level < IF
    drop 2drop
  ELSE
    log.msg tos-rewrite                  \ Format the message
    
    time&date
               log.msg tos-write-number  \ Year
    [char] -   log.msg tos-write-char
               log.msg tos-write-number  \ Month
    [char] 0 2 log.msg tos-align-right
    [char] -   log.msg tos-write-char
               log.msg tos-write-number  \ Day
    [char] 0 2 log.msg tos-align-right
    bl         log.msg tos-write-char
               log.msg tos-write-number  \ hour
    [char] 0 2 log.msg tos-align-right
    [char] :   log.msg tos-write-char
               log.msg tos-write-number  \ minutes
    [char] 0 2 log.msg tos-align-right
    [char] :   log.msg tos-write-char
               log.msg tos-write-number  \ seconds
    bl         log.msg tos-write-char

    dup log.trace max log.fatal min log.event>string  \ convert event to text
               log.msg tos-write-string

    -rot       log.msg tos-write-string  \ the message

    log.stack IF                         \ add max log.stack stack elements to the message
      s"  : (" log.msg tos-write-string  \ stack depth
      depth    log.msg tos-write-number
      [char] ) log.msg tos-write-char

      depth 0 max log.stack min          \ stack elements
      dup 0 ?DO
        dup I - pick 
        bl     log.msg tos-write-char
               log.msg tos-write-number
      LOOP
      drop
    THEN

    log.msg str-get log.appender         \ send the message to the appender

    log.fatal = IF
      abort                              \ Fatal error -> abort
    THEN
  THEN
;


( Log words )

: log-level        ( n -- = Skip and suppress all events below level n )
  log.trace max log.none min to log.level
;


: log-stack        ( n -- = Append max n top stack elements to the log message )
  to log.stack
;


: do-log           ( c-addr u n -- = Log the message c-addr u with event n )
  dup log.level < IF
    drop 2drop
  ELSE
    state @ IF
      -rot
      postpone     sliteral
      postpone     literal
      ['] (do-log) compile,
    ELSE
      (do-log)               \ Interpreting: feed the event and message to the appender
    THEN
  THEN
;


( Private log message words )

: (log")          ( "ccc<quote>" n -- = Log the message with event n )
  [char] " parse 
  rot do-log
;


( Log message words )

: fatal"           ( "ccc<quote>" -- = Log a fatal message )
  log.fatal (log")
; immediate


: error"           ( "ccc<quote>" -- = Log an error message )
  log.error (log")
; immediate


: warning"         ( "ccc<quote>" -- = Log a warning message )
  log.warning (log")
; immediate


: info"            ( "ccc<quote>" -- = Log an info message )
  log.info (log")
; immediate


: debug"           ( "ccc<quote>" -- = Log a debug message )
  log.debug (log")
; immediate


: trace"           ( "ccc<quote>" -- = Log a trace message )
  log.trace (log")
; immediate
 
[THEN]

\ ==============================================================================
