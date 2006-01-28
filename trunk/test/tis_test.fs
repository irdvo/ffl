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
\  $Date: 2006-01-28 19:48:40 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/tis.fs


.( Testing: tis ) cr 
  
t{ tis-create t1                }t

t{ s" abcdefghijklmnopqrstuvwxyz" t1 str-set }t

t{ t1 tis-eof? ?false }t
t{ t1 tis-read-char ?true char a ?s }t
t{ t1 tis-read-char ?true char b ?s }t

t{ t1 tis-tell 2 ?s }t

t{ 3 t1 tis-read-string s" cde" compare ?0 }t
t{ t1 tis-tell 5 ?s }t

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

\ other tests

t{ 20 t1 tis-seek-start ?true }t
t{ t1 tis-read-char ?true char u ?s }t

t{ 3  t1 tis-seek-current ?true }t
t{ 4  t1 tis-read-string s" yz" compare ?0 }t
t{ t1 tis-eof? ?true }t

t{ 3  t1 tis-read-string ?0 }t

t{ 5  t1 tis-seek-end ?true }t
t{ t1 tis-tell 21 ?s }t
t{ t1 tis-read-char ?true char v ?s }t

t{ 26 t1 tis-seek-start ?false }t
t{ 27 t1 tis-seek-end ?false }t

t{ s" ab   cd" t1 str-set }t
t{ t1 tis-reset }t

t{ chr.cr t1 str-push-char }t
t{ s" ef" t1 str-append }t
t{ chr.lf t1 str-push-char }t

t{ 2 t1 tis-read-string s" ab" compare ?0 }t
t{ t1 tis-skip-spaces 3 ?s }t
t{ t1 tis-read-line s" cd" compare ?0 }t
t{ t1 tis-read-line s" ef" compare ?0 }t
t{ t1 tis-read-line ?0 }t


\ ==============================================================================
