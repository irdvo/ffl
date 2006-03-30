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
\  $Date: 2006-03-30 17:25:40 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] car.version [IF]


include ffl/stc.fs


( car = Cell array module )
( The car module implements a dynamic cell array. )


1 constant car.version


( Public structure )

struct: car%       ( - n = Get the required space for the car data structure )
  cell:  car>length
  cell:  car>size
  cell:  car>extra
  cell:  car>cells
;struct



( Private database )

8 value car.extra  ( - w = Initial extra space )



( Public words )

: car-init         ( n:length w:car - = Initialise the cell array with an initial length )
  >r
  0 max                      \ length >= 0
  dup 
  dup 0= IF                  \ size = length or car.extra
    drop car.extra
  THEN
  
  dup r@ car>size !
  dup cells allocate throw   \ allocate the array
  tuck swap cells erase
  r@ car>cells !
  r@ car>length !
  car.extra r> car>extra !
;


: car-create       ( C: n:length "name" - R: - w:car = Create a cell array with an initial length in the dictionary )
  create  here  car% allot  car-init
;


: car-new          ( n:length - w:car = Create a cell array with an initial length bit array on the heap )
  car% allocate throw  dup >r car-init r> 
;


: car-free         ( w:car - = Free the bit array from the heap )
  dup car>cells @ free throw
  free throw
;



( Private words )

: index2offset     ( n:index n:length - n:offset = Convert an index <-length..length> into an offset [0..length> )
  over 0< IF
    +
  ELSE
    drop
  THEN
;


: car-offset?      ( n w:car - f = Check if the offset is valid in the bit array )
  0 swap car>length @ within
;


: car-offset       ( n w:car - n = Get the array offset [0..length> from the index <-length..length> with checking )
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


: car-index?       ( n w:car - f = Check if the index is valid in the array )
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


: car-extra@       ( w:car - u = Get the extra heap space allocated during resizing of the array )
  car>extra @
;


: car-extra!       ( u w:car - = Set the extra heap space allocated during resizing of the array )
  swap 1 max swap car>extra !
;


: car+extra@       ( - u = Get the initial extra space allocated during resizing of the array )
  car.extra
;


: car+extra!       ( u - = Set the initial extra space allocated during resizing of the array )
  1 max to car.extra
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


: car-set          ( w n:index w:car - = Set the cell at the indexth position )
  tuck car-offset
  cells swap car-cells@ + !
;


: car-get          ( n:index w:car - w = Get the cell at the indexth position )
  tuck car-offset
  cells swap car-cells@ + @
;


: car-append       ( w w:car - = Append the cell in the array )
  1 over car-length+!        \ increase length (and size)
  cells swap car-cells@ + !  \ put cell at the end
;


: car-prepend      ( w w:car - = Prepend the cell in the array )
  1 over car-length+!        \ increase the length (and size)
  >r
  dup car-cells@ dup cell+   \ source = start, dest = start+1
  r> cells move              \ move length cells
  car-cells@ !               \ put cell at the start
;


: car-insert       ( w n:index w:car - = Insert the cell at the indexth position )
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


: car-delete       ( n:index w:car - w = Delete the cell at the indexth position )
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


: car-insert-sorted ( w w:car - = Insert the cell sorted )
;


( Special words )

: car-count        ( w w:car - u = Count the occurences of the cell in the array )
  0 -rot
  dup car-cells@ swap car-length@ 0 ?DO     \ Loop the array
    2dup @ = IF                             \ If the contents is the cell Then
      rot 1+ -rot                           \   Increase the counter
    THEN
    
    cell+
  LOOP
  2drop
;


: car-find         ( w w:car - n:index = Find the first occurence of the cell in the array, -1 if not found )
  dup car-cells@ swap car-length@ 0 ?DO     \ Loop the array
    2dup @ = IF                             \ If contents is the cell Then
      2drop                                 \   Return index and exit
      I unloop exit
    THEN
    
    cell +
  LOOP
  2drop -1                                  \ Not found, return -1
;


: car-has?         ( w w:car - f = Check if the cell is present in the array )
  car-find 0>=
;


: car-execute      ( ... xt w:car - ... = Execute the token for every cell in the array )
  dup car-cells@ swap car-length@ 0 ?DO     \ Loop the array
    2dup 2>r                                \ Clear the stack
    @ swap execute                          \ Execute the token with the array contents
    2r>
    cell +
  LOOP
  2drop
;



( Inspection )

: car-dump         ( w:car - = Dump the cell array )
  ." car:" dup . cr
  ."  length:" dup car>length ? cr
  ."  size  :" dup car>size   ? cr
  ."  extra :" dup car>extra  ? cr
  ."  cells :" ['] . swap car-execute 
;

[THEN]

\ ==============================================================================
