\ ==============================================================================
\
\                 fst - the FSM state module in the ffl
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


[UNDEFINED] fst.version [IF]

include ffl/ftr.fs
include ffl/snl.fs
include ffl/str.fs


( fst = FSM State )
( The fst module implements a state in a Finite State Machine.            )


1 constant fst.version


( State structure )

begin-structure fst%       ( -- n = Get the required space for a state variable )
  snn%
  +field  fst>node            \ the state extends the list node
  snl%
  +field  fst>transitions     \ the list with all the transitions
  str%
  +field  fst>label           \ the current state
  field:  fst>data            \ the data
  field:  fst>entry           \ the entry action
  field:  fst>exit            \ the exit action
  str%
  +field  fst>attributes      \ the graphviz attributes
end-structure


( State creation, initialisation and destruction )

: fst-init         ( c-addr u fst -- = Initialise the state with label c-addr u )
  >r
  r@ fst>node         snn-init
  r@ fst>transitions  snl-init
  r@ fst>label dup    str-init str-set
  r@ fst>data         0!
  r@ fst>entry        nil!
  r@ fst>exit         nil!
  r> fst>attributes   str-init
;


: fst-(free)       ( fst -- = Free the internal, private variables from the heap )
  ['] ftr-free over fst>transitions snl-(free)
  dup fst>label      str-(free)
      fst>attributes str-(free)
;


: fst-new          ( -- fst = Create a new fst on the heap )
  fst% allocate  throw  >r r@ fst-init  r>
;


: fst-free         ( fst -- = Free the fst from the heap )
  dup fst-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the fst
;


( Member words )

: fst-label@       ( fst -- c-addr u = Get the label of the state )
  fst>label str-get
;


: fst-label!       ( c-addr u fst -- = Set the label of the state )
  fst>label str-set
;


: fst-label?       ( c-addr u fst -- c-addr u false | fst true = Compare the label )
  >r
  2dup r@ str-ccompare 0= IF
    2drop r@ true
  ELSE
    false
  THEN
  rdrop
;


: fst-data@        ( fst -- x = Get the data of the state )
  fst>data @
;


: fst-data!        ( x fst -- = Set the data of the state )
  fst>data !
;


: fst-entry@       ( fst -- xt = Get the entry action of the state )
  fst>entry @
;


: fst-entry!       ( xt fst -- = Set the entry action of the state )
  fst>entry !
;


: fst-exit@        ( fst -- xt = Get the entry action of the state )
  fst>exit @
;


: fst-exit!        ( xt fst -- = Set the entry action of the state )
  fst>exit !
;


: fst-attributes@  ( fst -- c-addr u = Get the attributes of the state )
  fst>attributes str-get
;


: fst-attributes!  ( c-addr u fst -- = Set the attributes of the state )
  fst>attributes str-set
;


( Transition words )

: fst-new-transition  ( c-addr u fst1 n fst2 -- ftr = Add a new transition to state fst1 with label c-addr u and number events n to this state )
  >r fst-new dup r>
  fst>transitions snl-push
;


: fst-find-transition  ( c-addr u fst -- ftr | nil = Find the transition with label c-addr u, else return nil )
  ['] ftr-label? swap fst>transitions snl-execute? 0= IF
    2drop nil
  THEN
;


( Event words )

: fst-feed         ( n fst -- fst | nil = Feed the event to this state, return the next state or nil if no condition hits )
  >r
  r@ fst-exit@ nil<>? IF          \ Execute the exit action for the current state
    r@ swap execute
  THEN
  
  ['] ftr-feed r> fst>transitions snl-execute? IF   \ Call fst-feed for every transition until a hit
    >r
    r@ fst-entry@ nil<>? IF       \ Execute the entry action for the next state
      r@ swap execute
    THEN
    r>
  ELSE
    drop                          \ ToDo: any transition
    nil
  THEN
;


: fst-try          ( n fst -- fst | nil = Try the event for this state, return the result )
  ['] ftr-try swap fst>transitions snl-execute? 0= IF  \ Call fst-try for every transition until a hit
    drop                          \ ToDo: any transition
    nil
  THEN
;


( Inspection )

: fst-dump   ( fst - = Dump the state )
  ." fst:" dup . cr
  ."  transitions: " ['] ftr-dump over fst>transitions snl-execute cr
  ."  label      : " dup fst>label str-get type cr
  ."  data       : " dup fst>data ? cr
  ."  entry      : " dup fst>entry ? cr
  ."  exit       : " dup fst>exit ? cr
  ."  attributes : "     fst>attributes str-get type cr 
;

[THEN]

\ ==============================================================================
