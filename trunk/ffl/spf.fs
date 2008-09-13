\ ==============================================================================
\
\                spf - the sprintf formatter in the ffl
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
\  $Date: 2008-09-13 13:34:03 $ $Revision: 1.2 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] spf.version [IF]

include ffl/str.fs


( spf = Sprintf string formatter )
( The spf module implements a string with escaped characters. The code is    )
( inspired by the proposal for escaped strings by Stephen Pelc and Peter     )
( Knaggs. The following conversion characters are translated:                )
( <pre>                                                                      )
( \a  - bel = ascii 7                                                        )
( \b  - backspace = ascii 8                                                  )
( \e  - escape = ascii 27                                                    )
( \f  - formfeed = ascii 12                                                  )
( \l  - linefeed = ascii 10                                                  )
( \m  - cr/lf = ascii 13,10                                                  )
( \n  - new line                                                             )
( \q  - quote = ascii 34                                                     )
( \r  - cr = ascii 13                                                        )
( \t  - ht = ascii 9                                                         )
( \v  - vt = ascii 11                                                        )
( \z  - nul = ascii 0                                                        )
( \"  - quote = ascii 34                                                     )
( xhh - hex digit                                                            )
( \\  - backslash                                                            )
( </pre>                                                                     )


1 constant spf.version


( Private bit fields )

1  constant spf.zero-padding   \ 0
2  constant spf.left-align     \ -
4  constant spf.space-sign     \ ' '
8  constant spf.plus-sign      \ +
16 constant spf.double         \ l


( Private words )

: spf+flags        ( c-addr1 u1 -- c-addr2 u2 n1 = Read the flags from the format string )
  0 >r
  true                  
  BEGIN 
    over AND
  WHILE
    over c@ 
    dup [char] 0 = IF   \ Zero padding
      drop
      r> spf.zero-padding OR >r
      1 /string
      true
    ELSE dup [char] - = IF   \ Left align
      drop
      r> spf.left-align OR >r
      1 /string
      true
    ELSE dup bl = IF         \ Space sign
      drop
      r> spf.space-sign OR >r
      1 /string
      true
    ELSE [char] + = IF       \ Plus sign
      r> spf.plus-sign OR >r
      1 /string
      true
    ELSE
      false
    THEN THEN THEN THEN
  REPEAT
  r>
;

: spf+length       ( c-addr1 u1 -- c-addr2 u2 n1 = Process the length in the format string )
  0 >r
  BEGIN
    dup IF
      over c@ chr-digit? 
    ELSE
      false
    THEN
  WHILE                      \ while length > 0 and char is digit
    over c@ [char] 0 -
    
    r> 10 * + >r             \   convert digit and add to length

    1 /string
  REPEAT
  r>
;


: spf+double       ( c-addr1 u1 -- c-addr2 u2 n1 = Process the double flag in the format string )
  dup IF
    over c@ [char] l = IF
      1 /string
      spf.double
    ELSE
      0
    THEN
  ELSE
    0
  THEN
;


: spf+sign         ( n1 n2 -- char = Use the flags n1 and number n2 to determine the sign char )
  0< IF
    [char] -
  ELSE
    dup spf.plus-sign AND IF
      drop
      [char] +
    ELSE spf.space-sign AND IF
      bl
    ELSE
      0
    THEN THEN
  THEN
;


: spf-left-pad     ( n1 n2 str -- = Pad n2 zeros or spaces if indicated by n1 )
  over IF
    rot dup spf.left-align AND IF
      spf.zero-padding AND IF
        [char] 0
      ELSE
        bl
      THEN
      -rot str-append-chars
    ELSE
      2drop drop
    THEN
  ELSE
    2drop drop
  THEN
;


: spf-right-pad    ( n1 n2 str -- = Pad n2 spaces if indicated by n1 )
  over IF
    rot spf.left-align AND IF
      bl -rot str-append-chars
    ELSE
      2drop drop
    THEN
  ELSE
    2drop drop
  THEN
;


: spf+specifier    ( i*x n1 n2 str c-addr u -- j*x c-addr u = Process the specifier in c-addr u with length n1, flags n2 and parameters i*x )
  dup IF
    over c@ -rot
    1 /string
    2>r
    CASE
      [char] % OF [char] % swap str-append-char 2drop ENDOF  \ %% specifier
      \ ToDo more
      >r [char] ? swap str-append-char 2drop r>              \ Unknown specifier
    ENDCASE
    2r>
  ELSE
    2>r drop 2drop 2r>
  THEN
;


( Sprintf words )

: spf-append    ( i*x c-addr u str -- = Convert the arguments i*x with the format string c-addr u and append the result to str )
  >r
  BEGIN
    dup
  WHILE
    over c@ 
    dup [char] % = IF
      drop
      1 /string

      spf+flags >r spf+length -rot spf+double r> OR -rot \ Process flags, length and double indication

      r@ -rot spf+specifier  \ Process the specifier
    ELSE
      r@ str-append-char

      1 /string
    THEN
  REPEAT
  2drop
  rdrop
;


: spf-set       ( i*x c-addr u str -- = Convert the arguments i*x with the format string c-addr u and set the result in str )
  dup str-clear spf-append
;

[THEN]

\ ==============================================================================
