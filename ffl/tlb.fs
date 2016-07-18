\ ==============================================================================
\
\                   tlb - the toolbelt in the ffl
\
\              Copyright (C) 2015 Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU Lesser General Public
\ License as published by the Free Software Foundation; either
\ version 3 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ Lesser General Public License for more details.
\
\ You should have received a copy of the GNU Lesser General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2009-05-23 05:37:24 $ $Revision: 1.20 $
\
\ ==============================================================================
\
\ This toolbelt file contains general purpose forth words that are not always
\ present in forth engines. It is inspired by the toolbelt of Wil Baden.
\
\ ==============================================================================


( tlb = Toolbelt forth words )
( The tlb module contains general purpose forth words. )


[UNDEFINED] cell [IF]
: cell             ( - n = Cell size)
  1 cells
;
[THEN]


[UNDEFINED] char/ [IF]
1 chars 1 = [IF]
: char/            ( n:aus - n:chars = Convert address units to chars )
; immediate
[ELSE]
: char/
  1 chars /
;
[THEN]
[THEN]


[UNDEFINED] 3dup [IF]
: 3dup    ( x1 x2 x3 -- x1 x2 x3 x1 x2 x3 = Duplicate the three cells on stack )
  dup 2over rot
;
[THEN]


[UNDEFINED] 4dup [IF]
: 4dup    ( x1 x2 x3 x4 -- x1 x2 x3 x4 x1 x2 x3 x4 = Duplicate the four cells on stack )
  2over 2over
;
[THEN]


[UNDEFINED] 3drop [IF]
: 3drop   ( x1 x2 x3 -- = Drop the three cells on stack )
  2drop drop
;
[THEN]


[UNDEFINED] 4drop [IF]
: 4drop   ( x1 x2 x3 x4 -- = Drop the four cells on stack )
  2drop 2drop
;
[THEN]


[UNDEFINED] lroll [IF]
: lroll   ( u1 u2 -- u3 = Rotate u1 u2 bits to the left )
  2dup lshift >r
  #bits/cell swap - rshift r>
  or
;
[THEN]


[UNDEFINED] rroll [IF]
: rroll   ( u1 u2 -- u3 = Rotate u1 u2 bits to the right )
  2dup rshift >r
  #bits/cell swap - lshift r>
  or
;
[THEN]


[UNDEFINED] 0! [IF]
: 0!   ( a-addr -- = Set address to zero )
  0 swap !
;
[THEN]


[UNDEFINED] on [IF]
: on               ( w - = Set boolean variable to true)
  true swap !
;
[THEN]


[UNDEFINED] off [IF]
: off              ( w - = Set boolean variable to false)
  false swap !
;
[THEN]


[UNDEFINED] u<> [IF]
: u<>              ( u u - f = Check for not equal )
  <>
;
[THEN]


[UNDEFINED] u<= [IF]
: u<=              ( u u - f = Check for smaller and equal )
  u> 0=
;
[THEN]


[UNDEFINED] u>= [IF]
: u>=
  u< 0=
;
[THEN]


[UNDEFINED] 0>= [IF]
: 0>=              ( n - f = Check for equal and greater zero )
  0< 0=
;
[THEN]


[UNDEFINED] 0<= [IF]
: 0<=
  0> 0=
;
[THEN]


[UNDEFINED] >= [IF]
: >=               ( n n - f = Check for greater equal )
  < 0=
;
[THEN]


[UNDEFINED] <= [IF]
: <=               ( n n - f = Check for smaller equal )
  > 0=
;
[THEN]


[UNDEFINED] d<> [IF]
: d<>              ( d d - f = Check if two two double are unequal )
  d= 0=
;
[THEN]


[UNDEFINED] nil [IF]

0 CONSTANT nil

[THEN]


[UNDEFINED] nil! [IF]
: nil!   ( a-addr -- = Set address to nil )
  nil swap !
;
[THEN]


[UNDEFINED] nil= [IF]
: nil=   ( addr -- flag = Check for nil )
  nil =
;
[THEN]


[UNDEFINED] nil<> [IF]
: nil<>   ( addr -- flag = Check for unequal to nil )
  nil <>
;
[THEN]


[UNDEFINED] nil<>? [IF]

0 nil= [IF]
: nil<>?    ( addr -- false | addr true = If addr is nil, then return false, else return address with true )
  state @ IF
    postpone ?dup
  ELSE
    ?dup
  THEN
; immediate
[ELSE]
: nil<>?
  dup nil<> IF
    true
  ELSE
    drop
    false
  THEN
;
[THEN

[THEN]


[UNDEFINED] ?free [IF]
: ?free   ( addr -- ior = Free the address if not nil )
  dup nil<> IF
    free
  ELSE
    drop 0
  THEN
;
[THEN]


[UNDEFINED] 1+! [IF]
: 1+!   ( a-addr -- = Increase contents of address by 1 )
  1 swap +!
;
[THEN]


[UNDEFINED] 1-! [IF]
: 1-!   ( a-addr -- = Decrease contents of address by 1 )
  -1 swap +!
;
[THEN]


[UNDEFINED] @! [IF]
: @!   ( x1 a-addr -- x2 = First fetch the contents x2 and then store value x1 )
  dup @ -rot !
;
[THEN]


[UNDEFINED] sgn [IF]
: sgn              ( n1 - n2 = Determine the sign of the number )
  -1 max 1 min
;
[THEN]


[UNDEFINED] toupper [IF]
: toupper          ( char1 -- char2 = Convert the character to upper case )
  dup [char] a >= over [char] z <= AND IF
    [ char a char A - ] literal -
  THEN
;
[THEN]


[UNDEFINED] icompare [IF]
: icompare   ( c-addr1 u1 c-addr2 u2 -- n = Compare case-insensitive two strings and return the result [-1,0,1] )
  rot swap 2swap 2over
  min 0 ?DO
    over c@ toupper over c@ toupper - sgn ?dup IF
      >r 2drop 2drop r>
      unloop 
      exit
    THEN
    1 chars + swap 1 chars + swap
  LOOP
  2drop
  - sgn
;
[THEN]


[UNDEFINED] bounds [IF]
: bounds           ( c-addr u - c-addr+u c-addr = Get end and start address for ?do )
  over + swap
;
[THEN]


[UNDEFINED] ud. [IF]
: ud.              ( ud -- = Type the unsigned double number )
  <# #s #> type
;
[THEN]


[UNDEFINED] <=> [IF]
: <=>   ( n1 n2 -- n = Compare the two numbers and return the compare result [-1,0,1] )
  2dup = IF 
    2drop 0 EXIT 
  THEN
  < 2* 1+
;
[THEN]


[UNDEFINED] index2offset [IF]
: index2offset   ( n1 n2 -- n3 = Convert the index n1 range [-n2..n2> into offset n3 range [0..n2>, negative values of n1 downward length n2 )
  over 0< IF
    +
  ELSE
    drop
  THEN
;
[THEN]


[UNDEFINED] rdrop [IF]
: rdrop            ( R: x -- = Drop the first cell on the return stack )
  postpone r> postpone drop
; immediate
[THEN]


[UNDEFINED] r'@ [IF]
: r'@              ( R: x1 x2 -- x1 x2; -- x1 = Fetch the second cell on the return stack )
  postpone 2r@ postpone drop
; immediate
[THEN]


[DEFINED] float [IF]

( Float extension words )

[UNDEFINED] f-rot [IF]
: f-rot            ( F: r1 r2 r3 -- r3 r1 r2 = Rotate counter clockwise three floats )
  frot frot
;
[THEN]


[UNDEFINED] f2dup [IF]
: f2dup            ( F: r1 r2 -- r1 r2 r1 r2 = Duplicate two floats )
  fover fover
;
[THEN]


[UNDEFINED] ftuck [IF]
: ftuck
  fswap fover
;
[THEN]


[UNDEFINED] f= [IF]
: f=
  f- f0=
;
[THEN]


[UNDEFINED] f>r [IF]
: f>r              ( F: r -- ; R: -- r = Push float on the return stack )
  r> rp@ float - rp! rp@ f! >r 
;
[THEN]


[UNDEFINED] fr> [IF]
: fr>              ( F: -- r ; R: r -- = Pop float from the return stack )
  r> rp@ f@ float rp@ + rp! >r
;
[THEN]


[UNDEFINED] fr@ [IF]
: fr@              ( F: -- r ; R: r -- r = Get float from top of return stack )
  r> rp@ f@ >r
;
[THEN]


[THEN]

\ ==============================================================================

