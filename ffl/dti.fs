\ ==============================================================================
\
\                 dti - the date time iterator in the ffl
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
\  $Date: 2006-06-01 18:45:52 $ $Revision: 1.1 $
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
  dup dtm-year@
  1- swap dtm-year!  \ ToDo: leap year ??
;


: dti-year+        ( w:dtm - = Increase the date/time with one year )
  dup dtm-year@
  1+ swap dtm-year!
;


: dti-month-       ( w:dtm - = Decrease the date/time with one months )
  dup dtm-month@
  1- dup 1 < IF
    over dti-year-
    drop 12
  THEN
  swap dtm-month!
;

  
: dti-month+       ( w:dtm - = Increase the date/time with one months )
  dup dtm-month@
  1+ dup 12 > IF
    over dti-year+
    drop 1
  THEN
  swap dtm-month!
;


: dti-day-         ( w:dtm - = Decrease the date/time with one day )
  dup dtm-day@
  1- dup 1 < IF
    over dti-month-
    drop 31                  \ ToDo: beter check
  THEN
  swap dtm-day!
;


: dti-days-        ( n:days w:dtm - = Decrease the date/time with n days )
  \ ToDo
;


: dti-day+         ( w:dtm - = Increase the date/time with one day )
  dup dtm-day@
  1+ dup 31 > IF             \ ToDo: beter check
    over dti-month+
    drop 1
  THEN
  swap dtm-day!
;


: dti-days+        ( n:days w:dtm - = Increase the date/time with n days )
  \ ToDo
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


: dti-seconds-     ( d:seconds w:dtm - = Decrease the date/time with d seconds )
  \ ToDo
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
  \ ToDo
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
