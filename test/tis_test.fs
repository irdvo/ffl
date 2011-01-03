\ ==============================================================================
\
\          chr_test - the test words for the chr module in ffl
\
\               Copyright (C) 2005  Dick van Oudheusden
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
\  $Date: 2008-03-18 19:09:48 $ $Revision: 1.9 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/tis.fs


.( Testing: tis ) cr 
  
t{ tis-create t1                }t

t{ s" abcdefghijklmnopqrstuvwxyz" t1 tis-set }t

t{ t1 tis-eof? ?false }t
t{ t1 tis-read-char ?true char a ?s }t
t{ t1 tis-read-char ?true char b ?s }t

t{ t1 tis-pntr@ 2 ?s }t

t{ 3 t1 tis-read-string s" cde" ?str }t
t{ t1 tis-pntr@ 5 ?s }t

\ Match

t{ char g t1 tis-cmatch-char ?false }t
t{ char F t1 tis-cmatch-char ?false }t
t{ char f t1 tis-cmatch-char ?true  }t

t{ char h t1 tis-imatch-char ?false }t
t{ char G t1 tis-imatch-char ?true  }t
t{ char h t1 tis-imatch-char ?true  }t

t{ s" abcd" t1 tis-cmatch-chars ?false }t
t{ s" abid" t1 tis-cmatch-chars ?true char i ?s }t
t{ s" JjKk" t1 tis-cmatch-chars ?true char j ?s }t

t{ s" lmno" t1 tis-cmatch-string ?false }t
t{ s" Klmn" t1 tis-cmatch-string ?false }t
t{ s" kLmn" t1 tis-cmatch-string ?false }t
t{ s" klmn" t1 tis-cmatch-string ?true }t

t{ s" qrst" t1 tis-imatch-string ?false }t
t{ s" O"    t1 tis-imatch-string ?true }t
t{ s" pq"   t1 tis-imatch-string ?true }t
t{ s" rSt"  t1 tis-imatch-string ?true }t

t{ chs-create chs3 }t

t{ chs3 chs-set-lower }t 

t{ chs3 t1 tis-match-set ?true char u ?s }t


\ Scan

t{ t1 tis-reset }t
t{ char 0 t1 tis-scan-char ?false }t
t{ char c t1 tis-scan-char ?true s" ab" ?str }t
t{ char f t1 tis-scan-char ?true s" de" ?str }t

t{ s" ab" t1 tis-scan-chars ?false }t
t{ s" ik" t1 tis-scan-chars ?true char i ?s s" gh" ?str }t
t{ s" lL" t1 tis-scan-chars ?true char l ?s s" jk" ?str }t

t{ s" ab" t1 tis-scan-string ?false }t
t{ s" stu" t1 tis-scan-string ?true s" mnopqr" ?str }t
t{ s" x"  t1 tis-scan-string ?true s" vw" ?str }t
t{ s" abc" t1 tis-scan-string ?false }t

t{ t1 tis-reset }t

t{ chs3 chs-reset }t

t{ char g chs3 chs-set-char }t
t{ char k chs3 chs-set-char }t
t{ char B chs3 chs-set-char }t

t{ chs3 t1 tis-scan-set ?true char g ?s s" abcdef" ?str }t


\ Seek

t{ 20 t1 tis-pntr! ?true }t
t{ t1 tis-read-char ?true char u ?s }t

t{ 3  t1 tis-pntr+! ?true }t
t{ 4  t1 tis-read-string s" yz" ?str }t
t{ t1 tis-eof? ?true }t

t{ 3  t1 tis-read-string ?0 }t

t{ -5  t1 tis-pntr! ?true }t
t{ t1 tis-pntr@ 21 ?s }t
t{ t1 tis-read-char ?true char v ?s }t

t{ 26 t1 tis-pntr! ?false }t
t{ -27 t1 tis-pntr! ?false }t

\ Read

t{ s" ab   cd" t1 tis-set }t

t{ chr.cr t1 str-push-char }t
t{ s" ef" t1 str-append-string }t
t{ chr.lf t1 str-push-char }t

t{ 2 t1 tis-read-string s" ab" ?str }t
t{ t1 tis-skip-spaces 3 ?s }t
t{ t1 tis-read-line s" cd" ?str }t
t{ t1 tis-read-line s" ef" ?str }t
t{ t1 tis-read-line ?0 }t


t{ s" -1257abc" t1 tis-set }t

t{ t1 tis-read-number ?true -1257 ?s }t
t{ t1 tis-read-char ?true char a ?s }t

hex
t{ s" ba1z" t1 tis-set }t

t{ t1 tis-read-number ?true decimal 2977 ?s }t
t{ t1 tis-read-char ?true char z ?s }t
decimal

t{ s" -abc" t1 tis-set }t

t{ t1 tis-read-number ?false }t
t{ t1 tis-read-char ?true char - ?s }t

t{ s" +abc" t1 tis-set }t

t{ t1 tis-read-number ?false }t
t{ t1 tis-read-char ?true char + ?s }t

t{ s" abc" t1 tis-set }t
t{ 3 t1 tis-read-string 3 ?s drop }t
t{ t1 tis-read-number ?false }t


t{ s" -1231231123123abc" t1 tis-set }t
t{ t1 tis-read-double ?true -1231231123123. ?d }t
t{ t1 tis-read-char ?true char a ?s }t

t{ s" 9899898989898998abc" t1 tis-set }t
t{ t1 tis-read-double ?true 9899898989898998. ?d }t
t{ t1 tis-read-char ?true char a ?s }t

t{ s" -abc" t1 tis-set }t
t{ t1 tis-read-double ?false }t
t{ t1 tis-read-char ?true char - ?s }t

[DEFINED] tis-read-float [IF]
t{ s" 1.0." t1 tis-set }t
t{ t1 tis-read-float ?true 1E0 ?r }t
t{ t1 tis-read-char ?true char . ?s }t

t{ s" -.5f" t1 tis-set }t
t{ t1 tis-read-float ?true -0.5E0 ?r }t
t{ t1 tis-read-char ?true char f ?s }t

t{ s" e10" t1 tis-set }t
t{ t1 tis-read-float ?false }t
t{ t1 tis-read-char ?true char e ?s }t

t{ s" +56.78E-2-" t1 tis-set }t
t{ t1 tis-read-float ?true 56.78E-2 ?r }t
t{ t1 tis-read-char ?true char - ?s }t

t{ s" +56.78e-2-" t1 tis-set }t
t{ t1 tis-read-float ?true 56.78E-2 ?r }t
t{ t1 tis-read-char ?true char - ?s }t

t{ s" -56.78D+2+" t1 tis-set }t
t{ t1 tis-read-float ?true -56.78E+2 ?r }t
t{ t1 tis-read-char ?true char + ?s }t

t{ s" -56.78d+2+" t1 tis-set }t
t{ t1 tis-read-float ?true -56.78E+2 ?r }t
t{ t1 tis-read-char ?true char + ?s }t
[THEN]

\ ==============================================================================
