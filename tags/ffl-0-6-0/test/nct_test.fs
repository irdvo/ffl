\ ==============================================================================
\
\        nct_test - the test words for the nct module in the ffl
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
\  $Date: 2008-02-20 19:30:06 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/nct.fs
include ffl/nci.fs


.( Testing: nct, ncn and nci) cr 


t{ nct-create nct1  }t

t{    nct1 nct-length@  ?0    }t
t{    nct1 nct-empty?   ?true }t


t{ nct1 nci-new value nci1  }t

\ Root & children tests

t{ 1 nci1 nci-append-child     }t

t{ nct1 nct-length@ 1 ?s       }t
t{ nct1 nct-empty?  ?false     }t

t{ nci1 nci-root?     ?true    }t
t{ nci1 nci-children? ?false   }t

t{ 2 nci1 nci-append-child     }t
t{ nci1 nci-root? ?false       }t

t{ nci1 nci-parent ?true 1 ?s  }t
t{ nci1 nci-root?  ?true       }t
t{ nci1 nci-children? ?true    }t

t{ 3 nci1 nci-prepend-child    }t
t{ nci1 nci-parent ?true 1 ?s  }t
t{ nci1 nci-children 2 ?s      }t

t{ nci1 nci-child ?true 3 ?s   }t

t{ 4 nci1 nci-append-child     }t
t{ 5 nci1 nci-append-child     }t
t{ 6 nci1 nci-append-child     }t

t{ nci1 nci-get  ?true 6 ?s    }t
t{ nci1 nci-root ?true 1 ?s    }t
t{ nci1 nci-get  ?true 1 ?s    }t

t{ nct1 nct-length@ 6 ?s       }t


\ Tree words

: nct-test-sum ( n m - n+m )
  +
;

t{ 0   ' nct-test-sum   nct1 nct-execute   21 ?s }t

: nct-test-has? ( n m p - n+1 m f )
  over =
  2>r 1+ 2r>
;

t{ 0 6   ' nct-test-has?   nct1 nct-execute?   ?true drop 5 ?s }t
t{ 0 2   ' nct-test-has?   nct1 nct-execute?   ?true drop 6 ?s }t
t{ 0 3   ' nct-test-has?   nct1 nct-execute?   ?true drop 2 ?s }t
t{ 0 7   ' nct-test-has?   nct1 nct-execute?  ?false drop 6 ?s }t

t{  6 nct1 nct-has? ?true  }t
t{  2 nct1 nct-has? ?true  }t
t{  3 nct1 nct-has? ?true  }t
t{ -1 nct1 nct-has? ?false }t
t{  9 nct1 nct-has? ?false }t

\ Sibling words

t{ nci1 nci-root  ?true 1 ?s }t
t{ nci1 nci-child ?true 3 ?s }t

t{ nci1 nci-last  ?true 2 ?s   }t
t{ 7 nci1 nci-insert-before    }t
t{ nci1 nci-get  ?true 2 ?s    }t
t{ 8 nci1 nci-insert-after     }t
t{ nci1 nci-get  ?true 2 ?s    }t

t{ nci1  nci-first ?true 3 ?s  }t
t{ 9  nci1 nci-insert-before   }t
t{ nci1  nci-get   ?true 3 ?s  }t
t{ 10 nci1 nci-insert-after    }t
t{ nci1  nci-get   ?true 3 ?s  }t

t{ nci1  nci-first ?true 9 ?s  }t
t{ nci1  nci-first? ?true      }t
t{ nci1  nci-last?  ?false     }t

t{ nci1  nci-next  ?true 3 ?s  }t
t{ nci1  nci-first? ?false     }t
t{ nci1  nci-last?  ?false     }t

t{ nci1  nci-next  ?true 10 ?s }t

t{ nci1  nci-next  ?true 7 ?s  }t

t{ nci1  nci-next  ?true 2 ?s  }t

t{ nci1  nci-next  ?true 8 ?s  }t
t{ nci1  nci-first? ?false     }t
t{ nci1  nci-last?  ?true      }t

t{ nci1  nci-next  ?false      }t


\ Move the iterator to 'end' of the tree

t{ nci1  nci-root  ?true 1 ?s  }t
t{ nci1  nci-children 6 ?s     }t

t{ nci1  nci-child ?true 9 ?s  }t
t{ nci1  nci-next  ?true 3 ?s  }t
t{ nci1  nci-child ?true 4 ?s  }t

t{ nci1  nci-first? ?true      }t
t{ nci1  nci-last?  ?true      }t

t{ nci1  nci-child ?true 5 ?s  }t
t{ nci1  nci-child ?true 6 ?s  }t
t{ nci1  nci-children? ?false  }t


\ Remove the nodes

t{ nci1  nci-remove ?true 6 ?s   }t
t{ nci1  nci-children? ?false    }t \ Move to the parent

t{ nci1  nci-remove ?true 5 ?s   }t
t{ nci1  nci-children? ?false    }t \ Move to the parent

t{ nci1  nci-remove ?true 4 ?s   }t
t{ nci1  nci-children? ?false    }t
t{ nci1  nci-get    ?true 3 ?s   }t \ Move to the parent

t{ nci1  nci-remove ?true 3 ?s   }t
t{ nci1  nci-get    ?true 10 ?s  }t  \ Move to the next sibling

t{ nci1  nci-remove ?true 10 ?s  }t
t{ nci1  nci-get    ?true 7 ?s   }t  \ Move to the next sibling

t{ nci1  nci-remove ?true 7 ?s   }t
t{ nci1  nci-get    ?true 2 ?s   }t  \ Move to the next sibling

t{ nci1  nci-remove ?true 2 ?s   }t
t{ nci1  nci-get    ?true 8 ?s   }t  \ Move to the next sibling

t{ nci1  nci-remove ?true 8 ?s   }t
t{ nci1  nci-get    ?true 9 ?s   }t  \ Move to the previous sibling

t{ nci1  nci-remove ?true 9 ?s   }t
t{ nci1  nci-get    ?true 1 ?s   }t  \ Move to the parent

t{ nct1  nct-length@ 1 ?s        }t

t{ nci1  nci-remove ?true 1 ?s   }t  \ Remove the root

t{ nct1  nct-length@ 0 ?s        }t


t{ nci1 nci-free }t


\ Tree on the heap

t{ nct-new value nct2  }t

t{ nct2 nci-create nci2 }t

: nct-build-tree
  5 1 DO
    I nci2 nci-append-child
    
    I dup 1 ?DO
      dup nci2 nci-insert-before
    LOOP
    drop
  LOOP
;


t{ nct-build-tree         }t

t{ nct2 nct-length@ 10 ?s }t

t{ nct2 nct-clear         }t

t{ nct-build-tree         }t

t{ 0 ' nct-test-sum nct2 nct-execute 30 ?s }t


t{ nct2 nct-free          }t


\ ==============================================================================

