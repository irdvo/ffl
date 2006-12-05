\ ==============================================================================
\
\            car - the cell array collection module in the ffl
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
\  $Date: 2006-12-05 18:32:48 $ $Revision: 1.7 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] car.version [IF]


include ffl/stc.fs


( car = Cell array module )
( The car module implements a dynamic cell array. )


1 constant car.version


( Cell Array Structure )

struct: car%       ( - n = Get the required space for the car data structure )
  cell:  car>length
  cell:  car>size
  cell:  car>extra
  cell:  car>cells
  cell:  car>compare
;struct



( Private database )

8 value car.extra  ( - w = Initial extra space )



( Array creation, initialisation and destruction )

: car-init         ( n:length w:car - = Initialise the cell array with an initial length )
  >r
  car.extra r@ car>extra !
  ['] - r@ car>compare !
  
  0 max dup r@ car>length !  \ length >= 0
  
  dup 0= IF                  \ size = length or car.extra
    drop car.extra
  THEN
  dup r@ car>size !
  
  cells dup allocate throw   \ allocate the array
  
  dup r> car>cells !
  
  swap erase
;


: car-create       ( C: n:length "name" - R: - w:car = Create a cell array with an initial length in the dictionary )
  create  here  car% allot  car-init
;


: car-new          ( n:length - w:car = Create a cell array with an initial length on the heap )
  car% allocate throw  dup >r car-init r> 
;


: car-free         ( w:car - = Free the array from the heap )
  dup car>cells @ free throw
  free throw
;



( Private words )

: car-offset?      ( n w:car - f = Check if an offset is valid in the array )
  0 swap car>length @ within
;


: car-offset       ( n w:car - n = Get the array offset [0..length> from an index [-length..length> with checking )
  tuck car>length @ index2offset
  
  dup rot car-offset?
  
  0= exp-index-out-of-range AND throw
;


: car-cells@       ( w:car - addr = Get the start of the cells array )
  car>cells @
;



( Member words )

: car-length@      ( w:car - u = Get the number of elements in the array )
  car>length @
;


: car-index?       ( n w:car - f = Check if an index is valid in the array )
  tuck car-length@  index2offset  swap car-offset?
;


: car-size!        ( n w:car - = Insure the size of the array )  
  2dup car>size @ > IF
    tuck car>extra @ +       \ add extra space 
    2dup swap car>size !     \ set the new size in cells
    cells                    \ size in bytes
    over car-cells@
    swap resize throw        \ resize the array
    swap car>cells !
  ELSE
    2drop
  THEN
;


: car-extra@       ( w:car - u = Get the extra space allocated during resizing of the array )
  car>extra @
;


: car-extra!       ( u w:car - = Set the extra space allocated during resizing of the array )
  swap 1 max swap car>extra !
;


: car+extra@       ( - u = Get the initial extra space allocated during resizing of the array )
  car.extra
;


: car+extra!       ( u - = Set the initial extra space allocated during resizing of the array )
  1 max to car.extra
;


: car-compare@     ( w:car - xt = Get the compare execution token for sorting the array )
  car>compare @
;


: car-compare!     ( xt w:car - = Set the compare execution token for sorting the array )
  car>compare !
;



( Private words )

: car-length+!     ( u w:car - n = Increase the length of the array, return the previous length )
  tuck car-length@ +
  swap
  2dup car-size!
  car>length @!
;



( Array words )

: car-clear        ( w:car - = Clear the array )
  dup car>cells @ swap car>length @ cells erase
;


: car-set          ( w n:index w:car - = Set the cell value at the indexth position )
  tuck car-offset
  cells swap car-cells@ + !
;


: car-get          ( n:index w:car - w = Get the cell value from the indexth position )
  tuck car-offset
  cells swap car-cells@ + @
;


: car-append       ( w w:car - = Append the cell value at the end of the array )
  1 over car-length+!        \ increase length (and size)
  cells swap car-cells@ + !  \ put cell at the end
;


: car-prepend      ( w w:car - = Prepend the cell value before the start of the array )
  1 over car-length+!        \ increase the length (and size)
  >r
  dup car-cells@ dup cell+   \ source = start, dest = start+1
  r> cells move              \ move length cells
  car-cells@ !               \ put cell at the start
;


: car-insert       ( w n:index w:car - = Insert the cell value at the indexth position )
  tuck car-offset
  over 1 swap car-length+!             \ Increase the length (and size)
  over - 
  >r cells swap car-cells@ + r>        \ Source = offset
  dup 0> IF
    cells over dup cell+ rot move      \ Move the trailing cells
  ELSE
    drop
  THEN
  !                                    \ Store the cell
;


: car-delete       ( n:index w:car - w = Delete the cell value at the indexth position, return the previous cell value )
  tuck car-offset
  2dup cells swap car-cells@ + @ >r  \ Fetch the cell info
  over car-length@                   \ Calculate the number of elements to move
  over - 1-
  dup 0> IF                          \ If elements to move Then
    cells
    >r cells 
    over car-cells@ +                \   Destination = offset, source = offset + 1
    dup cell+ swap
    r> move
  ELSE
    2drop
  THEN
  car>length 1-!                     \ Decrease the length
  r>
;



( Private sorting words )

: car-sort-exchange  ( n n w:car - = Exchange the values in the array for the two offsets )
  car-cells@
  swap cells over + >r       \ Convert first offset to an address
  swap cells +               \ Convert second offset to an address
  r@ @                       \ Fetch value from first address
  swap dup @ >r              \ Fetch value from second address
  ! r> r> !                  \ Store contents exchanged
;


: car-sort-compare  ( n n w:car - n = Compare both offsets with the compare word )
  >r
  r@ car-cells@
  swap cells over + @ 
  >r swap cells + @ r>
  r> car-compare@ execute
;


: car-sift-down    ( n:bottom n:root w:car - = Sift down during heap sort )
  >r
  tuck 2* 1+
  BEGIN
    2dup >
  WHILE
    2dup 1+ > IF
      dup 1+ over r@ car-sort-compare 0> IF
        1+
      THEN
    THEN
    
    rot
    2dup swap r@ car-sort-compare 0>= IF
      -rot drop dup
    ELSE
      2dup r@ car-sort-exchange
      drop
      tuck 2* 1+
    THEN
  REPEAT
  2drop drop
  rdrop
;



( Sorting words )

: car-sort         ( w:car - = Sort the array using the compare execution token using heap sort )
  >r
  r@ car-length@ 2/ 1-
  BEGIN
    dup 0>= 
  WHILE
    r@ car-length@ over r@ car-sift-down
    1-
  REPEAT
  drop
  
  r@ car-length@
  BEGIN
    dup 1 >
  WHILE
    1-
    dup 0 r@ car-sort-exchange
    dup 0 r@ car-sift-down
  REPEAT
  drop
  rdrop
;


: car-find-sorted  ( w w:car - n f = Find the cell value in the already sorted array using binary search )
  swap >r >r
  r@ car-length@ 1- 0
  BEGIN
    2dup >=
  WHILE
    2dup + 2/
    r> r@ swap >r
    over cells r@ car-cells@ + @
    r@ car-compare@ execute
    dup 0< IF
      drop 1- rot drop swap
    ELSE
      0> IF
        1+ nip
      ELSE
        rdrop rdrop
        nip nip
        true
        exit
      THEN
    THEN
  REPEAT
  
  rdrop rdrop
  nip
  false
;


: car-has-sorted?  ( w w:car - f = Check if a cell value is present in an already sorted array )
  car-find-sorted nip
;

      
: car-insert-sorted ( w w:car - = Insert the cell value sorted in an already sorted array )
  2dup car-find-sorted
  drop over car-length@ over <= IF
    drop car-append
  ELSE
    swap car-insert
  THEN
;



( Fifo/Lifo words )

: car-tos          ( w:car - w = Get the cell value at the end of the array )
  dup car-length@
  
  dup 0= exp-no-data AND throw
  
  1- cells swap car-cells@ + @
;  


: car-push         ( w w:car - = Push a cell value at the end of the array )
  car-append
;


: car-pop          ( w:car - w = Pop a cell value from the end of the array )
  dup car-tos
  
  swap car>length 1-!
;


: car-enqueue      ( w w:car - = Enqueue a cell value at the start of the array )
  car-prepend
;


: car-dequeue      ( w:car - w = Dequeue a cell value from the end of the array )
  car-pop
;



( Special words )

: car-count        ( w w:car - u = Count the occurences of a cell value in the array )
  0 -rot
  dup car-cells@ swap car-length@ 0 ?DO     \ Loop the array
    2dup @ = IF                             \ If the contents is the cell Then
      rot 1+ -rot                           \   Increase the counter
    THEN
    
    cell+
  LOOP
  2drop
;


: car-find         ( w w:car - n:index = Find the first occurence of a cell value in the array, -1 if not found )
  dup car-cells@ swap car-length@ 0 ?DO     \ Loop the array
    2dup @ = IF                             \ If contents is the cell Then
      2drop                                 \   Return index and exit
      I unloop exit
    THEN
    
    cell+
  LOOP
  2drop -1                                  \ Not found, return -1
;


: car-has?         ( w w:car - f = Check if a cell value is present in the array )
  car-find 0>=
;


: car-execute      ( ... xt w:car - ... = Execute the token for every cell in the array )
  dup car-cells@ swap car-length@ 0 ?DO     \ Loop the array
    2dup 2>r                                \ Clear the stack
    @ swap execute                          \ Execute the token with the array contents
    2r>
    cell+
  LOOP
  2drop
;



( Inspection )

: car-dump         ( w:car - = Dump the cell array )
  ." car:" dup . cr
  ."  length :" dup car>length ? cr
  ."  size   :" dup car>size   ? cr
  ."  extra  :" dup car>extra  ? cr
  ."  compare:" dup car>compare ? cr
  ."  cells  :" ['] . swap car-execute 
;

[THEN]

\ ==============================================================================
