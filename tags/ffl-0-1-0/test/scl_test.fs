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
\  $Date: 2005-12-24 06:46:48 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/scl.fs
include ffl/sci.fs


." Testing: scl, scn and sci" cr 

t{ scl-create l1  }t

t{    l1 scl-length@   ?0   }t
t{    l1 scl-empty?   ?true   }t

t{ 1  l1 scl-append   }t
t{ 2  l1 scl-append   }t
t{ 3  l1 scl-append   }t

t{ 0  l1 scl-prepend   }t
t{ -1 l1 scl-prepend   }t

t{    l1 scl-length@   5 ?s   }t
t{    l1 scl-empty?   ?false  }t

t{ 3 3 l1 scl-insert   }t

t{ 3  l1 scl-count   2 ?s   }t
t{ 4  l1 scl-count   ?0   }t

t{ -1 l1 scl-find   ?0   }t
t{ 5  l1 scl-find   -1 ?s   }t

t{ 2  l1 scl-has?   ?true   }t
t{ -2 l1 scl-has?   ?false   }t

t{    l1 scl-delete-all   }t

t{    l1 scl-length@   ?0   }t
t{    l1 scl-empty?   ?true   }t

t{ scl-new constant l2   }t

\ Insert-sorted

t{ 9  l2 scl-insert-sorted   }t
t{ 12 l2 scl-insert-sorted   }t
t{ 10 l2 scl-insert-sorted   }t
t{ 5  l2 scl-insert-sorted   }t
t{ 1  l2 scl-insert-sorted   }t
t{ 7  l2 scl-insert-sorted   }t
t{ 7  l2 scl-insert-sorted   }t

t{    l2 scl-length@   7 ?s   }t

t{ 0  l2 scl-get   1 ?s }t
t{ 1  l2 scl-get   5 ?s }t
t{ 2  l2 scl-get   7 ?s }t
t{ 3  l2 scl-get   7 ?s }t
t{ 4  l2 scl-get   9 ?s }t
t{ 5  l2 scl-get   10 ?s }t
t{ 6  l2 scl-get   12 ?s }t

t{    l2 scl-free   }t

\ Iterator test

t{ 1  l1 scl-append              }t
t{ 2  l1 scl-append              }t
t{ 4  l1 scl-append              }t
t{ 4  l1 scl-append              }t
t{ 5  l1 scl-append              }t
t{ 6  l1 scl-append              }t

t{    l1 scl-empty?   ?false     }t

t{    l1 sci-create i1           }t

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

t{    l1 scl-reverse   }t

t{    i1 sci-first    ?true 6 ?s }t
t{    i1 sci-next     ?true 5 ?s }t
t{    i1 sci-next     ?true 4 ?s }t
t{    i1 sci-next     ?true 3 ?s }t
t{    i1 sci-next     ?true 2 ?s }t
t{    i1 sci-next     ?true 1 ?s }t
t{    i1 sci-next     ?false     }t

t{    l1 scl-delete-all          }t

t{ 1  l1 scl-append              }t
t{ 2  l1 scl-append              }t
t{ 3  l1 scl-append              }t
t{ 4  l1 scl-append              }t
t{ 5  l1 scl-append              }t

\ scl-delete test

t{ 2  l1 scl-delete         3 ?s }t
t{ 0  l1 scl-delete         1 ?s }t
t{ 2  l1 scl-delete         5 ?s }t

t{ 0  l1 scl-prepend             }t
t{ 6  l1 scl-append              }t

t{    i1 sci-first    ?true 0 ?s }t
t{    i1 sci-next     ?true 2 ?s }t
t{    i1 sci-next     ?true 4 ?s }t
t{    i1 sci-next     ?true 6 ?s }t
t{    i1 sci-next     ?false     }t

t{ 5  l1 scl-remove   ?false     }t
t{ 6  l1 scl-remove   ?true      }t

t{ ' drop l1 scl-execute         }t

t{    l1 scl-delete-all          }t

\ ==============================================================================

