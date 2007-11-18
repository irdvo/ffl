\ ==============================================================================
\
\              dos - the datetime output stream in the ffl
\
\               Copyright (C) 2007  Dick van Oudheusden
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
\  $Date: 2007-11-18 19:04:04 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] dos.version [IF]

include ffl/tos.fs
include ffl/dtm.fs


( dos = Datetime output stream )
( The dos module implements words for formatting date and time in a string.  )
( It is built upon the tos structure. If the tos structure contains a        )
( message catalog, it is used for locale weekday and months names. The       )
( format word uses most of the same conversion characters as the strftime    )
( c-function.                                                                )


1 constant dos.version


( Date and time writing words )

: dos-write-abbr-weekday-name   ( w:dtm w:tos - = Write the abbreviated weekday name, using the catalog for locale )
;


: dos-write-full-weekday-name   ( w:dtm w:tos - = Write the full weekday name, using the catalog for locale )
;


: dos-write-abbr-month-name   ( w:dtm w:tos - = Write the abbreviated month name, using the catalog for locale )
;


: dos-write-full-month-name   ( w:dtm w:tos - = Write the full month name, using the catalog for locale )
;


: dos-write-date-time   ( w:dtm w:tos - = Write the preferred time and date representation for the locale, else yyyy/mm/dd hh:mm:ss)
;


: dos-write-century   ( w:dtm w:tos - = Write the century number )
;


: dos-write-monthday0   ( w:dtm w:tos - = Write the day of the month: 01..31, zero padded )
;


: dos-write-american-date   ( w:dtm w:tos - = Write the date in mm/dd/yy format )
;


: dos-write-monthday   ( w:dtm w:tos - = Write the day of the month,  1..31, space padded)
;

: dos-write-iso8601-date   ( w:dtm w:tos - = Write the date in ISO 8601 format: yyyy-mm-dd )
;


: dos-write-24hour0   ( w:dtm w:tos - = Write the hour using a 24-hour clock: 00..23 )
;


: dos-write-12hour0   ( w:dtm w:tos - = Write the hour using a 12-hour clock: 01..12 )
;


: dos-write-yearday   ( w:dtm w:tos - = Write the day of the year: 001..366 )
;


: dos-write-24hour   ( w:dtm w:tos - = Write the hour using a 24-hour clock:  0..23, space padded )
;


: dos-write-12hour   ( w:dtm w:tos - = Write the hour using a 12-hour clock:  1..12, space padded )
;


: dos-write-month   ( w:dtm w:tos - = Write the month: 01..12)
;


: dos-write-minute   ( w:dtm w:tos - = Write the minute: 00..59)
;


: dos-write-ampm   ( w:dtm w:tos - = Write the am or pm notation, using the catalog for locale )
;


: dos-write-upper-ampm   ( w:dtm w:tos - = Write the AM or PM notation, using the catalog for locale )
;


: dos-write-ampm-time   ( w:dtum w:tos - = Write the time in ampm notation: hh:mm:ss ?m, using the catalog for locale)
;


: dos-write-hhmm-time   ( w:dtm w:tos - = Write the time: hh:mm)
;


: dos-write-seconds-since-epoch  ( w:dtm w:tos - = Write the number of seconds since 1970-01-01 00:00:00)
;


: dos-write-seconds   ( w:dtm w:tos - = Write the number of seconds: 00..61)
;


: dos-write-hhmmss-time   ( w:dtm w:tos - = Write the time: hh:mm:ss)
;



: dos-write-weekday   ( w:dtm w:tos - = Write the weekday: 0..6, 0 = sunday )
;


: dos-write-week-number   ( w:dtm w:tos - = Write the week number: 01..53 )
;


: dos-write-date   ( w:dtm w:tos - = Write the preferred date representation for the locale, else yyyy/mm/dd )
;


: dos-write-time   ( w:dtm w:tos - = Write the preferred time representation for the locale, else hh:mm:ss )
;


: dos-write-2year    ( w:dtm w:tos - = Write the year without the century: 00..99)
;


: dos-write-year   ( w:dtm w:tos - = Write the year including the century )
;


( Private formatting words )

: dos-nothing   ( w:dtm w:tos - = Not a valid conversion character )
  2drop
;

create dos.jmp   
  ' dos-write-abbr-weekday-name ,   \ a
  ' dos-write-abbr-month-name ,     \ b
  ' dos-write-date-time ,           \ c
  ' dos-write-monthday0 ,           \ d
  ' dos-write-monthday ,            \ e
  ' dos-nothing ,                   \ f
  ' dos-nothing ,                   \ g
  ' dos-write-abbr-month-name ,     \ h
  ' dos-nothing ,                   \ i
  ' dos-write-yearday ,             \ j
  ' dos-write-24hour ,              \ k
  ' dos-write-12hour ,              \ l
  ' dos-write-month ,               \ m
  ' dos-nothing ,                   \ n
  ' dos-nothing ,                   \ o
  ' dos-write-upper-ampm ,          \ p
  ' dos-nothing ,                   \ q
  ' dos-write-ampm-time ,           \ r
  ' dos-write-seconds-since-epoch , \ s
  ' dos-nothing ,                   \ t
  ' dos-nothing ,                   \ u
  ' dos-nothing ,                   \ v
  ' dos-write-weekday ,             \ w
  ' dos-write-date ,                \ x
  ' dos-write-2year ,               \ y
  ' dos-nothing ,                   \ z
  
  
: dos+jmp   ( .. c - .. = Execute the word related to the formatting conversion character 'a'..'Z' )
  [char] 'a' - cells + @ execute
;

( Date and time formatting word )

: dos-format   ( w:dtm c-addr u w:tos - = Write a formatted date and time in the stream )
;

[THEN]

\ ==============================================================================
