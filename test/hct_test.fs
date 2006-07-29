\ ==============================================================================
\
\        hct_test - the test words for the hct module in the ffl
\
\               Copyright (C) 2005  Dick van Oudheusden
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
\  $Date: 2006-07-29 12:27:26 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/hct.fs
include ffl/hci.fs
include ffl/tos.fs

.( Testing: hct, hcn and hci) cr 
  
marker hct-test
  
tos-create t1
  
: hct-repeat-insert         ( n w:hct - )
  swap 0 DO
    I over >r
    dup 
    t1 tos-rewrite
    t1 tos-write-number
    t1 str-get
    r> hct-insert
  LOOP
  drop
;

: hct-sum ( n1 w c-addr u - n1+w )
  2drop                      \ remove key
  +
;


: hct-repeat-delete         ( n w:hct - = delete the nodes [different sequence])
  swap 2/ 0 DO
    I 500 + over >r
    t1 tos-rewrite
    t1 tos-write-number
    t1 str-get
    r> hct-delete
    2drop
    I over >r
    t1 tos-rewrite
    t1 tos-write-number
    t1 str-get
    r> hct-delete
    2drop
  LOOP
  drop
;

    
t{ 20 hct-create h1  }t

t{ h1 hct-length@  ?0      }t
t{ h1 hct-empty?   ?true   }t

t{ 1  s" one"   h1 hct-insert }t
t{ 2  s" two"   h1 hct-insert }t
t{ 3  s" three" h1 hct-insert }t
t{ 1  s" again" h1 hct-insert }t
t{ 1  s" same"  h1 hct-insert }t

t{ h1 hct-length@   5 ?s   }t
t{ h1 hct-empty?   ?false  }t

t{ s" one" h1 hct-has?  ?true      }t
t{ s" one" h1 hct-get   ?true 1 ?s }t
t{ s" zero" h1 hct-get  ?false     }t
t{ s" bye"  h1 hct-has? ?false     }t
t{ s" three" h1 hct-get ?true 3 ?s }t

t{ 0 h1 hct-count   ?0 }t
t{ 1 h1 hct-count 3 ?s }t
t{ 3 h1 hct-count 1 ?s }t

t{ 0 ' hct-sum h1 hct-execute 8 ?s }t

t{ s" again" h1 hct-delete ?true 1 ?s }t
t{ s" again" h1 hct-delete ?false }t

t{ h1 hct-length@ 4 ?s }t

t{ 0 ' hct-sum h1 hct-execute 7 ?s }t

\ Insert and delete a lot more nodes ..

t{ 50 hct-new value h2 }t

1000 h2 hct-repeat-insert

t{ h2 hct-length@   1000 ?s   }t

800 h2 hct-repeat-delete

t{ h2 hct-length@   200 ?s   }t

t{ h2 hct-free }t


\ Iterator test

\ hct-test

\ ==============================================================================

