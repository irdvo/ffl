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
\  $Date: 2009-05-25 19:13:34 $ $Revision: 1.6 $
\
\ ==============================================================================

include ffl/zif.fs
include ffl/tst.fs
include ffl/dtm.fs


[DEFINED] zif.version [IF]


.( Testing: gzf gzi zif) cr 


t{ zif-create zif1 }t

t{ s" unknown.gz" zif1 zif-open-file ?true }t 

t{ s" stored.gz"  zif1 zif-open-file ?0 }t

t{ zif1 zif-read-header ?0 }t

t{ zif1 zif-gzf@ value gzf1 }t

t{ gzf1 gzf-text@ . }t

t{ gzf1 gzf-os@ . }t

t{ gzf1 gzf-mtime@ . }t

t{ gzf1 gzf-name@ type }t

t{ gzf1 gzf-comment@ type }t

\ t{ pad 80 zif1 zif-read-file . pad swap type }t

t{ zif1 zif-close-file ?0 }t

t{ zif1 zif-(free) }t

0 [IF]


t{ pad 80 gzf1 gzf-read-file ?0 . }t

gzf1 gzf-dump

dtm-create gzfd

gzf1 gzf-mtime@ 0  dtm.unix-epoch  gzfd  dtm-set-with-seconds 

.( Mtime: ) gzfd dtm-get . . . . . . . cr

t{ gzf1 gzf-close-file ?0 }t

[THEN]

t{ zif-new value zif2 }t

t{ s" fixed.gz"  zif2 zif-open-file ?0 }t
t{ s" gzipped.gz"  zif2 zif-open-file ?0 }t
\ t{ s" extra.gz"  zif2 zif-open-file ?0 }t

t{ zif2 zif-read-header ?0 }t

t{ zif2 zif-gzf@ value gzf2 }t

t{ gzf2 gzf-text@ . cr }t

t{ gzf2 gzf-os@ . cr }t

t{ gzf2 gzf-mtime@ . cr }t

t{ gzf2 gzf-name@ type cr }t

t{ gzf2 gzf-comment@ type cr }t

: zif-test-file  ( -- ior )
  s" temp.txt" r/w create-file throw
  BEGIN
    pad 80 zif2 zif-read-file
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
;

t{ zif-test-file ?0 }t

t{ zif2 zif-close-file ?0 }t

t{ zif2 zif-free }t

[THEN]

\ ==============================================================================
