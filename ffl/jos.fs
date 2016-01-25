\ ==============================================================================
\
\                jos - the json writer in the ffl
\
\               Copyright (C) 2010  Dick van Oudheusden
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
\  $Date: 2008-01-13 08:09:33 $ $Revision: 1.7 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] jos.version [IF]

include ffl/tos.fs
include ffl/car.fs
include ffl/enm.fs

( jos = JSON output stream )
( The jos module implements an JSON [Javascript Object Notation] writer.     )
( JSON is a lightweight data interchange format. See                         )
( <a href='http://www.json.org'>json.org</a>. The jos module extends the     )
( <a href='tos.html'>tos</a> module with extra words, so the tos words can   )
( be used on a jos variable. The module checks the use of the writer words   )
( in relation to the syntax. If a word is used outside the syntax, an        )
( exp-invalid-state exception is thrown.                                     )


1 constant jos.version


( JSON output stream structure )

begin-structure jos%   ( -- n = Get the required space for a jos writer variable )
  tos%
  +field  jos>tos       \ the jos writer extends the text output stream
  car%
  +field  jos>stack     \ the state stack
  field:  jos>state     \ the current state
end-structure


( JSON output stream creation, initialisation and destruction )

: jos-init  ( jos -- = Initialise an empty output stream )
  dup tos-init               \ Initialise the base stream
  16 
  over jos>stack car-init    \ Initialise the state stack with 16 entries
       jos>state  0!         \ Initial state
;


: jos-(free)       ( tos -- = Free the stream data from the heap )
  dup jos>stack car-(free)
  tos-(free)
;


: jos-create       ( "<spaces>name" -- ; -- jos = Create a named JSON output stream in the dictionary )
  create   here   jos% allot   jos-init
;


: jos-new          ( -- jos = Create a new JSON output stream on the heap )
  jos% allocate  throw  dup jos-init
;


: jos-free         ( jos -- = Free the JSON output stream from the heap )
  dup jos-(free)
  
  free throw
;


( Private words )

begin-enumeration  \ constants for the jos.state
  1 >enum:  jos.first     \ first
  2 >enum:  jos.object    \ object
  4 >enum:  jos.array     \ array
  6 >enum:  jos.type      \ type = object or array
  8 >enum:  jos.name      \ name
 16 >enum:  jos.value     \ value
 24 >enum:  jos.field     \ field = name or value
 30 >enum:  jos.typefield \ type & field mask
end-enumeration


( private words )

: jos-start-name  ( jos -- = Start with a json name )
  >r
  r@ jos>state @

  dup jos.typefield AND jos.object jos.name OR <> exp-invalid-state AND throw \ state = object & name

  dup jos.first AND IF
    jos.first invert AND            \ if first then !first
  ELSE
    [char] , r@ tos-write-char      \ else write ,
  THEN

  jos.name invert AND jos.value OR  \ state = value
  r> jos>state !
;


: jos-start-value  ( jos -- = Write a json value )
  >r
  r@ jos>state @

  dup jos.field AND jos.value <> exp-invalid-state AND throw  \ state = value ?

  dup jos.type AND jos.array = IF       \ if array
    dup jos.first AND IF                \  if first then !first
      jos.first invert AND
    ELSE
      [char] , r@ tos-write-char        \  else write ,
    THEN
  ELSE dup jos.type AND jos.object = IF \ else if object
    jos.value invert AND jos.name OR    \   !value, name
  THEN THEN

  r> jos>state !
;


: jos-push-state  ( jos -- = Save the current state on the stack)
  dup jos>state @ swap jos>stack car-push
;


: jos-pop-state  ( jos -- = Restore the previous state )
  dup jos>stack car-pop swap jos>state !
;


: jos-write-qstring  ( c-addr u jos -- = Write a quoted string )
  >r
  [char] " r@ tos-write-char
  BEGIN
    dup
  WHILE
    over c@ CASE
      [char] " OF [char] " true ENDOF
      [char] / OF [char] / true ENDOF
      [char] \ OF [char] \ true ENDOF
             8 OF [char] b true ENDOF
            12 OF [char] f true ENDOF
            10 OF [char] n true ENDOF
            13 OF [char] r true ENDOF
             9 OF [char] t true ENDOF \ ToDo unicode
      false over
    ENDCASE                           \ S: c-addr u ch escape?
    IF
      [char] \ r@ tos-write-char
    THEN
    r@ tos-write-char
    1 /string
  REPEAT
  2drop
  [char] " r> tos-write-char
;


( json writer words )

: jos-write-start-object   ( jos -- = Write the start of an object )
  >r
  r@ jos>state @ jos.type AND IF
    r@ jos-start-value
  THEN

  r@ jos-push-state

  jos.object jos.name OR jos.first OR r@ jos>state !  \ state = object & first & name
  [char] { r> tos-write-char
;


: jos-write-end-object  ( jos -- = Write the end of an object )
  >r
  r@ jos>state @ jos.typefield AND jos.object jos.name OR <> exp-invalid-state AND throw \ state = object & name ?

  [char] } r@ tos-write-char

  r> jos-pop-state
;


: jos-write-start-array  ( jos -- = Write the start of an array )
  >r
  r@ jos>state @ jos.type AND IF
    r@ jos-start-value
  THEN
  
  r@ jos-push-state                      \ save state
  
  jos.array jos.value OR jos.first OR r@ jos>state !  \ state = array & first & value
  [char] [ r> tos-write-char
;


: jos-write-end-array  ( jos -- = Write the end of an array )
  >r
  r@ jos>state @ jos.typefield AND jos.array jos.value OR <> exp-invalid-state AND throw  \ state = array & value ?

  [char] ] r@ tos-write-char

  r> jos-pop-state
;


: jos-write-name    ( c-addr u jos -- = Write a name )
  >r
  r@ jos-start-name
  r@ jos-write-qstring
  [char] : r@ tos-write-char
  rdrop
;


: jos-write-string  ( c-addr u jos -- = Write a string value )
  >r
  r@ jos-start-value
  r@ jos-write-qstring
  rdrop
;


: jos-write-number  ( n jos -- = Write a number as value)
  dup
  jos-start-value
  tos-write-number
;


: jos-write-double  ( d jos -- = Write a double as value )
  dup
  jos-start-value
  tos-write-double
;


[DEFINED] tos-write-float [IF]
: jos-write-float  ( r jos -- = Write a float as value )
  dup
  jos-start-value
  tos-write-float
;
[THEN]


: jos-write-boolean  ( flag jos -- = Write a boolean as value )
  >r
  r@ jos-start-value

  IF
    s" true"
  ELSE
    s" false"
  THEN

  r> tos-write-string
;


: jos-write-nil  ( jos -- = Write the nil pointer as value )
  >r
  r@ jos-start-value
  s" null" r> tos-write-string
;


: jos-dump  ( jos -- = Dump the jos variable )
  ." jos:" dup . cr
  dup jos>tos   tos-dump
  dup jos>stack car-dump
  ." state:" jos>state ? cr
;

[THEN]

\ ==============================================================================
