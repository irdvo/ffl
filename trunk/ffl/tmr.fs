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
\  $Date: 2007-12-09 07:23:17 $ $Revision: 1.5 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] tmr.version [IF]


[DEFINED] ms@ [DEFINED] max-ms@ AND [IF]


include ffl/stc.fs


( tmr = Timer module )
( The tmr module implements a poll able interval timer. Due to the fact that  )
( the ANS standard does not define a way to fetch milliseconds, this module   )
( has a environmental dependency.                                             )


1 constant tmr.version


( Timer structure )

begin-structure tmr%       ( - n = Get the required space for the timer variable )
  field:  tmr>now            \ start of timer
  field:  tmr>timeout        \ timeout time
  field:  tmr>rem            \ remaining ms after expired? or wait
end-structure


( Timer variable creation, initialisation and destruction )

: tmr-init         ( u tmr -- = Initialise the timer with timeout u )
  >r
      r@ tmr>timeout  !
  ms@ r@ tmr>now      !
      r> tmr>rem     0!
;


: tmr-create       ( u "<spaces>name" -- ; -- tmr = Create a named timer variable in the dictionary with timeout u )
  create  here tmr% allot  tmr-init
;


: tmr-new          ( u -- tmr = Create a new timer variable on the heap with timeout u )
  tmr% allocate  throw   >r r@ tmr-init r>
;


: tmr-free         ( tmr -- = Free the timer from the heap )
  free throw
;


( Member words )

: tmr-timeout@     ( tmr -- u = Get the timeout value from the timer )
  tmr>timeout @
;


: tmr-timer@       ( tmr -- u = Get the running time u from the timer in ms, after last [re]start, expired? or wait )
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


: tmr-start        ( u tmr -- = Start the timer with a timeout value u )
  tmr-init
;


: tmr-restart      ( tmr -- = Restart the timer with the current timeout value )
  ms@ over tmr>now  !
           tmr>rem 0!
;


: tmr-expired?     ( tmr -- flag = Check if the timer is expired, if so the timer is restarted )
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


: tmr-wait         ( tmr -- = Wait till the timer expires and restart the timer )
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

: tmr-dump         ( tmr -- = Dump the tmr state )
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
