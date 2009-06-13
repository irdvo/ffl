\ ==============================================================================
\
\        bnt_test - the test words for the bnt module in the ffl
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
\  $Date: 2008-04-12 05:56:55 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/bnt.fs
include ffl/bni.fs


.( Testing: bnt, bnn and bni) cr 
  

\ Simple test

t{ bnt-create bnt1  }t

t{ bnt1 bnt-length@  ?0      }t
t{ bnt1 bnt-empty?   ?true   }t

t{ ' bnn-new 5 bnt1 bnt-insert ?true value bnn1 }t
t{ ' bnn-new 3 bnt1 bnt-insert ?true value bnn2 }t
t{ ' bnn-new 4 bnt1 bnt-insert ?true value bnn3 }t
t{ ' bnn-new 7 bnt1 bnt-insert ?true value bnn4 }t
t{ ' bnn-new 2 bnt1 bnt-insert ?true value bnn5 }t
t{ ' bnn-new 6 bnt1 bnt-insert ?true value bnn6 }t
t{ ' bnn-new 8 bnt1 bnt-insert ?true value bnn7 }t
t{ ' bnn-new 4 bnt1 bnt-insert ?false bnn3 ?s }t
t{ ' bnn-new 1 bnt1 bnt-insert ?true value bnn8 }t


t{ bnt1 bnt-length@   8 ?s   }t
t{ bnt1 bnt-empty?   ?false  }t

t{ 1 bnt1 bnt-has?  ?true      }t
t{ 1 bnt1 bnt-get   ?true bnn8 ?s }t
t{ 0 bnt1 bnt-has?  ?false     }t
t{ 0 bnt1 bnt-get   ?false     }t
t{ 5 bnt1 bnt-get   ?true bnn1 ?s }t


: bnt-sum ( n bnn - n = Sum )
  bnn-key@ +
;

: bnt-count ( n bnn - n = Count the nodes )
  drop 1+
;

t{ 0 ' bnt-sum   bnt1 bnt-execute 36 ?s }t
t{ 0 ' bnt-count bnt1 bnt-execute  8 ?s }t


\ Iterator test

t{ bnt1 bni-create bni1 }t

t{ bni1 bni-first bnn8 ?s }t
t{ bni1 bni-get   bnn8 ?s }t
t{ bni1 bni-key   ?true 1 ?s }t

  t{ bni1 bni-first? ?true  }t
  t{ bni1 bni-last?  ?false }t

t{ bni1 bni-next  bnn5 ?s }t

  t{ bni1 bni-first? ?false }t
  t{ bni1 bni-last?  ?false }t

t{ bni1 bni-next  bnn2 ?s }t
t{ bni1 bni-key  ?true 3 ?s }t  

  t{ bni1 bni-first? ?false }t
  t{ bni1 bni-last?  ?false }t

t{ bni1 bni-next  bnn3 ?s }t
t{ bni1 bni-next  bnn1 ?s }t
t{ bni1 bni-next  bnn6 ?s }t
t{ bni1 bni-next  bnn4 ?s }t
t{ bni1 bni-next  bnn7 ?s }t

  t{ bni1 bni-first? ?false  }t
  t{ bni1 bni-last?  ?true }t

t{ bni1 bni-next  ?nil }t

t{ bni1 bni-first bnn8 ?s }t


t{ bni1 bni-last bnn7 ?s }t

  t{ bni1 bni-first? ?false  }t
  t{ bni1 bni-last?  ?true }t
  
t{ bni1 bni-prev  bnn4 ?s }t

  t{ bni1 bni-first? ?false  }t
  t{ bni1 bni-last?  ?false  }t
  
t{ bni1 bni-prev  bnn6 ?s }t
t{ bni1 bni-prev  bnn1 ?s }t
t{ bni1 bni-prev  bnn3 ?s }t
t{ bni1 bni-prev  bnn2 ?s }t
t{ bni1 bni-prev  bnn5 ?s }t
t{ bni1 bni-prev  bnn8 ?s }t

  t{ bni1 bni-first? ?true   }t
  t{ bni1 bni-last?  ?false  }t
  
t{ bni1 bni-prev  ?nil }t


\ Delete test

t{  5 bnt1 bnt-delete ?true bnn1 ?s }t


t{  7 bnt1 bnt-delete ?true bnn4 ?s }t
t{  2 bnt1 bnt-delete ?true bnn5 ?s }t
t{  5 bnt1 bnt-delete ?false        }t
t{ 12 bnt1 bnt-delete ?false        }t

t{ bnt1 bnt-length@ 5 ?s }t

t{ 0 ' bnt-sum   bnt1 bnt-execute 22 ?s }t
t{ 0 ' bnt-count bnt1 bnt-execute  5 ?s }t


t{ ' bnn-free bnt1 bnt-clear }t

t{ bnt1 bnt-length@ ?0 }t


\ Test compare

t{ bnt-new value bnt2 }t

: bnt-compare  ( n1 n2 -- n3 = Compare n1 and n2 inverted, return the result )
  <=> negate
;

t{ bnt2 bnt-length@ ?0 }t

t{ ' bnt-compare bnt2 bnt-compare!   }t
t{ bnt2 bnt-compare@ ' bnt-compare ?s }t

t{ ' bnn-new 5 bnt2 bnt-insert ?true to bnn1 }t
t{ ' bnn-new 3 bnt2 bnt-insert ?true to bnn2 }t
t{ ' bnn-new 4 bnt2 bnt-insert ?true to bnn3 }t
t{ ' bnn-new 7 bnt2 bnt-insert ?true to bnn4 }t
t{ ' bnn-new 2 bnt2 bnt-insert ?true to bnn5 }t
t{ ' bnn-new 6 bnt2 bnt-insert ?true to bnn6 }t
t{ ' bnn-new 8 bnt2 bnt-insert ?true to bnn7 }t
t{ ' bnn-new 4 bnt2 bnt-insert ?false bnn3 ?s }t
t{ ' bnn-new 1 bnt2 bnt-insert ?true to bnn8 }t

t{ bnt2 bnt-length@ 8 ?s }t

: bnt-count2   ( n bnn -- n flag = Count till key 6 )
  swap 1+ swap bnn-key@ 6 =
;

t{ 0 ' bnt-count2 bnt2 bnt-execute? ?true 3 ?s }t


t{ bnt2 bni-new value bni2 }t

t{ bni2 bni-first bnn7 ?s }t
t{ bni2 bni-next  bnn4 ?s }t
t{ bni2 bni-next  bnn6 ?s }t
t{ bni2 bni-next  bnn1 ?s }t
t{ bni2 bni-next  bnn3 ?s }t
t{ bni2 bni-next  bnn2 ?s }t
t{ bni2 bni-next  bnn5 ?s }t
t{ bni2 bni-next  bnn8 ?s }t
t{ bni2 bni-next  ?nil }t

t{ bni2 bni-free }t
t{ bnt2 bnt-free }t

[THEN]

\ ==============================================================================

