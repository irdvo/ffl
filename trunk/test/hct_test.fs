\ ==============================================================================
\
\        hct_test - the test words for the hct module in the ffl
\
\               Copyright (C) 2006  Dick van Oudheusden
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
\  $Date: 2007-12-24 19:32:12 $ $Revision: 1.9 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/hct.fs
 include ffl/hci.fs
include ffl/tos.fs

.( Testing: hct, hcn and hci) cr 
  
  
tos-create tosh
  
: hct-repeat-insert         ( n w:hct - )
  swap 0 DO
    I over >r
    dup 
    tosh tos-rewrite
    tosh tos-write-number
    tosh str-get
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
    tosh tos-rewrite
    tosh tos-write-number
    tosh str-get
    r> hct-delete
    2drop
    I over >r
    tosh tos-rewrite
    tosh tos-write-number
    tosh str-get
    r> hct-delete
    2drop
  LOOP
  drop
;

    
t{ 3 hct-create hct1  }t

t{ hct1 hct-length@  ?0      }t
t{ hct1 hct-empty?   ?true   }t

t{ 200 hct1 hct-load! }t

t{ 1  s" one"   hct1 hct-insert }t
t{ 2  s" two"   hct1 hct-insert }t
t{ 3  s" three" hct1 hct-insert }t
t{ 1  s" again" hct1 hct-insert }t
t{ 5  s" same"  hct1 hct-insert }t
t{ 1  s" same"  hct1 hct-insert }t   \ actually replace of cell

t{ hct1 hct-length@   5 ?s   }t
t{ hct1 hct-empty?   ?false  }t

t{ s" one" hct1 hct-has?  ?true      }t
t{ s" one" hct1 hct-get   ?true 1 ?s }t
t{ s" zero" hct1 hct-get  ?false     }t
t{ s" bye"  hct1 hct-has? ?false     }t
t{ s" three" hct1 hct-get ?true 3 ?s }t

t{ 0 hct1 hct-count   ?0 }t
t{ 1 hct1 hct-count 3 ?s }t
t{ 3 hct1 hct-count 1 ?s }t

t{ 0 ' hct-sum hct1 hct-execute 8 ?s }t

t{ s" again" hct1 hct-delete ?true 1 ?s }t
t{ s" again" hct1 hct-delete ?false }t

t{ hct1 hct-length@ 4 ?s }t

t{ 0 ' hct-sum hct1 hct-execute 7 ?s }t

: hct-count?  ( n1 n2 n3 c-addr u --  n4 n5 flag = n1: stop level, n2: count, n3: cell value, c-addr u: key n4: stop level n5: count flag:stop? )
  2drop drop 1+ 2dup <
;

t{ 2 0 ' hct-count? hct1 hct-execute? ?true  3 ?s 2 ?s }t
t{ 5 0 ' hct-count? hct1 hct-execute? ?false 4 ?s 5 ?s }t


\ Insert and delete a lot more nodes ..

t{ 50 hct-new value hct2 }t

t{ hct2 hct-load@ 100 ?s }t
t{ 200 hct2 hct-load!    }t   \ rehash when length > 200% of size
t{ hct2 hct-load@ 200 ?s }t

1000 hct2 hct-repeat-insert

t{ hct2 hct-length@   1000 ?s   }t

800 hct2 hct-repeat-delete

t{ hct2 hct-length@   200 ?s   }t

t{ hct2 hct-free }t


\ Iterator test

t{ hct1 hci-new value hci1 }t

t{ hci1 hci-first ?true 2 ?s }t
t{ hci1 hci-get   ?true 2 ?s }t
t{ hci1 hci-key   s" two" ?str }t

  t{ hci1 hci-first? ?true  }t
  t{ hci1 hci-last?  ?false }t

t{ hci1 hci-next  ?true 1 ?s }t

  t{ hci1 hci-first? ?false }t
  t{ hci1 hci-last?  ?false }t

t{ hci1 hci-next  ?true 3 ?s }t
  
  t{ hci1 hci-first? ?false }t
  t{ hci1 hci-last?  ?false }t

t{ hci1 hci-next  ?true 1 ?s }t

  t{ hci1 hci-first? ?false  }t
  t{ hci1 hci-last?  ?true }t

t{ hci1 hci-next  ?false  }t


t{ hci1 hci-first ?true drop }t

t{ 1 hci1 hci-move ?true  }t
t{ 1 hci1 hci-move ?true  }t
t{ 1 hci1 hci-move ?false }t

t{ hci1 hci-free }t

\ ==============================================================================

