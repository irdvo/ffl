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
\  $Date: 2008-02-21 20:31:19 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/dcl.fs
include ffl/dci.fs
include ffl/rng.fs

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

t{ 2 dcl1 dcl-find  4 swap dcl1 dcl-set }t   \ find item 2 and change it to 4

t{ 2  dcl1 dcl-has?   ?false    }t

t{    dcl1 dcl-clear            }t

t{    dcl1 dcl-length@   ?0     }t
t{    dcl1 dcl-empty?   ?true   }t



t{ dcl-new value dcl2   }t

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

\ Iterator test

t{ 1  dcl1 dcl-append              }t
t{ 2  dcl1 dcl-append              }t
t{ 4  dcl1 dcl-append              }t
t{ 4  dcl1 dcl-append              }t
t{ 5  dcl1 dcl-append              }t
t{ 6  dcl1 dcl-append              }t

t{    dcl1 dcl-empty?   ?false     }t

t{    dcl1 dci-create dci1           }t

t{    dci1 dci-first    ?true 1 ?s }t
t{  4 dci1 dci-move     ?true      }t
t{  4 dci1 dci-move     ?true      }t
t{  4 dci1 dci-move     ?false     }t

t{    dci1 dci-first    ?true 1 ?s }t
t{    dci1 dci-first?   ?true      }t
t{    dci1 dci-last?    ?false     }t
t{    dci1 dci-next     ?true 2 ?s }t
t{    dci1 dci-next     ?true 4 ?s }t
t{  3 dci1 dci-set                 }t
t{    dci1 dci-get      ?true 3 ?s }t
t{    dci1 dci-next     ?true 4 ?s }t
t{    dci1 dci-first?   ?false     }t
t{    dci1 dci-last?    ?false     }t
t{    dci1 dci-next     ?true 5 ?s }t
t{    dci1 dci-next     ?true 6 ?s }t
t{    dci1 dci-first?   ?false     }t
t{    dci1 dci-last?    ?true      }t


t{    dci1 dci-first    ?true 1 ?s }t
t{    dci1 dci-next     ?true 2 ?s }t
t{    dci1 dci-next     ?true 3 ?s }t
t{    dci1 dci-next     ?true 4 ?s }t
t{    dci1 dci-next     ?true 5 ?s }t
t{    dci1 dci-next     ?true 6 ?s }t
t{    dci1 dci-next     ?false     }t

t{    dci1 dci-first    2drop      }t
t{  3 dci1 dci-move     ?true      }t
t{  7 dci1 dci-insert-after        }t
t{    dci1 dci-get      ?true 3 ?s }t
t{    dci1 dci-next     ?true 7 ?s }t

t{    dcl1 dcl-reverse   }t

t{    dci1 dci-first    ?true 6 ?s }t
t{    dci1 dci-next     ?true 5 ?s }t
t{    dci1 dci-next     ?true 4 ?s }t
t{    dci1 dci-next     ?true 7 ?s }t
t{    dci1 dci-next     ?true 3 ?s }t
t{    dci1 dci-next     ?true 2 ?s }t
t{    dci1 dci-next     ?true 1 ?s }t
t{    dci1 dci-next     ?false     }t


t{    dcl1 dcl-clear               }t

t{ 1  dcl1 dcl-append              }t
t{ 2  dcl1 dcl-append              }t
t{ 3  dcl1 dcl-append              }t
t{ 4  dcl1 dcl-append              }t
t{ 5  dcl1 dcl-append              }t


t{    dci1 dci-last    ?true 5 ?s  }t
t{    dci1 dci-last?   ?true       }t
t{    dci1 dci-prev    ?true 4 ?s  }t
t{    dci1 dci-prev    ?true 3 ?s  }t
t{    dci1 dci-prev    ?true 2 ?s  }t
t{    dci1 dci-first?  ?false      }t
t{    dci1 dci-prev    ?true 1 ?s  }t
t{    dci1 dci-first?  ?true       }t
t{    dci1 dci-prev    ?nil        }t

\ dcl-delete test

t{ 2  dcl1 dcl-delete         3 ?s }t
t{ 0  dcl1 dcl-delete         1 ?s }t

t{ 2  dcl1 dcl-delete         5 ?s }t

t{ 0  dcl1 dcl-prepend             }t
t{ 6  dcl1 dcl-append              }t

t{    dci1 dci-first    ?true 0 ?s }t
t{    dci1 dci-next     ?true 2 ?s }t
t{    dci1 dci-next     ?true 4 ?s }t
t{    dci1 dci-next     ?true 6 ?s }t
t{    dci1 dci-next     ?false     }t

t{ 5  dcl1 dcl-remove   ?false     }t
t{ 6  dcl1 dcl-remove   ?true      }t

t{ 0 ' + dcl1 dcl-execute  6 ?s   }t \ sum contents list

: dcl-test-execute?  ( n1 n2 -- n3 flag )
  + dup 6 >
;

t{ 0 ' dcl-test-execute? dcl1 dcl-execute? ?false 6 ?s }t

t{ 6  dcl1 dcl-append              }t

t{ 0 ' dcl-test-execute? dcl1 dcl-execute? ?true 12 ?s }t

t{    dcl1 dcl-clear               }t


t{      dcl-new to    dcl2         }t
t{ dcl2 dci-new value dci2         }t

: dcl-test-compare
  <=> negate
;

t{ ' dcl-test-compare dcl2 dcl-compare! }t

t{ 5  dcl2 dcl-insert-sorted       }t
t{ 1  dcl2 dcl-insert-sorted       }t
t{ 9  dcl2 dcl-insert-sorted       }t
t{ 7  dcl2 dcl-insert-sorted       }t
t{ 7  dcl2 dcl-insert-sorted       }t

t{    dci2 dci-first    ?true 9 ?s }t
t{    dci2 dci-next     ?true 7 ?s }t
t{    dci2 dci-next     ?true 7 ?s }t
t{    dci2 dci-next     ?true 5 ?s }t
t{    dci2 dci-next     ?true 1 ?s }t
t{    dci2 dci-next     ?false     }t

t{ dci2 dci-free }t
t{ dcl2 dcl-free }t

\ dcl-sort test by inserting 1001 random numbers, sorting them and checking the sequence

7898 rng-create dcl-rng

: dcl-rng-insert     ( n -- = Insert n random numbers in the list )
  0 DO
    dcl-rng rng-next-number
    dcl1 dcl-append
  LOOP
;

t{ 1001 dcl-rng-insert }t

: dcl-out-sequence ( n1 n2 flag n3 -- n4 n5 true = Count the number of out of sequence numbers, n1: count n2:previous float n3: number )
  swap IF
    tuck > IF >r 1+ r> THEN
  ELSE
    nip
  THEN
  true
;

t{ ' <=> dcl1 dcl-sort }t

t{ 0 0 false ' dcl-out-sequence dcl1 dcl-execute 2drop 0 ?s }t

t{ dcl1 dcl-reverse }t

t{ 0 0 false ' dcl-out-sequence dcl1 dcl-execute 2drop 1000 ?s }t

t{ dcl1 dcl-clear }t

\ ==============================================================================

