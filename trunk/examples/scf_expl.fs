\ ==============================================================================
\
\              scf_expl - the sscanf example in the ffl
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

\ Conversions
.( Scan the string : ) 
s" 10 a hello CE 55 20" 2dup type     \ the source string
.(  with the specifiers: ) 
s" %d %c %s %x %o %ld"  2dup type cr  \ the specifier string

scf+scan                              \ scan the source string with the specifier string

.( Number of conversions: ) . cr      \ the number of conversions done by scf+scan
.( Double: ) d. cr                    \ the results
.( Octal: ) . cr
.( Hex: ) . cr
.( String: ) type cr
.( Character: ) emit cr
.( Decimal: ) . cr

\ Float conversions
[DEFINED] >float [IF]
.( Scan floats ) cr
s" 10 20.1 -30.3E+5" s" %e %e %e" scf+scan
.( Number of floats: ) . 
.(  values: ) f. f. f. cr
[THEN]

\ Matching and conversion
.( Match and scan, with parsing word ) cr
s" %var = 20 (24)" scf" %%%s = %d [%o]"

.( Number of conversions: ) .          \ Only 2 conversion due to mismatch of '['
.(  values: ) . type cr
