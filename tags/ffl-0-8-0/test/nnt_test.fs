\ ==============================================================================
\
\        nnt_test - the test words for the nnt module in the ffl
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
\  $Date: 2008-01-31 19:27:37 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/nnt.fs
include ffl/nni.fs


.( Testing: nnt, nnn and nni) cr 

t{ nnt-new value nnt1  }t

t{ nnt1 nnt-length@  ?0    }t
t{ nnt1 nnt-empty?   ?true }t

t{ nnn-new value nnn1 }t
t{ nnn-new value nnn2 }t
t{ nnn-new value nnn3 }t
t{ nnn-new value nnn4 }t
t{ nnn-new value nnn5 }t
t{ nnn-new value nnn6 }t

t{ nnt1 nni-new value nni1  }t

\ Root & children tests

t{ nnn1 nni1 nni-append-child }t

t{ nnt1 nnt-length@ 1 ?s       }t
t{ nnt1 nnt-empty?  ?false     }t

t{ nni1 nni-root? ?true        }t
t{ nni1 nni-children? ?false   }t

t{ nnn2 nni1 nni-append-child  }t
t{ nni1 nni-root? ?false       }t

t{ nni1 nni-parent nnn1 ?s     }t
t{ nni1 nni-root? ?true        }t
t{ nni1 nni-children? ?true    }t

t{ nnn3 nni1 nni-prepend-child }t
t{ nni1 nni-parent nnn1 ?s     }t
t{ nni1 nni-children 2 ?s      }t

t{ nni1 nni-child nnn3 ?s      }t

t{ nnn4 nni1 nni-append-child  }t
t{ nnn5 nni1 nni-append-child  }t
t{ nnn6 nni1 nni-append-child  }t

t{ nni1 nni-get  nnn6 ?s       }t
t{ nni1 nni-root nnn1 ?s       }t
t{ nni1 nni-get  nnn1 ?s       }t

t{ nnt1 nnt-length@ 6 ?s       }t

\ Tree words

t{ nnt1 nnt-root@ nnn1 ?s      }t

: nnt-test-count ( n nnn - n+1 )
  drop 1+
;

t{ 0   ' nnt-test-count   nnt1 nnt-execute   6 ?s }t

: nnt-test-has? ( n nnn1 nnn2 - n+1 nn1 f )
  over =
  2>r 1+ 2r>
;

t{ 0 nnn6   ' nnt-test-has?   nnt1 nnt-execute?   ?true drop 5 ?s }t
t{ 0 nnn2   ' nnt-test-has?   nnt1 nnt-execute?   ?true drop 6 ?s }t
t{ 0 nnn3   ' nnt-test-has?   nnt1 nnt-execute?   ?true drop 2 ?s }t
t{ 0 nil    ' nnt-test-has?   nnt1 nnt-execute?  ?false drop 6 ?s }t


\ Sibling words

t{ nnn-new value nnn7 }t
t{ nnn-new value nnn8 }t
t{ nnn-new value nnn9 }t
t{ nnn-new value nnn10 }t

t{ nni1 nni-root nnn1  ?s }t
t{ nni1 nni-child nnn3 ?s }t

t{ nni1 nni-last nnn2 ?s          }t
t{ nnn7 nni1 nni-insert-before    }t
t{ nni1 nni-get  nnn2 ?s          }t
t{ nnn8 nni1 nni-insert-after     }t
t{ nni1 nni-get  nnn2 ?s          }t

t{ nni1  nni-first nnn3 ?s         }t
t{ nnn9  nni1 nni-insert-before    }t
t{ nni1  nni-get   nnn3 ?s         }t
t{ nnn10 nni1 nni-insert-after     }t
t{ nni1  nni-get   nnn3 ?s         }t

t{ nni1  nni-first nnn9 ?s         }t
t{ nni1  nni-first? ?true          }t
t{ nni1  nni-last?  ?false         }t

t{ nni1  nni-next  nnn3 ?s         }t
t{ nni1  nni-first? ?false         }t
t{ nni1  nni-last?  ?false         }t

t{ nni1  nni-next  nnn10 ?s        }t

t{ nni1  nni-next  nnn7 ?s         }t

t{ nni1  nni-next  nnn2 ?s         }t

t{ nni1  nni-next  nnn8 ?s         }t
t{ nni1  nni-first? ?false         }t
t{ nni1  nni-last?  ?true          }t

t{ nni1  nni-next  ?nil            }t

\ Move the iterator to 'end' of the tree

t{ nni1  nni-root  nnn1 ?s         }t
t{ nni1  nni-children 6 ?s         }t

t{ nni1  nni-child nnn9 ?s         }t
t{ nni1  nni-next  nnn3 ?s         }t
t{ nni1  nni-child nnn4 ?s         }t

t{ nni1  nni-first? ?true          }t
t{ nni1  nni-last?  ?true          }t

t{ nni1  nni-child nnn5 ?s         }t
t{ nni1  nni-child nnn6 ?s         }t
t{ nni1  nni-children? ?false      }t


\ Remove the nodes

t{ nni1  nni-remove nnn6 ?s        }t
t{ nni1  nni-children? ?false      }t \ Move to the parent

t{ nni1  nni-remove nnn5 ?s        }t
t{ nni1  nni-children? ?false      }t \ Move to the parent

t{ nni1  nni-remove nnn4 ?s        }t
t{ nni1  nni-children? ?false      }t
t{ nni1  nni-get    nnn3 ?s        }t \ Move to the parent

t{ nni1  nni-remove nnn3 ?s        }t
t{ nni1  nni-get    nnn10 ?s       }t  \ Move to the next sibling

t{ nni1  nni-remove nnn10 ?s       }t
t{ nni1  nni-get    nnn7 ?s        }t  \ Move to the next sibling

t{ nni1  nni-remove nnn7 ?s        }t
t{ nni1  nni-get    nnn2 ?s        }t  \ Move to the next sibling

t{ nni1  nni-remove nnn2 ?s        }t
t{ nni1  nni-get    nnn8 ?s        }t  \ Move to the next sibling

t{ nni1  nni-remove nnn8 ?s        }t
t{ nni1  nni-get    nnn9 ?s        }t  \ Move to the previous sibling

t{ nni1  nni-remove nnn9 ?s        }t
t{ nni1  nni-get    nnn1 ?s        }t  \ Move to the parent

t{ nnt1  nnt-length@ 1 ?s          }t

t{ nni1  nni-remove nnn1 ?s        }t  \ Remove the root

t{ nnt1  nnt-length@ 0 ?s          }t

t{ nnn1 nnn-free }t
t{ nnn2 nnn-free }t
t{ nnn3 nnn-free }t
t{ nnn4 nnn-free }t
t{ nnn5 nnn-free }t
t{ nnn6 nnn-free }t
t{ nnn7 nnn-free }t
t{ nnn8 nnn-free }t
t{ nnn9 nnn-free }t
t{ nnn10 nnn-free }t

t{ nnt1 nnt-free }t
t{ nni1 nni-free }t

\ ==============================================================================

