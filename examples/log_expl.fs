\ ==============================================================================
\
\              log_expl - the logging example in the ffl
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
\  $Date: 2008-10-22 16:48:40 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/log.fs


.( Logging to the console:) cr

log-to-console

warning" Warning message"

log.error log-from-level          \ Log only errors and higher

warning" Skip warning message"

error" Error message"

log.trace log-from-level          \ Log all events


.( Logging to file "log.tmp" ) cr

s" log.tmp" w/o create-file 0= [IF]
  dup log-to-file

  trace" Trace message"

  info" Info message"

  close-file drop
[ELSE]
  drop
  .( Error: could not create "log.tmp" ) cr
[THEN]


.( Logging to rolling files: log.1 log.2 and log.3, 5 entries per file .." ) cr

s" log" 3 5 log-to-rolling

3 log-stack-depth            \ Log also the stack contents, maximum 3 values

: do-18logs
  18 0 DO
    info" Infos message via rolling files"
  LOOP
;

23 56                        \ Put some example values on the stack for the logger

do-18logs                    \ Generate 18 log messages in the rolling files

2drop


.( Logging to callback ) cr

: callback ( c-addr u -- )
  ." Logging:" type cr       \ Callback shows the message on the console
;

' callback log-to-callback

0 log-stack-depth            \ Stop logging the stack contents

error" Error message via callback"

debug" Debug message via callback"

