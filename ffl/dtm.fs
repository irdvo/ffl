\ ==============================================================================
\
\                 dtm - the date time module in the ffl
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
\  $Date: 2006-05-29 19:04:01 $ $Revision: 1.2 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] dtm.version [IF]


include ffl/stc.fs


( dtm = Date time module )
( The dtm module implements words for using date and time. )


1 constant dtm.version


( Public structure )

struct: dtm%       ( - n = Get the required space for the dtm data structure )
  cell:  dtm>year
  cell:  dtm>month
  cell:  dtm>day
  cell:  dtm>hour
  cell:  dtm>minute
  cell:  dtm>second
  cell:  dtm>milli
;struct



( Private database )

create dtm.days-in-month 
  31 , 28 , 31 , 30 , 31 , 30 , 31 , 31 , 30 , 31 , 30 , 31 ,
does>              ( n:month - n:days = Get the number of days in a month )
  swap 1- cells + @
;
  
create dtm.month-offsets  
  0 ,  3 ,  3 ,  6 ,  1 ,  4 ,  6 ,  2 ,  5 ,  0 ,  3 ,  5 ,
does>              ( n:month - n:offset = Get the month offset for week day calculation )
  swap 1- cells + @
;


( Constants )

0  constant dtm.sunday     ( - n = Sunday )
1  constant dtm.monday     ( - n = Monday )
2  constant dtm.tuesday    ( - n = Tuesday )
3  constant dtm.wednesday  ( - n = Wednesday )
4  constant dtm.thursday   ( - n = Thursday )
5  constant dtm.friday     ( - n = Friday )
6  constant dtm.saturday   ( - n = Saturday )

1  constant dtm.january    ( - n = January )
2  constant dtm.february   ( - n = February )
3  constant dtm.march      ( - n = March )
4  constant dtm.april      ( - n = April )
5  constant dtm.may        ( - n = May )
6  constant dtm.june       ( - n = June )
7  constant dtm.july       ( - n = July )
8  constant dtm.august     ( - n = August )
9  constant dtm.september  ( - n = September )
10 constant dtm.october    ( - n = October )
11 constant dtm.november   ( - n = November )
12 constant dtm.december   ( - n = December )



( Public words )

: dtm-init         ( w:dtm - = Initialise the structure with the current date and time )
  >r
  time&date
  r@ dtm>year !
  r@ dtm>month !
  r@ dtm>day !
  r@ dtm>hour !
  r@ dtm>minute !
  r@ dtm>second !
  0 r> dtm>milli !
;


: dtm-create       ( C: "name" - R: - w:dtm = Create a date time structure with the current date and time )
  create  here  dtm% allot  dtm-init
;


: dtm-new          ( - w:dtm = Create a date time structure with the current date and time )
  dtm% allocate throw  dup >r dtm-init r> 
;


: dtm-free         ( w:dtm - = Free the structure from the heap )
  free throw
;



( Module words )

: dtm+leap-year?    ( year - f = Check if the year is a leap year )
  dup  400 mod 0=
  over 100 mod 0<> OR
  swap 4   mod 0=  AND
;


: dtm+days-in-month ( n:month n:year - n = Get the number of days in a month )
  over 1 13 within 0= exp-invalid-parameters AND throw
  
  over dtm.days-in-month
  
  -rot swap dtm.february = IF 
    dtm+leap-year? IF
      1+
    THEN
  ELSE
    drop
  THEN
;


: dtm+milli?        ( n - f = Check if milliseconds are valid )
  0 1000 within
;


: dtm+second?       ( n - f = Check if seconds are valid )
  0 60 within
;


: dtm+minute?       ( n - f = Check if minutes are valid )
  0 60 within
;


: dtm+hour?         ( n - f = Check if hours are valid )
  0 24 within
;


: dtm+day?          ( n:years n:months n:day - f = Check if the day is valid )
  \ Todo
;


: dtm+month?        ( n:year n:month - f = Check if the month is valid )
  \ ToDo
;


: dtm+year?        ( n:year - f = Check if the year is valid )
  1970 >=
;


: dtm+count-years  ( w:dtm w:dtm - n = Count the number of years between two dates )
  \ ToDo
;


: dtm+count-months ( w:dtm w:dtm - n = Count the number of months between two dates )
  \ ToDo
;


: dtm+count-days   ( w:dtm w:dtm - n = Count the number of days between two dates )
  \ ToDo
;


: dtm+count-hours  ( w:dtm w:dtm - n = Count the number of hours between two dates )
  \ ToDo
;


: dtm+count-minutes  ( w:dtm w:dtm - n = Count the number of minutes between two dates )
  \ ToDo
;


: dtm+count-seconds  ( w:dtm w:dtm - n = Count the number of seconds between two dates )
  \ ToDo
;



( Member words )

: dtm-milli@       ( w:dtm - n = Get the milliseconds )
  dtm>milli @
;


: dtm-milli!       ( n w:dtm - = Set the milliseconds )
  swap 0 max 999 min swap dtm>milli !
;


: dtm-second@      ( w:dtm - n = Get the seconds )
  dtm>second @
;


: dtm-second!      ( n w:dtm - = Set the seconds )
  swap 0 max 59 min swap dtm>second !
;


: dtm-minute@      ( w:dtm - n = Get the minutes )
  dtm>minute @
;


: dtm-minute!      ( n w:dtm - = Set the minutes )
  swap 0 max 59 min swap dtm>minute !
;


: dtm-hour@        ( w:dtm - n = Get the hours )
  dtm>hour @
;


: dtm-hour!        ( n w:dtm - = Set the hours )
  swap 0 max 23 min swap dtm>hour !
;


: dtm-day@         ( w:dtm - n = Get the days )
  dtm>day @
;


: dtm-day!         ( n w:dtm - = Set the days )
  swap 0 max 31 min swap dtm>day ! \ ToDo: more checks ..
;


: dtm-month@       ( w:dtm - n = Get the months )
  dtm>month @
;


: dtm-month!       ( n w:dtm - = Set the months )
  swap 0 max 12 min swap dtm>month !
;


: dtm-year@        ( w:dtm - n = Get the years )
  dtm>year @
;


: dtm-year!        ( n w:dtm - n = Set the years )
  swap 1970 max swap dtm>year !
;



( Iterator words )

: dtm-year-        ( w:dtm - = Decrease the date/time with one year )
  dup dtm-year@
  1- swap dtm-year!  \ ToDo: leap year ??
;


: dtm-year+        ( w:dtm - = Increase the date/time with one year )
  dup dtm-year@
  1+ swap dtm-year!
;


: dtm-month-       ( w:dtm - = Decrease the date/time with one months )
  dup dtm-month@
  1- dup 1 < IF
    over dtm-year-
    drop 12
  THEN
  swap dtm-month!
;

  
: dtm-month+       ( w:dtm - = Increase the date/time with one months )
  dup dtm-month@
  1+ dup 12 > IF
    over dtm-year+
    drop 1
  THEN
  swap dtm-month!
;


: dtm-day-         ( w:dtm - = Decrease the date/time with one day )
  dup dtm-day@
  1- dup 1 < IF
    over dtm-month-
    drop 31                  \ ToDo: beter check
  THEN
  swap dtm-day!
;


: dtm-day+         ( w:dtm - = Increase the date/time with one day )
  dup dtm-day@
  1+ dup 31 > IF             \ ToDo: beter check
    over dtm-month+
    drop 1
  THEN
  swap dtm-day!
;


: dtm-hour-        ( w:dtm - = Decrease the date/time with one hour )
  dup dtm-hour@
  1- dup 0< IF
    over dtm-day-
    drop 23
  THEN
  swap dtm-hour!
;


: dtm-hour+        ( w:dtm - = Increase the date/time with one hour )
  dup dtm-hour@
  1+ dup 23 > IF
    over dtm-day+
    drop 0
  THEN
  swap dtm-hour!
;


: dtm-minute-      ( w:dtm - = Decrease the date/time with one minute )
  dup dtm-minute@
  1- dup 0< IF
    over dtm-hour-
    drop 59
  THEN
  swap dtm-minute!
;


: dtm-minute+      ( w:dtm - = Increase the date/time with one minute )
  dup dtm-minute@
  1+ dup 59 > IF
    over dtm-hour+
    drop 0
  THEN
  swap dtm-minute!
;


: dtm-second-      ( w:dtm - = Decrease the date/time with one second )
  dup dtm-second@
  1- dup 0< IF
    over dtm-minute-
    drop 59
  THEN
  swap dtm-second!
;


: dtm-second+      ( w:dtm - = Increase the date/time with one second )
  dup dtm-second@
  1+ dup 59 > IF
    over dtm-minute+
    drop 0
  THEN
  swap dtm-second!
;


: dtm-milli+       ( w:dtm - = Increase the date/time with one millisecond )
  dup dtm-milli@
  1+ dup 999 > IF
    over dtm-second+
    drop 0
  THEN
  swap dtm-milli!
;


: dtm-milli-       ( w:dtm - = Decrease the date/time with one millisecond )
  dup dtm-milli@
  1- dup 0< IF
    over dtm-second-
    drop 999
  THEN
  swap dtm-milli!
;



( Set words )

: dtm-set          ( n:mili n:sec n:min n:hour n:day n:month n:year w:dtm - = Set the date and time )
  >r
  r@ dtm-year!
  r@ dtm-month!
  r@ dtm-day!
  r@ dtm-hour!
  r@ dtm-minute!
  r@ dtm-second!
  r> dtm-milli!
;

: dtm-set-now      ( w:dtm - = Set the date time with the current date time )
  >r
  0 time&date
  r> dtm-set
;


( Get words )

: dtm-get          ( w:dtm - n:mili n:sec n:min n:hour n:day n:month n:year = Get the date and time )
  >r
  r@ dtm-milli@
  r@ dtm-second@
  r@ dtm-minute@
  r@ dtm-hour@
  r@ dtm-day@
  r@ dtm-month@
  r> dtm-year@
;


: dtm-weekday  ( w:dtm - n = Get the week day for the date )
  >r
  3  
  r@ dtm-year@ 100 / 4 mod - 2*              \   (3 - (century mod 4)) * 2
  r@ dtm-year@ 100 mod dup 4 / +             \   (year + year / 4)
  r@ dtm-year@ dtm+leap-year? IF
    r@ dtm-month@ dtm.february <= IF
      1-                                     \   change year offset for jan/feb in leap year
    THEN
  THEN
  7 mod +                                    \ + year-offset mod 7
  r@ dtm-month@ dtm.month-offsets +          \ + (month-offset[month])
  r> dtm-day@ 7 mod +                        \ + day mod 7
  7 mod                                      \ result mod 7
;



( Inspection )

: dtm-dump         ( w:dtm - = Dump the date time structure )
  ." dtm:" dup . cr
  ."  year  :" dup dtm>year ? cr
  ."  month :" dup dtm>month ? cr
  ."  day   :" dup dtm>day ? cr
  ."  hour  :" dup dtm>hour ? cr
  ."  minute:" dup dtm>minute ? cr
  ."  second:" dup dtm>second ? cr
  ."  milli :" dtm>milli ? cr
;

[THEN]

\ ==============================================================================
