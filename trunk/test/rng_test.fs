\ ==============================================================================
\
\         rng_test - the test words for the rng module in the ffl
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
\  $Date: 2007-01-11 19:22:04 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/rng.fs
include ffl/tst.fs


[DEFINED] rng.version [IF]


.( Testing: rng) cr 

t{ 5489 rng-create rng1 }t

t{ rng1 rng-next-number 3499211612 ?u }t

: rng-skip  ( w:rng n - = skip n random numbers )
  0 DO
    dup rng-next-number drop
  LOOP
  drop
;

t{ rng1 622 rng-skip }t 

t{ rng1 rng-next-number 4020325887 ?u }t
t{ rng1 rng-next-number 4178893912 ?u }t
t{ rng1 rng-next-number 610818241  ?u }t

t{ rng1 9999 rng-skip }t

t{ rng1 rng-next-number 862334504 ?u }t


[DEFINED] rng-next-float [IF]
t{ 19650218 rng-new value rng2 }t

t{ rng2 rng-next-float 0.54146918e0 ?r }t

t{ rng2 9999 rng-skip }t

t{ rng2 rng-next-float 0.95790732e0 ?r }t

t{ rng2 rng-free }t
[THEN]

[THEN]

\ ==============================================================================
