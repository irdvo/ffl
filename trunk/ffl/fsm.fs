\ ==============================================================================
\
\          fsm - the Finite State Machine module in the ffl
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
\  $Date: 2008-03-05 20:35:13 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] fsm.version [IF]

include ffl/ftr.fs
include ffl/fst.fs
include ffl/snl.fs


( fsm = Finite State Machine )
( The fsm module implements a Finite State Machine.                       )


1 constant fsm.version


( fsm structure )

begin-structure fsm%       ( -- n = Get the required space for a fsm variable )
  snl%
  +field  fsm>states          \ the list with all the states
  field:  fsm>current         \ the current state
  field:  fsm>previous        \ the previous state
  field:  fsm>events          \ the number of events in the machine
end-structure


( FSM creation, initialisation and destruction )

: fsm-init         ( +n fsm -- = Initialise the fsm with the number of events n )
  dup  fsm>states   snl-init
  tuck fsm>events   !
  dup  fsm>current  nil!
       fsm>previous nil!
;


: fsm-(free)       ( fsm -- = Free the internal, private variables from the heap )
  ['] fst-free swap fsm>states snl-(free)
;


: fsm-create       ( "<spaces>name" +n -- ; -- fsm = Create a named FSM in the dictionary with the number of events n )
  create   here   fsm% allot   fsm-init
;


: fsm-new          ( +n -- fsm = Create a new FSM on the heap with the number of events n )
  fsm% allocate  throw  tuck fsm-init
;


: fsm-free         ( fsm -- = Free the FSM from the heap )
  dup fsm-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the fsm
;


( State words )

: fsm-new-state    ( c-addr u fsm -- fst = Add a new state with label c-addr u to the FSM )
  >r fst-new dup r>
  fsm>states snl-push
;


: fsm-start        ( fst fsm -- = Set the start state )
  dup fsm>current  !
      fsm>previous nil!
;


: fsm-find-state   ( c-addr u fsm -- fst | nil = Find the state by its label in the FSM )
  ['] fst-label? swap fsm>states snl-execute? 0= IF
    2drop nil
  THEN
;


( Transition words )

: fsm-new-transition  ( c-addr u fst1 fst2 fsm -- ftr = Add a new transition from state fst1 to state fst2 with label c-addr u )
  fsm>events @ swap
  fst-new-transition
;


( Event words )

: fsm-feed         ( n fsm -- fst | nil = Feed the event to the current state, return the next state or nil if no condition hits )
  >r
  r@ fsm>current @
  dup r@ fsm>previous ! 
  dup nil= exp-invalid-state AND throw
  fst-feed 
  dup r> fsm>current !
;


: fsm-try          ( n fsm -- fst | nil = Try the event for the current event, return the result, but do not move to the state )
  fsm>current @
  dup nil= exp-invalid-state AND throw
  fst-try
;


( Conversion words )

: fsm-to-dot       ( fsm -- c-addr u = Convert the FSM to a dot string )
  \ ToDo
;

 
( Inspection )

: fsm-dump   ( fsm - = Dump the FSM )
  ." fsm:" dup . cr
  ."  states  : " ['] fst-dump over fsm>states snl-execute cr
  ."  current : " dup fsm>current ? cr
  ."  previous: " dup fsm>previous ? cr
  ."  events  : "     fsm>events ? cr
;
  
[THEN]

\ ==============================================================================
