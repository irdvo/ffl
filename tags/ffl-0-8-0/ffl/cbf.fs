\ ==============================================================================
\
\             cbf - the circular buffer module in the ffl
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
\  $Date: 2008-07-03 17:21:49 $ $Revision: 1.5 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] cbf.version [IF]


include ffl/stc.fs


( cbf = Circular buffer module )
( The cbf module implements a circular buffer with variable elements.        )
( During adding of extra data, the buffer will be resized. The cbf-access!   )
( word expects two execution tokens on the stack: store with stack effect:   )
( i*x addr --  and fetch: addr -- i*x. Those two words are used to store     )
( data in the buffer and fetch data from the buffer. Their behavior should   )
( match the size of the elements in the buffer.                              )
( Important: the cbf-get and cbf-fetch copy data from the buffer to the      )
( destination address. This is different from the linear buffer [lbf]        )
( implementation: the lbf-get and lbf-fetch return addresses located         )
( in the buffer.                                                             )


1 constant cbf.version

\ Layout:  0  in  out       size
\          v   v   v         v
\         +------------------+
\         |xxxx    xxxxxxxxxx|
\         +------------------+


( Circular Buffer Structure )

begin-structure cbf%       ( -- n = Get the required space for a cbf variable )
  field: cbf>record        \ the element size
  field: cbf>in            \ the in pointer
  field: cbf>out           \ the out pointer
  field: cbf>start         \ the start pointer, during fetching
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
            r@ cbf>start 0!
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


: cbf-create       ( +n1 +n2 "<spaces>name" -- ; -- cbf = Create a circular buffer in the dictionary with element size n1 and initial length n2 )
  create  here  cbf% allot  cbf-init
;


: cbf-new          ( +n1 +n2 -- cbf = Create a circular buffer with element size n1 and initial length n2 on the heap )
  cbf% allocate throw  dup >r cbf-init r> 
;


: cbf-free         ( cbf -- = Free the circular buffer from the heap )
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


: cbf-start@       ( cbf -- u = Get the seek offset )
  cbf>start @
;


: cbf-record@      ( cbf -- u = Get the element size )
  cbf>record @
;


: cbf-size@        ( cbf -- u = Get the size of the buffer )
  cbf>size @
;


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


: cbf-start        ( cbf -- addr = Get the address of start )
  dup  cbf-start@
  over cbf-record@ *
  swap cbf-buffer@ +
;


: cbf-in+!         ( n cbf -- = Add n elements to the in index )
  tuck
  cbf-in@ +
  2dup swap cbf-size@ >= IF   \ Circular buffer
    over cbf-size@ -
  THEN
  swap cbf>in !
;


: cbf-out+!        ( n cbf -- = Add n elements to the out index )
  tuck
  cbf-out@ +
  2dup swap cbf-size@ >= IF   \ Circular buffer
    over cbf-size@ -
  THEN
  swap cbf>out !
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


: cbf-extra!       ( u cbf -- = Set the number of extra elements allocated during resizing of the buffer )
  swap 1 max swap cbf>extra !
;


: cbf-size!        ( +n cbf -- = Insure the size of the buffer )
  2dup cbf>size @ >= IF
    >r
    r@ cbf-size@ swap        \ Save original size
    r@ cbf-extra@ +          \ add extra space 
    dup r@ cbf>size !        \ set the new size in cells
    r@ cbf-record@ *         \ size in bytes
    r@ cbf-buffer@
    swap resize throw        \ resize the array
    r@ cbf>buffer !
    
    r@ cbf-out@ r@ cbf-in@ > IF       \ Loop around ?
      r@ cbf-size@ over -             \ offset for out index
      swap r@ cbf-out@ -              \ number elements to copy
      r@ cbf-size@ over -             \ the to index
      r@ cbf-record@ * r@ cbf-buffer@ +  \ the to pointer
      swap r@ cbf-record@ *           \ number bytes to copy
      r@ cbf-out                      \ the from pointer
      -rot move                       \ move the data to the end of the buffer
      r@ cbf-out+!                    \ move the out index
    ELSE
      drop
    THEN
    rdrop
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


( Private lifo words )

: cbf-do-fetch     ( addr u1 cbf -- u2 = Fetch maximum u1 elements from start in the buffer in addr, return the actual number of elements u2 )
  >r
  r@ cbf-in@ r@ cbf-start@ -
  dup 0< IF
    r@ cbf-size@ +
  THEN
  min                                   \ Actual number of elements to fetch
  tuck                                  \ Save actual number of elements
  2dup r@ cbf-size@ r@ cbf-start@ - min \ Fetch until end of buffer
  ?dup IF
    tuck
    r@ cbf-start -rot  r@ cbf-record@ *  move  \ Move till end of buffer
    tuck -
    -rot r@ cbf-record@ * + swap        \ Update address and number after moving
  ELSE
    drop
  THEN
  ?dup IF
    r@ cbf-buffer@ -rot  r@ cbf-record@ * move  \ Move remaining from start of buffer
  ELSE
    drop
  THEN
  rdrop
;


( Lifo words )

: cbf-set          ( addr u cbf -- = Set u elements, starting from addr in the buffer, resize if necessary )
  >r
  r@ cbf-length@ over + r@ cbf-size!    \ Insure size of buffer
  tuck                                  \ Save number of elements
  2dup r@ cbf-size@ r@ cbf-in@ - min    \ Insert until end of buffer
  ?dup IF
    tuck
    r@ cbf-in swap  r@ cbf-record@ * move \ Move till end of buffer
    tuck -
    -rot r@ cbf-record@ * + swap        \ Update address and number after moving
  ELSE
    drop
  THEN
  ?dup IF
    r@ cbf-buffer@ swap  r@ cbf-record@ * move \ Move remaining at start of buffer
  ELSE
    drop
  THEN
  r> cbf-in+!                           \ Update in index
;


: cbf-fetch        ( addr u1 cbf -- u2 = Fetch maximum u1 elements from the buffer in addr, return the actual number of elements u2 )
  dup cbf-out@ over cbf>start !  \ Fetch from out
  cbf-do-fetch
;


: cbf-get          ( addr u1 cbf -- u2 = Get maximum u1 elements from the buffer in addr, return the actual number of elements u2 )
  >r
  r@ cbf-fetch              \ Fetch the data
  dup r> cbf-out+!          \ Update the out index
;


: cbf-skip         ( +n1 cbf -- +n2 = Skip maximum u1 elements from the buffer, return the actual skipped elements u2 )
  swap
  over cbf-length@ min       \ Actual elements to skip
  tuck swap cbf-out+!        \ Update out pointer
;


: cbf-enqueue      ( i*x | addr cbf -- = Enqueue one element in the buffer, optional using the store word )
  >r
  r@ cbf-length@ 1+ r@ cbf-size!        \ Insure size of one extra element
  r@ cbf-in
  r@ cbf>store @ nil<>? IF              \ If store word Then
    execute                             \   Store i*x
  ELSE                                  \ Else
    r@ cbf-record@ move                 \   Move addr
  THEN
  1 r> cbf-in+!
;


: cbf-dequeue      ( cbf -- i*x | addr true | false = Dequeue one element from the buffer, optional using the fetch word )
  >r
  r@ cbf-length@ IF              \ Check data present
    r@ cbf-out
    r@ cbf>fetch @ nil<>? IF     \ If fetcher present, then fetch the data
      execute
    THEN
    1 r@ cbf-out+!               \ Set data read
    true
  ELSE
    false
  THEN
  rdrop
;


( Fifo words )

: cbf-tos          ( cbf -- i*x | addr true | false = Fetch the top element, optional using the fetch word )
  dup cbf-length@ IF            \ Check data present
    dup cbf-in@ ?dup 0= IF      \ Fetch tos
      dup cbf-size@
    THEN
    1-
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


: cbf-push         ( i*x | addr cbf -- = Push one element in the buffer, optional using the store word )
  cbf-enqueue
;


: cbf-pop          ( cbf -- i*x | addr true | false = Pop one element from the buffer, optional using the fetch word )
  >r
  r@ cbf-length@ IF             \ Check data present
    r@ cbf-in@ ?dup 0= IF       \ Move in pointer for pop
      r@ cbf-size@
    THEN
    1- r@ cbf>in !
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

: cbf-dump         ( cbf -- = Dump the circular buffer variable )
  ." cbf:" dup . cr
  ."   record:" dup cbf>record ? cr
  ."   in    :" dup cbf>in     ? cr
  ."   out   :" dup cbf>out    ? cr
  ."   start :" dup cbf>start  ? cr
  ."   extra :" dup cbf>extra  ? cr
  ."   size  :" dup cbf>size   ? cr
  ."   fetch :" dup cbf>fetch  ? cr
  ."   store :" dup cbf>store  ? cr
  ."   buffer:"     cbf>buffer ? cr
;

[THEN]

\ ==============================================================================
