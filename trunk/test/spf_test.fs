\ ==============================================================================
\
\        spf_test - the test words for the spf module in the ffl
\
\               Copyright (C) 2008  Dick van Oudheusden
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
\  $Date: 2008-09-22 18:46:53 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/spf.fs
include ffl/tst.fs
include ffl/est.fs


.( Testing: spf) cr

str-new value spf1

t{ s" hello" spf1 spf-set }t
t{ spf1 str-get s" hello" ?str }t

t{ s" value:10%%" spf1 spf-set }t
t{ spf1 str-get s" value:10%" ?str }t


t{ char ! char ? s" hello %c%c" spf1 spf-set }t
t{ spf1 str-get s" hello ?!" ?str }t

t{ char = s" %4c" spf1 spf-set }t
t{ spf1 str-get s"    =" ?str }t

t{ char = s" %-4c" spf1 spf-set }t
t{ spf1 str-get s" =   " ?str }t


t{ s" abc" s" String:%s" spf1 spf-set }t
t{ spf1 str-get s" String:abc" ?str }t


: spf-2string-test 
  s" def" s" abc" s" String:%5s %-5s" 
;

t{ spf-2string-test spf1 spf-set }t
t{ spf1 str-get s" String:  abc def  " ?str }t

: spf-qstring-test
  s" def" s" abc" s" String:%7q %-7q"
;

t{ spf-qstring-test spf1 spf-set }t
t{ spf1 str-get s\" String:  \"abc\" \"def\"  " ?str }t

t{ 77 88 99 s" 1:%d % d %+d" spf1 spf-set }t
t{ spf1 str-get s" 1:99  88 +77" ?str }t

t{ -77 -88 -99 s" 2:%d % d %+d" spf1 spf-set }t
t{ spf1 str-get s" 2:-99 -88 -77" ?str }t

t{ 66 77 88 99 s" 3:%4d %-4d %04d %-04d" spf1 spf-set }t
t{ spf1 str-get s" 3:  99 88   0077 66  " ?str }t

t{ 66 77 88 99 s" 4:%+4d %+-4d %0+4d %-+04d" spf1 spf-set }t
t{ spf1 str-get s" 4: +99 +88  +077 +66 " ?str }t

t{ -66 -77 -88 -99 s" 5:%4d %-4d %04d %-04d" spf1 spf-set }t
t{ spf1 str-get s" 5: -99 -88  -077 -66 " ?str }t

t{ -66 77 -88 99 s" 7:%+4i %+-4i %0+4i %-+04i" spf1 spf-set }t
t{ spf1 str-get s" 7: +99 -88  +077 -66 " ?str }t


t{ 77. 88. 99. s" 8:%ld % ld %+ld" spf1 spf-set }t
t{ spf1 str-get s" 8:99  88 +77" ?str }t

t{ -77. -88. -99. s" 9:%ld % ld %+ld" spf1 spf-set }t
t{ spf1 str-get s" 9:-99 -88 -77" ?str }t

t{ 66. 77. 88. 99. s" 10:%4ld %-4ld %04ld %-04ld" spf1 spf-set }t
t{ spf1 str-get s" 10:  99 88   0077 66  " ?str }t

t{ 66. 77. 88. 99. s" 11:%+4ld %+-4ld %0+4ld %-+04ld" spf1 spf-set }t
t{ spf1 str-get s" 11: +99 +88  +077 +66 " ?str }t

t{ -66. -77. -88. -99. s" 12:%4ld %-4ld %04ld %-04ld" spf1 spf-set }t
t{ spf1 str-get s" 12: -99 -88  -077 -66 " ?str }t


t{ 77 88 99 s" 13:%u % u %+u" spf1 spf-set }t
t{ spf1 str-get s" 13:99 88 77" ?str }t

t{ 66 77 88 99 s" 14:%4u %-4u %04u %-04u" spf1 spf-set }t
t{ spf1 str-get s" 14:  99 88   0077 66  " ?str }t

t{ 66 77 88 99 s" 15:%+4u %+-4u %0+4u %-+04u" spf1 spf-set }t
t{ spf1 str-get s" 15:  99 88   0077 66  " ?str }t


t{ 77 88 111 s" 16:%x % x %+x" spf1 spf-set }t
t{ spf1 str-get s" 16:6f 58 4d" ?str }t

t{ 66 77 88 111 s" 17:%4x %-4x %04x %-04x" spf1 spf-set }t
t{ spf1 str-get s" 17:  6f 58   004d 42  " ?str }t

t{ 66 77 88 111 s" 18:%+4x %+-4x %0+4x %-+04x" spf1 spf-set }t
t{ spf1 str-get s" 18:  6f 58   004d 42  " ?str }t


t{ 77 88 111 s" 19:%X % X %+X" spf1 spf-set }t
t{ spf1 str-get s" 19:6F 58 4D" ?str }t

t{ 66 77 88 111 s" 20:%4X %-4X %04X %-04X" spf1 spf-set }t
t{ spf1 str-get s" 20:  6F 58   004D 42  " ?str }t

t{ 66 77 88 111 s" 21:%+4X %+-4X %0+4X %-+04X" spf1 spf-set }t
t{ spf1 str-get s" 21:  6F 58   004D 42  " ?str }t


t{ 77 88 99 s" 22:%o % o %+o" spf1 spf-set }t
t{ spf1 str-get s" 22:143  130 +115" ?str }t

t{ -77 -88 -99 s" 23:%o % o %+o" spf1 spf-set }t
t{ spf1 str-get s" 23:-143 -130 -115" ?str }t

t{ 66 77 88 99 s" 24:%4o %-4o %04o %-04o" spf1 spf-set }t
t{ spf1 str-get s" 24: 143 130  0115 102 " ?str }t

t{ 66 63 88 99 s" 25:%+4o %+-4o %0+4o %-+04o" spf1 spf-set }t
t{ spf1 str-get s" 25:+143 +130 +077 +102" ?str }t

t{ -66 -63 -88 -99 s" 26:%4o %-4o %04o %-04o" spf1 spf-set }t
t{ spf1 str-get s" 26:-143 -130 -077 -102" ?str }t

precision value spf-save  4 set-precision


256 spf1 str-size!

t{ 1.0E+0 1.2344E+0 5.4321E-19 8.9E+250 s" 27:%e % e %+e %-e" spf1 spf-set }t
t{ spf1 str-get s" 27:8.900e+250  5.432e-19 +1.234e+00 1.000e+00" ?str }t

t{ -1.0E+0 -1.2344E+0 -5.4321E-19 -8.9E+250 s" 28:%e % e %+e %-e" spf1 spf-set }t
t{ spf1 str-get s" 28:-8.900e+250 -5.432e-19 -1.234e+00 -1.000e+00" ?str }t

t{ 1.0E+0 1.2344E+0 5.4321E-19 8.9E+250 s" 29:%13e %-12e %012e %-012e" spf1 spf-set }t
t{ spf1 str-get s" 29:   8.900e+250 5.432e-19    0001.234e+00 1.000e+00   " ?str }t

t{ 1.0E+0 1.2344E+0 5.4321E-19 8.9E+250 s" 30:%+13e %+-12e %+012e %+-012e" spf1 spf-set }t
t{ spf1 str-get s" 30:  +8.900e+250 +5.432e-19   +001.234e+00 +1.000e+00  " ?str }t

t{ -1.0E+0 -1.2344E+0 -5.4321E-19 -8.9E+250 s" 31:%13e %-12e %012e %-012e" spf1 spf-set }t
t{ spf1 str-get s" 31:  -8.900e+250 -5.432e-19   -001.234e+00 -1.000e+00  " ?str }t


t{ 1.0E+0 1.2344E+0 5.4321E-19 8.9E+250 s" 32:%E % E %+E %-E" spf1 spf-set }t
t{ spf1 str-get s" 32:8.900E+250  5.432E-19 +1.234E+00 1.000E+00" ?str }t

t{ -1.0E+0 -1.2344E+0 -5.4321E-19 -8.9E+250 s" 33:%E % E %+E %-E" spf1 spf-set }t
t{ spf1 str-get s" 33:-8.900E+250 -5.432E-19 -1.234E+00 -1.000E+00" ?str }t

t{ 1.0E+0 1.2344E+0 5.4321E-19 8.9E+250 s" 34:%13E %-12E %012E %-012E" spf1 spf-set }t
t{ spf1 str-get s" 34:   8.900E+250 5.432E-19    0001.234E+00 1.000E+00   " ?str }t

t{ 1.0E+0 1.2344E+0 5.4321E-19 8.9E+250 s" 35:%+13E %+-12E %+012E %+-012E" spf1 spf-set }t
t{ spf1 str-get s" 35:  +8.900E+250 +5.432E-19   +001.234E+00 +1.000E+00  " ?str }t

t{ -1.0E+0 -1.2344E+0 -5.4321E-19 -8.9E+250 s" 36:%13E %-12E %012E %-012E" spf1 spf-set }t
t{ spf1 str-get s" 36:  -8.900E+250 -5.432E-19   -001.234E+00 -1.000E+00  " ?str }t


2 set-precision

variable spf-result

t{ s" All:" spf1 str-set }t
t{ spf-result 1.0E+0 9 8 7 s" #" char * s" %c %s %d %i %u %n %% %E" spf1 spf-append }t
t{ spf1 str-get s" All:* # 7 8 9  % 1.0E+00" ?str }t
t{ spf-result @ 14 ?s }t

t{ char ! s" hello" spf1 spf" %s !%c" }t
t{ spf1 str-get s" hello !!" ?str }t

: spf-test
  [char] ? s" bye" spf1 spf" %s %c!"
;

t{ spf-test }t
t{ spf1 str-get s" bye ?!" ?str }t

spf-save set-precision

t{ spf1 str-free }t

\ ==============================================================================
