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
\  $Date: 2006-01-28 08:11:58 $ $Revision: 1.1 $
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

\ ==============================================================================
