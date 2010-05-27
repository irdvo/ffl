\ ==============================================================================
\
\                scf - the sscanf scanner in the ffl
\
\               Copyright (C) 2009  Dick van Oudheusden
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
\  $Date: 2008-09-22 18:46:53 $ $Revision: 1.4 $
\
\ ==============================================================================


include ffl/config.fs

include ffl/chr.fs

[UNDEFINED] scf.version [IF]


( scf = sscanf scanner )
( The scf module implements a simplified version of C's sscanf function.     )
( The words in this module expect a format string with specifiers &lb;see    )
( below for the format&rb;. For every specifier &lb;except %%&rb; the string )
( is scanned and converted to a stack item. The white space character in the )
( specifier string will match repeatedly all white space in the source       )
( string. All other characters must be matched in the source string.         )
( {{{                                                                        )
( Format: %[double]specifier                                                 )
(    Double: l      = the argument is interpreted as a double                )
( Specifier: c      = scan a character [char]                                )
(            d      = scan a signed number [n or d]                          )
(            o      = scan an unsigned octal [n or d]                        )
(            s      = scan a string [c-addr u]                               )
(            u      = scan an unsigned number [u or ud]                      )
(            x      = scan an unsigned hexadecimal number [u or ud]          )
(            X      = scan an unsigned hexadecimal number [u or ud]          )
(            e      = scan a float number [r]                                )
(            E      = scan a float number [r]                                )
(            q      = scan a quoted string &lb;non sprintf&rb; [c-addr u]    )
(            %      = match a '%' []                                         )
( }}}                                                                        )


1 constant scf.version


( Private flags )

1  constant scf.unsigned       \ u x X
2  constant scf.double         \ l
4  constant scf.minus-sign     \ -


( Private state )

0  constant scf.error
1  constant scf.matched
2  constant scf.scanned

1  constant scf.fsign
2  constant scf.fdigit
4  constant scf.fpoint
8  constant scf.fexp


( Private scan words )

: scf+skip-spaces  ( c-addr1 u1 -- c-addr2 u2 = Skip spaces in the source string )
  BEGIN
    dup IF 
      over c@ chr-space?
    ELSE 
      false 
    THEN
  WHILE
    1 /string
  REPEAT
;


: scf+match         ( c-addr1 u1 char -- c-addr2 u2 n = Match the character in the source string, resulting in state scf.error or scf.matched )
  over IF
    >r over c@ r> = IF
      1 /string 
      scf.matched
    ELSE
      scf.error
    THEN
  ELSE
    drop
    scf.error
  THEN
;


: scf+scan-char    ( c-addr1 u1 -- char c-addr2 u2 scf.scanned | c-addr2 u2 scf.error = Scan the source string for a char )
  dup IF
    over c@
    -rot 1 /string
    scf.scanned
  ELSE
    scf.error
  THEN
;


: scf+scan-number  ( c-addr1 u1 n1 n2 -- n|u|d|ud c-addr2 u2 scf.scanned | c-addr2 u2 scf.error = Scan the source string for a number with flags n1 and base n2 )
  base @ >r
  base !
  >r                              \ flags
  scf+skip-spaces
  dup IF
    over c@  dup [char] - = IF    \ if leading minus sign, then update flags
      drop 1 /string r> scf.minus-sign OR >r 
    ELSE
      [char] + = IF               \ if leading plus sign, ignore
        1 /string
      THEN
    THEN
    
    0. 2over >number              \ convert the number to a string
    2rot 2over d= IF              \ if not a number is converted then
      2swap 2drop                 \   no success
      scf.error
    ELSE                          \ else
      2swap
      r@ scf.minus-sign AND IF     \   update the sign
        dnegate
      THEN
      r@ scf.double AND IF         \    convert to correct type
        2swap
      ELSE
        r@ scf.unsigned AND IF
          drop
        ELSE
          d>s
        THEN
        -rot
      THEN
      scf.scanned
    THEN
  ELSE
    scf.error
  THEN
  rdrop
  r> base !
;


: scf+split-float  ( c-addr1 u1 -- c-addr2 u2 c-addr3 u3 = Check string c-addr1 u1 for a float number string c-addr3 u3 and remaining string c-addr2 u2 )
  0 >r                            \ flags
  2dup
  BEGIN
    over 0= IF                    \ Ready when string is empty
      true
    ELSE
      over c@
      dup [char] + = over [char] - = OR IF
        drop r@ scf.fsign AND IF  \ Second sign -> done
          true
        ELSE
          r> scf.fsign OR >r  1 /string  false
        THEN
      ELSE dup chr-digit? IF
        drop r> scf.fsign OR scf.fdigit OR >r  1 /string false
      ELSE dup [char] . = IF
        drop r@ scf.fpoint AND IF \ Second point -> done
          true
        ELSE
          r> scf.fpoint OR >r  1 /string  false
        THEN
      ELSE chr-upper dup [char] E = swap [char] D = OR IF
          r@ scf.fexp scf.fdigit OR AND scf.fdigit <> IF
            true                  \ Second exponent or no digits -> done
          ELSE
            r> scf.fexp OR scf.fpoint OR scf.fsign INVERT AND >r 1 /string false
          THEN
        ELSE                      \ Everything else -> done
          true
        THEN
      THEN THEN THEN
    THEN
  UNTIL
  rdrop
  2swap 2over nip -
;


[DEFINED] >float [IF]
: scf+scan-float   ( c-addr1 u1 -- r c-addr2 u2 r scf.scanned | c-addr2 u2 scf.error = Scan the source string for a float )
  scf+skip-spaces
  scf+split-float
  >float IF
    scf.scanned
  ELSE
    scf.error
  THEN
;
[ELSE]
: scf+scan-float   ( c-addr1 u1 -- r c-addr2 u2 r scf.scanned | c-addr2 u2 scf.error = Scan the source string for a float )
  scf+skip-spaces
  scf+split-float
  2drop
  scf.error
;
[THEN]


: scf+scan-string  ( c-addr1 u1 -- c-addr2 u2 c-addr3 u3 scf.scanned | c-addr3 u3 scf.error = Scan the source string for a string c-addr2 u2 )
  dup IF
    2dup
    BEGIN
      dup IF 
        over c@ chr-space? 0=
      ELSE 
        false 
      THEN
    WHILE
      1 /string
    REPEAT
    tuck 2>r - 2r>
    scf.scanned
  ELSE
    scf.error
  THEN
;


: scf+scan-quoted ( c-addr1 u1 -- c-addr2 u2 c-addr3 u3 scf.scanned | c-addr1 u1 scf.error = Scan the source string for a quoted string c-addr2 u2 )
  dup IF
    over c@ [char] " = IF         \ Starts with quote ?
      1 /string                   \ Skip starting quote
      2dup
      false >r                    \ Escape
      BEGIN
        dup IF
          over c@ [char] " <> r@ OR \ Loop until non escaped quote
        ELSE
          false
        THEN
      WHILE
        r> IF
          false
        ELSE
          over c@ [char] \ =      \  Check for escape
        THEN
        >r
        1 /string
      REPEAT
      rdrop
      tuck
      dup IF
        1 /string                 \ Skip trailing quote
      THEN
      2>r - 2r>
      scf.scanned
    ELSE
      scf+scan-string
    THEN
  ELSE
    scf.error
  THEN
;


: scf+specifier   ( c-addr1 u1 c-addr2 u2 -- r|d|ud|n|u|c-addr u c-addr3 u3 c-addr4 u4 scf.matched | c-addr3 u3 c-addr4 u4 other = Process a specifier in the specifier string )
  dup IF
    over c@ [char] l = IF
      1 /string
      scf.double                  \ double number
    ELSE
      0
    THEN
    -rot 2>r
    r@ IF
      r'@ c@
      CASE
        [char] % OF drop      [char] % scf+match       ENDOF
        [char] d OF                 10 scf+scan-number ENDOF
        [char] u OF scf.unsigned OR 10 scf+scan-number ENDOF
        [char] x OF scf.unsigned OR 16 scf+scan-number ENDOF
        [char] X OF scf.unsigned OR 16 scf+scan-number ENDOF
        [char] o OF scf.unsigned OR  8 scf+scan-number ENDOF
        [char] s OF drop               scf+scan-string ENDOF
        [char] c OF drop               scf+scan-char   ENDOF
        [char] e OF drop               scf+scan-float  ENDOF
        [char] E OF drop               scf+scan-float  ENDOF
        [char] q OF drop               scf+scan-quoted ENDOF
        >r drop scf.error r>
      ENDCASE
    ELSE
      scf.error
    THEN
    2r> rot
  ELSE
    scf.error
  THEN
;


( sscanf words )

: scf+scan         ( c-addr1 u1 c-addr2 u2  -- i*x j*r n = Scan the source string c-addr1 u1 with specifier string c-addr2 u2, resulting in n arguments i*x j*r )
  0 >r                            \ argument count
  BEGIN
    dup                           \ format string
  WHILE
    2>r
    r'@ c@
    dup chr-space? IF
      drop scf+skip-spaces scf.scanned \ skip spaces if spaces in format string
    ELSE
      dup [char] % = IF
        drop
        2r>                       \ get the format string
        1 /string scf+specifier dup scf.scanned = IF  \ check modifier in format string
          r> 1+ >r                \ Argument scanned, increment argument counter
        THEN
        -rot 
        2>r
      ELSE
        scf+match
      THEN
    THEN
    
    2r> rot
    IF
      1 /string                   \ Argument matched or scanned, next character
    ELSE
      drop 0                      \ Error, done
    THEN
  REPEAT
  2drop 2drop
  r>
;


: scf"             ( "ccc<quote>" c-addr u -- i*x j*r n = Scan the source string c-addr1 u1 with the specifier string c-addr2 u2, resulting in n arguments i*x and j*r )
  [char] " parse
  state @ IF
    postpone     sliteral
    ['] scf+scan compile,
  ELSE
    scf+scan
  THEN
; immediate

[THEN]

\ ==============================================================================
