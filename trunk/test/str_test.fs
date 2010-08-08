\ ==============================================================================
\
\          str_test - the test words for the str module in ffl
\
\             Copyright (C) 2005-2007  Dick van Oudheusden
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
\  $Date: 2008-03-18 19:09:48 $ $Revision: 1.12 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/str.fs


.( Testing: str ) cr 
  
t{ str-create s1                }t
t{ s1 str-length@        0 ?s   }t
t{ s1 str-empty?         ?true  }t

t{ s" hello" s1 str-set         }t
t{ s1 str-length@        5 ?s   }t
t{ s1 str-empty?         ?false }t
t{ s1 str-get         5 ?s drop }t

t{  0 s1 str-index? ?true       }t
t{  4 s1 str-index? ?true       }t
t{  5 s1 str-index? ?false      }t
t{ -5 s1 str-index? ?true       }t
t{ -6 s1 str-index? ?false      }t

t{ s1 str-clear                 }t
t{ s1 str-length@        0 ?s   }t
t{ s1 str-empty?         ?true  }t

t{ s" hello" s1 str-set         }t
t{ s" after" s1 str-append-string }t
t{ s1 str-length@         10 ?s }t
t{ s" before" s1 str-prepend-string }t
t{ s1 str-length@         16 ?s }t
t{ s" beforehelloafter" s1 str-icompare ?0 }t

t{ 0 ' + s1 str-execute 1689 ?s }t

: str-sum? ( n1 n2 ch -- n1 n3 flag = n1: stop n2: sum ch = char of string n3: sum flag: stop? )
  + 2dup <
;

t{ 1474 0 ' str-sum? s1 str-execute? ?true  1575 ?s 1474 ?s }t
t{ 1575 0 ' str-sum? s1 str-execute? ?true  1689 ?s 1575 ?s }t
t{ 1689 0 ' str-sum? s1 str-execute? ?false 1689 ?s 1689 ?s }t

\ str-set-cstring
\ str-get-cstring

t{ str-new constant s2          }t

t{ s" bye" s2 str-set           }t
t{ s2 s1 str^move               }t
t{ s1 str-length@          3 ?s }t

t{ s1 str-clear                 }t
t{ char a s1 str-push-char      }t
t{ char b s1 str-push-char      }t
t{ char c s1 str-push-char      }t
t{ s1 str-length@          3 ?s }t
t{ s" abc" s1 str-icompare   ?0 }t

t{ s1 str-pop-char   char  c ?s }t
t{ s1 str-pop-char   char  b ?s }t

t{ char d s1 str-push-char      }t

t{ s1 str-pop-char   char  d ?s }t
t{ s1 str-pop-char   char  a ?s }t
t{ s1 str-length@          0 ?s }t


t{ s" Hello" s1 str-set                }t
t{ char h 0 s1 str-set-char            }t
t{ 0 s1 str-get-char  char h ?s        }t
t{ -1 s1 str-get-char char o ?s        }t
t{ char O -1 s1 str-set-char           }t
t{ -1 s1 str-get-char char O ?s        }t

t{ s" Hello" s1 str-set                }t
t{ chr.sp 3 s1 str-append-chars        }t
t{ char - 4 s1 str-prepend-chars       }t

t{ s" ----Hello   " s1 str-icompare ?0 }t

\ str-extra@
\ str-extra!
\ str+extra@
\ str+extra!


t{ s" Hello" s1 str-set             }t
t{ s" io" 4 s1 str-insert-string    }t
t{ s" Ha" 0 s1 str-insert-string    }t
t{ s" HaHellioo" s1 str-ccompare ?0 }t


t{ s" Hello" s1 str-set             }t
t{ char ? 2 3 s1 str-insert-chars   }t
t{ s" Hel??lo" s1 str-ccompare   ?0 }t



t{ 6 -6 s1 str-get-substring s2 str-set  }t
t{ s" el??lo" s2 str-ccompare ?0      }t


t{ s1 str-get s" Hel??lo" ?str       }t

t{ 3 0 s1 str-delete                 }t
t{ s" ??lo" s1 str-ccompare ?0       }t
t{ 2 2 s1 str-delete                 }t
t{ s" ??" s1 str-ccompare ?0         }t

t{ s" Hello" s1 str-set              }t
t{ char ! s1 str-enqueue-char        }t
t{ s" !Hello" s1 str-ccompare ?0     }t 
t{ s1 str-dequeue-char char o ?s     }t

t{ char + 0 s1 str-insert-char       }t
t{ char - 5 s1 str-insert-char       }t
t{ char ) 6 s1 str-insert-char       }t
t{ 3 s1 str-delete-char              }t
t{ s" +!Hl-)l" s1 str-ccompare ?0    }t

\ capatilize
t{ s" 1. title" s1 str-set           }t
t{ s1 str-capatilize                 }t
t{ s" 1. Title" s1 str-ccompare ?0   }t

\ cap words
t{ s" this is a test string" s1 str-set         }t
t{ s1 str-cap-words                             }t
t{ s" This Is A Test String" s1 str-ccompare ?0 }t

\ center, ljust, rjust
t{ s" Hello" s1 str-set                 }t
t{ 4 s1 str-align-left                  }t
t{ s" Hello" s1 str-ccompare ?0         }t
t{ 7 s1 str-align-left                  }t
t{ s" Hello  " s1 str-ccompare ?0       }t

t{ s" Hello" s1 str-set                 }t
t{ 2 s1 str-align-right                 }t
t{ s" Hello" s1 str-ccompare ?0         }t
t{ 8 s1 str-align-right                 }t
t{ s"    Hello" s1 str-ccompare ?0      }t

t{ s" Hello" s1 str-set                 }t
t{ 1 s1 str-center                      }t
t{ s" Hello" s1 str-ccompare ?0         }t
t{ 10 s1 str-center                     }t
t{ s"   Hello   " s1 str-ccompare ?0    }t

\ strip
t{ s"    Hello   " s1 str-set           }t
t{ s1 str-strip-leading                 }t
t{ s" Hello   " s1 str-ccompare  ?0     }t
t{ s"    Hello   " s1 str-set           }t
t{ s1 str-strip-trailing                }t
t{ s"    Hello" s1 str-ccompare ?0      }t
t{ s"    Hello   " s1 str-set           }t
t{ s1 str-strip                         }t
t{ s" Hello" s1 str-ccompare ?0         }t

\ upper and lower
t{ s" hello" s1 str-set                 }t

t{ s1 str-upper                         }t
t{ s" HELLO" s1 str-ccompare ?0         }t

t{ s1 str-lower                         }t
t{ s" hello" s1 str-ccompare ?0         }t

\ expand tabs
t{ s2 str-clear                         }t
t{ s" Hello" s2 str-append-string       }t
t{ chr.ht s2 str-push-char              }t
t{ s" Bye" s2 str-append-string         }t
t{ chr.ht s2 str-push-char              }t

t{ s2 s1 str^move                       }t
t{ 0 s1 str-expand-tabs                 }t
t{ s" HelloBye" s1 str-ccompare ?0      }t

t{ s2 s1 str^move                       }t
t{ 1 s1  str-expand-tabs                }t
t{ s" Hello Bye " s1 str-ccompare ?0    }t

t{ s2 s1 str^move                       }t
t{ 3 s1  str-expand-tabs                }t
t{ s" Hello   Bye   " s1 str-ccompare ?0 }t

t{ s" Hello" s1 str-set                }t
t{ s" hello" s2 str-set                }t
t{ s1 s2 str^icompare ?0               }t
t{ s1 s2 str^ccompare 0< ?true         }t
t{ s2 s1 str^ccompare 0> ?true         }t
t{ s" hallo" s1 str-set                }t
t{ s" hello" s2 str-set                }t
t{ s1 s2 str^icompare 0< ?true         }t
t{ s2 s1 str^icompare 0> ?true         }t
t{ s1 s2 str^ccompare 0< ?true         }t
t{ s2 s1 str^ccompare 0> ?true         }t

\ count
t{ s" This is a longish string" s1 str-set }t
t{ s" this" s1 str-count  0 ?s         }t
t{ s" This" s1 str-count  1 ?s         }t
t{ s" is"   s1 str-count  3 ?s         }t
t{ s" i"    s1 str-count  4 ?s         }t

\ find
t{ s" is" 0  s1 str-find   2 ?s        }t
t{ s" is" 3  s1 str-find   5 ?s        }t
t{ s" is" 6  s1 str-find  14 ?s        }t
t{ s" is" 15 s1 str-find  -1 ?s        }t

\ replace
t{ s" g" s" ng" s1 str-replace         }t
t{ s" This is a logish strig" s1 str-ccompare ?0 }t
t{ s" iii" s" i" s1 str-replace        }t
t{ s" Thiiis iiis a logiiish striiig" s1 str-ccompare ?0 }t

\ split
: str-test-split1 s" this is a test of the column splitter" ;
: str-test-split2 s" thisisatestofthecolumnsplitter" ;

t{ str-test-split1 10 str+columns 4 ?s
   s" this is a"  ?str
   s" test of"    ?str
   s" the column" ?str
   s" splitter"   ?str }t

t{ s" 1234567890" 10 str+columns 1 ?s  s" 1234567890" ?str }t

t{ s" 1234567890    " 10 str+columns 1 ?s  s" 1234567890" ?str }t

t{ s"     1234567890" 10 str+columns 1 ?s  s" 1234567890" ?str }t

t{ str-test-split2 10 str+columns 3 ?s
   s" thisisates" ?str
   s" tofthecolu" ?str 
   s" mnsplitter" ?str }t

t{ s2 str-free }t

\ ==============================================================================
