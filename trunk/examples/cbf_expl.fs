\ ==============================================================================
\
\      cbf_expl - the example file for the cbf module in the ffl
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
\  $Date: 2008-06-24 18:18:58 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/cbf.fs


\ Example 1: buffering characters strings


\ Create the circulair buffer in the dictionary with an initial size of 10 chars

1 chars 10 cbf-create char-buf


\ Put characters in the buffer

s" Hello" char-buf cbf-set

\ Get the length of the stored characters

.( Number characters in buffer:) char-buf cbf-length@ . cr

\ Put more characters in the buffer, resulting in a resize of the buffer

s" , a nice morning to you." char-buf cbf-set


\ Get characters from the buffer

.( Read the buffer:) pad 29 char-buf cbf-get pad swap type cr



\ Example 2: buffering compound data: pair of cells as element


\ Create the circulair buffer on the heap with an initial size of 3 elements

2 cells 3 cbf-new value xy-buf


\ Set the store and fetch words for the buffer

' 2! ' 2@ xy-buf cbf-access!


\ Use the buffer as fifo buffer, using the store and fetch words

1 2 xy-buf cbf-enqueue
3 4 xy-buf cbf-enqueue
5 6 xy-buf cbf-enqueue
7 8 xy-buf cbf-enqueue       \ Buffer is resized

\ Get the length of the stored elements in the buffer

.( Number elements in buffer:) xy-buf cbf-length@ . cr

\ Get first element from buffer

.( First pair in buffer:) xy-buf cbf-dequeue [IF]
  .  . cr
[ELSE]
  .(  nothing in buffer) cr
[THEN]


\ Use the buffer as lifo buffer, using the store and fetch words

\ Get last pair from buffer

.( Last pair in buffer:) xy-buf cbf-pop [IF]
  . . cr
[ELSE]
  .(  nothing in buffer) cr
[THEN]

\ Free the buffer from the heap

xy-buf cbf-free

\ ==============================================================================
