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
\  $Date: 2008-03-24 20:48:38 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/fsm.fs
include ffl/enm.fs


\ Example: vending machine


\ Enumerate the events

begin-enumeration
enum: voilence
enum: coin
enum: choice
enum: refuse
enum: #events
end-enumeration


\ Create the finite state machine on the heap with #events events

#events fsm-new value machine


\ Create the entry and exit action words

: say-thank-you  ( fst -- = Say 'thank you' after a coin or choice )
  drop
  ." Thank you" cr
;

: say-choice?    ( fst -- = Ask for choice after the coin )
  drop
  ." Please make your choice" cr
;

: say-coin?      ( fst -- = Ask for coin after the choice )
  drop
  ." Please enter your coin" cr
;

: call-support   ( fst -- = Call support after voilence, states data contains the phone number )
  ." Voilence against the machine, calling: " fst-data@ . cr
;


\ Create the states in the state machine

0     nil            ' say-thank-you s" start"    machine fsm-new-state value start
0   ' say-choice?    ' say-thank-you s" choice?"  machine fsm-new-state value choice?
0   ' say-coin?      ' say-thank-you s" coin?"    machine fsm-new-state value coin?
112 ' call-support     nil           s" support"  machine fsm-new-state value support


\ Set extra dot attributes for the support state

s" color=red" support fst-attributes!


\ Create the transitions for state start, use the bit array to set the condition

0     nil             s" coin"      start choice? machine fsm-new-transition ftr-condition@ coin     swap bar-set-bit
0     nil             s" choice"    start coin?   machine fsm-new-transition ftr-condition@ choice   swap bar-set-bit
0     nil             s" voilence"  start support machine fsm-new-transition ftr-condition@ voilence swap bar-set-bit

s" voilence" start fst-find-transition s" color=yellow" rot ftr-attributes!


\ Create the transition actions for choice? and coin? states

: deliver-choice   ( n ftr -- = Deliver the choosen product )
  2drop
  ." Deliver choice" cr
;

: do-refund        ( n ftr -- = Refused the product, refund the coin )
  2drop
  ." Refund coin" cr
;


\ Create the transitions for state choice?

0   ' deliver-choice  s" choice"    choice? start   machine fsm-new-transition ftr-condition@ choice   swap bar-set-bit
0   ' do-refund       s" refuse"    choice? start   machine fsm-new-transition ftr-condition@ refuse   swap bar-set-bit
0     nil             s" voilence"  choice? support machine fsm-new-transition ftr-condition@ voilence swap bar-set-bit


\ Set extra dot attributes for the voilence transition

s" voilence" choice? fst-find-transition s" color=yellow" rot ftr-attributes!


\ Create the transitions for state coin?

0   ' deliver-choice  s" coin"      coin? start   machine fsm-new-transition ftr-condition@ coin     swap bar-set-bit
0     nil             s" refuse"    coin? start   machine fsm-new-transition ftr-condition@ refuse   swap bar-set-bit
0     nil             s" voilence"  coin? support machine fsm-new-transition ftr-condition@ voilence swap bar-set-bit

s" voilence" coin? fst-find-transition s" color=yellow" rot ftr-attributes!


\ Start the state machine and feed it events 

machine fsm-start

coin     machine fsm-feed drop
choice   machine fsm-feed drop
coin     machine fsm-feed drop
refuse   machine fsm-feed drop
choice   machine fsm-feed drop
coin     machine fsm-feed drop
voilence machine fsm-feed drop


\ Create a text output stream for writing the state machine to dot

tos-new value dot-tos


\ Create the writer word

: write-dot    ( c-addr u file-id -- flag = Write the string )
  write-line 
  0=
;


\ Create the output file

s" out.dot" w/o create-file throw value dot-file


\ Set the writer word and the file in the stream

dot-file  ' write-dot  dot-tos  tos-set-writer


\ Write the state machine to the dot-file with graph name "Machine"

s" Machine" dot-tos machine fsm-to-dot


\ Free the dot stream

dot-tos tos-free


\ Close the dot file

dot-file close-file throw


\ To generate an image, use for example: dot -Tpng -o fsm.png out.dot

\ ==============================================================================
