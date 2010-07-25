\ ==============================================================================
\
\        fsm_test - the test words for the fsm module in the ffl
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
\  $Date: 2009-05-20 13:46:51 $ $Revision: 1.7 $
\
\ ==============================================================================

include ffl/fsm.fs
include ffl/enm.fs
include ffl/tst.fs


.( Testing: fsm, fst, ftr) cr

t{ 256 fsm-create fsm1 }t

variable fsm-entries  fsm-entries 0!
variable fsm-exits    fsm-exits   0!
variable fsm-hits     fsm-hits    0!

: fsm-test-entry  ( fst -- = Test the entry of a state )
  \ ." Entry: " dup fst-label@ type cr
  dup fst-id@ swap fst-data@ = IF
    fsm-entries 1+!
  THEN
;

: fsm-test-exit    ( fst -- = Test the exit of a state )
  \ ." Exit: " dup fst-label@ type cr
  dup fst-id@ swap fst-data@ = IF
    fsm-exits 1+!
  THEN
;

: fsm-test-hit     ( n ftr -- = Test the transition hit )
  \ ." Hit: " dup ftr-label@ type ."  with event: " over . cr
  fsm-hits 1+!
  2drop
;


t{ 9 ' fsm-test-entry ' fsm-test-exit s" start"    fsm1 fsm-new-state value fst1 }t
t{ 2 ' fsm-test-entry ' fsm-test-exit s" sign"     fsm1 fsm-new-state value fst2 }t
t{ 3 ' fsm-test-entry ' fsm-test-exit s" number"   fsm1 fsm-new-state value fst3 }t
t{ 4 ' fsm-test-entry ' fsm-test-exit s" dot"      fsm1 fsm-new-state value fst4 }t
t{ 5 ' fsm-test-entry ' fsm-test-exit s" float"    fsm1 fsm-new-state value fst5 }t
t{ 6 ' fsm-test-entry ' fsm-test-exit s" exp"      fsm1 fsm-new-state value fst6 }t
t{ 7 ' fsm-test-entry ' fsm-test-exit s" mantissa" fsm1 fsm-new-state value fst7 }t
t{ 8 ' fsm-test-entry ' fsm-test-exit s" done"     fsm1 fsm-new-state value fst8 }t

t{ fst1 fst-id@ 1 ?s }t
t{ fst8 fst-id@ 8 ?s }t

t{ fst2 fst-label@ s" sign" ?str }t
t{ fst8 fst-label@ s" done" ?str }t

t{ fst1 fst-data@ 9 ?s }t
t{ 1 fst1 fst-data!    }t
t{ fst1 fst-data@ 1 ?s }t

t{ fst4 fst-entry@ ' fsm-test-entry ?s }t
t{ fst5 fst-exit@  ' fsm-test-exit  ?s }t

t{ fst8 fst-attributes@ s" " ?str          }t
t{ s" color=red" fst8 fst-attributes!      }t
t{ fst8 fst-attributes@ s" color=red" ?str }t

t{ s" mantissa" fsm1 fsm-find-state fst7 ?s }t
t{ s" unknown"  fsm1 fsm-find-state ?nil    }t
t{ s" start"    fsm1 fsm-find-state fst1 ?s }t
t{ s" done"     fsm1 fsm-find-state fst8 ?s }t

t{ 1  ' fsm-test-hit s" +-"  fst1 fst2 fsm1 fsm-new-transition ftr-condition@ char + over bar-set-bit char - swap bar-set-bit }t
t{ 2  ' fsm-test-hit s" 0-9" fst1 fst3 fsm1 fsm-new-transition ftr-condition@ 10 char 0 rot bar-set-bits }t
t{ 3  ' fsm-test-hit s" 0-9" fst2 fst3 fsm1 fsm-new-transition ftr-condition@ 10 char 0 rot bar-set-bits }t
t{ 4  ' fsm-test-hit s" 0-9" fst3 fst3 fsm1 fsm-new-transition ftr-condition@ 10 char 0 rot bar-set-bits }t
t{ 5  ' fsm-test-hit s" ."   fst3 fst4 fsm1 fsm-new-transition ftr-condition@ char . swap bar-set-bit }t
t{ 6  ' fsm-test-hit s" eE"  fst3 fst6 fsm1 fsm-new-transition ftr-condition@ char e over bar-set-bit char E swap bar-set-bit }t
t{ 7  ' fsm-test-hit s" 0-9" fst4 fst5 fsm1 fsm-new-transition ftr-condition@ 10 char 0 rot bar-set-bits }t
t{ 8  ' fsm-test-hit s" 0-9" fst5 fst5 fsm1 fsm-new-transition ftr-condition@ 10 char 0 rot bar-set-bits }t
t{ 9  ' fsm-test-hit s" eE"  fst5 fst6 fsm1 fsm-new-transition ftr-condition@ char e over bar-set-bit char E swap bar-set-bit }t
t{ 10 ' fsm-test-hit s" any" fst5 fst8 fsm1 fsm-any-transition drop }t
t{ 11 ' fsm-test-hit s" 0-9" fst6 fst7 fsm1 fsm-new-transition ftr-condition@ 10 char 0 rot bar-set-bits }t
t{ 12 ' fsm-test-hit s" 0-9" fst7 fst7 fsm1 fsm-new-transition ftr-condition@ 10 char 0 rot bar-set-bits }t
t{ 13 ' fsm-test-hit s" any" fst7 fst8 fsm1 fsm-any-transition drop }t
t{ 99 ' fsm-test-hit s" x"   fst7 fst8 fsm1 fsm-new-transition ftr-condition@ char x swap bar-set-bit }t

t{ s" +-" fst1 fst-find-transition ftr-data@ 1 ?s  }t
t{ s" ."  fst1 fst-find-transition ?nil            }t
t{ s" ?"  fst8 fst-find-transition ?nil            }t
t{ s" x"  fst7 fst-find-transition value ftr1      }t

t{ ftr1 ftr-data@ 99 ?s }t
t{ 14 ftr1 ftr-data!    }t
t{ ftr1 ftr-data@ 14 ?s }t

t{ ftr1 ftr-action@ ' fsm-test-hit ?s }t

t{ ftr1 ftr-condition@ bar-count 1 ?s }t
t{ ftr1 ftr-label@ s" x" ?str }t
t{ ftr1 ftr-attributes@ s" " ?str }t
t{ s" color=red" ftr1 ftr-attributes! }t
t{ ftr1 ftr-attributes@ s" color=red" ?str }t


t{ fsm1 fsm-start@ fst1 ?s }t

t{ fsm1 fsm-start }t

t{ char + fsm1 fsm-feed fst2 ?s }t
t{ char 1 fsm1 fsm-feed fst3 ?s }t
t{ char . fsm1 fsm-feed fst4 ?s }t
t{ char 7 fsm1 fsm-feed fst5 ?s }t
t{ char 2 fsm1 fsm-feed fst5 ?s }t
t{ char e fsm1 fsm-feed fst6 ?s }t
t{ char 5 fsm1 fsm-feed fst7 ?s }t
t{ bl     fsm1 fsm-try  fst8 ?s }t
t{ bl     fsm1 fsm-feed fst8 ?s }t

t{ fsm1 fsm-start }t

t{ char . fsm1 fsm-feed ?nil }t

t{ fst3 fsm1 fsm-start! }t

t{ fsm1 fsm-start }t

t{ char + fsm1 fsm-try ?nil }t

t{ char . fsm1 fsm-try fst4 ?s }t

t{ char 0 fsm1 fsm-feed fst3 ?s }t

\ t{ fsm1 fsm-(free) }t



begin-enumeration
enum: voilence
enum: coin
enum: choice
enum: refuse
enum: #events
end-enumeration

t{ #events fsm-new value machine }t

variable fsm-test-counter   fsm-test-counter 0!

: say-thank-you
  drop
  fsm-test-counter @ 7 = IF fsm-test-counter 1+! THEN
  fsm-test-counter @ 6 = IF fsm-test-counter 1+! THEN
  fsm-test-counter @ 4 = IF fsm-test-counter 1+! THEN
  fsm-test-counter @ 2 = IF fsm-test-counter 1+! THEN
  fsm-test-counter @ 0 = IF fsm-test-counter 1+! THEN
;

: say-choice?
  drop
  fsm-test-counter @ 1 = IF fsm-test-counter 1+! THEN
;

: say-coin?
  drop
  fsm-test-counter @ 5 = IF fsm-test-counter 1+! THEN
;

: call-support ( fst -- = Call support )
  fst-data@ 112 = fsm-test-counter @ 8 = AND IF fsm-test-counter 1+! THEN
;

t{ 1     nil            ' say-thank-you s" start"    machine fsm-new-state value start   }t
t{ 1   ' say-choice?    ' say-thank-you s" choice?"  machine fsm-new-state value choice? }t
t{ 1   ' say-coin?      ' say-thank-you s" coin?"    machine fsm-new-state value coin?   }t
t{ 112 ' call-support     nil           s" support"  machine fsm-new-state value support }t

t{ s" color=red" support fst-attributes! }t

t{ 1     nil             s" coin"      start choice? machine fsm-new-transition ftr-condition@ coin     swap bar-set-bit }t
t{ 1     nil             s" choice"    start coin?   machine fsm-new-transition ftr-condition@ choice   swap bar-set-bit }t
t{ 1     nil             s" voilence"  start support machine fsm-new-transition ftr-condition@ voilence swap bar-set-bit }t

t{ s" voilence" start fst-find-transition s" color=yellow" rot ftr-attributes! }t

: deliver-choice
  2drop
  fsm-test-counter @ 3 = IF fsm-test-counter 1+! THEN
;

: do-refund
  2drop
;

t{ 1   ' deliver-choice  s" choice"    choice? start   machine fsm-new-transition ftr-condition@ choice   swap bar-set-bit }t
t{ 1   ' do-refund       s" refuse"    choice? start   machine fsm-new-transition ftr-condition@ refuse   swap bar-set-bit }t
t{ 1     nil             s" voilence"  choice? support machine fsm-new-transition ftr-condition@ voilence swap bar-set-bit }t

t{ s" voilence" choice? fst-find-transition s" color=yellow" rot ftr-attributes! }t

t{ 1   ' deliver-choice  s" coin"      coin? start   machine fsm-new-transition ftr-condition@ coin     swap bar-set-bit }t
t{ 1     nil             s" refuse"    coin? start   machine fsm-new-transition ftr-condition@ refuse   swap bar-set-bit }t
t{ 1     nil             s" voilence"  coin? support machine fsm-new-transition ftr-condition@ voilence swap bar-set-bit }t

t{ s" voilence" coin? fst-find-transition s" color=yellow" rot ftr-attributes! }t

t{ machine fsm-start }t

t{ coin     machine fsm-feed choice? ?s }t
t{ choice   machine fsm-feed start   ?s }t
t{ choice   machine fsm-feed coin?   ?s }t
t{ refuse   machine fsm-feed start   ?s }t
t{ voilence machine fsm-feed support ?s }t

t{ fsm-test-counter @ 9 ?s }t


tos-new value fsm-tos

fsm-test-counter 0!

: fsm-tos-write    ( c-addr u x -- flag = Write the string )
  drop
  fsm-test-counter @ CASE
     0 OF s" digraph Machine {" compare 0= IF fsm-test-counter 1+! THEN ENDOF
     1 OF s" rankdir=LR;" compare 0= IF fsm-test-counter 1+! THEN ENDOF
     2 OF s\" n1 [shape=doublecircle,label=\"start\"];" compare 0= IF fsm-test-counter 1+! THEN ENDOF
     3 OF s\" n1 -> n2 [label=\"coin\"];" compare 0= IF fsm-test-counter 1+! THEN ENDOF
     4 OF s\" n1 -> n3 [label=\"choice\"];" compare 0= IF fsm-test-counter 1+! THEN ENDOF
     5 OF s\" n1 -> n4 [label=\"voilence\"] [color=yellow];" compare 0= IF fsm-test-counter 1+! THEN ENDOF
     6 OF s\" n2 [shape=circle,label=\"choice?\"];" compare 0= IF fsm-test-counter 1+! THEN ENDOF
     7 OF s\" n2 -> n1 [label=\"choice\"];" compare 0= IF fsm-test-counter 1+! THEN ENDOF
     8 OF s\" n2 -> n1 [label=\"refuse\"];" compare 0= IF fsm-test-counter 1+! THEN ENDOF
     9 OF s\" n2 -> n4 [label=\"voilence\"] [color=yellow];" compare 0= IF fsm-test-counter 1+! THEN ENDOF
    10 OF s\" n3 [shape=circle,label=\"coin?\"];" compare 0= IF fsm-test-counter 1+! THEN ENDOF
    11 OF s\" n3 -> n1 [label=\"coin\"];" compare 0= IF fsm-test-counter 1+! THEN ENDOF
    12 OF s\" n3 -> n1 [label=\"refuse\"];" compare 0= IF fsm-test-counter 1+! THEN ENDOF
    13 OF s\" n3 -> n4 [label=\"voilence\"] [color=yellow];" compare 0= IF fsm-test-counter 1+! THEN ENDOF
    14 OF s\" n4 [shape=doublecircle,label=\"support\",color=red];" compare 0= IF fsm-test-counter 1+! THEN ENDOF
    15 OF s" }" compare 0= IF fsm-test-counter 1+! THEN ENDOF
    >r 2drop r>
  ENDCASE
  true
;


0 ' fsm-tos-write fsm-tos tos-set-writer


t{ s" Machine" fsm-tos machine fsm-to-dot }t

t{ fsm-test-counter @ 16 ?s }t

t{ fsm-tos tos-free }t

t{ machine fsm-free }t

\ ==============================================================================
