\ ==============================================================================
\
\        arg_test - the test words for the arg module in the ffl
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
\  $Date: 2007-07-09 17:38:09 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/arg.fs
include ffl/tst.fs


[DEFINED] arg.version [IF]

.( Testing: arg) cr 

t{ s" test" 
   s" Usage: test [OPTION] .. [FILES]" 
   s" test v1.0" 
   s" Report bugs to bugs@bugs.com" arg-create arg1 }t


t{ s" alf" 
   s" Usage: alf [OPTION] .. [FILE]" 
   s" alf v0.5   Copyright (c) 2007 by Dick van Oudheusden" 
   s" Report bugs to bugs@bugs.com" arg-new value arg2 }t

t{ arg2 arg-free }t

[THEN]

\ ==============================================================================
