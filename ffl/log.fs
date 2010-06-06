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
\  $Date: 2009-05-20 13:27:22 $ $Revision: 1.5 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] log.version [IF]

include ffl/enm.fs
include ffl/tos.fs
include ffl/stt.fs

( log = Logging module )
( The log module implements a software logging and tracing module.           )
( The module uses 6 different log events, from low to high: trace, debug,    )
( info, warning, error and fatal. All log events will generate a log message.)
( Only the fatal log event will do an abort. A log message shows the         )
( date and time, the log event and the actual message. The log events can be )
( skipped during compilation and suppressed during execution. This is done   )
( by setting the log level with the log-level word. All log events that are  )
( equal or higher then this level will be compiled c.q. accepted. All events )
( can be skipped by setting log.none to the log level.                       )
( A log message can be sent to one of the four so called appenders. The      )
( default appender is the console. This appender is also used if one of the  )
( file appenders is not able to write to a file. The second appender is a    )
( normal text file. The third type appender is a rolling file appender. This )
( appender writes a number of log messages to the first file, then moves to  )
( the next file and writes again a number of log messages, and so on, until  )
( the number of files is reached. Then the appender starts again with the    )
( first file. The calling word provides the base filename for the rolling    )
( filename. The logging module appends ".1", ".2" and so on for the          )
( different filenames. The last appender is the callback appender. With this )
( appender the calling module can process the messages by its own. The stack )
( notation for the callback word is: [ c-addr u -- ]                         )


2 constant log.version


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

     defer log.appender   ( -- = the current log appender )
     defer log.flush      ( -- = the file flusher )

tos-create log.msg        ( -- tos = the log message )

0    value log.level      ( -- n    = the suppress level )
0    value log.stack      ( -- n    = the number of stack elements appended to the log message )
true value log.time&date  ( -- flag = log before the message the time and date ? )

str-create log.filename   ( -- str = the logging file name )

0    value log.entries    ( -- n   = the maximum number of entries in the rolling file )
0    value log.entry      ( -- n   = the current entry in the current file )
0    value log.files      ( -- n   = the maximum number of rolling files )
0    value log.filenr     ( -- n   = the current file number )
0    value log.file       ( -- n   = the current file )


begin-stringtable log.event>string
+" TRACE "
+" DEBUG "
+" INFO  "
+" WARN  "
+" ERROR "
+" FATAL "
end-stringtable


( Private flush words )

[UNDEFINED] noop [IF]
: noop             ( -- = Do nothing )
;
[THEN]


: log-flush        ( -- = Flush the log file )
  log.file flush-file drop
;
' log-flush is log.flush


( Private appender words )

: log-console      ( c-addr u -- = Log to the console )
  type cr
;


: log-open-rolling ( c-addr u -- fileid ior = Create the rolling log file )
  tos-new >r
  r@ tos-write-string         \ format the log filename
  [char] .   
  r@ tos-write-char           \ append the '.'
  log.filenr 
  r@ tos-write-number         \ append the file nr

  r@ str-get w/o create-file  \ create the file
  r> tos-free
;


: log-rolling      ( c-addr u -- = Log in a rolling file )
  log.entry 1+                                   \ Increase the entry number
  dup log.entries >= IF
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
    log.flush
  THEN
;


: log-file         ( c-addr u -- = Log in a file )
  log.file write-line IF
    ['] log-console is log.appender              \ Failure -> console as appender
  ELSE
    log.flush
  THEN
;


' log-console is log.appender  \ Default: log to console


( Appender words )

: log-to-rolling   ( c-addr u n1 n2 -- = Start logging to rolling files, with names starting with c-addr u, maximum n1 files and n2 entries in one file )
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


: log-to-file      ( fileid -- = Start logging to the file )
  to log.file
  ['] log-file is log.appender

;


: log-to-callback  ( xt -- = Start logging to the xt callback )
  is log.appender
;


: log-to-console   ( -- = Start logging to the console )
  ['] log-console is log.appender
;


( Private log words )

: (do-log)         ( c-addr u n -- = Log the message c-addr u with event n to the current appender )
  dup log.level < IF
    drop 2drop
  ELSE
    log.msg tos-rewrite                  \ Format the message

    log.time&date IF
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
      [char] 0 2 log.msg tos-align-right
      bl         log.msg tos-write-char
    THEN
    
    dup log.trace max log.fatal min log.event>string  \ convert event to text
               log.msg tos-write-string

    >r         log.msg tos-write-string  \ the message

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

    r> log.fatal = IF
      abort                              \ Fatal error -> abort
    THEN
  THEN
;


( Log settings words )

: log-from-level   ( n -- = Skip and suppress all events below level n )
  log.trace max log.none min to log.level
;


: log-stack-depth  ( n -- = Append max n top stack elements to the log message )
  to log.stack
;


: log-with-time&date ( flag -- = Set if the time&date should start the log message )
  to log.time&date
;


: log-with-flush   ( flag -- = Set if the log line should be flushed to file )
  IF
    ['] log-flush
  ELSE
    ['] noop
  THEN
  is log.flush
;


( Log word )

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


( Parsing log words )

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
