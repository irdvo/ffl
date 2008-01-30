\ ==============================================================================
\
\        dos_test - the test words for the dos module in the ffl
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
\  $Date: 2008-01-30 06:54:00 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/dos.fs
include ffl/tst.fs


.( Testing: dos) cr

t{ dtm-create dtm4 }t
t{ dtm-create dtm5 }t
t{ dtm-create dtm6 }t
t{ dtm-create dtm7 }t

t{ tos-create tos4 }t

t{ 0 45 15 18 21 dtm.november 2007 dtm4 dtm-set }t
t{ 0 45 15 12 28 dtm.october  2007 dtm5 dtm-set }t
t{ 0 45 15  6 31 dtm.december 2007 dtm6 dtm-set }t
t{ 0  8  5  0 21 dtm.june     2007 dtm7 dtm-set }t

\ Write words

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-abbr-weekday-name }t
t{ tos4 str-get s" Wed" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm5 tos4 dos-write-weekday-name }t
t{ tos4 str-get s" Sunday" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-abbr-month-name }t
t{ tos4 str-get s" Nov" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm6 tos4 dos-write-month-name }t
t{ tos4 str-get s" December" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-date-time }t
t{ tos4 str-get s" 2007/11/21 18:15:45" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-century }t
t{ tos4 str-get s" 20" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm7 tos4 dos-write-monthday }t
t{ tos4 str-get s" 21" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm7 tos4 dos-write-american-date }t
t{ tos4 str-get s" 06/21/07" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm7 tos4 dos-write-spaced-monthday }t
t{ tos4 str-get s" 21" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm7 tos4 dos-write-iso8601-date }t
t{ tos4 str-get s" 2007-06-21" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-24hour }t
t{ dtm5 tos4 dos-write-24hour }t
t{ dtm6 tos4 dos-write-24hour }t
t{ dtm7 tos4 dos-write-24hour }t
t{ tos4 str-get s" 18120600" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-12hour }t
t{ dtm5 tos4 dos-write-12hour }t
t{ dtm6 tos4 dos-write-12hour }t
t{ dtm7 tos4 dos-write-12hour }t
t{ tos4 str-get s" 06120612" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-yearday }t
t{ dtm5 tos4 dos-write-yearday }t
t{ dtm6 tos4 dos-write-yearday }t
t{ dtm7 tos4 dos-write-yearday }t
t{ tos4 str-get s" 325301365172" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-spaced-24hour }t
t{ dtm5 tos4 dos-write-spaced-24hour }t
t{ dtm6 tos4 dos-write-spaced-24hour }t
t{ dtm7 tos4 dos-write-spaced-24hour }t
t{ tos4 str-get s" 1812 6 0" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-spaced-12hour }t
t{ dtm5 tos4 dos-write-spaced-12hour }t
t{ dtm6 tos4 dos-write-spaced-12hour }t
t{ dtm7 tos4 dos-write-spaced-12hour }t
t{ tos4 str-get s"  612 612" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-month }t
t{ dtm5 tos4 dos-write-month }t
t{ dtm6 tos4 dos-write-month }t
t{ dtm7 tos4 dos-write-month }t
t{ tos4 str-get s" 11101206" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-month }t
t{ dtm5 tos4 dos-write-month }t
t{ dtm6 tos4 dos-write-month }t
t{ dtm7 tos4 dos-write-month }t
t{ tos4 str-get s" 11101206" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm7 tos4 dos-write-minute }t
t{ tos4 str-get s" 05" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-ampm }t
t{ dtm5 tos4 dos-write-ampm }t
t{ dtm6 tos4 dos-write-ampm }t
t{ dtm7 tos4 dos-write-ampm }t
t{ tos4 str-get s" pmpmamam" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-upper-ampm }t
t{ dtm5 tos4 dos-write-upper-ampm }t
t{ dtm6 tos4 dos-write-upper-ampm }t
t{ dtm7 tos4 dos-write-upper-ampm }t
t{ tos4 str-get s" PMPMAMAM" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm7 tos4 dos-write-ampm-time }t
t{ tos4 str-get s" 12:05:08 AM" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm7 tos4 dos-write-hhmm-time }t
t{ tos4 str-get s" 00:05" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm7 tos4 dos-write-seconds-since-epoch }t
t{ tos4 str-get s" 1182384308" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm7 tos4 dos-write-seconds }t
t{ tos4 str-get s" 08" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm7 tos4 dos-write-hhmmss-time }t
t{ tos4 str-get s" 00:05:08" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-weekday }t
t{ tos4 str-get s" 3" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm4 tos4 dos-write-week-number }t
t{ tos4 str-get s" 47" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm7 tos4 dos-write-date }t
t{ tos4 str-get s" 2007/06/21" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm6 tos4 dos-write-time }t
t{ tos4 str-get s" 06:15:45" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm6 tos4 dos-write-2year }t
t{ tos4 str-get s" 07" ?str }t

t{ tos4 tos-rewrite }t
t{ dtm6 tos4 dos-write-year }t
t{ tos4 str-get s" 2007" ?str }t

\ Format words

t{ tos4 tos-rewrite }t
t{ dtm6 s" %a%A%b%B%c%C%d%D%e%F%h%H%I%j%k%l%m%M%p%P%r%R%S%T%V%w%x%X%y%Y%%" tos4 dos-write-format }t
t{ tos4 str-get s" MonMondayDecDecember2007/12/31 06:15:45203112/31/07312007-12-31Dec0606365 6 61215AMam06:15:45 AM06:154506:15:450112007/12/3106:15:45072007%" ?str }t

\ ==============================================================================
