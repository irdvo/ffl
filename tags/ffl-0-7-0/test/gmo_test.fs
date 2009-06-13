\ ==============================================================================
\
\        gmo_test - the test words for the gmo module in the ffl
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
\  $Date: 2007-12-24 19:32:12 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/gmo.fs
include ffl/tst.fs


[DEFINED] gmo.version [IF]

bigendian? [IF]

.( Testing: gmo skipped, no bigendian test file)  cr

[ELSE]

.( Testing: gmo) cr

t{ msc-new value msc2 }t

t{ s" nl.mo" msc2 gmo-read ?0 }t

t{ msc2 hnt-length@ 38 ?s }t

\ msc2 msc-dump

t{ s" Sunday" msc2 msc-translate s" zondag" ?str }t
t{ s" Mar"    msc2 msc-translate s" maa"    ?str }t

t{ msc2 msc-free }t

[THEN]

[THEN]

\ ==============================================================================
