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
\  $Date: 2008-03-05 20:35:13 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] ftr.version [IF]

include ffl/bar.fs
include ffl/snn.fs
include ffl/str.fs


( ftr = FSM Transition )
( The ftr module implements a transition in a Finite State Machine.       )


1 constant ftr.version


( ftr structure )

begin-structure ftr%       ( -- n = Get the required space for a transition variable )
  snn%
  +field  ftr>node            \ the transition extends the single list node
  bar%
  +field  ftr>condition       \ the condition
  str%
  +field  ftr>label           \ the label
  field:  ftr>next            \ the next state
  field:  ftr>data            \ the data
  field:  ftr>action          \ the action
  str%
  +field  ftr>attributes      \ the graphviz attributes
end-structure


( Transition creation, initialisation and destruction )

: ftr-init         ( c-addr u fst +n ftr -- = Initialise the transition with next state fst, label c-addr u and number events n )
  >r
  r@ ftr>node         snn-init
  r@ ftr>condition    bar-init
  r@ ftr>next         !
  r@ ftr>label dup    str-init str-set
  r@ ftr>data         0!
  r@ ftr>action       nil!
  r> ftr>attributes   str-init
;


: ftr-(free)       ( ftr -- = Free the internal, private variables from the heap )
  dup ftr>condition  bar-(free)
  dup ftr>label      str-(free)
      ftr>attributes str-(free)
;


: ftr-new          ( c-addr u fst +n -- ftr = Create a new transition on the heap with next state fst, label c-addr u and number events n )
  ftr% allocate  throw  >r  r@ ftr-init  r>
;


: ftr-free         ( ftr -- = Free the ftr from the heap )
  dup ftr-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the ftr
;


( Member words )

: ftr-label@       ( ftr -- c-addr u = Get the label of the transition )
  ftr>label str-get
;


: ftr-label!       ( c-addr u ftr -- = Set the label of the transition )
  ftr>label str-set
;


: ftr-label?       ( c-addr u ftr -- ftr true | c-addr u false = Compare the label )
  >r
  2dup r@ str-ccompare 0= IF
    2drop r@ true
  ELSE
    false
  THEN
  rdrop
;


: ftr-data@        ( ftr -- x = Get the data of the transition )
  ftr>data @
;


: ftr-data!        ( x ftr -- = Set the data of the transition )
  ftr>data !
;


: ftr-action@      ( ftr -- xt = Get the entry action of the transition )
  ftr>action @
;


: ftr-action!      ( xt ftr -- = Set the entry action of the transition )
  ftr>action !
;


: ftr-attributes@  ( ftr -- c-addr u = Get the attributes of the transition )
  ftr>attributes str-get
;


: ftr-attributes!  ( c-addr u ftr -- = Set the attributes of the transition )
  ftr>attributes str-set
;


( Event words )

: ftr-feed         ( n ftr -- n false | fst true = Feed the event to this transition, return the next state or the event if the condition does not hit )
  2dup ftr>condition bar-get-bit IF    \ If event in bit array Then
    dup ftr-action@ nil<>? IF          \   If action set Then
      over swap execute                \     Execute action with transition
    THEN
    nip
    ftr>next @                         \   Next state
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

: ftr-dump   ( ftr - = Dump the transition )
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
