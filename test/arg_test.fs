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
\  $Date: 2007-07-18 19:16:10 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/arg.fs
include ffl/tst.fs


[DEFINED] arg.version [IF]

.( Testing: arg) cr

t{ 60 TO arg.cols }t

t{ s" test" 
   s" [OPTION] .. [FILES]" 
   s" v1.0" 
   s" Report bugs to bugs@bugs.com" arg-new value arg1 }t

t{ arg1 arg-add-default-options }t

variable arg-count   0 arg-count !

: arg-do-switch ( data - f )
  1+! true
;

: arg-do-caption ( c-addr u data - f )
  -rot
  s" TEST" compare 0= IF
    1+!
  ELSE
    drop ." do caption"
  THEN
  true
;

: arg-do-file ( c-addr u data - f )
  -rot
  s" FILE" compare 0= IF
    1+!
  ELSE
    drop ." do file"
  THEN
  true
;

: arg-do-input ( c-addr u data - f )
  -rot
  s" input" compare 0= IF
    1+!
  ELSE
    drop ." do input"
  THEN
  true
;


t{ char a
   s" "
   s" test option a"
   true
   arg-count
   ' arg-do-switch
   arg1 arg-add-option }t
   
t{ char b
   s" bold"
   s" test option b/bold"
   true
   arg-count
   ' arg-do-switch
   arg1 arg-add-option }t
   
t{ char c
   s" caption"
   s" test option c/caption"
   false
   arg-count
   ' arg-do-caption
   arg1 arg-add-option }t
   
t{ 0
   s" verbose"
   s" test option verbose"
   true
   arg-count
   ' arg-do-switch
   arg1 arg-add-option }t
   
t{ char f
   s" file"
   s" test option f/file"
   false
   arg-count
   ' arg-do-file
   arg1 arg-add-option }t

#args [IF]

t{ arg-count ' arg-do-input arg1 arg-parse ?true }t

t{ arg-count @ 6 ?s }t

[THEN]
   
t{ arg1 arg-free }t

[THEN]

\ ==============================================================================
