\ ==============================================================================
\
\        scl_test - the test words for the scl module in the ffl
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
\  $Date: 2008-03-02 15:03:03 $ $Revision: 1.8 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/scl.fs
include ffl/sci.fs
include ffl/rng.fs


.( Testing: scl, scn and sci) cr

t{ scl-create scl1  }t

t{    scl1 scl-length@   ?0   }t
t{    scl1 scl-empty?   ?true   }t

t{ 1  scl1 scl-append   }t
t{ 2  scl1 scl-append   }t
t{ 3  scl1 scl-append   }t

t{ 0  scl1 scl-prepend   }t
t{ -1 scl1 scl-prepend   }t

t{    scl1 scl-length@   5 ?s   }t
t{    scl1 scl-empty?   ?false  }t

t{ 3 3 scl1 scl-insert   }t

t{ 3  scl1 scl-count   2 ?s }t
t{ 4  scl1 scl-count   ?0   }t

t{ -1 scl1 scl-find   ?0    }t
t{ 5  scl1 scl-find   -1 ?s }t
t{ 3  scl1 scl-find    3 ?s }t

t{ 2  scl1 scl-has?   ?true   }t
t{ -2 scl1 scl-has?   ?false   }t

t{    scl1 scl-clear          }t

t{    scl1 scl-length@   ?0   }t
t{    scl1 scl-empty?   ?true   }t

t{ scl-new constant scl2   }t

\ Insert-sorted

t{ 9  scl2 scl-insert-sorted   }t
t{ 12 scl2 scl-insert-sorted   }t
t{ 10 scl2 scl-insert-sorted   }t
t{ 5  scl2 scl-insert-sorted   }t
t{ 1  scl2 scl-insert-sorted   }t
t{ 7  scl2 scl-insert-sorted   }t
t{ 7  scl2 scl-insert-sorted   }t

t{    scl2 scl-length@   7 ?s   }t

t{ 0  scl2 scl-get   1 ?s }t
t{ 1  scl2 scl-get   5 ?s }t
t{ 2  scl2 scl-get   7 ?s }t
t{ 3  scl2 scl-get   7 ?s }t
t{ 4  scl2 scl-get   9 ?s }t
t{ 5  scl2 scl-get   10 ?s }t
t{ 6  scl2 scl-get   12 ?s }t

t{    scl2 scl-free   }t


t{ 1  scl1 scl-append              }t
t{ 2  scl1 scl-append              }t
t{ 4  scl1 scl-append              }t
t{ 4  scl1 scl-append              }t
t{ 5  scl1 scl-append              }t
t{ 6  scl1 scl-append              }t

\ set test

t{ 3  scl1 scl-get 4 ?s            }t
t{ 7 3 scl1 scl-set                }t
t{ 3  scl1 scl-get 7 ?s            }t
t{ 4 3 scl1 scl-set                }t

t{    scl1 scl-empty?   ?false     }t

\ Iterator test

t{    scl1 sci-create sci1           }t

t{    sci1 sci-first    ?true 1 ?s }t
t{    sci1 sci-first?   ?true      }t
t{    sci1 sci-last?    ?false     }t
t{    sci1 sci-next     ?true 2 ?s }t
t{    sci1 sci-next     ?true 4 ?s }t
t{  3 sci1 sci-set                 }t
t{    sci1 sci-get      ?true 3 ?s }t
t{    sci1 sci-next     ?true 4 ?s }t
t{    sci1 sci-first?   ?false     }t
t{    sci1 sci-last?    ?false     }t
t{    sci1 sci-next     ?true 5 ?s }t
t{    sci1 sci-next     ?true 6 ?s }t
t{    sci1 sci-first?   ?false     }t
t{    sci1 sci-last?    ?true      }t


t{    sci1 sci-first    ?true 1 ?s }t
t{    sci1 sci-next     ?true 2 ?s }t
t{    sci1 sci-next     ?true 3 ?s }t
t{    sci1 sci-next     ?true 4 ?s }t
t{    sci1 sci-next     ?true 5 ?s }t
t{    sci1 sci-next     ?true 6 ?s }t
t{    sci1 sci-next     ?false     }t

t{    scl1 scl-reverse   }t

t{    sci1 sci-first    ?true 6 ?s }t
t{    sci1 sci-next     ?true 5 ?s }t
t{    sci1 sci-next     ?true 4 ?s }t
t{    sci1 sci-next     ?true 3 ?s }t
t{    sci1 sci-next     ?true 2 ?s }t
t{    sci1 sci-next     ?true 1 ?s }t
t{    sci1 sci-next     ?false     }t

t{    sci1 sci-first    ?true 6 ?s }t
t{ 2  sci1 sci-move     ?true      }t
t{    sci1 sci-get      ?true 2 ?s }t
t{ 7  sci1 sci-insert-after        }t
t{    sci1 sci-get      ?true 2 ?s }t
t{    sci1 sci-next     ?true 7 ?s }t
t{    sci1 sci-next     ?true 1 ?s }t
t{    sci1 sci-next     ?false     }t

t{    scl1 scl-clear               }t


t{ 1  scl1 scl-append              }t
t{ 2  scl1 scl-append              }t
t{ 3  scl1 scl-append              }t
t{ 4  scl1 scl-append              }t
t{ 5  scl1 scl-append              }t

\ scl-delete test

t{ 2  scl1 scl-delete         3 ?s }t
t{ 0  scl1 scl-delete         1 ?s }t
t{ 2  scl1 scl-delete         5 ?s }t

t{ 0  scl1 scl-prepend             }t
t{ 6  scl1 scl-append              }t

t{    sci1 sci-first    ?true 0 ?s }t
t{    sci1 sci-next     ?true 2 ?s }t
t{    sci1 sci-next     ?true 4 ?s }t
t{    sci1 sci-next     ?true 6 ?s }t
t{    sci1 sci-next     ?false     }t

t{ 5  scl1 scl-remove   ?false     }t
t{ 6  scl1 scl-remove   ?true      }t

t{ 0 ' + scl1 scl-execute  6 ?s   }t \ sum contents list

: scl-sum?  ( n1 n2 n3 -- n1 n4 flag = n1: level n2:sum n3:value n4:sum flag:done )
  + 2dup <
;

t{ 1 0 ' scl-sum? scl1 scl-execute? ?true  2 ?s 1 ?s }t
t{ 4 0 ' scl-sum? scl1 scl-execute? ?true  6 ?s 4 ?s }t
t{ 8 0 ' scl-sum? scl1 scl-execute? ?false 6 ?s 8 ?s }t
 
t{    scl1 scl-clear              }t

: scl-test-compare
  - negate
;

t{ ' scl-test-compare scl1 scl-compare! }t

t{ 5  scl1 scl-insert-sorted       }t
t{ 1  scl1 scl-insert-sorted       }t
t{ 9  scl1 scl-insert-sorted       }t
t{ 7  scl1 scl-insert-sorted       }t
t{ 7  scl1 scl-insert-sorted       }t

t{  scl1 sci-new constant sci2     }t

t{  sci2 sci-first    ?true 9 ?s }t
t{  sci2 sci-next     ?true 7 ?s }t
t{  sci2 sci-next     ?true 7 ?s }t
t{  sci2 sci-next     ?true 5 ?s }t
t{  sci2 sci-next     ?true 1 ?s }t
t{  sci2 sci-next     ?false     }t

t{  sci2 sci-free                }t

t{  scl1 scl-clear                 }t

\ scl-sort test by inserting 1001 random numbers, sorting them and checking the sequence

4598 rng-create scl-rng

: scl-rng-insert     ( n -- = Insert n random numbers in the list )
  0 DO
    scl-rng rng-next-number
    scl1 scl-append
  LOOP
;

t{ 1001 scl-rng-insert }t

: scl-out-sequence ( n1 n2 flag n3 -- n4 n5 true = Count the number of out of sequence numbers, n1: count n2:previous float n3: number )
  swap IF
    tuck > IF >r 1+ r> THEN
  ELSE
    nip
  THEN
  true
;

t{ ' <=> scl1 scl-sort }t

t{ 0 0 false ' scl-out-sequence scl1 scl-execute 2drop 0 ?s }t

t{  scl1 scl-clear }t

\ ==============================================================================

