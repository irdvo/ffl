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
\  $Date: 2006-06-08 19:33:34 $ $Revision: 1.5 $
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
  31 , 28 , 31 , 30 , 31  , 30  , 31  , 31  , 30  , 31  , 30  , 31  ,
does>              ( n:month - n:days = Get the number of days in a month )
  swap 1- cells + @
;
create dtm.days-till-month
  0 ,  31 , 59 , 90 , 120 , 151 , 181 , 212 , 243 , 273 , 304 , 334 ,
does>
  swap 1- cells + @
;

create dtm.month-offsets  
  0 ,  3 ,  3  ,  6 ,  1  ,  4  ,  6  ,  2  ,  5  ,  0  ,  3  ,  5  ,
does>              ( n:month - n:offset = Get the month offset for week day calculation )
  swap 1- cells + @
;


( Constants )

1970 constant dtm.unix-epoch  ( - n = Unix epoch [1970] )

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


: dtm-create       ( C: "name" - R: - w:dtm = Create a date/time structure with the current date/time )
  create  here  dtm% allot  dtm-init
;


: dtm-new          ( - w:dtm = Allocate a date/time structure with the current date/time on the heap )
  dtm% allocate throw  dup dtm-init
;


: dtm-free         ( w:dtm - = Free the structure from the heap )
  free throw
;



( Module words )

: dtm+leap-year?   ( n:year - f = Check if the year is a leap year )
  dup  400 mod 0=
  over 100 mod 0<> OR
  swap 4   mod 0=  AND
;


: dtm+calc-leap-years  ( n:end n:start - n = Calculate the number of leap years in a year range )
  1-
  over 100 / over 100 / - 
  >r
  over 4   / over 4   / - 
  r> - >r
  swap 400 / swap 400 / -
  r> +
;


: dtm+days-in-month  ( n:month n:year - n = Get the number of days in a month )
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


: dtm+days-till-month  ( n:month n:year - n = Get the number of days till the month )
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


: dtm+milli?       ( n:millis - f = Check if milliseconds are valid )
  0 1000 within
;


: dtm+second?      ( n:seconds - f = Check if seconds are valid )
  0 60 within
;


: dtm+minute?      ( n:minutes - f = Check if minutes are valid )
  0 60 within
;


: dtm+hour?        ( n:hour - f = Check if hours are valid )
  0 24 within
;


: dtm+day?         ( n:day n:month n:year - f = Check if the day is valid )
  dtm+days-in-month 1+ 1 swap within  
;


: dtm+month?       ( n:month - f = Check if the month is valid )
  1 13 within
;


: dtm+year?        ( n:year - f = Check if the year [>1582] is valid )
  1582 >
;



( Member words )

: dtm-milli@       ( w:dtm - n = Get the milliseconds )
  dtm>milli @
;


: dtm-milli!       ( n w:dtm - = Set the milliseconds )
  swap 
  dup dtm+milli?   0= exp-invalid-parameters AND throw
  swap dtm>milli !
;


: dtm-second@      ( w:dtm - n = Get the seconds )
  dtm>second @
;


: dtm-second!      ( n w:dtm - = Set the seconds )
  swap 
  dup dtm+second?   0= exp-invalid-parameters AND throw
  swap dtm>second !
;


: dtm-minute@      ( w:dtm - n = Get the minutes )
  dtm>minute @
;


: dtm-minute!      ( n w:dtm - = Set the minutes )
  swap 
  dup dtm+minute?   0= exp-invalid-parameters AND throw
  swap dtm>minute !
;


: dtm-hour@        ( w:dtm - n = Get the hour )
  dtm>hour @
;


: dtm-hour!        ( n w:dtm - = Set the hour )
  swap 
  dup dtm+hour?   0= exp-invalid-parameters AND throw
  swap dtm>hour !
;


: dtm-day@         ( w:dtm - n = Get the day )
  dtm>day @
;


: dtm-day!         ( n w:dtm - = Set the day )
  >r 
  dup r@ dtm>month @ r@ dtm>year @ dtm+day?  0= exp-invalid-parameters AND throw
  r> dtm>day !
;


: dtm-month@       ( w:dtm - n = Get the month )
  dtm>month @
;


: dtm-month!       ( n w:dtm - = Set the month )
  swap 
  dup dtm+month?   0= exp-invalid-parameters AND throw
  swap dtm>month !
;


: dtm-year@        ( w:dtm - n = Get the year )
  dtm>year @
;


: dtm-year!        ( n w:dtm - = Set the year )
  swap 
  dup dtm+year?   0= exp-invalid-parameters AND throw
  swap dtm>year !
;


( Set words )

: dtm-set          ( n:mili n:sec n:min n:hour n:day n:month n:year w:dtm - = Set the date/time )
  >r
  r@ dtm-year!
  r@ dtm-month!
  r@ dtm-day!
  r@ dtm-hour!
  r@ dtm-minute!
  r@ dtm-second!
  r> dtm-milli!
;


: dtm-set-now      ( w:dtm - = Set the date time with the current date/time )
  >r
  0 time&date
  r> dtm-set
;


: dtm-set-with-days  ( d:days n:epoch w:dtm - = Set the date with days since epoch )
  \ ToDo
;


: dtm-set-with-seconds ( d:secs n:epoch w:dtm - = Set the date/time with seconds since epoch )
  \ ToDo
;


( Get words )

: dtm-get          ( w:dtm - n:mili n:sec n:min n:hour n:day n:month n:year = Get the date/time )
  >r
  r@ dtm-milli@
  r@ dtm-second@
  r@ dtm-minute@
  r@ dtm-hour@
  r@ dtm-day@
  r@ dtm-month@
  r> dtm-year@
;


( Special calculations )

: dtm-calc-weekday  ( w:dtm - n = Calculate the week day from the date )
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


: dtm-calc-daynumber   ( w:dtm - n = Calculate the day number [in the year] from the date )
  dup dtm-month@ over dtm-year@ 
  dtm+days-till-month
  swap dtm-day@ +
;


: dtm-calc-weeknumber  ( w:dtm - n = Calculate the week number from the date )
  \ ToDo
;


: dtm-calc-days-since-epoch  ( n:epoch w:dtm - d = Calculate the number of days since epoch from the date )
  >r
  r@ dtm-year@ swap
  2dup - 365 m*                           \ days for the years
  2swap dtm+calc-leap-years m+            \ add extra days for leap years
  r@ dtm-month@ r@ dtm-year@ 
  dtm+days-till-month m+                  \ add days for the months
  r> dtm-day@ 1- m+                       \ add days for the day
;


: dtm-calc-seconds-since-epoch ( n:epoch w:dtm - d = Calculate the number of seconds since epoch from the date/time )
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

: dtm-compare      ( n:mili n:sec n:min n:hour n:day n:month n:year w:dtm - n = Compare the date/time )
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


: dtm^compare      ( w:dtm w:dtm - n = Compare two date/times )
  >r dtm-get r> dtm-compare
;


( Inspection )

: dtm-dump         ( w:dtm - = Dump the date/time structure )
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
