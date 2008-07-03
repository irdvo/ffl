\ ==============================================================================
\
\              lbf - the lineair buffer module in the ffl
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
\  $Date: 2008-07-03 17:26:06 $ $Revision: 1.6 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] lbf.version [IF]


include ffl/stc.fs


( lbf = Lineair buffer module )
( The lbf module implements a lineair buffer with variable elements. During  )
( adding of extra data, the buffer will be resized. This type of buffer is   )
( most efficient if the buffer is empty on a regular bases: the unused space )
( in the buffer is then automatically reduced. If the buffer is not          )
( regularly empty, use the lbf-reduce word to reuse the unused space in the  )
( buffer. The lbf-access! word expects two execution tokens on the stack:    )
( store with stack effect:  i*x addr --  and fetch: addr -- i*x. Those two   )
( words are used to store data in the buffer and fetch data from the buffer. )
( Their behaviour should match the size of the elements in the buffer.       )
( Besides the normal out pointer there is a secondary out pointer. This      )
( pointer will always stay between the normal out pointer and the in         )
( pointer. The words lbf-get' and lbf-length' use the secondary out pointer. )
( Important: the lbf-get and lbf-fetch returning addresses are located       )
( in the buffer so the contents of these addresses can change with the next  )
( call to the buffer. This is different from the circulair buffer [cbf]      )
( implementation: the cbf-get and cbf-fetch words copy data from the buffer  )
( to the destination addresses. )


1 constant lbf.version

\ Layout:  0  out      in   size
\          v   v       v     v
\         +------------------+
\         |    xxxxxxxx      |
\         +------------------+


( Lineair Buffer Structure )

begin-structure lbf%       ( -- n = Get the required space for a lbf variable )
  field: lbf>record        \ the element size
  field: lbf>in            \ the in offset
  field: lbf>out           \ the out offset
  field: lbf>out'          \ the secondary out offset
  field: lbf>extra         \ the extra size during resizing
  field: lbf>size          \ the size of the buffer array
  field: lbf>buffer        \ the buffer array
  field: lbf>fetch         \ the fetch xt or nil
  field: lbf>store         \ the store xt or nil
end-structure



( Private database )

8 value lbf.extra  ( -- +n = Initial extra space )



( Buffer creation, initialisation and destruction )

: lbf-init         ( +n1 +n2 lbf -- = Initialise the buffer with element size n1 and initial length n2 )
  >r
  swap      r@ lbf>record !
  lbf.extra r@ lbf>extra  !
  nil       r@ lbf>fetch  !
  nil       r@ lbf>store  !
            r@ lbf>in    0!
            r@ lbf>out   0!
            r@ lbf>out'  0!
  ?dup 0= IF                        \ size = length or lbf.extra
    lbf.extra
  THEN
  dup r@ lbf>size !
  
  r@ lbf>record @ * allocate throw  \ allocate the buffer
  
  r> lbf>buffer !
;


: lbf-(free)       ( lbf -- = Free the internal data from the heap )
  lbf>buffer @ free throw
;


: lbf-create       ( +n1 +n2 "<spaces>name" -- ; -- lbf = Create a lineair buffer in the dictionary with element size n1 and initial length n2 )
  create  here  lbf% allot  lbf-init
;


: lbf-new          ( +n1 +n2 -- lbf = Create a lineair buffer with element size n1 and initial length n2 on the heap )
  lbf% allocate throw  dup >r lbf-init r> 
;


: lbf-free         ( lbf -- = Free the lineair buffer from the heap )
  dup lbf-(free)
   
  free throw
;


( Private words )

: lbf-buffer@      ( lbf -- addr = Get the start of the buffer )
  lbf>buffer @
;


: lbf-in@          ( lbf -- u = Get the in offset )
  lbf>in @
;


: lbf-out@         ( lbf -- u = Get the out offset )
  lbf>out @
;


: lbf-out'@        ( lbf -- u = Get the secondary out offset )
  lbf>out' @
;


: lbf-record@      ( lbf -- u = Get the element size )
  lbf>record @
;


( Member words )

: lbf-length@      ( lbf -- u = Get the number of elements in the buffer )
  dup  lbf-in@
  swap lbf-out@ -
;


: lbf-length'@     ( lbf -- u = Get the number of elements in the buffer based on the second out pointer )
  dup  lbf-in@
  swap lbf-out'@ -
;


: lbf-extra@       ( lbf -- u = Get the extra space allocated during resizing of the buffer )
  lbf>extra @
;


: lbf-extra!       ( u lbf -- = Set the extra space allocated during resizing of the buffer )
  swap 1 max swap lbf>extra !
;


: lbf-size!        ( +n lbf -- = Insure the size of the buffer )
  2dup lbf>size @ > IF
    tuck lbf-extra@ +        \ add extra space 
    2dup swap lbf>size !     \ set the new size in cells
    over lbf-record@ *       \ size in bytes
    over lbf-buffer@
    swap resize throw        \ resize the array
    swap lbf>buffer !
  ELSE
    2drop
  THEN
;


: lbf+extra@       ( -- u = Get the initial extra space allocated during resizing of the buffer )
  lbf.extra
;


: lbf+extra!       ( u -- = Set the initial extra space allocated during resizing of the buffer )
  1 max to lbf.extra
;


: lbf-access@      ( lbf -- xt1 xt2 = Get the store word xt1 and the fetch word xt2 for the buffer )
  dup  lbf>store @
  swap lbf>fetch @
;


: lbf-access!      ( xt1 xt2 lbf -- = Set the store word xt1 and the fetch word x2 for the buffer )
  tuck lbf>fetch !
       lbf>store !
;
 
( Private words )

: lbf-in           ( lbf -- addr = Get the address of in )
  dup  lbf-in@
  over lbf-record@ *
  swap lbf-buffer@ +
;


: lbf-out          ( lbf -- addr = Get the address of out )
  dup  lbf-out@
  over lbf-record@ *
  swap lbf-buffer@ +
;


: lbf-out'         ( lbf -- addr = Get the address of secondary out )
  dup  lbf-out'@
  over lbf-record@ *
  swap lbf-buffer@ +
;


: lbf-norm         ( lbf -- = Reset the buffer to initial state if empty )
  dup  lbf-out@
  over lbf-out'@ > IF   \ If out higher than secondary out Then 
    dup  lbf-out@
    over lbf>out' !     \   Update secondary out
  THEN
  dup  lbf-in@
  over lbf-out@ = IF
    dup lbf>in   0!
    dup lbf>out  0!
        lbf>out' 0!
  ELSE
    drop
  THEN
;
  

( Lifo words )

: lbf-set          ( addr u lbf -- = Set u elements, starting from addr in the buffer, resize if necessary )
  >r
  r@ lbf-in@ over + r@ lbf-size!              \ Insure size of in + new elements
  tuck r@ lbf-in swap r@ lbf-record@ * move   \ Move data in the buffer
  r> lbf>in +!                                \ in += new elements
;


: lbf-get          ( u1 lbf -- addr u2 | 0 = Get maximum u1 elements from the buffer, return the actual number of elements u2 )
  >r
  r@ lbf-length@ min            \ actual number of elements
  dup IF
    r@ lbf-out swap
    dup r@ lbf>out +!           \ out += returned elements
    r@ lbf-norm
  THEN
  rdrop
;


: lbf-get'         ( u1 lbf -- addr u2 | 0 = Get maximum u1 elements from the buffer, based on secondary out, return the actual number of elements u2 )
  >r
  r@ lbf-length'@ min           \ actual number of elements (secondary based)
  dup IF
    r@ lbf-out' swap
    dup r@ lbf>out' +!          \ out += returned elements
  THEN
  rdrop
;

: lbf-fetch        ( u1 lbf -- addr u2 | 0 = Fetch maximum u1 elements from the buffer, return the actual number of elements u2 )
  >r
  r@ lbf-length@ min            \ actual number of elements
  dup IF
    r@ lbf-out swap             \ return the output pointer
  THEN
  rdrop
;


: lbf-fetch+       ( u1 n lbf -- addr u2 | 0 = Fetch maximum u1 elements from the buffer, offsetted by n, return the actual number of elements u2 )
  >r
  dup 0< IF                     \ Find index for fetch
    r@ lbf-in@ +
    dup r@ lbf-out@ <
  ELSE
    r@ lbf-out@ +
    dup r@ lbf-in@ >= 
  THEN
  exp-index-out-of-range AND throw

  tuck r@ lbf-in@ swap - min    \ Calculate length
  dup IF
    swap
    r@ lbf-record@ *
    r@ lbf-buffer@ +
    swap
  ELSE
    nip
  THEN
  rdrop
;


: lbf-skip         ( u1 lbf -- u2 = Skip maximum u1 elements from the buffer, return the actual skipped elements u2 )
  >r
  r@ lbf-length@ min         \ actual number of elements to skip
  dup IF
    dup r@ lbf>out +!        \ update the out pointer
    r@ lbf-norm
  THEN
  rdrop
;


: lbf-enqueue      ( i*x | addr lbf -- = Enqueue one element in the buffer, optional using the store word )
  >r
  r@ lbf-in@ 1+ r@ lbf-size!            \ Insure size of one extra element
  r@ lbf>store @ nil<>? IF              \ If store word Then
    r@ lbf-in swap execute              \   Store i*x
  ELSE                                  \ Else
    r@ lbf-in r@ lbf-record@ move       \   Move addr
  THEN
  r> lbf>in 1+!
;


: lbf-dequeue      ( lbf -- i*x | addr true | false = Dequeue one element from the buffer, optional using the fetch word )
  >r
  r@ lbf-length@ IF              \ Check data present
    r@ lbf-out
    r@ lbf>fetch @ nil<>? IF     \ If fetcher present, then fetch the data
      execute
    THEN
    r@ lbf>out 1+!               \ Set data read
    r@ lbf-norm
    true
  ELSE
    false
  THEN
  rdrop
;


( Fifo words )

: lbf-tos          ( lbf -- i*x | addr true | false = Fetch the top element, optional using the fetch word )
  dup lbf-length@ IF            \ Check data present
    dup  lbf-in@      1-        \ Fetch tos
    over lbf-record@ *
    over lbf-buffer@ +
    swap lbf>fetch @ nil<>? IF  \ If fetcher present, then fetch data
      execute
    THEN
    true
  ELSE
    drop
    false
  THEN
;


: lbf-push         ( i*x | addr lbf -- = Push one element in the buffer, optional using the store word )
  lbf-enqueue
;


: lbf-pop          ( lbf -- i*x | addr true | false = Pop one element from the buffer, optional using the fetch word )
  >r
  r@ lbf-length@ IF             \ Check data present
    r@ lbf>in 1-!               \ Pop data
    r@ lbf-in
    r@ lbf>fetch @ nil<>? IF    \ If fetcher present, then fetch data
      execute 
    THEN
    r@ lbf-norm
    true
  ELSE
    false
  THEN
  rdrop
;


( Buffer words )

: lbf-clear        ( lbf -- = Clear the buffer )
  dup lbf>out  0!
  dup lbf>out' 0!
      lbf>in   0!
;


: lbf-reduce       ( u lbf -- = Remove the leading unused space in the buffer if the unused length is at least u elements )
  >r
  r@ lbf-out@ < IF              \ Test for threshold
    r@ lbf-out  r@ lbf-buffer@  r@ lbf-length@ r@ lbf-record@ *  move  \ Move the data to start of buffer
    r@ lbf-out@  negate  
    dup r@ lbf>in   +!          \ Update the in and .. 
        r@ lbf>out' +!          \ .. the out' and ..   
        r@ lbf>out  0!          \ .. and out offset 
  THEN
  rdrop 
;


( Inspection )

: lbf-dump         ( lbf -- = Dump the lineair buffer variable )
  ." lbf:" dup . cr
  ."   record:" dup lbf>record ? cr
  ."   in    :" dup lbf>in     ? cr
  ."   out   :" dup lbf>out    ? cr
  ."   out'  :" dup lbf>out'   ? cr
  ."   extra :" dup lbf>extra  ? cr
  ."   size  :" dup lbf>size   ? cr
  ."   fetch :" dup lbf>fetch  ? cr
  ."   store :" dup lbf>store  ? cr
  ."   buffer:"     lbf>buffer ? cr
;

[THEN]

\ ==============================================================================
