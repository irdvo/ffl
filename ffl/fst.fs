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
\  $Date: 2008-03-25 06:56:00 $ $Revision: 1.6 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] fst.version [IF]

include ffl/ftr.fs
include ffl/snl.fs
include ffl/str.fs
include ffl/tos.fs


( fst = FSM State )
( The fst module implements a state in a Finite State Machine. See [fsm] for )
( more info about using this module.                                         )


1 constant fst.version


( State structure )

begin-structure fst%       ( -- n = Get the required space for a state variable )
  snn%
  +field  fst>node            \ the state extends the list node
  snl%
  +field  fst>transitions     \ the list with all the transitions
  field:  fst>id              \ the state id
  str%
  +field  fst>label           \ the current state
  field:  fst>data            \ the data
  field:  fst>entry           \ the entry action
  field:  fst>exit            \ the exit action
  str%
  +field  fst>attributes      \ the graphviz attributes
  field:  fst>any             \ the any transition
end-structure


( State creation, initialisation and destruction )

: fst-init         ( x xt1 xt2 c-addr u n fst -- = Initialise the state with id n and label c-addr u, entry action xt1, exit action xt2 and data x )
  >r
  r@ fst>node           snn-init
  r@ fst>transitions    snl-init
  r@ fst>any            nil!
  r@ fst>id             !
  r@ fst>attributes     str-init
  r@ fst>label      dup str-init str-set
  r@ fst>exit           !
  r@ fst>entry          !
  r> fst>data           !
;


: fst-(free)       ( fst -- = Free the internal, private variables from the heap )
  ['] ftr-free over fst>transitions snl-(free)
  
  dup fst>any @ nil<>? IF
    ftr-free
  THEN
  
  dup fst>label      str-(free)
      fst>attributes str-(free)
;


: fst-new          ( x xt1 xt2 c-addr u n -- fst = Create a new state on the heap with id n, label c-addr u, entry action xt1, exit action xt2 and data x )
  fst% allocate  throw  >r r@ fst-init  r>
;


: fst-free         ( fst -- = Free the state from the heap )
  dup fst-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the fst
;


( Member words )

: fst-id@          ( fst -- n = Get the id of the state )
  fst>id @
;


: fst-label@       ( fst -- c-addr u = Get the label of the state )
  fst>label str-get
;


: fst-label?       ( c-addr u fst -- c-addr u false | fst true = Check the label c-addr u with this state )
  >r
  2dup r@ fst>label str-ccompare 0= IF
    2drop r@ true
  ELSE
    false
  THEN
  rdrop
;


: fst-data@        ( fst -- x = Get the data of the state )
  fst>data @
;


: fst-data!        ( x fst -- = Set the data for the state )
  fst>data !
;


: fst-entry@       ( fst -- xt = Get the entry action of the state )
  fst>entry @
;


: fst-exit@        ( fst -- xt = Get the exit action of the state )
  fst>exit @
;


: fst-attributes!  ( c-addr u fst -- = Set the extra graphviz attributes for the state )
  fst>attributes str-set
;


: fst-attributes@  ( fst -- c-addr u = Get the extra graphviz attributes of the state )
  fst>attributes str-get
;


( Transition words )

: fst-new-transition  ( x xt c-addr u fst1 n fst -- ftr = Add a new transition to state fst1 with label c-addr u, number events n, action xt and data x )
  >r ftr-new dup r>
  fst>transitions snl-append
;


: fst-any-transition  ( x xt c-addr u fst1 fst -- ftr = Set the any transition to state fst1 with label c-addr u, action xt and data x )
  dup fst>any @ nil<> exp-invalid-state AND throw     \ Only one any transition allowed
  
  >r 1 ftr-new dup r> fst>any !
;

  
: fst-find-transition  ( c-addr u fst -- ftr | nil = Find the transition with label c-addr u, else return nil )
  >r
  ['] ftr-label? r@ fst>transitions snl-execute? 0= IF
    r@ fst>any @ nil<>? IF
      ftr-label? 0= IF
        2drop nil
      THEN
    ELSE
      2drop nil
    THEN
  THEN
  rdrop
;


( Event words )

: fst-feed         ( n fst -- fst | nil = Feed the event to this state, return the next state or nil if the event did not match any condition )
  >r
  r@ fst-exit@ nil<>? IF          \ Execute the exit action for the current state
    r@ swap execute
  THEN
  
  ['] ftr-feed r@ fst>transitions snl-execute? IF   \ Call ftr-feed for every transition until a hit
    >r
    r@ fst-entry@ nil<>? IF       \ Execute the entry action for the next state
      r@ swap execute
    THEN
    r>
  ELSE
    r@ fst>any @ nil<>? IF        \ If nothing hits And any transition set Then
      ftr-fire                    \   Fire the any transition
    ELSE                          \ Else
      drop                        \   No hit
      nil
    THEN
  THEN
  rdrop
;


: fst-try          ( n fst -- fst | nil = Try the event for this state, return the result )
  >r
  ['] ftr-try r@ fst>transitions snl-execute? 0= IF  \ Call fst-try for every transition until a hit
    drop
    r@ fst>any @ nil<>? IF        \ No hit, try the any transition
      ftr>next @
    ELSE
      nil
    THEN
  THEN
  rdrop
;


( Private dot words )

: ftr-to-dot   ( fst tos ftr -- fst tos = Convert transition ftr to dot format, using output stream tos, origin state fst )
  swap >r
  [char] n                r@ tos-write-char           \ Write the transition
  over fst-id@            r@ tos-write-number
  s"  -> "                r@ tos-write-string
  [char] n                r@ tos-write-char
  dup ftr>next @ fst-id@  r@ tos-write-number
  
  dup ftr-label@ ?dup IF
    s"  [label="          r@ tos-write-string         \ Write the optional label
    [char] "              r@ tos-write-char
                          r@ tos-write-string
    [char] "              r@ tos-write-char
    [char] ]              r@ tos-write-char
  ELSE
    drop
  THEN
  
  ftr-attributes@ ?dup IF
    s"  ["                r@ tos-write-string         \ Write the optional extra attributes
                          r@ tos-write-string
    [char] ]              r@ tos-write-char
  ELSE
    drop
  THEN
  
  [char] ;                r@ tos-write-char           \ Finish the line
                          r@ tos-flush
  r>
;


: fst-to-dot       ( fst1 tos fst -- fst1 tos = Convert state fst to dot format, using output stream tos and fst1 as start state )
  swap >r
  [char] n       r@ tos-write-char          \ Write the node id
  dup fst-id@    r@ tos-write-number
  
  2dup = over fst>transitions snl-empty? OR IF \ If this is a start or end node Then
    s"  [shape=doublecircle"                   \   Double circle
  ELSE                                         \ Else
    s"  [shape=circle"                         \   Single circle
  THEN
  r@ tos-write-string

  dup fst-label@ ?dup IF
    s" ,label="   r@ tos-write-string       \ Write the optional label
    [char] "       r@ tos-write-char
                   r@ tos-write-string
    [char] "       r@ tos-write-char
  ELSE
    drop
  THEN
  
  dup fst-attributes@ ?dup IF
    [char] , r@ tos-write-char              \ Write the optional attributes
             r@ tos-write-string
  ELSE
    drop
  THEN
  s" ];" r@ tos-write-string                \ Finish the node line
         r@ tos-flush
                                            \ Write the transitions to the stream
  dup fst>transitions r@ swap ['] ftr-to-dot swap snl-execute drop

  dup fst>any @ nil<>? IF
    r@ swap ftr-to-dot drop                 \ Write the any transition to the stream
  THEN
  drop 
  
  r>
;


( Inspection )

: fst-dump   ( fst -- = Dump the fst variable )
  ." fst:" dup . cr
  ."  transitions: " ['] ftr-dump over fst>transitions snl-execute cr
  ."  id         : " dup fst>id ? cr
  ."  label      : " dup fst>label str-get type cr
  ."  data       : " dup fst>data ? cr
  ."  entry      : " dup fst>entry ? cr
  ."  exit       : " dup fst>exit ? cr
  ."  attributes : "     fst>attributes str-get type cr 
;

[THEN]

\ ==============================================================================
