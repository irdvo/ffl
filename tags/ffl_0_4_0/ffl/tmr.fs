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
\  $Date: 2006-12-30 06:19:02 $ $Revision: 1.4 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] tmr.version [IF]


[DEFINED] ms@ [DEFINED] max-ms@ AND [IF]


include ffl/stc.fs


( tmr = Timer module )
( The tmr module implements words for using a pollable interval timer. Due to )
( the fact that the ANS standard does not define a way to fetch milliseconds, )
( this module has a environmental dependency.                                 )


1 constant tmr.version


( Private constants )


( Timer structure )

struct: tmr%       ( - n = Get the required space for the timer data structure )
  cell:  tmr>now             \ start of timer
  cell:  tmr>timeout         \ timeout time
  cell:  tmr>rem             \ remaining ms after expired? or wait
;struct


( Timer structure creation, initialisation and destruction )

: tmr-init         ( u:timeout w:tmr - = Initialise the timer structure )
  >r
      r@ tmr>timeout  !
  ms@ r@ tmr>now      !
      r> tmr>rem     0!
;


: tmr-create       ( C: "name" u:timeout -  R: - w:tmr = Create a named timer structure in the dictionary )
  create  here tmr% allot  tmr-init
;


: tmr-new          ( u:timeout - w:tmr = Create a new timer structure on the heap )
  tmr% allocate  throw   >r r@ tmr-init r>
;


: tmr-free         ( w:tmr - = Free the timer from the heap )
  free throw
;


( Member words )

: tmr-timeout@     ( w:tmr - u:timeout = Get the time out value )
  tmr>timeout @
;


: tmr-timer@       ( w:tmr - u:timer = Get the running time of the timer in ms, after last [re]start, expired? or wait )
  >r
  ms@ r@ tmr>now @
  2dup u< IF
    max-ms@ swap - +
  ELSE
    -
  THEN
  r> tmr>rem @ +
;


( Timer words )


: tmr-start        ( u:timeout w:tmr - = Start the timer with a timeout value )
  tmr-init
;


: tmr-restart      ( w:tmr - = Restart the timer with the current timeout value )
  ms@ over tmr>now  !
           tmr>rem 0!
;


: tmr-expired?     ( w:tmr - f = Check if the timer is expired, if so the timer is restarted )
  >r
  r@ tmr-timer@ r@ tmr-timeout@
  2dup u< IF
    2drop
    false
  ELSE
    -   r@ tmr>rem !
    ms@ r@ tmr>now !
    true
  THEN
  rdrop
;


: tmr-wait         ( w:tmr - = Wait till the timer expires and restart the timer )
  >r
  r@ tmr-timer@ r@ tmr-timeout@
  2dup u< IF
    swap - ms
    r@ tmr>rem 0!
  ELSE
    - r@ tmr>rem !
  THEN
  ms@ r> tmr>now !
;


( Inspection )

: tmr-dump         ( w:tmr - = Dump the tmr state )
  ." tmr:" dup . cr
  ."  now      :" dup tmr>now @ u. cr
  ."  timeout  :" dup tmr>timeout @ u. cr
  ."  remaining:"     tmr>rem @ u. cr
;

[ELSE]
.( Warning: tmr requires ms@ and max-ms@ ) cr
[THEN]

[THEN]

\ ==============================================================================
