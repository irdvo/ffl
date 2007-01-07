\ ==============================================================================
\
\          dtm_test - the test words for the dtm and dti module in ffl
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
\  $Date: 2007-01-07 18:40:35 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/dtm.fs
include ffl/dti.fs


.( Testing: dtm and dti ) cr 

t{ dtm-create d1 }t

t{ dtm-new value d2 }t

\ Module methods check

t{ 2006 dtm+leap-year? ?false }t
t{ 2004 dtm+leap-year? ?true  }t
t{ 2000 dtm+leap-year? ?true  }t
t{ 1900 dtm+leap-year? ?false }t

t{ 2006 2002 dtm+calc-leap-years 1 ?s }t
t{ 2006 1998 dtm+calc-leap-years 2 ?s }t
t{ 2006 2005 dtm+calc-leap-years ?0   }t

t{ dtm.june 2006     dtm+days-in-month 30 ?s }t
t{ dtm.february 2006 dtm+days-in-month 28 ?s }t
t{ dtm.february 2008 dtm+days-in-month 29 ?s }t
t{ dtm.december 2008 dtm+days-in-month 31 ?s }t

t{ dtm.january  2006 dtm+days-till-month ?0     }t
t{ dtm.march    2006 dtm+days-till-month 59  ?s }t
t{ dtm.march    2008 dtm+days-till-month 60  ?s }t
t{ dtm.december 2008 dtm+days-till-month 335 ?s }t

t{ 0    dtm+milli? ?true }t
t{ 999  dtm+milli? ?true }t
t{ 1000 dtm+milli? ?false }t

t{ -1   dtm+second? ?false }t
t{ 0    dtm+second? ?true  }t
t{ 59   dtm+second? ?true  }t
t{ 60   dtm+second? ?false }t

t{ -1   dtm+minute? ?false }t
t{ 0    dtm+minute? ?true  }t
t{ 59   dtm+minute? ?true  }t
t{ 60   dtm+minute? ?false }t

t{ -1   dtm+hour? ?false }t
t{ 0    dtm+hour? ?true  }t
t{ 23   dtm+hour? ?true  }t
t{ 24   dtm+hour? ?false }t

t{ 32 dtm.january  2006 dtm+day? ?false }t
t{ 29 dtm.february 2006 dtm+day? ?false }t
t{ 29 dtm.february 1996 dtm+day? ?true  }t
t{ 31 dtm.june     2006 dtm+day? ?false }t
t{ 31 dtm.december 2005 dtm+day? ?true  }t

t{ 0    dtm+month? ?false }t
t{ 1    dtm+month? ?true  }t
t{ 12   dtm+month? ?true  }t
t{ 13   dtm+month? ?false }t

t{ 1582 dtm+year? ?false }t
t{ 1583 dtm+year? ?true  }t
t{ 2006 dtm+year? ?true  }t
t{ 9999 dtm+year? ?true  }t

\ Set, Get, Member and compare checks

t{ 234 46 35 7 4 dtm.june 2006 d1 dtm-set }t

t{ d1 dtm-milli@ 234 ?s }t
t{ 654 d1 dtm-milli!    }t
t{ d1 dtm-milli@ 654 ?s }t

t{ d1 dtm-second@ 46 ?s }t
t{ 21 d1 dtm-second!    }t
t{ d1 dtm-second@ 21 ?s }t

t{ d1 dtm-minute@ 35 ?s }t
t{ 50 d1 dtm-minute!    }t
t{ d1 dtm-minute@ 50 ?s }t

t{ d1 dtm-hour@ 7 ?s }t
t{ 6 d1 dtm-hour!    }t
t{ d1 dtm-hour@ 6 ?s }t

t{ d1 dtm-day@ 4 ?s }t
t{ 7 d1 dtm-day!    }t
t{ d1 dtm-day@ 7 ?s }t

t{ d1 dtm-month@ dtm.june ?s }t
t{ dtm.may d1 dtm-month!    }t
t{ d1 dtm-month@ dtm.may ?s }t

t{ d1 dtm-year@ 2006 ?s }t
t{ 2005 d1 dtm-year!    }t
t{ d1 dtm-year@ 2005 ?s }t

t{ 654 21 50 6 7 dtm.may 2005 d1 dtm-compare ?0 }t

t{ d1 dtm-get 2005 ?s dtm.may ?s 7 ?s 6 ?s 50 ?s 21 ?s 654 ?s }t
t{ d1 dtm-get-date 2005 ?s dtm.may ?s 7 ?s }t
t{ d1 dtm-get-time 6 ?s 50 ?s 21 ?s 654 ?s }t

t{ d1 dtm-set-now }t

t{ 0 0 0 0 1 dtm.january 1600 d2 dtm-set }t

t{ d1 d2 dtm^compare 1 ?s }t

t{ 13512. dtm.unix-epoch d1 dtm-set-with-days }t

t{ d1 dtm-get-date 2006 ?s dtm.december ?s 30 ?s }t

t{ 13513. dtm.unix-epoch d1 dtm-set-with-days }t

t{ d1 dtm-get-date 2006 ?s dtm.december ?s 31 ?s }t

t{ 13514. dtm.unix-epoch d1 dtm-set-with-days }t

t{ d1 dtm-get-date 2007 ?s dtm.january ?s 1 ?s }t

t{ 13515. dtm.unix-epoch d1 dtm-set-with-days }t

t{ d1 dtm-get-date 2007 ?s dtm.january ?s 2 ?s }t


\ Calculations

t{ 234 46 35 7 4 dtm.june 2006 d2 dtm-set }t

t{ d2 dtm-weekday dtm.sunday ?s }t

t{ 14 dtm.february 1920 d2 dtm-set-date }t

t{ d2 dtm-weekday dtm.saturday ?s }t

t{ 4 dtm.july 1776 d2 dtm-set-date }t

t{ d2 dtm-weekday dtm.thursday ?s }t

t{ 14 dtm.february 2020 d2 dtm-set-date }t

t{ d2 dtm-weekday dtm.friday ?s }t

t{ 1 dtm.january 2006 d2 dtm-set-date }t

t{ d2 dtm-yearday 1 ?s }t

t{ d2 dtm-iso-weeknumber 2005 ?s 52 ?s }t

t{ 2 d2 dtm-day! }t

t{ d2 dtm-yearday 2 ?s }t

t{ d2 dtm-iso-weeknumber 2006 ?s 1 ?s }t

t{ 31 dtm.december 2006 d2 dtm-set-date }t

t{ d2 dtm-yearday 365 ?s }t

t{ d2 dtm-iso-weeknumber 2006 ?s 52 ?s }t

t{ 2007 d2 dtm-year! }t

t{ d2 dtm-iso-weeknumber 2008 ?s 1 ?s }t

t{ 2008 d2 dtm-year! }t

t{ d2 dtm-yearday 366 ?s }t

t{ 0 26 06 20 20 dtm.june 2006 d2 dtm-set }t

t{ dtm.unix-epoch d2 dtm-calc-seconds-since-epoch 1150833986. ?d }t

\ iterator


t{ 999 59 59 23 31 dtm.december 2006 d2 dtm-set }t

t{ d2 dti-milli+ }t

t{   0  0  0  0  1 dtm.january 2007 d2 dtm-compare ?0 }t

t{ d2 dti-milli- }t

t{ 999 59 59 23 31 dtm.december 2006 d2 dtm-compare ?0 }t

t{ 260 26 20 06 29 dtm.february 2008 d2 dtm-set }t


t{ d2 dti-milli- }t

t{ d2 dtm-milli@ 259 ?s }t

t{ d2 dti-milli+ }t

t{ d2 dtm-milli@ 260 ?s }t


t{ d2 dti-second- }t

t{ d2 dtm-second@ 25 ?s }t

t{ d2 dti-second+ }t

t{ d2 dtm-second@ 26 ?s }t


t{ d2 dti-minute- }t

t{ d2 dtm-minute@ 19 ?s }t

t{ d2 dti-minute+ }t

t{ d2 dtm-minute@ 20 ?s }t


t{ d2 dti-hour- }t

t{ d2 dtm-hour@ 5 ?s }t

t{ d2 dti-hour+ }t

t{ d2 dtm-hour@ 6 ?s }t


t{ d2 dti-day- }t

t{ d2 dtm-day@ 28 ?s }t

t{ d2 dti-day+ }t

t{ d2 dtm-day@ 29 ?s }t


t{ d2 dti-month- }t

t{ d2 dtm-month@ dtm.january ?s }t

t{ d2 dti-month+ }t

t{ d2 dtm-month@ dtm.february ?s }t


t{ d2 dti-year- }t

t{ d2 dtm-year@ 2007 ?s }t
t{ d2 dtm-day@  28   ?s }t

t{ d2 dti-year+ }t

t{ d2 dtm-year@ 2008 ?s }t
t{ d2 dtm-day@  28   ?s }t


t{ dtm.march d2 dtm-month! }t
t{ 31        d2 dtm-day!   }t

t{ d2 dti-month+ }t

t{ d2 dtm-month@ dtm.april ?s }t
t{ d2 dtm-day@   30        ?s }t

t{ 90061. d2 dti-seconds+ }t   \ 1 sec + 1 min + 1 hour + 1 day

t{ 260 27 21 7 1 5 2008 d2 dtm-compare ?0 }t

t{ 90061. d2 dti-seconds- }t

t{ 260 26 20 6 30 4 2008 d2 dtm-compare ?0 }t

t{ 366. d2 dti-days+ }t

t{ 260 26 20 6 1 5 2009 d2 dtm-compare ?0 }t

t{ 365. d2 dti-days- }t

t{ 260 26 20 6 30 4 2008 d2 dtm-compare ?0 }t

\ free

t{ d2 dtm-free }t

\ ==============================================================================
