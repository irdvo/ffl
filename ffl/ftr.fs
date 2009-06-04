\ ==============================================================================
\
\               ftr - the FSM transition module in the ffl
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
\  $Date: 2008-03-25 06:56:00 $ $Revision: 1.4 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] ftr.version [IF]

include ffl/bar.fs
include ffl/snn.fs
include ffl/str.fs


( ftr = FSM Transition )
( The ftr module implements a transition in a Finite State Machine. See [fsm] )
( for more info about using this module.                                      )


1 constant ftr.version


( ftr structure )

begin-structure ftr%       ( -- n = Get the required space for a transition variable )
  snn%
  +field  ftr>node            \ the transition extends the single list node
  bar%
  +field  ftr>condition       \ the condition (bit array)
  str%
  +field  ftr>label           \ the label
  field:  ftr>next            \ the next state
  field:  ftr>data            \ the data
  field:  ftr>action          \ the action
  str%
  +field  ftr>attributes      \ the graphviz attributes
end-structure


( Transition creation, initialisation and destruction )

: ftr-init         ( x xt c-addr u fst +n ftr -- = Initialise the transition to the state fst, with label c-addr u, number events n, action xt and data x )
  >r
  r@ ftr>node           snn-init
  r@ ftr>condition      bar-init
  r@ ftr>next           !
  r@ ftr>attributes     str-init
  r@ ftr>label      dup str-init str-set
  r@ ftr>action         !
  r> ftr>data           !
;


: ftr-(free)       ( ftr -- = Free the internal, private variables from the heap )
  dup ftr>condition  bar-(free)
  dup ftr>label      str-(free)
      ftr>attributes str-(free)
;


: ftr-new          ( x xt c-addr u fst +n -- ftr = Create a new transition on the heap to the state fst, with label c-addr u, number events n, action xt and data x )
  ftr% allocate  throw  >r  r@ ftr-init  r>
;


: ftr-free         ( ftr -- = Free the transition from the heap )
  dup ftr-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the ftr
;


( Member words )

: ftr-condition@   ( ftr -- bar = Get the condition of the transition as reference to a bit array )
  ftr>condition
;


: ftr-label@       ( ftr -- c-addr u = Get the label of the transition )
  ftr>label str-get
;


: ftr-label?       ( c-addr u ftr -- ftr true | c-addr u false = Check the label c-addr u with the transition ftr )
  >r
  2dup r@ ftr>label str-ccompare 0= IF
    2drop r@ true
  ELSE
    false
  THEN
  rdrop
;


: ftr-data@        ( ftr -- x = Get the data of the transition )
  ftr>data @
;


: ftr-data!        ( x ftr -- = Set the data for the transition )
  ftr>data !
;


: ftr-action@      ( ftr -- xt = Get the action of the transition )
  ftr>action @
;


: ftr-attributes!  ( c-addr u ftr -- = Set the extra graphviz attributes for the transition )
  ftr>attributes str-set
;


: ftr-attributes@  ( ftr -- c-addr u = Get the extra graphviz attributes of the transition )
  ftr>attributes str-get
;


( Event words )

: ftr-fire         ( n ftr -- fst = Fire the transition for event n, without checking the condition )
  tuck
  dup ftr-action@ nil<>? IF          \   If action set Then
    execute                          \     Execute action with event and transition
  ELSE
    2drop
  THEN
  ftr>next @                         \   Next state
;


: ftr-feed         ( n ftr -- n false | fst true = Feed the event to this transition, return the next state or the event if the event did not match the condition )
  2dup ftr>condition bar-get-bit IF    \ If event in bit array Then
    ftr-fire
    true
  ELSE
    drop 
    false                              \ Else no hit
  THEN
;


: ftr-try          ( n ftr -- n false | fst true = Try the event for this transition, return the result )
  2dup ftr>condition bar-get-bit IF
    nip
    ftr>next @
    true
  ELSE
    drop 
    false
  THEN
;


( Inspection )

: ftr-dump   ( ftr -- = Dump the transition )
  ." ftr:" dup . cr
  ."  condition : " dup ftr>condition bar-dump cr
  ."  label     : " dup ftr-label@ type cr
  ."  next      : " dup ftr>next ? cr
  ."  data      : " dup ftr>data  ? cr
  ."  action    : " dup ftr>action ? cr
  ."  attributes: "     ftr-attributes@ type cr 
;

[THEN]

\ ==============================================================================
