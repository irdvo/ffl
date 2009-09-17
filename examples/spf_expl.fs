\ ==============================================================================
\
\              spf_expl - the sprintf example in the ffl
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

str-new value spf1                                     \ Create string as destination

.( Format numbers                       : ) 
[DEFINED] represent [IF]
18E+0 15 15 10 8 1 s" %d %u %o %x %X %E" spf1 spf-set  \ Format the numbers
[ELSE]
      15 15 10 8 1 s" %d %u %o %x %X"    spf1 spf-set  \ Format the numbers
[THEN]

spf1 str-get type cr                                   \ Print the result

.( Format strings and characters        : )
char ! s" Hello" s" %s %c" spf1 spf-set                \ Format strings and characters

spf1 str-get type cr                                   \ Print result

.( Format numbers with signs            : )
-30 20 10 s" %+d % d %d" spf1 spf-set                  \ Format numbers with signs

spf1 str-get type cr                                   \ Print result

.( Leading and trailing zeros and spaces: )
-40 -30 20 10 s" %5d %-+5d %05d %5d" spf1 spf-set      \ Format numbers with leading and trailing spaces

spf1 str-get type cr

.( Format with parsing word             : )
time&date spf1 spf" %04d-%02d-%2d %02d:%02d:%02d"      \ Format date & time

spf1 str-get type cr                                   \ Print result

