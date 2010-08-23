\ ==============================================================================
\
\       dnl_expl - the generic double linked list example in the ffl
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

include ffl/dnl.fs
include ffl/rng.fs


\ Example: sort a double linked list with 1001 random floats


\ Create the double linked list on the heap

dnl-new value flist

\ Extend the generic double linked list node with a float field

begin-structure fnode%
  dnn%
  +field   fnode>node
  ffield:  fnode>float
end-structure

\ Create the pseudo random generator in the dictionary with seed 5489

5479 rng-create frng

\ Insert 1001 float nodes in the flist

: flist-insert     ( n -- = Insert n random floats in flist )
  0 DO
    frng rng-next-float           \ Generate random float
    fnode% allocate throw         \ Allocate fnode
    dup fnode>node  dnn-init      \ Initialise generic node
    dup fnode>float f!            \ Store random float
        flist dnl-append          \ Append to flist
  LOOP
;

1001 flist-insert

\ Check the number of floats out of sequence

: fnode-out-sequence ( n1 r1 fnode -- n2 r2 = Count the number of out of sequence floats, n: count r:previous float )
  fnode>float f@
  fswap fover
  f> IF 1+ THEN                   \ Compare current float with previous float, increment counter if out of sequence
;

.( Before sorting there are ) 0 -1.0E+0 ' fnode-out-sequence flist dnl-execute fdrop . .( floats out of sequence. ) cr

\ Sort the list using the fnode-compare word

: fnode-compare    ( fnode1 fnode2 -- n = Compare fnode1 with fnode2 )
  swap fnode>float f@ fnode>float f@ f-
  fdup f0< IF
    fdrop -1
  ELSE f0= IF
    0
  ELSE
    1
  THEN THEN
;

' fnode-compare flist dnl-sort

\ Check the number of floats out of sequence again

.( After sorting there are ) 0 -1.0E+0 ' fnode-out-sequence flist dnl-execute fdrop . .( floats out of sequence. ) cr

\ Reverse the list 

flist dnl-reverse

\ Check the number of out of sequence floats again

.( After reversing the list there are ) 0 -1.0E+0 ' fnode-out-sequence flist dnl-execute fdrop . .( floats out of sequence. ) cr

\ Cleanup the list

flist dnl-free                    \ No dynamic memory stored in node, so default free word can be used

