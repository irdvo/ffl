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
\  $Date: 2008-05-11 05:52:05 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/gzf.fs
include ffl/tst.fs


[DEFINED] gzf.version [IF]


.( Testing: gzf) cr 

t{ gzf-create gzf1 }t

t{ s" unknown.gz" gzf1 gzf-open-file ?true }t  \ actually <>0

t{ s" gzf.gz"     gzf1 gzf-open-file ?0 }t

gzf1 gzf-dump

[THEN]

\ ==============================================================================
