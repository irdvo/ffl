\ ==============================================================================
\
\        gzf_test - the test words for the gzf module in the ffl
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
\  $Date: 2008-05-19 05:44:00 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/gzf.fs
include ffl/tst.fs
include ffl/dtm.fs


[DEFINED] gzf.version [IF]


.( Testing: gzf) cr 

t{ gzf-create gzf1 }t

t{ s" unknown.gz" gzf1 gzf-open-file ?true }t  \ actually <>0

t{ s" gzf.gz"     gzf1 gzf-open-file ?0 }t

t{ gzf1 gzf-read-header ?0 }t

gzf1 gzf-dump

dtm-create gzfd

gzf1 gzf-mtime@ 0  dtm.unix-epoch  gzfd  dtm-set-with-seconds 

.( Mtime: ) gzfd dtm-get . . . . . . . cr

t{ gzf1 gzf-close-file ?0 }t

t{ s" temp.gz" gzf1 gzf-create-file ?0 }t

t{ s" temp.txt" gzf1 gzf-name! }t
t{ s" Temporary test file for forth zlib library" gzf1 gzf-comment! }t
t{ gzf.unix gzf1 gzf-os! }t

t{ gzf1 gzf-write-header ?0 }t

t{ gzf1 gzf-close-file ?0 }t

t{ gzf1 gzf-(free) }t

[THEN]

\ ==============================================================================
