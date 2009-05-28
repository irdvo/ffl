\ ==============================================================================
\
\        gzf_test - the test words for the gzf module in the ffl
\
\               Copyright (C) 2008  Dick van Oudheusden
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
\  $Date: 2009-05-28 17:40:17 $ $Revision: 1.7 $
\
\ ==============================================================================

include ffl/zif.fs
include ffl/tst.fs
include ffl/dtm.fs


[DEFINED] zif.version [IF]


.( Testing: gzf gzi zif) cr 


: zif-test-file  ( c-addr u zif -- ior )
  >r
  r/w create-file throw
  BEGIN
    pad 80 r@ zif-read-file
    dup 0= IF
      over 0>
    ELSE
      false
    THEN
  WHILE
    drop
    over pad -rot write-file throw
  REPEAT
  nip
  swap close-file throw
  rdrop
;


t{ zif-create zif1 }t

t{ s" unknown.gz" zif1 zif-open-file ?true }t 



t{ s" stored.gz"  zif1 zif-open-file ?0 }t

t{ zif1 zif-read-header ?0 }t

t{ zif1 zif-gzf@ value gzf1 }t

t{ gzf1 gzf-text@ ?0 }t

t{ gzf1 gzf-os@ gzf.unix ?s }t

t{ s" stored" zif1 zif-test-file ?0 }t

t{ zif1 zif-close-file ?0 }t

t{ zif1 zif-(free) }t



t{ zif-new value zif2 }t

t{ s" fixed.gz"  zif2 zif-open-file ?0 }t

t{ zif2 zif-read-header ?0 }t

t{ s" fixed" zif2 zif-test-file ?0 }t

t{ zif2 zif-close-file ?0 }t

t{ zif2 zif-free }t



t{ zif-new value zif3 }t

t{ s" gzipped.gz" zif3 zif-open-file ?0 }t

t{ zif3 zif-read-header ?0 }t

t{ zif3 zif-gzf@ value gzf3 }t

t{ gzf3 gzf-name@ s" COPYING" ?str }t

t{ s" gzipped" zif3 zif-test-file ?0 }t

t{ zif3 zif-close-file ?0 }t

t{ zif3 zif-free }t

[THEN]

\ ==============================================================================
