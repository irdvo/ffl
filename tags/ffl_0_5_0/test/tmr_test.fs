\ ==============================================================================
\
\          tmr_test - the test words for the tmr module in the ffl
\
\               Copyright (C) 2006  Dick van Oudheusden
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
\  $Date: 2006-12-23 08:07:07 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/tmr.fs
include ffl/tst.fs


[DEFINED] tmr.version [IF]

.( Testing: tmr) cr 
  
300 tmr-create tmr1   \ 0.3 sec.

t{ tmr1 tmr-expired? ?false }t

200 ms

t{ tmr1 tmr-expired? ?false }t

200 ms

t{ tmr1 tmr-expired? ?true }t

210 ms

t{ tmr1 tmr-expired? ?true }t

300 ms

t{ tmr1 tmr-restart }t

t{ tmr1 tmr-timer@ 2 u< ?true }t

t{ tmr1 tmr-expired? ?false }t

t{ tmr1 tmr-timeout@ 300 ?u }t



t{ 0 tmr-new value tmr2 }t

200 ms

t{ tmr2 tmr-timer@ 250 u< ?true }t

t{ 20 tmr2 tmr-start }t

t{ tmr2 tmr-wait }t

t{ tmr2 tmr-timer@ 2 u< ?true }t

t{ tmr2 tmr-free }t

[THEN]


\ ==============================================================================

