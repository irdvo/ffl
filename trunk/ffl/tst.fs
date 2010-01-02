\ ==============================================================================
\
\                tst - the module test module in the ffl
\
\              Copyright (C) 2005-2006  Dick van Oudheusden
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
\  $Date: 2007-12-24 08:06:28 $ $Revision: 1.11 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] tst.version [IF]


( tst = Module testing )
( The tst module implements a module testing framework. )


3 constant tst.version


( Private database )

variable tst-errors
variable tst-tests
variable tst-timer


( Private words )

: tst-empty-data-stack  ( i*x -- = Empty the data stack )
  depth dup 0> IF            \ if stack-depth > 0 then
    0 DO
      drop                   \    remove the extras
    LOOP
  ELSE
    drop
  THEN
;

[DEFINED] fdepth [IF]
: tst-empty-float-stack  ( i*r -- = Empty the float stack )
  fdepth dup 0> IF           \ if fstack-depth > 0 then
    0 DO
      fdrop                  \  remove extras
    LOOP
  ELSE
    drop
  THEN
;  
[THEN]


: tst-report-error ( c-addr u -- = Report an error with the current source line )
  type                       \ report error
  source type cr             \ report current source line
  tst-errors 1+!
;


: tst-report-mismatch   ( -- = Report an mismatch error )
  s" stack contents mismatch: " tst-report-error
;


: tst-report-underflow  ( -- = Report a stack underflow )
  s" stack underflow: " tst-report-error
;


: tst-depth1?       ( x -- flag = Check for one value on the stack )
  depth 0= dup IF
    tst-report-underflow
  THEN
  0=
;


: tst-depth2?       ( x1 x2 -- flag = Check for two values on the stack )
  depth 2 < dup IF
    tst-report-underflow
  THEN
  0=
;


: tst-depth4?       ( x1 x2 x3 x4 -- flag = Check for four values on the stack )
  depth 4 < dup IF
    tst-report-underflow
  THEN
  0=
;


[DEFINED] fdepth [IF]
: tst-fdepth2?   ( r1 r2 -- flag = Check for two float values on the float stack )
  fdepth 2 < dup IF
    s" float stack underflow: " tst-report-error
  THEN
  0=
;
[THEN]


: tst-report-checking   ( -- = Report checking for )
  ."   expecting "
;


: tst-report-found      ( -- = Report found: )
  ." and found "
;


( Test syntax words )

: t{           ( -- = Start a test )
  tst-tests 1+!
;


: }t           ( i*x j*r -- = Check for stack overflow )
  depth 0> IF
    s" stack overflow: " tst-report-error
    tst-empty-data-stack
  THEN
    
[DEFINED] fdepth [IF]
  fdepth 0> IF
    s" float stack overflow: " tst-report-error
    tst-empty-float-stack
  THEN
[THEN]
;


( Test value words )

: ?s           ( n1 n2 -- = Check for signed value on stack )
  tst-depth2? IF
    2dup
    <> IF 
      tst-report-mismatch
      tst-report-checking . tst-report-found . cr
    ELSE
      2drop
    THEN
  THEN
;


: ?u           ( u1 u2 -- = Check for unsigned value on stack )
  tst-depth2? IF
    2dup
    <> IF 
      tst-report-mismatch
      tst-report-checking u. tst-report-found u. cr
    ELSE
      2drop
    THEN
  THEN
;


: ?d           ( d1 d2 -- = Check for a signed double on stack )
  tst-depth4? IF
    2over 2over
    d<> IF 
      tst-report-mismatch
      tst-report-checking d. tst-report-found d. cr
    ELSE
      2drop 2drop
    THEN
  THEN
;


: ?ud          ( ud1 ud2 -- = Check for an unsigned double on stack )
  tst-depth4? IF
    2over 2over
    d<> IF 
      tst-report-mismatch
      tst-report-checking ud. tst-report-found ud. cr
    ELSE
      2drop 2drop
    THEN
  THEN
;


: ?0           ( x -- = Check for zero value on stack )
  tst-depth1? IF
    dup
    0<> IF
      tst-report-mismatch
      tst-report-checking ." zero " tst-report-found . cr
    ELSE
      drop
    THEN
  THEN
;


: ?nil         ( addr -- = Check for nil value on stack )
  tst-depth1? IF
    dup
    nil<> IF
      tst-report-mismatch
      tst-report-checking ." nil " tst-report-found . cr
    ELSE
      drop
    THEN
  THEN
;


: ?true        ( flag -- = Check for true value on stack )
  tst-depth1? IF
    dup
    0= IF 
      tst-report-mismatch
      tst-report-checking ." true flag " tst-report-found . cr
    ELSE
      drop
    THEN
  THEN
;


: ?false       ( flag -- = Check for false value on stack )
  tst-depth1? IF
    dup IF
      tst-report-mismatch
      tst-report-checking ." false flag " tst-report-found . cr
    ELSE
      drop
    THEN
  THEN
;

[DEFINED] fdepth [IF]
: ?r           ( r1 r2 -- = Check for float value on stack )
  tst-fdepth2? IF
    f2dup
    f- fabs 1e-5 f< 0= IF
      s" float stack contents mismatch: " tst-report-error
      tst-report-checking f. tst-report-found f. cr
    ELSE
      fdrop fdrop
    THEN
  THEN
;
[THEN]


: ?str         ( c-addr1 u1 c-addr2 u2 -- = Check for a string on the stack )
  tst-depth4? IF
    2over 2over compare IF
      tst-report-mismatch
      tst-report-checking type space tst-report-found type cr
    ELSE
      2drop 2drop
    THEN
  THEN
;

( Test results words )

: tst-reset-tests ( -- = Reset the test results )
      tst-errors 0!
      tst-tests  0!
  ms@ tst-timer !
;


: tst-get-result  ( -- u1 u2 u3 = Get the test results, return the test time u1, the number of tests u2 and the number of errors u3 )
  ms@ tst-timer @ - tst-tests @ tst-errors @
;


[THEN]

\ ==============================================================================
