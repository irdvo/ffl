\ ==============================================================================
\
\        dcl_test - the test words for the dcl module in the ffl
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
\  $Date: 2007-01-01 18:14:16 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/dcl.fs
\ include ffl/sci.fs


.( Testing: dcl, dcn and dci) cr 

t{ dcl-create dcl1  }t

t{    dcl1 dcl-length@   ?0   }t
t{    dcl1 dcl-empty?   ?true   }t

t{ 1  dcl1 dcl-append   }t
t{ 2  dcl1 dcl-append   }t
t{ 3  dcl1 dcl-append   }t

t{ 0  dcl1 dcl-prepend   }t
t{ -1 dcl1 dcl-prepend   }t

t{    dcl1 dcl-length@   5 ?s   }t
t{    dcl1 dcl-empty?   ?false  }t

t{ 3 3 dcl1 dcl-insert   }t

t{ 3  dcl1 dcl-count   2 ?s   }t
t{ 4  dcl1 dcl-count   ?0   }t

t{ -1 dcl1 dcl-find   ?0   }t
t{ 5  dcl1 dcl-find   -1 ?s   }t

t{ 2  dcl1 dcl-has?   ?true   }t
t{ -2 dcl1 dcl-has?   ?false   }t

t{    dcl1 dcl-delete-all   }t

t{    dcl1 dcl-length@   ?0   }t
t{    dcl1 dcl-empty?   ?true   }t



t{ dcl-new constant dcl2   }t

\ Insert-sorted

t{ 9  dcl2 dcl-insert-sorted   }t
t{ 12 dcl2 dcl-insert-sorted   }t
t{ 10 dcl2 dcl-insert-sorted   }t
t{ 5  dcl2 dcl-insert-sorted   }t
t{ 1  dcl2 dcl-insert-sorted   }t
t{ 7  dcl2 dcl-insert-sorted   }t
t{ 7  dcl2 dcl-insert-sorted   }t

t{    dcl2 dcl-length@   7 ?s   }t

t{ 0  dcl2 dcl-get   1 ?s }t
t{ 1  dcl2 dcl-get   5 ?s }t
t{ 2  dcl2 dcl-get   7 ?s }t
t{ 3  dcl2 dcl-get   7 ?s }t
t{ 4  dcl2 dcl-get   9 ?s }t
t{ 5  dcl2 dcl-get   10 ?s }t
t{ 6  dcl2 dcl-get   12 ?s }t

t{    dcl2 dcl-free   }t

0 [IF]
\ Iterator test

t{ 1  dcl1 dcl-append              }t
t{ 2  dcl1 dcl-append              }t
t{ 4  dcl1 dcl-append              }t
t{ 4  dcl1 dcl-append              }t
t{ 5  dcl1 dcl-append              }t
t{ 6  dcl1 dcl-append              }t

t{    dcl1 dcl-empty?   ?false     }t

t{    dcl1 sci-create i1           }t

t{    i1 sci-first    ?true 1 ?s }t
t{    i1 sci-first?   ?true      }t
t{    i1 sci-last?    ?false     }t
t{    i1 sci-next     ?true 2 ?s }t
t{    i1 sci-next     ?true 4 ?s }t
t{  3 i1 sci-set                 }t
t{    i1 sci-get      ?true 3 ?s }t
t{    i1 sci-next     ?true 4 ?s }t
t{    i1 sci-first?   ?false     }t
t{    i1 sci-last?    ?false     }t
t{    i1 sci-next     ?true 5 ?s }t
t{    i1 sci-next     ?true 6 ?s }t
t{    i1 sci-first?   ?false     }t
t{    i1 sci-last?    ?true      }t


t{    i1 sci-first    ?true 1 ?s }t
t{    i1 sci-next     ?true 2 ?s }t
t{    i1 sci-next     ?true 3 ?s }t
t{    i1 sci-next     ?true 4 ?s }t
t{    i1 sci-next     ?true 5 ?s }t
t{    i1 sci-next     ?true 6 ?s }t
t{    i1 sci-next     ?false     }t

t{    dcl1 dcl-reverse   }t

t{    i1 sci-first    ?true 6 ?s }t
t{    i1 sci-next     ?true 5 ?s }t
t{    i1 sci-next     ?true 4 ?s }t
t{    i1 sci-next     ?true 3 ?s }t
t{    i1 sci-next     ?true 2 ?s }t
t{    i1 sci-next     ?true 1 ?s }t
t{    i1 sci-next     ?false     }t

t{    dcl1 dcl-delete-all          }t

t{ 1  dcl1 dcl-append              }t
t{ 2  dcl1 dcl-append              }t
t{ 3  dcl1 dcl-append              }t
t{ 4  dcl1 dcl-append              }t
t{ 5  dcl1 dcl-append              }t

\ dcl-delete test

t{ 2  dcl1 dcl-delete         3 ?s }t
t{ 0  dcl1 dcl-delete         1 ?s }t
t{ 2  dcl1 dcl-delete         5 ?s }t

t{ 0  dcl1 dcl-prepend             }t
t{ 6  dcl1 dcl-append              }t

t{    i1 sci-first    ?true 0 ?s }t
t{    i1 sci-next     ?true 2 ?s }t
t{    i1 sci-next     ?true 4 ?s }t
t{    i1 sci-next     ?true 6 ?s }t
t{    i1 sci-next     ?false     }t

t{ 5  dcl1 dcl-remove   ?false     }t
t{ 6  dcl1 dcl-remove   ?true      }t

t{ 0 ' + dcl1 dcl-execute  6 ?s   }t \ sum contents list

t{    dcl1 dcl-delete-all          }t

: dcl-test-compare
  <=> negate
;

t{ ' dcl-test-compare dcl1 dcl-compare! }t

t{ 5  dcl1 dcl-insert-sorted       }t
t{ 1  dcl1 dcl-insert-sorted       }t
t{ 9  dcl1 dcl-insert-sorted       }t
t{ 7  dcl1 dcl-insert-sorted       }t
t{ 7  dcl1 dcl-insert-sorted       }t

t{    i1 sci-first    ?true 9 ?s }t
t{    i1 sci-next     ?true 7 ?s }t
t{    i1 sci-next     ?true 7 ?s }t
t{    i1 sci-next     ?true 5 ?s }t
t{    i1 sci-next     ?true 1 ?s }t
t{    i1 sci-next     ?false     }t

t{    dcl1 dcl-delete-all          }t
[THEN]

\ ==============================================================================

