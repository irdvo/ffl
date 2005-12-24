\ ==============================================================================
\
\          chr_test - the test words for the chr module in ffl
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
\  $Date: 2005-12-24 22:03:07 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/str.fs


." Testing: str" cr 
  
t{ str-create s1                }t
t{ s1 str-length@        0 ?s   }t
t{ s1 str-empty?         ?true  }t

t{ s" hallo" s1 str-set         }t
t{ s1 str-length@        5 ?s   }t
t{ s1 str-empty?         ?false }t
t{ s1 str-get         5 ?s drop }t

t{ s1 str-clear                 }t
t{ s1 str-length@        0 ?s   }t
t{ s1 str-empty?         ?true  }t

t{ s" hallo" s1 str-set         }t
t{ s" after" s1 str-append      }t
t{ s1 str-length@         10 ?s }t
t{ s" before" s1 str-prepend    }t
t{ s1 str-length@         16 ?s }t

t{ 0 ' + s1 str-execute 1685 ?s }t  \ ToDo: check

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

t{ s1 str-pop-char   char  c ?s }t
t{ s1 str-pop-char   char  b ?s }t

t{ char d s1 str-push-char      }t

t{ s1 str-pop-char   char  d ?s }t
t{ s1 str-pop-char   char  a ?s }t
t{ s1 str-length@          0 ?s }t

t{ s2 str-free                  }t

\ ==============================================================================
