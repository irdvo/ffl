\ ==============================================================================
\
\        dnl_test - the test words for the dnl module in the ffl
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
\  $Date: 2007-01-06 06:31:19 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/dnl.fs
include ffl/dni.fs


.( Testing: dnl, dnn and dni) cr 

t{ dnl-create dnl1  }t

t{    dnl1 dnl-length@   ?0   }t
t{    dnl1 dnl-empty?   ?true   }t

t{ dnn-new value dnn1 }t
t{ dnn-new value dnn2 }t
t{ dnn-new value dnn3 }t
t{ dnn-new value dnn4 }t
t{ dnn-new value dnn5 }t

\ append & prepend tests

t{ dnn1 dnl1 dnl-append   }t
t{ dnn2 dnl1 dnl-append   }t
t{ dnn3 dnl1 dnl-append   }t

t{ dnn4 dnl1 dnl-prepend   }t
t{ dnn5 dnl1 dnl-prepend   }t

t{ dnl1 dnl-length@   5 ?s   }t
t{ dnl1 dnl-empty?   ?false  }t

\ Iterator test

t{ dnl1 dni-create dni1 }t

t{ dni1 dni-first dnn5 ?s }t
t{ dni1 dni-get   dnn5 ?s }t
t{ dni1 dni-first? ?true  }t
t{ dni1 dni-last?  ?false }t

t{ dni1 dni-next dnn4 ?s  }t
t{ dni1 dni-next dnn1 ?s  }t
t{ dni1 dni-first? ?false }t
t{ dni1 dni-last?  ?false }t

t{ dni1 dni-next dnn2 ?s  }t
t{ dni1 dni-next dnn3 ?s  }t
t{ dni1 dni-first? ?false }t
t{ dni1 dni-last?  ?true  }t

t{ dni1 dni-next ?nil     }t

\ pop tests

t{ dnl1 dnl-pop dnn3 ?s }t

t{ dnl1 dnl-length@   4 ?s   }t

t{ dnl1 dnl-pop dnn2 ?s }t
t{ dnl1 dnl-pop dnn1 ?s }t
t{ dnl1 dnl-pop dnn4 ?s }t
t{ dnl1 dnl-pop dnn5 ?s }t

t{ dnl1 dnl-empty?   ?true }t

t{ dnl1 dnl-pop ?nil }t

t{ dnl1 dnl-first@ ?nil }t
t{ dnl1 dnl-last@  ?nil }t

t{ dnl1 dnl-pop ?nil }t



t{ dnl-new value dnl2 }t

\ insert tests

t{ dnn1 0 dnl2 dnl-insert }t
t{ dnn2 1 dnl2 dnl-insert }t
t{ dnn3 1 dnl2 dnl-insert }t
t{ dnn4 0 dnl2 dnl-insert }t
t{ dnn5 4 dnl2 dnl-insert }t

t{ dnl2 dnl-length@  5 ?s }t

\ get tests

t{ 0 dnl2 dnl-get dnn4 ?s }t
t{ 1 dnl2 dnl-get dnn1 ?s }t
t{ 2 dnl2 dnl-get dnn3 ?s }t
t{ 3 dnl2 dnl-get dnn2 ?s }t
t{ 4 dnl2 dnl-get dnn5 ?s }t

\ index tests

t{ 0  dnl2 dnl-index? ?true  }t
t{ 4  dnl2 dnl-index? ?true  }t
t{ 5  dnl2 dnl-index? ?false }t
t{ 99 dnl2 dnl-index? ?false }t

t{ -1  dnl2 dnl-index? ?true  }t
t{ -5  dnl2 dnl-index? ?true  }t
t{ -6  dnl2 dnl-index? ?false }t
t{ -99 dnl2 dnl-index? ?false }t

t{ dnl2 dnl-reverse }t

t{ 0 dnl2 dnl-get dnn5 ?s }t
t{ 1 dnl2 dnl-get dnn2 ?s }t
t{ 2 dnl2 dnl-get dnn3 ?s }t
t{ 3 dnl2 dnl-get dnn1 ?s }t
t{ 4 dnl2 dnl-get dnn4 ?s }t

\ Iterator test

t{ dnl2 dni-new value dni2 }t

t{ dni2 dni-last dnn4 ?s  }t
t{ dni2 dni-get  dnn4 ?s  }t
t{ dni2 dni-last?  ?true  }t
t{ dni2 dni-first? ?false }t

t{ dni2 dni-prev dnn1 ?s  }t
t{ dni2 dni-last?  ?false }t
t{ dni2 dni-first? ?false }t

t{ dni2 dni-prev dnn3 ?s  }t
t{ dni2 dni-prev dnn2 ?s  }t
t{ dni2 dni-prev dnn5 ?s  }t

t{ dni2 dni-last?  ?false }t
t{ dni2 dni-first? ?true  }t

t{ dni2 dni-prev ?nil     }t
t{ dni2 dni-get  ?nil     }t

t{ dni2 dni-free          }t

\ delete tests

t{ 0 dnl2 dnl-delete dnn5 ?s }t
t{ 2 dnl2 dnl-delete dnn1 ?s }t

t{ 0 dnl2 dnl-get dnn2 ?s }t
t{ 1 dnl2 dnl-get dnn3 ?s }t
t{ 2 dnl2 dnl-get dnn4 ?s }t

t{ 1 dnl2 dnl-delete dnn3 ?s }t
t{ 0 dnl2 dnl-delete dnn2 ?s }t
t{ 0 dnl2 dnl-delete dnn4 ?s }t

t{ dnl2 dnl-length@  ?0 }t
t{ dnl2 dnl-first@ ?nil }t
t{ dnl2 dnl-last@  ?nil }t

\ insert-before & insert-after tests

t{ dnn1 dnl2 dnl-append }t
t{ dnn2 dnn1 dnl2 dnl-insert-before }t
t{ dnn3 dnn1 dnl2 dnl-insert-before }t
t{ dnn4 dnn3 dnl2 dnl-insert-after  }t
t{ dnn5 dnn1 dnl2 dnl-insert-after  }t

t{ dnl2 dnl-length@  5 ?s }t

t{ 0 dnl2 dnl-get dnn2 ?s }t
t{ 1 dnl2 dnl-get dnn3 ?s }t
t{ 2 dnl2 dnl-get dnn4 ?s }t
t{ 3 dnl2 dnl-get dnn1 ?s }t
t{ 4 dnl2 dnl-get dnn5 ?s }t

t{ dnl2 dnl-free }t

t{ dnn1 dnn-free }t
t{ dnn2 dnn-free }t
t{ dnn3 dnn-free }t
t{ dnn4 dnn-free }t
t{ dnn5 dnn-free }t

\ ==============================================================================

