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
\  $Date: 2008-09-20 05:31:18 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/spf.fs

str-new value spf1

.( Format numbers: ) 
16 15 10 8 1 s" %d %u %o %x %X" spf1 spf-set   \ Format the numbers

spf1 str-get type                              \ Print the result

