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
\  $Date: 2006-07-27 18:08:01 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/hct.fs
include ffl/hci.fs
include ffl/tos.fs

.( Testing: hct, hcn and hci) cr 
  
marker hct-test
  
tos-create t1
  
: hct-repeat         ( w:hct - )
  1000 0 DO
    I over >r
    dup 
    t1 tos-rewrite
    t1 tos-write-number
    t1 str-get
    r> hct-insert
  LOOP
  drop
;

t{ 20 hct-create h1  }t

t{ h1 hct-length@  ?0      }t
t{ h1 hct-empty?   ?true   }t

t{ 1  s" one"   h1 hct-insert }t
t{ 2  s" two"   h1 hct-insert }t
t{ 3  s" three" h1 hct-insert }t

t{ h1 hct-length@   3 ?s   }t
t{ h1 hct-empty?   ?false  }t


\ t{ 3  l1 scl-count   2 ?s   }t
\ t{ 4  l1 scl-count   ?0   }t

t{ s" one" h1 hct-has?  ?true      }t
t{ s" one" h1 hct-get   ?true 1 ?s }t
t{ s" zero" h1 hct-get  ?false     }t
t{ s" bye"  h1 hct-has? ?false     }t
t{ s" three" h1 hct-get ?true 3 ?s }t

h1 hct-repeat

t{ h1 hct-length@   1003 ?s   }t

\ t{    l1 scl-length@   ?0   }t
\ t{    l1 scl-empty?   ?true   }t

\ t{ scl-new constant l2   }t

\ Insert-sorted

\ t{    l2 scl-free   }t

\ Iterator test

\ t{ 0 ' + l1 scl-execute  6 ?s   }t \ sum contents list

\ hct-test

\ ==============================================================================

