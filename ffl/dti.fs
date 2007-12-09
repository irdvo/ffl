\ ==============================================================================
\
\               dti - the date time iterator in the ffl
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
\  $Date: 2007-12-09 07:23:15 $ $Revision: 1.5 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] dti.version [IF]


include ffl/dtm.fs


( dti = Date time iterator module )
( The dti module implements a Date and time iterator. This module extends    )
( the dtm data type with extra words.                                        )


1 constant dti.version



( Year iterator words )

: dti-year-        ( dtm -- = Decrease the date/time with one year )
  >r
  r@ dtm-year@ 1-
  r@ dtm-day@ over
  r@ dtm-month@ swap
  dtm+days-in-month min
  r@ dtm-day!
  r> dtm-year!
;


: dti-year+        ( dtm -- = Increase the date/time with one year )
  >r
  r@ dtm-year@ 1+
  r@ dtm-day@ over
  r@ dtm-month@ swap
  dtm+days-in-month min
  r@ dtm-day!
  r> dtm-year!
;


( Month iterator words )

: dti-month-       ( dtm -- = Decrease the date/time with one months )
  >r 
  r@ dtm-month@
  1- dup 1 < IF         \ ToDo: Day valid?
    r@ dti-year-
    drop 12
  THEN
  r@ dtm-day@ over                     \ Limit to number of days in new month
  r@ dtm-year@ dtm+days-in-month min
  r@ dtm-day!
  r> dtm-month!
;

  
: dti-month+       ( dtm -- = Increase the date/time with one months )
  >r 
  r@ dtm-month@
  1+ dup 12 > IF       
    r@ dti-year+
    drop 1
  THEN
  r@ dtm-day@ over                     \ Limit to number of days in new month
  r@ dtm-year@ dtm+days-in-month min
  r@ dtm-day!
  r> dtm-month!
;


( Day iterator words )

: dti-day-         ( dtm -- = Decrease the date/time with one day )
  >r
  r@ dtm-day@
  1- dup 1 < IF
    r@ dti-month-
    drop r@ dtm-month@ r@ dtm-year@ dtm+days-in-month
  THEN
  r> dtm-day!
;


: dti-day+         ( dtm -- = Increase the date/time with one day )
  >r
  r@ dtm-day@
  1+ dup r@ dtm-month@ r@ dtm-year@ dtm+days-in-month > IF
    r@ dti-month+
    drop 1
  THEN
  r> dtm-day!
;


: dti-days+        ( d dtm -- = Increase the date/time with d days )
  >r
  dtm.start-epoch r@ dtm-calc-days-since-epoch
  d+
  dtm.start-epoch r> dtm-set-with-days
;


: dti-days-        ( d dtm -- = Decrease the date/time with d days )
  >r dnegate r> dti-days+
;


( Hour iterator words )

: dti-hour-        ( dtm -- = Decrease the date/time with one hour )
  dup dtm-hour@
  1- dup 0< IF
    over dti-day-
    drop 23
  THEN
  swap dtm-hour!
;


: dti-hour+        ( dtm -- = Increase the date/time with one hour )
  dup dtm-hour@
  1+ dup 23 > IF
    over dti-day+
    drop 0
  THEN
  swap dtm-hour!
;


( Minute iterator words )

: dti-minute-      ( dtm -- = Decrease the date/time with one minute )
  dup dtm-minute@
  1- dup 0< IF
    over dti-hour-
    drop 59
  THEN
  swap dtm-minute!
;


: dti-minute+      ( dtm -- = Increase the date/time with one minute )
  dup dtm-minute@
  1+ dup 59 > IF
    over dti-hour+
    drop 0
  THEN
  swap dtm-minute!
;


( Seconds iterator words )

: dti-second-      ( dtm -- = Decrease the date/time with one second )
  dup dtm-second@
  1- dup 0< IF
    over dti-minute-
    drop 59
  THEN
  swap dtm-second!
;


: dti-second+      ( dtm -- = Increase the date/time with one second )
  dup dtm-second@
  1+ dup 59 > IF
    over dti-minute+
    drop 0
  THEN
  swap dtm-second!
;


: dti-seconds+     ( d dtm -- = Increase the date/time with d seconds )
  >r
  dtm.start-epoch r@ dtm-calc-seconds-since-epoch
  d+
  dtm.start-epoch r> dtm-set-with-seconds
;


: dti-seconds-     ( d dtm -- = Decrease the date/time with d seconds )
  >r dnegate r> dti-seconds+
;


( Milliseconds iterator words )

: dti-milli+       ( dtm -- = Increase the date/time with one millisecond )
  dup dtm-milli@
  1+ dup 999 > IF
    over dti-second+
    drop 0
  THEN
  swap dtm-milli!
;


: dti-milli-       ( dtm -- = Decrease the date/time with one millisecond )
  dup dtm-milli@
  1- dup 0< IF
    over dti-second-
    drop 999
  THEN
  swap dtm-milli!
;

[THEN]

\ ==============================================================================
