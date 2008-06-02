\ ==============================================================================
\
\             cbf - the circulair buffer module in the ffl
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
\  $Date: 2008-06-02 05:24:51 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] cbf.version [IF]


include ffl/stc.fs


( cbf = Circulair buffer module )
( The cbf module implements a circulair buffer with variable elements.       )
( During adding of extra data, the buffer will be resized. The cbf-access!   )
( word expects two execution tokens on the stack: store with stack effect:   )
( i*x addr --  and fetch: addr -- i*x. Those two words are used to store     )
( data in the buffer and fetch data from the buffer. Their behaviour should  )
( match the size of the elements in the buffer.                              )
( Important: the cbf-get and cbf-fetch copy data from the buffer to the      )
( destination address. This is different from the lineair buffer [lbf]       )
( implementation: the lbf-get and lbf-fetch return addresses located         )
( in the buffer.                                                             )


1 constant cbf.version

\ Layout:  0  in  out       size
\          v   v   v         v
\         +------------------+
\         |xxxx    xxxxxxxxxx|
\         +------------------+


( Circulair Buffer Structure )

begin-structure cbf%       ( -- n = Get the required space for a cbf variable )
  field: cbf>record        \ the element size
  field: cbf>in            \ the in pointer
  field: cbf>out           \ the out pointer
  field: cbf>extra         \ the extra size during resizing
  field: cbf>size          \ the size of the buffer array
  field: cbf>buffer        \ the buffer array
  field: cbf>fetch         \ the fetch xt or nil
  field: cbf>store         \ the store xt or nil
end-structure



( Private database )

8 value cbf.extra  ( -- +n = Initial extra space )



( Buffer creation, initialisation and destruction )

: cbf-init         ( +n1 +n2 cbf -- = Initialise the buffer with element size n1 and initial length n2 )
  >r
  swap      r@ cbf>record !
  cbf.extra r@ cbf>extra  !
  nil       r@ cbf>fetch  !
  nil       r@ cbf>store  !
            r@ cbf>in    0!
            r@ cbf>out   0!
  ?dup 0= IF                        \ size = length or cbf.extra
    cbf.extra
  THEN
  dup r@ cbf>size !
  
  r@ cbf>record @ * allocate throw  \ allocate the buffer
  
  r> cbf>buffer !
;


: cbf-(free)       ( cbf -- = Free the internal data from the heap )
  cbf>buffer @ free throw
;


: cbf-create       ( +n1 +n2 "<spaces>name" -- ; -- cbf = Create a circulair buffer in the dictionary with element size n1 and initial length n2 )
  create  here  cbf% allot  cbf-init
;


: cbf-new          ( +n1 +n2 -- cbf = Create a circulair buffer with element size n1 and initial length n2 on the heap )
  cbf% allocate throw  dup >r cbf-init r> 
;


: cbf-free         ( cbf -- = Free the circulair buffer from the heap )
  dup cbf-(free)
   
  free throw
;


( Private words )

: cbf-buffer@      ( cbf -- addr = Get the start of the buffer )
  cbf>buffer @
;


: cbf-in@          ( cbf -- u = Get the in offset )
  cbf>in @
;


: cbf-out@         ( cbf -- u = Get the out offset )
  cbf>out @
;


: cbf-record@      ( cbf -- u = Get the element size )
  cbf>record @
;


: cbf-size@        ( cbf -- u = Get the size of the buffer )
  cbf>size @
;


( Member words )

: cbf-length@      ( cbf -- u = Get the number of elements in the buffer )
  dup  cbf-in@
  over cbf-out@ -
  dup 0< IF
    swap cbf-size@ +
  ELSE
    nip
  THEN
;


: cbf-extra@       ( cbf -- u = Get the number of extra elements allocated during resizing of the buffer )
  cbf>extra @
;


: cbf-extra!       ( u cbf -- = Set the nmber of extra elements allocated during resizing of the buffer )
  swap 1 max swap cbf>extra !
;


: cbf-size!        ( +n cbf -- = Insure the size of the buffer )
  2dup cbf>size @ > IF
    tuck cbf-extra@ +        \ add extra space 
    2dup swap cbf>size !     \ set the new size in cells
    over cbf-record@ *       \ size in bytes
    over cbf-buffer@
    swap resize throw        \ resize the array
    swap cbf>buffer !
    \ ToDo : move
  ELSE
    2drop
  THEN
;


: cbf+extra@       ( -- u = Get the initial number of extra elements allocated during resizing of the buffer )
  cbf.extra
;


: cbf+extra!       ( u -- = Set the initial number of extra elements allocated during resizing of the buffer )
  1 max to cbf.extra
;


: cbf-access@      ( cbf -- xt1 xt2 = Get the store word xt1 and the fetch word xt2 for the buffer )
  dup  cbf>store @
  swap cbf>fetch @
;


: cbf-access!      ( xt1 xt2 cbf -- = Set the store word xt1 and the fetch word x2 for the buffer )
  tuck cbf>fetch !
       cbf>store !
;
 
( Private words )

: cbf-in           ( cbf -- addr = Get the address of in )
  dup  cbf-in@
  over cbf-record@ *
  swap cbf-buffer@ +
;


: cbf-out          ( cbf -- addr = Get the address of out )
  dup  cbf-out@
  over cbf-record@ *
  swap cbf-buffer@ +
;


( Lifo words )

: cbf-set          ( addr u cbf -- = Set u elements, starting from addr in the buffer, resize if necessary )
  \ ToDo
  >r
  r@ cbf-in@ over + r@ cbf-size!              \ Insure size of in + new elements
  tuck r@ cbf-in swap r@ cbf-record@ * move   \ Move data in the buffer
  r> cbf>in +!                                \ in += new elements
;


: cbf-get          ( u1 cbf -- addr u2 | 0 = Get maximum u1 elements from the buffer, return the actual number of elements u2 )
  \ ToDo
  >r
  r@ cbf-length@ min            \ actual number of elements
  dup IF
    r@ cbf-out swap
    dup r@ cbf>out +!           \ out += returned elements
  THEN
  rdrop
;


: cbf-fetch        ( u1 cbf -- addr u2 | 0 = Fetch maximum u1 elements from the buffer, return the actual number of elements u2 )
  \ ToDo
  >r
  r@ cbf-length@ min            \ actual number of elements
  dup IF
    r@ cbf-out swap             \ return the output pointer
  THEN
  rdrop
;


: cbf-seek-fetch   ( u1 n cbf -- addr u2 | 0 = Fetch maximum u1 elements from the buffer, offsetted by n, return the actual number of elements u2 )
  \ ToDo
  >r
  dup 0< IF                     \ Find index for fetch
    r@ cbf-in@ +
    dup r@ cbf-out@ <
  ELSE
    r@ cbf-out@ +
    dup r@ cbf-in@ >= 
  THEN
  exp-index-out-of-range AND throw

  tuck r@ cbf-in@ swap - min    \ Calculate length
  dup IF
    swap
    r@ cbf-record@ *
    r@ cbf-buffer@ +
    swap
  ELSE
    nip
  THEN
  rdrop
;


: cbf-skip         ( u1 cbf -- u2 = Skip maximum u1 elements from the buffer, return the actually skipped elements u2 )
  \ ToDo
  >r
  r@ cbf-length@ min         \ actual number of elements to skip
  dup IF
    dup r@ cbf>out +!        \ update the out pointer
  THEN
  rdrop
;


: cbf-enqueue      ( i*x | addr cbf -- = Enqueue one element in the buffer, optional using the store word )
  \ ToDo
  >r
  r@ cbf-in@ 1+ r@ cbf-size!            \ Insure size of one extra element
  r@ cbf>store @ nil<>? IF              \ If store word Then
    r@ cbf-in swap execute              \   Store i*x
  ELSE                                  \ Else
    r@ cbf-in r@ cbf-record@ move       \   Move addr
  THEN
  r> cbf>in 1+!
;


: cbf-dequeue      ( cbf -- i*x | addr true | false = Dequeue one element from the buffer, optional using the fetch word )
  \ ToDo
  >r
  r@ cbf-length@ IF              \ Check data present
    r@ cbf-out
    r@ cbf>fetch @ nil<>? IF     \ If fetcher present, then fetch the data
      execute
    THEN
    r@ cbf>out 1+!               \ Set data read
    true
  ELSE
    false
  THEN
  rdrop
;


( Fifo words )

: cbf-tos          ( cbf -- i*x | addr true | false = Fetch the top element, optional using the fetch word )
  \ ToDo
  dup cbf-length@ IF            \ Check data present
    dup  cbf-in@      1-        \ Fetch tos
    over cbf-record@ *
    over cbf-buffer@ +
    swap cbf>fetch @ nil<>? IF  \ If fetcher present, then fetch data
      execute
    THEN
    true
  ELSE
    drop
    false
  THEN
;


: cbf-push         ( i*x | addr cbf -- = Push one element in the buffer, optional using the fetch word )
  cbf-enqueue
;


: cbf-pop          ( cbf -- i*x | addr true | false = Pop one element from the buffer, optional using the fetch word )
  \ ToDo
  >r
  r@ cbf-length@ IF             \ Check data present
    r@ cbf>in 1-!               \ Pop data
    r@ cbf-in
    r@ cbf>fetch @ nil<>? IF    \ If fetcher present, then fetch data
      execute 
    THEN
    true
  ELSE
    false
  THEN
  rdrop
;


( Buffer words )

: cbf-clear        ( cbf -- = Clear the buffer )
  dup cbf>out 0!
      cbf>in  0!
;


( Inspection )

: cbf-dump         ( cbf -- = Dump the circulair buffer )
  ." cbf:" dup . cr
  ."   record:" dup cbf>record ? cr
  ."   in    :" dup cbf>in     ? cr
  ."   out   :" dup cbf>out    ? cr
  ."   extra :" dup cbf>extra  ? cr
  ."   size  :" dup cbf>size   ? cr
  ."   fetch :" dup cbf>fetch  ? cr
  ."   store :" dup cbf>store  ? cr
  ."   buffer:"     cbf>buffer ? cr
;

[THEN]

\ ==============================================================================
