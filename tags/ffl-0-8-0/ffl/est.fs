\ ==============================================================================
\
\           est - the string with escaped characters in the ffl
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
\  $Date: 2007-12-09 07:23:15 $ $Revision: 1.4 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] est.version [IF]

( est = String with escaped characters )
( The est module implements a string with escaped characters. The code is    )
( inspired by the proposal for escaped strings by Stephen Pelc and Peter     )
( Knaggs. The following conversion characters are translated:                )
( {{{                                                                        )
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
( }}}                                                                        )


1 constant est.version


( Private words )

create est-table
       7 c,     \ a
       8 c,     \ b
  char c c,     \ c
  char d c,     \ d
      27 c,     \ e
      12 c,     \ f
  char g c,     \ g
  char h c,     \ h
  char i c,     \ i
  char j c,     \ j
  char k c,     \ k
      10 c,     \ l
  char m c,     \ m
      10 c,     \ n
  char o c,     \ o
  char p c,     \ p
  char " c,     \ q
      13 c,     \ r
  char s c,     \ s
       9 c,     \ t
  char u c,     \ u
      11 c,     \ v
  char w c,     \ w
  char x c,     \ x
  char y c,     \ y
       0 c,     \ z
  
       
: est-add-char   ( char c-addr -- = Add the char to the counted string )
  tuck count + c!
  dup c@ 1+ swap c!
;


: est-add-2hex  ( c-addr1 u1 c-addr2 -- c-addr3 u3 = Read a two hex digit from string c-addr1 u1, convert it to a character and store it in c-addr2 and return the string after the two digits )
  base @ >r                  \ Save current base
  >r
  hex
  0 0 2over 2 min            \ Maximum convert 2 hex digits
  >r 
  r@ >number
  nip nip                    \ No interest in start of string and most significant cell
  r> swap -                  \ Get converted length
  swap r> est-add-char       \ Add result in string
  /string                    \ Update source string
  r> base !
;


[UNDEFINED] place [IF]
: place   ( c-addr1 u1 c-addr2 -- = Place string c-addr1 u1 at address c-addr2 as counted string )
  2dup c!
  char+ swap cmove
;
[THEN]


( String words )

[UNDEFINED] parse-esc [IF]
: parse-esc   ( c-addr1 u1 c-addr2 -- c-addr3 u3 = Parse the escaped character in string c-addr1 u1, store the result in string c-addr2 and return the remaining string c-addr3 u3 )
  over 0<> IF
    >r
    over c@
    dup [char] x = IF
      drop 
      1 /string r@ est-add-2hex
    ELSE
      dup [char] m = IF
        drop 
        13 r@ est-add-char
        10 r@ est-add-char
      ELSE
        dup [char] n = IF
          drop
          end-of-line count tuck r@ count + swap cmove r@ c@ + r@ c!
        ELSE
          dup [char] a [char] z 1+ within IF
            [char] a - chars est-table + c@
          THEN
          r@ est-add-char
        THEN
      THEN
      1 /string
    THEN
    rdrop
  ELSE
    drop
  THEN
;
[THEN]


[UNDEFINED] parse\" [DEFINED] overrule:parse\" OR [IF]
: parse\"   ( "ccc<quote>" -- c-addr u = Parse the input stream for a escaped string )
  source >in @ /string tuck
  pad >r
  0 r@ c!                    \ Length destination = 0
  BEGIN
    dup IF
      over c@ [char] " <>
    ELSE
      false
    THEN
  WHILE
    over c@ dup [char] \ = IF
      drop
      1 /string r@ parse-esc
    ELSE
      r@ est-add-char
      1 /string
    THEN
  REPEAT
  nip
  dup IF 1- THEN            \ Skip "
  - >in +!                  \ Store processed chars
  r> count                  \ Parsed string
  ;
[THEN]


[UNDEFINED] s\" [IF]
: s\"   ( "ccc<quote>" -- c-addr u = Create a string with escaped characters )
  parse\" state @ IF
    postpone sliteral
  ELSE
    dup chars allocate throw swap 2>r 2r@ cmove 2r>
  THEN
; immediate
[THEN]


[UNDEFINED] ,\" [IF]
: ,\"   ( "ccc<quote>" -- = Store a string with escaped characters in the dictionary )
  parse\"
  here 
  over char+ allot
  place align
;
[THEN]

[THEN]

\ ==============================================================================
