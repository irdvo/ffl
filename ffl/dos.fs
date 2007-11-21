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
\  $Date: 2007-11-21 18:29:11 $ $Revision: 1.2 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] dos.version [IF]

include ffl/tos.fs
include ffl/dtm.fs
include ffl/stt.fs


( dos = Datetime output stream )
( The dos module implements words for formatting date and time in a string.  )
( It is built upon the tos structure. If the tos structure contains a        )
( message catalog, it is used for locale weekday and months names. The       )
( format word uses most of the same conversion characters as the strftime    )
( c-function:                                                                )
( <pre>                                                                      )
( %a - the abbreviated weekday name according to the current locale          )
( %A - the full weekday name according to the current locale                 )
( %b - the abbreviated month name according to the current locale            )
( %B - the full month name according to the current locale                   )
( %c - the preferred date and time representation for the current locale     )
( %C - the century number as a 2-digit integer                               )
( %d - the day of the month as a decimal number: 00..31                      )
( %D - equivalent to %m/%d/%y -- for Americans                               )
( %e - like %d, the day of the month as a decimal number with leading space  )
( %F - equivalent to %Y-%m-%d, the ISO 8601 date format                      )
( %h - equivalent to %b                                                      )
( %H - the hour as a decimal number using a 24-hour clock: 00..23            )
( %I - the hour as a decimal number using a 12-hour clock: 01..12            )
( %j - the day of the year as a decimal number: 001..366                     )
( %k - the hour, 24-hour clock, as a decimal number: 0..23, leading space    )
( %l - the hour, 12-hour clock, as a decimal number: 1..12, leading space    )
( %m - the month as a decimal number: 01..12                                 )
( %M - the minute as a decimal number: 00..59                                )
( %p - either 'AM' or 'PM' according to the given time value with locale     ) \  Noon is treated as 'pm' and midnight as 'am'
( %P - like %p but in lowercase: 'am' with locale                            )
( %r - the time in a.m. or p.m. notation, '%I:%M:%S %p'                      ) 
( %R - the time in 24-hour notation, %H:%M                                   )
( %s - the number of seconds since the 1970-01-01 00:00:00                   )
( %S - the second as a decimal number: 00..60                                )
( %T - the time in 24-hour notation, %H:%M:%S                                )
( %V - the ISO 8601 week number as a decimal number: 01..53                  )
( %w - the day of the week as a decimal, range 0 to 6, Sunday being 0        )
( %x - the preferred date representation for the current locale              )
( %X - the preferred time representation for the current locale              )
( %y - the year as a decimal number without a century: 00..99                )
( %Y - the year as a decimal number including the century                    )
( </pre> )

1 constant dos.version


( Private string tables )

begin-stringtable dos.weekday-names   ( n:weekday - c-addr u = Get the weekday name )
," Sunday"
," Monday"
," Tuesday"
," Wednesday"
," Thursday"
," Friday"
," Saturday"
end-stringtable

begin-stringtable dos.month-names   ( n:month - c-addr u = Get the month name )
," Januari"
," Februari"
," March"
," April"
," May"
," June"
," July"
," August"
," September"
," October"
," November"
," December"
end-stringtable


( Private words )

: dos+24to12   ( n - n = Convert hours from 24 clock to 12 clock )
  ?dup 0= IF
    12
  ELSE
    dup 12 > IF
      12 -
    THEN
  THEN
;


: dos-write-2spaced   ( w:tos n - = Write a number in two digits, with space padding )
  over tos-write-number
  bl 2 
  rot  tos-align-right
;


: dos-write-2zeroed   ( w:tos n - = Write a number in two digits, with zero padding )
  over tos-write-number
  [char] 0 2 
  rot  tos-align-right
;


: dos-translate   ( w:tos c-addr u w:tos - c-addr u = Translate a string with the optional message catalog )
  tos-msc@ dup nil<> IF
    msc-translate
  ELSE
    drop
  THEN
;


: dos-write-ampm-str   ( w:dtm w:tos c-addr - = Write an AM/PM string)
  rot dtm-hour@ 11 > IF           \ If hour > 11 Then
    char+ char+                   \   Move to pm
  THEN
  2                               \ String size
  rot tos-write-string            \ Write
;

defer dos.write-format


( Date and time writing words )

: dos-write-abbr-weekday-name   ( w:dtm w:tos - = Write the abbreviated weekday name, using the catalog for locale )
  swap dtm-weekday dtm.sunday -
  dos.weekday-names 3 min         \ Abbreviate the name to 3 characters
  rot  tos-write-string
;


: dos-write-weekday-name   ( w:dtm w:tos - = Write the full weekday name, using the catalog for locale )
  swap dtm-weekday dtm.sunday -
  dos.weekday-names
  rot  tos-write-string
;


: dos-write-abbr-month-name   ( w:dtm w:tos - = Write the abbreviated month name, using the catalog for locale )
  swap dtm-month@ dtm.january -
  dos.month-names 3 min           \ Abbreviate the name to 3 characters
  rot  tos-write-string
;


: dos-write-month-name   ( w:dtm w:tos - = Write the full month name, using the catalog for locale )
  swap dtm-month@ dtm.january -
  dos.month-names
  rot  tos-write-string
;


: dos-write-date-time   ( w:dtm w:tos - = Write the preferred time and date representation for the locale, else yyyy/mm/dd hh:mm:ss)
  >r
  s" %Y/%m/%d %H:%M:%S"
  r@ dos-translate
  r> dos.write-format
;


: dos-write-century   ( w:dtm w:tos - = Write the century number )
  swap dtm-year@ 100 /
  swap tos-write-number
;


: dos-write-monthday   ( w:dtm w:tos - = Write the day of the month: 01..31)
  swap dtm-day@ 
  dos-write-2zeroed
;


: dos-write-american-date   ( w:dtm w:tos - = Write the date in mm/dd/yy format )
  s" %m/%d/%y" rot dos.write-format
;


: dos-write-spaced-monthday   ( w:dtm w:tos - = Write the day of the month,  1..31, space padded)
  swap dtm-day@ 
  dos-write-2spaced
;


: dos-write-iso8601-date   ( w:dtm w:tos - = Write the date in ISO 8601 format: yyyy-mm-dd )
  s" %Y-%m-%d" rot dos.write-format
;


: dos-write-24hour   ( w:dtm w:tos - = Write the hour using a 24-hour clock: 00..23 )
  swap dtm-hour@ 
  dos-write-2zeroed
;


: dos-write-12hour   ( w:dtm w:tos - = Write the hour using a 12-hour clock: 01..12 )
  swap dtm-hour@ dos+24to12 
  dos-write-2zeroed
;


: dos-write-yearday   ( w:dtm w:tos - = Write the day of the year: 001..366 )
  swap dtm-yearday
  over tos-write-number
  >r [char] 0 3 r> tos-align-right
;


: dos-write-spaced-24hour   ( w:dtm w:tos - = Write the hour using a 24-hour clock:  0..23, space padded )
  swap dtm-hour@ dos-write-2spaced
;


: dos-write-spaced-12hour   ( w:dtm w:tos - = Write the hour using a 12-hour clock:  1..12, space padded )
  swap dtm-hour@ dos+24to12 
  dos-write-2spaced
;


: dos-write-month   ( w:dtm w:tos - = Write the month: 01..12)
  swap dtm-month@
  dos-write-2zeroed
;


: dos-write-minute   ( w:dtm w:tos - = Write the minute: 00..59)
  swap dtm-minute@
  dos-write-2zeroed
;


: dos-write-ampm   ( w:dtm w:tos - = Write the am or pm notation, using the catalog for locale )
  s" ampm" drop
  dos-write-ampm-str
;


: dos-write-upper-ampm   ( w:dtm w:tos - = Write the AM or PM notation, using the catalog for locale )
  s" AMPM" drop
  dos-write-ampm-str
;


: dos-write-ampm-time   ( w:dtum w:tos - = Write the time in ampm notation: hh:mm:ss ?m, using the catalog for locale)
  >r
  s" %I:%M:%S %p" 
  r@ dos-translate
  r> dos.write-format
;


: dos-write-hhmm-time   ( w:dtm w:tos - = Write the time: hh:mm)
  s" %H:%M" rot dos.write-format
;


: dos-write-seconds-since-epoch  ( w:dtm w:tos - = Write the number of seconds since 1970-01-01 00:00:00)
  swap 1970 swap dtm-calc-seconds-since-epoch
  swap tos-write-number
;


: dos-write-seconds   ( w:dtm w:tos - = Write the number of seconds: 00..61)
  swap dtm-second@
  dos-write-2zeroed
;


: dos-write-hhmmss-time   ( w:dtm w:tos - = Write the time: hh:mm:ss)
  s" %H:%M:%S" rot dos.write-format
;


: dos-write-weekday   ( w:dtm w:tos - = Write the weekday: 0..6, 0 = sunday )
  swap dtm-weekday
  swap tos-write-number
;


: dos-write-week-number   ( w:dtm w:tos - = Write the week number: 01..53 )
  swap dtm-iso-weeknumber
  dos-write-2zeroed
;


: dos-write-date   ( w:dtm w:tos - = Write the preferred date representation for the locale, else yyyy/mm/dd )
  >r
  s" %Y/%m/%d"
  r@ dos-translate
  r> dos.write-format
;


: dos-write-time   ( w:dtm w:tos - = Write the preferred time representation for the locale, else hh:mm:ss )
  >r
  s" %H:%M:%S"
  r@ dos-translate
  r> dos.write-format
;


: dos-write-2year    ( w:dtm w:tos - = Write the year without the century: 00..99)
  swap dtm-year@ 100 mod
  dos-write-2zeroed
;


: dos-write-year   ( w:dtm w:tos - = Write the year including the century )
  swap dtm-year@
  swap tos-write-number
;


( Private formatting words )

: dos-nothing   ( w:dtm w:tos - = Not a valid conversion character )
  2drop
;

create dos.jump
  ' dos-write-weekday-name ,        \ A
  ' dos-write-month-name ,          \ B
  ' dos-write-century ,             \ C
  ' dos-write-american-date ,       \ D
  ' dos-nothing ,                   \ E
  ' dos-write-iso8601-date ,        \ F
  ' dos-nothing ,                   \ G
  ' dos-write-24hour ,              \ H
  ' dos-write-12hour ,              \ I
  ' dos-nothing ,                   \ J
  ' dos-nothing ,                   \ K
  ' dos-nothing ,                   \ L
  ' dos-write-minute ,              \ M
  ' dos-nothing ,                   \ N
  ' dos-nothing ,                   \ O
  ' dos-write-ampm ,                \ P
  ' dos-nothing ,                   \ Q
  ' dos-write-hhmm-time ,           \ R
  ' dos-write-seconds ,             \ S
  ' dos-write-hhmmss-time ,         \ T
  ' dos-nothing ,                   \ U
  ' dos-write-week-number ,         \ V
  ' dos-nothing ,                   \ W
  ' dos-write-time ,                \ X
  ' dos-write-year ,                \ Y
  ' dos-nothing ,                   \ Z
  ' dos-nothing ,                   \ [
  ' dos-nothing ,                   \ \
  ' dos-nothing ,                   \ ]
  ' dos-nothing ,                   \ ^
  ' dos-nothing ,                   \ _
  ' dos-nothing ,                   \ '
  ' dos-write-abbr-weekday-name ,   \ a
  ' dos-write-abbr-month-name ,     \ b
  ' dos-write-date-time ,           \ c
  ' dos-write-monthday ,            \ d
  ' dos-write-spaced-monthday ,     \ e
  ' dos-nothing ,                   \ f
  ' dos-nothing ,                   \ g
  ' dos-write-abbr-month-name ,     \ h
  ' dos-nothing ,                   \ i
  ' dos-write-yearday ,             \ j
  ' dos-write-spaced-24hour ,       \ k
  ' dos-write-spaced-12hour ,       \ l
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
  
  
: dos+jump   ( .. c - .. = Execute the word related to the formatting conversion character 'a'..'Z' )
  [char] A - cells dos.jump + @ execute
;


( Date and time formatting word )

: dos-write-format   ( w:dtm c-addr u w:tos - = Write a formatted date and time in the stream )
  false 2swap
  bounds ?DO
    IF                            \ If previous char was '%' Then
      I c@                        \   If char = A..z then call word
      dup [char] A [char] z 1+ within IF
        >r 2dup r> dos+jump
      ELSE                        \   Else
        over tos-write-char       \     Write character
      THEN
      false
    ELSE                          \ Else
      I c@
      dup [char] % = IF           \   If format character Then
        drop 
        true                      \     Next char special
      ELSE                        \   Else
        over tos-write-char       \     Write char
        false
      THEN
    THEN
  1 chars +LOOP
  drop 2drop  
;

' dos-write-format is dos.write-format

[THEN]

\ ==============================================================================
