\ ==============================================================================
\
\                tst - the unit test module in the ffl
\
\               Copyright (C) 2005  Dick van Oudheusden
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
\  $Date: 2005-12-19 19:51:27 $ $Revision: 1.2 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] tst.version [IF]


( tst = Unit testing )
( The tst module implements an unit testing framework.)


1 constant tst.version


( Private database )

variable tst-errors
variable tst-tests


( Private words )

: tst-empty-stack  ( ... -  = Empty the stack )
  depth dup 0> IF            \ if stack-depth > 0 then
    0 DO
      drop                   \    remove the extra's
    LOOP
  ELSE
    drop
  THEN
  
  fdepth dup 0> IF           \ if fstack-depth > 0 then
    0 DO
      fdrop                  \  remove extra's
    LOOP
  ELSE
    drop
  THEN
;  


: tst-report-error ( w:caddr u - = Report an error with the current source line )
  type                       \ report error
  source type cr             \ report current source line
  tst-empty-stack
  tst-errors 1+!
;


: tst-depth?       ( .. - f = Check for a value on the stack )
  depth 0= dup IF
    s" stack underflow: " tst-report-error
  THEN
  0=
;


: tst-fdepth?   ( .. - f = Check for a value on the float stack )
  fdepth 0= dup IF
    s" float stack underflow: " tst-report-error
  THEN
  0=
;


: tst-report-mismatch   ( f - = Report an mismatch error )
  IF s" stack contents mismatch: " tst-report-error THEN
;
  

( Public words )

: t{           ( - = Start a test )
  tst-tests 1+!
;


: }t           ( .. - = Check for stack overflow )
  depth 0> IF
    s" stack overflow: " tst-report-error
  THEN
  fdepth 0> IF
    s" float stack overflow: " tst-report-error
  THEN
;


: ?s           ( s s - = Check for signed value on stack )
  tst-depth? IF
    <> tst-report-mismatch
  THEN
;


: ?u           ( u u - = Check for unsigned value on stack )
  tst-depth? IF
    u<> tst-report-mismatch
  THEN
;


: ?0           ( n - = Check for zero value on stack )
  tst-depth? IF
    0<> tst-report-mismatch
  THEN
;


: ?nil         ( w - = Check for nil value on stack )
  tst-depth? IF
    nil<> tst-report-mismatch
  THEN
;


: ?true        ( f - = Check for true value on stack )
  tst-depth? IF
    0= tst-report-mismatch
  THEN
;


: ?false       ( f - = Check for false value on stack )
  tst-depth? IF
    tst-report-mismatch
  THEN
;


: ?r           ( r r - = Check for float value on stack )
  tst-fdepth? IF
    f<> IF
      s" float stack contents mismatch: " tst-report-error
    THEN
  THEN
;


: tst-reset-tests ( - = Reset the test results )
  tst-errors 0!
  tst-tests  0!
;


: tst-get-result  ( - u:tests u:errors = Get the test results )
  tst-tests @ tst-errors @
;


[THEN]

\ ==============================================================================
