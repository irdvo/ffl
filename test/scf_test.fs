\ ==============================================================================
\
\        scf_test - the test words for the scf module in the ffl
\
\               Copyright (C) 2009  Dick van Oudheusden
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

include ffl/scf.fs
include ffl/tst.fs


.( Testing: scf) cr

\ c specifier
t{ s" ab" s" %c" scf+scan 1 ?s char a ?s }t
t{ s" ab" s" %c%c" scf+scan 2 ?s char b ?s char a ?s }t

\ d specifier
t{ s" 15 23" s" %d" scf+scan 1 ?s 15 ?s }t
t{ s" 15 23" s" %d%d" scf+scan 2 ?s 23 ?s 15 ?s }t

t{ s" -15 -23" s" %d" scf+scan 1 ?s -15 ?s }t
t{ s" -15 -23" s" %d%d" scf+scan 2 ?s -23 ?s -15 ?s }t

t{ s" +15 +23" s" %d" scf+scan 1 ?s 15 ?s }t
t{ s" +15 +23" s" %d%d" scf+scan 2 ?s 23 ?s 15 ?s }t

t{ s" 15 23" s" %ld" scf+scan 1 ?s 15. ?d }t
t{ s" 15 23" s" %ld%ld" scf+scan 2 ?s 23. ?d 15. ?d }t

t{ s" -15 -23" s" %ld" scf+scan 1 ?s -15. ?d }t
t{ s" -15 -23" s" %ld%ld" scf+scan 2 ?s -23. ?d -15. ?d }t

\ o specifier
t{ s" 15 23" s" %o" scf+scan 1 ?s 13 ?u }t
t{ s" 15 23" s" %o%o" scf+scan 2 ?s 19 ?u 13 ?u }t

t{ s" -15 -23" s" %o" scf+scan 1 ?s -13 ?u }t
t{ s" -15 -23" s" %o%o" scf+scan 2 ?s -19 ?s -13 ?s }t

t{ s" 15 23" s" %lo" scf+scan 1 ?s 13. ?ud }t
t{ s" 15 23" s" %lo%lo" scf+scan 2 ?s 19. ?ud 13. ?ud }t

t{ s" -15 -23" s" %lo" scf+scan 1 ?s -13. ?ud }t
t{ s" -15 -23" s" %lo%lo" scf+scan 2 ?s -19. ?ud -13. ?ud }t

\ s specifier
t{ s" abc def" s" %s" scf+scan 1 ?s s" abc" ?str }t
t{ s" abc def" s" %s %s" scf+scan 2 ?s s" def" ?str s" abc" ?str }t

\ q specifier
t{ s\" abc \"def\" \"g\\\"h\\\"i\"" s" %q %q %q" scf+scan 3 ?s s\" g\\\"h\\\"i" ?str s" def" ?str s" abc" ?str ) }t

\ x specifier
t{ s" 1C C3" s" %x" scf+scan 1 ?s 28 ?u }t
t{ s" 1c c3" s" %x%x" scf+scan 2 ?s 195 ?u 28 ?u }t

t{ s" -1C -C3" s" %x" scf+scan 1 ?s -28 ?u }t
t{ s" -1c -c3" s" %x%x" scf+scan 2 ?s -195 ?s -28 ?s }t

t{ s" 1C C3" s" %lx" scf+scan 1 ?s 28. ?ud }t
t{ s" 1c c3" s" %lx%lx" scf+scan 2 ?s 195. ?ud 28. ?ud }t

t{ s" -1C -C3" s" %lx" scf+scan 1 ?s -28. ?ud }t
t{ s" -1c -c3" s" %lx%lx" scf+scan 2 ?s -195. ?ud -28. ?ud }t

\ X specifier
t{ s" 1C C3" s" %X" scf+scan 1 ?s 28 ?u }t
t{ s" 1c c3" s" %X%X" scf+scan 2 ?s 195 ?u 28 ?u }t

t{ s" -1C -C3" s" %X" scf+scan 1 ?s -28 ?u }t
t{ s" -1c -c3" s" %X%X" scf+scan 2 ?s -195 ?s -28 ?s }t

t{ s" 1C C3" s" %lX" scf+scan 1 ?s 28. ?ud }t
t{ s" 1c c3" s" %lX%lX" scf+scan 2 ?s 195. ?ud 28. ?ud }t

t{ s" -1C -C3" s" %lX" scf+scan 1 ?s -28. ?ud }t
t{ s" -1c -c3" s" %lX%lX" scf+scan 2 ?s -195. ?ud -28. ?ud }t

\ eE specifier
t{ s" 1.0 1.a 1e1 -1e-1 +1e+1" s" %e %e%c %e %e %e" scf+scan 6 ?s 1e+1 ?r -1e-1 ?r 1e+1 ?r char a ?s 1e+0 ?r 1e+0 ?r }t
 
t{ s" 72.0 -81 0.1E-9 +0.2 0.00001" s" %e %e %e %e %e" scf+scan 5 ?s 1e-5 ?r 0.2e+0 ?r 0.1e-9 ?r -81e+0 ?r 72e+0 ?r }t

t{ s" 72.10E0E 72.10. +72.10+" s" %e%c %e%c %e%c" scf+scan 6 ?s char + ?s 72.10e+0 ?r char . ?s 72.10e+0 ?r char E ?s 72.10e+0 ?r }t

\ Combined
t{ s" %var = -100 (+99.99E+0)" scf" %%%s = %c%d (%e)" 4 ?s 99.99E+0 ?r 100 ?s char - ?s s" var" ?str }t

\ Not fully matched
t{ s" FF 77 + 99" scf" %x %o %% %d" 2 ?s 63 ?s 255 ?s }t

: scf-test
  s" FF 77 + 99" scf" %x %o %% %d" 2 = 
;

t{ scf-test ?true 63 ?s 255 ?s }t

\ ==============================================================================
