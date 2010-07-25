\ ==============================================================================
\
\        log_test - the test words for the log module in the ffl
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
include ffl/tst.fs


.( Testing: log) cr

variable log-count  log-count 0!

: log-callback ( c-addr u -- )
  2drop
  log-count 1+!
;

\ Test log-to-callback

t{ ' log-callback log-to-callback }t

t{ trace" Message 1" }t

t{ debug" Message 2" }t

t{ info" Message 3" }t

t{ warning" Message 4" }t

t{ s" Message 5" log.error do-log }t

\ t{ fatal" Message 6" }t

log.warning log-from-level

t{ trace" Message 7" }t

t{ debug" Message 8" }t

t{ info" Message 9" }t

t{ warning" Message 10" }t

t{ error" Message 11" }t

\ t{ fatal" Message 12" }t

log.none log-from-level

t{ trace" Message 13" }t

t{ debug" Message 14" }t

t{ info" Message 15" }t

t{ warning" Message 15" }t

t{ error" Message 16" }t

\ t{ fatal" Message 17" }t

t{ log-count @ 7 ?s }t

log.trace log-from-level


\ Test log-by-file

s" log.tmp" w/o create-file 0= [IF]
  
  t{ log-to-file }t

  t{ trace" Message 18" }t

  t{ debug" Message 19" }t

  t{ info" Message 20" }t

  t{ warning" Message 21" }t

  t{ error" Message 22" }t

  log.file close-file drop
[ELSE]
  drop
[THEN]

\ Test log-by-rolling

t{ s" log" 10 15 log-to-rolling }t

: log-test1
  202 0 DO
    I
    trace" Message"
    drop
  LOOP
;

t{ false log-with-flush }t

t{ 3 log-stack-depth }t

t{ log-test1 }t

t{ log.filenr 4 ?s }t
t{ log.entry  7 ?s }t


\ ==============================================================================
