\ ==============================================================================
\
\                  tmr - the timer module in the ffl
\
\               Copyright (C) 2006  Dick van Oudheusden
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
\  $Date: 2006-12-08 20:59:48 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] tmr.version [IF]


[DEFINED] sys.timer@ [IF]


include ffl/stc.fs


( tmr = Timer module )
( The tmr module implements words for using a pollable timer. Due to the fact )
( that the ANS standard does not define a way to fetch microseconds, this     )
( module has a environmental dependency.                                      )


1 constant tmr.version


( Private constants )


( Timer structure )

struct: tmr%       ( - n = Get the required space for the timer data structure )
  double:  tmr>now             \ start us of timer
  double:  tmr>timeout         \ timeout time
  double:  tmr>rem             \ remaining us after expired? or wait
;struct


( Timer structure creation, initialisation and destruction )

: tmr-init         ( ud:timeout w:tmr - = Initialise the timer structure )
  >r
             r@ tmr>timeout 2!
  sys.timer@ r@ tmr>now     2!
  0.         r> tmr>rem     2!
;


: tmr-create       ( C: "name" ud:timeout -  R: - w:tmr = Create a named timer structure in the dictionary )
  create  here tmr% allot  tmr-init
;


: tmr-new          ( ud:timeout - w:tmr = Create a new timer structure on the heap )
  tmr% allocate  throw   >r r@ dup tmr-init r>
;


: tmr-free         ( w:tmr - = Free the timer from the heap )
  free throw
;


( Private words )


( Member words )

: tmr-timeout@     ( w:tmr - ud:timeout = Get the time out value )
  tmr>timeout @
;


: tmr-timer@       ( w:tmr - ud:timer = Get the running time of the timer in us, after last [re]start, expired? or wait )
  >r
  sys.timer@ r@ tmr>now 2@
  2over 2over du< IF
    sys.timer-max 2swap d- d+
  ELSE
    d-
  THEN
  r> tmr>rem 2@ d+
;


( Timer words )


: tmr-start        ( ud:timeout w:tmr - = Start the timer with a timeout value )
  tmr-init
;


: tmr-restart      ( w:tmr - = Restart the timer with the current timeout value )
  >r
  sys.timer@ r@ tmr>now 2!
  0.         r> tmr>rem 2!
;


: tmr-expired?     ( w:tmr - f = Check if the timer is expired, if so the timer is restarted )
  >r
  r@ tmr-timer@ r@ tmr>timeout 2@
  2over 2over du< IF
    2drop 2drop 
    false
  ELSE
    d-         r@ tmr>rem 2!
    sys.timer@ r@ tmr>now 2!
    true
  THEN
  rdrop
;


: tmr-wait         ( w:tmr - = Wait, blocking, till the timer expires )
  >r
  r@ tmr-timer@ r@ tmr>timeout 2@
  2over 2over du< IF
    2swap d- 1000 um/mod nip ms
    0. r@ tmr>rem 2!                \ ToDo: not correct
  ELSE
    d- r@ tmr>rem 2!
  THEN
  sys.timer@ r> tmr>now 2!
;


( Inspection )

: tmr-dump         ( w:tmr - = Dump the tmr state )
  ." tmr:" dup . cr
  ."  now      :" dup tmr>now 2@ ud. cr
  ."  timeout  :" dup tmr>timeout 2@ ud. cr
  ."  remaining:"     tmr>rem 2@ ud. cr
;

[ELSE]
.( Warning: tmr requires sys.timer@ ) cr
[THEN]

[THEN]

\ ==============================================================================
