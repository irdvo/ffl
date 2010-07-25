\ ==============================================================================
\
\            fwt - the fixed width types module in the ffl
\
\               Copyright (C) 2010  Dick van Oudheusden
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
\  $Date: 2007-12-09 07:23:15 $ $Revision: 1.6 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] fwt.version [IF]


include ffl/stc.fs


( fwt = Fixed Width Types module )
( The fwt module implements types that have a fixed bit width. They are mainly )
( used in interfaces. The words are defined for 32- and 64-bit forth engines.  )


1 constant fwt.version

( Fixed width constants )

[UNDEFINED] #bytes/word [IF]
2 constant #bytes/word
[THEN]

[UNDEFINED] #bytes/long [IF]
4 constant #bytes/long
[THEN]


( Fixed width structure fields )

[UNDEFINED] wfield: [IF]
: wfield:    ( structure-sys "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field for a word, 2 bytes, not aligned, return the field address ) 
  #bytes/word +field
;
[THEN]


[UNDEFINED] lfield: [IF]
: lfield:    ( structure-sys "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field for a long word, 4 bytes, not aligned, return the field address )
  #bytes/long +field
;
[THEN]


[UNDEFINED] wfields: [IF]
: wfields:   ( structure-sys n "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field for n words, not aligned, return the field address ) 
  #bytes/word * +field
;
[THEN]


[UNDEFINED] lfields: [IF]
: lfields:    ( structure-sys n "<spaces>name" -- structure-sys ; addr1 -- addr2 = Create a structure field for n long words, not aligned, return the field address )
  #bytes/long * +field
;
[THEN]


( Fixed width store and fetch )

[UNDEFINED] w@ [DEFINED] overrule:w@ OR [IF]
: w@         ( w-addr -- u = Fetch a word, 16 bit, zero extend )
  @ [ hex ] FFFF [ decimal ] AND
;
[THEN]


[UNDEFINED] <w@ [IF]
: <w@        ( w-addr -- n = Fetch a word, 16 bit, sign extend )
  w@ dup [ hex ] 8000 [ decimal ] AND negate OR
;
[THEN]


[UNDEFINED] w! [IF]
bigendian? [IF]
: w!         ( n w-addr -- = Store a word, 16 bit )
  over 8 rshift over c!
  char+ c!
;
[ELSE]
: w!
  over 8 rshift over char+ c!
  c!
;
[THEN]
[THEN]


[UNDEFINED] w, [IF]
: w,  ( x -- = Store 16 bits of data in the data storage )
  here #bytes/word allot w!
;
[THEN]


[UNDEFINED] u>l [IF]
cell 4 = [IF]
: u>l  ( u -- l = Convert a unsigned number to 32 bit number )
; immediate
[ELSE]
: u>l  ( u -- l = Convert a unsigned number to 32 bit number )
  [ hex ] FFFFFFFF [ decimal ] AND
;
[THEN]
[THEN]


[UNDEFINED] l@ [IF]
cell 4 = [IF]
: l@         ( l-addr -- n = Fetch a long word, 32 bit, zero extend )
  state @ IF
    postpone @
  ELSE
    @
  THEN
; immediate
[ELSE]
: l@
  @ u>l
;
[THEN]
[THEN]


[UNDEFINED] <l@ [IF]
cell 4 = [IF]
: <l@        ( l-addr -- n = Fetch a long word, 32 bit, sign extend )
  state @ IF
    postpone @
  ELSE
    @
  THEN
; immediate
[ELSE]
: <l@
  l@ dup [ hex ] 80000000 [ decimal ] AND negate OR
;
[THEN]
[THEN]


[UNDEFINED] l! [IF]
cell 4 = [IF]
: l!         ( n l-addr -- = Store a long word, 32 bit )
  state @ IF
    postpone !
  ELSE
    !
  THEN
; immediate
[ELSE]
bigendian? [IF]
: l!
  over 24 rshift over c!
  char+
  over 16 rshift over c!
  char+
  over 8  rshift over c!
  char+
  c!
;
[ELSE]
: l!
  2dup c!
  char+
  over 8  rshift over c!
  char+
  over 16 rshift over c!
  char+
  swap 24 rshift swap c!
;
[THEN]
[THEN]
[THEN]


[UNDEFINED] l+! [IF]
cell 4 = [IF]
: l+!  ( l l-addr -- = Add l to l-addr )
  state @ IF
    postpone +!
  ELSE
    +!
  THEN
; immediate
[ELSE]
: l+!  ( l l-addr -- = Add l to l-addr )
  tuck l@ + swap l!
; 
[THEN]
[THEN]


[UNDEFINED] l, [IF]
cell 4 = [IF]
: l,  ( x -- = Store 32 bits of data in the data storage )
  state @ IF
    postpone ,
  ELSE
    ,
  THEN
; immediate
[ELSE]
: l,  ( x -- = Store 32 bits of data in the data storage )
  here #bytes/long allot l!
;
[THEN]
[THEN]


[UNDEFINED] llroll [IF]
cell 4 = [IF]
: llroll  ( l1 u2 -- l3 = Rotate l1 u2 bits to the left )
  state @ IF
    postpone lroll
  ELSE
    lroll
  THEN
; immediate
[ELSE]
: llroll  ( l1 u2 -- l3 = Rotate l1 u2 bits to the left )
  2dup lshift >r
  32 swap - rshift r>
  or
  u>l
;
[THEN]
[THEN]


[UNDEFINED] lrroll [IF]
cell 4 = [IF]
: lrroll  ( l1 u2 -- l3 = Rotate l1 u2 bits to the right )
  state @ IF
    postpone rroll
  ELSE
    rroll
  THEN
; immediate
[ELSE]
: lrroll  ( l1 u2 -- l3 = Rotate l1 u2 bits to the left )
  2dup rshift >r
  32 swap - lshift r>
  or
  u>l
;
[THEN]
[THEN]


[UNDEFINED] l@! [IF]
cell 4 = [IF]
: l@!  ( l1 l-addr -- l2 = Fetch l2 from l-addr and then store l1 at l-addr )
  state @ IF
    postpone @!
  ELSE
    @!
  THEN
; immediate
[ELSE]
: l@!  ( l1 l-addr -- l2 = Fetch l2 from l-addr and then store l1 at l-addr )
  dup l@ -rot l!
;
[THEN]
[THEN]

[THEN]

\ ==============================================================================
