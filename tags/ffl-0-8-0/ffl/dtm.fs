\ ==============================================================================
\
\                dtm - the date time module in the ffl
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
\  $Date: 2007-12-09 07:23:15 $ $Revision: 1.11 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] dtm.version [IF]


include ffl/stc.fs


( dtm = Date time data type )
( The dtm module implements a [gregorian] date and time data type. )


2 constant dtm.version


( Date time structure )

begin-structure dtm%       ( -- n = Get the required space for a dtm variable )
  field: dtm>year
  field: dtm>month
  field: dtm>day
  field: dtm>hour
  field: dtm>minute
  field: dtm>second
  field: dtm>milli
end-structure



( Private database )

: dtm.create-month-table
  create
  does> 
    swap 1- cells + @
;

dtm.create-month-table dtm.days-in-month    ( n1 -- n2 = Get the number of days in a month n1 )
  31 , 28 , 31 , 30 , 31  , 30  , 31  , 31  , 30  , 31  , 30  , 31  ,
  
dtm.create-month-table dtm.days-till-month  ( n1 -- n2 = Get the number of days till a month n1 )
  0 ,  31 , 59 , 90 , 120 , 151 , 181 , 212 , 243 , 273 , 304 , 334 ,

dtm.create-month-table dtm.month-offsets    ( n1 -- n2 = Get the offset for month n1 for week day calculation )
  0 ,  3 ,  3  ,  6 ,  1  ,  4  ,  6  ,  2  ,  5  ,  0  ,  3  ,  5  ,


( Constants )

1970 constant dtm.unix-epoch  ( -- n = Unix epoch [1970] )
1583 constant dtm.start-epoch ( -- n = Start epoch [1583] )

0  constant dtm.sunday     ( -- n = Sunday )
1  constant dtm.monday     ( -- n = Monday )
2  constant dtm.tuesday    ( -- n = Tuesday )
3  constant dtm.wednesday  ( -- n = Wednesday )
4  constant dtm.thursday   ( -- n = Thursday )
5  constant dtm.friday     ( -- n = Friday )
6  constant dtm.saturday   ( -- n = Saturday )

1  constant dtm.january    ( -- n = January )
2  constant dtm.february   ( -- n = February )
3  constant dtm.march      ( -- n = March )
4  constant dtm.april      ( -- n = April )
5  constant dtm.may        ( -- n = May )
6  constant dtm.june       ( -- n = June )
7  constant dtm.july       ( -- n = July )
8  constant dtm.august     ( -- n = August )
9  constant dtm.september  ( -- n = September )
10 constant dtm.october    ( -- n = October )
11 constant dtm.november   ( -- n = November )
12 constant dtm.december   ( -- n = December )



( Date time variable creation, initialisation and destruction )

: dtm-init         ( dtm -- = Initialise the date/time with the current date and time )
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


: dtm-create       ( "<spaces>name" -- ; -- dtm = Create a date/time variable in the dictionary with the current date/time )
  create  here  dtm% allot  dtm-init
;


: dtm-new          ( -- dtm = Allocate a date/time variable on the heap with the current date/time )
  dtm% allocate throw  dup dtm-init
;


: dtm-free         ( dtm -- = Free the date/time variable from the heap )
  free throw
;



( Module words )

: dtm+leap-year?   ( n -- flag = Check if the year n is a leap year )
  dup  400 mod 0=
  over 100 mod 0<> OR
  swap 4   mod 0=  AND
;


: dtm+calc-leap-years  ( n1 n2 -- n3 = Calculate the number of leap years in the year range [n2..n1] )
  1-
  over 100 / over 100 / -
  >r
  over 4   / over 4   / -
  r> - >r
  swap 400 / swap 400 / -
  r> +
;


: dtm+days-in-year   ( n1 -- n2 = Get the number of days in the year n1 )
  dtm+leap-year? IF 366 ELSE 365 THEN
;


: dtm+days-in-month  ( n1 n2 -- n3 = Get the number of days in the month n1 and year n2 )
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


: dtm+days-till-month  ( n1 n2 -- n3 = Get the number of days till the month n1 and year n2 )
  over 1 13 within 0= exp-invalid-parameters AND throw
  
  over dtm.days-till-month
  
  -rot swap dtm.february > IF 
    dtm+leap-year? IF
      1+
    THEN
  ELSE
    drop
  THEN
;


: dtm+milli?       ( n -- flag = Check if the milliseconds n are valid )
  0 1000 within
;


: dtm+second?      ( n -- flag = Check if the seconds n are valid )
  0 60 within
;


: dtm+minute?      ( n -- flag = Check if the minutes n are valid )
  0 60 within
;


: dtm+hour?        ( n -- flag = Check if the hours n are valid )
  0 24 within
;


: dtm+day?         ( n1 n2 n3 -- flag = Check if the day n1 in the month n2 and year n3 is valid )
  dtm+days-in-month 1+ 1 swap within  
;


: dtm+month?       ( n -- flag = Check if the month n is valid )
  1 13 within
;


: dtm+year?        ( n -- flag = Check if the year n [>1582] is valid )
  dtm.start-epoch >=
;



( Member words )

: dtm-milli@       ( dtm -- n = Get the milliseconds )
  dtm>milli @
;


: dtm-milli!       ( n dtm -- = Set the milliseconds )
  swap 
  dup dtm+milli?   0= exp-invalid-parameters AND throw
  swap dtm>milli !
;


: dtm-second@      ( dtm -- n = Get the seconds )
  dtm>second @
;


: dtm-second!      ( n dtm -- = Set the seconds )
  swap 
  dup dtm+second?   0= exp-invalid-parameters AND throw
  swap dtm>second !
;


: dtm-minute@      ( dtm -- n = Get the minutes )
  dtm>minute @
;


: dtm-minute!      ( n dtm -- = Set the minutes )
  swap 
  dup dtm+minute?   0= exp-invalid-parameters AND throw
  swap dtm>minute !
;


: dtm-hour@        ( dtm -- n = Get the hour )
  dtm>hour @
;


: dtm-hour!        ( n dtm -- = Set the hour )
  swap 
  dup dtm+hour?   0= exp-invalid-parameters AND throw
  swap dtm>hour !
;


: dtm-day@         ( dtm -- n = Get the day )
  dtm>day @
;


: dtm-day!         ( n dtm -- = Set the day )
  >r 
  dup r@ dtm>month @ r@ dtm>year @ dtm+day?  0= exp-invalid-parameters AND throw
  r> dtm>day !
;


: dtm-month@       ( dtm -- n = Get the month )
  dtm>month @
;


: dtm-month!       ( n dtm -- = Set the month )
  swap 
  dup dtm+month?   0= exp-invalid-parameters AND throw
  swap dtm>month !
;


: dtm-year@        ( dtm -- n = Get the year )
  dtm>year @
;


: dtm-year!        ( n dtm -- = Set the year )
  swap 
  dup dtm+year?   0= exp-invalid-parameters AND throw
  swap dtm>year !
;


( Set words )

: dtm-set-date     ( n1 n2 n3 dtm -- = Set the date with day n1, month n2 and year n3 )
  >r
  r@ dtm-year!
  r@ dtm-month!
  r> dtm-day!
;


: dtm-set-time     ( n1 n2 n3 n4 dtm -- = Set the time with milliseconds n1, seconds n2, minutes n3 and hours n4 )
  >r
  r@ dtm-hour!
  r@ dtm-minute!
  r@ dtm-second!
  r> dtm-milli!
;
  
: dtm-set          ( n1 n2 n3 n4 n5 n6 n7 dtm -- = Set the date/time with milliseconds n1, seconds n2, minutes n3, hours n4, day n5, month n6 and year n7 )
  >r
  r@ dtm-set-date
  r> dtm-set-time
;


: dtm-set-now      ( dtm -- = Set the date time with the current date/time )
  >r
  0 time&date
  r> dtm-set
;


: dtm-set-with-days  ( d1 n2 dtm -- = Set the date with days d1 since epoch n2 )
  >r >r
  BEGIN
    2dup 
    r@ dtm+days-in-year s>d 
    2swap 
    
    2over d= IF                             \ If number of days is exact days in this year Then
      d- r> 1+ >r                           \   Number of days = 0 and year++
      false
    ELSE                                    \ Else
      2over d<                              \   Found the year ?
    THEN
    
  WHILE
    2dup 366 fm/mod nip                     \ Guess the number of years
    dup 
    >r 365 m* d- r>
    r> tuck + dup >r
    swap dtm+calc-leap-years negate m+      \ Calculate the actual number of days in the guessed years
  REPEAT
  r> r@ dtm-year!

  d>s dtm.december                          \ Look for the correct month
  BEGIN
    2dup r@ dtm-year@ dtm+days-till-month <
  WHILE
    1-
  REPEAT
  
  dup r@ dtm-month!
  
  r@ dtm-year@ dtm+days-till-month -
  1+ r> dtm-day!
;


: dtm-set-with-seconds ( d n dtm -- = Set the date/time with d seconds since epoch n )
  swap >r >r
  2dup 60 fm/mod drop             \ dtm.seconds = secs % 60
  r@ dtm-second!
  1 60 m*/                        
  2dup 60 fm/mod drop             \ dtm.minutes = mins % 60
  r@ dtm-minute!
  1 60 m*/
  2dup 24 fm/mod drop             \ dtm.hours   = hours % 24
  r@ dtm-hour!
  1 24 m*/                        \ days
  r> r> swap
  dtm-set-with-days
;


( Private words )

: dtm+iso-weekday   ( n1 n2 -- n3 = Calculate the iso-weekday for weekday n2 and year n1 )
  over swap -                   \ yday - wday
  4 +                           \ + iso-week1-wday
  [ 366 7 / 2 + 7 * ] literal + \ + keep it positive
  7 mod -                       \ yday - (yday - wday + iso-week1-wday + magic * 7) % 7
  4 +                           \ + iso-week1-wday
  1 -                           \ - monday
;


( Get words )

: dtm-get          ( dtm -- n1 n2 n3 n4 n5 n6 n7 = Get the date/time, return milliseconds n1, seconds n2, minutes n3, hours n4, day n5, month n6 and year n7 )
  >r
  r@ dtm-milli@
  r@ dtm-second@
  r@ dtm-minute@
  r@ dtm-hour@
  r@ dtm-day@
  r@ dtm-month@
  r> dtm-year@
;


: dtm-get-date     ( dtm -- n1 n2 n3 = Get the date, return day n1, month n2 and year n3 )
  >r
  r@ dtm-day@
  r@ dtm-month@
  r> dtm-year@
;


: dtm-get-time     ( dtm -- n1 n2 n3 n4 = Get the time, return milliseconds n1, seconds n2, minutes n3 and hours n4 )
  >r
  r@ dtm-milli@
  r@ dtm-second@
  r@ dtm-minute@
  r> dtm-hour@
;

: dtm-weekday       ( dtm -- n = Get the week day from the date )
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


: dtm-yearday   ( dtm -- n = Get the day number [in the year] from the date )
  dup dtm-month@ over dtm-year@ 
  dtm+days-till-month
  swap dtm-day@ + 
;


: dtm-iso-weeknumber  ( dtm -- n1 n2 = Get the iso week number n1 and year n2 from the date )
  >r
  r@ dtm-year@
  r@ dtm-yearday 1-                    \ Change range from 1..366 till 365
  r> dtm-weekday
  2dup dtm+iso-weekday                 \ Calculate iso weekday
  dup 0< IF                            \ If negative than week in previous year
    drop
    >r >r 1- dup dtm+days-in-year r> + r>
    dtm+iso-weekday
  ELSE
    >r
    >r >r dup dtm+days-in-year r> swap - r>
    dtm+iso-weekday
    r> swap
    dup 0>= IF                         \ If positive than week in next year
      nip
      swap 1+ swap
    ELSE
      drop
    THEN
  THEN
  7 / 1+ swap                          \ Calculate the week number from the days
;




( Epoch words )

: dtm-calc-days-since-epoch  ( n dtm -- d = Calculate the number of days since epoch n from the date )
  >r
  r@ dtm-year@ swap
  2dup - 365 m*                           \ days for the years
  2swap dtm+calc-leap-years m+            \ add extra days for leap years
  r@ dtm-month@ r@ dtm-year@ 
  dtm+days-till-month m+                  \ add days for the months
  r> dtm-day@ 1- m+                       \ add days for the day
;


: dtm-calc-seconds-since-epoch ( n dtm -- d = Calculate the number of seconds since epoch n from the date/time )
  >r
  r@ dtm-calc-days-since-epoch            \ days
  24 1 m*/
  r@ dtm-hour@ m+                         \ hours
  60 1 m*/
  r@ dtm-minute@ m+                       \ minutes
  60 1 m*/
  r> dtm-second@ m+                       \ seconds
;


( Compare words )

: dtm-compare      ( n1 n2 n3 n4 n5 n6 n7 dtm -- n = Compare the date/time with milliseconds n1, seconds n2, minutes n3, hours n4, day n5, month n6 and year n7 )
  >r
  r@ dtm-year@ - sgn
  
  ?dup 0= IF
    r@ dtm-month@ - sgn
  ELSE
    nip
  THEN
  
  ?dup 0= IF
    r@ dtm-day@ - sgn
  ELSE
    nip
  THEN
  
  ?dup 0= IF
    r@ dtm-hour@ - sgn
  ELSE
    nip
  THEN
  
  ?dup 0= IF
    r@ dtm-minute@ - sgn
  ELSE
    nip
  THEN
  
  ?dup 0= IF
    r@ dtm-second@ - sgn
  ELSE
    nip
  THEN
  
  ?dup 0= IF
    r@ dtm-milli@ - sgn
  ELSE
    nip
  THEN
  
  rdrop
;


: dtm^compare      ( dtm1 dtm2 -- n = Compare two date/times, return the compare result [-1,0,1] )
  >r dtm-get r> dtm-compare
;


( Inspection )

: dtm-dump         ( dtm -- = Dump the date/time variable )
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
