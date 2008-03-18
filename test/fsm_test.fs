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
\  $Date: 2008-03-18 19:09:48 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/fsm.fs
include ffl/tst.fs


.( Testing: fsm, fst, ftr) cr

t{ 256 fsm-create fsm1 }t

variable fsm-entries  fsm-entries 0!
variable fsm-exits    fsm-exits   0!
variable fsm-hits     fsm-hits    0!

: fsm-test-entry  ( fst -- = Test the entry of a state )
  ." Entry: " dup fst-label@ type cr
  dup fst-id@ swap fst-data@ = IF
    fsm-entries 1+!
  THEN
;

: fsm-test-exit    ( fst -- = Test the exit of a state )
  ." Exit: " dup fst-label@ type cr
  dup fst-id@ swap fst-data@ = IF
    fsm-exits 1+!
  THEN
;

: fsm-test-hit     ( n ftr -- = Test the transition hit )
  ." Hit: " dup ftr-label@ type ."  with event: " over . cr
  fsm-hits 1+!
  2drop
;


t{ 1 ' fsm-test-entry ' fsm-test-exit s" start"    fsm1 fsm-new-state value fst1 }t
t{ 2 ' fsm-test-entry ' fsm-test-exit s" sign"     fsm1 fsm-new-state value fst2 }t
t{ 3 ' fsm-test-entry ' fsm-test-exit s" number"   fsm1 fsm-new-state value fst3 }t
t{ 4 ' fsm-test-entry ' fsm-test-exit s" dot"      fsm1 fsm-new-state value fst4 }t
t{ 5 ' fsm-test-entry ' fsm-test-exit s" float"    fsm1 fsm-new-state value fst5 }t
t{ 6 ' fsm-test-entry ' fsm-test-exit s" exp"      fsm1 fsm-new-state value fst6 }t
t{ 7 ' fsm-test-entry ' fsm-test-exit s" mantissa" fsm1 fsm-new-state value fst7 }t
t{ 8 ' fsm-test-entry ' fsm-test-exit s" done"     fsm1 fsm-new-state value fst8 }t

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

t{ s" +-" fst1 fst-find-transition fst-data@ 1 ?s }t
t{ s" ."  fst1 fst-find-transition ?nil           }t
t{ s" ?"  fst8 fst-find-transition ?nil           }t

t{ fsm1 fsm-start }t

t{ char + fsm1 fsm-feed fst2 ?s }t
t{ char 1 fsm1 fsm-feed fst3 ?s }t
t{ char . fsm1 fsm-feed fst4 ?s }t
t{ char 7 fsm1 fsm-feed fst5 ?s }t
t{ char 2 fsm1 fsm-feed fst5 ?s }t
t{ char e fsm1 fsm-feed fst6 ?s }t
t{ char 5 fsm1 fsm-feed fst7 ?s }t
t{ bl     fsm1 fsm-feed fst8 ?s }t

t{ fsm1 fsm-start }t

t{ char . fsm1 fsm-feed ?nil }t

tos-new value fsm-tos

: fsm-tos-write    ( c-addr u x -- flag = Write the string )
  drop type true
;

0 ' fsm-tos-write fsm-tos tos-set-writer

t{ s" test" fsm-tos fsm1 fsm-to-dot }t


\ ==============================================================================
