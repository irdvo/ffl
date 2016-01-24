\ ==============================================================================
\
\                  jis - the json reader in the ffl
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
\  $Date: 2008-02-21 20:31:19 $ $Revision: 1.16 $
\
\ ==============================================================================

include ffl/config.fs

[UNDEFINED] jis.version [IF]

include ffl/enm.fs
include ffl/tis.fs
include ffl/car.fs


( jis = JSON input stream )
( The jis module implements a validating JSON parser. Feeding of the parser  )
( can be done with two words: jis-set-string for feeding the parser          )
( with strings and jis-set-reader for feeding the parser through an          )
( execution token. This token should have the following stack behavior:      )
( <pre>                                                                      )
(    x -- c-addr u | 0                                                       )
( </pre>                                                                     )
( x is a word indicating the context of the reader and is identical to       )
( first parameter of jis-set-reader. For example this value is the file      )
( descriptor during reading of a file. The execution token returns, if       )
( successful, the read data in c-addr u.                                     )
( The input is parsed by calling the jis-read word repeatedly until it       )
( returns jis.error or jis.done. The word returns one of the following       )
( results with its stack parameters:                                         )
( <pre>                                                                      )
( jis.error          --             = Error                                  )
( jis.done           --             = Stream is correctly parsed             )
( jis.start-object   --             = Start of new object                    )
( jis.end-object     --             = End of object                          )
( jis.start-array    --             = Start of new array                     )
( jis.end-array      --             = End of array                           )
( jis.name           -- c-addr u    = Name as string                         )
( jis.string         -- c-addr u    = String value                           )
( jis.number         -- n           = Number value                           )
( jis.double         -- d           = Double value                           )
( jis.float          -- r           = Float value                            )
( jis.boolean        -- flag        = Boolean value                          )
( jis.null           --             = Null value                             )
( </pre>                                                                     )

1 constant jis.version


( json reader constants )

begin-enumeration
  -1
  >enum: jis.error          ( -- n = Error          )
  enum:  jis.done           ( -- n = Done reading   )
  enum:  jis.start-object   ( -- n = Start object   )
  enum:  jis.start-array    ( -- n = Start array    )
  enum:  jis.name           ( -- n = Name           )
  enum:  jis.string         ( -- n = String value   )
  enum:  jis.number         ( -- n = Number value   )
  enum:  jis.double         ( -- n = Double value   )
  enum:  jis.float          ( -- n = Float value    )
  enum:  jis.boolean        ( -- n = Boolean value  )
  enum:  jis.null           ( -- n = Null value     )
  enum:  jis.end-object     ( -- n = End object     )
  enum:  jis.end-array      ( -- n = End array      )
end-enumeration


( Private values )

0 value jis.parse-start       \ xt of jis-parse-start word
0 value jis.parse-first-value \ xt of jis-parse-first-value word
0 value jis.parse-first-pair  \ xt of jis-parse-first-pair
0 value jis.parse-pair-value  \ xt of jis-parse-pair-value


( json reader structure )

begin-structure jis%   ( -- n = Get the required space for a jis reader variable )
  tis%
  +field  jis>tis       \ the tis reader (extends the text input stream)
  field:  jis>state     \ the state execution token
  car%
  +field  jis>stack     \ the state stack
  str%
  +field  jis>string    \ the parsed string
end-structure


( json reader variable creation, initialisation and destruction )

: jis-init   ( jis -- = Initialise the json reader variable )
  dup  jis>tis tis-init
  16
  over jis>stack car-init
  dup  jis>string str-init
  jis.parse-start
  swap jis>state !
;


: jis-(free)   ( jis -- = Free the internal, private variables from the heap )
  dup 
  jis>tis    tis-(free)
  dup
  jis>stack  car-(free)
  jis>string str-(free)
;

    
: jis-create   ( "<spaces>name" -- ; -- jis = Create a named json reader variable in the dictionary )
  create  here jis% allot  jis-init
;


: jis-new   ( -- jis = Create a new json reader variable on the heap )
  jis% allocate  throw   dup jis-init
;


: jis-free   ( jis -- = Free the jis reader variable from the heap )
  dup jis-(free)
  
  free throw
;


( json reader init words )

: jis-set-reader  ( x xt jis -- = Init the json parser for reading using the reader callback xt with its data x )
  tis-set-reader
;


: jis-set-string  ( c-addr u jis -- = Init the json parser for for reading from the string c-addr u )
  tis-set
;


( Private state words )

: jis-skip-spaces  ( jis -- = Skip all whitespace in the stream )
  >r
  BEGIN
    r@ tis-fetch-char IF
      chr-space?
    ELSE
      false
    THEN
  WHILE
    r@ tis-next-char
  REPEAT
  rdrop
;


: jis-state!  ( n jis -- = Set the next state n )
  jis>state !
;


: jis-push-state  ( n jis -- = Push the current state and set the next state n )
  tuck jis>state @!               \ Fetch the current state and set the next state
  swap jis>stack car-push         \ Push the current state
;


: jis-pop-state  ( jis -- = Pop the state, use parse-start if empty )
  dup jis>stack
  dup car-length@ IF
    car-pop
  ELSE
    drop jis.parse-start
  THEN
  swap jis-state!
;


( Private reader words )

: jis-parse-unicode  ( jis -- ch = Parse an unicode character )
  4 swap tis-read-string ?dup IF \ Read 4 hex digits
    base @ >r hex                
    0. swap >number 2drop d>s    \ Convert to a number 
    r> base ! 
  ELSE
    [char] u
  THEN
;


: jis-parse-string  ( jis -- false | c-addr u true = Parse a string )
  >r
  r@ jis>string str-clear
  
  BEGIN
    r@ tis-read-char IF
      dup [char] " <>             \ Read till " or no more chars
    ELSE
      0 false
    THEN
  WHILE
    dup [char] \ = IF
      r@ tis-read-char IF
        nip
        CASE
          [char] b OF chr.bs ENDOF
          [char] f OF chr.ff ENDOF
          [char] n OF chr.lf ENDOF
          [char] r OF chr.cr ENDOF
          [char] t OF chr.ht ENDOF
          [char] u OF r@ jis-parse-unicode ENDOF 
          dup                     \ Default incl. \\ \/ and \"
        ENDCASE
      THEN
    THEN
    r@ jis>string str-append-char \ Append char to string
  REPEAT
  
  [char] " = IF                   \ If closing " then success
    r@ jis>string str-get true
  ELSE
    false
  THEN
  rdrop
;


: jis-parse-name  ( jis -- i*x n = Parse a name )
  >r
  r@ jis-skip-spaces
  [char] " r@ tis-cmatch-char IF
    r@ jis-parse-string IF
      jis.name
    ELSE
      jis.error
    THEN
  ELSE
    jis.error
  THEN
  rdrop
;


[DEFINED] tis-read-float [IF]
: jis-parse-number  ( jis -- jis.error | r jis.float | d jis.double | n jis.number = Parse a number )
  tis-read-float IF
    fdup fdup f>d d>f f= IF       \ Is it a double or number ?
      f>d
      2dup 2dup d>s s>d d= IF     \ Is it a number
        d>s jis.number
      ELSE
        jis.double
      THEN
    ELSE
      jis.float                   \ No, float
    THEN
  ELSE
    jis.error
  THEN
;
[ELSE]
: jis-parse-number  ( jis -- jis.error | r jis.float | d jis.double | n jis.number = Parse a number )
  tis-read-double IF
    2dup 2dup d>s s>d d= IF     \ Is it a double ?
      jis.double
    ELSE
      s>d jis.number            \ Else number
    THEN
  ELSE
    jis.error
  THEN
;
[THEN]


: jis-parse-value  ( jis -- i*x n = Parse a value )
  >r
  r@ jis-skip-spaces
  s" true" r@ tis-cmatch-string IF  \ Check for true value
    true jis.boolean
  ELSE
    s" false" r@ tis-cmatch-string IF  \ Check for false value
      false jis.boolean
    ELSE
      s" null" r@ tis-cmatch-string IF  \ Check for null value
        jis.null
      ELSE
        [char] " r@ tis-cmatch-char IF  \ Check for string value
          r@ jis-parse-string IF
            jis.string
          ELSE
            jis.error
          THEN
        ELSE
          [char] { r@ tis-cmatch-char IF  \ } Check for start of object
            jis.parse-first-pair r@ jis-push-state
            jis.start-object
          ELSE
            [char] [ r@ tis-cmatch-char IF  \ ] Check for start of array
              jis.parse-first-value r@ jis-push-state
              jis.start-array
            ELSE
              r@ jis-parse-number \ Check for float, double or number
            THEN
          THEN
        THEN
      THEN
    THEN
  THEN
  rdrop
;


: jis-parse-next-value  ( jis -- i*x n  = Parse the next value of an array )
  >r
  r@ jis-skip-spaces
  [char] ] r@ tis-cmatch-char IF  \ Check for end of array
    r@ jis-pop-state
    jis.end-array
  ELSE
    [char] , r@ tis-cmatch-char IF  \ Check for ,
      r@ jis-parse-value dup jis.error = IF \ Check for value
        jis.parse-start r@ jis-state!
      THEN
    ELSE
      jis.parse-start r@ jis-state!
      jis.error
    THEN
  THEN
  rdrop
;


: jis-parse-first-value  ( jis -- i*x n = Parse the first value of an array )
  >r
  r@ jis-skip-spaces
  [char] ] r@ tis-cmatch-char IF  \ Check for end of array
    r@ jis-pop-state
    jis.end-array
  ELSE
    ['] jis-parse-next-value r@ jis-state!  \ Default next state
    r@ jis-parse-value dup jis.error = IF   \ Check for value
      jis.parse-start r@ jis-state!
    ELSE
    THEN
  THEN
  rdrop
;
' jis-parse-first-value to jis.parse-first-value


: jis-parse-next-pair  ( jis -- i*x n = Parse the next pair of an object )
  >r
  r@ jis-skip-spaces
  [char] } r@ tis-cmatch-char IF  \ Check for end of object
    r@ jis-pop-state
    jis.end-object
  ELSE
    [char] , r@ tis-cmatch-char IF
      r@ jis-skip-spaces
      [char] " r@ tis-cmatch-char IF
        r@ jis-parse-string IF
          jis.name
          jis.parse-pair-value
        ELSE
          jis.error
          jis.parse-start
        THEN
      ELSE
        jis.error
        jis.parse-start
      THEN
    ELSE
      jis.error
      jis.parse-start
    THEN
    r@ jis-state!
  THEN
  rdrop
;


: jis-parse-pair-value  ( jis -- i*x n = Parse the value of a pair )
  >r
  r@ jis-skip-spaces
  [char] : r@ tis-cmatch-char IF
    ['] jis-parse-next-pair r@ jis-state! \ Default: next-state = parse-next-pair
    r@ jis-parse-value dup jis.error = IF \ Check for value, update next state
      jis.parse-start r@ jis-state!
    THEN
  ELSE
    jis.parse-start r@ jis-state!
    jis.error
  THEN
  rdrop
;
' jis-parse-pair-value to jis.parse-pair-value


: jis-parse-first-pair  ( jis -- i*x n = Parse the first pair of an object )
  >r
  r@ jis-skip-spaces
  [char] } r@ tis-cmatch-char IF  \ Check for end of object
    r@ jis-pop-state
    jis.end-object
  ELSE
    [char] " r@ tis-cmatch-char IF  \ " Check for name
      r@ jis-parse-string IF
        jis.name                    \ Name found
        ['] jis-parse-pair-value
      ELSE
        jis.error
        jis.parse-start             \ Error
      THEN
    ELSE
      jis.error
      jis.parse-start               \ Error
    THEN
    r@ jis-state!
  THEN
  rdrop
;
' jis-parse-first-pair to jis.parse-first-pair


: jis-parse-start  ( jis -- i*x n = Start reading json tokens )
  >r
  r@ jis>stack car-clear          \ Clear the state stack
  r@ jis-skip-spaces
  r@ tis-eof? IF
    jis.done
  ELSE
    [char] { r@ tis-cmatch-char IF  \ Check for start of object }
      ['] jis-parse-first-pair r@ jis-push-state
      jis.start-object
    ELSE
      [char] [ r@ tis-cmatch-char IF \ Check for start of array
        ['] jis-parse-first-value r@ jis-push-state
        jis.start-array
      ELSE
        jis.error
      THEN
    THEN
  THEN
  rdrop
;
' jis-parse-start to jis.parse-start


( json reader word )

: jis-read ( jis -- i*x n = Read the next json token n with its parameters from the source &lb;see json reader constants&rb; )
  dup tis-reduce                            \ Keep the stream compact
  dup jis>state @ execute
;


: jis+remove-read-parameters  ( i*x n -- = Remove the various parameters of a json token after calling jis-read &lb;see json reader constants&rb; )
  CASE
    jis.error        OF       ENDOF
    jis.done         OF       ENDOF
    jis.start-object OF       ENDOF
    jis.start-array  OF       ENDOF
    jis.name         OF 2drop ENDOF
    jis.string       OF 2drop ENDOF
    jis.number       OF drop  ENDOF 
    jis.double       OF 2drop ENDOF
    jis.float        OF fdrop ENDOF
    jis.boolean      OF drop  ENDOF
    jis.null         OF       ENDOF
    jis.end-object   OF       ENDOF
    jis.end-array    OF       ENDOF
  ENDCASE
;


( Inspection )

: jis-dump  ( jis -- = Dump the json input stream )
  ." jis:" dup . cr
  dup jis>tis     tis-dump
  ." state: " dup jis>state ? cr
  dup jis>stack   car-dump
      jis>string  str-dump
;

[THEN]

\ ==============================================================================
