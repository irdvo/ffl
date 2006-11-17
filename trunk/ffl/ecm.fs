\ ==============================================================================
\
\       ecm - the ECMA-48 terminal escape sequences outputter in the ffl
\
\                Copyright (C) 2006  Dick van Oudheusden
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
\  $Date: 2006-11-17 20:30:16 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] ecm.version [IF]

( ecm = ECMA-48 Terminal Escape Sequence Outputter )
( The ecm module implements an ECMA-48 Terminal Escape Sequences Outputter.  )
( It outputs special escape sequences to the terminal in order to perform    )
( special terminal actions like colors, cursor movements, inserting and      )
( erasing lines. This module uses the escape sequences listed in the ECMA-48 )
( which is used by the linux console, the xterm, vt102 terminals, etc.       )
( Note: the module uses the pictured output buffer.                          )


1 constant ecm.version



( Public words )

( Attributes )

0  constant ecm.reset                 ( - u = Reset attributes to defaults )
1  constant ecm.bold                  ( - u = Set bold )
2  constant ecm.half-bright           ( - u = Set half bright )
4  constant ecm.underscore-on         ( - u = Set underscore )
5  constant ecm.blink-on              ( - u = Set blink )
7  constant ecm.reverse-on            ( - u = Set reverse video )
22 constant ecm.normal-intensity      ( - u = Set normal intensity )
24 constant ecm.underline-off         ( - u = Reset underline )
25 constant ecm.blink-off             ( - u = Reset blink )
27 constant ecm.reverse-off           ( - u = Reset reverse )
30 constant ecm.foreground-black      ( - u = Set black foreground )
31 constant ecm.foreground-red        ( - u = Set red foreground )
32 constant ecm.foreground-green      ( - u = Set green foreground )
33 constant ecm.foreground-brown      ( - u = Set brown foreground )
34 constant ecm.foreground-blue       ( - u = Set blue foreground )
35 constant ecm.foreground-magenta    ( - u = Set magenta foreground )
36 constant ecm.foreground-cyan       ( - u = Set cyan foreground )
37 constant ecm.foreground-white      ( - u = Set white foreground )
38 constant ecm.foreground-default-underscore ( - u = Set default foreground with underscore on )
39 constant ecm.foreground-default    ( - u = Set default foreground )
40 constant ecm.background-black      ( - u = Set black background )
41 constant ecm.background-red        ( - u = Set red background )
42 constant ecm.background-green      ( - u = Set green background )
43 constant ecm.background-brown      ( - u = Set brown background )
44 constant ecm.background-blue       ( - u = Set blue background )
45 constant ecm.background-magenta    ( - u = Set magenta background )
46 constant ecm.background-cyan       ( - u = Set cyan background )
47 constant ecm.background-white      ( - u = Set white background )
49 constant ecm.background-default    ( - u = Set default background )


( Private words )

: ecm+do-esc1      ( c - = Output a single escape sequence )
  27 emit
  emit
;

: ecm+do-esc2      ( c c - = Output a double escape sequence )
  27 emit
  emit
  emit
;


: ecm+do-csin      ( u1 .. un n c - = Output a CSI escape sequence with multiple fields )
  base @ >r decimal
  >r
  27 emit
  [char] [ emit
  BEGIN
    ?dup
  WHILE
    1- swap
    0 u.r
    dup IF
      [char] ; emit
    THEN
  REPEAT
  r> emit
  r> base !
;


: ecm+do-csi0      ( c - = Output a CSI escape sequence with no number fields )
  27 emit
  [char] [ emit
  emit
;  


: ecm+do-csi1      ( u c - = Output a CSI escape sequence with one number field )
  1 swap ecm+do-csin
;  


( Terminal words )

: ecm+reset        ( - = Reset the terminal )
  [char] c ecm+do-esc1
;


: ecm+set-tab-stop ( - = Set tab stop at current column )
  [char] H ecm+do-esc1
;


: ecm+save-current-state  ( - = Save the current state: cursor, attributes and character sets )
  [char] 7 ecm+do-esc1
;


: ecm+restore-current-state  ( - = Restore the current state: cursor, attributes and character sets )
  [char] 8 ecm+do-esc1
;


: ecm+set-scroll-region  ( u:btm u:top - = Set the scroll region rows )
  2 [char] r ecm+do-csin
;



( Cursor words )

: ecm+move-cursor-up ( u - = Move cursor up u rows )
  [char] A ecm+do-csi1
;


: ecm+move-cursor-down ( u - = Move cursor down u rows )
  [char] B ecm+do-csi1
;


: ecm+move-cursor-right ( u - = Move cursor right u columns )
  [char] C ecm+do-csi1
;


: ecm+move-cursor-left ( u - = Move cursor left u columns )
  [char] D ecm+do-csi1
;


: ecm+move-cursor-down-col1 ( u - = Move cursor down u rows on column 1 )
  [char] E ecm+do-csi1
;


: ecm+move-cursor-up-col1  ( u - = Move cursor up u rows on column 1 )
  [char] F ecm+do-csi1
;


: ecm+move-cursor  ( u:x u:y - = Move cursor to column and row )
  2 [char] H ecm+do-csin
;

: ecm+save-cursor  ( - = Save cursor location )
  [char] s ecm+do-csi0
;


: ecm+restore-cursor  ( - = Restore cursor location )
  [char] u ecm+do-csi0
;


( Erase display words )

: ecm+erase-display-from-cursor  ( - = Erase display from cursor to end )
  [char] J ecm+do-csi0
;


: ecm+erase-display-from-start   ( - = Erase from start display to cursor )
  1 [char] J ecm+do-csi1
;


: ecm+erase-display  ( - = Erase the whole display )
  2 [char] J ecm+do-csi1
;


( Erase line words )

: ecm+erase-line-from-cursor  ( - = Erase the line from cursor to end of line )
  [char] K ecm+do-csi0
;


: ecm+erase-line-from-start  ( - = Erase the line from start line to cursor )
  1 [char] K ecm+do-csi1
;


: ecm+erase-line   ( - = Erase the whole line )
  2 [char] K ecm+do-csi1
;


( Insert and delete lines words )

: ecm+insert-lines  ( u - = Insert u blank lines )
  [char] L ecm+do-csi1
;


: ecm+delete-lines  ( u - = Delete u lines )
  [char] M ecm+do-csi1
;


( Character words )

: ecm+insert-spaces  ( u - = Insert u spaces )
  [char] @ ecm+do-csi1
;


: ecm+delete-chars  ( u - = Delete n characters on the current line )
  [char] P ecm+do-csi1
;


: ecm+erase-chars  ( u - = Erase u characters on the current line )
  [char] X ecm+do-csi1
;


( Attribute words )

: ecm+set-attributes  ( u1 .. un n - = Set n attributes )
  [char] m ecm+do-csin
;


( LED words )

: ecm+clear-all-leds  ( - = Clear all LEDs )
  0 [char] q ecm+do-csi1
;


: ecm+set-scroll-led  ( - = Set the scroll lock LED )
  1 [char] q ecm+do-csi1
;


: ecm+set-num-led  ( - = Set the num lock LED )
  2 [char] q ecm+do-csi1
;


: ecm+set-caps-led  ( - = Set the caps lock LED )
  3 [char] q ecm+do-csi1
;






[THEN]

\ ==============================================================================
