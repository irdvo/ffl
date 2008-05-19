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
\  $Date: 2008-05-19 05:44:00 $ $Revision: 1.6 $
\
\ ==============================================================================

include ffl/arg.fs
include ffl/tst.fs


[DEFINED] arg.version [IF]

.( Testing: arg) cr

t{ 60 TO arg.cols }t

t{ s" test" s" [OPTION] .. [FILES]" s" v1.0" s" Report bugs to bugs@bugs.com" arg-new value arg1 }t

   
t{ arg1 arg-add-help-option }t

t{ arg1 arg-add-version-option }t


t{ char a s" " s" test option a" true 4 arg1 arg-add-option }t
   
t{ char b s" bold" s" test option b/bold" true 5 arg1 arg-add-option }t
   
t{ char c s" caption" s" test option c/caption" false 6 arg1 arg-add-option }t
   
t{ 0 s" verbose" s" test option verbose" true 7 arg1 arg-add-option }t
   
t{ char f s" file=FILE" s" test option f/file" false 8 arg1 arg-add-option }t

#args [IF]

\ test with -ab -c TEST --verbose --file=FILE input

\ t{ arg1 arg-print-help    }t
\ t{ arg1 arg-print-version }t

t{ arg1 arg-parse 4 ?s                                   }t
t{ arg1 arg-parse 5 ?s                                   }t
t{ arg1 arg-parse 6 ?s s" TEST" ?str                     }t
t{ arg1 arg-parse 7 ?s                                   }t
t{ arg1 arg-parse 8 ?s s" FILE" ?str                     }t
t{ arg1 arg-parse arg.non-option ?s s" input" ?str       }t
t{ arg1 arg-parse arg.done ?s                            }t

[THEN]
   
t{ arg1 arg-free }t

[THEN]

\ ==============================================================================
