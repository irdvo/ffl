\ ==============================================================================
\
\        snl_test - the test words for the snl module in the ffl
\
\               Copyright (C) 2007  Dick van Oudheusden
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
\  $Date: 2007-02-27 06:06:15 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/snl.fs
\ include ffl/dni.fs


.( Testing: snl, snn and sni) cr 

t{ snl-create snl1  }t

t{    snl1 snl-length@   ?0   }t
t{    snl1 snl-empty?   ?true   }t

t{ snn-new value snn1 }t
t{ snn-new value snn2 }t
t{ snn-new value snn3 }t
t{ snn-new value snn4 }t
t{ snn-new value snn5 }t

\ append & prepend tests

t{ snn1 snl1 snl-append   }t
t{ snn2 snl1 snl-append   }t
t{ snn3 snl1 snl-append   }t

t{ snn4 snl1 snl-prepend   }t
t{ snn5 snl1 snl-prepend   }t

t{ snl1 snl-length@   5 ?s   }t
t{ snl1 snl-empty?   ?false  }t

\ execute test

: snl-test-count ( u w:snn - u+1 )
  drop 1+
;

t{ 0 ' snl-test-count snl1 snl-execute 5 ?s }t

\ Iterator test

\ t{ snl1 dni-create dni1 }t

\ t{ dni1 dni-first snn5 ?s }t
\ t{ dni1 dni-get   snn5 ?s }t
\ t{ dni1 dni-first? ?true  }t
\ t{ dni1 dni-last?  ?false }t

\ t{ dni1 dni-next snn4 ?s  }t
\ t{ dni1 dni-next snn1 ?s  }t
\ t{ dni1 dni-first? ?false }t
\ t{ dni1 dni-last?  ?false }t

\ t{ dni1 dni-next snn2 ?s  }t
\ t{ dni1 dni-next snn3 ?s  }t
\ t{ dni1 dni-first? ?false }t
\ t{ dni1 dni-last?  ?true  }t

\ t{ dni1 dni-next ?nil     }t

\ pop tests

t{ snl1 snl-pop snn5 ?s }t

t{ snl1 snl-length@   4 ?s   }t

t{ snl1 snl-pop snn4 ?s }t
t{ snl1 snl-pop snn1 ?s }t
t{ snl1 snl-pop snn2 ?s }t
t{ snl1 snl-pop snn3 ?s }t

t{ snl1 snl-empty?   ?true }t

t{ snl1 snl-pop ?nil }t

t{ snl1 snl-first@ ?nil }t
t{ snl1 snl-last@  ?nil }t

t{ snl1 snl-pop ?nil }t



t{ snl-new value snl2 }t

\ insert tests

t{ snn1 0 snl2 snl-insert }t
t{ snn2 1 snl2 snl-insert }t
t{ snn3 1 snl2 snl-insert }t
t{ snn4 0 snl2 snl-insert }t
t{ snn5 4 snl2 snl-insert }t

t{ snl2 snl-length@  5 ?s }t

\ get tests

t{ 0 snl2 snl-get snn4 ?s }t
t{ 1 snl2 snl-get snn1 ?s }t
t{ 2 snl2 snl-get snn3 ?s }t
t{ 3 snl2 snl-get snn2 ?s }t
t{ 4 snl2 snl-get snn5 ?s }t

\ index tests

t{ 0  snl2 snl-index? ?true  }t
t{ 4  snl2 snl-index? ?true  }t
t{ 5  snl2 snl-index? ?false }t
t{ 99 snl2 snl-index? ?false }t

t{ -1  snl2 snl-index? ?true  }t
t{ -5  snl2 snl-index? ?true  }t
t{ -6  snl2 snl-index? ?false }t
t{ -99 snl2 snl-index? ?false }t

t{ snl2 snl-reverse }t

t{ 0 snl2 snl-get snn5 ?s }t
t{ 1 snl2 snl-get snn2 ?s }t
t{ 2 snl2 snl-get snn3 ?s }t
t{ 3 snl2 snl-get snn1 ?s }t
t{ 4 snl2 snl-get snn4 ?s }t

\ Iterator test

\ t{ snl2 dni-new value dni2 }t

\ t{ dni2 dni-last snn4 ?s  }t
\ t{ dni2 dni-get  snn4 ?s  }t
\ t{ dni2 dni-last?  ?true  }t
\ t{ dni2 dni-first? ?false }t

\ t{ dni2 dni-prev snn1 ?s  }t
\ t{ dni2 dni-last?  ?false }t
\ t{ dni2 dni-first? ?false }t

\ t{ dni2 dni-prev snn3 ?s  }t
\ t{ dni2 dni-prev snn2 ?s  }t
\ t{ dni2 dni-prev snn5 ?s  }t

\ t{ dni2 dni-last?  ?false }t
\ t{ dni2 dni-first? ?true  }t

\ t{ dni2 dni-prev ?nil     }t
\ t{ dni2 dni-get  ?nil     }t

\ t{ dni2 dni-free          }t

\ delete tests

t{ 0 snl2 snl-delete snn5 ?s }t
t{ 2 snl2 snl-delete snn1 ?s }t

t{ 0 snl2 snl-get snn2 ?s }t
t{ 1 snl2 snl-get snn3 ?s }t
t{ 2 snl2 snl-get snn4 ?s }t

t{ 1 snl2 snl-delete snn3 ?s }t
t{ 0 snl2 snl-delete snn2 ?s }t
t{ 0 snl2 snl-delete snn4 ?s }t

t{ snl2 snl-length@  ?0 }t
t{ snl2 snl-first@ ?nil }t
t{ snl2 snl-last@  ?nil }t

\ insert-before & insert-after tests

t{ snn1 snl2 snl-append }t
t{ snn4 snn1 snl2 snl-insert-after  }t
t{ snn5 snn1 snl2 snl-insert-after  }t
t{ snn3 snn4 snl2 snl-insert-after  }t
t{ snn2 snn5 snl2 snl-insert-after  }t

t{ snl2 snl-length@  5 ?s }t

t{ 0 snl2 snl-get snn1 ?s }t
t{ 1 snl2 snl-get snn5 ?s }t
t{ 2 snl2 snl-get snn2 ?s }t
t{ 3 snl2 snl-get snn4 ?s }t
t{ 4 snl2 snl-get snn3 ?s }t

t{ snn2 snl2 snl-remove-after snn4 ?s }t
t{ snn3 snl2 snl-remove-after ?nil    }t
t{ snn2 snl2 snl-remove-after snn3 ?s }t

t{ 0 snl2 snl-get snn1 ?s }t
t{ 1 snl2 snl-get snn5 ?s }t
t{ 2 snl2 snl-get snn2 ?s }t

t{   snl2 snl-last@   snn2 ?s }t
t{   snl2 snl-first@  snn1 ?s }t

t{   snl2 snl-length@ 3 ?s }t

t{ snl2 snl-free }t

t{ snn1 snn-free }t
t{ snn2 snn-free }t
t{ snn3 snn-free }t
t{ snn4 snn-free }t
t{ snn5 snn-free }t

\ ==============================================================================

