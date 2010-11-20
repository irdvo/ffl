\ ==============================================================================
\
\    scl_expl - the cell based single linked list example in the ffl
\
\               Copyright (C) 2010  Dick van Oudheusden
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
\  $Date: 2008-04-10 16:12:01 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/scl.fs
include ffl/rng.fs


\ Example: sort a cell based single linked list with 1001 random numbers


\ Create the single linked list on the heap

scl-new value nlist

\ Create the pseudo random generator in the dictionary with seed 5498

5498 rng-create nrng

\ Insert 1001 numbers in the nlist

: nlist-insert     ( n -- = Insert n random numbers in nlist )
  0 DO
    nrng rng-next-number          \ Generate random number and ..
    nlist scl-append              \ .. append to the list
  LOOP
;

1001 nlist-insert

\ Check the number of numbers out of sequence

: nnode-out-sequence ( n1 n2 flag n3 -- n4 n5 true = Count the number of out of sequence number, n1: count n2:previous number n3: number )
  swap IF
    tuck > IF >r 1+ r> THEN       \ Compare current number with previous number increment counter if out of sequence
  ELSE
    nip                           \ First call, no check
  THEN
  true
;

.( Before sorting there are ) 0 0 false ' nnode-out-sequence nlist scl-execute 2drop . .( numbers out of sequence. ) cr

\ Sort the list using the <=> word

' <=> nlist scl-sort

\ Check the number of numbers out of sequence again

.( After sorting there are ) 0 0 false ' nnode-out-sequence nlist scl-execute 2drop . .( numbers out of sequence. ) cr

\ Cleanup the list

nlist scl-free

