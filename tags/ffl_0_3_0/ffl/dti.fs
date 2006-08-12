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
\  $Date: 2006-06-21 19:24:40 $ $Revision: 1.3 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] dti.version [IF]


include ffl/dtm.fs


( dti = Date time iterator module )
( The dti module implements words for iterating date and time.  )
( This module extends the dtm module with extra words.          )


1 constant dti.version



( Public words )

: dti-year-        ( w:dtm - = Decrease the date/time with one year )
  >r
  r@ dtm-year@ 1-
  r@ dtm-day@ over
  r@ dtm-month@ swap
  dtm+days-in-month min
  r@ dtm-day!
  r> dtm-year!
;


: dti-year+        ( w:dtm - = Increase the date/time with one year )
  >r
  r@ dtm-year@ 1+
  r@ dtm-day@ over
  r@ dtm-month@ swap
  dtm+days-in-month min
  r@ dtm-day!
  r> dtm-year!
;


: dti-month-       ( w:dtm - = Decrease the date/time with one months )
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

  
: dti-month+       ( w:dtm - = Increase the date/time with one months )
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


: dti-day-         ( w:dtm - = Decrease the date/time with one day )
  >r
  r@ dtm-day@
  1- dup 1 < IF
    r@ dti-month-
    drop r@ dtm-month@ r@ dtm-year@ dtm+days-in-month
  THEN
  r> dtm-day!
;


: dti-day+         ( w:dtm - = Increase the date/time with one day )
  >r
  r@ dtm-day@
  1+ dup r@ dtm-month@ r@ dtm-year@ dtm+days-in-month > IF
    r@ dti-month+
    drop 1
  THEN
  r> dtm-day!
;


: dti-days+        ( d:days w:dtm - = Increase the date/time with d days )
  >r
  dtm.start-epoch r@ dtm-calc-days-since-epoch
  d+
  dtm.start-epoch r> dtm-set-with-days
;


: dti-days-        ( d:days w:dtm - = Decrease the date/time with d days )
  >r dnegate r> dti-days+
;


: dti-hour-        ( w:dtm - = Decrease the date/time with one hour )
  dup dtm-hour@
  1- dup 0< IF
    over dti-day-
    drop 23
  THEN
  swap dtm-hour!
;


: dti-hour+        ( w:dtm - = Increase the date/time with one hour )
  dup dtm-hour@
  1+ dup 23 > IF
    over dti-day+
    drop 0
  THEN
  swap dtm-hour!
;


: dti-minute-      ( w:dtm - = Decrease the date/time with one minute )
  dup dtm-minute@
  1- dup 0< IF
    over dti-hour-
    drop 59
  THEN
  swap dtm-minute!
;


: dti-minute+      ( w:dtm - = Increase the date/time with one minute )
  dup dtm-minute@
  1+ dup 59 > IF
    over dti-hour+
    drop 0
  THEN
  swap dtm-minute!
;


: dti-second-      ( w:dtm - = Decrease the date/time with one second )
  dup dtm-second@
  1- dup 0< IF
    over dti-minute-
    drop 59
  THEN
  swap dtm-second!
;


: dti-second+      ( w:dtm - = Increase the date/time with one second )
  dup dtm-second@
  1+ dup 59 > IF
    over dti-minute+
    drop 0
  THEN
  swap dtm-second!
;


: dti-seconds+     ( d:seconds w:dtm - = Increase the date/time with d seconds )
  >r
  dtm.start-epoch r@ dtm-calc-seconds-since-epoch
  d+
  dtm.start-epoch r> dtm-set-with-seconds
;


: dti-seconds-     ( d:seconds w:dtm - = Decrease the date/time with d seconds )
  >r dnegate r> dti-seconds+
;


: dti-milli+       ( w:dtm - = Increase the date/time with one millisecond )
  dup dtm-milli@
  1+ dup 999 > IF
    over dti-second+
    drop 0
  THEN
  swap dtm-milli!
;


: dti-milli-       ( w:dtm - = Decrease the date/time with one millisecond )
  dup dtm-milli@
  1- dup 0< IF
    over dti-second-
    drop 999
  THEN
  swap dtm-milli!
;


[THEN]

\ ==============================================================================
