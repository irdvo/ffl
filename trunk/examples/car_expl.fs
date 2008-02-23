\ ==============================================================================
\
\          car_expl - the cell array collection example in the ffl
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
\  $Date: 2008-02-23 06:40:17 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/car.fs


\ Example: simple numeric values in an array


\ Create a dynamic cell array in the dictionary with an initial size of 10

10 car-create values


\ Put values in the array

 7  0 values car-set                             \ Put value 7 on index 0 in values
 1  1 values car-set
 9  2 values car-set
 2  3 values car-set
 4  4 values car-set
 6  5 values car-set
 3  6 values car-set
 5  7 values car-set
 8  8 values car-set
 0 -1 values car-set                             \ Put value 0 on index 9 in values

10  values car-prepend                           \ Prepend value 10 at the start of the array
11  values car-append                            \ Append value 11 at the end of the array

12 5 values car-insert                           \ Insert value 12 on index 5 in the array


\ Get values from the array

.( Value on index 7: ) 7 values car-get . cr     \ Get the value on index 7 (6)

.( Delete second: ) 1 values car-delete . cr     \ Delete the value on index 1 and print this (7)

.( Length: ) values car-length@ . cr             \ Print the length of the array (12)

6 values car-has? [IF]                           \ Check the presence of a value in the array
  .( Value 6 is in the array.) cr
[ELSE]
  .( Value 6 is not in the array.) cr
[THEN]

9 values car-find dup -1 <> [IF]                 \ Find the index of a value in the array
  .( Value 9 is in the array on index: ) . cr
[ELSE]
  drop
  .( Value 9 is not in the array.) cr
[THEN]

values car-sort                                  \ Sort the values in the array

.( Values: ) ' . values car-execute cr           \ Print all values in the array



\ Example 2: store references to date/times in the array

include ffl/dtm.fs


\ Allocate a dynamic array with an initial size of 5 on the heap

5 car-new value dates


\ Create and add five dates to the array

dtm-new 0 dates car-set
dtm-new 1 dates car-set
dtm-new 2 dates car-set
dtm-new 3 dates car-set
dtm-new 4 dates car-set


\ Set the dates

23 dtm.february 2008  0 dates car-get  dtm-set-date   \ saturday
 3 dtm.march    2008  1 dates car-get  dtm-set-date   \ monday
24 dtm.january  2008  2 dates car-get  dtm-set-date   \ thursday
14 dtm.april    2008  3 dates car-get  dtm-set-date   \ monday
 4 dtm.may      2008  4 dates car-get  dtm-set-date   \ sunday

 
\ Sort the dates based on week day

: dates-compare  ( dtm1 dtm2 -- n = Compare two dates for week day )
  dtm-weekday swap dtm-weekday swap <=>
;

' dates-compare dates car-compare!                   \ Set the compare word for sorting

dates car-sort                                       \ Sort the array using the dates-compare word


\ Print the dates

: dates-print  ( dtm -- = Print the date )
  >r ." Day:" r@ dtm-day@ 2 .r ."  Month:" r@ dtm-month@ 2 .r ."  Year:" r> dtm-year@ . cr
;

' dates-print dates car-execute                      \ Print all the dates sorted on week day in the array


\ Free the dates from the heap

' dtm-free dates car-execute


\ Free the array from the heap

dates car-free

