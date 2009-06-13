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
\  $Date: 2008-03-25 06:56:00 $ $Revision: 1.6 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] fsm.version [IF]

include ffl/ftr.fs
include ffl/fst.fs
include ffl/snl.fs
include ffl/tos.fs


( fsm = Finite State Machine )
( The fsm module implements a Finite State Machine. Use fsm-new-state to add )
( states to the machine. Then use fsm-new-transition to add transitions      )
( between the states. fsm-new-transition returns the new transition. Use     )
( ftr-condition@ on this new transition to get a reference to the condition  )
( in the transition. This is actually a bit array, see [bar]. Use the words  )
( of the bar module to set the condition. When the whole FSM is built, start )
( using the machine by calling fsm-start. By default the first created       )
( state is the start state, but this can be changed by fsm-start! After      )
( starting the machine, feed events to the machine by fsm-feed. This word    )
( returns the new, current state or nil if no transition matched. The        )
( machine can be converted to graphviz's dot files by fsm-to-dot. This word  )
( uses the labels of the states and transitions to build the dot             )
( representation. It also set the shape of the states &lb;double circle for  )
( start and end states, circles for the others&rb;. Use fst-attributes! and  )
( ftr-attributes! to set additional graphviz attributes.                     )
( During the feeding of events, the optional actions are called. When a      )
( state is left, the exit action is called, when a state is entered the      )
( entry state is called. If a transition matched, the action of this         )
( transition is also called. The stack usage for those actions:              )
( {{{ )
( state entry action:  fst -- = State fst is entered                         )
( state exit action:   fst -- = State fst is left                            )
( transition action: n ftr -- = Transition fst matched for event n           )
( }}} )


1 constant fsm.version


( fsm structure )

begin-structure fsm%       ( -- n = Get the required space for a fsm variable )
  snl%
  +field  fsm>states          \ the list with all the states
  field:  fsm>ids             \ the state id counter
  field:  fsm>start           \ the start state
  field:  fsm>current         \ the current state
  field:  fsm>events          \ the number of events in the machine
end-structure


( FSM creation, initialisation and destruction )

: fsm-init         ( +n fsm -- = Initialise the fsm with the number of events n )
  dup  fsm>states   snl-init
  dup  fsm>ids      0!
  tuck fsm>events   !
  dup  fsm>start    nil!
       fsm>current  nil!
;


: fsm-(free)       ( fsm -- = Free the internal, private variables from the heap )
  ['] fst-free swap fsm>states snl-(free)
;


: fsm-create       ( "<spaces>name" +n -- ; -- fsm = Create a named fsm in the dictionary with the number of events n )
  create   here   fsm% allot   fsm-init
;


: fsm-new          ( +n -- fsm = Create a new fsm on the heap with the number of events n )
  fsm% allocate  throw  tuck fsm-init
;


: fsm-free         ( fsm -- = Free the fsm from the heap )
  dup fsm-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the fsm
;


( Member words )

: fsm-start@       ( fsm -- fst = Get the start state )
  fsm>start @
;


: fsm-start!       ( fst fsm -- = Set the start state )
  fsm>start !
;


( State words )

: fsm-new-state    ( x xt1 xt2 c-addr1 u1 fsm -- fst = Add a new state with label c-addr1 u1, entry action xt1, exit action xt2 and data x )
  >r 
  r@ fsm>ids dup 1+! @ fst-new    \ Increase ids and create the state 
  dup r@ fsm>states snl-append
  r@ fsm-start@ nil= IF
    dup r@ fsm>start !
  THEN  
  rdrop
;


: fsm-start        ( fsm -- = Start the finite state machine )
  dup  fsm-start@
  swap fsm>current  !
;


: fsm-find-state   ( c-addr u fsm -- fst | nil = Find the state by its label c-addr u in the fsm )
  ['] fst-label? swap fsm>states snl-execute? 0= IF
    2drop nil
  THEN
;


( Transition words )

: fsm-new-transition  ( x xt c-addr1 u1 fst1 fst2 fsm -- ftr = Add a new transition from state fst1 to state fst2 with label c-addr1 u1, action xt and data x )
  fsm>events @ rot fst-new-transition
;


: fsm-any-transition  ( x xt c-addr1 u1 fst1 fst2 fsm -- ftr = Set the any transition for state fst1 to state fst2 with label c-addr1 u1, action xt and data x )
  drop swap fst-any-transition
;


( Event words )

: fsm-feed         ( n fsm -- fst | nil = Feed the event to the current state, return the next state or nil if the event did not match any condition )
  >r
  r@ fsm>current @
  dup nil= exp-invalid-state AND throw
  fst-feed 
  dup r> fsm>current !
;


: fsm-try          ( n fsm -- fst | nil = Try the event for the current event, return the next state, but do not move to this state )
  fsm>current @
  dup nil= exp-invalid-state AND throw
  fst-try
;


( Conversion words )

: fsm-to-dot       ( c-addr u tos fsm -- = Convert the fsm to a dot string using the stream, giving the graph the name c-addr u )
  swap >r -rot
  s" digraph "    r@ tos-write-string
                  r@ tos-write-string     \ Write graph name
  s"  {"          r@ tos-write-string 
                  r@ tos-flush
  s" rankdir=LR;" r@ tos-write-string 
                  r@ tos-flush

                                          \ Write all nodes with their attributes to the tos
  dup fsm-start@ swap fsm>states r@ swap ['] fst-to-dot swap snl-execute 2drop
  
  [char] }        r@ tos-write-char   
                  r> tos-flush
;

 
( Inspection )

: fsm-dump   ( fsm -- = Dump the fsm variable )
  ." fsm:" dup . cr
  ."  states  : " ['] fst-dump over fsm>states snl-execute cr
  ."  start   : " dup fsm>start ? cr
  ."  current : " dup fsm>current ? cr
  ."  events  : "     fsm>events ? cr
;
  
[THEN]

\ ==============================================================================
